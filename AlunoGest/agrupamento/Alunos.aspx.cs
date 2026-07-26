using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Web.Security;
using AlunoGest.Util;

namespace AlunoGest.agrupamento
{
    public partial class Alunos : System.Web.UI.Page
    {
        #region Campos

        private readonly string _connectionString =
            ConfigurationManager
                .ConnectionStrings["DefaultConnection"]
                .ConnectionString;

        #endregion


        #region Página

        protected void Page_Load(
            object sender,
            EventArgs e)
        {
            int agrupamentoId;

            if (!TryGetAgrupamentoId(out agrupamentoId))
            {
                Response.Redirect("~/login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                Controlos.Visible = false;

                LimparMensagem();
                GetAlunos();
                GetAlunosSemEncarregado();
            }
        }

        #endregion


        #region Eventos dos botões

        protected void ButtonCriar_Click(
            object sender,
            EventArgs e)
        {
            LimparMensagem();
            LimparFormulario();

            GridAlunos.SelectedIndex = -1;

            Controlos.Visible = true;
            ViewState["Op"] = "criar";

            ChkAtivo.Checked = true;
        }

        protected void ButtonEditar_Click(
            object sender,
            EventArgs e)
        {
            LimparMensagem();

            int idAluno;

            if (!AlunoSelecionado(out idAluno))
            {
                MostrarMensagem(
                    "Selecione um aluno."
                );

                return;
            }

            AlunoDados aluno =
                GetAlunoById(idAluno);

            if (aluno == null)
            {
                MostrarMensagem(
                    "Não foi possível encontrar o aluno selecionado."
                );

                GetAlunos();
                return;
            }

            CarregarFormulario(aluno);

            Controlos.Visible = true;
            ViewState["Op"] = "editar";
            ViewState["AlunoId"] = idAluno;
        }

        protected void ButtonReenviarCredenciais_Click(
            object sender,
            EventArgs e)
        {
            LimparMensagem();

            int idAluno;

            if (!AlunoSelecionado(out idAluno))
            {
                MostrarMensagem(
                    "Selecione um aluno."
                );

                return;
            }

            AlunoDados aluno =
                GetAlunoById(idAluno);

            if (aluno == null)
            {
                MostrarMensagem(
                    "Não foi possível encontrar o aluno selecionado."
                );

                GetAlunos();
                return;
            }

            if (!aluno.Ativo)
            {
                MostrarMensagem(
                    "O aluno selecionado está inativo. " +
                    "Ative a conta antes de reenviar as credenciais."
                );

                return;
            }

            if (string.IsNullOrWhiteSpace(aluno.Email))
            {
                MostrarMensagem(
                    "O aluno não possui um email válido. " +
                    "Edite o registo antes de reenviar as credenciais."
                );

                return;
            }

            bool passwordRedefinida =
                false;

            try
            {
                MembershipUser utilizador =
                    Membership.GetUser(
                        aluno.UserId,
                        false
                    );

                if (utilizador == null)
                {
                    throw new InvalidOperationException(
                        "Não foi possível encontrar a conta de acesso " +
                        "associada ao aluno."
                    );
                }

                string emailAtual =
                    aluno.Email
                        .Trim()
                        .ToLowerInvariant();

                if (EmailJaExisteNoMembership(
                        emailAtual,
                        aluno.UserId))
                {
                    throw new InvalidOperationException(
                        "O email atual já está associado a outra conta."
                    );
                }

                utilizador.Email =
                    emailAtual;

                utilizador.IsApproved =
                    true;

                Membership.UpdateUser(
                    utilizador
                );

                string novaPassword =
                    CriarConta.RedefinirPassword(
                        utilizador.UserName
                    );

                passwordRedefinida =
                    true;

                string urlLogin =
                    ObterUrlLogin();

                CriarConta
                    .EnviarEmailCredenciaisRedefinidas(
                        emailAtual,
                        aluno.NomeCompleto,
                        utilizador.UserName,
                        novaPassword,
                        urlLogin
                    );

                MostrarMensagem(
                    "Foi gerada uma nova palavra-passe e as credenciais " +
                    "foram enviadas para " + emailAtual + ".",
                    false
                );
            }
            catch (Exception ex)
            {
               System.Diagnostics.Trace.TraceError(
                    "Erro ao reenviar credenciais do aluno: " +
                    ex
                );

                if (passwordRedefinida)
                {
                    MostrarMensagem(
                        "A palavra-passe foi redefinida, mas não foi possível " +
                        "enviar o email. Confirme o endereço e a configuração " +
                        "SMTP e volte a clicar em Reenviar credenciais. " +
                        ex.Message
                    );
                }
                else
                {
                    MostrarMensagem(
                        "Não foi possível reenviar as credenciais. " +
                        ex.Message
                    );
                }
            }
        }

        protected void ButtonGuardar_Click(
            object sender,
            EventArgs e)
        {
            LimparMensagem();

            Page.Validate("aluno");

            if (!Page.IsValid)
            {
                Controlos.Visible = true;
                return;
            }

            int agrupamentoId;

            if (!TryGetAgrupamentoId(out agrupamentoId))
            {
                Response.Redirect("~/login.aspx");
                return;
            }

            string operacao =
                Convert.ToString(ViewState["Op"]);

            string nomeCompleto =
                TxtNomeCompleto.Text.Trim();

            string numeroProcesso =
                TxtNumeroProcesso.Text.Trim();

            string email =
                TxtEmail.Text
                    .Trim()
                    .ToLowerInvariant();

            string telefone =
                TxtTelefone.Text.Trim();

            string nif =
                ValidadorNif.Normalizar(
                    TxtNIF.Text
                );

            string mensagemNif;

            if (!ValidadorNif.ValidarLocalmente(
                    nif,
                    out mensagemNif))
            {
                MostrarMensagem(mensagemNif);
                Controlos.Visible = true;
                return;
            }

            if (operacao == "criar")
            {
                CriarNovoAluno(
                    agrupamentoId,
                    nomeCompleto,
                    numeroProcesso,
                    email,
                    telefone,
                    nif
                );

                return;
            }

            if (operacao == "editar")
            {
                AtualizarAlunoExistente(
                    agrupamentoId,
                    nomeCompleto,
                    numeroProcesso,
                    email,
                    telefone,
                    nif
                );

                return;
            }

            MostrarMensagem(
                "Operação inválida."
            );
        }

        protected void ButtonCancelar_Click(
            object sender,
            EventArgs e)
        {
            LimparFormulario();
            LimparMensagem();

            GridAlunos.SelectedIndex = -1;

            ViewState["Op"] = null;
            ViewState["AlunoId"] = null;
            Controlos.Visible = false;
        }

        #endregion


        #region Criar aluno

        private void CriarNovoAluno(
            int agrupamentoId,
            string nomeCompleto,
            string numeroProcesso,
            string email,
            string telefone,
            string nif)
        {
            if (NifJaExiste(nif, null))
            {
                MostrarMensagem(
                    "Já existe um aluno, professor ou encarregado " +
                    "de educação com este NIF."
                );

                Controlos.Visible = true;
                return;
            }

            if (EmailJaExisteNaTabela(email, null))
            {
                MostrarMensagem(
                    "Já existe um aluno, professor ou encarregado " +
                    "de educação com este email."
                );

                Controlos.Visible = true;
                return;
            }

            if (EmailJaExisteNoMembership(email, null))
            {
                MostrarMensagem(
                    "Já existe uma conta de utilizador associada " +
                    "a este email."
                );

                Controlos.Visible = true;
                return;
            }

            string username =
                null;

            string password =
                null;

            Guid userId =
                Guid.Empty;

            try
            {
                userId =
                    CriarContaAluno(
                        nomeCompleto,
                        email,
                        ChkAtivo.Checked,
                        out username,
                        out password
                    );

                int linhas =
                    InsertAluno(
                        userId,
                        agrupamentoId,
                        nomeCompleto,
                        numeroProcesso,
                        email,
                        telefone,
                        nif,
                        ChkAtivo.Checked
                    );

                if (linhas != 1)
                {
                    throw new InvalidOperationException(
                        "O registo do aluno não foi criado."
                    );
                }
            }
            catch (MembershipCreateUserException ex)
            {
                RemoverContaCriada(username);

                MostrarMensagem(
                    "Não foi possível criar a conta do aluno: " +
                    ex.Message
                );

                Controlos.Visible = true;
                return;
            }
            catch (SqlException ex)
            {
                RemoverContaCriada(username);

                MostrarMensagem(
                    "Erro na base de dados ao criar o aluno: " +
                    ex.Message
                );

                Controlos.Visible = true;
                return;
            }
            catch (Exception ex)
            {
                RemoverContaCriada(username);

                MostrarMensagem(
                    "Não foi possível criar o aluno: " +
                    ex.Message
                );

                Controlos.Visible = true;
                return;
            }

            bool emailEnviado =
                EnviarCredenciaisIniciais(
                    email,
                    nomeCompleto,
                    username,
                    password
                );

            FinalizarOperacaoComSucesso();

            if (emailEnviado)
            {
                MostrarMensagem(
                    "Aluno criado com sucesso. " +
                    "As credenciais foram enviadas por email.",
                    false
                );
            }
            else
            {
                MostrarMensagem(
                    "O aluno e a respetiva conta foram criados, " +
                    "mas não foi possível enviar o email com as credenciais."
                );
            }
        }

        #endregion


        #region Atualizar aluno

        private void AtualizarAlunoExistente(
            int agrupamentoId,
            string nomeCompleto,
            string numeroProcesso,
            string email,
            string telefone,
            string nif)
        {
            int idAluno;

            if (!TryGetAlunoIdViewState(out idAluno))
            {
                MostrarMensagem(
                    "Não foi possível identificar o aluno."
                );

                return;
            }

            AlunoDados dadosAtuais =
                GetAlunoById(idAluno);

            if (dadosAtuais == null)
            {
                MostrarMensagem(
                    "O aluno já não está disponível."
                );

                GetAlunos();
                return;
            }

            if (NifJaExiste(nif, idAluno))
            {
                MostrarMensagem(
                    "Já existe outro aluno, professor ou encarregado " +
                    "de educação com este NIF."
                );

                Controlos.Visible = true;
                return;
            }

            if (EmailJaExisteNaTabela(email, idAluno))
            {
                MostrarMensagem(
                    "Já existe outro aluno, professor ou encarregado " +
                    "de educação com este email."
                );

                Controlos.Visible = true;
                return;
            }

            if (EmailJaExisteNoMembership(
                    email,
                    dadosAtuais.UserId))
            {
                MostrarMensagem(
                    "Já existe outra conta associada a este email."
                );

                Controlos.Visible = true;
                return;
            }

            try
            {
                AtualizarAlunoEMembership(
                    dadosAtuais,
                    agrupamentoId,
                    nomeCompleto,
                    numeroProcesso,
                    email,
                    telefone,
                    nif,
                    ChkAtivo.Checked
                );
            }
            catch (SqlException ex)
            {
                MostrarMensagem(
                    "Erro na base de dados ao atualizar o aluno: " +
                    ex.Message
                );

                Controlos.Visible = true;
                return;
            }
            catch (Exception ex)
            {
                MostrarMensagem(
                    "Não foi possível atualizar o aluno: " +
                    ex.Message
                );

                Controlos.Visible = true;
                return;
            }

            FinalizarOperacaoComSucesso();

            MostrarMensagem(
                "Aluno atualizado com sucesso.",
                false
            );
        }

        private void AtualizarAlunoEMembership(
            AlunoDados dadosAtuais,
            int agrupamentoId,
            string nomeCompleto,
            string numeroProcesso,
            string email,
            string telefone,
            string nif,
            bool ativo)
        {
            MembershipUser utilizador =
                Membership.GetUser(
                    dadosAtuais.UserId,
                    false
                );

            if (utilizador == null)
            {
                throw new InvalidOperationException(
                    "Não foi possível encontrar a conta do aluno."
                );
            }

            string emailAnterior =
                utilizador.Email;

            bool estadoAnterior =
                utilizador.IsApproved;

            utilizador.Email =
                email;

            utilizador.IsApproved =
                ativo;

            Membership.UpdateUser(
                utilizador
            );

            try
            {
                int linhas =
                    UpdateAluno(
                        dadosAtuais.Id,
                        agrupamentoId,
                        nomeCompleto,
                        numeroProcesso,
                        email,
                        telefone,
                        nif,
                        ativo
                    );

                if (linhas != 1)
                {
                    throw new InvalidOperationException(
                        "O registo do aluno não foi atualizado."
                    );
                }
            }
            catch
            {
                try
                {
                    utilizador.Email =
                        emailAnterior;

                    utilizador.IsApproved =
                        estadoAnterior;

                    Membership.UpdateUser(
                        utilizador
                    );
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.TraceError(
                        "Erro ao repor os dados do Membership do aluno: " +
                        ex
                    );
                }

                throw;
            }
        }

        #endregion


        #region Listagem e leitura

        private void GetAlunos()
        {
            int agrupamentoId;

            if (!TryGetAgrupamentoId(out agrupamentoId))
            {
                Response.Redirect("~/login.aspx");
                return;
            }

            DataTable tabela =
                new DataTable();

            const string sql = @"
                SELECT
                    Id,
                    NomeCompleto,
                    NumeroProcesso,
                    NIF,
                    Email,
                    Telefone,
                    Ativo

                FROM dbo.Aluno

                WHERE AgrupamentoId = @AgrupamentoId

                ORDER BY NomeCompleto;";

            using (SqlConnection conn =
                new SqlConnection(_connectionString))
            using (SqlCommand cmd =
                new SqlCommand(sql, conn))
            using (SqlDataAdapter adapter =
                new SqlDataAdapter(cmd))
            {
                cmd.Parameters
                    .Add(
                        "@AgrupamentoId",
                        SqlDbType.Int
                    )
                    .Value = agrupamentoId;

                adapter.Fill(tabela);
            }

            GridAlunos.DataSource =
                tabela;

            GridAlunos.DataBind();
        }

        private AlunoDados GetAlunoById(
            int idAluno)
        {
            int agrupamentoId;

            if (!TryGetAgrupamentoId(out agrupamentoId))
            {
                return null;
            }

            const string sql = @"
                SELECT TOP 1
                    Id,
                    UserId,
                    NomeCompleto,
                    NumeroProcesso,
                    NIF,
                    Email,
                    Telefone,
                    Ativo

                FROM dbo.Aluno

                WHERE Id = @Id
                  AND AgrupamentoId = @AgrupamentoId;";

            using (SqlConnection conn =
                new SqlConnection(_connectionString))
            using (SqlCommand cmd =
                new SqlCommand(sql, conn))
            {
                cmd.Parameters
                    .Add(
                        "@Id",
                        SqlDbType.Int
                    )
                    .Value = idAluno;

                cmd.Parameters
                    .Add(
                        "@AgrupamentoId",
                        SqlDbType.Int
                    )
                    .Value = agrupamentoId;

                conn.Open();

                using (SqlDataReader reader =
                    cmd.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        return null;
                    }

                    return new AlunoDados
                    {
                        Id =
                            Convert.ToInt32(
                                reader["Id"]
                            ),

                        UserId =
                            (Guid)reader["UserId"],

                        NomeCompleto =
                            Convert.ToString(
                                reader["NomeCompleto"]
                            ),

                        NumeroProcesso =
                            ValorTexto(
                                reader["NumeroProcesso"]
                            ),

                        NIF =
                            ValorTexto(
                                reader["NIF"]
                            ),

                        Email =
                            ValorTexto(
                                reader["Email"]
                            ),

                        Telefone =
                            ValorTexto(
                                reader["Telefone"]
                            ),

                        Ativo =
                            Convert.ToBoolean(
                                reader["Ativo"]
                            )
                    };
                }
            }
        }

