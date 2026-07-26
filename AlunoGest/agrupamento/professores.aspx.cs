using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using AlunoGest.Util;

namespace AlunoGest.agrupamento
{
    public partial class professores : Page
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
            if (!Request.IsAuthenticated ||
                !Roles.IsUserInRole(
                    User.Identity.Name,
                    "agrupamento"))
            {
                FormsAuthentication.SignOut();
                Session.Clear();

                Response.Redirect("~/login.aspx");
                return;
            }

            try
            {
                GetAgrupamentoIdFromSession();
            }
            catch
            {
                FormsAuthentication.SignOut();
                Session.Clear();

                Response.Redirect("~/login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                CarregarGrupoRecrutamento();
                GetProfessores();

                controlos.Visible = false;
                painelDisciplinasProfessor.Visible = false;
            }
        }

        #endregion


        #region Botões principais

        protected void buttonVer_Click(
            object sender,
            EventArgs e)
        {
            int idProfessor;

            if (!ProfessorSelecionado(out idProfessor))
            {
                MostrarAlert(
                    "Selecione um professor."
                );

                return;
            }

            ProfessorDados professor =
                GetProfessorById(idProfessor);

            if (professor == null)
            {
                MostrarAlert(
                    "Não foi possível encontrar o professor selecionado."
                );

                GetProfessores();
                return;
            }

            CarregarFormulario(professor);
            CarregarDisciplinasDoProfessor(idProfessor);

            controlos.Visible = true;
            painelDisciplinasProfessor.Visible = true;

            ViewState["op"] = "v";
            ViewState["ProfessorId"] = idProfessor;

            ActivarControlos(false);
        }

        protected void buttonCriar_Click(
            object sender,
            EventArgs e)
        {
            LimparFormulario();

            gridProfessores.SelectedIndex = -1;

            gridDisciplinasProfessor.DataSource = null;
            gridDisciplinasProfessor.DataBind();

            controlos.Visible = true;
            painelDisciplinasProfessor.Visible = false;

            ViewState["op"] = "i";
            ViewState["ProfessorId"] = null;

            ActivarControlos(true);
        }

        protected void buttonEditar_Click(
            object sender,
            EventArgs e)
        {
            int idProfessor;

            if (!ProfessorSelecionado(out idProfessor))
            {
                MostrarAlert(
                    "Selecione um professor."
                );

                return;
            }

            ProfessorDados professor =
                GetProfessorById(idProfessor);

            if (professor == null)
            {
                MostrarAlert(
                    "Não foi possível encontrar o professor selecionado."
                );

                GetProfessores();
                return;
            }

            CarregarFormulario(professor);
            CarregarDisciplinasDoProfessor(idProfessor);

            controlos.Visible = true;
            painelDisciplinasProfessor.Visible = true;

            ViewState["op"] = "u";
            ViewState["ProfessorId"] = idProfessor;

            ActivarControlos(true);
        }

