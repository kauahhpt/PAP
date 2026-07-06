using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI.WebControls;

namespace AlunoGest.professor
{
    public partial class turma : System.Web.UI.Page
    {
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        #region Propriedades

        private int TurmaId
        {
            get
            {
                int id;
                return int.TryParse(Request.QueryString["id"], out id) ? id : 0;
            }
        }

        private int ProfessorId
        {
            get
            {
                int professorId;

                if (Session["ProfessorID"] != null && int.TryParse(Session["ProfessorID"].ToString(), out professorId))
                {
                    return professorId;
                }

                if (Session["UserId"] == null)
                {
                    throw new InvalidOperationException("A sessão terminou. Inicie sessão novamente.");
                }

                Guid userId;
                if (!Guid.TryParse(Session["UserId"].ToString(), out userId))
                {
                    throw new InvalidOperationException("O identificador do utilizador é inválido.");
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

                    object resultado = cmd.ExecuteScalar();
                    if (resultado == null || resultado == DBNull.Value)
                    {
                        throw new InvalidOperationException("Professor não encontrado.");
                    }

                    professorId = Convert.ToInt32(resultado);
                    Session["ProfessorID"] = professorId;
                    return professorId;
                }
            }
        }

        #endregion

        #region Página

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (TurmaId == 0 || !ProfessorTemAcessoATurma())
                {
                    Response.Redirect("~/professor/dashboard.aspx");
                    return;
                }