        private void CarregarFormulario(
            AlunoDados aluno)
        {
            TxtNomeCompleto.Text =
                aluno.NomeCompleto;

            TxtNumeroProcesso.Text =
                aluno.NumeroProcesso;

            TxtNIF.Text =
                aluno.NIF;

            TxtEmail.Text =
                aluno.Email;

            TxtTelefone.Text =
                aluno.Telefone;

            ChkAtivo.Checked =
                aluno.Ativo;
        }

        #endregion


        #region Inserção e atualização

        private int InsertAluno(
            Guid userId,
            int agrupamentoId,
            string nomeCompleto,
            string numeroProcesso,
            string email,
            string telefone,
            string nif,
            bool ativo)
        {
            const string sql = @"
                INSERT INTO dbo.Aluno
                (
                    AgrupamentoId,
                    UserId,
                    NomeCompleto,
                    NumeroProcesso,
                    NIF,
                    Email,
                    Telefone,
                    Ativo,
                    CreatedAt
                )
                VALUES
                (
                    @AgrupamentoId,
                    @UserId,
                    @NomeCompleto,
                    @NumeroProcesso,
                    @NIF,
                    @Email,
                    @Telefone,
                    @Ativo,
                    SYSDATETIME()
                );";

            using (SqlConnection conn =
                new SqlConnection(_connectionString))
            using (SqlCommand cmd =
                new SqlCommand(sql, conn))
            {
                AdicionarParametrosAluno(
                    cmd,
                    agrupamentoId,
                    nomeCompleto,
                    numeroProcesso,
                    email,
                    telefone,
                    nif,
                    ativo
                );

                cmd.Parameters
                    .Add(
                        "@UserId",
                        SqlDbType.UniqueIdentifier
                    )
                    .Value = userId;

                conn.Open();

                return cmd.ExecuteNonQuery();
            }
        }

