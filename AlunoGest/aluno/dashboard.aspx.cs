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
        private readonly string _connectionString =
            ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        #region Página

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    CarregarResumo();
                    CarregarPublicacoes();
                    CarregarEventos();
                    CarregarNotas();
                }
            }
            catch (Exception ex)
            {
                MostrarMensagem(
                    "Não foi possível carregar o dashboard: " + ex.Message,
                    true
                );
            }
        }

        #endregion

        #region Publicações

        protected void BtnPublicar_Click(object sender, EventArgs e)
        {
            LimparMensagem();

            if (string.IsNullOrWhiteSpace(TxtTituloPublicacao.Text))
            {
                MostrarMensagem(
                    "Indica o título da publicação.",
                    true
                );

                return;
            }

            if (string.IsNullOrWhiteSpace(TxtConteudoPublicacao.Text))
            {
                MostrarMensagem(
                    "Escreve o conteúdo da publicação.",
                    true
                );

                return;
            }

            if (FilePublicacao.HasFile)
            {
                if (FilePublicacao.PostedFile.ContentLength > 10 * 1024 * 1024)
                {
                    MostrarMensagem(
                        "O anexo não pode ter mais de 10 MB.",
                        true
                    );

                    return;
                }

                string extensao =
                    Path.GetExtension(FilePublicacao.FileName)
                        .ToLowerInvariant();

                string[] extensoesPermitidas =
                {
                    ".pdf",
                    ".doc",
                    ".docx",
                    ".ppt",
                    ".pptx",
                    ".xls",
                    ".xlsx",
                    ".txt",
                    ".jpg",
                    ".jpeg",
                    ".png"
                };

                if (Array.IndexOf(extensoesPermitidas, extensao) < 0)
                {
                    MostrarMensagem(
                        "Este tipo de ficheiro não é permitido.",
                        true
                    );

                    return;
                }
            }

            try
            {
                int? turmaId = null;

                if (ChkPublicaTurma.Checked)
                {
                    turmaId = ObterTurmaAtualAluno();

                    if (!turmaId.HasValue)
                    {
                        MostrarMensagem(
                            "Não estás associado a nenhuma turma.",
                            true
                        );

                        return;
                    }
                }

                const string sql = @"
                    INSERT INTO dbo.Publicacao
                    (
                        AlunoId,
                        TurmaId,
                        Titulo,
                        Conteudo,
                        Tipo,
                        PublicaParaTurma
                    )
                    VALUES
                    (
                        @AlunoId,
                        @TurmaId,
                        @Titulo,
                        @Conteudo,
                        @Tipo,
                        @PublicaParaTurma
                    );

                    SELECT CAST(SCOPE_IDENTITY() AS INT);";

                int publicacaoId;

                using (SqlConnection conn =
                    new SqlConnection(_connectionString))
                using (SqlCommand cmd =
                    new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue(
                        "@AlunoId",
                        AlunoId
                    );

                    cmd.Parameters.AddWithValue(
                        "@TurmaId",
                        turmaId.HasValue
                            ? (object)turmaId.Value
                            : DBNull.Value
                    );

                    cmd.Parameters.AddWithValue(
                        "@Titulo",
                        TxtTituloPublicacao.Text.Trim()
                    );

                    cmd.Parameters.AddWithValue(
                        "@Conteudo",
                        TxtConteudoPublicacao.Text.Trim()
                    );

                    cmd.Parameters.AddWithValue(
                        "@Tipo",
                        DdlTipoPublicacao.SelectedValue
                    );

                    cmd.Parameters.AddWithValue(
                        "@PublicaParaTurma",
                        ChkPublicaTurma.Checked
                    );

                    conn.Open();

                    publicacaoId =
                        Convert.ToInt32(
                            cmd.ExecuteScalar()
                        );
                }

                if (FilePublicacao.HasFile)
                {
                    GuardarAnexoPublicacao(publicacaoId);
                }

                LimparFormularioPublicacao();

                MostrarMensagem(
                    "Publicação criada com sucesso.",
                    false
                );

                CarregarPublicacoes();
            }
            catch (Exception ex)
            {
                MostrarMensagem(
                    "Erro ao publicar: " + ex.Message,
                    true
                );
            }
        }

        protected void RepeaterPublicacoes_ItemCommand(
            object source,
            RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "Like")
                return;

            int publicacaoId;

            if (!int.TryParse(
                    e.CommandArgument.ToString(),
                    out publicacaoId))
            {
                MostrarMensagem(
                    "Publicação inválida.",
                    true
                );

                return;
            }

            try
            {
                if (!PublicacaoVisivelParaAluno(publicacaoId))
                {
                    MostrarMensagem(
                        "Não tens acesso a esta publicação.",
                        true
                    );

                    return;
                }

                AlternarLike(publicacaoId);

                CarregarPublicacoes();
            }
            catch (Exception ex)
            {
                MostrarMensagem(
                    "Erro ao atualizar o gosto: " + ex.Message,
                    true
                );
            }
        }

        private void CarregarPublicacoes()
        {
            int? turmaId = ObterTurmaAtualAluno();

            const string sql = @"
                SELECT
                    p.Id,
                    p.Titulo,
                    p.Conteudo,
                    p.Tipo,
                    p.CreatedAt,
                    p.AlunoId,
                    p.PublicaParaTurma,
                    a.NomeCompleto,
                    a.foto,
                    COUNT(pl.Id) AS TotalLikes

                FROM dbo.Publicacao p

                INNER JOIN dbo.Aluno a
                    ON a.Id = p.AlunoId

                LEFT JOIN dbo.PublicacaoLike pl
                    ON pl.PublicacaoId = p.Id

                WHERE
                    p.AlunoId = @AlunoId

                    OR

                    (
                        p.PublicaParaTurma = 1
                        AND p.TurmaId = @TurmaId
                    )

                GROUP BY
                    p.Id,
                    p.Titulo,
                    p.Conteudo,
                    p.Tipo,
                    p.CreatedAt,
                    p.AlunoId,
                    p.PublicaParaTurma,
                    a.NomeCompleto,
                    a.foto

                ORDER BY p.CreatedAt DESC;";

            DataTable tabela = new DataTable();

            using (SqlConnection conn =
                new SqlConnection(_connectionString))
            using (SqlCommand cmd =
                new SqlCommand(sql, conn))
            using (SqlDataAdapter adapter =
                new SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddWithValue(
                    "@AlunoId",
                    AlunoId
                );

                cmd.Parameters.AddWithValue(
                    "@TurmaId",
                    turmaId.HasValue
                        ? (object)turmaId.Value
                        : DBNull.Value
                );

                adapter.Fill(tabela);
            }

            RepeaterPublicacoes.DataSource = tabela;
            RepeaterPublicacoes.DataBind();
        }

        private bool PublicacaoVisivelParaAluno(int publicacaoId)
        {
            int? turmaId = ObterTurmaAtualAluno();

            const string sql = @"
                SELECT COUNT(1)

                FROM dbo.Publicacao

                WHERE Id = @PublicacaoId

                  AND
                  (
                      AlunoId = @AlunoId

                      OR

                      (
                          PublicaParaTurma = 1
                          AND TurmaId = @TurmaId
                      )
                  );";

            using (SqlConnection conn =
                new SqlConnection(_connectionString))
            using (SqlCommand cmd =
                new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue(
                    "@PublicacaoId",
                    publicacaoId
                );

                cmd.Parameters.AddWithValue(
                    "@AlunoId",
                    AlunoId
                );

                cmd.Parameters.AddWithValue(
                    "@TurmaId",
                    turmaId.HasValue
                        ? (object)turmaId.Value
                        : DBNull.Value
                );

                conn.Open();

                return Convert.ToInt32(
                    cmd.ExecuteScalar()
                ) > 0;
            }
        }

        private void AlternarLike(int publicacaoId)
        {
            const string sqlExiste = @"
                SELECT Id

                FROM dbo.PublicacaoLike

                WHERE PublicacaoId = @PublicacaoId
                  AND AlunoId = @AlunoId;";

            using (SqlConnection conn =
                new SqlConnection(_connectionString))
            {
                conn.Open();

                object likeExistente;

                using (SqlCommand cmd =
                    new SqlCommand(sqlExiste, conn))
                {
                    cmd.Parameters.AddWithValue(
                        "@PublicacaoId",
                        publicacaoId
                    );

                    cmd.Parameters.AddWithValue(
                        "@AlunoId",
                        AlunoId
                    );

                    likeExistente = cmd.ExecuteScalar();
                }

                if (likeExistente == null ||
                    likeExistente == DBNull.Value)
                {
                    const string sqlAdicionar = @"
                        INSERT INTO dbo.PublicacaoLike
                        (
                            PublicacaoId,
                            AlunoId
                        )
                        VALUES
                        (
                            @PublicacaoId,
                            @AlunoId
                        );";

                    using (SqlCommand cmd =
                        new SqlCommand(sqlAdicionar, conn))
                    {
                        cmd.Parameters.AddWithValue(
                            "@PublicacaoId",
                            publicacaoId
                        );

                        cmd.Parameters.AddWithValue(
                            "@AlunoId",
                            AlunoId
                        );

                        cmd.ExecuteNonQuery();
                    }
                }
                else
                {
                    const string sqlRemover = @"
                        DELETE FROM dbo.PublicacaoLike

                        WHERE PublicacaoId = @PublicacaoId
                          AND AlunoId = @AlunoId;";

                    using (SqlCommand cmd =
                        new SqlCommand(sqlRemover, conn))
                    {
                        cmd.Parameters.AddWithValue(
                            "@PublicacaoId",
                            publicacaoId
                        );

                        cmd.Parameters.AddWithValue(
                            "@AlunoId",
                            AlunoId
                        );

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        private void GuardarAnexoPublicacao(int publicacaoId)
        {
            string pastaFisica =
                Server.MapPath("~/uploads/publicacoes/");

            if (!Directory.Exists(pastaFisica))
            {
                Directory.CreateDirectory(pastaFisica);
            }

            string nomeOriginal =
                Path.GetFileName(FilePublicacao.FileName);

            string nomeGuardado =
                Guid.NewGuid().ToString("N") +
                "_" +
                nomeOriginal;

            string caminhoFisico =
                Path.Combine(
                    pastaFisica,
                    nomeGuardado
                );

            string caminhoRelativo =
                ResolveUrl(
                    "~/uploads/publicacoes/" +
                    nomeGuardado
                );

            FilePublicacao.SaveAs(caminhoFisico);

            const string sql = @"
                INSERT INTO dbo.PublicacaoAnexo
                (
                    PublicacaoId,
                    NomeFicheiro,
                    CaminhoFicheiro
                )
                VALUES
                (
                    @PublicacaoId,
                    @NomeFicheiro,
                    @CaminhoFicheiro
                );";

            using (SqlConnection conn =
                new SqlConnection(_connectionString))
            using (SqlCommand cmd =
                new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue(
                    "@PublicacaoId",
                    publicacaoId
                );

                cmd.Parameters.AddWithValue(
                    "@NomeFicheiro",
                    nomeOriginal
                );

                cmd.Parameters.AddWithValue(
                    "@CaminhoFicheiro",
                    caminhoRelativo
                );

                conn.Open();

                cmd.ExecuteNonQuery();
            }
        }

        private void LimparFormularioPublicacao()
        {
            TxtTituloPublicacao.Text = "";
            TxtConteudoPublicacao.Text = "";
            DdlTipoPublicacao.SelectedIndex = 0;
            ChkPublicaTurma.Checked = false;
        }

        #endregion

        #region Eventos e calendário

        protected void GridEventos_RowCommand(
            object sender,
            GridViewCommandEventArgs e)
        {
            int index;

            if (e.CommandName != "AbrirEvento" ||
                !int.TryParse(
                    e.CommandArgument.ToString(),
                    out index) ||
                index < 0 ||
                index >= GridEventos.Rows.Count)
            {
                return;
            }

            int eventoId =
                Convert.ToInt32(
                    GridEventos.DataKeys[index].Value
                );

            AbrirEvento(eventoId);
        }

        private void CarregarEventos()
        {
            const string sql = @"
                SELECT
                    ev.Id,
                    ev.Tipo,
                    ev.Titulo,
                    ev.DataHora,

                    CASE
                        WHEN EXISTS
                        (
                            SELECT 1

                            FROM dbo.EventoEntrega ee

                            WHERE ee.EventoId = ev.Id
                              AND ee.AlunoId = @AlunoId
                        )
                        THEN N'Entregue'

                        ELSE N'Por entregar'

                    END AS EstadoEntrega

                FROM dbo.Evento ev

                INNER JOIN dbo.AlunoTurma at2
                    ON at2.TurmaId = ev.TurmaId
                   AND at2.Ate IS NULL

                WHERE at2.AlunoId = @AlunoId

                ORDER BY ev.DataHora DESC;";

            DataTable tabela = new DataTable();

            using (SqlConnection conn =
                new SqlConnection(_connectionString))
            using (SqlCommand cmd =
                new SqlCommand(sql, conn))
            using (SqlDataAdapter adapter =
                new SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddWithValue(
                    "@AlunoId",
                    AlunoId
                );

                adapter.Fill(tabela);
            }

            GridEventos.DataSource = tabela;
            GridEventos.DataBind();

            List<object> eventos =
                new List<object>();

            foreach (DataRow row in tabela.Rows)
            {
                string tipo =
                    row["Tipo"].ToString();

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

                    title =
                        row["Titulo"].ToString(),

                    start =
                        Convert.ToDateTime(
                            row["DataHora"]
                        )
                        .ToString(
                            "yyyy-MM-ddTHH:mm:ss"
                        ),

                    color = cor
                });
            }

            HdnEvents.Value =
                new JavaScriptSerializer()
                    .Serialize(eventos);
        }

        private void AbrirEvento(int eventoId)
        {
            if (!EventoPertenceAoAluno(eventoId))
            {
                MostrarMensagem(
                    "Não tens acesso a este evento.",
                    true
                );

                return;
            }

            const string sqlEvento = @"
                SELECT
                    Titulo,
                    Tipo,
                    DataHora

                FROM dbo.Evento

                WHERE Id = @Id;";

            using (SqlConnection conn =
                new SqlConnection(_connectionString))
            using (SqlCommand cmd =
                new SqlCommand(sqlEvento, conn))
            {
                cmd.Parameters.AddWithValue(
                    "@Id",
                    eventoId
                );

                conn.Open();

                using (SqlDataReader reader =
                    cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        LblEventoSelecionado.Text =
                            reader["Tipo"] +
                            ": " +
                            reader["Titulo"] +
                            " (" +
                            Convert.ToDateTime(
                                reader["DataHora"]
                            )
                            .ToString(
                                "dd/MM/yyyy HH:mm"
                            ) +
                            ")";
                    }
                }
            }

            const string sqlAnexos = @"
                SELECT
                    NomeFicheiro,
                    CaminhoFicheiro

                FROM dbo.EventoAnexo

                WHERE EventoId = @Id

                ORDER BY CreatedAt;";

            DataTable anexos =
                new DataTable();

            using (SqlConnection conn =
                new SqlConnection(_connectionString))
            using (SqlCommand cmd =
                new SqlCommand(sqlAnexos, conn))
            using (SqlDataAdapter adapter =
                new SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddWithValue(
                    "@Id",
                    eventoId
                );

                adapter.Fill(anexos);
            }

            RepeaterAnexosProfessor.DataSource = anexos;
            RepeaterAnexosProfessor.DataBind();

            HdnEventoId.Value = eventoId.ToString();

            TxtObservacao.Text = "";

            PainelEntrega.Visible = true;
        }

        private bool EventoPertenceAoAluno(int eventoId)
        {
            const string sql = @"
                SELECT COUNT(1)

                FROM dbo.Evento ev

                INNER JOIN dbo.AlunoTurma at2
                    ON at2.TurmaId = ev.TurmaId
                   AND at2.Ate IS NULL

                WHERE ev.Id = @EventoId
                  AND at2.AlunoId = @AlunoId;";

            using (SqlConnection conn =
                new SqlConnection(_connectionString))
            using (SqlCommand cmd =
                new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue(
                    "@EventoId",
                    eventoId
                );

                cmd.Parameters.AddWithValue(
                    "@AlunoId",
                    AlunoId
                );

                conn.Open();

                return Convert.ToInt32(
                    cmd.ExecuteScalar()
                ) > 0;
            }
        }

        #endregion

        #region Entregas

        protected void BtnEntregar_Click(
            object sender,
            EventArgs e)
        {
            LimparMensagem();

            int eventoId;

            if (!int.TryParse(
                    HdnEventoId.Value,
                    out eventoId))
            {
                MostrarMensagem(
                    "Escolhe um evento.",
                    true
                );

                return;
            }

            if (!EventoPertenceAoAluno(eventoId))
            {
                MostrarMensagem(
                    "Este evento não pertence à tua turma.",
                    true
                );

                return;
            }

            if (!FileEntrega.HasFile)
            {
                MostrarMensagem(
                    "Escolhe um ficheiro para entregar.",
                    true
                );

                return;
            }

            if (FileEntrega.PostedFile.ContentLength >
                10 * 1024 * 1024)
            {
                MostrarMensagem(
                    "O anexo não pode ter mais de 10 MB.",
                    true
                );

                return;
            }

            try
            {
                string pasta =
                    Server.MapPath(
                        "~/uploads/entregas/"
                    );

                Directory.CreateDirectory(pasta);

                string original =
                    Path.GetFileName(
                        FileEntrega.FileName
                    );

                string nomeGuardado =
                    Guid.NewGuid()
                        .ToString("N") +
                    "_" +
                    original;

                FileEntrega.SaveAs(
                    Path.Combine(
                        pasta,
                        nomeGuardado
                    )
                );

                const string sql = @"
                    INSERT INTO dbo.EventoEntrega
                    (
                        EventoId,
                        AlunoId,
                        NomeFicheiro,
                        CaminhoFicheiro,
                        Observacao
                    )
                    VALUES
                    (
                        @EventoId,
                        @AlunoId,
                        @NomeFicheiro,
                        @CaminhoFicheiro,
                        @Observacao
                    );";

                using (SqlConnection conn =
                    new SqlConnection(_connectionString))
                using (SqlCommand cmd =
                    new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue(
                        "@EventoId",
                        eventoId
                    );

                    cmd.Parameters.AddWithValue(
                        "@AlunoId",
                        AlunoId
                    );

                    cmd.Parameters.AddWithValue(
                        "@NomeFicheiro",
                        original
                    );

                    cmd.Parameters.AddWithValue(
                        "@CaminhoFicheiro",
                        ResolveUrl(
                            "~/uploads/entregas/" +
                            nomeGuardado
                        )
                    );

                    cmd.Parameters.AddWithValue(
                        "@Observacao",
                        string.IsNullOrWhiteSpace(
                            TxtObservacao.Text
                        )
                            ? (object)DBNull.Value
                            : TxtObservacao.Text.Trim()
                    );

                    conn.Open();

                    cmd.ExecuteNonQuery();
                }

                MostrarMensagem(
                    "Entrega enviada com sucesso.",
                    false
                );

                PainelEntrega.Visible = false;

                CarregarEventos();
                CarregarNotas();
            }
            catch (Exception ex)
            {
                MostrarMensagem(
                    "Não foi possível enviar: " +
                    ex.Message,
                    true
                );
            }
        }

        protected void BtnFecharEntrega_Click(
            object sender,
            EventArgs e)
        {
            PainelEntrega.Visible = false;
        }

        #endregion

        #region Notas

        private void CarregarNotas()
        {
            const string sql = @"
                SELECT
                    ev.Titulo AS Evento,
                    ee.Nota,
                    ee.Feedback

                FROM dbo.EventoEntrega ee

                INNER JOIN dbo.Evento ev
                    ON ev.Id = ee.EventoId

                WHERE ee.AlunoId = @AlunoId
                  AND ee.Nota IS NOT NULL

                ORDER BY ee.AvaliadoEm DESC;";

            DataTable tabela =
                new DataTable();

            using (SqlConnection conn =
                new SqlConnection(_connectionString))
            using (SqlCommand cmd =
                new SqlCommand(sql, conn))
            using (SqlDataAdapter adapter =
                new SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddWithValue(
                    "@AlunoId",
                    AlunoId
                );

                adapter.Fill(tabela);
            }

            GridNotas.DataSource = tabela;
            GridNotas.DataBind();
        }

        #endregion

        #region Aluno

        private int AlunoId
        {
            get
            {
                int id;

                if (Session["AlunoID"] != null &&
                    int.TryParse(
                        Session["AlunoID"].ToString(),
                        out id))
                {
                    return id;
                }

                if (Session["UserId"] == null)
                {
                    throw new InvalidOperationException(
                        "Sessão terminada."
                    );
                }

                const string sql = @"
                    SELECT Id

                    FROM dbo.Aluno

                    WHERE UserId = @UserId
                      AND Ativo = 1;";

                using (SqlConnection conn =
                    new SqlConnection(_connectionString))
                using (SqlCommand cmd =
                    new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue(
                        "@UserId",
                        new Guid(
                            Session["UserId"].ToString()
                        )
                    );

                    conn.Open();

                    object value =
                        cmd.ExecuteScalar();

                    if (value == null ||
                        value == DBNull.Value)
                    {
                        throw new InvalidOperationException(
                            "Aluno não encontrado."
                        );
                    }

                    id = Convert.ToInt32(value);

                    Session["AlunoID"] = id;

                    return id;
                }
            }
        }

        private int? ObterTurmaAtualAluno()
        {
            const string sql = @"
                SELECT TOP 1 TurmaId

                FROM dbo.AlunoTurma

                WHERE AlunoId = @AlunoId
                  AND Ate IS NULL;";

            using (SqlConnection conn =
                new SqlConnection(_connectionString))
            using (SqlCommand cmd =
                new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue(
                    "@AlunoId",
                    AlunoId
                );

                conn.Open();

                object valor =
                    cmd.ExecuteScalar();

                if (valor == null ||
                    valor == DBNull.Value)
                {
                    return null;
                }

                return Convert.ToInt32(valor);
            }
        }

        private void CarregarResumo()
        {
            const string sql = @"
                SELECT TOP 1

                    a.NomeCompleto,

                    CAST(
                        t.AnoEscolaridade AS varchar(2)
                    )
                    + '.º'
                    + t.CodigoTurma
                    AS Turma,

                    e.Nome AS Escola

                FROM dbo.Aluno a

                LEFT JOIN dbo.AlunoTurma at2
                    ON at2.AlunoId = a.Id
                   AND at2.Ate IS NULL

                LEFT JOIN dbo.Turma t
                    ON t.Id = at2.TurmaId

                LEFT JOIN dbo.Escola e
                    ON e.Id = t.EscolaId

                WHERE a.Id = @AlunoId;";

            using (SqlConnection conn =
                new SqlConnection(_connectionString))
            using (SqlCommand cmd =
                new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue(
                    "@AlunoId",
                    AlunoId
                );

                conn.Open();

                using (SqlDataReader reader =
                    cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string turma =
                            reader["Turma"] == DBNull.Value
                                ? "sem turma"
                                : reader["Turma"].ToString();

                        string escola =
                            reader["Escola"] == DBNull.Value
                                ? ""
                                : reader["Escola"].ToString();

                        LblResumo.Text =
                            reader["NomeCompleto"] +
                            " | Turma: " +
                            turma +
                            " | " +
                            escola;
                    }
                }
            }
        }

        #endregion

        #region Arrumar Foto no feed de publicações
        protected bool TemFotoPublicacao(object foto)
        {
            if (foto == null || foto == DBNull.Value)
                return false;

            string caminho = foto.ToString().Trim();

            return !string.IsNullOrWhiteSpace(caminho);
        }


        protected string ObterFotoPublicacao(object foto)
        {
            if (!TemFotoPublicacao(foto))
                return "";

            string caminho = foto.ToString().Trim();

            // URL externa
            if (caminho.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                caminho.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                return caminho;
            }

            // Exemplo: ~/uploads/perfis/foto.jpg
            if (caminho.StartsWith("~/"))
            {
                return ResolveUrl(caminho);
            }

            // Exemplo: /uploads/perfis/foto.jpg
            if (caminho.StartsWith("/"))
            {
                return ResolveUrl("~" + caminho);
            }

            // Exemplo: uploads/perfis/foto.jpg
            return ResolveUrl("~/" + caminho);
        }
        #endregion

        #region Mensagens

        private void MostrarMensagem(
            string texto,
            bool erro)
        {
            LblMensagem.Text = texto;

            LblMensagem.CssClass =
                erro
                    ? "alert alert-warning d-block"
                    : "alert alert-success d-block";

            LblMensagem.Visible = true;
        }

        private void LimparMensagem()
        {
            LblMensagem.Text = "";
            LblMensagem.Visible = false;
        }

        #endregion
    }
}