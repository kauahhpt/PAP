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

        private readonly string _ConnectionString =
            ConfigurationManager
                .ConnectionStrings["DefaultConnection"]
                .ConnectionString;

        #endregion

        #region Eventos de Página

        protected void Page_Load(object sender, EventArgs e)
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
                LblMensagem.Visible = false;

                GetAlunos();
            }
        }

        #endregion

        #region Eventos de Controlos

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
                MostrarMensagem("Selecione um aluno.");
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
                return;

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

            try
            {
                if (operacao == "criar")
                {
                    Guid userIdAluno =
                        CriarContaAluno(
                            nomeCompleto,
                            email
                        );

                    int linhas = InsertAluno(
                        userIdAluno,
                        agrupamentoId,
                        nomeCompleto,
                        numeroProcesso,
                        email,
                        telefone,
                        ChkAtivo.Checked
                    );

                    if (linhas > 0)
                    {
                        MostrarMensagem(
                            "Aluno criado com sucesso.",
                            false
                        );
                    }
                    else
                    {
                        MostrarMensagem(
                            "Não foi possível criar o aluno."
                        );

                        return;
                    }
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

                    int linhas = UpdateAluno(
                        idAluno,
                        agrupamentoId,
                        nomeCompleto,
                        numeroProcesso,
                        email,
                        telefone,
                        ChkAtivo.Checked
                    );

                    if (linhas > 0)
                    {
                        MostrarMensagem(
                            "Aluno atualizado com sucesso.",
                            false
                        );
                    }
                    else
                    {
                        MostrarMensagem(
                            "Não foi possível atualizar o aluno."
                        );

                        return;
                    }
                }
                else
                {
                    MostrarMensagem("Operação inválida.");
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

        #region Acesso a Dados

        private void GetAlunos()
        {
            int agrupamentoId;

            if (!TryGetAgrupamentoId(out agrupamentoId))
            {
                Response.Redirect("~/login.aspx");
                return;
            }

            DataTable dt = new DataTable();

            const string sql = @"
                SELECT
                    Id,
                    NomeCompleto,
                    NumeroProcesso,
                    Email,
                    Telefone,
                    Ativo
                FROM dbo.Aluno
                WHERE AgrupamentoId = @AgrupamentoId
                ORDER BY NomeCompleto;";

            using (SqlConnection conn =
                   new SqlConnection(_ConnectionString))
            using (SqlCommand cmd =
                   new SqlCommand(sql, conn))
            using (SqlDataAdapter da =
                   new SqlDataAdapter(cmd))
            {
                cmd.Parameters
                    .Add(
                        "@AgrupamentoId",
                        SqlDbType.Int
                    )
                    .Value = agrupamentoId;

                da.Fill(dt);
            }

            GridAlunos.DataSource = dt;
            GridAlunos.DataBind();
        }

        private int InsertAluno(
            Guid userId,
            int agrupamentoId,
            string nomeCompleto,
            string numeroProcesso,
            string email,
            string telefone,
            bool ativo)
        {
            const string sql = @"
                INSERT INTO dbo.Aluno
                (
                    AgrupamentoId,
                    UserId,
                    NomeCompleto,
                    NumeroProcesso,
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
                    @Email,
                    @Telefone,
                    @Ativo,
                    SYSDATETIME()
                );";

            using (SqlConnection conn =
                   new SqlConnection(_ConnectionString))
            using (SqlCommand cmd =
                   new SqlCommand(sql, conn))
            {
                cmd.Parameters
                    .Add(
                        "@AgrupamentoId",
                        SqlDbType.Int
                    )
                    .Value = agrupamentoId;

                cmd.Parameters
                    .Add(
                        "@UserId",
                        SqlDbType.UniqueIdentifier
                    )
                    .Value = userId;

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
                    .Value =
                    string.IsNullOrWhiteSpace(numeroProcesso)
                        ? (object)DBNull.Value
                        : numeroProcesso;

                cmd.Parameters
                    .Add(
                        "@Email",
                        SqlDbType.NVarChar,
                        150
                    )
                    .Value =
                    string.IsNullOrWhiteSpace(email)
                        ? (object)DBNull.Value
                        : email;

                cmd.Parameters
                    .Add(
                        "@Telefone",
                        SqlDbType.NVarChar,
                        20
                    )
                    .Value =
                    string.IsNullOrWhiteSpace(telefone)
                        ? (object)DBNull.Value
                        : telefone;

                cmd.Parameters
                    .Add(
                        "@Ativo",
                        SqlDbType.Bit
                    )
                    .Value = ativo;

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
            bool ativo)
        {
            const string sql = @"
                UPDATE dbo.Aluno
                SET
                    NomeCompleto   = @NomeCompleto,
                    NumeroProcesso = @NumeroProcesso,
                    Email          = @Email,
                    Telefone       = @Telefone,
                    Ativo          = @Ativo
                WHERE Id            = @Id
                  AND AgrupamentoId = @AgrupamentoId;";

            using (SqlConnection conn =
                   new SqlConnection(_ConnectionString))
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
                    .Value =
                    string.IsNullOrWhiteSpace(numeroProcesso)
                        ? (object)DBNull.Value
                        : numeroProcesso;

                cmd.Parameters
                    .Add(
                        "@Email",
                        SqlDbType.NVarChar,
                        150
                    )
                    .Value =
                    string.IsNullOrWhiteSpace(email)
                        ? (object)DBNull.Value
                        : email;

                cmd.Parameters
                    .Add(
                        "@Telefone",
                        SqlDbType.NVarChar,
                        20
                    )
                    .Value =
                    string.IsNullOrWhiteSpace(telefone)
                        ? (object)DBNull.Value
                        : telefone;

                cmd.Parameters
                    .Add(
                        "@Ativo",
                        SqlDbType.Bit
                    )
                    .Value = ativo;

                conn.Open();

                return cmd.ExecuteNonQuery();
            }
        }

        private DataRow GetAlunoById(int idAluno)
        {
            int agrupamentoId;

            if (!TryGetAgrupamentoId(out agrupamentoId))
                return null;

            DataTable dt = new DataTable();

            const string sql = @"
                SELECT
                    Id,
                    NomeCompleto,
                    NumeroProcesso,
                    Email,
                    Telefone,
                    Ativo
                FROM dbo.Aluno
                WHERE Id            = @Id
                  AND AgrupamentoId = @AgrupamentoId;";

            using (SqlConnection conn =
                   new SqlConnection(_ConnectionString))
            using (SqlCommand cmd =
                   new SqlCommand(sql, conn))
            using (SqlDataAdapter da =
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

                da.Fill(dt);
            }

            if (dt.Rows.Count == 0)
                return null;

            return dt.Rows[0];
        }

        #endregion

        #region Membership

        private Guid CriarContaAluno(
            string nomeCompleto,
            string email)
        {
            string usernameBase =
                CriarConta.GerarUsername(nomeCompleto);

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

            MembershipUser user =
                Membership.GetUser(username);

            if (user == null)
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

            return (Guid)user.ProviderUserKey;
        }

        #endregion

        #region Auxiliares

        private void CarregarAluno(int idAluno)
        {
            DataRow dr =
                GetAlunoById(idAluno);

            if (dr == null)
            {
                MostrarMensagem(
                    "Não foi possível carregar o aluno."
                );

                return;
            }

            TxtNomeCompleto.Text =
                dr["NomeCompleto"] == DBNull.Value
                    ? string.Empty
                    : dr["NomeCompleto"].ToString();

            TxtNumeroProcesso.Text =
                dr["NumeroProcesso"] == DBNull.Value
                    ? string.Empty
                    : dr["NumeroProcesso"].ToString();

            TxtEmail.Text =
                dr["Email"] == DBNull.Value
                    ? string.Empty
                    : dr["Email"].ToString();

            TxtTelefone.Text =
                dr["Telefone"] == DBNull.Value
                    ? string.Empty
                    : dr["Telefone"].ToString();

            ChkAtivo.Checked =
                dr["Ativo"] != DBNull.Value &&
                Convert.ToBoolean(dr["Ativo"]);
        }

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
                GridAlunos.SelectedDataKey.Value.ToString(),
                out idAluno
            );
        }

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
                return false;

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
                   new SqlConnection(_ConnectionString))
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

        private void LimparFormulario()
        {
            TxtNomeCompleto.Text = string.Empty;
            TxtNumeroProcesso.Text = string.Empty;
            TxtEmail.Text = string.Empty;
            TxtTelefone.Text = string.Empty;

            ChkAtivo.Checked = true;
        }

        private void MostrarMensagem(
            string mensagem,
            bool erro = true)
        {
            LblMensagem.Visible = true;
            LblMensagem.Text = mensagem;

            LblMensagem.CssClass = erro
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