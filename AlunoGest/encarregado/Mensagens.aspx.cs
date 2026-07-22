using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AlunoGest.encarregado
{
    public partial class Mensagens : Page
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
            if (!Request.IsAuthenticated ||
                !Roles.IsUserInRole(
                    User.Identity.Name,
                    "encarregado"))
            {
                TerminarSessao();
                return;
            }

            if (!IsPostBack)
            {
                try
                {
                    CarregarPagina();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.TraceError(
                        "Erro ao carregar mensagens do encarregado: " +
                        ex
                    );

                    MostrarMensagem(
                        "Não foi possível carregar as mensagens: " +
                        ex.Message,
                        true
                    );
                }
            }
        }

        #endregion


        #region Carregamento principal

        private void CarregarPagina()
        {
            LimparMensagem();

            int encarregadoId;

            if (!TryGetEncarregadoId(
                    out encarregadoId))
            {
                TerminarSessao();
                return;
            }

            int educandoId;

            if (!TryGetEducandoId(
                    out educandoId))
            {
                MostrarSemEducando();
                return;
            }

            string nomeEducando =
                ObterNomeEducandoAssociado(
                    educandoId,
                    encarregadoId
                );

            if (string.IsNullOrWhiteSpace(
                    nomeEducando))
            {
                Session["EducandoId"] = null;

                MostrarSemEducando();

                MostrarMensagem(
                    "O educando selecionado não está associado " +
                    "à sua conta ou já não está ativo.",
                    true
                );

                return;
            }

            LblEducandoSelecionado.Text =
                nomeEducando;

            LblEducandoSelecionado.Visible =
                true;

            PnlSemEducando.Visible =
                false;

            PnlConteudo.Visible =
                true;

            CarregarProfessores(
                educandoId,
                encarregadoId
            );

            CarregarConversas(
                educandoId,
                encarregadoId,
                nomeEducando
            );

            LimparConversaAtual();
        }

        #endregion


        #region Utilizador autenticado

        private Guid UserIdAtual
        {
            get
            {
                MembershipUser utilizador =
                    Membership.GetUser(
                        User.Identity.Name,
                        false
                    );

                if (utilizador == null ||
                    utilizador.ProviderUserKey == null)
                {
                    throw new InvalidOperationException(
                        "Não foi possível identificar o utilizador."
                    );
                }

                Guid userId;

                if (!Guid.TryParse(
                        utilizador.ProviderUserKey.ToString(),
                        out userId))
                {
                    throw new InvalidOperationException(
                        "O identificador da conta é inválido."
                    );
                }

                Session["UserId"] =
                    userId;

                return userId;
            }
        }

        #endregion


        #region Educando e encarregado

        private bool TryGetEducandoId(
            out int educandoId)
        {
            educandoId = 0;

            if (Session["EducandoId"] == null)
            {
                return false;
            }

            return int.TryParse(
                Session["EducandoId"].ToString(),
                out educandoId
            );
        }


        private bool TryGetEncarregadoId(
            out int encarregadoId)
        {
            encarregadoId = 0;

            if (Session[
                    "EncarregadoEducacaoId"
                ] != null &&
                int.TryParse(
                    Session[
                        "EncarregadoEducacaoId"
                    ].ToString(),
                    out encarregadoId))
            {
                return true;
            }

            const string sql = @"
                SELECT TOP 1
                    Id

                FROM dbo.EncarregadoEducacao

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

                object resultado =
                    cmd.ExecuteScalar();

                if (resultado == null ||
                    resultado == DBNull.Value)
                {
                    return false;
                }

                encarregadoId =
                    Convert.ToInt32(
                        resultado
                    );

                Session[
                    "EncarregadoEducacaoId"
                ] = encarregadoId;

                return true;
            }
        }


        private string ObterNomeEducandoAssociado(
            int educandoId,
            int encarregadoId)
        {
            const string sql = @"
                SELECT TOP 1
                    a.NomeCompleto

                FROM dbo.Aluno a

                INNER JOIN dbo.AlunoEncarregado ae
                    ON ae.AlunoId = a.Id

                INNER JOIN dbo.EncarregadoEducacao ee
                    ON ee.Id =
                       ae.EncarregadoEducacaoId

                WHERE a.Id = @AlunoId

                  AND ae.EncarregadoEducacaoId =
                      @EncarregadoEducacaoId

                  AND a.Ativo = 1
                  AND ae.Ativo = 1
                  AND ee.Ativo = 1;";

            using (SqlConnection conn =
                new SqlConnection(_connectionString))
            using (SqlCommand cmd =
                new SqlCommand(sql, conn))
            {
                cmd.Parameters
                    .Add(
                        "@AlunoId",
                        SqlDbType.Int
                    )
                    .Value = educandoId;

                cmd.Parameters
                    .Add(
                        "@EncarregadoEducacaoId",
                        SqlDbType.Int
                    )
                    .Value = encarregadoId;

                conn.Open();

                object resultado =
                    cmd.ExecuteScalar();

                return resultado == null ||
                       resultado == DBNull.Value
                    ? null
                    : resultado.ToString().Trim();
            }
        }

        #endregion


        #region Professores disponíveis

        private void CarregarProfessores(
            int educandoId,
            int encarregadoId)
        {
            const string sql = @"
                SELECT
                    dados.UserId,
                    dados.Nome,

                    STRING_AGG(
                        dados.Disciplina,
                        N', '
                    ) AS Disciplinas

                FROM
                (
                    SELECT DISTINCT
                        p.UserId,
                        p.Nome,
                        d.Nome AS Disciplina

                    FROM dbo.Aluno a

                    INNER JOIN dbo.AlunoEncarregado ae
                        ON ae.AlunoId = a.Id

                    INNER JOIN dbo.AlunoTurma at2
                        ON at2.AlunoId = a.Id

                    INNER JOIN dbo.TurmaDisciplina td
                        ON td.TurmaId = at2.TurmaId

                    INNER JOIN dbo.TurmaDisciplinaProfessor tdp
                        ON tdp.TurmaDisciplinaId = td.Id

                    INNER JOIN dbo.Professor p
                        ON p.Id = tdp.ProfessorId

                    INNER JOIN dbo.Disciplina d
                        ON d.Id = td.DisciplinaId

                    WHERE a.Id = @AlunoId

                      AND ae.EncarregadoEducacaoId =
                          @EncarregadoEducacaoId

                      AND a.Ativo = 1
                      AND ae.Ativo = 1
                      AND at2.Ate IS NULL
                      AND tdp.Ate IS NULL
                      AND p.Ativo = 1
                      AND p.UserId IS NOT NULL

                ) AS dados

                GROUP BY
                    dados.UserId,
                    dados.Nome

                ORDER BY
                    dados.Nome;";

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
                    .Value = educandoId;

                cmd.Parameters
                    .Add(
                        "@EncarregadoEducacaoId",
                        SqlDbType.Int
                    )
                    .Value = encarregadoId;

                adapter.Fill(tabela);
            }

            DdlProfessores.Items.Clear();

            DdlProfessores.Items.Add(
                new ListItem(
                    "Selecione um professor",
                    string.Empty
                )
            );

            foreach (DataRow row
                in tabela.Rows)
            {
                string nome =
                    ValorTexto(
                        row["Nome"]
                    );

                string disciplinas =
                    ValorTexto(
                        row["Disciplinas"]
                    );

                string texto =
                    string.IsNullOrWhiteSpace(
                        disciplinas)
                        ? nome
                        : nome +
                          " — " +
                          disciplinas;

                DdlProfessores.Items.Add(
                    new ListItem(
                        texto,
                        row["UserId"].ToString()
                    )
                );
            }

            DdlProfessores.Enabled =
                tabela.Rows.Count > 0;

            BtnIniciarConversa.Enabled =
                tabela.Rows.Count > 0;
        }


        private bool ProfessorPermitido(
            int educandoId,
            int encarregadoId,
            Guid professorUserId)
        {
            const string sql = @"
                SELECT
                    CASE
                        WHEN EXISTS
                        (
                            SELECT 1

                            FROM dbo.Aluno a

                            INNER JOIN dbo.AlunoEncarregado ae
                                ON ae.AlunoId = a.Id

                            INNER JOIN dbo.AlunoTurma at2
                                ON at2.AlunoId = a.Id

                            INNER JOIN dbo.TurmaDisciplina td
                                ON td.TurmaId = at2.TurmaId

                            INNER JOIN dbo.TurmaDisciplinaProfessor tdp
                                ON tdp.TurmaDisciplinaId = td.Id

                            INNER JOIN dbo.Professor p
                                ON p.Id = tdp.ProfessorId

                            WHERE a.Id = @AlunoId

                              AND ae.EncarregadoEducacaoId =
                                  @EncarregadoEducacaoId

                              AND p.UserId =
                                  @ProfessorUserId

                              AND a.Ativo = 1
                              AND ae.Ativo = 1
                              AND at2.Ate IS NULL
                              AND tdp.Ate IS NULL
                              AND p.Ativo = 1
                        )
                        THEN 1
                        ELSE 0
                    END;";

            using (SqlConnection conn =
                new SqlConnection(_connectionString))
            using (SqlCommand cmd =
                new SqlCommand(sql, conn))
            {
                cmd.Parameters
                    .Add(
                        "@AlunoId",
                        SqlDbType.Int
                    )
                    .Value = educandoId;

                cmd.Parameters
                    .Add(
                        "@EncarregadoEducacaoId",
                        SqlDbType.Int
                    )
                    .Value = encarregadoId;

                cmd.Parameters
                    .Add(
                        "@ProfessorUserId",
                        SqlDbType.UniqueIdentifier
                    )
                    .Value = professorUserId;

                conn.Open();

                return Convert.ToInt32(
                    cmd.ExecuteScalar()
                ) == 1;
            }
        }

        #endregion


        #region Iniciar conversa

        protected void BtnIniciarConversa_Click(
            object sender,
            EventArgs e)
        {
            LimparMensagem();

            int encarregadoId;
            int educandoId;

            if (!TryGetEncarregadoId(
                    out encarregadoId) ||
                !TryGetEducandoId(
                    out educandoId))
            {
                MostrarMensagem(
                    "Não foi possível identificar o educando.",
                    true
                );

                return;
            }

            Guid professorUserId;

            if (!Guid.TryParse(
                    DdlProfessores.SelectedValue,
                    out professorUserId))
            {
                MostrarMensagem(
                    "Selecione um professor.",
                    true
                );

                return;
            }

            try
            {
                if (!ProfessorPermitido(
                        educandoId,
                        encarregadoId,
                        professorUserId))
                {
                    MostrarMensagem(
                        "O professor selecionado não está associado " +
                        "à turma atual do educando.",
                        true
                    );

                    return;
                }

                int conversaId =
                    ObterOuCriarConversa(
                        UserIdAtual,
                        professorUserId
                    );

                HdnConversaId.Value =
                    conversaId.ToString();

                HdnProfessorUserId.Value =
                    professorUserId.ToString();

                string nomeEducando =
                    ObterNomeEducandoAssociado(
                        educandoId,
                        encarregadoId
                    );

                MarcarMensagensComoLidas(
                    conversaId
                );

                CarregarConversas(
                    educandoId,
                    encarregadoId,
                    nomeEducando
                );

                CarregarConversaAtual(
                    conversaId,
                    educandoId,
                    encarregadoId,
                    nomeEducando
                );
            }
            catch (Exception ex)
            {
                MostrarMensagem(
                    "Não foi possível iniciar a conversa: " +
                    ex.Message,
                    true
                );
            }
        }


        private int ObterOuCriarConversa(
            Guid utilizadorAtual,
            Guid outroUtilizador)
        {
            int? conversaExistente =
                ProcurarConversa(
                    utilizadorAtual,
                    outroUtilizador
                );

            if (conversaExistente.HasValue)
            {
                return conversaExistente.Value;
            }

            Guid utilizador1;
            Guid utilizador2;

            OrdenarUtilizadores(
                utilizadorAtual,
                outroUtilizador,
                out utilizador1,
                out utilizador2
            );

            const string sql = @"
                INSERT INTO dbo.ConversaDireta
                (
                    Utilizador1Id,
                    Utilizador2Id,
                    Ativa
                )
                VALUES
                (
                    @Utilizador1Id,
                    @Utilizador2Id,
                    1
                );

                SELECT CAST(
                    SCOPE_IDENTITY()
                    AS INT
                );";

            try
            {
                using (SqlConnection conn =
                    new SqlConnection(_connectionString))
                using (SqlCommand cmd =
                    new SqlCommand(sql, conn))
                {
                    cmd.Parameters
                        .Add(
                            "@Utilizador1Id",
                            SqlDbType.UniqueIdentifier
                        )
                        .Value = utilizador1;

                    cmd.Parameters
                        .Add(
                            "@Utilizador2Id",
                            SqlDbType.UniqueIdentifier
                        )
                        .Value = utilizador2;

                    conn.Open();

                    return Convert.ToInt32(
                        cmd.ExecuteScalar()
                    );
                }
            }
            catch (SqlException ex)
                when (ex.Number == 2601 ||
                      ex.Number == 2627)
            {
                conversaExistente =
                    ProcurarConversa(
                        utilizadorAtual,
                        outroUtilizador
                    );

                if (conversaExistente.HasValue)
                {
                    return conversaExistente.Value;
                }

                throw;
            }
        }


        private int? ProcurarConversa(
            Guid utilizadorAtual,
            Guid outroUtilizador)
        {
            const string sql = @"
                SELECT TOP 1
                    Id

                FROM dbo.ConversaDireta

                WHERE
                    (
                        Utilizador1Id =
                            @UtilizadorAtual

                        AND Utilizador2Id =
                            @OutroUtilizador
                    )

                    OR

                    (
                        Utilizador1Id =
                            @OutroUtilizador

                        AND Utilizador2Id =
                            @UtilizadorAtual
                    )

                ORDER BY Id;";

            using (SqlConnection conn =
                new SqlConnection(_connectionString))
            using (SqlCommand cmd =
                new SqlCommand(sql, conn))
            {
                cmd.Parameters
                    .Add(
                        "@UtilizadorAtual",
                        SqlDbType.UniqueIdentifier
                    )
                    .Value = utilizadorAtual;

                cmd.Parameters
                    .Add(
                        "@OutroUtilizador",
                        SqlDbType.UniqueIdentifier
                    )
                    .Value = outroUtilizador;

                conn.Open();

                object resultado =
                    cmd.ExecuteScalar();

                if (resultado == null ||
                    resultado == DBNull.Value)
                {
                    return null;
                }

                return Convert.ToInt32(
                    resultado
                );
            }
        }


        private void OrdenarUtilizadores(
            Guid primeiro,
            Guid segundo,
            out Guid utilizador1,
            out Guid utilizador2)
        {
            int comparacao =
                string.Compare(
                    primeiro.ToString("N"),
                    segundo.ToString("N"),
                    StringComparison.Ordinal
                );

            if (comparacao <= 0)
            {
                utilizador1 = primeiro;
                utilizador2 = segundo;
            }
            else
            {
                utilizador1 = segundo;
                utilizador2 = primeiro;
            }
        }

        #endregion


        #region Lista de conversas

        private void CarregarConversas(
            int educandoId,
            int encarregadoId,
            string nomeEducando)
        {
            const string sql = @"
                WITH ProfessoresPermitidos AS
                (
                    SELECT
                        dados.UserId,
                        dados.Nome,

                        STRING_AGG(
                            dados.Disciplina,
                            N', '
                        ) AS Disciplinas

                    FROM
                    (
                        SELECT DISTINCT
                            p.UserId,
                            p.Nome,
                            d.Nome AS Disciplina

                        FROM dbo.Aluno a

                        INNER JOIN dbo.AlunoEncarregado ae
                            ON ae.AlunoId = a.Id

                        INNER JOIN dbo.AlunoTurma at2
                            ON at2.AlunoId = a.Id

                        INNER JOIN dbo.TurmaDisciplina td
                            ON td.TurmaId = at2.TurmaId

                        INNER JOIN dbo.TurmaDisciplinaProfessor tdp
                            ON tdp.TurmaDisciplinaId = td.Id

                        INNER JOIN dbo.Professor p
                            ON p.Id = tdp.ProfessorId

                        INNER JOIN dbo.Disciplina d
                            ON d.Id = td.DisciplinaId

                        WHERE a.Id = @AlunoId

                          AND ae.EncarregadoEducacaoId =
                              @EncarregadoEducacaoId

                          AND a.Ativo = 1
                          AND ae.Ativo = 1
                          AND at2.Ate IS NULL
                          AND tdp.Ate IS NULL
                          AND p.Ativo = 1
                          AND p.UserId IS NOT NULL

                    ) AS dados

                    GROUP BY
                        dados.UserId,
                        dados.Nome
                )

                SELECT
                    c.Id,
                    professores.Nome,

                    N'Sobre ' +
                    @NomeEducando +

                    CASE
                        WHEN professores.Disciplinas IS NULL
                             OR professores.Disciplinas = N''
                            THEN N''

                        ELSE
                            N' — ' +
                            professores.Disciplinas
                    END AS Contexto,

                    COALESCE(
                        ultima.Texto,
                        N'Sem mensagens'
                    ) AS UltimaMensagem,

                    ultima.CriadoEm AS DataUltima,

                    (
                        SELECT COUNT(1)

                        FROM dbo.MensagemDireta naoLida

                        WHERE naoLida.ConversaId = c.Id

                          AND naoLida.RemetenteUserId <>
                              @UserIdAtual

                          AND naoLida.LidaEm IS NULL
                    ) AS NaoLidas

                FROM dbo.ConversaDireta c

                INNER JOIN ProfessoresPermitidos professores
                    ON professores.UserId =
                        CASE
                            WHEN c.Utilizador1Id =
                                 @UserIdAtual
                                THEN c.Utilizador2Id
                            ELSE c.Utilizador1Id
                        END

                OUTER APPLY
                (
                    SELECT TOP 1
                        m.Texto,
                        m.CriadoEm

                    FROM dbo.MensagemDireta m

                    WHERE m.ConversaId = c.Id

                    ORDER BY
                        m.CriadoEm DESC,
                        m.Id DESC
                ) AS ultima

                WHERE c.Ativa = 1

                  AND
                  (
                      c.Utilizador1Id =
                          @UserIdAtual

                      OR c.Utilizador2Id =
                          @UserIdAtual
                  )

                ORDER BY
                    CASE
                        WHEN ultima.CriadoEm IS NULL
                            THEN 1
                        ELSE 0
                    END,

                    ultima.CriadoEm DESC,

                    professores.Nome;";

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
                    .Value = educandoId;

                cmd.Parameters
                    .Add(
                        "@EncarregadoEducacaoId",
                        SqlDbType.Int
                    )
                    .Value = encarregadoId;

                cmd.Parameters
                    .Add(
                        "@UserIdAtual",
                        SqlDbType.UniqueIdentifier
                    )
                    .Value = UserIdAtual;

                cmd.Parameters
                    .Add(
                        "@NomeEducando",
                        SqlDbType.NVarChar,
                        200
                    )
                    .Value = nomeEducando;

                adapter.Fill(tabela);
            }

            if (tabela.Columns["Ativa"] == null)
            {
                tabela.Columns.Add(
                    "Ativa",
                    typeof(bool)
                );
            }

            foreach (DataRow row
                in tabela.Rows)
            {
                row["Ativa"] =
                    row["Id"].ToString() ==
                    HdnConversaId.Value;
            }

            RepeaterConversas.DataSource =
                tabela;

            RepeaterConversas.DataBind();

            PnlSemConversas.Visible =
                tabela.Rows.Count == 0;
        }

        #endregion


        #region Abrir conversa

        protected void RepeaterConversas_ItemCommand(
            object source,
            RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "AbrirConversa")
            {
                return;
            }

            int conversaId;

            if (!int.TryParse(
                    e.CommandArgument.ToString(),
                    out conversaId))
            {
                return;
            }

            int encarregadoId;
            int educandoId;

            if (!TryGetEncarregadoId(
                    out encarregadoId) ||
                !TryGetEducandoId(
                    out educandoId))
            {
                MostrarMensagem(
                    "Não foi possível identificar o educando.",
                    true
                );

                return;
            }

            try
            {
                string nomeEducando =
                    ObterNomeEducandoAssociado(
                        educandoId,
                        encarregadoId
                    );

                Guid professorUserId;
                string nomeProfessor;
                string disciplinas;

                if (!ObterProfessorAutorizadoDaConversa(
                        conversaId,
                        educandoId,
                        encarregadoId,
                        out professorUserId,
                        out nomeProfessor,
                        out disciplinas))
                {
                    MostrarMensagem(
                        "Não tem acesso a esta conversa.",
                        true
                    );

                    LimparConversaAtual();

                    return;
                }

                HdnConversaId.Value =
                    conversaId.ToString();

                HdnProfessorUserId.Value =
                    professorUserId.ToString();

                MarcarMensagensComoLidas(
                    conversaId
                );

                CarregarConversas(
                    educandoId,
                    encarregadoId,
                    nomeEducando
                );

                CarregarConversaAtual(
                    conversaId,
                    educandoId,
                    encarregadoId,
                    nomeEducando
                );
            }
            catch (Exception ex)
            {
                MostrarMensagem(
                    "Não foi possível abrir a conversa: " +
                    ex.Message,
                    true
                );
            }
        }


        private bool ObterProfessorAutorizadoDaConversa(
            int conversaId,
            int educandoId,
            int encarregadoId,
            out Guid professorUserId,
            out string nomeProfessor,
            out string disciplinas)
        {
            professorUserId =
                Guid.Empty;

            nomeProfessor =
                string.Empty;

            disciplinas =
                string.Empty;

            const string sql = @"
                SELECT
                    dados.UserId,
                    dados.Nome,

                    STRING_AGG(
                        dados.Disciplina,
                        N', '
                    ) AS Disciplinas

                FROM
                (
                    SELECT DISTINCT
                        p.UserId,
                        p.Nome,
                        d.Nome AS Disciplina

                    FROM dbo.ConversaDireta c

                    INNER JOIN dbo.Professor p
                        ON p.UserId =
                            CASE
                                WHEN c.Utilizador1Id =
                                     @UserIdAtual
                                    THEN c.Utilizador2Id
                                ELSE c.Utilizador1Id
                            END

                    INNER JOIN dbo.TurmaDisciplinaProfessor tdp
                        ON tdp.ProfessorId = p.Id

                    INNER JOIN dbo.TurmaDisciplina td
                        ON td.Id =
                           tdp.TurmaDisciplinaId

                    INNER JOIN dbo.Disciplina d
                        ON d.Id = td.DisciplinaId

                    INNER JOIN dbo.AlunoTurma at2
                        ON at2.TurmaId = td.TurmaId

                    INNER JOIN dbo.AlunoEncarregado ae
                        ON ae.AlunoId = at2.AlunoId

                    INNER JOIN dbo.Aluno a
                        ON a.Id = at2.AlunoId

                    WHERE c.Id = @ConversaId

                      AND c.Ativa = 1

                      AND
                      (
                          c.Utilizador1Id =
                              @UserIdAtual

                          OR c.Utilizador2Id =
                              @UserIdAtual
                      )

                      AND a.Id = @AlunoId

                      AND ae.EncarregadoEducacaoId =
                          @EncarregadoEducacaoId

                      AND a.Ativo = 1
                      AND ae.Ativo = 1
                      AND at2.Ate IS NULL
                      AND tdp.Ate IS NULL
                      AND p.Ativo = 1

                ) AS dados

                GROUP BY
                    dados.UserId,
                    dados.Nome;";

            using (SqlConnection conn =
                new SqlConnection(_connectionString))
            using (SqlCommand cmd =
                new SqlCommand(sql, conn))
            {
                cmd.Parameters
                    .Add(
                        "@ConversaId",
                        SqlDbType.Int
                    )
                    .Value = conversaId;

                cmd.Parameters
                    .Add(
                        "@UserIdAtual",
                        SqlDbType.UniqueIdentifier
                    )
                    .Value = UserIdAtual;

                cmd.Parameters
                    .Add(
                        "@AlunoId",
                        SqlDbType.Int
                    )
                    .Value = educandoId;

                cmd.Parameters
                    .Add(
                        "@EncarregadoEducacaoId",
                        SqlDbType.Int
                    )
                    .Value = encarregadoId;

                conn.Open();

                using (SqlDataReader reader =
                    cmd.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        return false;
                    }

                    professorUserId =
                        (Guid)reader["UserId"];

                    nomeProfessor =
                        ValorTexto(
                            reader["Nome"]
                        );

                    disciplinas =
                        ValorTexto(
                            reader["Disciplinas"]
                        );

                    return true;
                }
            }
        }

        #endregion


        #region Carregar mensagens

        private void CarregarConversaAtual(
            int conversaId,
            int educandoId,
            int encarregadoId,
            string nomeEducando)
        {
            Guid professorUserId;
            string nomeProfessor;
            string disciplinas;

            if (!ObterProfessorAutorizadoDaConversa(
                    conversaId,
                    educandoId,
                    encarregadoId,
                    out professorUserId,
                    out nomeProfessor,
                    out disciplinas))
            {
                LimparConversaAtual();
                return;
            }

            const string sql = @"
                SELECT
                    mensagens.Id,
                    mensagens.Minha,
                    mensagens.Autor,
                    mensagens.Texto,
                    mensagens.CriadoEm

                FROM
                (
                    SELECT TOP 100
                        m.Id,

                        CASE
                            WHEN m.RemetenteUserId =
                                 @UserIdAtual
                                THEN CAST(1 AS BIT)
                            ELSE CAST(0 AS BIT)
                        END AS Minha,

                        CASE
                            WHEN m.RemetenteUserId =
                                 @UserIdAtual
                                THEN N'Você'
                            ELSE @NomeProfessor
                        END AS Autor,

                        m.Texto,
                        m.CriadoEm

                    FROM dbo.MensagemDireta m

                    WHERE m.ConversaId =
                          @ConversaId

                    ORDER BY
                        m.CriadoEm DESC,
                        m.Id DESC

                ) AS mensagens

                ORDER BY
                    mensagens.CriadoEm,
                    mensagens.Id;";

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
                        "@UserIdAtual",
                        SqlDbType.UniqueIdentifier
                    )
                    .Value = UserIdAtual;

                cmd.Parameters
                    .Add(
                        "@ConversaId",
                        SqlDbType.Int
                    )
                    .Value = conversaId;

                cmd.Parameters
                    .Add(
                        "@NomeProfessor",
                        SqlDbType.NVarChar,
                        200
                    )
                    .Value = nomeProfessor;

                adapter.Fill(tabela);
            }

            RepeaterMensagens.DataSource =
                tabela;

            RepeaterMensagens.DataBind();

            PnlSemMensagens.Visible =
                tabela.Rows.Count == 0;

            LblConversaAtual.Text =
                nomeProfessor;

            LblContextoConversa.Text =
                "Sobre " +
                nomeEducando +

                (
                    string.IsNullOrWhiteSpace(
                        disciplinas)
                        ? string.Empty
                        : " — " + disciplinas
                );

            LblTipoConversa.Text =
                "Professor";

            LblTipoConversa.Visible =
                true;

            TxtMensagem.Enabled =
                true;

            BtnEnviarMensagem.Enabled =
                true;

            HdnProfessorUserId.Value =
                professorUserId.ToString();
        }

        #endregion


        #region Enviar mensagem

        protected void BtnEnviarMensagem_Click(
            object sender,
            EventArgs e)
        {
            LimparMensagem();

            string texto =
                TxtMensagem.Text.Trim();

            if (string.IsNullOrWhiteSpace(
                    texto))
            {
                MostrarMensagem(
                    "Escreva uma mensagem antes de enviar.",
                    true
                );

                return;
            }

            if (texto.Length > 2000)
            {
                MostrarMensagem(
                    "A mensagem não pode ter mais de 2000 caracteres.",
                    true
                );

                return;
            }

            int conversaId;

            if (!int.TryParse(
                    HdnConversaId.Value,
                    out conversaId))
            {
                MostrarMensagem(
                    "Selecione uma conversa primeiro.",
                    true
                );

                return;
            }

            int encarregadoId;
            int educandoId;

            if (!TryGetEncarregadoId(
                    out encarregadoId) ||
                !TryGetEducandoId(
                    out educandoId))
            {
                MostrarMensagem(
                    "Não foi possível identificar o educando.",
                    true
                );

                return;
            }

            try
            {
                Guid professorUserId;
                string nomeProfessor;
                string disciplinas;

                if (!ObterProfessorAutorizadoDaConversa(
                        conversaId,
                        educandoId,
                        encarregadoId,
                        out professorUserId,
                        out nomeProfessor,
                        out disciplinas))
                {
                    MostrarMensagem(
                        "Não tem autorização para enviar " +
                        "mensagens nesta conversa.",
                        true
                    );

                    return;
                }

                const string sql = @"
                    INSERT INTO dbo.MensagemDireta
                    (
                        ConversaId,
                        RemetenteUserId,
                        Texto
                    )
                    VALUES
                    (
                        @ConversaId,
                        @RemetenteUserId,
                        @Texto
                    );";

                using (SqlConnection conn =
                    new SqlConnection(_connectionString))
                using (SqlCommand cmd =
                    new SqlCommand(sql, conn))
                {
                    cmd.Parameters
                        .Add(
                            "@ConversaId",
                            SqlDbType.Int
                        )
                        .Value = conversaId;

                    cmd.Parameters
                        .Add(
                            "@RemetenteUserId",
                            SqlDbType.UniqueIdentifier
                        )
                        .Value = UserIdAtual;

                    cmd.Parameters
                        .Add(
                            "@Texto",
                            SqlDbType.NVarChar,
                            2000
                        )
                        .Value = texto;

                    conn.Open();

                    cmd.ExecuteNonQuery();
                }

                TxtMensagem.Text =
                    string.Empty;

                string nomeEducando =
                    ObterNomeEducandoAssociado(
                        educandoId,
                        encarregadoId
                    );

                CarregarConversas(
                    educandoId,
                    encarregadoId,
                    nomeEducando
                );

                CarregarConversaAtual(
                    conversaId,
                    educandoId,
                    encarregadoId,
                    nomeEducando
                );
            }
            catch (Exception ex)
            {
                MostrarMensagem(
                    "Não foi possível enviar a mensagem: " +
                    ex.Message,
                    true
                );
            }
        }

        #endregion


        #region Mensagens lidas

        private void MarcarMensagensComoLidas(
            int conversaId)
        {
            const string sql = @"
                UPDATE dbo.MensagemDireta

                SET LidaEm = SYSDATETIME()

                WHERE ConversaId = @ConversaId

                  AND RemetenteUserId <>
                      @UserIdAtual

                  AND LidaEm IS NULL;";

            using (SqlConnection conn =
                new SqlConnection(_connectionString))
            using (SqlCommand cmd =
                new SqlCommand(sql, conn))
            {
                cmd.Parameters
                    .Add(
                        "@ConversaId",
                        SqlDbType.Int
                    )
                    .Value = conversaId;

                cmd.Parameters
                    .Add(
                        "@UserIdAtual",
                        SqlDbType.UniqueIdentifier
                    )
                    .Value = UserIdAtual;

                conn.Open();

                cmd.ExecuteNonQuery();
            }
        }

        #endregion


        #region Interface

        private void LimparConversaAtual()
        {
            HdnConversaId.Value =
                string.Empty;

            HdnProfessorUserId.Value =
                string.Empty;

            LblConversaAtual.Text =
                "Selecione uma conversa";

            LblContextoConversa.Text =
                "Nenhuma conversa aberta.";

            LblTipoConversa.Visible =
                false;

            RepeaterMensagens.DataSource =
                null;

            RepeaterMensagens.DataBind();

            PnlSemMensagens.Visible =
                true;

            TxtMensagem.Text =
                string.Empty;

            TxtMensagem.Enabled =
                false;

            BtnEnviarMensagem.Enabled =
                false;
        }


        private void MostrarSemEducando()
        {
            PnlConteudo.Visible =
                false;

            PnlSemEducando.Visible =
                true;

            LblEducandoSelecionado.Visible =
                false;
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


        private void LimparMensagem()
        {
            LblMensagem.Text =
                string.Empty;

            LblMensagem.CssClass =
                string.Empty;

            LblMensagem.Visible =
                false;
        }


        private string ValorTexto(
            object valor)
        {
            if (valor == null ||
                valor == DBNull.Value)
            {
                return string.Empty;
            }

            return valor
                .ToString()
                .Trim();
        }

        #endregion


        #region Terminar sessão

        private void TerminarSessao()
        {
            FormsAuthentication.SignOut();

            Session.Clear();
            Session.Abandon();

            Response.Redirect(
                "~/login.aspx",
                false
            );

            Context.ApplicationInstance
                .CompleteRequest();
        }

        #endregion
    }
}