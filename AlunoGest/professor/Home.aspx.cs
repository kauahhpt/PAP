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
        private readonly string _connectionString =
            ConfigurationManager
                .ConnectionStrings["DefaultConnection"]
                .ConnectionString;


        #region Página

        protected void Page_Load(
            object sender,
            EventArgs e)
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
                MostrarMensagem(
                    "Não foi possível carregar a página: " +
                    ex.Message,
                    true
                );
            }
        }


        protected void DdlTurmas_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            int turmaId =
                TurmaSelecionada;

            if (turmaId > 0)
            {
                Response.Redirect(
                    "~/professor/Home.aspx?turma=" +
                    turmaId
                );
            }
        }

        #endregion


        #region Professor autenticado

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


        private int ProfessorId
        {
            get
            {
                int id;

                if (Session["ProfessorID"] != null &&
                    int.TryParse(
                        Session["ProfessorID"].ToString(),
                        out id))
                {
                    return id;
                }

                const string sql = @"
                    SELECT Id
                    FROM dbo.Professor
                    WHERE UserId = @UserId
                      AND Ativo = 1;";

                using (SqlConnection conn =
                    new SqlConnection(_connectionString))
                using (SqlCommand cmd =
                    new SqlCommand(sql, conn))
                {
                    cmd.Parameters
                        .Add(
                            "@UserId",
                            SqlDbType.UniqueIdentifier
                        )
                        .Value = UserIdAtual;

                    conn.Open();

                    object valor =
                        cmd.ExecuteScalar();

                    if (valor == null ||
                        valor == DBNull.Value)
                    {
                        throw new InvalidOperationException(
                            "Professor não encontrado."
                        );
                    }

                    id =
                        Convert.ToInt32(valor);

                    Session["ProfessorID"] =
                        id;

                    return id;
                }
            }
        }


        private int TurmaSelecionada
        {
            get
            {
                int id;

                return int.TryParse(
                    DdlTurmas.SelectedValue,
                    out id)
                        ? id
                        : 0;
            }
        }

        #endregion


        #region Turmas

        private void CarregarTurmas()
        {
            const string sql = @"
                SELECT DISTINCT
                    td.TurmaId,

                    CAST(
                        t.AnoEscolaridade
                        AS varchar(2)
                    )
                    + '.º'
                    + t.CodigoTurma
                    + ' - '
                    + e.Nome
                    AS Nome

                FROM dbo.TurmaDisciplinaProfessor tdp

                INNER JOIN dbo.TurmaDisciplina td
                    ON td.Id = tdp.TurmaDisciplinaId

                INNER JOIN dbo.Turma t
                    ON t.Id = td.TurmaId

                INNER JOIN dbo.Escola e
                    ON e.Id = t.EscolaId

                WHERE tdp.ProfessorId = @ProfessorId
                  AND tdp.Ate IS NULL

                ORDER BY Nome;";

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
                        "@ProfessorId",
                        SqlDbType.Int
                    )
                    .Value = ProfessorId;

                adapter.Fill(tabela);
            }

            DdlTurmas.DataSource =
                tabela;

            DdlTurmas.DataTextField =
                "Nome";

            DdlTurmas.DataValueField =
                "TurmaId";

            DdlTurmas.DataBind();
        }


        private void SelecionarTurmaDaUrl()
        {
            if (DdlTurmas.Items.Count == 0)
            {
                HdnEvents.Value =
                    "[]";

                return;
            }

            int turmaId;

            bool encontrouTurma =
                int.TryParse(
                    Request.QueryString["turma"],
                    out turmaId
                );

            if (!encontrouTurma)
            {
                encontrouTurma =
                    int.TryParse(
                        Request.QueryString["id"],
                        out turmaId
                    );
            }

            if (encontrouTurma &&
                DdlTurmas.Items.FindByValue(
                    turmaId.ToString()) != null &&
                ProfessorTemTurma(turmaId))
            {
                DdlTurmas.SelectedValue =
                    turmaId.ToString();

                return;
            }

            DdlTurmas.SelectedIndex =
                0;
        }


        private bool ProfessorTemTurma(
            int turmaId)
        {
            if (turmaId <= 0)
            {
                return false;
            }

            const string sql = @"
                SELECT COUNT(1)

                FROM dbo.TurmaDisciplinaProfessor tdp

                INNER JOIN dbo.TurmaDisciplina td
                    ON td.Id = tdp.TurmaDisciplinaId

                WHERE tdp.ProfessorId = @ProfessorId
                  AND td.TurmaId = @TurmaId
                  AND tdp.Ate IS NULL;";

            using (SqlConnection conn =
                new SqlConnection(_connectionString))
            using (SqlCommand cmd =
                new SqlCommand(sql, conn))
            {
                cmd.Parameters
                    .Add(
                        "@ProfessorId",
                        SqlDbType.Int
                    )
                    .Value = ProfessorId;

                cmd.Parameters
                    .Add(
                        "@TurmaId",
                        SqlDbType.Int
                    )
                    .Value = turmaId;

                conn.Open();

                return Convert.ToInt32(
                    cmd.ExecuteScalar()
                ) > 0;
            }
        }

        #endregion


        #region Carregamento geral

        private void CarregarTudo()
        {
            CarregarResumoTurma();
            CarregarDestinatariosPublicacao();
            CarregarPublicacoes();
            CarregarEventos();
            CarregarEntregas();
        }


        private void CarregarResumoTurma()
        {
            int turmaId =
                TurmaSelecionada;

            if (turmaId == 0)
            {
                LblResumoTurma.Text =
                    "Nenhuma turma selecionada.";

                return;
            }

            const string sql = @"
                SELECT
                    CAST(
                        t.AnoEscolaridade
                        AS varchar(2)
                    )
                    + '.º'
                    + t.CodigoTurma
                    AS Turma,

                    e.Nome AS Escola

                FROM dbo.Turma t

                INNER JOIN dbo.Escola e
                    ON e.Id = t.EscolaId

                WHERE t.Id = @TurmaId;";

            using (SqlConnection conn =
                new SqlConnection(_connectionString))
            using (SqlCommand cmd =
                new SqlCommand(sql, conn))
            {
                cmd.Parameters
                    .Add(
                        "@TurmaId",
                        SqlDbType.Int
                    )
                    .Value = turmaId;

                conn.Open();

                using (SqlDataReader reader =
                    cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        LblResumoTurma.Text =
                            "Turma " +
                            reader["Turma"] +
                            " | " +
                            reader["Escola"];
                    }
                }
            }
        }

        #endregion


        #region Criar publicação

        protected void BtnPublicarProfessor_Click(
            object sender,
            EventArgs e)
        {
            int turmaId =
                TurmaSelecionada;

            if (turmaId <= 0 ||
                !ProfessorTemTurma(turmaId))
            {
                MostrarMensagem(
                    "Escolha uma turma válida.",
                    true
                );

                return;
            }

            if (string.IsNullOrWhiteSpace(
                    TxtTituloPublicacaoProfessor.Text))
            {
                MostrarMensagem(
                    "Indique o título da publicação.",
                    true
                );

                return;
            }

            if (string.IsNullOrWhiteSpace(
                    TxtConteudoPublicacaoProfessor.Text))
            {
                MostrarMensagem(
                    "Escreva o conteúdo da publicação.",
                    true
                );

                return;
            }

            if (FilePublicacaoProfessor.HasFile)
            {
                if (FilePublicacaoProfessor
                        .PostedFile
                        .ContentLength >
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
                        FilePublicacaoProfessor.FileName
                    )
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

            List<Guid> destinatarios =
                ObterDestinatariosSelecionados();

            if (!ChkPublicaTurmaProfessor.Checked &&
                destinatarios.Count == 0)
            {
                MostrarMensagem(
                    "Selecione pelo menos um aluno ou professor, " +
                    "ou marque a opção para publicar para toda a turma.",
                    true
                );

                return;
            }

            try
            {
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
                                    ProfessorId,
                                    TurmaId,
                                    Titulo,
                                    Conteudo,
                                    Tipo,
                                    PublicaParaTurma
                                )
                                VALUES
                                (
                                    NULL,
                                    @ProfessorId,
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
                                        "@ProfessorId",
                                        SqlDbType.Int
                                    )
                                    .Value = ProfessorId;

                                cmd.Parameters
                                    .Add(
                                        "@TurmaId",
                                        SqlDbType.Int
                                    )
                                    .Value = turmaId;

                                cmd.Parameters
                                    .Add(
                                        "@Titulo",
                                        SqlDbType.NVarChar,
                                        200
                                    )
                                    .Value =
                                    TxtTituloPublicacaoProfessor
                                        .Text
                                        .Trim();

                                cmd.Parameters
                                    .Add(
                                        "@Conteudo",
                                        SqlDbType.NVarChar,
                                        -1
                                    )
                                    .Value =
                                    TxtConteudoPublicacaoProfessor
                                        .Text
                                        .Trim();

                                cmd.Parameters
                                    .Add(
                                        "@Tipo",
                                        SqlDbType.NVarChar,
                                        30
                                    )
                                    .Value =
                                    DdlTipoPublicacaoProfessor
                                        .SelectedValue;

                                cmd.Parameters
                                    .Add(
                                        "@PublicaParaTurma",
                                        SqlDbType.Bit
                                    )
                                    .Value =
                                    ChkPublicaTurmaProfessor.Checked;

                                publicacaoId =
                                    Convert.ToInt32(
                                        cmd.ExecuteScalar()
                                    );
                            }

                            if (!ChkPublicaTurmaProfessor.Checked)
                            {
                                foreach (Guid destinatario
                                    in destinatarios)
                                {
                                    GuardarDestinatarioPublicacao(
                                        publicacaoId,
                                        turmaId,
                                        destinatario,
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

                if (FilePublicacaoProfessor.HasFile)
                {
                    GuardarAnexoPublicacaoProfessor(
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
                    "Não foi possível criar a publicação: " +
                    ex.Message,
                    true
                );
            }
        }


        private List<Guid> ObterDestinatariosSelecionados()
        {
            List<Guid> destinatarios =
                new List<Guid>();

            foreach (ListItem item
                in CblAlunosDestinatariosProfessor.Items)
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
                in CblProfessoresDestinatariosProfessor.Items)
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
                        "já não pertence à turma."
                    );
                }
            }
        }


        private void GuardarAnexoPublicacaoProfessor(
            int publicacaoId)
        {
            string pasta =
                Server.MapPath(
                    "~/uploads/publicacoes/"
                );

            Directory.CreateDirectory(
                pasta
            );

            string nomeOriginal =
                Path.GetFileName(
                    FilePublicacaoProfessor.FileName
                );

            string nomeGuardado =
                Guid.NewGuid().ToString("N") +
                "_" +
                nomeOriginal;

            FilePublicacaoProfessor.SaveAs(
                Path.Combine(
                    pasta,
                    nomeGuardado
                )
            );

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
                cmd.Parameters
                    .Add(
                        "@PublicacaoId",
                        SqlDbType.Int
                    )
                    .Value = publicacaoId;

                cmd.Parameters
                    .Add(
                        "@NomeFicheiro",
                        SqlDbType.NVarChar,
                        255
                    )
                    .Value = nomeOriginal;

                cmd.Parameters
                    .Add(
                        "@CaminhoFicheiro",
                        SqlDbType.NVarChar,
                        500
                    )
                    .Value =
                    ResolveUrl(
                        "~/uploads/publicacoes/" +
                        nomeGuardado
                    );

                conn.Open();

                cmd.ExecuteNonQuery();
            }
        }


        private void LimparFormularioPublicacao()
        {
            TxtTituloPublicacaoProfessor.Text =
                string.Empty;

            TxtConteudoPublicacaoProfessor.Text =
                string.Empty;

            DdlTipoPublicacaoProfessor.SelectedIndex =
                0;

            ChkPublicaTurmaProfessor.Checked =
                false;

            foreach (ListItem item
                in CblAlunosDestinatariosProfessor.Items)
            {
                item.Selected =
                    false;
            }

            foreach (ListItem item
                in CblProfessoresDestinatariosProfessor.Items)
            {
                item.Selected =
                    false;
            }
        }

        #endregion


        #region Destinatários

        private void CarregarDestinatariosPublicacao()
        {
            CblAlunosDestinatariosProfessor
                .Items
                .Clear();

            CblProfessoresDestinatariosProfessor
                .Items
                .Clear();

            int turmaId =
                TurmaSelecionada;

            if (turmaId <= 0)
            {
                LblSemAlunosDestinatariosProfessor.Visible =
                    true;

                LblSemProfessoresDestinatariosProfessor.Visible =
                    true;

                return;
            }

            CarregarAlunosDestinatarios(
                turmaId
            );

            CarregarProfessoresDestinatarios(
                turmaId
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

                FROM dbo.AlunoTurma alunoTurma

                INNER JOIN dbo.Aluno a
                    ON a.Id = alunoTurma.AlunoId

                WHERE alunoTurma.TurmaId = @TurmaId
                  AND alunoTurma.Ate IS NULL
                  AND a.Ativo = 1
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
                    .Add(
                        "@TurmaId",
                        SqlDbType.Int
                    )
                    .Value = turmaId;

                adapter.Fill(tabela);
            }

            foreach (DataRow row in tabela.Rows)
            {
                string numeroProcesso =
                    row["NumeroProcesso"] ==
                    DBNull.Value
                        ? "sem número de processo"
                        : row["NumeroProcesso"].ToString();

                string texto =
                    row["NomeCompleto"] +
                    " — Processo: " +
                    numeroProcesso;

                CblAlunosDestinatariosProfessor
                    .Items
                    .Add(
                        new ListItem(
                            texto,
                            row["UserId"].ToString()
                        )
                    );
            }

            LblSemAlunosDestinatariosProfessor.Visible =
                tabela.Rows.Count == 0;
        }


        private void CarregarProfessoresDestinatarios(
            int turmaId)
        {
            const string sql = @"
                SELECT
                    p.Id,
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
                  AND p.Id <> @ProfessorId

                GROUP BY
                    p.Id,
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
                    .Add(
                        "@TurmaId",
                        SqlDbType.Int
                    )
                    .Value = turmaId;

                cmd.Parameters
                    .Add(
                        "@ProfessorId",
                        SqlDbType.Int
                    )
                    .Value = ProfessorId;

                adapter.Fill(tabela);
            }

            foreach (DataRow row in tabela.Rows)
            {
                string disciplinas =
                    row["Disciplinas"] ==
                    DBNull.Value
                        ? "Professor"
                        : row["Disciplinas"].ToString();

                string texto =
                    row["Nome"] +
                    " — " +
                    disciplinas;

                CblProfessoresDestinatariosProfessor
                    .Items
                    .Add(
                        new ListItem(
                            texto,
                            row["UserId"].ToString()
                        )
                    );
            }

            LblSemProfessoresDestinatariosProfessor.Visible =
                tabela.Rows.Count == 0;
        }

        #endregion


        #region Feed

        private void CarregarPublicacoes()
        {
            int turmaId =
                TurmaSelecionada;

            DataTable tabela =
                new DataTable();

            if (turmaId <= 0)
            {
                RepeaterPublicacoes.DataSource =
                    tabela;

                RepeaterPublicacoes.DataBind();

                PainelSemPublicacoes.Visible =
                    true;

                return;
            }

            const string sql = @"
                SELECT
                    p.Id,
                    p.Titulo,
                    p.Conteudo,
                    p.Tipo,
                    p.CreatedAt,
                    p.PublicaParaTurma,

                    COALESCE(
                        aluno.NomeCompleto,
                        professor.Nome,
                        N'Utilizador'
                    ) AS NomeCompleto,

                    aluno.Foto,

                    CASE
                        WHEN p.ProfessorId IS NOT NULL
                            THEN N'Professor'
                        ELSE N'Aluno'
                    END AS AutorTipo,

                    COUNT(publicacaoLike.Id)
                        AS TotalLikes

                FROM dbo.Publicacao p

                LEFT JOIN dbo.Aluno aluno
                    ON aluno.Id = p.AlunoId

                LEFT JOIN dbo.Professor professor
                    ON professor.Id = p.ProfessorId

                LEFT JOIN dbo.PublicacaoLike publicacaoLike
                    ON publicacaoLike.PublicacaoId = p.Id

                WHERE p.TurmaId = @TurmaId

                  AND
                  (
                      p.ProfessorId = @ProfessorId

                      OR

                      p.PublicaParaTurma = 1

                      OR

                      EXISTS
                      (
                          SELECT 1

                          FROM dbo.PublicacaoDestinatario destinatario

                          WHERE destinatario.PublicacaoId = p.Id

                            AND destinatario.DestinatarioUserId =
                                @UserIdAtual
                      )
                  )

                GROUP BY
                    p.Id,
                    p.Titulo,
                    p.Conteudo,
                    p.Tipo,
                    p.CreatedAt,
                    p.PublicaParaTurma,
                    p.ProfessorId,
                    aluno.NomeCompleto,
                    aluno.Foto,
                    professor.Nome

                ORDER BY p.CreatedAt DESC;";

            using (SqlConnection conn =
                new SqlConnection(_connectionString))
            using (SqlCommand cmd =
                new SqlCommand(sql, conn))
            using (SqlDataAdapter adapter =
                new SqlDataAdapter(cmd))
            {
                cmd.Parameters
                    .Add(
                        "@TurmaId",
                        SqlDbType.Int
                    )
                    .Value = turmaId;

                cmd.Parameters
                    .Add(
                        "@ProfessorId",
                        SqlDbType.Int
                    )
                    .Value = ProfessorId;

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

            PainelSemPublicacoes.Visible =
                tabela.Rows.Count == 0;
        }


        protected bool TemFotoPublicacao(
            object foto)
        {
            if (foto == null ||
                foto == DBNull.Value)
            {
                return false;
            }

            return !string.IsNullOrWhiteSpace(
                foto.ToString().Trim()
            );
        }


        protected string ObterFotoPublicacao(
            object foto)
        {
            if (!TemFotoPublicacao(foto))
            {
                return "";
            }

            string caminho =
                foto.ToString().Trim();

            if (caminho.StartsWith(
                    "http://",
                    StringComparison.OrdinalIgnoreCase) ||
                caminho.StartsWith(
                    "https://",
                    StringComparison.OrdinalIgnoreCase))
            {
                return caminho;
            }

            if (caminho.StartsWith("~/"))
            {
                return ResolveUrl(caminho);
            }

            if (caminho.StartsWith("/"))
            {
                return ResolveUrl(
                    "~" + caminho
                );
            }

            return ResolveUrl(
                "~/" + caminho
            );
        }


        protected string ObterInicial(
            object nome)
        {
            if (nome == null ||
                nome == DBNull.Value)
            {
                return "?";
            }

            string texto =
                nome.ToString().Trim();

            if (string.IsNullOrWhiteSpace(texto))
            {
                return "?";
            }

            return texto
                .Substring(0, 1)
                .ToUpper();
        }

        #endregion


        #region Eventos

        protected void BtnGuardar_Click(
            object sender,
            EventArgs e)
        {
            int turmaId =
                TurmaSelecionada;

            if (turmaId <= 0 ||
                !ProfessorTemTurma(turmaId))
            {
                MostrarMensagem(
                    "Escolha uma turma válida.",
                    true
                );

                return;
            }

            if (string.IsNullOrWhiteSpace(
                    TxtTitulo.Text))
            {
                MostrarMensagem(
                    "Indique o título.",
                    true
                );

                return;
            }

            DateTime dataHora;

            if (!DateTime.TryParse(
                    TxtDataHora.Text,
                    out dataHora))
            {
                MostrarMensagem(
                    "Indique uma data válida.",
                    true
                );

                return;
            }

            try
            {
                int eventoId;

                if (string.IsNullOrEmpty(
                        HdnEventoId.Value))
                {
                    eventoId =
                        InserirEvento(
                            turmaId,
                            dataHora
                        );
                }
                else
                {
                    eventoId =
                        Convert.ToInt32(
                            HdnEventoId.Value
                        );

                    AtualizarEvento(
                        eventoId,
                        turmaId,
                        dataHora
                    );
                }

                if (FileAnexo.HasFile)
                {
                    GuardarAnexoProfessor(
                        eventoId
                    );
                }

                MostrarMensagem(
                    "Evento guardado.",
                    false
                );

                LimparEvento();
                CarregarTudo();
            }
            catch (Exception ex)
            {
                MostrarMensagem(
                    "Não foi possível guardar: " +
                    ex.Message,
                    true
                );
            }
        }


        protected void BtnLimpar_Click(
            object sender,
            EventArgs e)
        {
            LimparEvento();
        }


        private void CarregarEventos()
        {
            int turmaId =
                TurmaSelecionada;

            DataTable tabela =
                new DataTable();

            if (turmaId > 0)
            {
                const string sql = @"
                    SELECT
                        Id,
                        Tipo,
                        Titulo,
                        DataHora

                    FROM dbo.Evento

                    WHERE TurmaId = @TurmaId

                    ORDER BY DataHora DESC;";

                using (SqlConnection conn =
                    new SqlConnection(_connectionString))
                using (SqlCommand cmd =
                    new SqlCommand(sql, conn))
                using (SqlDataAdapter adapter =
                    new SqlDataAdapter(cmd))
                {
                    cmd.Parameters
                        .Add(
                            "@TurmaId",
                            SqlDbType.Int
                        )
                        .Value = turmaId;

                    adapter.Fill(tabela);
                }
            }

            GridEventos.DataSource =
                tabela;

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

                eventos.Add(
                    new
                    {
                        id =
                            row["Id"],

                        title =
                            row["Titulo"].ToString(),

                        start =
                            Convert.ToDateTime(
                                row["DataHora"]
                            )
                            .ToString(
                                "yyyy-MM-ddTHH:mm:ss"
                            ),

                        color =
                            cor
                    }
                );
            }

            HdnEvents.Value =
                new JavaScriptSerializer()
                    .Serialize(eventos);
        }


        protected void GridEventos_RowCommand(
            object sender,
            GridViewCommandEventArgs e)
        {
            int index;

            if (!int.TryParse(
                    e.CommandArgument.ToString(),
                    out index) ||
                index < 0 ||
                index >= GridEventos.Rows.Count)
            {
                return;
            }

            int eventoId =
                Convert.ToInt32(
                    GridEventos
                        .DataKeys[index]
                        .Value
                );

            int turmaId =
                TurmaSelecionada;

            if (!ProfessorTemTurma(turmaId))
            {
                return;
            }

            if (e.CommandName ==
                "EditarEvento")
            {
                CarregarEventoParaFormulario(
                    eventoId,
                    turmaId
                );
            }
            else if (e.CommandName ==
                "ApagarEvento")
            {
                try
                {
                    ApagarEvento(
                        eventoId,
                        turmaId
                    );

                    MostrarMensagem(
                        "Evento apagado.",
                        false
                    );

                    LimparEvento();
                    CarregarTudo();
                }
                catch (Exception ex)
                {
                    MostrarMensagem(
                        "Não foi possível apagar o evento: " +
                        ex.Message,
                        true
                    );
                }
            }
        }


        private int InserirEvento(
            int turmaId,
            DateTime dataHora)
        {
            const string sql = @"
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
                    NULL,
                    @TurmaId,
                    @Titulo,
                    @Tipo,
                    @DataHora
                );

                SELECT CAST(
                    SCOPE_IDENTITY()
                    AS int
                );";

            using (SqlConnection conn =
                new SqlConnection(_connectionString))
            using (SqlCommand cmd =
                new SqlCommand(sql, conn))
            {
                ParametrosEvento(
                    cmd,
                    turmaId,
                    dataHora
                );

                conn.Open();

                return Convert.ToInt32(
                    cmd.ExecuteScalar()
                );
            }
        }


        private void AtualizarEvento(
            int eventoId,
            int turmaId,
            DateTime dataHora)
        {
            const string sql = @"
                UPDATE dbo.Evento

                SET
                    Titulo = @Titulo,
                    Tipo = @Tipo,
                    DataHora = @DataHora

                WHERE Id = @Id
                  AND TurmaId = @TurmaId;";

            using (SqlConnection conn =
                new SqlConnection(_connectionString))
            using (SqlCommand cmd =
                new SqlCommand(sql, conn))
            {
                ParametrosEvento(
                    cmd,
                    turmaId,
                    dataHora
                );

                cmd.Parameters
                    .Add(
                        "@Id",
                        SqlDbType.Int
                    )
                    .Value = eventoId;

                conn.Open();

                cmd.ExecuteNonQuery();
            }
        }


        private void ParametrosEvento(
            SqlCommand cmd,
            int turmaId,
            DateTime dataHora)
        {
            cmd.Parameters
                .Add(
                    "@TurmaId",
                    SqlDbType.Int
                )
                .Value = turmaId;

            cmd.Parameters
                .Add(
                    "@Titulo",
                    SqlDbType.NVarChar,
                    200
                )
                .Value = TxtTitulo.Text.Trim();

            cmd.Parameters
                .Add(
                    "@Tipo",
                    SqlDbType.NVarChar,
                    50
                )
                .Value = DdlTipo.SelectedValue;

            cmd.Parameters
                .Add(
                    "@DataHora",
                    SqlDbType.DateTime
                )
                .Value = dataHora;
        }


        private void CarregarEventoParaFormulario(
            int eventoId,
            int turmaId)
        {
            const string sql = @"
                SELECT
                    Id,
                    Tipo,
                    Titulo,
                    DataHora

                FROM dbo.Evento

                WHERE Id = @Id
                  AND TurmaId = @TurmaId;";

            using (SqlConnection conn =
                new SqlConnection(_connectionString))
            using (SqlCommand cmd =
                new SqlCommand(sql, conn))
            {
                cmd.Parameters
                    .Add(
                        "@Id",
                        SqlDbType.Int
                    )
                    .Value = eventoId;

                cmd.Parameters
                    .Add(
                        "@TurmaId",
                        SqlDbType.Int
                    )
                    .Value = turmaId;

                conn.Open();

                using (SqlDataReader reader =
                    cmd.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        return;
                    }

                    HdnEventoId.Value =
                        reader["Id"].ToString();

                    DdlTipo.SelectedValue =
                        reader["Tipo"].ToString();

                    TxtTitulo.Text =
                        reader["Titulo"].ToString();

                    TxtDataHora.Text =
                        Convert.ToDateTime(
                            reader["DataHora"]
                        )
                        .ToString(
                            "yyyy-MM-ddTHH:mm"
                        );
                }
            }
        }


        private void ApagarEvento(
            int eventoId,
            int turmaId)
        {
            using (SqlConnection conn =
                new SqlConnection(_connectionString))
            {
                conn.Open();

                SqlTransaction transacao =
                    conn.BeginTransaction();

                try
                {
                    Executar(
                        conn,
                        transacao,
                        @"
                            DELETE ee

                            FROM dbo.EventoEntrega ee

                            INNER JOIN dbo.Evento ev
                                ON ev.Id = ee.EventoId

                            WHERE ev.Id = @Id
                              AND ev.TurmaId = @TurmaId;
                        ",
                        eventoId,
                        turmaId
                    );

                    Executar(
                        conn,
                        transacao,
                        @"
                            DELETE FROM dbo.EventoAnexo
                            WHERE EventoId = @Id;
                        ",
                        eventoId,
                        turmaId
                    );

                    Executar(
                        conn,
                        transacao,
                        @"
                            DELETE FROM dbo.Evento
                            WHERE Id = @Id
                              AND TurmaId = @TurmaId;
                        ",
                        eventoId,
                        turmaId
                    );

                    transacao.Commit();
                }
                catch
                {
                    transacao.Rollback();
                    throw;
                }
            }
        }


        private void GuardarAnexoProfessor(
            int eventoId)
        {
            if (FileAnexo.PostedFile.ContentLength >
                10 * 1024 * 1024)
            {
                throw new InvalidOperationException(
                    "O anexo excede 10 MB."
                );
            }

            string pasta =
                Server.MapPath(
                    "~/uploads/eventos/"
                );

            Directory.CreateDirectory(
                pasta
            );

            string original =
                Path.GetFileName(
                    FileAnexo.FileName
                );

            string nome =
                Guid.NewGuid().ToString("N") +
                "_" +
                original;

            FileAnexo.SaveAs(
                Path.Combine(
                    pasta,
                    nome
                )
            );

            const string sql = @"
                INSERT INTO dbo.EventoAnexo
                (
                    EventoId,
                    NomeFicheiro,
                    CaminhoFicheiro
                )
                VALUES
                (
                    @EventoId,
                    @Nome,
                    @Caminho
                );";

            using (SqlConnection conn =
                new SqlConnection(_connectionString))
            using (SqlCommand cmd =
                new SqlCommand(sql, conn))
            {
                cmd.Parameters
                    .Add(
                        "@EventoId",
                        SqlDbType.Int
                    )
                    .Value = eventoId;

                cmd.Parameters
                    .Add(
                        "@Nome",
                        SqlDbType.NVarChar,
                        255
                    )
                    .Value = original;

                cmd.Parameters
                    .Add(
                        "@Caminho",
                        SqlDbType.NVarChar,
                        500
                    )
                    .Value =
                    ResolveUrl(
                        "~/uploads/eventos/" +
                        nome
                    );

                conn.Open();

                cmd.ExecuteNonQuery();
            }
        }

        #endregion


        #region Entregas e avaliações

        private void CarregarEntregas()
        {
            int turmaId =
                TurmaSelecionada;

            DataTable tabela =
                new DataTable();

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

                    INNER JOIN dbo.Evento ev
                        ON ev.Id = ee.EventoId

                    INNER JOIN dbo.Aluno a
                        ON a.Id = ee.AlunoId

                    WHERE ev.TurmaId = @TurmaId

                    ORDER BY ee.CreatedAt DESC;";

                using (SqlConnection conn =
                    new SqlConnection(_connectionString))
                using (SqlCommand cmd =
                    new SqlCommand(sql, conn))
                using (SqlDataAdapter adapter =
                    new SqlDataAdapter(cmd))
                {
                    cmd.Parameters
                        .Add(
                            "@TurmaId",
                            SqlDbType.Int
                        )
                        .Value = turmaId;

                    adapter.Fill(tabela);
                }
            }

            GridEntregas.DataSource =
                tabela;

            GridEntregas.DataBind();
        }


        protected void GridEntregas_RowCommand(
            object sender,
            GridViewCommandEventArgs e)
        {
            int index;

            if (e.CommandName !=
                    "AvaliarEntrega" ||
                !int.TryParse(
                    e.CommandArgument.ToString(),
                    out index) ||
                index < 0 ||
                index >= GridEntregas.Rows.Count)
            {
                return;
            }

            int entregaId =
                Convert.ToInt32(
                    GridEntregas
                        .DataKeys[index]
                        .Value
                );

            CarregarEntrega(
                entregaId
            );
        }


        private void CarregarEntrega(
            int entregaId)
        {
            const string sql = @"
                SELECT
                    ee.Id,
                    ee.Nota,
                    ee.Feedback

                FROM dbo.EventoEntrega ee

                INNER JOIN dbo.Evento ev
                    ON ev.Id = ee.EventoId

                WHERE ee.Id = @Id
                  AND ev.TurmaId = @TurmaId;";

            using (SqlConnection conn =
                new SqlConnection(_connectionString))
            using (SqlCommand cmd =
                new SqlCommand(sql, conn))
            {
                cmd.Parameters
                    .Add(
                        "@Id",
                        SqlDbType.Int
                    )
                    .Value = entregaId;

                cmd.Parameters
                    .Add(
                        "@TurmaId",
                        SqlDbType.Int
                    )
                    .Value = TurmaSelecionada;

                conn.Open();

                using (SqlDataReader reader =
                    cmd.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        return;
                    }

                    HdnEntregaId.Value =
                        reader["Id"].ToString();

                    TxtNota.Text =
                        reader["Nota"] == DBNull.Value
                            ? ""
                            : reader["Nota"].ToString();

                    TxtFeedback.Text =
                        reader["Feedback"] == DBNull.Value
                            ? ""
                            : reader["Feedback"].ToString();

                    PainelAvaliacao.Visible =
                        true;
                }
            }
        }


        protected void BtnGuardarAvaliacao_Click(
            object sender,
            EventArgs e)
        {
            int entregaId;
            decimal nota;

            if (!int.TryParse(
                    HdnEntregaId.Value,
                    out entregaId))
            {
                MostrarMensagem(
                    "Escolha uma entrega.",
                    true
                );

                return;
            }

            if (!decimal.TryParse(
                    TxtNota.Text.Replace(".", ","),
                    out nota) ||
                nota < 0 ||
                nota > 20)
            {
                MostrarMensagem(
                    "A nota deve estar entre 0 e 20.",
                    true
                );

                return;
            }

            const string sql = @"
                UPDATE ee

                SET
                    Nota = @Nota,
                    Feedback = @Feedback,
                    AvaliadoEm = SYSDATETIME()

                FROM dbo.EventoEntrega ee

                INNER JOIN dbo.Evento ev
                    ON ev.Id = ee.EventoId

                WHERE ee.Id = @Id
                  AND ev.TurmaId = @TurmaId;";

            using (SqlConnection conn =
                new SqlConnection(_connectionString))
            using (SqlCommand cmd =
                new SqlCommand(sql, conn))
            {
                cmd.Parameters
                    .Add(
                        "@Id",
                        SqlDbType.Int
                    )
                    .Value = entregaId;

                cmd.Parameters
                    .Add(
                        "@TurmaId",
                        SqlDbType.Int
                    )
                    .Value = TurmaSelecionada;

                cmd.Parameters
                    .Add(
                        "@Nota",
                        SqlDbType.Decimal
                    )
                    .Value = nota;

                cmd.Parameters
                    .Add(
                        "@Feedback",
                        SqlDbType.NVarChar,
                        1000
                    )
                    .Value =
                    string.IsNullOrWhiteSpace(
                        TxtFeedback.Text
                    )
                        ? (object)DBNull.Value
                        : TxtFeedback.Text.Trim();

                conn.Open();

                cmd.ExecuteNonQuery();
            }

            PainelAvaliacao.Visible =
                false;

            MostrarMensagem(
                "Avaliação guardada.",
                false
            );

            CarregarEntregas();
        }


        protected void BtnCancelarAvaliacao_Click(
            object sender,
            EventArgs e)
        {
            PainelAvaliacao.Visible =
                false;
        }

        #endregion


        #region Base de dados

        private void GarantirTabelaEntregas()
        {
            const string sql = @"
                IF OBJECT_ID(
                    'dbo.EventoEntrega',
                    'U'
                ) IS NULL
                BEGIN
                    CREATE TABLE dbo.EventoEntrega
                    (
                        Id int IDENTITY(1,1)
                            NOT NULL PRIMARY KEY,

                        EventoId int NOT NULL,
                        AlunoId int NOT NULL,

                        NomeFicheiro
                            nvarchar(255) NOT NULL,

                        CaminhoFicheiro
                            nvarchar(500) NOT NULL,

                        Observacao
                            nvarchar(1000) NULL,

                        Nota
                            decimal(5,2) NULL,

                        Feedback
                            nvarchar(1000) NULL,

                        AvaliadoEm
                            datetime2 NULL,

                        CreatedAt
                            datetime2 NOT NULL

                            CONSTRAINT
                                DF_EventoEntrega_CreatedAt

                            DEFAULT SYSDATETIME()
                    );
                END;";

            using (SqlConnection conn =
                new SqlConnection(_connectionString))
            using (SqlCommand cmd =
                new SqlCommand(sql, conn))
            {
                conn.Open();

                cmd.ExecuteNonQuery();
            }
        }


        private void Executar(
            SqlConnection conn,
            SqlTransaction transacao,
            string sql,
            int id,
            int turmaId)
        {
            using (SqlCommand cmd =
                new SqlCommand(
                    sql,
                    conn,
                    transacao))
            {
                cmd.Parameters
                    .Add(
                        "@Id",
                        SqlDbType.Int
                    )
                    .Value = id;

                cmd.Parameters
                    .Add(
                        "@TurmaId",
                        SqlDbType.Int
                    )
                    .Value = turmaId;

                cmd.ExecuteNonQuery();
            }
        }

        #endregion


        #region Utilidades

        private void LimparEvento()
        {
            HdnEventoId.Value =
                string.Empty;

            TxtTitulo.Text =
                string.Empty;

            TxtDataHora.Text =
                string.Empty;

            DdlTipo.SelectedIndex =
                0;
        }


        private void MostrarMensagem(
            string texto,
            bool erro)
        {
            LblMensagem.Text =
                texto;

            LblMensagem.CssClass =
                erro
                    ? "alert alert-warning d-block"
                    : "alert alert-success d-block";

            LblMensagem.Visible =
                true;
        }

        #endregion
    }
}