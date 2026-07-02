using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.Security;
using AlunoGest.Util;

namespace AlunoGest.agrupamento
{
    public partial class Alunos : System.Web.UI.Page
    {
        #region Campos

        private readonly string _ConnectionString =
            ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        #endregion

        #region Eventos de Página

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!TryGetAgrupamentoId(out _))
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

        protected void ButtonCriar_Click(object sender, EventArgs e)
        {
            LimparMensagem();
            LimparFormulario();

            GridAlunos.SelectedIndex = -1;
            Controlos.Visible = true;
            ViewState["Op"] = "criar";
            ChkAtivo.Checked = true;
        }

        protected void ButtonEditar_Click(object sender, EventArgs e)
        {
            LimparMensagem();

            if (!AlunoSelecionado(out int IdAluno))
            {
                MostrarMensagem("Selecione um aluno.");
                return;
            }

            CarregarAluno(IdAluno);
            Controlos.Visible = true;
            ViewState["Op"] = "editar";
        }

        protected void ButtonGuardar_Click(object sender, EventArgs e)
        {
            LimparMensagem();

            if (!Page.IsValid)
                return;

            if (!TryGetAgrupamentoId(out int AgrupamentoId))
            {
                Response.Redirect("~/login.aspx");
                return;
            }

            string Operacao = Convert.ToString(ViewState["Op"]);

            try
            {
                if (Operacao == "criar")
                {
                    string NomeCompleto = TxtNomeCompleto.Text.Trim();
                    string EmailPessoal = TxtEmail.Text.Trim();

                    if (string.IsNullOrWhiteSpace(EmailPessoal))
                    {
                        MostrarMensagem("É necessário indicar um email para gerar as credenciais de acesso.");
                        return;
                    }

                    // Cria a conta de login, envia o email com as credenciais.
                    Guid UserIdAluno = CriarContaAluno(NomeCompleto, EmailPessoal);

                    int Linhas = InsertAluno(
                        UserIdAluno,
                        AgrupamentoId,
                        NomeCompleto,
                        TxtNumeroProcesso.Text.Trim(),
                        EmailPessoal,
                        ChkAtivo.Checked);

                    if (Linhas > 0)
                        MostrarMensagem("Aluno criado com sucesso.", false);
                    else
                        MostrarMensagem("Não foi possível criar o aluno.");
                }
                else if (Operacao == "editar")
                {
                    if (!AlunoSelecionado(out int IdAluno))
                    {
                        MostrarMensagem("Selecione um aluno.");
                        return;
                    }

                    // No editar não criamos conta nem enviamos email.
                    int Linhas = UpdateAluno(
                        IdAluno,
                        AgrupamentoId,
                        TxtNomeCompleto.Text.Trim(),
                        TxtNumeroProcesso.Text.Trim(),
                        TxtEmail.Text.Trim(),
                        ChkAtivo.Checked);

                    if (Linhas > 0)
                        MostrarMensagem("Aluno atualizado com sucesso.", false);
                    else
                        MostrarMensagem("Não foi possível atualizar o aluno.");
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
            catch (Exception Ex)
            {
                MostrarMensagem("Erro ao guardar o aluno: " + Ex.Message);
            }
        }

        protected void ButtonCancelar_Click(object sender, EventArgs e)
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
            if (!TryGetAgrupamentoId(out int AgrupamentoId))
            {
                Response.Redirect("~/login.aspx");
                return;
            }

            DataTable Dt = new DataTable();

            const string Sql = @"
                SELECT
                    Id,
                    NomeCompleto,
                    NumeroProcesso,
                    Email,
                    Ativo
                FROM dbo.Aluno
                WHERE AgrupamentoId = @AgrupamentoId
                ORDER BY NomeCompleto;";

            using (SqlConnection Conn = new SqlConnection(_ConnectionString))
            using (SqlCommand Cmd = new SqlCommand(Sql, Conn))
            using (SqlDataAdapter Da = new SqlDataAdapter(Cmd))
            {
                Cmd.Parameters.Add("@AgrupamentoId", SqlDbType.Int).Value = AgrupamentoId;
                Da.Fill(Dt);
            }

            GridAlunos.DataSource = Dt;
            GridAlunos.DataBind();
        }

        private int InsertAluno(Guid UserId, int AgrupamentoId, string NomeCompleto, string NumeroProcesso, string Email, bool Ativo)
        {
            const string Sql = @"
                INSERT INTO dbo.Aluno
                (
                    AgrupamentoId,
                    UserId,
                    NomeCompleto,
                    NumeroProcesso,
                    Email,
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
                    @Ativo,
                    SYSDATETIME()
                );";

            using (SqlConnection Conn = new SqlConnection(_ConnectionString))
            using (SqlCommand Cmd = new SqlCommand(Sql, Conn))
            {
                Cmd.Parameters.Add("@AgrupamentoId", SqlDbType.Int).Value = AgrupamentoId;
                Cmd.Parameters.Add("@UserId", SqlDbType.UniqueIdentifier).Value = UserId;
                Cmd.Parameters.Add("@NomeCompleto", SqlDbType.NVarChar, 200).Value = NomeCompleto;
                Cmd.Parameters.Add("@NumeroProcesso", SqlDbType.NVarChar, 50).Value =
                    string.IsNullOrWhiteSpace(NumeroProcesso) ? (object)DBNull.Value : NumeroProcesso;
                Cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 150).Value =
                    string.IsNullOrWhiteSpace(Email) ? (object)DBNull.Value : Email;
                Cmd.Parameters.Add("@Ativo", SqlDbType.Bit).Value = Ativo;

                Conn.Open();
                return Cmd.ExecuteNonQuery();
            }
        }

