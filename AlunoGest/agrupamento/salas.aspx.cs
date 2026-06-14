using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace AlunoGest.agrupamento
{
    public partial class salas : System.Web.UI.Page
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

            if (!TryGetEscolaId(out int idEscola))
            {
                Response.Redirect("~/agrupamento/escolas.aspx");
                return;
            }

            if (!EscolaPertenceAoAgrupamento(idEscola))
            {
                Response.Redirect("~/agrupamento/escolas.aspx");
                return;
            }

            if (!IsPostBack)
            {
                controlos.Visible = false;
                lblMensagem.Visible = false;

                Session["EscolaIdSalas"] = idEscola;

                CarregarCabecalhoEscola(idEscola);
                GetSalas();
            }
        }

        protected void buttonVer_Click(object sender, EventArgs e)
        {
            LimparMensagem();

            if (!SalaSelecionada(out int idSala))
            {
                MostrarMensagem("Selecione uma sala.");
                return;
            }

            CarregarSala(idSala);
            controlos.Visible = true;
            ViewState["op"] = "ver";
            AtivarControlos(false);
        }

        protected void buttonCriar_Click(object sender, EventArgs e)
        {
            LimparMensagem();
            LimparFormulario();

            gridSalas.SelectedIndex = -1;
            controlos.Visible = true;
            ViewState["op"] = "criar";
            AtivarControlos(true);
        }

        protected void buttonEditar_Click(object sender, EventArgs e)
        {
            LimparMensagem();

            if (!SalaSelecionada(out int idSala))
            {
                MostrarMensagem("Selecione uma sala.");
                return;
            }

            CarregarSala(idSala);
            controlos.Visible = true;
            ViewState["op"] = "editar";
            AtivarControlos(true);
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

            if (!TryGetEscolaId(out int idEscola))
            {
                Response.Redirect("~/agrupamento/escolas.aspx");
                return;
            }

            if (!EscolaPertenceAoAgrupamento(idEscola))
            {
                Response.Redirect("~/agrupamento/escolas.aspx");
                return;
            }

            string operacao = Convert.ToString(ViewState["op"]);

            int? capacidade = null;
            if (!string.IsNullOrWhiteSpace(txtCapacidade.Text))
                capacidade = Convert.ToInt32(txtCapacidade.Text.Trim());

            try
            {
                if (operacao == "criar")
                {
                    int linhas = InsertSala(
                        idEscola,
                        txtNome.Text.Trim(),
                        capacidade);

                    if (linhas > 0)
                        MostrarMensagem("Sala criada com sucesso.", false);
                    else
                        MostrarMensagem("Não foi possível criar a sala.");
                }
                else if (operacao == "editar")
                {
                    if (!SalaSelecionada(out int idSala))
                    {
                        MostrarMensagem("Selecione uma sala.");
                        return;
                    }

                    int linhas = UpdateSala(
                        idSala,
                        idEscola,
                        txtNome.Text.Trim(),
                        capacidade);

                    if (linhas > 0)
                        MostrarMensagem("Sala atualizada com sucesso.", false);
                    else
                        MostrarMensagem("Não foi possível atualizar a sala.");
                }
                else
                {
                    MostrarMensagem("Operação inválida.");
                    return;
                }

                GetSalas();
                LimparFormulario();
                gridSalas.SelectedIndex = -1;
                ViewState["op"] = null;
                controlos.Visible = false;
            }
            catch (Exception ex)
            {
                MostrarMensagem("Erro ao guardar a sala: " + ex.Message);
            }
        }

        protected void buttonCancelar_Click(object sender, EventArgs e)
        {
            LimparFormulario();
            LimparMensagem();
            gridSalas.SelectedIndex = -1;
            ViewState["op"] = null;
            controlos.Visible = false;
        }

        protected void buttonVoltar_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/agrupamento/escolas.aspx");
        }

        private void GetSalas()
        {
            if (!TryGetEscolaId(out int idEscola))
            {
                Response.Redirect("~/agrupamento/escolas.aspx");
                return;
            }

            DataTable dt = new DataTable();

            const string sql = @"
                SELECT
                    Id,
                    Nome,
                    Capacidade
                FROM dbo.Sala
                WHERE EscolaId = @EscolaId
                ORDER BY Nome;";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.Add("@EscolaId", SqlDbType.Int).Value = idEscola;
                da.Fill(dt);
            }

            gridSalas.DataSource = dt;
            gridSalas.DataBind();
        }

        private int InsertSala(int escolaId, string nome, int? capacidade)
        {
            const string sql = @"
                INSERT INTO dbo.Sala
                (
                    EscolaId,
                    Nome,
                    Capacidade
                )
                VALUES
                (
                    @EscolaId,
                    @Nome,
                    @Capacidade
                );";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add("@EscolaId", SqlDbType.Int).Value = escolaId;
                cmd.Parameters.Add("@Nome", SqlDbType.NVarChar, 50).Value = nome;
                cmd.Parameters.Add("@Capacidade", SqlDbType.Int).Value =
                    capacidade.HasValue ? (object)capacidade.Value : DBNull.Value;

                conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        private int UpdateSala(int idSala, int escolaId, string nome, int? capacidade)
        {
            const string sql = @"
                UPDATE dbo.Sala
                SET
                    Nome = @Nome,
                    Capacidade = @Capacidade
                WHERE Id = @Id
                  AND EscolaId = @EscolaId;";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = idSala;
                cmd.Parameters.Add("@EscolaId", SqlDbType.Int).Value = escolaId;
                cmd.Parameters.Add("@Nome", SqlDbType.NVarChar, 50).Value = nome;
                cmd.Parameters.Add("@Capacidade", SqlDbType.Int).Value =
                    capacidade.HasValue ? (object)capacidade.Value : DBNull.Value;

                conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        private DataRow GetSalaById(int idSala)
        {
            if (!TryGetEscolaId(out int idEscola))
                return null;

            DataTable dt = new DataTable();

            const string sql = @"
                SELECT
                    Id,
                    Nome,
                    Capacidade
                FROM dbo.Sala
                WHERE Id = @Id
                  AND EscolaId = @EscolaId;";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = idSala;
                cmd.Parameters.Add("@EscolaId", SqlDbType.Int).Value = idEscola;
                da.Fill(dt);
            }

            if (dt.Rows.Count == 0)
                return null;

            return dt.Rows[0];
        }

        private void CarregarSala(int idSala)
        {
            DataRow dr = GetSalaById(idSala);

            if (dr == null)
            {
                MostrarMensagem("Não foi possível carregar a sala.");
                return;
            }

            txtNome.Text = dr["Nome"] == DBNull.Value ? "" : dr["Nome"].ToString();
            txtCapacidade.Text = dr["Capacidade"] == DBNull.Value ? "" : dr["Capacidade"].ToString();
        }

        private void CarregarCabecalhoEscola(int idEscola)
        {
            if (!TryGetAgrupamentoId(out int agrupamentoId))
                return;

            const string sql = @"
                SELECT Nome
                FROM dbo.Escola
                WHERE Id = @Id
                  AND AgrupamentoId = @AgrupamentoId;";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = idEscola;
                cmd.Parameters.Add("@AgrupamentoId", SqlDbType.Int).Value = agrupamentoId;

                conn.Open();
                object result = cmd.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                    lblEscola.Text = "Escola: " + result.ToString();
                else
                    lblEscola.Text = "Escola não encontrada.";
            }
        }

        private bool SalaSelecionada(out int idSala)
        {
            idSala = 0;

            if (gridSalas.SelectedDataKey == null || gridSalas.SelectedDataKey.Value == null)
                return false;

            return int.TryParse(gridSalas.SelectedDataKey.Value.ToString(), out idSala);
        }

        private bool TryGetEscolaId(out int escolaId)
        {
            escolaId = 0;

            if (!string.IsNullOrWhiteSpace(Request.QueryString["id"]) &&
                int.TryParse(Request.QueryString["id"], out escolaId))
            {
                return true;
            }

            if (Session["EscolaIdSalas"] != null &&
                int.TryParse(Session["EscolaIdSalas"].ToString(), out escolaId))
            {
                return true;
            }

            return false;
        }

        private bool EscolaPertenceAoAgrupamento(int idEscola)
        {
            if (!TryGetAgrupamentoId(out int agrupamentoId))
                return false;

            const string sql = @"
                SELECT COUNT(1)
                FROM dbo.Escola
                WHERE Id = @Id
                  AND AgrupamentoId = @AgrupamentoId;";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = idEscola;
                cmd.Parameters.Add("@AgrupamentoId", SqlDbType.Int).Value = agrupamentoId;

                conn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
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
            txtCapacidade.Enabled = ativo;
            buttonGuardar.Visible = ativo;
        }

        private void LimparFormulario()
        {
            txtNome.Text = "";
            txtCapacidade.Text = "";
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
    }
}