        private int UpdateAluno(
            int idAluno,
            int agrupamentoId,
            string nomeCompleto,
            string numeroProcesso,
            string email,
            string telefone,
            string nif,
            bool ativo)
        {
            const string sql = @"
                UPDATE dbo.Aluno

                SET
                    NomeCompleto = @NomeCompleto,
                    NumeroProcesso = @NumeroProcesso,
                    NIF = @NIF,
                    Email = @Email,
                    Telefone = @Telefone,
                    Ativo = @Ativo

                WHERE Id = @Id
                  AND AgrupamentoId = @AgrupamentoId;";

            using (SqlConnection conn =
                new SqlConnection(_connectionString))
            using (SqlCommand cmd =
                new SqlCommand(sql, conn))
            {
                AdicionarParametrosAluno(
                    cmd,
                    agrupamentoId,
                    nomeCompleto,
                    numeroProcesso,
                    email,
                    telefone,
                    nif,
                    ativo
                );

                cmd.Parameters
                    .Add(
                        "@Id",
                        SqlDbType.Int
                    )
                    .Value = idAluno;

                conn.Open();

                return cmd.ExecuteNonQuery();
            }
        }

        private void AdicionarParametrosAluno(
            SqlCommand cmd,
            int agrupamentoId,
            string nomeCompleto,
            string numeroProcesso,
            string email,
            string telefone,
            string nif,
            bool ativo)
        {
            cmd.Parameters
                .Add(
                    "@AgrupamentoId",
                    SqlDbType.Int
                )
                .Value = agrupamentoId;

            cmd.Parameters
                .Add(
                    "@NomeCompleto",
                    SqlDbType.NVarChar,
                    200
                )
                .Value = nomeCompleto;

            cmd.Parameters
                .Add(
                    "@NumeroProcesso",
                    SqlDbType.NVarChar,
                    50
                )
                .Value = numeroProcesso;

            cmd.Parameters
                .Add(
                    "@NIF",
                    SqlDbType.NVarChar,
                    9
                )
                .Value = nif;

            cmd.Parameters
                .Add(
                    "@Email",
                    SqlDbType.NVarChar,
                    150
                )
                .Value = email;

            cmd.Parameters
                .Add(
                    "@Telefone",
                    SqlDbType.NVarChar,
                    20
                )
                .Value = telefone;

            cmd.Parameters
                .Add(
                    "@Ativo",
                    SqlDbType.Bit
                )
                .Value = ativo;
        }