        private int UpdateAluno(int IdAluno, int AgrupamentoId, string NomeCompleto, string NumeroProcesso, string Email, bool Ativo)
        {
            const string Sql = @"
                UPDATE dbo.Aluno
                SET
                    NomeCompleto   = @NomeCompleto,
                    NumeroProcesso = @NumeroProcesso,
                    Email          = @Email,
                    Ativo          = @Ativo
                WHERE Id            = @Id
                  AND AgrupamentoId = @AgrupamentoId;";

            using (SqlConnection Conn = new SqlConnection(_ConnectionString))
            using (SqlCommand Cmd = new SqlCommand(Sql, Conn))
            {
                Cmd.Parameters.Add("@Id", SqlDbType.Int).Value = IdAluno;
                Cmd.Parameters.Add("@AgrupamentoId", SqlDbType.Int).Value = AgrupamentoId;
                Cmd.Parameters.Add("@NomeCompleto", SqlDbType.NVarChar, 200).Value = NomeCompleto;
                Cmd.Parameters.Add("@NumeroProcesso", SqlDbType.NVarChar, 50).Value =
                    string.IsNullOrWhiteSpace(NumeroProcesso) ? (object)DBNull.Value : NumeroProcesso;
                Cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 150).Value =
                    string.IsNullOrWhiteSpace(Email) ? (object)DBNull.Value : Email;
                Cmd.Parameters.Add("@Ativo", SqlDbType.Bit).Value = Ativo;

                Conn.Open();
                return Cmd.ExecuteNonQuery();
            }
        }

        private DataRow GetAlunoById(int IdAluno)
        {
            if (!TryGetAgrupamentoId(out int AgrupamentoId))
                return null;

            DataTable Dt = new DataTable();

            const string Sql = @"
                SELECT
                    Id,
                    NomeCompleto,
                    NumeroProcesso,
                    Email,
                    Ativo
                FROM dbo.Aluno
                WHERE Id            = @Id
                  AND AgrupamentoId = @AgrupamentoId;";

            using (SqlConnection Conn = new SqlConnection(_ConnectionString))
            using (SqlCommand Cmd = new SqlCommand(Sql, Conn))
            using (SqlDataAdapter Da = new SqlDataAdapter(Cmd))
            {
                Cmd.Parameters.Add("@Id", SqlDbType.Int).Value = IdAluno;
                Cmd.Parameters.Add("@AgrupamentoId", SqlDbType.Int).Value = AgrupamentoId;
                Da.Fill(Dt);
            }

            if (Dt.Rows.Count == 0)
                return null;

            return Dt.Rows[0];
        }

        #endregion

        #region Membership

        private Guid CriarContaAluno(string NomeCompleto, string Email)
        {
            string UsernameBase = CriarConta.GerarUsername(NomeCompleto);
            string Username = CriarConta.GarantirUsernameUnico(UsernameBase);
            string Password = CriarConta.GerarPassword();

            Membership.CreateUser(Username, Password, Email);
            Roles.AddUserToRole(Username, "Aluno");

            MembershipUser User = Membership.GetUser(Username);

            CriarConta.EnviarEmailCredenciais(
                Email,
                NomeCompleto,
                Username,
                Password,
                "http://localhost/login.aspx");

            return (Guid)User.ProviderUserKey;
        }

        #endregion

        #region Auxiliares

        private void CarregarAluno(int IdAluno)
        {
            DataRow Dr = GetAlunoById(IdAluno);

            if (Dr == null)
            {
                MostrarMensagem("Não foi possível carregar o aluno.");
                return;
            }

            TxtNomeCompleto.Text = Dr["NomeCompleto"] == DBNull.Value ? "" : Dr["NomeCompleto"].ToString();
            TxtNumeroProcesso.Text = Dr["NumeroProcesso"] == DBNull.Value ? "" : Dr["NumeroProcesso"].ToString();
            TxtEmail.Text = Dr["Email"] == DBNull.Value ? "" : Dr["Email"].ToString();
            ChkAtivo.Checked = Dr["Ativo"] != DBNull.Value && Convert.ToBoolean(Dr["Ativo"]);
        }

        private bool AlunoSelecionado(out int IdAluno)
        {
            IdAluno = 0;

            if (GridAlunos.SelectedDataKey == null || GridAlunos.SelectedDataKey.Value == null)
                return false;

            return int.TryParse(GridAlunos.SelectedDataKey.Value.ToString(), out IdAluno);
        }

        private bool TryGetAgrupamentoId(out int AgrupamentoId)
        {
            AgrupamentoId = 0;

            if (Session["AgrupamentoID"] != null &&
                int.TryParse(Session["AgrupamentoID"].ToString(), out AgrupamentoId))
            {
                return true;
            }

            if (Session["UserId"] == null)
                return false;

            string UserId = Session["UserId"].ToString();

            const string Sql = @"
                SELECT Id
                FROM dbo.Agrupamento
                WHERE UserID = @UserID;";

            using (SqlConnection Conn = new SqlConnection(_ConnectionString))
            using (SqlCommand Cmd = new SqlCommand(Sql, Conn))
            {
                Cmd.Parameters.Add("@UserID", SqlDbType.NVarChar, 128).Value = UserId;

                Conn.Open();
                object Result = Cmd.ExecuteScalar();

                if (Result == null || Result == DBNull.Value)
                    return false;

                AgrupamentoId = Convert.ToInt32(Result);
                Session["AgrupamentoID"] = AgrupamentoId;
                return true;
            }
        }

        private void LimparFormulario()
        {
            TxtNomeCompleto.Text = "";
            TxtNumeroProcesso.Text = "";
            TxtEmail.Text = "";
            ChkAtivo.Checked = true;
        }

        private void MostrarMensagem(string Mensagem, bool Erro = true)
        {
            LblMensagem.Visible = true;
            LblMensagem.Text = Mensagem;
            LblMensagem.CssClass = Erro
                ? "alert alert-warning d-block"
                : "alert alert-success d-block";
        }

        private void LimparMensagem()
        {
            LblMensagem.Visible = false;
            LblMensagem.Text = "";
        }

        #endregion
    }
}