        protected void buttonReenviarCredenciais_Click(
            object sender,
            EventArgs e)
        {
            int idProfessor;

            if (!ProfessorSelecionado(out idProfessor))
            {
                MostrarAlert(
                    "Selecione um professor."
                );

                return;
            }

            ProfessorDados professor =
                GetProfessorById(idProfessor);

            if (professor == null)
            {
                MostrarAlert(
                    "Não foi possível encontrar o professor selecionado."
                );

                GetProfessores();
                return;
            }

            if (!professor.Ativo)
            {
                MostrarAlert(
                    "O professor selecionado está inativo. " +
                    "Ative a conta antes de reenviar as credenciais."
                );

                return;
            }

            if (string.IsNullOrWhiteSpace(professor.Email))
            {
                MostrarAlert(
                    "O professor não possui um email válido. " +
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
                        professor.UserId,
                        false
                    );

                if (utilizador == null)
                {
                    throw new InvalidOperationException(
                        "Não foi possível encontrar a conta de acesso " +
                        "associada ao professor."
                    );
                }

                string emailAtual =
                    professor.Email
                        .Trim()
                        .ToLowerInvariant();

                if (EmailJaExisteNoMembership(
                        emailAtual,
                        professor.UserId))
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

                CriarConta
                    .EnviarEmailCredenciaisRedefinidas(
                        emailAtual,
                        professor.Nome,
                        utilizador.UserName,
                        novaPassword,
                        ObterUrlLogin()
                    );

                MostrarAlert(
                    "Foi gerada uma nova palavra-passe e as credenciais " +
                    "foram enviadas para " + emailAtual + "."
                );
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError(
                    "Erro ao reenviar credenciais do professor: " +
                    ex
                );

                if (passwordRedefinida)
                {
                    MostrarAlert(
                        "A palavra-passe foi redefinida, mas não foi possível " +
                        "enviar o email. Confirme o endereço e a configuração " +
                        "SMTP e volte a clicar em Reenviar credenciais. " +
                        ex.Message
                    );
                }
                else
                {
                    MostrarAlert(
                        "Não foi possível reenviar as credenciais. " +
                        ex.Message
                    );
                }
            }
        }

        protected void buttonGuardar_Click(
            object sender,
            EventArgs e)
        {
            Page.Validate("professor");

            if (!Page.IsValid)
            {
                controlos.Visible = true;
                return;
            }

            string modo =
                Convert.ToString(
                    ViewState["op"]
                )
                .ToLowerInvariant();

            string nome =
                txtNome.Text.Trim();

            string email =
                txtEmail.Text
                    .Trim()
                    .ToLowerInvariant();

            string telefone =
                txtTelefone.Text.Trim();

            string numeroProcesso =
                txtNumeroProcesso.Text.Trim();

            string nif =
                ValidadorNif.Normalizar(
                    txtNIF.Text
                );

            string mensagemNif;

            if (!ValidadorNif.ValidarLocalmente(
                    nif,
                    out mensagemNif))
            {
                MostrarAlert(mensagemNif);

                controlos.Visible = true;
                return;
            }

            int agrupamentoId;

            try
            {
                agrupamentoId =
                    GetAgrupamentoIdFromSession();
            }
            catch (Exception ex)
            {
                MostrarAlert(ex.Message);
                return;
            }

            int? grupoRecrutamentoId =
                null;

            if (!string.IsNullOrWhiteSpace(
                    ddlGrupoRecrutamento.SelectedValue))
            {
                grupoRecrutamentoId =
                    Convert.ToInt32(
                        ddlGrupoRecrutamento
                            .SelectedValue
                    );
            }

            if (modo == "i")
            {
                CriarNovoProfessor(
                    agrupamentoId,
                    nome,
                    numeroProcesso,
                    email,
                    telefone,
                    nif,
                    grupoRecrutamentoId
                );

                return;
            }

            if (modo == "u")
            {
                AtualizarProfessorExistente(
                    agrupamentoId,
                    nome,
                    numeroProcesso,
                    email,
                    telefone,
                    nif,
                    grupoRecrutamentoId
                );

                return;
            }

            FecharFormulario();
        }

        protected void buttonCancelar_Click(
            object sender,
            EventArgs e)
        {
            FecharFormulario();
        }

        protected void buttonDisciplinasProfessor_Click(
            object sender,
            EventArgs e)
        {
            int idProfessor;

            if (!ProfessorSelecionado(out idProfessor))
            {
                MostrarAlert(
                    "Selecione o professor."
                );

                return;
            }

            Session["ProfessorId"] =
                idProfessor;

            Response.Redirect(
                "~/agrupamento/professor_disciplinas.aspx"
            );
        }

        #endregion


        #region Criar professor