        #endregion


        #region Validações de duplicação

        private bool NifJaExiste(
            string nif,
            int? alunoIdIgnorar)
        {
            const string sql = @"
                SELECT
                    CASE
                        WHEN EXISTS
                        (
                            SELECT 1

                            FROM dbo.Aluno

                            WHERE NIF = @NIF
                              AND
                              (
                                  @AlunoIdIgnorar IS NULL
                                  OR Id <> @AlunoIdIgnorar
                              )
                        )

                        OR EXISTS
                        (
                            SELECT 1

                            FROM dbo.Professor

                            WHERE NIF = @NIF
                        )

                        OR EXISTS
                        (
                            SELECT 1

                            FROM dbo.EncarregadoEducacao

                            WHERE NIF = @NIF
                        )

                        THEN 1
                        ELSE 0
                    END;";

            using (SqlConnection conn =
                new SqlConnection(_connectionString))
            using (SqlCommand cmd =
                new SqlCommand(sql, conn))
            {
                cmd.Parameters
                    .Add(
                        "@NIF",
                        SqlDbType.NVarChar,
                        9
                    )
                    .Value = nif;

                cmd.Parameters
                    .Add(
                        "@AlunoIdIgnorar",
                        SqlDbType.Int
                    )
                    .Value =
                    alunoIdIgnorar.HasValue
                        ? (object)alunoIdIgnorar.Value
                        : DBNull.Value;

                conn.Open();

                return Convert.ToInt32(
                    cmd.ExecuteScalar()
                ) == 1;
            }
        }

