using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace AlunoGest.professor
{
    public partial class dashboard : System.Web.UI.Page
    {
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try { CarregarResumo(); CarregarTurmas(); }
                catch (Exception ex) { MostrarErro(ex.Message); }
            }
        }

        protected void TxtPesquisa_TextChanged(object sender, EventArgs e) { CarregarTurmas(); }

        private int ProfessorId
        {
            get
            {
                int id;
                if (Session["ProfessorID"] != null && int.TryParse(Session["ProfessorID"].ToString(), out id)) return id;
                if (Session["UserId"] == null) throw new InvalidOperationException("A sessão terminou. Inicie sessão novamente.");
                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand("SELECT Id FROM dbo.Professor WHERE UserId=@UserId AND Ativo=1", conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", new Guid(Session["UserId"].ToString())); conn.Open();
                    object value = cmd.ExecuteScalar();
                    if (value == null) throw new InvalidOperationException("Professor não encontrado.");
                    id = Convert.ToInt32(value); Session["ProfessorID"] = id; return id;
                }
            }
        }

        private void CarregarResumo()
        {
            const string sql = @"
                SELECT COUNT(DISTINCT td.TurmaId), COUNT(DISTINCT at2.AlunoId), COUNT(DISTINCT ev.Id)
                FROM dbo.TurmaDisciplinaProfessor tdp
                INNER JOIN dbo.TurmaDisciplina td ON td.Id=tdp.TurmaDisciplinaId
                LEFT JOIN dbo.AlunoTurma at2 ON at2.TurmaId=td.TurmaId AND at2.Ate IS NULL
                LEFT JOIN dbo.Evento ev ON ev.TurmaId=td.TurmaId AND ev.DataHora>=GETDATE()
                WHERE tdp.ProfessorId=@ProfessorId AND tdp.Ate IS NULL;";
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@ProfessorId", ProfessorId); conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader()) if (reader.Read())
                { LblTotalTurmas.Text=reader.GetInt32(0).ToString(); LblTotalAlunos.Text=reader.GetInt32(1).ToString(); LblTotalAtividades.Text=reader.GetInt32(2).ToString(); }
            }
        }

        private void CarregarTurmas()
        {
            const string sql = @"
                SELECT td.TurmaId,
                    CAST(t.AnoEscolaridade AS varchar(2))+'.º'+t.CodigoTurma AS Turma,
                    e.Nome AS Escola, al.Descricao AS AnoLetivo,
                    STUFF((SELECT DISTINCT ', '+d2.Nome FROM dbo.TurmaDisciplina td2
                           INNER JOIN dbo.TurmaDisciplinaProfessor tdp2 ON tdp2.TurmaDisciplinaId=td2.Id AND tdp2.ProfessorId=@ProfessorId AND tdp2.Ate IS NULL
                           INNER JOIN dbo.Disciplina d2 ON d2.Id=td2.DisciplinaId WHERE td2.TurmaId=td.TurmaId FOR XML PATH(''), TYPE).value('.','nvarchar(max)'),1,2,'') AS Disciplinas,
                    (SELECT COUNT(*) FROM dbo.AlunoTurma at2 WHERE at2.TurmaId=td.TurmaId AND at2.Ate IS NULL) AS TotalAlunos
                FROM dbo.TurmaDisciplinaProfessor tdp
                INNER JOIN dbo.TurmaDisciplina td ON td.Id=tdp.TurmaDisciplinaId
                INNER JOIN dbo.Turma t ON t.Id=td.TurmaId INNER JOIN dbo.Escola e ON e.Id=t.EscolaId INNER JOIN dbo.AnoLetivo al ON al.Id=t.AnoLetivoId
                WHERE tdp.ProfessorId=@ProfessorId AND tdp.Ate IS NULL
                  AND (@Pesquisa='' OR t.CodigoTurma LIKE '%'+@Pesquisa+'%' OR e.Nome LIKE '%'+@Pesquisa+'%'
                       OR EXISTS(SELECT 1 FROM dbo.TurmaDisciplina tx INNER JOIN dbo.Disciplina dx ON dx.Id=tx.DisciplinaId WHERE tx.TurmaId=t.Id AND dx.Nome LIKE '%'+@Pesquisa+'%'))
                GROUP BY td.TurmaId,t.AnoEscolaridade,t.CodigoTurma,e.Nome,al.Descricao ORDER BY al.Descricao DESC,t.AnoEscolaridade,t.CodigoTurma;";
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(_connectionString)) using (SqlCommand cmd = new SqlCommand(sql, conn)) using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            { cmd.Parameters.AddWithValue("@ProfessorId", ProfessorId); cmd.Parameters.AddWithValue("@Pesquisa", TxtPesquisa.Text.Trim()); da.Fill(dt); }
            GridTurmas.DataSource=dt; GridTurmas.DataBind();
        }

        private void MostrarErro(string texto) { LblMensagem.Text="Não foi possível carregar os dados: "+texto; LblMensagem.Visible=true; }
    }
}