        private void CriarNovoProfessor(
            int agrupamentoId,
            string nome,
            string numeroProcesso,
            string email,
            string telefone,
            string nif,
            int? grupoRecrutamentoId)
        {
            if (NifJaExiste(nif, null))
            {
                MostrarAlert(
                    "Já existe um aluno, professor ou encarregado " +
                    "de educação com este NIF."
                );

                controlos.Visible = true;
                return;
            }

            if (EmailJaExisteNaTabela(email, null))
            {
                MostrarAlert(
                    "Já existe um aluno, professor ou encarregado " +
                    "de educação com este email."
                );

                controlos.Visible = true;
                return;
            }

            if (EmailJaExisteNoMembership(email, null))
            {
                MostrarAlert(
                    "Já existe uma conta de utilizador associada " +
                    "a este email."
                );

                controlos.Visible = true;
                return;
            }

            string username =
                null;

            string password =
                null;

            try
            {
                Guid userIdProfessor =
                    CriarContaProfessor(
                        nome,
                        email,
                        out username,
                        out password
                    );

                int linhas =
                    InsertProfessor(
                        userIdProfessor,
                        agrupamentoId,
                        nome,
                        numeroProcesso,
                        email,
                        telefone,
                        nif,
                        grupoRecrutamentoId
                    );

                if (linhas != 1)
                {
                    throw new InvalidOperationException(
                        "O registo do professor não foi criado."
                    );
                }
            }
            catch (MembershipCreateUserException ex)
            {
                RemoverContaCriada(username);

                MostrarAlert(
                    "Não foi possível criar a conta do professor: " +
                    ex.Message
                );

                controlos.Visible = true;
                return;
            }
            catch (SqlException ex)
            {
                RemoverContaCriada(username);

                MostrarAlert(
                    "Erro na base de dados ao criar o professor: " +
                    ex.Message
                );

                controlos.Visible = true;
                return;
            }
            catch (Exception ex)
            {
                RemoverContaCriada(username);

                MostrarAlert(
                    "Não foi possível criar o professor: " +
                    ex.Message
                );

                controlos.Visible = true;
                return;
            }

            bool emailEnviado =
                EnviarCredenciaisIniciais(
                    email,
                    nome,
                    username,
                    password
                );

            FinalizarOperacaoComSucesso();

            if (emailEnviado)
            {
                MostrarAlert(
                    "Professor criado com sucesso. " +
                    "As credenciais foram enviadas por email."
                );
            }
            else
            {
                MostrarAlert(
                    "O professor e a respetiva conta foram criados, " +
                    "mas não foi possível enviar o email com as credenciais."
                );
            }
        }

        #endregion


        #region Atualizar professor

        private void AtualizarProfessorExistente(
            int agrupamentoId,
            string nome,
            string numeroProcesso,
            string email,
            string telefone,
            string nif,
            int? grupoRecrutamentoId)
        {
            int idProfessor;

            if (!TryGetProfessorIdViewState(
                    out idProfessor))
            {
                MostrarAlert(
                    "Não foi possível identificar o professor."
                );

                return;
            }

            ProfessorDados dadosAtuais =
                GetProfessorById(idProfessor);

            if (dadosAtuais == null)
            {
                MostrarAlert(
                    "O professor já não está disponível."
                );

                GetProfessores();
                return;
            }

            if (NifJaExiste(nif, idProfessor))
            {
                MostrarAlert(
                    "Já existe outro aluno, professor ou encarregado " +
                    "de educação com este NIF."
                );

                controlos.Visible = true;
                return;
            }

            if (EmailJaExisteNaTabela(email, idProfessor))
            {
                MostrarAlert(
                    "Já existe outro aluno, professor ou encarregado " +
                    "de educação com este email."
                );

                controlos.Visible = true;
                return;
            }

            if (EmailJaExisteNoMembership(
                    email,
                    dadosAtuais.UserId))
            {
                MostrarAlert(
                    "Já existe outra conta associada a este email."
                );

                controlos.Visible = true;
                return;
            }

            try
            {
                AtualizarProfessorEMembership(
                    dadosAtuais,
                    agrupamentoId,
                    nome,
                    numeroProcesso,
                    email,
                    telefone,
                    nif,
                    grupoRecrutamentoId
                );
            }
            catch (SqlException ex)
            {
                MostrarAlert(
                    "Erro na base de dados ao atualizar o professor: " +
                    ex.Message
                );

                controlos.Visible = true;
                return;
            }
            catch (Exception ex)
            {
                MostrarAlert(
                    "Não foi possível atualizar o professor: " +
                    ex.Message
                );

                controlos.Visible = true;
                return;
            }

            FinalizarOperacaoComSucesso();

            MostrarAlert(
                "Professor atualizado com sucesso."
            );
        }