        private bool EmailJaExisteNaTabela(
            string email,
            int? alunoIdIgnorar)
        {
            const string sql = @"
                SELECT
                    CASE
                        WHEN EXISTS
                        (
                            SELECT 1

                            FROM dbo.Aluno

                            WHERE Email = @Email
                              AND
                              (
                                  @AlunoIdIgnorar IS NULL
                                  OR Id <> @AlunoIdIgnorar
                              )
                        )

                        OR EXISTS
                        (
                            SELECT 1

                            FROM dbo.Professor

                            WHERE Email = @Email
                        )

                        OR EXISTS
                        (
                            SELECT 1

                            FROM dbo.EncarregadoEducacao

                            WHERE Email = @Email
                        )

                        THEN 1
                        ELSE 0
                    END;";

            using (SqlConnection conn =
                new SqlConnection(_connectionString))
            using (SqlCommand cmd =
                new SqlCommand(sql, conn))
            {
                cmd.Parameters
                    .Add(
                        "@Email",
                        SqlDbType.NVarChar,
                        150
                    )
                    .Value = email;

                cmd.Parameters
                    .Add(
                        "@AlunoIdIgnorar",
                        SqlDbType.Int
                    )
                    .Value =
                    alunoIdIgnorar.HasValue
                        ? (object)alunoIdIgnorar.Value
                        : DBNull.Value;

                conn.Open();

                return Convert.ToInt32(
                    cmd.ExecuteScalar()
                ) == 1;
            }
        }

