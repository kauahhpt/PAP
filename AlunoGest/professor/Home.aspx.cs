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
                MostrarMensagem("Não foi possível carregar a página: " + ex.Message, true);
            }
        }

        protected void DdlTurmas_SelectedIndexChanged(object sender, EventArgs e)
        {
            int turmaId = TurmaSelecionada;
            if (turmaId > 0)
            {
                Response.Redirect("~/professor/Home.aspx?turma=" + turmaId);
            }
        }

        private int ProfessorId
        {
            get
            {
                int id;

                if (Session["ProfessorID"] != null && int.TryParse(Session["ProfessorID"].ToString(), out id))
                {
                    return id;
                }

                if (Session["UserId"] == null)
                {
                    throw new InvalidOperationException("A sessão terminou.");
                }

                Guid userId;
                if (!Guid.TryParse(Session["UserId"].ToString(), out userId))
                {
                    throw new InvalidOperationException("O utilizador da sessão é inválido.");
                }

                const string sql = @"
                    SELECT Id
                    FROM dbo.Professor
                    WHERE UserId = @UserId AND Ativo = 1;";

                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add("@UserId", SqlDbType.UniqueIdentifier).Value = userId;
                    conn.Open();

                    object valor = cmd.ExecuteScalar();
                    if (valor == null || valor == DBNull.Value)
                    {
                        throw new InvalidOperationException("Professor não encontrado.");
                    }

                    id = Convert.ToInt32(valor);
                    Session["ProfessorID"] = id;
                    return id;
                }
            }
        }

        private int TurmaSelecionada
        {
            get
            {
                int id;
                return int.TryParse(DdlTurmas.SelectedValue, out id) ? id : 0;
            }
        }

        private void CarregarTurmas()
        {
            const string sql = @"
                SELECT DISTINCT
                    td.TurmaId,
                    CAST(t.AnoEscolaridade AS varchar(2)) + '.º' + t.CodigoTurma + ' - ' + e.Nome AS Nome
                FROM dbo.TurmaDisciplinaProfessor tdp
                INNER JOIN dbo.TurmaDisciplina td ON td.Id = tdp.TurmaDisciplinaId
                INNER JOIN dbo.Turma t ON t.Id = td.TurmaId
                INNER JOIN dbo.Escola e ON e.Id = t.EscolaId
                WHERE tdp.ProfessorId = @ProfessorId
                  AND tdp.Ate IS NULL
                ORDER BY Nome;";

            DataTable tabela = new DataTable();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.Add("@ProfessorId", SqlDbType.Int).Value = ProfessorId;
                adapter.Fill(tabela);
            }

            DdlTurmas.DataSource = tabela;
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
            bool encontrouTurma = int.TryParse(Request.QueryString["turma"], out turmaId);

            if (!encontrouTurma)
            {
                encontrouTurma = int.TryParse(Request.QueryString["id"], out turmaId);
            }

            if (encontrouTurma && DdlTurmas.Items.FindByValue(turmaId.ToString()) != null && ProfessorTemTurma(turmaId))
            {
                DdlTurmas.SelectedValue = turmaId.ToString();
                return;
            }

            DdlTurmas.SelectedIndex = 0;
        }

        private void CarregarTudo()
        {
            CarregarResumoTurma();
            CarregarPublicacoes();
            CarregarEventos();
            CarregarEntregas();
        }

        private void CarregarResumoTurma()
        {
            int turmaId = TurmaSelecionada;

            if (turmaId == 0)
            {
                LblResumoTurma.Text = "Nenhuma turma selecionada.";
                return;
            }

            const string sql = @"
                SELECT
                    CAST(t.AnoEscolaridade AS varchar(2)) + '.º' + t.CodigoTurma AS Turma,
                    e.Nome AS Escola
                FROM dbo.Turma t
                INNER JOIN dbo.Escola e ON e.Id = t.EscolaId
                WHERE t.Id = @TurmaId;";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add("@TurmaId", SqlDbType.Int).Value = turmaId;
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        LblResumoTurma.Text = "Turma " + reader["Turma"].ToString() + " | " + reader["Escola"].ToString();
                    }
                }
            }
        }

        private void CarregarPublicacoes()
        {
            int turmaId = TurmaSelecionada;
            DataTable tabela = new DataTable();

            if (turmaId == 0)
            {
                RepeaterPublicacoes.DataSource = tabela;
                RepeaterPublicacoes.DataBind();
                PainelSemPublicacoes.Visible = true;
                return;
            }

            const string sql = @"
                SELECT
                    p.Id,
                    p.Titulo,
                    p.Conteudo,
                    p.Tipo,
                    p.CreatedAt,
                    a.NomeCompleto,
                    a.Foto,
                    COUNT(pl.Id) AS TotalLikes
                FROM dbo.Publicacao p
                INNER JOIN dbo.Aluno a ON a.Id = p.AlunoId
                LEFT JOIN dbo.PublicacaoLike pl ON pl.PublicacaoId = p.Id
                WHERE p.TurmaId = @TurmaId
                  AND p.PublicaParaTurma = 1
                GROUP BY p.Id, p.Titulo, p.Conteudo, p.Tipo, p.CreatedAt, a.NomeCompleto, a.Foto
                ORDER BY p.CreatedAt DESC;";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.Add("@TurmaId", SqlDbType.Int).Value = turmaId;
                adapter.Fill(tabela);
            }

            RepeaterPublicacoes.DataSource = tabela;
            RepeaterPublicacoes.DataBind();
            PainelSemPublicacoes.Visible = tabela.Rows.Count == 0;
        }

        protected bool TemFotoPublicacao(object foto)
        {
            if (foto == null || foto == DBNull.Value)
            {
                return false;
            }

            return !string.IsNullOrWhiteSpace(foto.ToString().Trim());
        }

        protected string ObterFotoPublicacao(object foto)
        {
            if (!TemFotoPublicacao(foto))
            {
                return "";
            }

            string caminho = foto.ToString().Trim();

            if (caminho.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                caminho.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                return caminho;
            }

            if (caminho.StartsWith("~/"))
            {
                return ResolveUrl(caminho);
            }

            if (caminho.StartsWith("/"))
            {
                return ResolveUrl("~" + caminho);
            }

            return ResolveUrl("~/" + caminho);
        }

        protected string ObterInicial(object nome)
        {
            if (nome == null || nome == DBNull.Value)
            {
                return "?";
            }

            string texto = nome.ToString().Trim();
            if (string.IsNullOrWhiteSpace(texto))
            {
                return "?";
            }

            return texto.Substring(0, 1).ToUpper();
        }

        protected void BtnGuardar_Click(object sender, EventArgs e)
        {
            int turmaId = TurmaSelecionada;

            if (turmaId == 0 || !ProfessorTemTurma(turmaId))
            {
                MostrarMensagem("Escolha uma turma válida.", true);
                return;
            }

            if (string.IsNullOrWhiteSpace(TxtTitulo.Text))
            {
                MostrarMensagem("Indique o título.", true);
                return;
            }

            DateTime dataHora;
            if (!DateTime.TryParse(TxtDataHora.Text, out dataHora))
            {
                MostrarMensagem("Indique uma data válida.", true);
                return;
            }

            try
            {
                int eventoId;

                if (string.IsNullOrEmpty(HdnEventoId.Value))
                {
                    eventoId = InserirEvento(turmaId, dataHora);
                }
                else
                {
                    eventoId = Convert.ToInt32(HdnEventoId.Value);
                    AtualizarEvento(eventoId, turmaId, dataHora);
                }

                if (FileAnexo.HasFile)
                {
                    GuardarAnexoProfessor(eventoId);
                }

                MostrarMensagem("Evento guardado.", false);
                LimparEvento();
                CarregarTudo();
            }
            catch (Exception ex)
            {
                MostrarMensagem("Não foi possível guardar: " + ex.Message, true);
            }
        }

        protected void BtnLimpar_Click(object sender, EventArgs e)
        {
            LimparEvento();
        }

        private void CarregarEventos()
        {
            int turmaId = TurmaSelecionada;
            DataTable tabela = new DataTable();

            if (turmaId > 0)
            {
                const string sql = @"
                    SELECT Id, Tipo, Titulo, DataHora
                    FROM dbo.Evento
                    WHERE TurmaId = @TurmaId
                    ORDER BY DataHora DESC;";

                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    cmd.Parameters.Add("@TurmaId", SqlDbType.Int).Value = turmaId;
                    adapter.Fill(tabela);
                }
            }

            GridEventos.DataSource = tabela;
            GridEventos.DataBind();

            List<object> eventos = new List<object>();

            foreach (DataRow row in tabela.Rows)
            {
                string tipo = row["Tipo"].ToString();
                string cor;

                if (tipo == "Teste")
                {
                    cor = "#1d4ed8";
                }
                else if (tipo == "Trabalho")
                {
                    cor = "#2563eb";
                }
                else
                {
                    cor = "#64748b";
                }

                eventos.Add(new
                {
                    id = row["Id"],
                    title = row["Titulo"].ToString(),
                    start = Convert.ToDateTime(row["DataHora"]).ToString("yyyy-MM-ddTHH:mm:ss"),
                    color = cor
                });
            }

            HdnEvents.Value = new JavaScriptSerializer().Serialize(eventos);
        }

        protected void GridEventos_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int index;
            if (!int.TryParse(e.CommandArgument.ToString(), out index) || index < 0 || index >= GridEventos.Rows.Count)
            {
                return;
            }

            int eventoId = Convert.ToInt32(GridEventos.DataKeys[index].Value);
            int turmaId = TurmaSelecionada;

            if (!ProfessorTemTurma(turmaId))
            {
                return;
            }

            if (e.CommandName == "EditarEvento")
            {
                CarregarEventoParaFormulario(eventoId, turmaId);
            }
            else if (e.CommandName == "ApagarEvento")
            {
                try
                {
                    ApagarEvento(eventoId, turmaId);
                    MostrarMensagem("Evento apagado.", false);
                    LimparEvento();
                    CarregarTudo();
                }
                catch (Exception ex)
                {
                    MostrarMensagem("Não foi possível apagar o evento: " + ex.Message, true);
                }
            }
        }

        private int InserirEvento(int turmaId, DateTime dataHora)
        {
            const string sql = @"
                INSERT INTO dbo.Evento (AlunoId, TurmaId, Titulo, Tipo, DataHora)
                VALUES (NULL, @TurmaId, @Titulo, @Tipo, @DataHora);

                SELECT CAST(SCOPE_IDENTITY() AS int);";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                ParametrosEvento(cmd, turmaId, dataHora);
                conn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        private void AtualizarEvento(int eventoId, int turmaId, DateTime dataHora)
        {
            const string sql = @"
                UPDATE dbo.Evento
                SET Titulo = @Titulo,
                    Tipo = @Tipo,
                    DataHora = @DataHora
                WHERE Id = @Id AND TurmaId = @TurmaId;";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                ParametrosEvento(cmd, turmaId, dataHora);
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = eventoId;
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private void ParametrosEvento(SqlCommand cmd, int turmaId, DateTime dataHora)
        {
            cmd.Parameters.Add("@TurmaId", SqlDbType.Int).Value = turmaId;
            cmd.Parameters.Add("@Titulo", SqlDbType.NVarChar, 200).Value = TxtTitulo.Text.Trim();
            cmd.Parameters.Add("@Tipo", SqlDbType.NVarChar, 50).Value = DdlTipo.SelectedValue;
            cmd.Parameters.Add("@DataHora", SqlDbType.DateTime).Value = dataHora;
        }

        private void CarregarEventoParaFormulario(int eventoId, int turmaId)
        {
            const string sql = @"
                SELECT Id, Tipo, Titulo, DataHora
                FROM dbo.Evento
                WHERE Id = @Id AND TurmaId = @TurmaId;";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = eventoId;
                cmd.Parameters.Add("@TurmaId", SqlDbType.Int).Value = turmaId;
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        return;
                    }

                    HdnEventoId.Value = reader["Id"].ToString();
                    DdlTipo.SelectedValue = reader["Tipo"].ToString();
                    TxtTitulo.Text = reader["Titulo"].ToString();
                    TxtDataHora.Text = Convert.ToDateTime(reader["DataHora"]).ToString("yyyy-MM-ddTHH:mm");
                }
            }
        }

        private void ApagarEvento(int eventoId, int turmaId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlTransaction transacao = conn.BeginTransaction();

                try
                {
                    Executar(conn, transacao, @"
                        DELETE ee
                        FROM dbo.EventoEntrega ee
                        INNER JOIN dbo.Evento ev ON ev.Id = ee.EventoId
                        WHERE ev.Id = @Id AND ev.TurmaId = @TurmaId;
                    ", eventoId, turmaId);

                    Executar(conn, transacao, @"
                        DELETE FROM dbo.EventoAnexo
                        WHERE EventoId = @Id;
                    ", eventoId, turmaId);

                    Executar(conn, transacao, @"
                        DELETE FROM dbo.Evento
                        WHERE Id = @Id AND TurmaId = @TurmaId;
                    ", eventoId, turmaId);

                    transacao.Commit();
                }
                catch
                {
                    transacao.Rollback();
                    throw;
                }
            }
        }

        private void GuardarAnexoProfessor(int eventoId)
        {
            if (FileAnexo.PostedFile.ContentLength > 10 * 1024 * 1024)
            {
                throw new InvalidOperationException("O anexo excede 10 MB.");
            }

            string pasta = Server.MapPath("~/uploads/eventos/");
            Directory.CreateDirectory(pasta);

            string original = Path.GetFileName(FileAnexo.FileName);
            string nome = Guid.NewGuid().ToString("N") + "_" + original;
            FileAnexo.SaveAs(Path.Combine(pasta, nome));

            const string sql = @"
                INSERT INTO dbo.EventoAnexo (EventoId, NomeFicheiro, CaminhoFicheiro)
                VALUES (@EventoId, @Nome, @Caminho);";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add("@EventoId", SqlDbType.Int).Value = eventoId;
                cmd.Parameters.Add("@Nome", SqlDbType.NVarChar, 255).Value = original;
                cmd.Parameters.Add("@Caminho", SqlDbType.NVarChar, 500).Value = ResolveUrl("~/uploads/eventos/" + nome);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private void CarregarEntregas()
        {
            int turmaId = TurmaSelecionada;
            DataTable tabela = new DataTable();

            if (turmaId > 0)
            {
                const string sql = @"
                    SELECT
                        ee.Id,
                        a.NomeCompleto AS Aluno,
                        ev.Titulo AS Evento,
                        ee.NomeFicheiro,
                        ee.CaminhoFicheiro,
                        ee.Nota
                    FROM dbo.EventoEntrega ee
                    INNER JOIN dbo.Evento ev ON ev.Id = ee.EventoId
                    INNER JOIN dbo.Aluno a ON a.Id = ee.AlunoId
                    WHERE ev.TurmaId = @TurmaId
                    ORDER BY ee.CreatedAt DESC;";

                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    cmd.Parameters.Add("@TurmaId", SqlDbType.Int).Value = turmaId;
                    adapter.Fill(tabela);
                }
            }

            GridEntregas.DataSource = tabela;
            GridEntregas.DataBind();
        }

        protected void GridEntregas_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int index;
            if (e.CommandName != "AvaliarEntrega" ||
                !int.TryParse(e.CommandArgument.ToString(), out index) ||
                index < 0 || index >= GridEntregas.Rows.Count)
            {
                return;
            }

            int entregaId = Convert.ToInt32(GridEntregas.DataKeys[index].Value);
            CarregarEntrega(entregaId);
        }

        private void CarregarEntrega(int entregaId)
        {
            const string sql = @"
                SELECT ee.Id, ee.Nota, ee.Feedback
                FROM dbo.EventoEntrega ee
                INNER JOIN dbo.Evento ev ON ev.Id = ee.EventoId
                WHERE ee.Id = @Id AND ev.TurmaId = @TurmaId;";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = entregaId;
                cmd.Parameters.Add("@TurmaId", SqlDbType.Int).Value = TurmaSelecionada;
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        return;
                    }

                    HdnEntregaId.Value = reader["Id"].ToString();
                    TxtNota.Text = reader["Nota"] == DBNull.Value ? "" : reader["Nota"].ToString();
                    TxtFeedback.Text = reader["Feedback"] == DBNull.Value ? "" : reader["Feedback"].ToString();
                    PainelAvaliacao.Visible = true;
                }
            }
        }

        protected void BtnGuardarAvaliacao_Click(object sender, EventArgs e)
        {
            int entregaId;
            decimal nota;

            if (!int.TryParse(HdnEntregaId.Value, out entregaId))
            {
                MostrarMensagem("Escolha uma entrega.", true);
                return;
            }

            if (!decimal.TryParse(TxtNota.Text.Replace(".", ","), out nota) || nota < 0 || nota > 20)
            {
                MostrarMensagem("A nota deve estar entre 0 e 20.", true);
                return;
            }

            const string sql = @"
                UPDATE ee
                SET Nota = @Nota,
                    Feedback = @Feedback,
                    AvaliadoEm = SYSDATETIME()
                FROM dbo.EventoEntrega ee
                INNER JOIN dbo.Evento ev ON ev.Id = ee.EventoId
                WHERE ee.Id = @Id AND ev.TurmaId = @TurmaId;";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = entregaId;
                cmd.Parameters.Add("@TurmaId", SqlDbType.Int).Value = TurmaSelecionada;
                cmd.Parameters.Add("@Nota", SqlDbType.Decimal).Value = nota;
                cmd.Parameters.Add("@Feedback", SqlDbType.NVarChar, 1000).Value =
                    string.IsNullOrWhiteSpace(TxtFeedback.Text) ? (object)DBNull.Value : TxtFeedback.Text.Trim();

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            PainelAvaliacao.Visible = false;
            MostrarMensagem("Avaliação guardada.", false);
            CarregarEntregas();
        }

        protected void BtnCancelarAvaliacao_Click(object sender, EventArgs e)
        {
            PainelAvaliacao.Visible = false;
        }

        private bool ProfessorTemTurma(int turmaId)
        {
            if (turmaId <= 0)
            {
                return false;
            }

            const string sql = @"
                SELECT COUNT(1)
                FROM dbo.TurmaDisciplinaProfessor tdp
                INNER JOIN dbo.TurmaDisciplina td ON td.Id = tdp.TurmaDisciplinaId
                WHERE tdp.ProfessorId = @ProfessorId
                  AND td.TurmaId = @TurmaId
                  AND tdp.Ate IS NULL;";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add("@ProfessorId", SqlDbType.Int).Value = ProfessorId;
                cmd.Parameters.Add("@TurmaId", SqlDbType.Int).Value = turmaId;
                conn.Open();

                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        private void GarantirTabelaEntregas()
        {
            const string sql = @"
                IF OBJECT_ID('dbo.EventoEntrega', 'U') IS NULL
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
                END;";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private void Executar(SqlConnection conn, SqlTransaction transacao, string sql, int id, int turmaId)
        {
            using (SqlCommand cmd = new SqlCommand(sql, conn, transacao))
            {
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                cmd.Parameters.Add("@TurmaId", SqlDbType.Int).Value = turmaId;
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