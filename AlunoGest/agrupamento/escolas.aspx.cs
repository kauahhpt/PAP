using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AlunoGest.agrupamento
{
    public partial class escolas : System.Web.UI.Page
    {
        private readonly string connectionString =
            ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!TryGetAgrupamentoId(out _))
            {
                Response.Redirect("~/login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                controlos.Visible = false;
                lblMensagem.Visible = false;
                GetEscolas();
            }
        }

        protected void buttonVer_Click(object sender, EventArgs e)
        {
            LimparMensagem();

            if (!EscolaSelecionada(out int idEscola))
            {
                MostrarMensagem("Selecione uma escola.");
                return;
            }

            CarregarEscola(idEscola);
            controlos.Visible = true;
            ViewState["op"] = "ver";
            AtivarControlos(false);
        }

        protected void buttonCriar_Click(object sender, EventArgs e)
        {
            LimparMensagem();
            LimparFormulario();

            gridEscolas.SelectedIndex = -1;
            controlos.Visible = true;
            ViewState["op"] = "criar";
            AtivarControlos(true);
            chkAtiva.Checked = true;
        }

        protected void buttonEditar_Click(object sender, EventArgs e)
        {
            LimparMensagem();

            if (!EscolaSelecionada(out int idEscola))
            {
                MostrarMensagem("Selecione uma escola.");
                return;
            }

            CarregarEscola(idEscola);
            controlos.Visible = true;
            ViewState["op"] = "editar";
            AtivarControlos(true);
        }

        protected void buttonSalas_Click(object sender, EventArgs e)
        {
            LimparMensagem();

            if (!EscolaSelecionada(out int idEscola))
            {
                MostrarMensagem("Selecione uma escola.");
                return;
            }

            Response.Redirect("~/agrupamento/salas.aspx?id=" + idEscola);
        }

        protected void buttonGuardar_Click(object sender, EventArgs e)
        {
            LimparMensagem();

            if (!Page.IsValid)
                return;

            if (!TryGetAgrupamentoId(out int agrupamentoId))
            {
                Response.Redirect("~/login.aspx");
                return;
            }

            string operacao = Convert.ToString(ViewState["op"]);

            try
            {
                if (operacao == "criar")
                {
                    int linhas = InsertEscola(
                        agrupamentoId,
                        txtNome.Text.Trim(),
                        txtCodigoMEC.Text.Trim(),
                        txtMorada.Text.Trim(),
                        txtCodigoPostal.Text.Trim(),
                        txtLocalidade.Text.Trim(),
                        txtEmail.Text.Trim(),
                        txtTelefone.Text.Trim(),
                        chkAtiva.Checked);

                    if (linhas > 0)
                        MostrarMensagem("Escola criada com sucesso.", false);
                    else
                        MostrarMensagem("Não foi possível criar a escola.");
                }
                else if (operacao == "editar")
                {
                    if (!EscolaSelecionada(out int idEscola))
                    {
                        MostrarMensagem("Selecione uma escola.");
                        return;
                    }

                    int linhas = UpdateEscola(
                        idEscola,
                        agrupamentoId,
                        txtNome.Text.Trim(),
                        txtCodigoMEC.Text.Trim(),
                        txtMorada.Text.Trim(),
                        txtCodigoPostal.Text.Trim(),
                        txtLocalidade.Text.Trim(),
                        txtEmail.Text.Trim(),
                        txtTelefone.Text.Trim(),
                        chkAtiva.Checked);

                    if (linhas > 0)
                        MostrarMensagem("Escola atualizada com sucesso.", false);
                    else
                        MostrarMensagem("Não foi possível atualizar a escola.");
                }
                else
                {
                    MostrarMensagem("Operação inválida.");
                    return;
                }

                GetEscolas();
                LimparFormulario();
                gridEscolas.SelectedIndex = -1;
                ViewState["op"] = null;
                controlos.Visible = false;
            }
            catch (Exception ex)
            {
                MostrarMensagem("Erro ao guardar a escola: " + ex.Message);
            }
        }

        protected void buttonCancelar_Click(object sender, EventArgs e)
        {
            LimparFormulario();
            LimparMensagem();
            gridEscolas.SelectedIndex = -1;
            ViewState["op"] = null;
            controlos.Visible = false;
        }

        private void GetEscolas()
        {
            if (!TryGetAgrupamentoId(out int agrupamentoId))
            {
                Response.Redirect("~/login.aspx");
                return;
            }

            DataTable dt = new DataTable();

            const string sql = @"
                SELECT
                    Id,
                    Nome,
                    CodigoMEC,
                    Morada,
                    CodigoPostal,
                    Localidade,
                    Email,
                    Telefone,
                    Ativa
                FROM dbo.Escola
                WHERE AgrupamentoId = @AgrupamentoId
                ORDER BY Nome;";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.Add("@AgrupamentoId", SqlDbType.Int).Value = agrupamentoId;
                da.Fill(dt);
            }

            gridEscolas.DataSource = dt;
            gridEscolas.DataBind();
        }

        private int InsertEscola(int agrupamentoId, string nome, string codigoMec, string morada,
            string codigoPostal, string localidade, string email, string telefone, bool ativa)
        {
            const string sql = @"
                INSERT INTO dbo.Escola
                (
                    AgrupamentoId,
                    Nome,
                    CodigoMEC,
                    Morada,
                    CodigoPostal,
                    Localidade,
                    Email,
                    Telefone,
                    Ativa
                )
                VALUES
                (
                    @AgrupamentoId,
                    @Nome,
                    @CodigoMEC,
                    @Morada,
                    @CodigoPostal,
                    @Localidade,
                    @Email,
                    @Telefone,
                    @Ativa
                );";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add("@AgrupamentoId", SqlDbType.Int).Value = agrupamentoId;
                cmd.Parameters.Add("@Nome", SqlDbType.NVarChar, 200).Value = nome;
                cmd.Parameters.Add("@CodigoMEC", SqlDbType.NVarChar, 20).Value =
                    string.IsNullOrWhiteSpace(codigoMec) ? (object)DBNull.Value : codigoMec;
                cmd.Parameters.Add("@Morada", SqlDbType.NVarChar, 300).Value =
                    string.IsNullOrWhiteSpace(morada) ? (object)DBNull.Value : morada;
                cmd.Parameters.Add("@CodigoPostal", SqlDbType.VarChar, 8).Value =
                    string.IsNullOrWhiteSpace(codigoPostal) ? (object)DBNull.Value : codigoPostal;
                cmd.Parameters.Add("@Localidade", SqlDbType.NVarChar, 100).Value =
                    string.IsNullOrWhiteSpace(localidade) ? (object)DBNull.Value : localidade;
                cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 150).Value =
                    string.IsNullOrWhiteSpace(email) ? (object)DBNull.Value : email;
                cmd.Parameters.Add("@Telefone", SqlDbType.NVarChar, 30).Value =
                    string.IsNullOrWhiteSpace(telefone) ? (object)DBNull.Value : telefone;
                cmd.Parameters.Add("@Ativa", SqlDbType.Bit).Value = ativa;

                conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        private int UpdateEscola(int idEscola, int agrupamentoId, string nome, string codigoMec, string morada,
            string codigoPostal, string localidade, string email, string telefone, bool ativa)
        {
            const string sql = @"
                UPDATE dbo.Escola
                SET
                    Nome = @Nome,
                    CodigoMEC = @CodigoMEC,
                    Morada = @Morada,
                    CodigoPostal = @CodigoPostal,
                    Localidade = @Localidade,
                    Email = @Email,
                    Telefone = @Telefone,
                    Ativa = @Ativa
                WHERE Id = @Id
                  AND AgrupamentoId = @AgrupamentoId;";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = idEscola;
                cmd.Parameters.Add("@AgrupamentoId", SqlDbType.Int).Value = agrupamentoId;
                cmd.Parameters.Add("@Nome", SqlDbType.NVarChar, 200).Value = nome;
                cmd.Parameters.Add("@CodigoMEC", SqlDbType.NVarChar, 20).Value =
                    string.IsNullOrWhiteSpace(codigoMec) ? (object)DBNull.Value : codigoMec;
                cmd.Parameters.Add("@Morada", SqlDbType.NVarChar, 300).Value =
                    string.IsNullOrWhiteSpace(morada) ? (object)DBNull.Value : morada;
                cmd.Parameters.Add("@CodigoPostal", SqlDbType.VarChar, 8).Value =
                    string.IsNullOrWhiteSpace(codigoPostal) ? (object)DBNull.Value : codigoPostal;
                cmd.Parameters.Add("@Localidade", SqlDbType.NVarChar, 100).Value =
                    string.IsNullOrWhiteSpace(localidade) ? (object)DBNull.Value : localidade;
                cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 150).Value =
                    string.IsNullOrWhiteSpace(email) ? (object)DBNull.Value : email;
                cmd.Parameters.Add("@Telefone", SqlDbType.NVarChar, 30).Value =
                    string.IsNullOrWhiteSpace(telefone) ? (object)DBNull.Value : telefone;
                cmd.Parameters.Add("@Ativa", SqlDbType.Bit).Value = ativa;

                conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        private DataRow GetEscolaById(int idEscola)
        {
            if (!TryGetAgrupamentoId(out int agrupamentoId))
                return null;

            DataTable dt = new DataTable();

            const string sql = @"
                SELECT
                    Id,
                    Nome,
                    CodigoMEC,
                    Morada,
                    CodigoPostal,
                    Localidade,
                    Email,
                    Telefone,
                    Ativa
                FROM dbo.Escola
                WHERE Id = @Id
                  AND AgrupamentoId = @AgrupamentoId;";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = idEscola;
                cmd.Parameters.Add("@AgrupamentoId", SqlDbType.Int).Value = agrupamentoId;
                da.Fill(dt);
            }

            if (dt.Rows.Count == 0)
                return null;

            return dt.Rows[0];
        }

        private void CarregarEscola(int idEscola)
        {
            DataRow dr = GetEscolaById(idEscola);

            if (dr == null)
            {
                MostrarMensagem("Não foi possível carregar a escola.");
                return;
            }

            txtNome.Text = dr["Nome"] == DBNull.Value ? "" : dr["Nome"].ToString();
            txtCodigoMEC.Text = dr["CodigoMEC"] == DBNull.Value ? "" : dr["CodigoMEC"].ToString();
            txtMorada.Text = dr["Morada"] == DBNull.Value ? "" : dr["Morada"].ToString();
            txtCodigoPostal.Text = dr["CodigoPostal"] == DBNull.Value ? "" : dr["CodigoPostal"].ToString();
            txtLocalidade.Text = dr["Localidade"] == DBNull.Value ? "" : dr["Localidade"].ToString();
            txtEmail.Text = dr["Email"] == DBNull.Value ? "" : dr["Email"].ToString();
            txtTelefone.Text = dr["Telefone"] == DBNull.Value ? "" : dr["Telefone"].ToString();
            chkAtiva.Checked = dr["Ativa"] != DBNull.Value && Convert.ToBoolean(dr["Ativa"]);
        }

        private bool EscolaSelecionada(out int idEscola)
        {
            idEscola = 0;

            if (gridEscolas.SelectedDataKey == null || gridEscolas.SelectedDataKey.Value == null)
                return false;

            return int.TryParse(gridEscolas.SelectedDataKey.Value.ToString(), out idEscola);
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

            string userId = Session["UserId"].ToString();

            const string sql = @"
                SELECT Id
                FROM dbo.Agrupamento
                WHERE UserID = @UserID;";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add("@UserID", SqlDbType.NVarChar, 128).Value = userId;

                conn.Open();
                object result = cmd.ExecuteScalar();

                if (result == null || result == DBNull.Value)
                    return false;

                agrupamentoId = Convert.ToInt32(result);
                Session["AgrupamentoID"] = agrupamentoId;
                return true;
            }
        }

        private void AtivarControlos(bool ativo)
        {
            txtNome.Enabled = ativo;
            txtCodigoMEC.Enabled = ativo;
            txtMorada.Enabled = ativo;
            txtCodigoPostal.Enabled = ativo;
            txtLocalidade.Enabled = ativo;
            txtEmail.Enabled = ativo;
            txtTelefone.Enabled = ativo;
            chkAtiva.Enabled = ativo;
            buttonGuardar.Visible = ativo;
        }

        private void LimparFormulario()
        {
            txtNome.Text = "";
            txtCodigoMEC.Text = "";
            txtMorada.Text = "";
            txtCodigoPostal.Text = "";
            txtLocalidade.Text = "";
            txtEmail.Text = "";
            txtTelefone.Text = "";
            chkAtiva.Checked = true;
        }

        private void MostrarMensagem(string mensagem, bool erro = true)
        {
            lblMensagem.Visible = true;
            lblMensagem.Text = mensagem;
            lblMensagem.CssClass = erro
                ? "alert alert-warning d-block"
                : "alert alert-success d-block";
        }

        private void LimparMensagem()
        {
            lblMensagem.Visible = false;
            lblMensagem.Text = "";
        }

        protected void buttonCriarTurma_Click(object sender, EventArgs e)
        {
            LimparMensagem();

            if (!EscolaSelecionada(out int idEscola))
            {
                MostrarMensagem("Selecione uma escola.");
                return;
            }

            string nivel = ddlNivelEnsino.SelectedValue;

            if (nivel == "Ensino Básico - 3º Ciclo")
                Response.Redirect("~/agrupamento/turma_basico.aspx?id=" + idEscola);
            else if (nivel == "Ensino Secundário")
                Response.Redirect("~/agrupamento/turma_secundario.aspx?id=" + idEscola);
            else
            {
                MostrarMensagem("Selecione o nível de ensino.");
                return;
            }
        }

        protected void buttonTurmas_Click(object sender, EventArgs e)
        {
            LimparMensagem();

            if (!EscolaSelecionada(out int idEscola))
            {
                MostrarMensagem("Selecione uma escola.");
                return;
            }

            Response.Redirect("~/agrupamento/turmas.aspx?id=" + idEscola);
        }
    }
}