        private bool EmailJaExisteNoMembership(
            string email,
            Guid? userIdIgnorar)
        {
            string username =
                Membership.GetUserNameByEmail(
                    email
                );

            if (string.IsNullOrWhiteSpace(username))
            {
                return false;
            }

            if (!userIdIgnorar.HasValue)
            {
                return true;
            }

            MembershipUser utilizador =
                Membership.GetUser(
                    username,
                    false
                );

            if (utilizador == null ||
                utilizador.ProviderUserKey == null)
            {
                return true;
            }

            Guid userIdEncontrado;

            if (!Guid.TryParse(
                    utilizador.ProviderUserKey.ToString(),
                    out userIdEncontrado))
            {
                return true;
            }

            return userIdEncontrado !=
                   userIdIgnorar.Value;
        }

        #endregion


        #region Membership

        private Guid CriarContaAluno(
            string nomeCompleto,
            string email,
            bool ativo,
            out string username,
            out string password)
        {
            username =
                null;

            password =
                null;

            MembershipUser utilizadorCriado =
                null;

            string usernameBase =
                CriarConta.GerarUsername(
                    nomeCompleto
                );

            username =
                CriarConta.GarantirUsernameUnico(
                    usernameBase
                );

            password =
                CriarConta.GerarPassword();

            try
            {
                utilizadorCriado =
                    Membership.CreateUser(
                        username,
                        password,
                        email
                    );

                if (utilizadorCriado == null)
                {
                    throw new InvalidOperationException(
                        "O Membership não devolveu o utilizador criado."
                    );
                }

                Roles.AddUserToRole(
                    username,
                    "Aluno"
                );

                utilizadorCriado.IsApproved =
                    ativo;

                Membership.UpdateUser(
                    utilizadorCriado
                );

                return (Guid)
                    utilizadorCriado.ProviderUserKey;
            }
            catch
            {
                RemoverContaCriada(username);
                throw;
            }
        }

