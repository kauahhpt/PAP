using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;

namespace AlunoGest.aluno
{
    public partial class dashboard : System.Web.UI.Page
    {
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                GarantirTabelaEntregas();
                if (!IsPostBack)
                {
                    CarregarResumo();
                    CarregarEventos();
                    CarregarNotas();
                }
            }
            catch (Exception ex)
            {
                MostrarMensagem("Nao foi possivel carregar o dashboard: " + ex.Message, true);
            }
        }

        protected void GridEventos_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int index;
            if (e.CommandName != "AbrirEvento" || !int.TryParse(e.CommandArgument.ToString(), out index) || index < 0 || index >= GridEventos.Rows.Count) return;
            int eventoId = Convert.ToInt32(GridEventos.DataKeys[index].Value);
            AbrirEvento(eventoId);
        }

        protected void BtnEntregar_Click(object sender, EventArgs e)
        {
            int eventoId;
            if (!int.TryParse(HdnEventoId.Value, out eventoId)) { MostrarMensagem("Escolha um evento.", true); return; }
            if (!EventoPertenceAoAluno(eventoId)) { MostrarMensagem("Este evento nao pertence a sua turma.", true); return; }
            if (!FileEntrega.HasFile) { MostrarMensagem("Escolha um ficheiro para entregar.", true); return; }

            try
            {
                if (FileEntrega.PostedFile.ContentLength > 10 * 1024 * 1024) throw new InvalidOperationException("O anexo excede 10 MB.");
                string pasta = Server.MapPath("~/uploads/entregas/");
                Directory.CreateDirectory(pasta);
                string original = Path.GetFileName(FileEntrega.FileName);
                string nome = Guid.NewGuid().ToString("N") + "_" + original;
                FileEntrega.SaveAs(Path.Combine(pasta, nome));

                const string sql = @"
                    INSERT INTO dbo.EventoEntrega(EventoId,AlunoId,NomeFicheiro,CaminhoFicheiro,Observacao)
                    VALUES(@EventoId,@AlunoId,@Nome,@Caminho,@Observacao);";
                using (SqlConnection c = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand(sql, c))
                {
                    cmd.Parameters.AddWithValue("@EventoId", eventoId);
                    cmd.Parameters.AddWithValue("@AlunoId", AlunoId);
                    cmd.Parameters.AddWithValue("@Nome", original);
                    cmd.Parameters.AddWithValue("@Caminho", ResolveUrl("~/uploads/entregas/" + nome));
                    cmd.Parameters.AddWithValue("@Observacao", string.IsNullOrWhiteSpace(TxtObservacao.Text) ? (object)DBNull.Value : TxtObservacao.Text.Trim());
                    c.Open();
                    cmd.ExecuteNonQuery();
                }

                MostrarMensagem("Entrega enviada.", false);
                PainelEntrega.Visible = false;
                CarregarEventos();
                CarregarNotas();
            }
            catch (Exception ex) { MostrarMensagem("Nao foi possivel enviar: " + ex.Message, true); }
        }

        protected void BtnFecharEntrega_Click(object sender, EventArgs e)
        {
            PainelEntrega.Visible = false;
        }

        private int AlunoId
        {
            get
            {
                int id;
                if (Session["AlunoID"] != null && int.TryParse(Session["AlunoID"].ToString(), out id)) return id;
                if (Session["UserId"] == null) throw new InvalidOperationException("Sessao terminada.");
                using (SqlConnection c = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand("SELECT Id FROM dbo.Aluno WHERE UserId=@UserId AND Ativo=1", c))
                {
                    cmd.Parameters.AddWithValue("@UserId", new Guid(Session["UserId"].ToString()));
                    c.Open();
                    object value = cmd.ExecuteScalar();
                    if (value == null) throw new InvalidOperationException("Aluno nao encontrado.");
                    id = Convert.ToInt32(value);
                    Session["AlunoID"] = id;
                    return id;
                }
            }
        }

        private void CarregarResumo()
        {
            const string sql = @"
                SELECT TOP 1 a.NomeCompleto,
                       CAST(t.AnoEscolaridade AS varchar(2))+'.º'+t.CodigoTurma AS Turma,
                       e.Nome AS Escola
                FROM dbo.Aluno a
                LEFT JOIN dbo.AlunoTurma at2 ON at2.AlunoId=a.Id AND at2.Ate IS NULL
                LEFT JOIN dbo.Turma t ON t.Id=at2.TurmaId
                LEFT JOIN dbo.Escola e ON e.Id=t.EscolaId
                WHERE a.Id=@AlunoId;";
            using (SqlConnection c = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, c))
            {
                cmd.Parameters.AddWithValue("@AlunoId", AlunoId);
                c.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                    if (r.Read())
                        LblResumo.Text = r["NomeCompleto"] + " | Turma: " + (r["Turma"] == DBNull.Value ? "sem turma" : r["Turma"]) + " | " + (r["Escola"] == DBNull.Value ? "" : r["Escola"]);
            }
        }

        private void CarregarEventos()
        {
            const string sql = @"
                SELECT ev.Id, ev.Tipo, ev.Titulo, ev.DataHora,
                       CASE WHEN EXISTS(SELECT 1 FROM dbo.EventoEntrega ee WHERE ee.EventoId=ev.Id AND ee.AlunoId=@AlunoId)
                            THEN N'Entregue' ELSE N'Por entregar' END AS EstadoEntrega
                FROM dbo.Evento ev
                INNER JOIN dbo.AlunoTurma at2 ON at2.TurmaId=ev.TurmaId AND at2.Ate IS NULL
                WHERE at2.AlunoId=@AlunoId
                ORDER BY ev.DataHora DESC;";
            DataTable dt = new DataTable();
            using (SqlConnection c = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, c))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddWithValue("@AlunoId", AlunoId);
                da.Fill(dt);
            }
            GridEventos.DataSource = dt;
            GridEventos.DataBind();

            List<object> eventos = new List<object>();
            foreach (DataRow row in dt.Rows)
            {
                string tipo = row["Tipo"].ToString();
                eventos.Add(new
                {
                    id = row["Id"],
                    title = row["Titulo"].ToString(),
                    start = Convert.ToDateTime(row["DataHora"]).ToString("yyyy-MM-ddTHH:mm:ss"),
                    color = tipo == "Teste" ? "#1d4ed8" : tipo == "Trabalho" ? "#2563eb" : "#64748b"
                });
            }
            HdnEvents.Value = new JavaScriptSerializer().Serialize(eventos);
        }

        private void CarregarNotas()
        {
            const string sql = @"
                SELECT ev.Titulo AS Evento, ee.Nota, ee.Feedback
                FROM dbo.EventoEntrega ee
                INNER JOIN dbo.Evento ev ON ev.Id=ee.EventoId
                WHERE ee.AlunoId=@AlunoId AND ee.Nota IS NOT NULL
                ORDER BY ee.AvaliadoEm DESC;";
            DataTable dt = new DataTable();
            using (SqlConnection c = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, c))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddWithValue("@AlunoId", AlunoId);
                da.Fill(dt);
            }
            GridNotas.DataSource = dt;
            GridNotas.DataBind();
        }

        private void AbrirEvento(int eventoId)
        {
            if (!EventoPertenceAoAluno(eventoId)) return;
            using (SqlConnection c = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("SELECT Titulo,Tipo,DataHora FROM dbo.Evento WHERE Id=@Id", c))
            {
                cmd.Parameters.AddWithValue("@Id", eventoId);
                c.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                    if (r.Read())
                        LblEventoSelecionado.Text = r["Tipo"] + ": " + r["Titulo"] + " (" + Convert.ToDateTime(r["DataHora"]).ToString("dd/MM/yyyy HH:mm") + ")";
            }

            DataTable anexos = new DataTable();
            using (SqlConnection c = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("SELECT NomeFicheiro,CaminhoFicheiro FROM dbo.EventoAnexo WHERE EventoId=@Id ORDER BY CreatedAt", c))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddWithValue("@Id", eventoId);
                da.Fill(anexos);
            }
            RepeaterAnexosProfessor.DataSource = anexos;
            RepeaterAnexosProfessor.DataBind();
            HdnEventoId.Value = eventoId.ToString();
            TxtObservacao.Text = "";
            PainelEntrega.Visible = true;
        }

        private bool EventoPertenceAoAluno(int eventoId)
        {
            const string sql = @"SELECT COUNT(1) FROM dbo.Evento ev
                INNER JOIN dbo.AlunoTurma at2 ON at2.TurmaId=ev.TurmaId AND at2.Ate IS NULL
                WHERE ev.Id=@EventoId AND at2.AlunoId=@AlunoId";
            using (SqlConnection c = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, c))
            {
                cmd.Parameters.AddWithValue("@EventoId", eventoId);
                cmd.Parameters.AddWithValue("@AlunoId", AlunoId);
                c.Open();
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        private void GarantirTabelaEntregas()
        {
            const string sql = @"
                IF OBJECT_ID('dbo.EventoEntrega','U') IS NULL
                BEGIN
                    CREATE TABLE dbo.EventoEntrega
                    (
                        Id int IDENTITY(1,1) NOT NULL PRIMARY KEY,
                        EventoId int NOT NULL,
                        AlunoId int NOT NULL,
                        NomeFicheiro nvarchar(255) NOT NULL,
                        CaminhoFicheiro nvarchar(500) NOT NULL,
                        Observacao nvarchar(1000) NULL,
                        Nota decimal(5,2) NULL,
                        Feedback nvarchar(1000) NULL,
                        AvaliadoEm datetime2 NULL,
                        CreatedAt datetime2 NOT NULL CONSTRAINT DF_EventoEntrega_CreatedAt DEFAULT SYSDATETIME()
                    );
                END";
            using (SqlConnection c = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, c))
            {
                c.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private void MostrarMensagem(string texto, bool erro)
        {
            LblMensagem.Text = texto;
            LblMensagem.CssClass = erro ? "alert alert-warning d-block" : "alert alert-success d-block";
            LblMensagem.Visible = true;
        }
    }
}
