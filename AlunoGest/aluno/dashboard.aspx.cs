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
                    CarregarDestinatariosPublicacao();
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

        private List<Guid> ObterDestinatariosSelecionados()
        {
            List<Guid> destinatarios =
                new List<Guid>();

            foreach (ListItem item
                in CblAlunosDestinatarios.Items)
            {
                Guid userId;

                if (item.Selected &&
                    Guid.TryParse(
                        item.Value,
                        out userId) &&
                    !destinatarios.Contains(userId))
                {
                    destinatarios.Add(userId);
                }
            }

            foreach (ListItem item
                in CblProfessoresDestinatarios.Items)
            {
                Guid userId;

                if (item.Selected &&
                    Guid.TryParse(
                        item.Value,
                        out userId) &&
                    !destinatarios.Contains(userId))
                {
                    destinatarios.Add(userId);
                }
            }

            return destinatarios;
        }

        private void GuardarDestinatarioPublicacao(
    int publicacaoId,
    int turmaId,
    Guid destinatarioUserId,
    SqlConnection conn,
    SqlTransaction transaction)
        {
            /*
             * Esta consulta também confirma que a pessoa
             * escolhida realmente pertence à turma atual.
             */
            const string sql = @"
        INSERT INTO dbo.PublicacaoDestinatario
        (
            PublicacaoId,
            DestinatarioUserId
        )

        SELECT
            @PublicacaoId,
            @DestinatarioUserId

        WHERE EXISTS
        (
            /*
             * O destinatário é um aluno ativo
             * da turma atual.
             */
            SELECT 1

            FROM dbo.Aluno aluno

            INNER JOIN dbo.AlunoTurma alunoTurma
                ON alunoTurma.AlunoId = aluno.Id

            WHERE aluno.UserId =
                    @DestinatarioUserId

              AND aluno.Ativo = 1

              AND alunoTurma.TurmaId =
                    @TurmaId

              AND alunoTurma.Ate IS NULL

            UNION ALL

            /*
             * Ou é um professor ativo
             * associado a uma disciplina da turma.
             */
            SELECT 1

            FROM dbo.Professor professor

            INNER JOIN dbo.TurmaDisciplinaProfessor
                turmaProfessor

                ON turmaProfessor.ProfessorId =
                    professor.Id

            INNER JOIN dbo.TurmaDisciplina
                turmaDisciplina

                ON turmaDisciplina.Id =
                    turmaProfessor.TurmaDisciplinaId

            WHERE professor.UserId =
                    @DestinatarioUserId

              AND professor.Ativo = 1

              AND turmaDisciplina.TurmaId =
                    @TurmaId

              AND turmaProfessor.Ate IS NULL
        );";

            using (SqlCommand cmd =
                new SqlCommand(
                    sql,
                    conn,
                    transaction))
            {
                cmd.Parameters
                    .Add(
                        "@PublicacaoId",
                        SqlDbType.Int
                    )
                    .Value = publicacaoId;

                cmd.Parameters
                    .Add(
                        "@TurmaId",
                        SqlDbType.Int
                    )
                    .Value = turmaId;

                cmd.Parameters
                    .Add(
                        "@DestinatarioUserId",
                        SqlDbType.UniqueIdentifier
                    )
                    .Value = destinatarioUserId;

                int linhas =
                    cmd.ExecuteNonQuery();

                if (linhas == 0)
                {
                    throw new InvalidOperationException(
                        "Um dos destinatários selecionados " +
                        "já não pertence a esta turma."
                    );
                }
            }
        }
        private void LimparFormularioPublicacao()
        {
            TxtTituloPublicacao.Text =
                string.Empty;

            TxtConteudoPublicacao.Text =
                string.Empty;

            DdlTipoPublicacao.SelectedIndex =
                0;

            ChkPublicaTurma.Checked =
                false;

            foreach (ListItem item
                in CblAlunosDestinatarios.Items)
            {
                item.Selected = false;
            }

            foreach (ListItem item
                in CblProfessoresDestinatarios.Items)
            {
                item.Selected = false;
            }
        }

        protected void BtnPublicar_Click(
    object sender,
    EventArgs e)
        {
            LimparMensagem();

            if (string.IsNullOrWhiteSpace(
                    TxtTituloPublicacao.Text))
            {
                MostrarMensagem(
                    "Indica o título da publicação.",
                    true
                );

                return;
            }

            if (string.IsNullOrWhiteSpace(
                    TxtConteudoPublicacao.Text))
            {
                MostrarMensagem(
                    "Escreve o conteúdo da publicação.",
                    true
                );

                return;
            }

            if (FilePublicacao.HasFile)
            {
                if (FilePublicacao.PostedFile.ContentLength >
                    10 * 1024 * 1024)
                {
                    MostrarMensagem(
                        "O anexo não pode ter mais de 10 MB.",
                        true
                    );

                    return;
                }

                string extensao =
                    Path.GetExtension(
                        FilePublicacao.FileName
                    ).ToLowerInvariant();

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

                if (Array.IndexOf(
                        extensoesPermitidas,
                        extensao) < 0)
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
                int? turmaId =
                    ObterTurmaAtualAluno();

                if (!turmaId.HasValue)
                {
                    MostrarMensagem(
                        "Não estás associado a nenhuma turma.",
                        true
                    );

                    return;
                }

                List<Guid> destinatarios =
                    ObterDestinatariosSelecionados();

                /*
                 * Quando não é para toda a turma,
                 * pelo menos uma pessoa precisa ser selecionada.
                 */
                if (!ChkPublicaTurma.Checked &&
                    destinatarios.Count == 0)
                {
                    MostrarMensagem(
                        "Seleciona pelo menos um aluno ou professor, " +
                        "ou marca a opção para publicar para toda a turma.",
                        true
                    );

                    return;
                }

                int publicacaoId;

                using (SqlConnection conn =
                    new SqlConnection(_connectionString))
                {
                    conn.Open();

                    using (SqlTransaction transaction =
                        conn.BeginTransaction())
                    {
                        try
                        {
                            const string sqlPublicacao = @"
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

                        SELECT CAST(
                            SCOPE_IDENTITY()
                            AS INT
                        );";

                            using (SqlCommand cmd =
                                new SqlCommand(
                                    sqlPublicacao,
                                    conn,
                                    transaction))
                            {
                                cmd.Parameters
                                    .Add(
                                        "@AlunoId",
                                        SqlDbType.Int
                                    )
                                    .Value = AlunoId;

                                /*
                                 * Guardamos sempre a turma,
                                 * mesmo quando a publicação é individual.
                                 */
                                cmd.Parameters
                                    .Add(
                                        "@TurmaId",
                                        SqlDbType.Int
                                    )
                                    .Value = turmaId.Value;

                                cmd.Parameters
                                    .Add(
                                        "@Titulo",
                                        SqlDbType.NVarChar,
                                        200
                                    )
                                    .Value =
                                    TxtTituloPublicacao.Text.Trim();

                                cmd.Parameters
                                    .Add(
                                        "@Conteudo",
                                        SqlDbType.NVarChar,
                                        -1
                                    )
                                    .Value =
                                    TxtConteudoPublicacao.Text.Trim();

                                cmd.Parameters
                                    .Add(
                                        "@Tipo",
                                        SqlDbType.NVarChar,
                                        30
                                    )
                                    .Value =
                                    DdlTipoPublicacao.SelectedValue;

                                cmd.Parameters
                                    .Add(
                                        "@PublicaParaTurma",
                                        SqlDbType.Bit
                                    )
                                    .Value =
                                    ChkPublicaTurma.Checked;

                                publicacaoId =
                                    Convert.ToInt32(
                                        cmd.ExecuteScalar()
                                    );
                            }

                            /*
                             * Se for para toda a turma,
                             * não precisamos guardar pessoas individuais.
                             */
                            if (!ChkPublicaTurma.Checked)
                            {
                                foreach (Guid destinatarioUserId
                                    in destinatarios)
                                {
                                    GuardarDestinatarioPublicacao(
                                        publicacaoId,
                                        turmaId.Value,
                                        destinatarioUserId,
                                        conn,
                                        transaction
                                    );
                                }
                            }

                            transaction.Commit();
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }

                if (FilePublicacao.HasFile)
                {
                    GuardarAnexoPublicacao(
                        publicacaoId
                    );
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
                    "Erro ao publicar: " +
                    ex.Message,
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
            int? turmaId =
                ObterTurmaAtualAluno();

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
            a.Foto,
            COUNT(pl.Id) AS TotalLikes

        FROM dbo.Publicacao p

        INNER JOIN dbo.Aluno a
            ON a.Id = p.AlunoId

        LEFT JOIN dbo.PublicacaoLike pl
            ON pl.PublicacaoId = p.Id

        WHERE
        (
            /* O aluno é o autor. */
            p.AlunoId = @AlunoId

            OR

            /* A publicação foi enviada para toda a turma. */
            (
                p.PublicaParaTurma = 1
                AND p.TurmaId = @TurmaId
            )

            OR

            /* O aluno foi escolhido individualmente. */
            EXISTS
            (
                SELECT 1

                FROM dbo.PublicacaoDestinatario pd

                WHERE pd.PublicacaoId = p.Id
                  AND pd.DestinatarioUserId = @UserIdAtual
            )
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
            a.Foto

        ORDER BY p.CreatedAt DESC;";

            DataTable tabela =
                new DataTable();

            using (SqlConnection conn =
                new SqlConnection(_connectionString))
            using (SqlCommand cmd =
                new SqlCommand(sql, conn))
            using (SqlDataAdapter adapter =
                new SqlDataAdapter(cmd))
            {
                cmd.Parameters
                    .Add(
                        "@AlunoId",
                        SqlDbType.Int
                    )
                    .Value = AlunoId;

                cmd.Parameters
                    .Add(
                        "@TurmaId",
                        SqlDbType.Int
                    )
                    .Value =
                    turmaId.HasValue
                        ? (object)turmaId.Value
                        : DBNull.Value;

                cmd.Parameters
                    .Add(
                        "@UserIdAtual",
                        SqlDbType.UniqueIdentifier
                    )
                    .Value = UserIdAtual;

                adapter.Fill(tabela);
            }

            RepeaterPublicacoes.DataSource =
                tabela;

            RepeaterPublicacoes.DataBind();
        }

        private bool PublicacaoVisivelParaAluno(
    int publicacaoId)
        {
            int? turmaId =
                ObterTurmaAtualAluno();

            const string sql = @"
        SELECT COUNT(1)

        FROM dbo.Publicacao p

        WHERE p.Id = @PublicacaoId

          AND
          (
              p.AlunoId = @AlunoId

              OR

              (
                  p.PublicaParaTurma = 1
                  AND p.TurmaId = @TurmaId
              )

              OR

              EXISTS
              (
                  SELECT 1

                  FROM dbo.PublicacaoDestinatario pd

                  WHERE pd.PublicacaoId = p.Id
                    AND pd.DestinatarioUserId =
                        @UserIdAtual
              )
          );";

            using (SqlConnection conn =
                new SqlConnection(_connectionString))
            using (SqlCommand cmd =
                new SqlCommand(sql, conn))
            {
                cmd.Parameters
                    .Add(
                        "@PublicacaoId",
                        SqlDbType.Int
                    )
                    .Value = publicacaoId;

                cmd.Parameters
                    .Add(
                        "@AlunoId",
                        SqlDbType.Int
                    )
                    .Value = AlunoId;

                cmd.Parameters
                    .Add(
                        "@TurmaId",
                        SqlDbType.Int
                    )
                    .Value =
                    turmaId.HasValue
                        ? (object)turmaId.Value
                        : DBNull.Value;

                cmd.Parameters
                    .Add(
                        "@UserIdAtual",
                        SqlDbType.UniqueIdentifier
                    )
                    .Value = UserIdAtual;

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


    

        private void CarregarDestinatariosPublicacao()
        {
            CblAlunosDestinatarios.Items.Clear();
            CblProfessoresDestinatarios.Items.Clear();

            int? turmaId =
                ObterTurmaAtualAluno();

            if (!turmaId.HasValue)
            {
                LblSemAlunosDestinatarios.Visible = true;
                LblSemProfessoresDestinatarios.Visible = true;

                return;
            }

            CarregarAlunosDestinatarios(
                turmaId.Value
            );

            CarregarProfessoresDestinatarios(
                turmaId.Value
            );
        }

        private void CarregarAlunosDestinatarios(
    int turmaId)
        {
            const string sql = @"
        SELECT
            a.UserId,
            a.NomeCompleto,
            a.NumeroProcesso

        FROM dbo.AlunoTurma at2

        INNER JOIN dbo.Aluno a
            ON a.Id = at2.AlunoId

        WHERE at2.TurmaId = @TurmaId
          AND at2.Ate IS NULL
          AND a.Ativo = 1
          AND a.Id <> @AlunoId
          AND a.UserId IS NOT NULL

        ORDER BY a.NomeCompleto;";

            DataTable tabela =
                new DataTable();

            using (SqlConnection conn =
                new SqlConnection(_connectionString))
            using (SqlCommand cmd =
                new SqlCommand(sql, conn))
            using (SqlDataAdapter adapter =
                new SqlDataAdapter(cmd))
            {
                cmd.Parameters
                    .Add("@TurmaId", SqlDbType.Int)
                    .Value = turmaId;

                cmd.Parameters
                    .Add("@AlunoId", SqlDbType.Int)
                    .Value = AlunoId;

                adapter.Fill(tabela);
            }

            foreach (DataRow row in tabela.Rows)
            {
                string numeroProcesso =
                    row["NumeroProcesso"] == DBNull.Value
                        ? "sem número de processo"
                        : row["NumeroProcesso"].ToString();

                string texto =
                    row["NomeCompleto"] +
                    " — Processo: " +
                    numeroProcesso;

                string userId =
                    row["UserId"].ToString();

                CblAlunosDestinatarios.Items.Add(
                    new ListItem(
                        texto,
                        userId
                    )
                );
            }

            LblSemAlunosDestinatarios.Visible =
                tabela.Rows.Count == 0;
        }

        private void CarregarProfessoresDestinatarios(
    int turmaId)
        {
            const string sql = @"
        SELECT
            p.UserId,
            p.Nome,

            STRING_AGG(
                disciplinas.Nome,
                ', '
            ) AS Disciplinas

        FROM dbo.Professor p

        INNER JOIN
        (
            SELECT DISTINCT
                tdp.ProfessorId,
                d.Nome

            FROM dbo.TurmaDisciplina td

            INNER JOIN dbo.TurmaDisciplinaProfessor tdp
                ON tdp.TurmaDisciplinaId = td.Id

            INNER JOIN dbo.Disciplina d
                ON d.Id = td.DisciplinaId

            WHERE td.TurmaId = @TurmaId
              AND tdp.Ate IS NULL

        ) AS disciplinas
            ON disciplinas.ProfessorId = p.Id

        WHERE p.Ativo = 1
          AND p.UserId IS NOT NULL

        GROUP BY
            p.UserId,
            p.Nome

        ORDER BY p.Nome;";

            DataTable tabela =
                new DataTable();

            using (SqlConnection conn =
                new SqlConnection(_connectionString))
            using (SqlCommand cmd =
                new SqlCommand(sql, conn))
            using (SqlDataAdapter adapter =
                new SqlDataAdapter(cmd))
            {
                cmd.Parameters
                    .Add("@TurmaId", SqlDbType.Int)
                    .Value = turmaId;

                adapter.Fill(tabela);
            }

            foreach (DataRow row in tabela.Rows)
            {
                string disciplinas =
                    row["Disciplinas"] == DBNull.Value
                        ? "Professor"
                        : row["Disciplinas"].ToString();

                string texto =
                    row["Nome"] +
                    " — " +
                    disciplinas;

                string userId =
                    row["UserId"].ToString();

                CblProfessoresDestinatarios.Items.Add(
                    new ListItem(
                        texto,
                        userId
                    )
                );
            }

            LblSemProfessoresDestinatarios.Visible =
                tabela.Rows.Count == 0;
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
            DataHora,
            CASE
                WHEN DataHora >= SYSDATETIME()
                    THEN CAST(1 AS BIT)
                ELSE CAST(0 AS BIT)
            END AS PrazoAberto
        FROM dbo.Evento
        WHERE Id = @Id;";

            using (SqlConnection conn =
                new SqlConnection(_connectionString))
            using (SqlCommand cmd =
                new SqlCommand(sqlEvento, conn))
            {
                cmd.Parameters
                    .Add("@Id", SqlDbType.Int)
                    .Value = eventoId;

                conn.Open();

                using (SqlDataReader reader =
                    cmd.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        MostrarMensagem(
                            "O evento não foi encontrado.",
                            true
                        );

                        return;
                    }

                    DateTime dataLimite =
                        Convert.ToDateTime(
                            reader["DataHora"]
                        );

                    bool prazoAberto =
                        Convert.ToBoolean(
                            reader["PrazoAberto"]
                        );

                    LblEventoSelecionado.Text =
                        reader["Tipo"] +
                        ": " +
                        reader["Titulo"];

                    if (prazoAberto)
                    {
                        LblPrazoEntrega.Text =
                            "Prazo de entrega: " +
                            dataLimite.ToString(
                                "dd/MM/yyyy HH:mm"
                            );

                        LblPrazoEntrega.CssClass =
                            "badge bg-success d-inline-block mt-2";

                        FileEntrega.Enabled = true;
                        TxtObservacao.Enabled = true;
                        BtnEntregar.Enabled = true;

                        BtnEntregar.Text =
                            "Enviar entrega";

                        BtnEntregar.CssClass =
                            "btn btn-success";
                    }
                    else
                    {
                        LblPrazoEntrega.Text =
                            "Prazo encerrado em " +
                            dataLimite.ToString(
                                "dd/MM/yyyy HH:mm"
                            );

                        LblPrazoEntrega.CssClass =
                            "badge bg-danger d-inline-block mt-2";

                        FileEntrega.Enabled = false;
                        TxtObservacao.Enabled = false;
                        BtnEntregar.Enabled = false;

                        BtnEntregar.Text =
                            "Prazo encerrado";

                        BtnEntregar.CssClass =
                            "btn btn-secondary";

                        MostrarMensagem(
                            "O prazo deste trabalho já terminou. " +
                            "Ainda podes consultar os dados, " +
                            "mas não podes enviar uma nova submissão.",
                            true
                        );
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
                cmd.Parameters
                    .Add("@Id", SqlDbType.Int)
                    .Value = eventoId;

                adapter.Fill(anexos);
            }

            RepeaterAnexosProfessor.DataSource =
                anexos;

            RepeaterAnexosProfessor.DataBind();

            HdnEventoId.Value =
                eventoId.ToString();

            TxtObservacao.Text =
                string.Empty;

            CarregarSubmissoes(eventoId);


            PainelEntrega.Visible =
                true;
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

        private bool TentarEliminarFicheiroFisico(
        string caminhoVirtual)
        {
            if (string.IsNullOrWhiteSpace(caminhoVirtual))
                return true;

            try
            {
                string caminhoFisico =
                    Server.MapPath(caminhoVirtual);

                if (File.Exists(caminhoFisico))
                {
                    File.Delete(caminhoFisico);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
        private bool EliminarSubmissao(
    int entregaId,
    int eventoId,
    out string caminhoFicheiro)
        {
            caminhoFicheiro = null;

            const string sqlSelecionar = @"
        SELECT CaminhoFicheiro

        FROM dbo.EventoEntrega

        WHERE Id = @EntregaId
          AND EventoId = @EventoId
          AND AlunoId = @AlunoId;";

            const string sqlEliminar = @"
        DELETE FROM dbo.EventoEntrega

        WHERE Id = @EntregaId
          AND EventoId = @EventoId
          AND AlunoId = @AlunoId;";

            using (SqlConnection conn =
                new SqlConnection(_connectionString))
            {
                conn.Open();

                using (SqlTransaction transaction =
                    conn.BeginTransaction())
                {
                    try
                    {
                        using (SqlCommand cmdSelecionar =
                            new SqlCommand(
                                sqlSelecionar,
                                conn,
                                transaction))
                        {
                            cmdSelecionar.Parameters
                                .Add("@EntregaId", SqlDbType.Int)
                                .Value = entregaId;

                            cmdSelecionar.Parameters
                                .Add("@EventoId", SqlDbType.Int)
                                .Value = eventoId;

                            cmdSelecionar.Parameters
                                .Add("@AlunoId", SqlDbType.Int)
                                .Value = AlunoId;

                            object resultado =
                                cmdSelecionar.ExecuteScalar();

                            if (resultado == null ||
                                resultado == DBNull.Value)
                            {
                                transaction.Rollback();
                                return false;
                            }

                            caminhoFicheiro =
                                resultado.ToString();
                        }

                        using (SqlCommand cmdEliminar =
                            new SqlCommand(
                                sqlEliminar,
                                conn,
                                transaction))
                        {
                            cmdEliminar.Parameters
                                .Add("@EntregaId", SqlDbType.Int)
                                .Value = entregaId;

                            cmdEliminar.Parameters
                                .Add("@EventoId", SqlDbType.Int)
                                .Value = eventoId;

                            cmdEliminar.Parameters
                                .Add("@AlunoId", SqlDbType.Int)
                                .Value = AlunoId;

                            int linhas =
                                cmdEliminar.ExecuteNonQuery();

                            if (linhas == 0)
                            {
                                transaction.Rollback();
                                return false;
                            }
                        }

                        transaction.Commit();

                        return true;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        protected void GridSubmissoes_RowCommand(
    object sender,
    GridViewCommandEventArgs e)
        {
            if (e.CommandName != "EliminarSubmissao")
                return;

            int entregaId;

            if (!int.TryParse(
                    e.CommandArgument.ToString(),
                    out entregaId))
            {
                MostrarMensagem(
                    "A submissão selecionada é inválida.",
                    true
                );

                return;
            }

            int eventoId;

            if (!int.TryParse(
                    HdnEventoId.Value,
                    out eventoId))
            {
                MostrarMensagem(
                    "Não foi possível identificar o trabalho.",
                    true
                );

                return;
            }

            if (!EventoPertenceAoAluno(eventoId))
            {
                MostrarMensagem(
                    "Não tens acesso a este trabalho.",
                    true
                );

                return;
            }

            if (!PrazoEntregaAberto(eventoId))
            {
                MostrarMensagem(
                    "O prazo deste trabalho já terminou. " +
                    "Já não podes eliminar submissões.",
                    true
                );

                AbrirEvento(eventoId);

                return;
            }

            try
            {
                string caminhoFicheiro;

                bool eliminada =
                    EliminarSubmissao(
                        entregaId,
                        eventoId,
                        out caminhoFicheiro
                    );

                if (!eliminada)
                {
                    MostrarMensagem(
                        "A submissão não foi encontrada " +
                        "ou não pertence a este aluno.",
                        true
                    );

                    AbrirEvento(eventoId);

                    return;
                }

                bool ficheiroEliminado =
                    TentarEliminarFicheiroFisico(
                        caminhoFicheiro
                    );

                if (ficheiroEliminado)
                {
                    MostrarMensagem(
                        "Submissão eliminada com sucesso.",
                        false
                    );
                }
                else
                {
                    MostrarMensagem(
                        "A submissão foi removida, mas o ficheiro " +
                        "não pôde ser eliminado do servidor.",
                        true
                    );
                }

                CarregarEventos();
                CarregarNotas();
                AbrirEvento(eventoId);
            }
            catch (Exception ex)
            {
                MostrarMensagem(
                    "Não foi possível eliminar a submissão: " +
                    ex.Message,
                    true
                );

                AbrirEvento(eventoId);
            }
        }
        private void CarregarSubmissoes(int eventoId)
        {
            const string sql = @"
        SELECT
            ee.Id,
            ee.NomeFicheiro,
            ee.CaminhoFicheiro,
            ee.Observacao,
            ee.CreatedAt,

            CASE
                WHEN ev.DataHora >= SYSDATETIME()
                    THEN CAST(1 AS BIT)
                ELSE CAST(0 AS BIT)
            END AS PodeEliminar

        FROM dbo.EventoEntrega ee

        INNER JOIN dbo.Evento ev
            ON ev.Id = ee.EventoId

        WHERE ee.EventoId = @EventoId
          AND ee.AlunoId = @AlunoId

        ORDER BY ee.CreatedAt DESC;";

            DataTable tabela =
                new DataTable();

            using (SqlConnection conn =
                new SqlConnection(_connectionString))
            using (SqlCommand cmd =
                new SqlCommand(sql, conn))
            using (SqlDataAdapter adapter =
                new SqlDataAdapter(cmd))
            {
                cmd.Parameters
                    .Add("@EventoId", SqlDbType.Int)
                    .Value = eventoId;

                cmd.Parameters
                    .Add("@AlunoId", SqlDbType.Int)
                    .Value = AlunoId;

                adapter.Fill(tabela);
            }

            GridSubmissoes.DataSource =
                tabela;

            GridSubmissoes.DataBind();
        }

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

            if (!PrazoEntregaAberto(eventoId))
            {
                MostrarMensagem(
                    "O prazo deste trabalho já terminou. " +
                    "Já não é possível enviar novas submissões.",
                    true
                );

                AbrirEvento(eventoId);

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

                CarregarEventos();
                CarregarNotas();

                AbrirEvento(eventoId);
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
        private Guid UserIdAtual
        {
            get
            {
                if (Session["UserId"] == null)
                {
                    throw new InvalidOperationException(
                        "A sessão terminou. Faz login novamente."
                    );
                }

                Guid userId;

                if (!Guid.TryParse(
                        Session["UserId"].ToString(),
                        out userId))
                {
                    throw new InvalidOperationException(
                        "O utilizador autenticado é inválido."
                    );
                }

                return userId;
            }
        }

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

        #region verifica o prazo de entrega
        private bool PrazoEntregaAberto(int eventoId)
        {
            const string sql = @"
        SELECT COUNT(1)

        FROM dbo.Evento ev

        INNER JOIN dbo.AlunoTurma at2
            ON at2.TurmaId = ev.TurmaId
           AND at2.Ate IS NULL

        WHERE ev.Id = @EventoId
          AND at2.AlunoId = @AlunoId
          AND ev.DataHora >= SYSDATETIME();";

            using (SqlConnection conn =
                new SqlConnection(_connectionString))
            using (SqlCommand cmd =
                new SqlCommand(sql, conn))
            {
                cmd.Parameters
                    .Add("@EventoId", SqlDbType.Int)
                    .Value = eventoId;

                cmd.Parameters
                    .Add("@AlunoId", SqlDbType.Int)
                    .Value = AlunoId;

                conn.Open();

                return Convert.ToInt32(
                    cmd.ExecuteScalar()
                ) > 0;
            }
        }
        #endregion
    }
}