        private bool EnviarCredenciaisIniciais(
            string email,
            string nomeCompleto,
            string username,
            string password)
        {
            try
            {
                CriarConta.EnviarEmailCredenciais(
                    email,
                    nomeCompleto,
                    username,
                    password,
                    ObterUrlLogin()
                );

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError(
                    "Erro ao enviar credenciais do aluno: " +
                    ex
                );

                return false;
            }
        }

        private void RemoverContaCriada(
            string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return;
            }

            try
            {
                if (Roles.IsUserInRole(
                        username,
                        "Aluno"))
                {
                    Roles.RemoveUserFromRole(
                        username,
                        "Aluno"
                    );
                }

                Membership.DeleteUser(
                    username,
                    true
                );
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError(
                    "Erro ao remover a conta incompleta do aluno: " +
                    ex
                );
            }
        }

        #endregion


        #region Agrupamento e sessão

        private bool TryGetAgrupamentoId(
            out int agrupamentoId)
        {
            agrupamentoId = 0;

            if (Session["AgrupamentoID"] != null &&
                int.TryParse(
                    Session["AgrupamentoID"].ToString(),
                    out agrupamentoId))
            {
                return true;
            }

            if (Session["UserId"] == null)
            {
                return false;
            }

            Guid userId;

            if (!Guid.TryParse(
                    Session["UserId"].ToString(),
                    out userId))
            {
                return false;
            }

            const string sql = @"
                SELECT Id

                FROM dbo.Agrupamento

                WHERE UserId = @UserId
                  AND Ativo = 1;";

            using (SqlConnection conn =
                new SqlConnection(_connectionString))
            using (SqlCommand cmd =
                new SqlCommand(sql, conn))
            {
                cmd.Parameters
                    .Add(
                        "@UserId",
                        SqlDbType.UniqueIdentifier
                    )
                    .Value = userId;

                conn.Open();

                object resultado =
                    cmd.ExecuteScalar();

                if (resultado == null ||
                    resultado == DBNull.Value)
                {
                    return false;
                }

                agrupamentoId =
                    Convert.ToInt32(resultado);

                Session["AgrupamentoID"] =
                    agrupamentoId;

                return true;
            }
        }

        #endregion


        #region Utilidades

        private bool AlunoSelecionado(
            out int idAluno)
        {
            idAluno = 0;

            if (GridAlunos.SelectedDataKey == null ||
                GridAlunos.SelectedDataKey.Value == null)
            {
                return false;
            }

            return int.TryParse(
                GridAlunos.SelectedDataKey
                    .Value
                    .ToString(),
                out idAluno
            );
        }

