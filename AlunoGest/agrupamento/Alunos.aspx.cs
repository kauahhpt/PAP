using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
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

            CarregarAluno(idAluno);

            Controlos.Visible = true;
            ViewState["Op"] = "editar";
        }

        protected void ButtonGuardar_Click(
            object sender,
            EventArgs e)
        {
            LimparMensagem();

            if (!Page.IsValid)
            {
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
                TxtEmail.Text.Trim();

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
                return;
            }

            //ResultadoValidacaoNifApi resultadoApi =
            //    ValidadorNif.ValidarNaApi(nif);

            //if (resultadoApi.Estado !=
            //    EstadoValidacaoNifApi.Valido)
            //{
            //    MostrarMensagem(
            //        resultadoApi.Mensagem
            //    );

            //    return;
            //}

            try
            {
                if (operacao == "criar")
                {
                    if (NifJaExiste(
                            nif,
                            null,
                            null))
                    {
                        MostrarMensagem(
                            "Já existe um aluno ou professor " +
                            "com este NIF."
                        );

                        return;
                    }

                    Guid userIdAluno =
                        CriarContaAluno(
                            nomeCompleto,
                            email
                        );

                    int linhas =
                        InsertAluno(
                            userIdAluno,
                            agrupamentoId,
                            nomeCompleto,
                            numeroProcesso,
                            email,
                            telefone,
                            nif,
                            ChkAtivo.Checked
                        );

                    if (linhas == 0)
                    {
                        MostrarMensagem(
                            "Não foi possível criar o aluno."
                        );

                        return;
                    }

                    MostrarMensagem(
                        "Aluno criado com sucesso.",
                        false
                    );
                }
                else if (operacao == "editar")
                {
                    int idAluno;

                    if (!AlunoSelecionado(out idAluno))
                    {
                        MostrarMensagem(
                            "Selecione um aluno."
                        );

                        return;
                    }

                    if (NifJaExiste(
                            nif,
                            idAluno,
                            null))
                    {
                        MostrarMensagem(
                            "Já existe outro aluno ou professor " +
                            "com este NIF."
                        );

                        return;
                    }

                    int linhas =
                        UpdateAluno(
                            idAluno,
                            agrupamentoId,
                            nomeCompleto,
                            numeroProcesso,
                            email,
                            telefone,
                            nif,
                            ChkAtivo.Checked
                        );

                    if (linhas == 0)
                    {
                        MostrarMensagem(
                            "Não foi possível atualizar o aluno."
                        );

                        return;
                    }

                    MostrarMensagem(
                        "Aluno atualizado com sucesso.",
                        false
                    );
                }
                else
                {
                    MostrarMensagem(
                        "Operação inválida."
                    );

                    return;
                }

                GetAlunos();
                LimparFormulario();

                GridAlunos.SelectedIndex = -1;
                ViewState["Op"] = null;
                Controlos.Visible = false;
            }
            catch (MembershipCreateUserException ex)
            {
                MostrarMensagem(
                    "Não foi possível criar a conta do aluno: " +
                    ex.Message
                );
            }
            catch (SqlException ex)
            {
                MostrarMensagem(
                    "Erro na base de dados ao guardar o aluno: " +
                    ex.Message
                );
            }
            catch (Exception ex)
            {
                MostrarMensagem(
                    "Erro ao guardar o aluno: " +
                    ex.Message
                );
            }
        }

        protected void ButtonCancelar_Click(
            object sender,
            EventArgs e)
        {
            LimparFormulario();
            LimparMensagem();

            GridAlunos.SelectedIndex = -1;

            ViewState["Op"] = null;
            Controlos.Visible = false;
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

            GridAlunos.DataSource = tabela;
            GridAlunos.DataBind();
        }

        private DataRow GetAlunoById(
            int idAluno)
        {
            int agrupamentoId;

            if (!TryGetAgrupamentoId(out agrupamentoId))
            {
                return null;
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

                WHERE Id = @Id
                  AND AgrupamentoId = @AgrupamentoId;";

            using (SqlConnection conn =
                new SqlConnection(_connectionString))
            using (SqlCommand cmd =
                new SqlCommand(sql, conn))
            using (SqlDataAdapter adapter =
                new SqlDataAdapter(cmd))
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

                adapter.Fill(tabela);
            }

            return tabela.Rows.Count > 0
                ? tabela.Rows[0]
                : null;
        }

        private void CarregarAluno(
            int idAluno)
        {
            DataRow aluno =
                GetAlunoById(idAluno);

            if (aluno == null)
            {
                MostrarMensagem(
                    "Não foi possível carregar o aluno."
                );

                return;
            }

            TxtNomeCompleto.Text =
                ValorTexto(
                    aluno["NomeCompleto"]
                );

            TxtNumeroProcesso.Text =
                ValorTexto(
                    aluno["NumeroProcesso"]
                );

            TxtNIF.Text =
                ValorTexto(
                    aluno["NIF"]
                );

            TxtEmail.Text =
                ValorTexto(
                    aluno["Email"]
                );

            TxtTelefone.Text =
                ValorTexto(
                    aluno["Telefone"]
                );

            ChkAtivo.Checked =
                aluno["Ativo"] != DBNull.Value &&
                Convert.ToBoolean(aluno["Ativo"]);
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

        #region Validação de duplicação do NIF

        private bool NifJaExiste(
            string nif,
            int? alunoIdIgnorar,
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
                              AND
                              (
                                  @ProfessorIdIgnorar IS NULL
                                  OR Id <> @ProfessorIdIgnorar
                              )
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

        #endregion

        #region Membership

        private Guid CriarContaAluno(
            string nomeCompleto,
            string email)
        {
            string usernameBase =
                CriarConta.GerarUsername(
                    nomeCompleto
                );

            string username =
                CriarConta.GarantirUsernameUnico(
                    usernameBase
                );

            string password =
                CriarConta.GerarPassword();

            Membership.CreateUser(
                username,
                password,
                email
            );

            Roles.AddUserToRole(
                username,
                "Aluno"
            );

            MembershipUser utilizador =
                Membership.GetUser(username);

            if (utilizador == null)
            {
                throw new InvalidOperationException(
                    "A conta foi criada, mas não foi possível " +
                    "obter os dados do utilizador."
                );
            }

            CriarConta.EnviarEmailCredenciais(
                email,
                nomeCompleto,
                username,
                password,
                "http://localhost/login.aspx"
            );

            return (Guid)utilizador.ProviderUserKey;
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
    }
}