        private void AtualizarProfessorEMembership(
            ProfessorDados dadosAtuais,
            int agrupamentoId,
            string nome,
            string numeroProcesso,
            string email,
            string telefone,
            string nif,
            int? grupoRecrutamentoId)
        {
            MembershipUser utilizador =
                Membership.GetUser(
                    dadosAtuais.UserId,
                    false
                );

            if (utilizador == null)
            {
                throw new InvalidOperationException(
                    "Não foi possível encontrar a conta do professor."
                );
            }

            string emailAnterior =
                utilizador.Email;

            utilizador.Email =
                email;

            Membership.UpdateUser(
                utilizador
            );

            try
            {
                int linhas =
                    UpdateProfessor(
                        dadosAtuais.Id,
                        agrupamentoId,
                        nome,
                        numeroProcesso,
                        email,
                        telefone,
                        nif,
                        grupoRecrutamentoId
                    );

                if (linhas != 1)
                {
                    throw new InvalidOperationException(
                        "O registo do professor não foi atualizado."
                    );
                }
            }
            catch
            {
                try
                {
                    utilizador.Email =
                        emailAnterior;

                    Membership.UpdateUser(
                        utilizador
                    );
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.TraceError(
                        "Erro ao repor o email do Membership do professor: " +
                        ex
                    );
                }

                throw;
            }
        }

        #endregion


        #region Paginação

        protected void gridProfessores_PageIndexChanging(
            object sender,
            GridViewPageEventArgs e)
        {
            gridProfessores.PageIndex =
                e.NewPageIndex;

            GetProfessores();

            gridProfessores.SelectedIndex = -1;

            ViewState["op"] = null;
            ViewState["ProfessorId"] = null;

            controlos.Visible = false;
            painelDisciplinasProfessor.Visible = false;
        }

        #endregion


        #region Listagem e leitura

        private void GetProfessores()
        {
            int agrupamentoId =
                GetAgrupamentoIdFromSession();

            DataTable tabela =
                new DataTable();

            const string sql = @"
                SELECT
                    p.Id,
                    p.Nome,
                    gr.Nome AS GrupoRecrutamento,
                    p.NIF,
                    p.Telefone,
                    p.Email

                FROM dbo.Professor p

                LEFT JOIN dbo.GrupoRecrutamento gr
                    ON p.GrupoRecrutamentoId = gr.Id

                WHERE p.AgrupamentoId = @AgrupamentoId
                  AND p.Ativo = 1

                ORDER BY p.Nome;";

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

            gridProfessores.DataSource =
                tabela;

            gridProfessores.DataBind();
        }

