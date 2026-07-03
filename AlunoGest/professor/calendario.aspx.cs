using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;

namespace AlunoGest.professor
{
    public partial class calendario : System.Web.UI.Page
    {
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                GarantirTabelaEntregas();
                if (!IsPostBack)
                {
                    CarregarTurmas();
                    SelecionarTurmaDaUrl();
                    CarregarTudo();
                }
            }
            catch (Exception ex)
            {
                MostrarMensagem("Nao foi possivel carregar o calendario: " + ex.Message, true);
            }
        }

        protected void DdlTurmas_SelectedIndexChanged(object sender, EventArgs e)
        {
            int turmaId = TurmaSelecionada;
            if (turmaId > 0)
                Response.Redirect("~/professor/calendario.aspx?turma=" + turmaId);
        }

        protected void BtnGuardar_Click(object sender, EventArgs e)
        {
            int turmaId = TurmaSelecionada;
            if (turmaId == 0 || !ProfessorTemTurma(turmaId)) { MostrarMensagem("Escolha uma turma valida.", true); return; }
            if (string.IsNullOrWhiteSpace(TxtTitulo.Text)) { MostrarMensagem("Indique o titulo.", true); return; }
            DateTime dataHora;
            if (!DateTime.TryParse(TxtDataHora.Text, out dataHora)) { MostrarMensagem("Indique uma data valida.", true); return; }

            try
            {
                int eventoId;
                if (string.IsNullOrEmpty(HdnEventoId.Value))
                    eventoId = InserirEvento(turmaId, dataHora);
                else
                {
                    eventoId = Convert.ToInt32(HdnEventoId.Value);
                    AtualizarEvento(eventoId, turmaId, dataHora);
                }

                if (FileAnexo.HasFile)
                    GuardarAnexoProfessor(eventoId);

                MostrarMensagem("Evento guardado.", false);
                LimparEvento();
                CarregarTudo();
            }
            catch (Exception ex) { MostrarMensagem("Nao foi possivel guardar: " + ex.Message, true); }
        }

        protected void BtnLimpar_Click(object sender, EventArgs e)
        {
            LimparEvento();
        }

        protected void GridEventos_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int index;
            if (!int.TryParse(e.CommandArgument.ToString(), out index) || index < 0 || index >= GridEventos.Rows.Count) return;
            int eventoId = Convert.ToInt32(GridEventos.DataKeys[index].Value);
            int turmaId = TurmaSelecionada;
            if (!ProfessorTemTurma(turmaId)) return;

            if (e.CommandName == "EditarEvento")
                CarregarEventoParaFormulario(eventoId, turmaId);
            else if (e.CommandName == "ApagarEvento")
            {
                ApagarEvento(eventoId, turmaId);
                MostrarMensagem("Evento apagado.", false);
                LimparEvento();
                CarregarTudo();
            }
        }

        protected void GridEntregas_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int index;
            if (e.CommandName != "AvaliarEntrega" || !int.TryParse(e.CommandArgument.ToString(), out index) || index < 0 || index >= GridEntregas.Rows.Count) return;
            int entregaId = Convert.ToInt32(GridEntregas.DataKeys[index].Value);
            CarregarEntrega(entregaId);
        }

        protected void BtnGuardarAvaliacao_Click(object sender, EventArgs e)
        {
            int entregaId;
            decimal nota;
            if (!int.TryParse(HdnEntregaId.Value, out entregaId)) { MostrarMensagem("Escolha uma entrega.", true); return; }
            if (!decimal.TryParse(TxtNota.Text.Replace(".", ","), out nota) || nota < 0 || nota > 20)
            {
                MostrarMensagem("A nota deve estar entre 0 e 20.", true);
                return;
            }

            const string sql = @"
                UPDATE ee
                SET Nota=@Nota, Feedback=@Feedback, AvaliadoEm=SYSDATETIME()
                FROM dbo.EventoEntrega ee
                INNER JOIN dbo.Evento ev ON ev.Id=ee.EventoId
                WHERE ee.Id=@Id AND ev.TurmaId=@TurmaId;";
            using (SqlConnection c = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, c))
            {
                cmd.Parameters.AddWithValue("@Id", entregaId);
                cmd.Parameters.AddWithValue("@TurmaId", TurmaSelecionada);
                cmd.Parameters.AddWithValue("@Nota", nota);
                cmd.Parameters.AddWithValue("@Feedback", string.IsNullOrWhiteSpace(TxtFeedback.Text) ? (object)DBNull.Value : TxtFeedback.Text.Trim());
                c.Open();
                cmd.ExecuteNonQuery();
            }

            PainelAvaliacao.Visible = false;
            MostrarMensagem("Avaliacao guardada.", false);
            CarregarEntregas();
        }

        protected void BtnCancelarAvaliacao_Click(object sender, EventArgs e)
        {
            PainelAvaliacao.Visible = false;
        }

        private int ProfessorId
        {
            get
            {
                int id;
                if (Session["ProfessorID"] != null && int.TryParse(Session["ProfessorID"].ToString(), out id)) return id;
                if (Session["UserId"] == null) throw new InvalidOperationException("Sessao terminada.");
                using (SqlConnection c = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand("SELECT Id FROM dbo.Professor WHERE UserId=@UserId AND Ativo=1", c))
                {
                    cmd.Parameters.AddWithValue("@UserId", new Guid(Session["UserId"].ToString()));
                    c.Open();
                    object value = cmd.ExecuteScalar();
                    if (value == null) throw new InvalidOperationException("Professor nao encontrado.");
                    id = Convert.ToInt32(value);
                    Session["ProfessorID"] = id;
                    return id;
                }
            }
        }

        private int TurmaSelecionada
        {
            get { int id; return int.TryParse(DdlTurmas.SelectedValue, out id) ? id : 0; }
        }

        private void CarregarTurmas()
        {
            const string sql = @"
                SELECT DISTINCT td.TurmaId,
                    CAST(t.AnoEscolaridade AS varchar(2))+'.º'+t.CodigoTurma+' - '+e.Nome AS Nome
                FROM dbo.TurmaDisciplinaProfessor tdp
                INNER JOIN dbo.TurmaDisciplina td ON td.Id=tdp.TurmaDisciplinaId
                INNER JOIN dbo.Turma t ON t.Id=td.TurmaId
                INNER JOIN dbo.Escola e ON e.Id=t.EscolaId
                WHERE tdp.ProfessorId=@ProfessorId AND tdp.Ate IS NULL
                ORDER BY Nome;";
            DataTable dt = new DataTable();
            using (SqlConnection c = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, c))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddWithValue("@ProfessorId", ProfessorId);
                da.Fill(dt);
            }
            DdlTurmas.DataSource = dt;
            DdlTurmas.DataTextField = "Nome";
            DdlTurmas.DataValueField = "TurmaId";
            DdlTurmas.DataBind();
        }

        private void SelecionarTurmaDaUrl()
        {
            if (DdlTurmas.Items.Count == 0)
            {
                HdnEvents.Value = "[]";
                return;
            }

            int turmaId;
            if (int.TryParse(Request.QueryString["turma"], out turmaId) &&
                DdlTurmas.Items.FindByValue(turmaId.ToString()) != null &&
                ProfessorTemTurma(turmaId))
            {
                DdlTurmas.SelectedValue = turmaId.ToString();
                return;
            }

            DdlTurmas.SelectedIndex = 0;
        }

        private void CarregarTudo()
        {
            CarregarEventos();
            CarregarEntregas();
        }

        private void CarregarEventos()
        {
            int turmaId = TurmaSelecionada;
            DataTable dt = new DataTable();
            if (turmaId > 0)
            {
                using (SqlConnection c = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand("SELECT Id,Tipo,Titulo,DataHora FROM dbo.Evento WHERE TurmaId=@TurmaId ORDER BY DataHora DESC", c))
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    cmd.Parameters.AddWithValue("@TurmaId", turmaId);
                    da.Fill(dt);
                }
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

        private void CarregarEntregas()
        {
            int turmaId = TurmaSelecionada;
            DataTable dt = new DataTable();
            if (turmaId > 0)
            {
                const string sql = @"
                    SELECT ee.Id, a.NomeCompleto AS Aluno, ev.Titulo AS Evento, ee.NomeFicheiro,
                           ee.CaminhoFicheiro, ee.Nota
                    FROM dbo.EventoEntrega ee
                    INNER JOIN dbo.Evento ev ON ev.Id=ee.EventoId
                    INNER JOIN dbo.Aluno a ON a.Id=ee.AlunoId
                    WHERE ev.TurmaId=@TurmaId
                    ORDER BY ee.CreatedAt DESC;";
                using (SqlConnection c = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand(sql, c))
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    cmd.Parameters.AddWithValue("@TurmaId", turmaId);
                    da.Fill(dt);
                }
            }
            GridEntregas.DataSource = dt;
            GridEntregas.DataBind();
        }

        private int InserirEvento(int turmaId, DateTime dataHora)
        {
            const string sql = @"INSERT INTO dbo.Evento(AlunoId,TurmaId,Titulo,Tipo,DataHora)
                VALUES(NULL,@TurmaId,@Titulo,@Tipo,@DataHora); SELECT CAST(SCOPE_IDENTITY() AS int);";
            using (SqlConnection c = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, c))
            {
                ParametrosEvento(cmd, turmaId, dataHora);
                c.Open();
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        private void AtualizarEvento(int eventoId, int turmaId, DateTime dataHora)
        {
            const string sql = "UPDATE dbo.Evento SET Titulo=@Titulo,Tipo=@Tipo,DataHora=@DataHora WHERE Id=@Id AND TurmaId=@TurmaId";
            using (SqlConnection c = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, c))
            {
                ParametrosEvento(cmd, turmaId, dataHora);
                cmd.Parameters.AddWithValue("@Id", eventoId);
                c.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private void ParametrosEvento(SqlCommand cmd, int turmaId, DateTime dataHora)
        {
            cmd.Parameters.AddWithValue("@TurmaId", turmaId);
            cmd.Parameters.AddWithValue("@Titulo", TxtTitulo.Text.Trim());
            cmd.Parameters.AddWithValue("@Tipo", DdlTipo.SelectedValue);
            cmd.Parameters.AddWithValue("@DataHora", dataHora);
        }

        private void CarregarEventoParaFormulario(int eventoId, int turmaId)
        {
            using (SqlConnection c = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("SELECT Id,Tipo,Titulo,DataHora FROM dbo.Evento WHERE Id=@Id AND TurmaId=@TurmaId", c))
            {
                cmd.Parameters.AddWithValue("@Id", eventoId);
                cmd.Parameters.AddWithValue("@TurmaId", turmaId);
                c.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    if (!r.Read()) return;
                    HdnEventoId.Value = r["Id"].ToString();
                    DdlTipo.SelectedValue = r["Tipo"].ToString();
                    TxtTitulo.Text = r["Titulo"].ToString();
                    TxtDataHora.Text = Convert.ToDateTime(r["DataHora"]).ToString("yyyy-MM-ddTHH:mm");
                }
            }
        }

        private void ApagarEvento(int eventoId, int turmaId)
        {
            using (SqlConnection c = new SqlConnection(_connectionString))
            {
                c.Open();
                SqlTransaction t = c.BeginTransaction();
                try
                {
                    Executar(c, t, "DELETE ee FROM dbo.EventoEntrega ee INNER JOIN dbo.Evento ev ON ev.Id=ee.EventoId WHERE ev.Id=@Id AND ev.TurmaId=@TurmaId", eventoId, turmaId);
                    Executar(c, t, "DELETE FROM dbo.EventoAnexo WHERE EventoId=@Id", eventoId, turmaId);
                    Executar(c, t, "DELETE FROM dbo.Evento WHERE Id=@Id AND TurmaId=@TurmaId", eventoId, turmaId);
                    t.Commit();
                }
                catch { t.Rollback(); throw; }
            }
        }

        private void GuardarAnexoProfessor(int eventoId)
        {
            if (FileAnexo.PostedFile.ContentLength > 10 * 1024 * 1024) throw new InvalidOperationException("O anexo excede 10 MB.");
            string pasta = Server.MapPath("~/uploads/eventos/");
            Directory.CreateDirectory(pasta);
            string original = Path.GetFileName(FileAnexo.FileName);
            string nome = Guid.NewGuid().ToString("N") + "_" + original;
            FileAnexo.SaveAs(Path.Combine(pasta, nome));
            using (SqlConnection c = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("INSERT INTO dbo.EventoAnexo(EventoId,NomeFicheiro,CaminhoFicheiro) VALUES(@EventoId,@Nome,@Caminho)", c))
            {
                cmd.Parameters.AddWithValue("@EventoId", eventoId);
                cmd.Parameters.AddWithValue("@Nome", original);
                cmd.Parameters.AddWithValue("@Caminho", ResolveUrl("~/uploads/eventos/" + nome));
                c.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private void CarregarEntrega(int entregaId)
        {
            const string sql = @"
                SELECT ee.Id, ee.Nota, ee.Feedback
                FROM dbo.EventoEntrega ee
                INNER JOIN dbo.Evento ev ON ev.Id=ee.EventoId
                WHERE ee.Id=@Id AND ev.TurmaId=@TurmaId;";
            using (SqlConnection c = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, c))
            {
                cmd.Parameters.AddWithValue("@Id", entregaId);
                cmd.Parameters.AddWithValue("@TurmaId", TurmaSelecionada);
                c.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    if (!r.Read()) return;
                    HdnEntregaId.Value = r["Id"].ToString();
                    TxtNota.Text = r["Nota"] == DBNull.Value ? "" : r["Nota"].ToString();
                    TxtFeedback.Text = r["Feedback"] == DBNull.Value ? "" : r["Feedback"].ToString();
                    PainelAvaliacao.Visible = true;
                }
            }
        }

        private bool ProfessorTemTurma(int turmaId)
        {
            const string sql = @"SELECT COUNT(1) FROM dbo.TurmaDisciplinaProfessor tdp
                INNER JOIN dbo.TurmaDisciplina td ON td.Id=tdp.TurmaDisciplinaId
                WHERE tdp.ProfessorId=@ProfessorId AND td.TurmaId=@TurmaId AND tdp.Ate IS NULL";
            using (SqlConnection c = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, c))
            {
                cmd.Parameters.AddWithValue("@ProfessorId", ProfessorId);
                cmd.Parameters.AddWithValue("@TurmaId", turmaId);
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

        private void Executar(SqlConnection c, SqlTransaction t, string sql, int id, int turmaId)
        {
            using (SqlCommand cmd = new SqlCommand(sql, c, t))
            {
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@TurmaId", turmaId);
                cmd.ExecuteNonQuery();
            }
        }

        private void LimparEvento()
        {
            HdnEventoId.Value = "";
            TxtTitulo.Text = "";
            TxtDataHora.Text = "";
            DdlTipo.SelectedIndex = 0;
        }

        private void MostrarMensagem(string texto, bool erro)
        {
            LblMensagem.Text = texto;
            LblMensagem.CssClass = erro ? "alert alert-warning d-block" : "alert alert-success d-block";
            LblMensagem.Visible = true;
        }
    }
}
