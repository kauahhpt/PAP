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
    public partial class HomeAluno : System.Web.UI.Page
    {
        #region Campos

        private readonly string _ConnectionString =
            ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        #endregion

        #region Eventos de Página

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LblMensagem.Visible = false;
                PainelAnexos.Visible = false;
                CarregarEventos();
            }
        }

        #endregion

        #region Eventos de Controlos

        protected void ButtonAdicionar_Click(object sender, EventArgs e)
        {
            LimparMensagem();

            if (TxtTitulo.Text.Trim() == "")
            {
                MostrarMensagem("Indica o título do evento.");
                return;
            }

            if (TxtDataHora.Text.Trim() == "")
            {
                MostrarMensagem("Indica a data e hora do evento.");
                return;
            }

            DateTime DataHora;

            if (!DateTime.TryParse(TxtDataHora.Text.Trim(), out DataHora))
            {
                MostrarMensagem("Data e hora inválidas.");
                return;
            }

            int IdAluno = GetIdAluno();

            try
            {
                int IdEvento = InserirEvento(IdAluno, TxtTitulo.Text.Trim(), DdlTipo.SelectedValue, DataHora);

                if (FileAnexo.HasFile)
                    GuardarAnexo(IdEvento, FileAnexo);

                MostrarMensagem("Evento adicionado com sucesso.", false);
                LimparFormulario();
                CarregarEventos();
            }
            catch (Exception Ex)
            {
                MostrarMensagem("Erro ao adicionar evento: " + Ex.Message);
            }
        }

        protected void ButtonAtualizar_Click(object sender, EventArgs e)
        {
            LimparMensagem();

            if (HdnEventoId.Value == "")
            {
                MostrarMensagem("Selecione um evento no calendário.");
                return;
            }

            int IdEvento = Convert.ToInt32(HdnEventoId.Value);
            int IdAluno = GetIdAluno();

            // Se o postback foi disparado apenas para mostrar os anexos
            // (clique no calendário), não actualizamos nada — só recarregamos.
            if (Request["__EVENTARGUMENT"] == "mostrarAnexos")
            {
                CarregarAnexos(IdEvento);
                CarregarEventos();
                return;
            }

            if (TxtTitulo.Text.Trim() == "")
            {
                MostrarMensagem("Indica o título do evento.");
                return;
            }

            DateTime DataHora;

            if (!DateTime.TryParse(TxtDataHora.Text.Trim(), out DataHora))
            {
                MostrarMensagem("Data e hora inválidas.");
                return;
            }

            try
            {
                AtualizarEvento(IdEvento, IdAluno, TxtTitulo.Text.Trim(), DdlTipo.SelectedValue, DataHora);

                if (FileAnexo.HasFile)
                    GuardarAnexo(IdEvento, FileAnexo);

                MostrarMensagem("Evento atualizado com sucesso.", false);
                LimparFormulario();
                CarregarEventos();
            }
            catch (Exception Ex)
            {
                MostrarMensagem("Erro ao atualizar evento: " + Ex.Message);
            }
        }

        protected void ButtonApagar_Click(object sender, EventArgs e)
        {
            LimparMensagem();

            if (HdnEventoId.Value == "")
            {
                MostrarMensagem("Selecione um evento no calendário.");
                return;
            }

            int IdEvento = Convert.ToInt32(HdnEventoId.Value);
            int IdAluno = GetIdAluno();

            try
            {
                ApagarAnexosDoEvento(IdEvento);
                ApagarEvento(IdEvento, IdAluno);

                MostrarMensagem("Evento apagado com sucesso.", false);
                LimparFormulario();
                CarregarEventos();
            }
            catch (Exception Ex)
            {
                MostrarMensagem("Erro ao apagar evento: " + Ex.Message);
            }
        }

        protected void ButtonLimpar_Click(object sender, EventArgs e)
        {
            LimparFormulario();
            LimparMensagem();
        }

        protected void LinkButtonRemoverAnexo_Command(object sender, CommandEventArgs e)
        {
            LimparMensagem();

            int IdAnexo = Convert.ToInt32(e.CommandArgument);
            int IdEvento = Convert.ToInt32(HdnEventoId.Value);

            try
            {
                ApagarAnexo(IdAnexo);
                CarregarAnexos(IdEvento);

                MostrarMensagem("Anexo removido.", false);
            }
            catch (Exception Ex)
            {
                MostrarMensagem("Erro ao remover anexo: " + Ex.Message);
            }
        }

        #endregion

        #region Carregamentos

        private void CarregarEventos()
        {
            int IdAluno = GetIdAluno();
            List<object> Eventos = new List<object>();

            string Sql = @"
                -- Eventos pessoais do aluno
                SELECT
                    Id,
                    Titulo,
                    DataHora,
                    Tipo,
                    'Pessoal' AS TipoOrigem
                FROM dbo.Evento
                WHERE AlunoId = @AlunoId

                UNION ALL

                -- Eventos das turmas em que o aluno está activo
                SELECT
                    ev.Id,
                    ev.Titulo,
                    ev.DataHora,
                    ev.Tipo,
                    'Turma' AS TipoOrigem
                FROM dbo.Evento ev
                INNER JOIN dbo.AlunoTurma at2 ON at2.TurmaId = ev.TurmaId
                WHERE at2.AlunoId = @AlunoId
                  AND at2.Ate IS NULL;";

            using (SqlConnection Conn = new SqlConnection(_ConnectionString))
            using (SqlCommand Cmd = new SqlCommand(Sql, Conn))
            {
                Cmd.Parameters.AddWithValue("@AlunoId", IdAluno);
                Conn.Open();

                using (SqlDataReader Dr = Cmd.ExecuteReader())
                {
                    while (Dr.Read())
                    {
                        string Tipo = Dr["Tipo"].ToString();
                        string TipoOrigem = Dr["TipoOrigem"].ToString();

                        // Cor pré-definida por tipo. Eventos de turma ficam
                        // num tom mais escuro para se distinguirem dos pessoais.
                        string Cor;

                        if (TipoOrigem == "Turma")
                            Cor = Tipo == "Teste" ? "#d63031" : "#00b894";
                        else
                            Cor = Tipo == "Teste" ? "#ff7675" : "#55efc4";

                        Eventos.Add(new
                        {
                            id = Dr["Id"],
                            title = Dr["Titulo"].ToString(),
                            start = Convert.ToDateTime(Dr["DataHora"]).ToString("yyyy-MM-ddTHH:mm:ss"),
                            color = Cor,
                            tipo = Tipo,
                            tipoOrigem = TipoOrigem
                        });
                    }
                }
            }

            HdnEvents.Value = new JavaScriptSerializer().Serialize(Eventos);
        }

        private void CarregarAnexos(int IdEvento)
        {
            string Sql = @"
                SELECT Id, NomeFicheiro, CaminhoFicheiro
                FROM dbo.EventoAnexo
                WHERE EventoId = @EventoId
                ORDER BY CreatedAt;";

            DataTable Dt = new DataTable();

            using (SqlConnection Conn = new SqlConnection(_ConnectionString))
            using (SqlCommand Cmd = new SqlCommand(Sql, Conn))
            using (SqlDataAdapter Da = new SqlDataAdapter(Cmd))
            {
                Cmd.Parameters.AddWithValue("@EventoId", IdEvento);
                Da.Fill(Dt);
            }

            RepeaterAnexos.DataSource = Dt;
            RepeaterAnexos.DataBind();

            PainelAnexos.Visible = Dt.Rows.Count > 0;
        }

        #endregion

        #region Acesso a Dados — Evento

        private int InserirEvento(int idAluno, string titulo, string tipo, DateTime dataHora)
        {
            string Sql = @"
                INSERT INTO dbo.Evento
                (
                    AlunoId,
                    TurmaId,
                    Titulo,
                    Tipo,
                    DataHora
                )
                VALUES
                (
                    @AlunoId,
                    NULL,
                    @Titulo,
                    @Tipo,
                    @DataHora
                );

                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using (SqlConnection Conn = new SqlConnection(_ConnectionString))
            using (SqlCommand Cmd = new SqlCommand(Sql, Conn))
            {
                Cmd.Parameters.AddWithValue("@AlunoId", idAluno);
                Cmd.Parameters.AddWithValue("@Titulo", titulo);
                Cmd.Parameters.AddWithValue("@Tipo", tipo);
                Cmd.Parameters.AddWithValue("@DataHora", dataHora);

                Conn.Open();
                return Convert.ToInt32(Cmd.ExecuteScalar());
            }
        }

        private void AtualizarEvento(int idEvento, int idAluno, string titulo, string tipo, DateTime dataHora)
        {
            string Sql = @"
                UPDATE dbo.Evento
                SET
                    Titulo   = @Titulo,
                    Tipo     = @Tipo,
                    DataHora = @DataHora
                WHERE Id = @Id
                  AND AlunoId = @AlunoId;";

            using (SqlConnection Conn = new SqlConnection(_ConnectionString))
            using (SqlCommand Cmd = new SqlCommand(Sql, Conn))
            {
                Cmd.Parameters.AddWithValue("@Id", idEvento);
                Cmd.Parameters.AddWithValue("@AlunoId", idAluno);
                Cmd.Parameters.AddWithValue("@Titulo", titulo);
                Cmd.Parameters.AddWithValue("@Tipo", tipo);
                Cmd.Parameters.AddWithValue("@DataHora", dataHora);

                Conn.Open();
                Cmd.ExecuteNonQuery();
            }
        }

        private void ApagarEvento(int idEvento, int idAluno)
        {
            string Sql = @"
                DELETE FROM dbo.Evento
                WHERE Id = @Id
                  AND AlunoId = @AlunoId;";

            using (SqlConnection Conn = new SqlConnection(_ConnectionString))
            using (SqlCommand Cmd = new SqlCommand(Sql, Conn))
            {
                Cmd.Parameters.AddWithValue("@Id", idEvento);
                Cmd.Parameters.AddWithValue("@AlunoId", idAluno);

                Conn.Open();
                Cmd.ExecuteNonQuery();
            }
        }

        #endregion

        #region Acesso a Dados — Anexos

        private void GuardarAnexo(int idEvento, System.Web.UI.WebControls.FileUpload fileAnexo)
        {
            // Garante que a pasta existe.
            string PastaFisica = Server.MapPath("~/uploads/eventos/");

            if (!Directory.Exists(PastaFisica))
                Directory.CreateDirectory(PastaFisica);

            // Nome único para evitar colisões, mantendo o nome original visível.
            string NomeOriginal = Path.GetFileName(fileAnexo.FileName);
            string NomeFicheiro = Guid.NewGuid().ToString("N") + "_" + NomeOriginal;
            string CaminhoFisico = Path.Combine(PastaFisica, NomeFicheiro);
            string CaminhoRelativo = "~/uploads/eventos/" + NomeFicheiro;

            fileAnexo.SaveAs(CaminhoFisico);

            string Sql = @"
                INSERT INTO dbo.EventoAnexo
                (
                    EventoId,
                    NomeFicheiro,
                    CaminhoFicheiro
                )
                VALUES
                (
                    @EventoId,
                    @NomeFicheiro,
                    @CaminhoFicheiro
                );";

            using (SqlConnection Conn = new SqlConnection(_ConnectionString))
            using (SqlCommand Cmd = new SqlCommand(Sql, Conn))
            {
                Cmd.Parameters.AddWithValue("@EventoId", idEvento);
                Cmd.Parameters.AddWithValue("@NomeFicheiro", NomeOriginal);
                Cmd.Parameters.AddWithValue("@CaminhoFicheiro", ResolveUrl(CaminhoRelativo));

                Conn.Open();
                Cmd.ExecuteNonQuery();
            }
        }

        private void ApagarAnexo(int idAnexo)
        {
            string CaminhoFicheiro = null;

            string SqlSelect = "SELECT CaminhoFicheiro FROM dbo.EventoAnexo WHERE Id = @Id;";

            using (SqlConnection Conn = new SqlConnection(_ConnectionString))
            using (SqlCommand Cmd = new SqlCommand(SqlSelect, Conn))
            {
                Cmd.Parameters.AddWithValue("@Id", idAnexo);
                Conn.Open();

                object Valor = Cmd.ExecuteScalar();

                if (Valor != null && Valor != DBNull.Value)
                    CaminhoFicheiro = Valor.ToString();
            }

            string SqlDelete = "DELETE FROM dbo.EventoAnexo WHERE Id = @Id;";

            using (SqlConnection Conn = new SqlConnection(_ConnectionString))
            using (SqlCommand Cmd = new SqlCommand(SqlDelete, Conn))
            {
                Cmd.Parameters.AddWithValue("@Id", idAnexo);
                Conn.Open();
                Cmd.ExecuteNonQuery();
            }

            // Apaga o ficheiro físico, se existir.
            if (!string.IsNullOrEmpty(CaminhoFicheiro))
            {
                try
                {
                    string CaminhoFisico = Server.MapPath(CaminhoFicheiro);

                    if (File.Exists(CaminhoFisico))
                        File.Delete(CaminhoFisico);
                }
                catch
                {
                    // Se o ficheiro físico não puder ser apagado, mantemos
                    // o registo apagado da BD — não bloqueia o utilizador.
                }
            }
        }

        private void ApagarAnexosDoEvento(int idEvento)
        {
            List<int> Ids = new List<int>();

            string SqlSelect = "SELECT Id FROM dbo.EventoAnexo WHERE EventoId = @EventoId;";

            using (SqlConnection Conn = new SqlConnection(_ConnectionString))
            using (SqlCommand Cmd = new SqlCommand(SqlSelect, Conn))
            {
                Cmd.Parameters.AddWithValue("@EventoId", idEvento);
                Conn.Open();

                using (SqlDataReader Dr = Cmd.ExecuteReader())
                {
                    while (Dr.Read())
                        Ids.Add(Convert.ToInt32(Dr["Id"]));
                }
            }

            foreach (int IdAnexo in Ids)
                ApagarAnexo(IdAnexo);
        }

        #endregion

        #region Auxiliares

        private int GetIdAluno()
        {
            // Segue o mesmo padrão de TryGetAgrupamentoId usado em escolas.aspx,
            // mas para o lado do aluno.
            if (Session["AlunoID"] != null &&
                int.TryParse(Session["AlunoID"].ToString(), out int IdAlunoSessao))
            {
                return IdAlunoSessao;
            }

            if (Session["UserId"] == null)
                throw new Exception("Sessão inválida. Faça login novamente.");

            string UserId = Session["UserId"].ToString();

            string Sql = "SELECT Id FROM dbo.Aluno WHERE UserId = @UserId;";

            using (SqlConnection Conn = new SqlConnection(_ConnectionString))
            using (SqlCommand Cmd = new SqlCommand(Sql, Conn))
            {
                Cmd.Parameters.AddWithValue("@UserId", new Guid(UserId));
                Conn.Open();

                object Valor = Cmd.ExecuteScalar();

                if (Valor == null || Valor == DBNull.Value)
                    throw new Exception("Aluno não encontrado.");

                int IdAluno = Convert.ToInt32(Valor);
                Session["AlunoID"] = IdAluno;
                return IdAluno;
            }
        }

        private void LimparFormulario()
        {
            TxtTitulo.Text = "";
            TxtDataHora.Text = "";
            HdnEventoId.Value = "";
            DdlTipo.SelectedIndex = 0;
            PainelAnexos.Visible = false;
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
