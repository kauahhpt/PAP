using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Security;
using System.Web.UI;

namespace AlunoGest.agrupamento
{
    public partial class professores : System.Web.UI.Page
    {
        private readonly string connectionString =
            ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CarregarGrupoRecrutamento();
                GetProfessores();
                controlos.Visible = false;
                painelDisciplinasProfessor.Visible = false;
            }
        }

        protected void buttonVer_Click(object sender, EventArgs e)
        {
            if (gridProfessores.SelectedRow == null)
                return;

            int idProfessor = Convert.ToInt32(gridProfessores.SelectedDataKey.Value);

            CarregarProfessor(idProfessor);
            CarregarDisciplinasDoProfessor(idProfessor);

            controlos.Visible = true;
            painelDisciplinasProfessor.Visible = true;
            ViewState["op"] = "v";
            ActivarControlos(false);
        }

        protected void buttonCriar_Click(object sender, EventArgs e)
        {
            LimparFormulario();
            gridDisciplinasProfessor.DataSource = null;
            gridDisciplinasProfessor.DataBind();

            controlos.Visible = true;
            painelDisciplinasProfessor.Visible = false;
            ViewState["op"] = "i";
            ActivarControlos(true);
        }

        protected void buttonEditar_Click(object sender, EventArgs e)
        {
            if (gridProfessores.SelectedRow == null)
                return;

            int idProfessor = Convert.ToInt32(gridProfessores.SelectedDataKey.Value);

            CarregarProfessor(idProfessor);
            CarregarDisciplinasDoProfessor(idProfessor);

            controlos.Visible = true;
            painelDisciplinasProfessor.Visible = true;
            ViewState["op"] = "u";
            ActivarControlos(true);
        }

        protected void buttonGuardar_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            int agrupamentoId = GetAgrupamentoIdFromSession();
            string modo = Convert.ToString(ViewState["op"])?.ToLowerInvariant();

            string nome = txtNome.Text.Trim();
            string email = txtEmail.Text.Trim();
            string numeroProcesso = txtNumeroProcesso.Text.Trim();

            int? grupoRecrutamentoId = null;
            if (!string.IsNullOrWhiteSpace(ddlGrupoRecrutamento.SelectedValue))
                grupoRecrutamentoId = Convert.ToInt32(ddlGrupoRecrutamento.SelectedValue);

            int linhasAfetadas = 0;

            if (modo == "i")
            {
                Guid userIdProfessor = CriarContaProfessor();

                linhasAfetadas = InsertProfessor(
                    userIdProfessor,
                    agrupamentoId,
                    nome,
                    numeroProcesso,
                    email,
                    grupoRecrutamentoId);

                if (linhasAfetadas == 0)
                    return;
            }
            else if (modo == "u")
            {
                if (gridProfessores.SelectedRow == null)
                    return;

                int idProfessor = Convert.ToInt32(gridProfessores.SelectedDataKey.Value);

                linhasAfetadas = UpdateProfessor(
                    idProfessor,
                    agrupamentoId,
                    nome,
                    numeroProcesso,
                    email,
                    grupoRecrutamentoId);

                if (linhasAfetadas == 0)
                    return;
            }
            else
            {
                LimparFormulario();
                ViewState["op"] = null;
                gridProfessores.SelectedIndex = -1;
                controlos.Visible = false;
                painelDisciplinasProfessor.Visible = false;
                return;
            }

            GetProfessores();
            LimparFormulario();
            ViewState["op"] = null;
            gridProfessores.SelectedIndex = -1;
            controlos.Visible = false;
            painelDisciplinasProfessor.Visible = false;
            gridDisciplinasProfessor.DataSource = null;
            gridDisciplinasProfessor.DataBind();
        }

        protected void buttonCancelar_Click(object sender, EventArgs e)
        {
            LimparFormulario();
            ViewState["op"] = null;
            gridProfessores.SelectedIndex = -1;
            controlos.Visible = false;
            painelDisciplinasProfessor.Visible = false;
            gridDisciplinasProfessor.DataSource = null;
            gridDisciplinasProfessor.DataBind();
        }

        protected void gridProfessores_PageIndexChanging(object sender, System.Web.UI.WebControls.GridViewPageEventArgs e)
        {
            gridProfessores.PageIndex = e.NewPageIndex;
            GetProfessores();
        }

        private void ActivarControlos(bool ativo)
        {
            txtNome.Enabled = ativo;
            txtEmail.Enabled = ativo;
            txtNumeroProcesso.Enabled = ativo;
            ddlGrupoRecrutamento.Enabled = ativo;
            buttonGuardar.Visible = ativo;
        }

        private void LimparFormulario()
        {
            txtNome.Text = "";
            txtEmail.Text = "";
            txtNumeroProcesso.Text = "";

            if (ddlGrupoRecrutamento.Items.Count > 0)
                ddlGrupoRecrutamento.SelectedIndex = 0;
        }

        private int GetAgrupamentoIdFromSession()
        {
            object sessionUserId = Session["UserId"];

            if (sessionUserId == null)
                throw new InvalidOperationException("Sessão inválida: não existe Session['UserId'].");

            if (!Guid.TryParse(sessionUserId.ToString(), out Guid userId))
                throw new InvalidOperationException("Session['UserId'] não contém um GUID válido.");

            const string sql = @"
                SELECT Id
                FROM Agrupamento
                WHERE UserId = @UserId
                  AND Ativo = 1;";

            using (var connection = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(sql, connection))
            {
                cmd.Parameters.Add("@UserId", SqlDbType.UniqueIdentifier).Value = userId;

                connection.Open();
                object result = cmd.ExecuteScalar();

                if (result == null || result == DBNull.Value)
                    throw new InvalidOperationException("Não foi encontrado nenhum agrupamento associado ao utilizador autenticado.");

                return Convert.ToInt32(result);
            }
        }

        #region Base de dados

        public void GetProfessores()
        {
            int agrupamentoId = GetAgrupamentoIdFromSession();

            var dt = new DataTable();

            const string sql = @"
                SELECT 
                    p.[Id], 
                    p.[Nome], 
                    gr.[Nome] AS GrupoRecrutamento, 
                    p.[Email]
                FROM [Professor] p
                LEFT JOIN [GrupoRecrutamento] gr ON p.[GrupoRecrutamentoId] = gr.[Id]
                WHERE p.[AgrupamentoId] = @AgrupamentoId
                  AND p.[Ativo] = 1
                ORDER BY p.[Nome];";

            using (var connection = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(sql, connection))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.Add("@AgrupamentoId", SqlDbType.Int).Value = agrupamentoId;
                da.Fill(dt);
            }

            gridProfessores.DataSource = dt;
            gridProfessores.DataBind();
        }

        public int InsertProfessor(Guid userId, int agrupamentoId, string nome, string numeroProcesso, string email, int? grupoRecrutamentoId)
        {
            const string sql = @"
                INSERT INTO [Professor]
                    ([AgrupamentoId], [UserId], [Nome], [Email], [NumeroProcesso], [GrupoRecrutamentoId], [Ativo], [CreatedAt])
                VALUES
                    (@AgrupamentoId, @UserId, @Nome, @Email, @NumeroProcesso, @GrupoRecrutamentoId, 1, SYSDATETIME());";

            using (var connection = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(sql, connection))
            {
                cmd.Parameters.Add("@AgrupamentoId", SqlDbType.Int).Value = agrupamentoId;
                cmd.Parameters.Add("@UserId", SqlDbType.UniqueIdentifier).Value = userId;
                cmd.Parameters.Add("@Nome", SqlDbType.NVarChar, 200).Value = nome;

                cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 150).Value =
                    string.IsNullOrWhiteSpace(email) ? (object)DBNull.Value : email;

                cmd.Parameters.Add("@NumeroProcesso", SqlDbType.NVarChar, 50).Value =
                    string.IsNullOrWhiteSpace(numeroProcesso) ? (object)DBNull.Value : numeroProcesso;

                cmd.Parameters.Add("@GrupoRecrutamentoId", SqlDbType.Int).Value =
                    grupoRecrutamentoId.HasValue ? (object)grupoRecrutamentoId.Value : DBNull.Value;

                connection.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        public int UpdateProfessor(int id, int agrupamentoId, string nome, string numeroProcesso, string email, int? grupoRecrutamentoId)
        {
            const string sql = @"
                UPDATE [Professor]
                SET
                    [Nome] = @Nome,
                    [Email] = @Email,
                    [NumeroProcesso] = @NumeroProcesso,
                    [GrupoRecrutamentoId] = @GrupoRecrutamentoId
                WHERE
                    [Id] = @Id
                    AND [AgrupamentoId] = @AgrupamentoId;";

            using (var connection = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(sql, connection))
            {
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                cmd.Parameters.Add("@AgrupamentoId", SqlDbType.Int).Value = agrupamentoId;
                cmd.Parameters.Add("@Nome", SqlDbType.NVarChar, 200).Value = nome;

                cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 150).Value =
                    string.IsNullOrWhiteSpace(email) ? (object)DBNull.Value : email;

                cmd.Parameters.Add("@NumeroProcesso", SqlDbType.NVarChar, 50).Value =
                    string.IsNullOrWhiteSpace(numeroProcesso) ? (object)DBNull.Value : numeroProcesso;

                cmd.Parameters.Add("@GrupoRecrutamentoId", SqlDbType.Int).Value =
                    grupoRecrutamentoId.HasValue ? (object)grupoRecrutamentoId.Value : DBNull.Value;

                connection.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        public DataRow GetProfessorById(int id)
        {
            int agrupamentoId = GetAgrupamentoIdFromSession();

            const string sql = @"
                SELECT [Id], [Nome], [Email], [NumeroProcesso], [GrupoRecrutamentoId]
                FROM [Professor]
                WHERE [Id] = @Id
                  AND [AgrupamentoId] = @AgrupamentoId;";

            var dt = new DataTable();

            using (var connection = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(sql, connection))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                cmd.Parameters.Add("@AgrupamentoId", SqlDbType.Int).Value = agrupamentoId;
                da.Fill(dt);
            }

            return dt.Rows.Count > 0 ? dt.Rows[0] : null;
        }

        protected void CarregarProfessor(int id)
        {
            DataRow prof = GetProfessorById(id);
            if (prof == null)
                return;

            txtNome.Text = prof["Nome"].ToString();
            txtEmail.Text = prof["Email"] == DBNull.Value ? "" : prof["Email"].ToString();
            txtNumeroProcesso.Text = prof["NumeroProcesso"] == DBNull.Value ? "" : prof["NumeroProcesso"].ToString();

            if (prof["GrupoRecrutamentoId"] == DBNull.Value)
            {
                if (ddlGrupoRecrutamento.Items.Count > 0)
                    ddlGrupoRecrutamento.SelectedIndex = 0;
            }
            else
            {
                string valor = prof["GrupoRecrutamentoId"].ToString();
                if (ddlGrupoRecrutamento.Items.FindByValue(valor) != null)
                    ddlGrupoRecrutamento.SelectedValue = valor;
            }
        }

        private void CarregarDisciplinasDoProfessor(int professorId)
        {
            var dt = new DataTable();

            const string sql = @"
                SELECT
                    gd.Nome AS GrupoDisciplinar,
                    d.Nome AS Disciplina,
                    pd.Desde,
                    pd.Ate,
                    CASE 
                        WHEN pd.Ate IS NULL THEN 'Ativa'
                        ELSE 'Terminada'
                    END AS Estado
                FROM ProfessorDisciplina pd
                INNER JOIN Disciplina d
                    ON pd.DisciplinaId = d.Id
                INNER JOIN GrupoDisciplinar gd
                    ON d.GrupoDisciplinarId = gd.Id
                WHERE pd.ProfessorId = @ProfessorId
                ORDER BY
                    CASE WHEN pd.Ate IS NULL THEN 0 ELSE 1 END,
                    gd.Nome,
                    d.Nome,
                    pd.Desde DESC;";

            using (var connection = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(sql, connection))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.Add("@ProfessorId", SqlDbType.Int).Value = professorId;
                da.Fill(dt);
            }

            gridDisciplinasProfessor.DataSource = dt;
            gridDisciplinasProfessor.DataBind();
        }

        private void CarregarGrupoRecrutamento()
        {
            var dt = new DataTable();

            const string sql = @"
                SELECT Id, Nome
                FROM GrupoRecrutamento
                WHERE Ativo = 1
                ORDER BY Nome;";

            using (var connection = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(sql, connection))
            using (var da = new SqlDataAdapter(cmd))
            {
                da.Fill(dt);
            }

            ddlGrupoRecrutamento.DataSource = dt;
            ddlGrupoRecrutamento.DataTextField = "Nome";
            ddlGrupoRecrutamento.DataValueField = "Id";
            ddlGrupoRecrutamento.DataBind();

            ddlGrupoRecrutamento.Items.Insert(0, new System.Web.UI.WebControls.ListItem("-- selecionar --", ""));
        }

        #endregion

        #region Membership

        private Guid CriarContaProfessor()
        {
            string username = GerarUsernameProfessor(txtNome.Text);
            string password = "123123";

            Membership.CreateUser(username, password, txtEmail.Text);
            Roles.AddUserToRole(username, "Professor");

            MembershipUser user = Membership.GetUser(username);
            return (Guid)user.ProviderUserKey;
        }

        private static readonly HashSet<string> PalavrasIgnorar = new HashSet<string>
        {
            "de", "da", "do", "das", "dos", "e", "a", "o"
        };

        private string NormalizarTexto(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return string.Empty;

            texto = texto.ToLowerInvariant();

            texto = texto.Replace("á", "a").Replace("à", "a").Replace("ã", "a").Replace("â", "a");
            texto = texto.Replace("é", "e").Replace("ê", "e");
            texto = texto.Replace("í", "i");
            texto = texto.Replace("ó", "o").Replace("ô", "o").Replace("õ", "o");
            texto = texto.Replace("ú", "u");
            texto = texto.Replace("ç", "c");

            texto = texto.Replace("-", "")
                         .Replace("_", "")
                         .Replace(".", "")
                         .Replace(",", "");

            return texto;
        }

        private string GerarUsernameProfessor(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new ArgumentException("Nome do professor inválido.");

            string[] partes = nome.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            var palavrasValidas = new List<string>();
            foreach (string p in partes)
            {
                string n = NormalizarTexto(p);
                if (!PalavrasIgnorar.Contains(n))
                    palavrasValidas.Add(n);
            }

            if (palavrasValidas.Count == 0)
                throw new InvalidOperationException("Não foi possível gerar username a partir do nome.");

            string primeiro = palavrasValidas.First();
            string ultimo = palavrasValidas.Last();

            int random = new Random().Next(1000, 9999);
            return $"{primeiro}.{ultimo}.{random}";
        }

        #endregion

        public void MostrarAlert(string mensagem)
        {
            string script = $"alert('{mensagem.Replace("'", "\\'")}');";
            ClientScript.RegisterStartupScript(this.GetType(), "alert", script, true);
        }

        protected void buttonDisciplinasProfessor_Click(object sender, EventArgs e)
        {
            if (gridProfessores.SelectedRow == null)
            {
                MostrarAlert("Selecione o professor.");
                return;
            }
            Session["ProfessorId"] = gridProfessores.SelectedDataKey.Value;
            Response.Redirect("~/agrupamento/professor_disciplinas.aspx");
        }
    }
}