        private ProfessorDados GetProfessorById(
            int idProfessor)
        {
            int agrupamentoId =
                GetAgrupamentoIdFromSession();

            const string sql = @"
                SELECT TOP 1
                    Id,
                    UserId,
                    Nome,
                    Email,
                    Telefone,
                    NIF,
                    NumeroProcesso,
                    GrupoRecrutamentoId,
                    Ativo

                FROM dbo.Professor

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
                    .Value = idProfessor;

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

                    ProfessorDados professor =
                        new ProfessorDados();

                    professor.Id =
                        Convert.ToInt32(
                            reader["Id"]
                        );

                    professor.UserId =
                        (Guid)reader["UserId"];

                    professor.Nome =
                        Convert.ToString(
                            reader["Nome"]
                        );

                    professor.Email =
                        ValorTexto(
                            reader["Email"]
                        );

                    professor.Telefone =
                        ValorTexto(
                            reader["Telefone"]
                        );

                    professor.NIF =
                        ValorTexto(
                            reader["NIF"]
                        );

                    professor.NumeroProcesso =
                        ValorTexto(
                            reader["NumeroProcesso"]
                        );

                    professor.Ativo =
                        Convert.ToBoolean(
                            reader["Ativo"]
                        );

                    if (reader["GrupoRecrutamentoId"] !=
                        DBNull.Value)
                    {
                        professor.GrupoRecrutamentoId =
                            Convert.ToInt32(
                                reader["GrupoRecrutamentoId"]
                            );
                    }

                    return professor;
                }
            }
        }

        private void CarregarFormulario(
            ProfessorDados professor)
        {
            txtNome.Text =
                professor.Nome;

            txtEmail.Text =
                professor.Email;

            txtTelefone.Text =
                professor.Telefone;

            txtNIF.Text =
                professor.NIF;

            txtNumeroProcesso.Text =
                professor.NumeroProcesso;

            if (!professor.GrupoRecrutamentoId.HasValue)
            {
                ddlGrupoRecrutamento.SelectedIndex =
                    0;

                return;
            }

            string valor =
                professor
                    .GrupoRecrutamentoId
                    .Value
                    .ToString();

            ListItem item =
                ddlGrupoRecrutamento
                    .Items
                    .FindByValue(valor);

            if (item != null)
            {
                ddlGrupoRecrutamento.SelectedValue =
                    valor;
            }
            else
            {
                ddlGrupoRecrutamento.SelectedIndex =
                    0;
            }
        }

        #endregion


        #region Inserção e atualização

        private int InsertProfessor(
            Guid userId,
            int agrupamentoId,
            string nome,
            string numeroProcesso,
            string email,
            string telefone,
            string nif,
            int? grupoRecrutamentoId)
        {
            const string sql = @"
                INSERT INTO dbo.Professor
                (
                    AgrupamentoId,
                    UserId,
                    Nome,
                    Email,
                    Telefone,
                    NIF,
                    NumeroProcesso,
                    GrupoRecrutamentoId,
                    Ativo,
                    CreatedAt
                )
                VALUES
                (
                    @AgrupamentoId,
                    @UserId,
                    @Nome,
                    @Email,
                    @Telefone,
                    @NIF,
                    @NumeroProcesso,
                    @GrupoRecrutamentoId,
                    1,
                    SYSDATETIME()
                );";

            using (SqlConnection conn =
                new SqlConnection(_connectionString))
            using (SqlCommand cmd =
                new SqlCommand(sql, conn))
            {
                AdicionarParametrosProfessor(
                    cmd,
                    agrupamentoId,
                    nome,
                    numeroProcesso,
                    email,
                    telefone,
                    nif,
                    grupoRecrutamentoId
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

        private int UpdateProfessor(
            int idProfessor,
            int agrupamentoId,
            string nome,
            string numeroProcesso,
            string email,
            string telefone,
            string nif,
            int? grupoRecrutamentoId)
        {
            const string sql = @"
                UPDATE dbo.Professor

                SET
                    Nome = @Nome,
                    Email = @Email,
                    Telefone = @Telefone,
                    NIF = @NIF,
                    NumeroProcesso = @NumeroProcesso,
                    GrupoRecrutamentoId = @GrupoRecrutamentoId

                WHERE Id = @Id
                  AND AgrupamentoId = @AgrupamentoId;";

            using (SqlConnection conn =
                new SqlConnection(_connectionString))
            using (SqlCommand cmd =
                new SqlCommand(sql, conn))
            {
                AdicionarParametrosProfessor(
                    cmd,
                    agrupamentoId,
                    nome,
                    numeroProcesso,
                    email,
                    telefone,
                    nif,
                    grupoRecrutamentoId
                );

                cmd.Parameters
                    .Add(
                        "@Id",
                        SqlDbType.Int
                    )
                    .Value = idProfessor;

                conn.Open();

                return cmd.ExecuteNonQuery();
            }
        }

        private void AdicionarParametrosProfessor(
            SqlCommand cmd,
            int agrupamentoId,
            string nome,
            string numeroProcesso,
            string email,
            string telefone,
            string nif,
            int? grupoRecrutamentoId)
        {
            cmd.Parameters
                .Add(
                    "@AgrupamentoId",
                    SqlDbType.Int
                )
                .Value = agrupamentoId;

            cmd.Parameters
                .Add(
                    "@Nome",
                    SqlDbType.NVarChar,
                    200
                )
                .Value = nome;

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
                    "@NIF",
                    SqlDbType.NVarChar,
                    9
                )
                .Value = nif;

            cmd.Parameters
                .Add(
                    "@NumeroProcesso",
                    SqlDbType.NVarChar,
                    50
                )
                .Value = numeroProcesso;

            cmd.Parameters
                .Add(
                    "@GrupoRecrutamentoId",
                    SqlDbType.Int
                )
                .Value =
                grupoRecrutamentoId.HasValue
                    ? (object)grupoRecrutamentoId.Value
                    : DBNull.Value;
        }

        #endregion


        #region Validações de duplicação

        private bool NifJaExiste(
            string nif,
            int? professorIdIgnorar)
        {
            const string sql = @"
                SELECT
                    CASE

                        WHEN EXISTS
                        (
                            SELECT 1

                            FROM dbo.Aluno

                            WHERE NIF = @NIF
                        )

                        OR EXISTS
                        (
                            SELECT 1

                            FROM dbo.Professor

                            WHERE NIF = @NIF

                              AND
                              (
                                  @ProfessorIdIgnorar IS NULL
                                  OR Id <> @ProfessorIdIgnorar
                              )
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
                        "@ProfessorIdIgnorar",
                        SqlDbType.Int
                    )
                    .Value =
                    professorIdIgnorar.HasValue
                        ? (object)professorIdIgnorar.Value
                        : DBNull.Value;

                conn.Open();

                return Convert.ToInt32(
                    cmd.ExecuteScalar()
                ) == 1;
            }
        }

        private bool EmailJaExisteNaTabela(
            string email,
            int? professorIdIgnorar)
        {
            const string sql = @"
                SELECT
                    CASE

                        WHEN EXISTS
                        (
                            SELECT 1

                            FROM dbo.Aluno

                            WHERE Email = @Email
                        )

                        OR EXISTS
                        (
                            SELECT 1

                            FROM dbo.Professor

                            WHERE Email = @Email

                              AND
                              (
                                  @ProfessorIdIgnorar IS NULL
                                  OR Id <> @ProfessorIdIgnorar
                              )
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
                        "@ProfessorIdIgnorar",
                        SqlDbType.Int
                    )
                    .Value =
                    professorIdIgnorar.HasValue
                        ? (object)professorIdIgnorar.Value
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


        #region Disciplinas e grupos

        private void CarregarDisciplinasDoProfessor(
            int professorId)
        {
            DataTable tabela =
                new DataTable();

            const string sql = @"
                SELECT
                    gd.Nome AS GrupoDisciplinar,
                    d.Nome AS Disciplina,
                    pd.Desde,
                    pd.Ate,

                    CASE
                        WHEN pd.Ate IS NULL
                            THEN 'Ativa'
                        ELSE 'Terminada'
                    END AS Estado

                FROM dbo.ProfessorDisciplina pd

                INNER JOIN dbo.Disciplina d
                    ON pd.DisciplinaId = d.Id

                INNER JOIN dbo.GrupoDisciplinar gd
                    ON d.GrupoDisciplinarId = gd.Id

                WHERE pd.ProfessorId = @ProfessorId

                ORDER BY
                    CASE
                        WHEN pd.Ate IS NULL THEN 0
                        ELSE 1
                    END,
                    gd.Nome,
                    d.Nome,
                    pd.Desde DESC;";

            using (SqlConnection conn =
                new SqlConnection(_connectionString))
            using (SqlCommand cmd =
                new SqlCommand(sql, conn))
            using (SqlDataAdapter adapter =
                new SqlDataAdapter(cmd))
            {
                cmd.Parameters
                    .Add(
                        "@ProfessorId",
                        SqlDbType.Int
                    )
                    .Value = professorId;

                adapter.Fill(tabela);
            }

            gridDisciplinasProfessor.DataSource =
                tabela;

            gridDisciplinasProfessor.DataBind();
        }

        private void CarregarGrupoRecrutamento()
        {
            DataTable tabela =
                new DataTable();

            const string sql = @"
                SELECT
                    Id,
                    Nome

                FROM dbo.GrupoRecrutamento

                WHERE Ativo = 1

                ORDER BY Nome;";

            using (SqlConnection conn =
                new SqlConnection(_connectionString))
            using (SqlCommand cmd =
                new SqlCommand(sql, conn))
            using (SqlDataAdapter adapter =
                new SqlDataAdapter(cmd))
            {
                adapter.Fill(tabela);
            }

            ddlGrupoRecrutamento.DataSource =
                tabela;

            ddlGrupoRecrutamento.DataTextField =
                "Nome";

            ddlGrupoRecrutamento.DataValueField =
                "Id";

            ddlGrupoRecrutamento.DataBind();

            ddlGrupoRecrutamento.Items.Insert(
                0,
                new ListItem(
                    "-- selecionar --",
                    string.Empty
                )
            );
        }

        #endregion


        #region Membership

        private Guid CriarContaProfessor(
            string nome,
            string email,
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
                    nome
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
                    "Professor"
                );

                utilizadorCriado.IsApproved =
                    true;

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
            string nome,
            string username,
            string password)
        {
            try
            {
                CriarConta.EnviarEmailCredenciais(
                    email,
                    nome,
                    username,
                    password,
                    ObterUrlLogin()
                );

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError(
                    "Erro ao enviar credenciais do professor: " +
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
                        "Professor"))
                {
                    Roles.RemoveUserFromRole(
                        username,
                        "Professor"
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
                    "Erro ao remover a conta incompleta do professor: " +
                    ex
                );
            }
        }

        #endregion


        #region Sessão e agrupamento

        private int GetAgrupamentoIdFromSession()
        {
            if (Session["AgrupamentoID"] != null)
            {
                int agrupamentoIdSessao;

                if (int.TryParse(
                        Session["AgrupamentoID"].ToString(),
                        out agrupamentoIdSessao))
                {
                    return agrupamentoIdSessao;
                }
            }

            object sessionUserId =
                Session["UserId"];

            if (sessionUserId == null)
            {
                MembershipUser utilizador =
                    Membership.GetUser(
                        User.Identity.Name,
                        false
                    );

                if (utilizador != null &&
                    utilizador.ProviderUserKey != null)
                {
                    sessionUserId =
                        utilizador.ProviderUserKey;

                    Session["UserId"] =
                        utilizador.ProviderUserKey;
                }
            }

            if (sessionUserId == null)
            {
                throw new InvalidOperationException(
                    "A sessão terminou. Inicie sessão novamente."
                );
            }

            Guid userId;

            if (!Guid.TryParse(
                    sessionUserId.ToString(),
                    out userId))
            {
                throw new InvalidOperationException(
                    "O utilizador guardado na sessão é inválido."
                );
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
                    throw new InvalidOperationException(
                        "Não foi encontrado um agrupamento " +
                        "associado ao utilizador autenticado."
                    );
                }

                int agrupamentoId =
                    Convert.ToInt32(resultado);

                Session["AgrupamentoID"] =
                    agrupamentoId;

                return agrupamentoId;
            }
        }

        #endregion


        #region Formulário e utilidades

        private void ActivarControlos(
            bool ativo)
        {
            txtNome.Enabled = ativo;
            txtEmail.Enabled = ativo;
            txtTelefone.Enabled = ativo;
            txtNIF.Enabled = ativo;
            txtNumeroProcesso.Enabled = ativo;
            ddlGrupoRecrutamento.Enabled = ativo;

            buttonGuardar.Visible = ativo;
        }

        private void LimparFormulario()
        {
            txtNome.Text = string.Empty;
            txtEmail.Text = string.Empty;
            txtTelefone.Text = string.Empty;
            txtNIF.Text = string.Empty;
            txtNumeroProcesso.Text = string.Empty;

            if (ddlGrupoRecrutamento.Items.Count > 0)
            {
                ddlGrupoRecrutamento.SelectedIndex =
                    0;
            }
        }

        private void FecharFormulario()
        {
            LimparFormulario();

            ViewState["op"] =
                null;

            ViewState["ProfessorId"] =
                null;

            gridProfessores.SelectedIndex =
                -1;

            controlos.Visible =
                false;

            painelDisciplinasProfessor.Visible =
                false;

            gridDisciplinasProfessor.DataSource =
                null;

            gridDisciplinasProfessor.DataBind();
        }

        private void FinalizarOperacaoComSucesso()
        {
            GetProfessores();
            FecharFormulario();
        }

        private bool ProfessorSelecionado(
            out int idProfessor)
        {
            idProfessor =
                0;

            if (gridProfessores.SelectedDataKey == null ||
                gridProfessores.SelectedDataKey.Value == null)
            {
                return false;
            }

            return int.TryParse(
                gridProfessores
                    .SelectedDataKey
                    .Value
                    .ToString(),
                out idProfessor
            );
        }

        private bool TryGetProfessorIdViewState(
            out int idProfessor)
        {
            idProfessor =
                0;

            if (ViewState["ProfessorId"] == null)
            {
                return false;
            }

            return int.TryParse(
                ViewState["ProfessorId"].ToString(),
                out idProfessor
            );
        }

        private string ObterUrlLogin()
        {
            return Request.Url.GetLeftPart(
                       UriPartial.Authority
                   ) +
                   ResolveUrl("~/login.aspx");
        }

        private string ValorTexto(
            object valor)
        {
            return valor == null ||
                   valor == DBNull.Value
                ? string.Empty
                : valor.ToString();
        }

        private void MostrarAlert(
            string mensagem)
        {
            if (mensagem == null)
            {
                mensagem =
                    string.Empty;
            }

            string mensagemSegura =
                mensagem
                    .Replace("\\", "\\\\")
                    .Replace("'", "\\'")
                    .Replace("\r", "\\r")
                    .Replace("\n", "\\n");

            string script =
                string.Format(
                    "alert('{0}');",
                    mensagemSegura
                );

            ScriptManager.RegisterStartupScript(
                this,
                GetType(),
                Guid.NewGuid().ToString(),
                script,
                true
            );
        }

        #endregion


        #region Classe auxiliar

        private class ProfessorDados
        {
            public int Id { get; set; }

            public Guid UserId { get; set; }

            public string Nome { get; set; }

            public string Email { get; set; }

            public string Telefone { get; set; }

            public string NIF { get; set; }

            public string NumeroProcesso { get; set; }

            public int? GrupoRecrutamentoId { get; set; }

            public bool Ativo { get; set; }
        }

        #endregion
    }
}