        private bool TryGetAlunoIdViewState(
            out int idAluno)
        {
            idAluno = 0;

            if (ViewState["AlunoId"] == null)
            {
                return false;
            }

            return int.TryParse(
                ViewState["AlunoId"].ToString(),
                out idAluno
            );
        }

        private string ObterUrlLogin()
        {
            return Request.Url.GetLeftPart(
                       UriPartial.Authority
                   ) +
                   ResolveUrl("~/login.aspx");
        }

        private void FinalizarOperacaoComSucesso()
        {
            GetAlunos();
            GetAlunosSemEncarregado();

            LimparFormulario();

            GridAlunos.SelectedIndex = -1;

            ViewState["Op"] = null;
            ViewState["AlunoId"] = null;
            Controlos.Visible = false;
        }

        private void LimparFormulario()
        {
            TxtNomeCompleto.Text = string.Empty;
            TxtNumeroProcesso.Text = string.Empty;
            TxtNIF.Text = string.Empty;
            TxtEmail.Text = string.Empty;
            TxtTelefone.Text = string.Empty;

            ChkAtivo.Checked = true;
        }

        private string ValorTexto(
            object valor)
        {
            return valor == null ||
                   valor == DBNull.Value
                ? string.Empty
                : valor.ToString();
        }

        private void MostrarMensagem(
            string mensagem,
            bool erro = true)
        {
            LblMensagem.Visible = true;
            LblMensagem.Text = mensagem;

            LblMensagem.CssClass =
                erro
                    ? "alert alert-warning d-block"
                    : "alert alert-success d-block";
        }

        private void LimparMensagem()
        {
            LblMensagem.Visible = false;
            LblMensagem.Text = string.Empty;
            LblMensagem.CssClass = string.Empty;
        }

        #endregion


        #region Listagem e leitura dos alunos sem encarregado

        private void GetAlunosSemEncarregado()
        {
            int agrupamentoId;

            if (!TryGetAgrupamentoId(out agrupamentoId))
            {
                Response.Redirect("~/login.aspx");
                return;
            }

            DataTable tabela =
                new DataTable();

            const string sql = @"
                SELECT
                    aluno.Id,
                    aluno.NomeCompleto,
                    aluno.NumeroProcesso,
                    aluno.NIF,
                    aluno.Email,
                    aluno.Telefone,
                    aluno.Ativo

                FROM dbo.Aluno aluno

                WHERE aluno.AgrupamentoId = @AgrupamentoId

                  AND NOT EXISTS
                  (
                      SELECT 1

                      FROM dbo.AlunoEncarregado alunoEncarregado

                      INNER JOIN dbo.EncarregadoEducacao encarregado
                          ON encarregado.Id =
                             alunoEncarregado.EncarregadoEducacaoId

                      WHERE alunoEncarregado.AlunoId = aluno.Id
                        AND alunoEncarregado.Ativo = 1
                        AND encarregado.Ativo = 1
                  )

                ORDER BY aluno.NomeCompleto;";

            using (SqlConnection conn =
                new SqlConnection(_connectionString))
            using (SqlCommand cmd =
                new SqlCommand(sql, conn))
            using (SqlDataAdapter adapter =
                new SqlDataAdapter(cmd))
            {
                cmd.Parameters
                    .Add(
                        "@AgrupamentoId",
                        SqlDbType.Int
                    )
                    .Value = agrupamentoId;

                adapter.Fill(tabela);
            }

            GridAlunosSemEncarregado.DataSource =
                tabela;

            GridAlunosSemEncarregado.DataBind();

            LblTotalAlunosSemEncarregado.Text =
                tabela.Rows.Count == 1
                    ? "1 aluno"
                    : tabela.Rows.Count +
                      " alunos";
        }

        #endregion


        #region Classe auxiliar

        private class AlunoDados
        {
            public int Id { get; set; }

            public Guid UserId { get; set; }

            public string NomeCompleto { get; set; }

            public string NumeroProcesso { get; set; }

            public string NIF { get; set; }

            public string Email { get; set; }

            public string Telefone { get; set; }

            public bool Ativo { get; set; }
        }

        #endregion
    }
}