                if (!IsPostBack)
                {
                    CarregarPagina();
                }
            }
            catch (Exception ex)
            {
                MostrarMensagem("Não foi possível carregar a turma: " + ex.Message, true);
            }
        }

        private void CarregarPagina()
        {
            CarregarCabecalho();
            CarregarResumo();
            CarregarAlunos();
            CarregarProfessores();
            CarregarAtividades();
        }

        protected void TxtPesquisaAluno_TextChanged(object sender, EventArgs e)
        {
            CarregarAlunos();
        }

        #endregion

        #region Acesso à turma

        private bool ProfessorTemAcessoATurma()
        {
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
                cmd.Parameters.Add("@TurmaId", SqlDbType.Int).Value = TurmaId;
                conn.Open();

                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        #endregion

        #region Cabeçalho e resumo

        private void CarregarCabecalho()
        {
            const string sql = @"
                SELECT
                    CAST(t.AnoEscolaridade AS varchar(2)) + '.º' + t.CodigoTurma AS Turma,
                    e.Nome AS Escola,
                    al.Descricao AS AnoLetivo
                FROM dbo.Turma t
                INNER JOIN dbo.Escola e ON e.Id = t.EscolaId
                INNER JOIN dbo.AnoLetivo al ON al.Id = t.AnoLetivoId
                WHERE t.Id = @TurmaId;

                SELECT STUFF
                (
                    (
                        SELECT DISTINCT ', ' + d.Nome
                        FROM dbo.TurmaDisciplina td
                        INNER JOIN dbo.TurmaDisciplinaProfessor tdp ON tdp.TurmaDisciplinaId = td.Id
                        INNER JOIN dbo.Disciplina d ON d.Id = td.DisciplinaId
                        WHERE td.TurmaId = @TurmaId
                          AND tdp.ProfessorId = @ProfessorId
                          AND tdp.Ate IS NULL
                        FOR XML PATH(''), TYPE
                    ).value('.', 'nvarchar(max)'), 1, 2, ''
                ) AS Disciplinas;";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add("@TurmaId", SqlDbType.Int).Value = TurmaId;
                cmd.Parameters.Add("@ProfessorId", SqlDbType.Int).Value = ProfessorId;
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        LblTurma.Text = "Turma " + reader["Turma"].ToString();
                        LblDetalhes.Text = reader["Escola"].ToString() + " | " + reader["AnoLetivo"].ToString();
                    }

                    if (reader.NextResult() && reader.Read())
                    {
                        LblDisciplinas.Text = reader["Disciplinas"] == DBNull.Value
                            ? "Sem disciplina atribuída"
                            : reader["Disciplinas"].ToString();
                    }
                }
            }
        }

        private void CarregarResumo()
        {
            const string sql = @"
                SELECT
                    (
                        SELECT COUNT(*)
                        FROM dbo.AlunoTurma at2
                        INNER JOIN dbo.Aluno a ON a.Id = at2.AlunoId
                        WHERE at2.TurmaId = @TurmaId
                          AND at2.Ate IS NULL
                          AND a.Ativo = 1
                    ) AS TotalAlunos,
                    (
                        SELECT COUNT(DISTINCT tdp.ProfessorId)
                        FROM dbo.TurmaDisciplina td
                        INNER JOIN dbo.TurmaDisciplinaProfessor tdp ON tdp.TurmaDisciplinaId = td.Id
                        INNER JOIN dbo.Professor p ON p.Id = tdp.ProfessorId
                        WHERE td.TurmaId = @TurmaId
                          AND tdp.Ate IS NULL
                          AND p.Ativo = 1
                    ) AS TotalProfessores,
                    (
                        SELECT COUNT(*)
                        FROM dbo.Evento ev
                        WHERE ev.TurmaId = @TurmaId
                          AND ev.DataHora >= GETDATE()
                    ) AS TotalAtividades;";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add("@TurmaId", SqlDbType.Int).Value = TurmaId;
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        LblTotalAlunos.Text = reader["TotalAlunos"].ToString();
                        LblTotalProfessores.Text = reader["TotalProfessores"].ToString();
                        LblTotalAtividades.Text = reader["TotalAtividades"].ToString();
                    }
                }
            }
        }

        #endregion

        #region Alunos

        private void CarregarAlunos()
        {
            const string sql = @"
                SELECT a.Id, a.NomeCompleto, a.NumeroProcesso, a.Email, a.Foto
                FROM dbo.AlunoTurma at2
                INNER JOIN dbo.Aluno a ON a.Id = at2.AlunoId
                WHERE at2.TurmaId = @TurmaId
                  AND at2.Ate IS NULL
                  AND a.Ativo = 1
                  AND (
                        @Pesquisa = ''
                        OR a.NomeCompleto LIKE '%' + @Pesquisa + '%'
                        OR a.NumeroProcesso LIKE '%' + @Pesquisa + '%'
                      )
                ORDER BY a.NomeCompleto;";

            DataTable tabela = new DataTable();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.Add("@TurmaId", SqlDbType.Int).Value = TurmaId;
                cmd.Parameters.Add("@Pesquisa", SqlDbType.NVarChar, 200).Value = TxtPesquisaAluno.Text.Trim();
                adapter.Fill(tabela);
            }

            RepeaterAlunos.DataSource = tabela;
            RepeaterAlunos.DataBind();
            PainelSemAlunos.Visible = tabela.Rows.Count == 0;
        }

        #endregion

        #region Professores

        private void CarregarProfessores()
        {
            const string sql = @"
                SELECT
                    p.Id,
                    p.Nome,
                    STUFF
                    (
                        (
                            SELECT DISTINCT ', ' + d2.Nome
                            FROM dbo.TurmaDisciplina td2
                            INNER JOIN dbo.TurmaDisciplinaProfessor tdp2 ON tdp2.TurmaDisciplinaId = td2.Id
                            INNER JOIN dbo.Disciplina d2 ON d2.Id = td2.DisciplinaId
                            WHERE td2.TurmaId = @TurmaId
                              AND tdp2.ProfessorId = p.Id
                              AND tdp2.Ate IS NULL
                            FOR XML PATH(''), TYPE
                        ).value('.', 'nvarchar(max)'), 1, 2, ''
                    ) AS Disciplinas
                FROM dbo.Professor p
                WHERE p.Ativo = 1
                  AND EXISTS
                  (
                        SELECT 1
                        FROM dbo.TurmaDisciplina td
                        INNER JOIN dbo.TurmaDisciplinaProfessor tdp ON tdp.TurmaDisciplinaId = td.Id
                        WHERE td.TurmaId = @TurmaId
                          AND tdp.ProfessorId = p.Id
                          AND tdp.Ate IS NULL
                  )
                ORDER BY p.Nome;";

            DataTable tabela = new DataTable();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.Add("@TurmaId", SqlDbType.Int).Value = TurmaId;
                adapter.Fill(tabela);
            }

            RepeaterProfessores.DataSource = tabela;
            RepeaterProfessores.DataBind();
            PainelSemProfessores.Visible = tabela.Rows.Count == 0;
        }

        #endregion

        #region Atividades

        protected void BtnNova_Click(object sender, EventArgs e)
        {
            LimparFormulario();
            PainelFormulario.Visible = true;
        }

        protected void BtnCancelar_Click(object sender, EventArgs e)
        {
            LimparFormulario();
            PainelFormulario.Visible = false;
        }

        protected void BtnGuardar_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid || !ProfessorTemAcessoATurma())
            {
                return;
            }

            DateTime dataHora;
            if (!DateTime.TryParse(TxtDataHora.Text, out dataHora))
            {
                MostrarMensagem("A data e hora não são válidas.", true);
                return;
            }

            try
            {
                int eventoId;

                if (string.IsNullOrWhiteSpace(HdnEventoId.Value))
                {
                    eventoId = InserirEvento(dataHora);
                }
                else
                {
                    eventoId = Convert.ToInt32(HdnEventoId.Value);
                    AtualizarEvento(eventoId, dataHora);
                }

                if (FileAnexo.HasFile)
                {
                    GuardarAnexo(eventoId);
                }

                LimparFormulario();
                PainelFormulario.Visible = false;
                CarregarAtividades();
                CarregarResumo();
                MostrarMensagem("Atividade guardada e publicada para a turma.", false);
            }
            catch (Exception ex)
            {
                MostrarMensagem("Não foi possível guardar: " + ex.Message, true);
            }
        }

        private void CarregarAtividades()
        {
            const string sql = @"
                SELECT
                    ev.Id,
                    ev.Tipo,
                    ev.Titulo,
                    ev.DataHora,
                    (
                        SELECT COUNT(*)
                        FROM dbo.EventoAnexo ea
                        WHERE ea.EventoId = ev.Id
                    ) AS TotalAnexos
                FROM dbo.Evento ev
                WHERE ev.TurmaId = @TurmaId
                ORDER BY ev.DataHora DESC;";

            DataTable tabela = new DataTable();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.Add("@TurmaId", SqlDbType.Int).Value = TurmaId;
                adapter.Fill(tabela);
            }

            GridAtividades.DataSource = tabela;
            GridAtividades.DataBind();
        }

        private int InserirEvento(DateTime dataHora)
        {
            const string sql = @"
                INSERT INTO dbo.Evento (AlunoId, TurmaId, Titulo, Tipo, DataHora)
                VALUES (NULL, @TurmaId, @Titulo, @Tipo, @DataHora);

                SELECT CAST(SCOPE_IDENTITY() AS int);";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                AdicionarParametrosEvento(cmd, dataHora);
                conn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        private void AtualizarEvento(int eventoId, DateTime dataHora)
        {
            const string sql = @"
                UPDATE dbo.Evento
                SET Titulo = @Titulo,
                    Tipo = @Tipo,
                    DataHora = @DataHora
                WHERE Id = @EventoId AND TurmaId = @TurmaId;";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                AdicionarParametrosEvento(cmd, dataHora);
                cmd.Parameters.Add("@EventoId", SqlDbType.Int).Value = eventoId;
                conn.Open();

                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new InvalidOperationException("Atividade não encontrada.");
                }
            }
        }

        private void AdicionarParametrosEvento(SqlCommand cmd, DateTime dataHora)
        {
            cmd.Parameters.Add("@TurmaId", SqlDbType.Int).Value = TurmaId;
            cmd.Parameters.Add("@Titulo", SqlDbType.NVarChar, 200).Value = TxtTitulo.Text.Trim();
            cmd.Parameters.Add("@Tipo", SqlDbType.NVarChar, 50).Value = DdlTipo.SelectedValue;
            cmd.Parameters.Add("@DataHora", SqlDbType.DateTime).Value = dataHora;
        }

        #endregion

        #region Grid de atividades

        protected void GridAtividades_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int index;
            if (!int.TryParse(e.CommandArgument.ToString(), out index) || index < 0 || index >= GridAtividades.Rows.Count)
            {
                return;
            }

            int eventoId = Convert.ToInt32(GridAtividades.DataKeys[index].Value);

            if (e.CommandName == "EditarAtividade")
            {
                CarregarEvento(eventoId);
            }
            else if (e.CommandName == "ApagarAtividade")
            {
                try
                {
                    ApagarEvento(eventoId);
                    CarregarAtividades();
                    CarregarResumo();
                    PainelFormulario.Visible = false;
                    MostrarMensagem("Atividade eliminada.", false);
                }
                catch (Exception ex)
                {
                    MostrarMensagem("Não foi possível eliminar: " + ex.Message, true);
                }
            }
        }

        private void CarregarEvento(int eventoId)
        {
            const string sql = @"
                SELECT Id, Titulo, Tipo, DataHora
                FROM dbo.Evento
                WHERE Id = @EventoId AND TurmaId = @TurmaId;";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add("@EventoId", SqlDbType.Int).Value = eventoId;
                cmd.Parameters.Add("@TurmaId", SqlDbType.Int).Value = TurmaId;
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        return;
                    }

                    HdnEventoId.Value = reader["Id"].ToString();
                    TxtTitulo.Text = reader["Titulo"].ToString();
                    DdlTipo.SelectedValue = reader["Tipo"].ToString();
                    TxtDataHora.Text = Convert.ToDateTime(reader["DataHora"]).ToString("yyyy-MM-ddTHH:mm");
                }
            }

            PainelFormulario.Visible = true;
            CarregarAnexos(eventoId);
        }

        private bool EventoPertenceATurma(int eventoId)
        {
            const string sql = @"
                SELECT COUNT(1)
                FROM dbo.Evento
                WHERE Id = @EventoId AND TurmaId = @TurmaId;";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add("@EventoId", SqlDbType.Int).Value = eventoId;
                cmd.Parameters.Add("@TurmaId", SqlDbType.Int).Value = TurmaId;
                conn.Open();

                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        #endregion

        #region Anexos

        protected void RepeaterAnexos_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "Remover")
            {
                return;
            }

            int eventoId;
            if (!int.TryParse(HdnEventoId.Value, out eventoId) || !EventoPertenceATurma(eventoId))
            {
                return;
            }

            int anexoId = Convert.ToInt32(e.CommandArgument);
            ApagarAnexo(anexoId, eventoId);
            CarregarAnexos(eventoId);
            MostrarMensagem("Anexo removido.", false);
        }

        private void GuardarAnexo(int eventoId)
        {
            if (FileAnexo.PostedFile.ContentLength > 10 * 1024 * 1024)
            {
                throw new InvalidOperationException("O anexo excede 10 MB.");
            }

            string nomeOriginal = Path.GetFileName(FileAnexo.FileName);
            string nomeGuardado = Guid.NewGuid().ToString("N") + "_" + nomeOriginal;
            string pastaFisica = Server.MapPath("~/uploads/eventos/");
            Directory.CreateDirectory(pastaFisica);

            string caminhoFisico = Path.Combine(pastaFisica, nomeGuardado);
            FileAnexo.SaveAs(caminhoFisico);

            string caminhoRelativo = ResolveUrl("~/uploads/eventos/" + nomeGuardado);

            const string sql = @"
                INSERT INTO dbo.EventoAnexo (EventoId, NomeFicheiro, CaminhoFicheiro)
                VALUES (@EventoId, @NomeFicheiro, @CaminhoFicheiro);";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add("@EventoId", SqlDbType.Int).Value = eventoId;
                cmd.Parameters.Add("@NomeFicheiro", SqlDbType.NVarChar, 255).Value = nomeOriginal;
                cmd.Parameters.Add("@CaminhoFicheiro", SqlDbType.NVarChar, 500).Value = caminhoRelativo;
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private void CarregarAnexos(int eventoId)
        {
            const string sql = @"
                SELECT Id, NomeFicheiro, CaminhoFicheiro
                FROM dbo.EventoAnexo
                WHERE EventoId = @EventoId
                ORDER BY CreatedAt;";

            DataTable tabela = new DataTable();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.Add("@EventoId", SqlDbType.Int).Value = eventoId;
                adapter.Fill(tabela);
            }

            RepeaterAnexos.DataSource = tabela;
            RepeaterAnexos.DataBind();
        }

        private void ApagarAnexo(int anexoId, int eventoId)
        {
            string caminhoFicheiro = null;

            const string sqlObter = @"
                SELECT CaminhoFicheiro
                FROM dbo.EventoAnexo
                WHERE Id = @AnexoId AND EventoId = @EventoId;";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(sqlObter, conn))
            {
                cmd.Parameters.Add("@AnexoId", SqlDbType.Int).Value = anexoId;
                cmd.Parameters.Add("@EventoId", SqlDbType.Int).Value = eventoId;
                conn.Open();

                object resultado = cmd.ExecuteScalar();
                if (resultado != null && resultado != DBNull.Value)
                {
                    caminhoFicheiro = resultado.ToString();
                }
            }

            if (string.IsNullOrWhiteSpace(caminhoFicheiro))
            {
                return;
            }

            const string sqlApagar = @"
                DELETE FROM dbo.EventoAnexo
                WHERE Id = @AnexoId AND EventoId = @EventoId;";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(sqlApagar, conn))
            {
                cmd.Parameters.Add("@AnexoId", SqlDbType.Int).Value = anexoId;
                cmd.Parameters.Add("@EventoId", SqlDbType.Int).Value = eventoId;
                conn.Open();
                cmd.ExecuteNonQuery();
            }

            ApagarFicheiro(caminhoFicheiro);
        }

        #endregion

        #region Eliminar atividade

        private void ApagarEvento(int eventoId)
        {
            if (!EventoPertenceATurma(eventoId))
            {
                return;
            }

            List<string> caminhos = new List<string>();

            const string sqlCaminhos = @"
                SELECT CaminhoFicheiro
                FROM dbo.EventoAnexo
                WHERE EventoId = @EventoId;";

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(sqlCaminhos, conn))
            {
                cmd.Parameters.Add("@EventoId", SqlDbType.Int).Value = eventoId;
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        caminhos.Add(reader["CaminhoFicheiro"].ToString());
                    }
                }
            }

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    using (SqlCommand cmdAnexos = new SqlCommand(
                        "DELETE FROM dbo.EventoAnexo WHERE EventoId = @EventoId;",
                        conn, transaction))
                    {
                        cmdAnexos.Parameters.Add("@EventoId", SqlDbType.Int).Value = eventoId;
                        cmdAnexos.ExecuteNonQuery();
                    }

                    using (SqlCommand cmdEvento = new SqlCommand(
                        "DELETE FROM dbo.Evento WHERE Id = @EventoId AND TurmaId = @TurmaId;",
                        conn, transaction))
                    {
                        cmdEvento.Parameters.Add("@EventoId", SqlDbType.Int).Value = eventoId;
                        cmdEvento.Parameters.Add("@TurmaId", SqlDbType.Int).Value = TurmaId;
                        cmdEvento.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }

            foreach (string caminho in caminhos)
            {
                ApagarFicheiro(caminho);
            }
        }

        private void ApagarFicheiro(string caminho)
        {
            try
            {
                string caminhoFisico = Server.MapPath(caminho);
                if (File.Exists(caminhoFisico))
                {
                    File.Delete(caminhoFisico);
                }
            }
            catch { }
        }

        #endregion

        #region Fotografias

        protected bool TemFoto(object foto)
        {
            if (foto == null || foto == DBNull.Value)
            {
                return false;
            }

            return !string.IsNullOrWhiteSpace(foto.ToString());
        }

        protected string ObterFoto(object foto)
        {
            if (!TemFoto(foto))
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

        #endregion

        #region Utilitários

        private void LimparFormulario()
        {
            HdnEventoId.Value = "";
            TxtTitulo.Text = "";
            TxtDataHora.Text = "";
            DdlTipo.SelectedIndex = 0;
            RepeaterAnexos.DataSource = null;
            RepeaterAnexos.DataBind();
        }

        private void MostrarMensagem(string texto, bool erro)
        {
            LblMensagem.Text = texto;
            LblMensagem.CssClass = erro ? "alert alert-warning d-block" : "alert alert-success d-block";
            LblMensagem.Visible = true;
        }

        #endregion
    }
}