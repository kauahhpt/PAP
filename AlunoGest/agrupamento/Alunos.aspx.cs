using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

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

            if (!AlunoSelecionado(out int idAluno))
            {
                MostrarMensagem("Selecione um aluno.");
                return;
            }

            CarregarAluno(idAluno);
            Controlos.Visible = true;
            ViewState["Op"] = "editar";
        }

        protected void ButtonGuardar_Click(object sender, EventArgs e)
        {
            LimparMensagem();

            if (!Page.IsValid)
                return;

            if (!TryGetAgrupamentoId(out int agrupamentoId))
            {
                Response.Redirect("~/login.aspx");
                return;
            }

            string operacao = Convert.ToString(ViewState["Op"]);

            try
            {
                if (operacao == "criar")
                {
                    int linhas = InsertAluno(
                        agrupamentoId,
                        TxtNomeCompleto.Text.Trim(),
                        TxtNumeroProcesso.Text.Trim(),
                        TxtEmail.Text.Trim(),
                        ChkAtivo.Checked);

                    if (linhas > 0)
                        MostrarMensagem("Aluno criado com sucesso.", false);
                    else
                        MostrarMensagem("Não foi possível criar o aluno.");
                }
                else if (operacao == "editar")
                {
                    if (!AlunoSelecionado(out int idAluno))
                    {
                        MostrarMensagem("Selecione um aluno.");
                        return;
                    }

                    int linhas = UpdateAluno(
                        idAluno,
                        agrupamentoId,
                        TxtNomeCompleto.Text.Trim(),
                        TxtNumeroProcesso.Text.Trim(),
                        TxtEmail.Text.Trim(),
                        ChkAtivo.Checked);

                    if (linhas > 0)
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
            catch (Exception ex)
            {
                MostrarMensagem("Erro ao guardar o aluno: " + ex.Message);
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
            if (!TryGetAgrupamentoId(out int agrupamentoId))
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
                Cmd.Parameters.Add("@AgrupamentoId", SqlDbType.Int).Value = agrupamentoId;
                Da.Fill(Dt);
            }

            GridAlunos.DataSource = Dt;
            GridAlunos.DataBind();
        }

        private int InsertAluno(int agrupamentoId, string nomeCompleto, string numeroProcesso, string email, bool ativo)
        {
            const string Sql = @"
                INSERT INTO dbo.Aluno
                (
                    AgrupamentoId,
                    NomeCompleto,
                    NumeroProcesso,
                    Email,
                    Ativo,
                    CreatedAt
                )
                VALUES
                (
                    @AgrupamentoId,
                    @NomeCompleto,
                    @NumeroProcesso,
                    @Email,
                    @Ativo,
                    SYSDATETIME()
                );";

            using (SqlConnection Conn = new SqlConnection(_ConnectionString))
            using (SqlCommand Cmd = new SqlCommand(Sql, Conn))
            {
                Cmd.Parameters.Add("@AgrupamentoId", SqlDbType.Int).Value = agrupamentoId;
                Cmd.Parameters.Add("@NomeCompleto", SqlDbType.NVarChar, 200).Value = nomeCompleto;
                Cmd.Parameters.Add("@NumeroProcesso", SqlDbType.NVarChar, 50).Value =
                    string.IsNullOrWhiteSpace(numeroProcesso) ? (object)DBNull.Value : numeroProcesso;
                Cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 150).Value =
                    string.IsNullOrWhiteSpace(email) ? (object)DBNull.Value : email;
                Cmd.Parameters.Add("@Ativo", SqlDbType.Bit).Value = ativo;

                Conn.Open();
                return Cmd.ExecuteNonQuery();
            }
        }

        private int UpdateAluno(int idAluno, int agrupamentoId, string nomeCompleto, string numeroProcesso, string email, bool ativo)
        {
            const string Sql = @"
                UPDATE dbo.Aluno
                SET
                    NomeCompleto = @NomeCompleto,
                    NumeroProcesso = @NumeroProcesso,
                    Email = @Email,
                    Ativo = @Ativo
                WHERE Id = @Id
                  AND AgrupamentoId = @AgrupamentoId;";

            using (SqlConnection Conn = new SqlConnection(_ConnectionString))
            using (SqlCommand Cmd = new SqlCommand(Sql, Conn))
            {
                Cmd.Parameters.Add("@Id", SqlDbType.Int).Value = idAluno;
                Cmd.Parameters.Add("@AgrupamentoId", SqlDbType.Int).Value = agrupamentoId;
                Cmd.Parameters.Add("@NomeCompleto", SqlDbType.NVarChar, 200).Value = nomeCompleto;
                Cmd.Parameters.Add("@NumeroProcesso", SqlDbType.NVarChar, 50).Value =
                    string.IsNullOrWhiteSpace(numeroProcesso) ? (object)DBNull.Value : numeroProcesso;
                Cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 150).Value =
                    string.IsNullOrWhiteSpace(email) ? (object)DBNull.Value : email;
                Cmd.Parameters.Add("@Ativo", SqlDbType.Bit).Value = ativo;

                Conn.Open();
                return Cmd.ExecuteNonQuery();
            }
        }

        private DataRow GetAlunoById(int idAluno)
        {
            if (!TryGetAgrupamentoId(out int agrupamentoId))
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
                WHERE Id = @Id
                  AND AgrupamentoId = @AgrupamentoId;";

            using (SqlConnection Conn = new SqlConnection(_ConnectionString))
            using (SqlCommand Cmd = new SqlCommand(Sql, Conn))
            using (SqlDataAdapter Da = new SqlDataAdapter(Cmd))
            {
                Cmd.Parameters.Add("@Id", SqlDbType.Int).Value = idAluno;
                Cmd.Parameters.Add("@AgrupamentoId", SqlDbType.Int).Value = agrupamentoId;
                Da.Fill(Dt);
            }

            if (Dt.Rows.Count == 0)
                return null;

            return Dt.Rows[0];
        }

        #endregion

        #region Auxiliares

        private void CarregarAluno(int idAluno)
        {
            DataRow Dr = GetAlunoById(idAluno);

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

        private bool AlunoSelecionado(out int idAluno)
        {
            idAluno = 0;

            if (GridAlunos.SelectedDataKey == null || GridAlunos.SelectedDataKey.Value == null)
                return false;

            return int.TryParse(GridAlunos.SelectedDataKey.Value.ToString(), out idAluno);
        }

        private bool TryGetAgrupamentoId(out int agrupamentoId)
        {
            agrupamentoId = 0;

            if (Session["AgrupamentoID"] != null &&
                int.TryParse(Session["AgrupamentoID"].ToString(), out agrupamentoId))
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

                agrupamentoId = Convert.ToInt32(Result);
                Session["AgrupamentoID"] = agrupamentoId;
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

        private void MostrarMensagem(string mensagem, bool erro = true)
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
            LblMensagem.Text = "";
        }

        #endregion
    }
}