using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AlunoGest.agrupamento
{
    public partial class professor_disciplinas : System.Web.UI.Page
    {
        private readonly string connectionString =
            ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                int professorId = GetProfessorId();
                if (professorId <= 0)
                {
                    MostrarAlert("Professor inválido.");
                    Response.Redirect("professores.aspx");
                    return;
                }

                CarregarDadosProfessor(professorId);
                CarregarGruposDisciplinares();
                InicializarGridDisciplinasDisponiveis();
                CarregarDisciplinasProfessor(professorId);
            }
        }

        private int GetProfessorId()
        {
            if (!string.IsNullOrWhiteSpace(Request.QueryString["id"]))
            {
                if (int.TryParse(Request.QueryString["id"], out int idQuery))
                    return idQuery;
            }

            if (Session["ProfessorId"] != null)
            {
                if (int.TryParse(Session["ProfessorId"].ToString(), out int idSession))
                    return idSession;
            }

            return 0;
        }

        private void CarregarDadosProfessor(int professorId)
        {
            const string sql = @"
                SELECT Id, Nome, NumeroProcesso
                FROM Professor
                WHERE Id = @Id
                  AND Ativo = 1;";

            using (var connection = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(sql, connection))
            {
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = professorId;

                connection.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        string nome = dr["Nome"].ToString();
                        string numero = dr["NumeroProcesso"] == DBNull.Value ? "" : dr["NumeroProcesso"].ToString();

                        lblProfessor.Text = $"Professor: {nome}" +
                                            (string.IsNullOrWhiteSpace(numero) ? "" : $" | Nº Processo: {numero}");
                    }
                }
            }
        }

        private void CarregarGruposDisciplinares()
        {
            DataTable dt = new DataTable();

            const string sql = @"
                SELECT Id, Nome
                FROM GrupoDisciplinar
                WHERE Ativo = 1
                ORDER BY Nome;";

            using (var connection = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(sql, connection))
            using (var da = new SqlDataAdapter(cmd))
            {
                da.Fill(dt);
            }

            ddlGrupoDisciplinar.DataSource = dt;
            ddlGrupoDisciplinar.DataTextField = "Nome";
            ddlGrupoDisciplinar.DataValueField = "Id";
            ddlGrupoDisciplinar.DataBind();

            ddlGrupoDisciplinar.Items.Insert(0, new ListItem("-- selecionar --", ""));
        }

        private void InicializarGridDisciplinasDisponiveis()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Id", typeof(int));
            dt.Columns.Add("Nome", typeof(string));

            gridDisciplinasDisponiveis.DataSource = dt;
            gridDisciplinasDisponiveis.DataBind();
        }

        private void CarregarDisciplinasDisponiveis(int professorId)
        {
            if (!int.TryParse(ddlGrupoDisciplinar.SelectedValue, out int grupoDisciplinarId))
            {
                InicializarGridDisciplinasDisponiveis();
                return;
            }

            DataTable dt = new DataTable();

            const string sql = @"
                SELECT d.Id, d.Nome
                FROM Disciplina d
                WHERE d.Ativa = 1
                  AND d.GrupoDisciplinarId = @GrupoDisciplinarId
                  AND NOT EXISTS
                  (
                      SELECT 1
                      FROM ProfessorDisciplina pd
                      WHERE pd.ProfessorId = @ProfessorId
                        AND pd.DisciplinaId = d.Id
                        AND pd.Ate IS NULL
                  )
                ORDER BY d.Nome;";

            using (var connection = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(sql, connection))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.Add("@ProfessorId", SqlDbType.Int).Value = professorId;
                cmd.Parameters.Add("@GrupoDisciplinarId", SqlDbType.Int).Value = grupoDisciplinarId;

                da.Fill(dt);
            }

            gridDisciplinasDisponiveis.DataSource = dt;
            gridDisciplinasDisponiveis.DataBind();
        }

        private void CarregarDisciplinasProfessor(int professorId)
        {
            DataTable dt = new DataTable();

            const string sql = @"
                SELECT
                    pd.Id AS ProfessorDisciplinaId,
                    d.Id AS DisciplinaId,
                    d.Nome AS Disciplina,
                    gd.Nome AS GrupoDisciplinar,
                    pd.Desde
                FROM ProfessorDisciplina pd
                INNER JOIN Disciplina d ON d.Id = pd.DisciplinaId
                INNER JOIN GrupoDisciplinar gd ON gd.Id = d.GrupoDisciplinarId
                WHERE pd.ProfessorId = @ProfessorId
                  AND pd.Ate IS NULL
                ORDER BY gd.Nome, d.Nome;";

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

        protected void ddlGrupoDisciplinar_SelectedIndexChanged(object sender, EventArgs e)
        {
            int professorId = GetProfessorId();
            if (professorId <= 0)
                return;

            CarregarDisciplinasDisponiveis(professorId);
        }

        protected void gridDisciplinasDisponiveis_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "Associar")
                return;

            int professorId = GetProfessorId();
            if (professorId <= 0)
            {
                MostrarAlert("Professor inválido.");
                return;
            }

            int rowIndex = Convert.ToInt32(e.CommandArgument);
            int disciplinaId = Convert.ToInt32(gridDisciplinasDisponiveis.DataKeys[rowIndex].Value);

            AssociarDisciplina(professorId, disciplinaId);

            CarregarDisciplinasDisponiveis(professorId);
            CarregarDisciplinasProfessor(professorId);
        }

        protected void gridDisciplinasProfessor_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "Dissociar")
                return;

            int professorId = GetProfessorId();
            if (professorId <= 0)
            {
                MostrarAlert("Professor inválido.");
                return;
            }

            int rowIndex = Convert.ToInt32(e.CommandArgument);
            int professorDisciplinaId = Convert.ToInt32(gridDisciplinasProfessor.DataKeys[rowIndex].Values["ProfessorDisciplinaId"]);

            DissociarDisciplina(professorDisciplinaId);

            CarregarDisciplinasDisponiveis(professorId);
            CarregarDisciplinasProfessor(professorId);
        }

        private void AssociarDisciplina(int professorId, int disciplinaId)
        {
            const string sqlVerificar = @"
                SELECT COUNT(1)
                FROM ProfessorDisciplina
                WHERE ProfessorId = @ProfessorId
                  AND DisciplinaId = @DisciplinaId
                  AND Ate IS NULL;";

            const string sqlInsert = @"
                INSERT INTO ProfessorDisciplina
                    (ProfessorId, DisciplinaId, Desde, Ate, Observacoes)
                VALUES
                    (@ProfessorId, @DisciplinaId, CAST(GETDATE() AS date), NULL, NULL);";

            using (var connection = new SqlConnection(connectionString))
            using (var cmdVerificar = new SqlCommand(sqlVerificar, connection))
            using (var cmdInsert = new SqlCommand(sqlInsert, connection))
            {
                connection.Open();

                cmdVerificar.Parameters.Add("@ProfessorId", SqlDbType.Int).Value = professorId;
                cmdVerificar.Parameters.Add("@DisciplinaId", SqlDbType.Int).Value = disciplinaId;

                int existe = Convert.ToInt32(cmdVerificar.ExecuteScalar());
                if (existe > 0)
                {
                    MostrarAlert("A disciplina já está associada ao professor.");
                    return;
                }

                cmdInsert.Parameters.Add("@ProfessorId", SqlDbType.Int).Value = professorId;
                cmdInsert.Parameters.Add("@DisciplinaId", SqlDbType.Int).Value = disciplinaId;

                cmdInsert.ExecuteNonQuery();
            }
        }

        private void DissociarDisciplina(int professorDisciplinaId)
        {
            const string sql = @"
                UPDATE ProfessorDisciplina
                SET Ate = CAST(GETDATE() AS date)
                WHERE Id = @Id
                  AND Ate IS NULL;";

            using (var connection = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(sql, connection))
            {
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = professorDisciplinaId;

                connection.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void MostrarAlert(string mensagem)
        {
            string script = $"alert('{mensagem.Replace("'", "\\'")}');";
            ClientScript.RegisterStartupScript(this.GetType(), "alert", script, true);
        }
    }
}