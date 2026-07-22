using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AlunoGest.professor
{
    public partial class Mensagens : Page
    {
        private readonly string _connectionString =
            ConfigurationManager
                .ConnectionStrings["DefaultConnection"]
                .ConnectionString;

        
        private Guid? _userIdAtual;
        private int? _professorIdAtual;
        private string _nomeProfessorAtual;


        #region Página

        protected void Page_Load(
            object sender,
            EventArgs e)
        {
            if (!Request.IsAuthenticated ||
                !Roles.IsUserInRole(
                    User.Identity.Name,
                    "professor"))
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
                        "Erro ao carregar mensagens do professor: " +
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


        private void CarregarPagina()
        {
            LimparMensagem();

            GarantirProfessorAtual();

            LblProfessor.Text =
                _nomeProfessorAtual;

            LblProfessor.Visible =
                true;

            bool possuiTurmas =
                CarregarTurmas();

            if (!possuiTurmas)
            {
                MostrarSemTurmas();
                return;
            }

            PnlSemTurmas.Visible =
                false;

            PnlConteudo.Visible =
                true;

            if (DdlTipoDestinatario.Items.Count > 0)
            {
                DdlTipoDestinatario.SelectedValue =
                    "aluno";
            }

            CarregarDestinatarios();
            CarregarConversas();
            LimparConversaAtual();
        }

        #endregion

        #region Professor autenticado

        private Guid UserIdAtual
        {
            get
            {
                if (_userIdAtual.HasValue)
                {
                    return _userIdAtual.Value;
                }

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
                        "O identificador do utilizador é inválido."
                    );
                }

                _userIdAtual =
                    userId;

                Session["UserId"] =
                    userId;

                return userId;
            }
        }


        private int ProfessorIdAtual
        {
            get
            {
                GarantirProfessorAtual();

                return _professorIdAtual.Value;
            }
        }


        private void GarantirProfessorAtual()
        {
            if (_professorIdAtual.HasValue)
            {
                return;
            }

            const string sql = @"
                SELECT TOP 1
                    Id,
                    Nome

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

                using (SqlDataReader reader =
                    cmd.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        throw new InvalidOperationException(
                            "Não foi encontrado um professor ativo " +
                            "associado a esta conta."
                        );
                    }

                    _professorIdAtual =
                        Convert.ToInt32(
                            reader["Id"]
                        );

                    _nomeProfessorAtual =
                        reader["Nome"]
                            .ToString()
                            .Trim();

                    Session["ProfessorId"] =
                        _professorIdAtual.Value;
                }
            }
        }

        #endregion

        #region Turmas

        private bool CarregarTurmas()
        {
            const string sql = @"
                SELECT
                    dados.TurmaId,
                    dados.Turma,

                    STRING_AGG(
                        dados.Disciplina,
                        N', '
                    ) AS Disciplinas

                FROM
                (
                    SELECT DISTINCT
                        t.Id AS TurmaId,

                        CAST(
                            t.AnoEscolaridade
                            AS NVARCHAR(3)
                        )
                        + N'.º '
                        + t.CodigoTurma
                        AS Turma,

                        d.Nome AS Disciplina

                    FROM dbo.TurmaDisciplinaProfessor tdp

                    INNER JOIN dbo.TurmaDisciplina td
                        ON td.Id =
                           tdp.TurmaDisciplinaId

                    INNER JOIN dbo.Turma t
                        ON t.Id = td.TurmaId

                    INNER JOIN dbo.Disciplina d
                        ON d.Id = td.DisciplinaId

                    WHERE tdp.ProfessorId =
                          @ProfessorId

                      AND tdp.Ate IS NULL
                      AND t.Ativa = 1
                      AND d.Ativa = 1

                ) AS dados

                GROUP BY
                    dados.TurmaId,
                    dados.Turma

                ORDER BY
                    dados.Turma;";

            DataTable tabela =
                ObterTabela(
                    sql,
                    Param(
                        "@ProfessorId",
                        ProfessorIdAtual
                    )
                );

            DdlTurmas.Items.Clear();

            DdlTurmas.Items.Add(
                new ListItem(
                    "Selecione uma turma",
                    string.Empty
                )
            );

            foreach (DataRow row
                in tabela.Rows)
            {
                string turma =
                    ValorTexto(
                        row["Turma"]
                    );

                string disciplinas =
                    ValorTexto(
                        row["Disciplinas"]
                    );

                string texto =
                    string.IsNullOrWhiteSpace(
                        disciplinas)
                        ? turma
                        : turma +
                          " — " +
                          disciplinas;

                DdlTurmas.Items.Add(
                    new ListItem(
                        texto,
                        row["TurmaId"].ToString()
                    )
                );
            }

            if (DdlTurmas.Items.Count > 1)
            {
                DdlTurmas.SelectedIndex =
                    1;
            }

            return tabela.Rows.Count > 0;
        }


        private bool ProfessorTemTurma(
            int turmaId)
        {
            const string sql = @"
                SELECT
                    CASE
                        WHEN EXISTS
                        (
                            SELECT 1

                            FROM dbo.TurmaDisciplinaProfessor tdp

                            INNER JOIN dbo.TurmaDisciplina td
                                ON td.Id =
                                   tdp.TurmaDisciplinaId

                            INNER JOIN dbo.Turma t
                                ON t.Id = td.TurmaId

                            WHERE tdp.ProfessorId =
                                  @ProfessorId

                              AND td.TurmaId =
                                  @TurmaId

                              AND tdp.Ate IS NULL
                              AND t.Ativa = 1
                        )
                        THEN 1
                        ELSE 0
                    END;";

            return Convert.ToInt32(
                Escalar(
                    sql,
                    Param(
                        "@ProfessorId",
                        ProfessorIdAtual
                    ),
                    Param(
                        "@TurmaId",
                        turmaId
                    )
                )
            ) == 1;
        }

        #endregion

        #region Alteração dos seletores

        protected void DdlTurmas_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            AtualizarSelecaoDestinatario();
        }


        protected void DdlTipoDestinatario_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            AtualizarSelecaoDestinatario();
        }


        private void AtualizarSelecaoDestinatario()
        {
            LimparMensagem();

            try
            {
                CarregarDestinatarios();
                CarregarConversas();

                int conversaId;

                if (int.TryParse(
                        HdnConversaId.Value,
                        out conversaId))
                {
                    ContactoDados contacto;

                    if (ObterContactoAutorizadoDaConversa(
                            conversaId,
                            out contacto))
                    {
                        CarregarConversaAtual(
                            conversaId,
                            contacto
                        );

                        return;
                    }
                }

                LimparConversaAtual();
            }
            catch (Exception ex)
            {
                MostrarMensagem(
                    "Não foi possível atualizar os destinatários: " +
                    ex.Message,
                    true
                );
            }
        }

        #endregion

        #region Destinatários

        private void CarregarDestinatarios()
        {
            DdlDestinatarios.Items.Clear();

            DdlDestinatarios.Items.Add(
                new ListItem(
                    "Selecione um destinatário",
                    string.Empty
                )
            );

            int turmaId;

            if (!int.TryParse(
                    DdlTurmas.SelectedValue,
                    out turmaId))
            {
                DdlDestinatarios.Enabled =
                    false;

                BtnIniciarConversa.Enabled =
                    false;

                return;
            }

            if (!ProfessorTemTurma(
                    turmaId))
            {
                DdlDestinatarios.Enabled =
                    false;

                BtnIniciarConversa.Enabled =
                    false;

                return;
            }

            string tipo =
                DdlTipoDestinatario.SelectedValue;

            if (tipo == "encarregado")
            {
                CarregarEncarregadosDestinatarios(
                    turmaId
                );
            }
            else
            {
                CarregarAlunosDestinatarios(
                    turmaId
                );
            }

            bool possuiDestinatarios =
                DdlDestinatarios.Items.Count > 1;

            DdlDestinatarios.Enabled =
                possuiDestinatarios;

            BtnIniciarConversa.Enabled =
                possuiDestinatarios;
        }


        private void CarregarAlunosDestinatarios(
            int turmaId)
        {
            const string sql = @"
                SELECT DISTINCT
                    a.UserId,
                    a.Id AS AlunoId,
                    a.NomeCompleto,
                    a.NumeroProcesso

                FROM dbo.AlunoTurma at2

                INNER JOIN dbo.Aluno a
                    ON a.Id = at2.AlunoId

                WHERE at2.TurmaId = @TurmaId
                  AND at2.Ate IS NULL
                  AND a.Ativo = 1
                  AND a.UserId IS NOT NULL

                ORDER BY
                    a.NomeCompleto,
                    a.NumeroProcesso;";

            DataTable tabela =
                ObterTabela(
                    sql,
                    Param(
                        "@TurmaId",
                        turmaId
                    )
                );

            foreach (DataRow row
                in tabela.Rows)
            {
                string nome =
                    ValorTexto(
                        row["NomeCompleto"]
                    );

                string processo =
                    ValorTexto(
                        row["NumeroProcesso"]
                    );

                string texto =
                    string.IsNullOrWhiteSpace(
                        processo)
                        ? nome
                        : nome +
                          " — Processo: " +
                          processo;

                string valor =
                    row["UserId"].ToString() +
                    "|" +
                    row["AlunoId"].ToString();

                DdlDestinatarios.Items.Add(
                    new ListItem(
                        texto,
                        valor
                    )
                );
            }
        }


        private void CarregarEncarregadosDestinatarios(
            int turmaId)
        {
            const string sql = @"
                WITH dados AS
                (
                    SELECT DISTINCT
                        ee.UserId,
                        ee.NomeCompleto,
                        a.Id AS AlunoId,
                        a.NomeCompleto AS NomeAluno

                    FROM dbo.AlunoTurma at2

                    INNER JOIN dbo.Aluno a
                        ON a.Id = at2.AlunoId

                    INNER JOIN dbo.AlunoEncarregado ae
                        ON ae.AlunoId = a.Id

                    INNER JOIN dbo.EncarregadoEducacao ee
                        ON ee.Id =
                           ae.EncarregadoEducacaoId

                    WHERE at2.TurmaId = @TurmaId

                      AND at2.Ate IS NULL
                      AND a.Ativo = 1
                      AND ae.Ativo = 1
                      AND ee.Ativo = 1
                      AND ee.UserId IS NOT NULL
                )

                SELECT
                    UserId,
                    NomeCompleto,
                    MIN(AlunoId) AS AlunoId,

                    STRING_AGG(
                        NomeAluno,
                        N', '
                    ) AS Educandos

                FROM dados

                GROUP BY
                    UserId,
                    NomeCompleto

                ORDER BY
                    NomeCompleto;";

            DataTable tabela =
                ObterTabela(
                    sql,
                    Param(
                        "@TurmaId",
                        turmaId
                    )
                );

            foreach (DataRow row
                in tabela.Rows)
            {
                string nome =
                    ValorTexto(
                        row["NomeCompleto"]
                    );

                string educandos =
                    ValorTexto(
                        row["Educandos"]
                    );

                string texto =
                    string.IsNullOrWhiteSpace(
                        educandos)
                        ? nome
                        : nome +
                          " — EE de: " +
                          educandos;

                string valor =
                    row["UserId"].ToString() +
                    "|" +
                    row["AlunoId"].ToString();

                DdlDestinatarios.Items.Add(
                    new ListItem(
                        texto,
                        valor
                    )
                );
            }
        }


        private bool TentarLerDestinatarioSelecionado(
            out Guid destinatarioUserId,
            out int alunoContextoId)
        {
            destinatarioUserId =
                Guid.Empty;

            alunoContextoId =
                0;

            string valor =
                DdlDestinatarios.SelectedValue;

            if (string.IsNullOrWhiteSpace(
                    valor))
            {
                return false;
            }

            string[] partes =
                valor.Split('|');

            if (partes.Length != 2)
            {
                return false;
            }

            return
                Guid.TryParse(
                    partes[0],
                    out destinatarioUserId
                )
                &&
                int.TryParse(
                    partes[1],
                    out alunoContextoId
                );
        }

        #endregion

        #region Iniciar conversa

        protected void BtnIniciarConversa_Click(
            object sender,
            EventArgs e)
        {
            LimparMensagem();

            int turmaId;

            if (!int.TryParse(
                    DdlTurmas.SelectedValue,
                    out turmaId))
            {
                MostrarMensagem(
                    "Selecione uma turma.",
                    true
                );

                return;
            }

            Guid destinatarioUserId;
            int alunoContextoId;

            if (!TentarLerDestinatarioSelecionado(
                    out destinatarioUserId,
                    out alunoContextoId))
            {
                MostrarMensagem(
                    "Selecione um destinatário.",
                    true
                );

                return;
            }

            try
            {
                ContactoDados contacto;

                if (!ObterContactoPermitidoNaTurma(
                        turmaId,
                        alunoContextoId,
                        destinatarioUserId,
                        DdlTipoDestinatario.SelectedValue,
                        out contacto))
                {
                    MostrarMensagem(
                        "O destinatário selecionado já não está " +
                        "relacionado com esta turma.",
                        true
                    );

                    CarregarDestinatarios();
                    return;
                }

                int conversaId =
                    ObterOuCriarConversa(
                        UserIdAtual,
                        destinatarioUserId
                    );

                GuardarConversaSelecionada(
                    conversaId,
                    contacto
                );

                MarcarMensagensComoLidas(
                    conversaId
                );

                CarregarConversas();

                CarregarConversaAtual(
                    conversaId,
                    contacto
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


        private bool ObterContactoPermitidoNaTurma(
            int turmaId,
            int alunoContextoId,
            Guid destinatarioUserId,
            string tipo,
            out ContactoDados contacto)
        {
            contacto =
                null;

            if (!ProfessorTemTurma(
                    turmaId))
            {
                return false;
            }

            string sql;

            if (tipo == "encarregado")
            {
                sql = @"
                    SELECT TOP 1
                        ee.UserId,
                        ee.NomeCompleto AS Nome,
                        N'encarregado' AS Tipo,
                        a.Id AS AlunoId,
                        t.Id AS TurmaId,

                        N'Encarregado de '
                        + a.NomeCompleto
                        + N' — '
                        + CAST(
                            t.AnoEscolaridade
                            AS NVARCHAR(3)
                        )
                        + N'.º '
                        + t.CodigoTurma
                        AS Contexto

                    FROM dbo.AlunoTurma at2

                    INNER JOIN dbo.Aluno a
                        ON a.Id = at2.AlunoId

                    INNER JOIN dbo.AlunoEncarregado ae
                        ON ae.AlunoId = a.Id

                    INNER JOIN dbo.EncarregadoEducacao ee
                        ON ee.Id =
                           ae.EncarregadoEducacaoId

                    INNER JOIN dbo.Turma t
                        ON t.Id = at2.TurmaId

                    WHERE at2.TurmaId = @TurmaId
                      AND a.Id = @AlunoId
                      AND ee.UserId = @UserId

                      AND at2.Ate IS NULL
                      AND a.Ativo = 1
                      AND ae.Ativo = 1
                      AND ee.Ativo = 1
                      AND t.Ativa = 1;";
            }
            else
            {
                sql = @"
                    SELECT TOP 1
                        a.UserId,
                        a.NomeCompleto AS Nome,
                        N'aluno' AS Tipo,
                        a.Id AS AlunoId,
                        t.Id AS TurmaId,

                        N'Aluno da turma '
                        + CAST(
                            t.AnoEscolaridade
                            AS NVARCHAR(3)
                        )
                        + N'.º '
                        + t.CodigoTurma
                        AS Contexto

                    FROM dbo.AlunoTurma at2

                    INNER JOIN dbo.Aluno a
                        ON a.Id = at2.AlunoId

                    INNER JOIN dbo.Turma t
                        ON t.Id = at2.TurmaId

                    WHERE at2.TurmaId = @TurmaId
                      AND a.Id = @AlunoId
                      AND a.UserId = @UserId

                      AND at2.Ate IS NULL
                      AND a.Ativo = 1
                      AND t.Ativa = 1;";
            }

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

                cmd.Parameters
                    .Add(
                        "@AlunoId",
                        SqlDbType.Int
                    )
                    .Value = alunoContextoId;

                cmd.Parameters
                    .Add(
                        "@UserId",
                        SqlDbType.UniqueIdentifier
                    )
                    .Value = destinatarioUserId;

                conn.Open();

                using (SqlDataReader reader =
                    cmd.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        return false;
                    }

                    contacto =
                        LerContacto(
                            reader
                        );

                    return true;
                }
            }
        }

        #endregion

        #region Criar ou obter conversa

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
                ReativarConversa(
                    conversaExistente.Value
                );

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
                return Convert.ToInt32(
                    Escalar(
                        sql,
                        Param(
                            "@Utilizador1Id",
                            utilizador1
                        ),
                        Param(
                            "@Utilizador2Id",
                            utilizador2
                        )
                    )
                );
            }
            catch (SqlException ex)
                when (
                    ex.Number == 2601 ||
                    ex.Number == 2627
                )
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

            object resultado =
                Escalar(
                    sql,
                    Param(
                        "@UtilizadorAtual",
                        utilizadorAtual
                    ),
                    Param(
                        "@OutroUtilizador",
                        outroUtilizador
                    )
                );

            if (resultado == null ||
                resultado == DBNull.Value)
            {
                return null;
            }

            return Convert.ToInt32(
                resultado
            );
        }


        private void ReativarConversa(
            int conversaId)
        {
            const string sql = @"
                UPDATE dbo.ConversaDireta

                SET Ativa = 1

                WHERE Id = @Id

                  AND
                  (
                      Utilizador1Id =
                          @UserId

                      OR Utilizador2Id =
                          @UserId
                  );";

            Executar(
                sql,
                Param(
                    "@Id",
                    conversaId
                ),
                Param(
                    "@UserId",
                    UserIdAtual
                )
            );
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
                utilizador1 =
                    primeiro;

                utilizador2 =
                    segundo;
            }
            else
            {
                utilizador1 =
                    segundo;

                utilizador2 =
                    primeiro;
            }
        }

        #endregion

        #region Lista de conversas

        private void CarregarConversas()
        {
            const string sql = @"
                WITH TurmasProfessor AS
                (
                    SELECT DISTINCT
                        t.Id AS TurmaId,

                        CAST(
                            t.AnoEscolaridade
                            AS NVARCHAR(3)
                        )
                        + N'.º '
                        + t.CodigoTurma
                        AS Turma

                    FROM dbo.TurmaDisciplinaProfessor tdp

                    INNER JOIN dbo.TurmaDisciplina td
                        ON td.Id =
                           tdp.TurmaDisciplinaId

                    INNER JOIN dbo.Turma t
                        ON t.Id = td.TurmaId

                    WHERE tdp.ProfessorId =
                          @ProfessorId

                      AND tdp.Ate IS NULL
                      AND t.Ativa = 1
                ),

                ContactosBase AS
                (
                    SELECT
                        a.UserId,
                        a.NomeCompleto AS Nome,
                        N'aluno' AS Tipo,
                        a.Id AS AlunoId,
                        tp.TurmaId,

                        N'Aluno — '
                        + tp.Turma
                        AS Contexto

                    FROM TurmasProfessor tp

                    INNER JOIN dbo.AlunoTurma at2
                        ON at2.TurmaId =
                           tp.TurmaId

                    INNER JOIN dbo.Aluno a
                        ON a.Id = at2.AlunoId

                    WHERE at2.Ate IS NULL
                      AND a.Ativo = 1
                      AND a.UserId IS NOT NULL

                    UNION ALL

                    SELECT
                        ee.UserId,
                        ee.NomeCompleto AS Nome,
                        N'encarregado' AS Tipo,
                        a.Id AS AlunoId,
                        tp.TurmaId,

                        N'Encarregado de '
                        + a.NomeCompleto
                        + N' — '
                        + tp.Turma
                        AS Contexto

                    FROM TurmasProfessor tp

                    INNER JOIN dbo.AlunoTurma at2
                        ON at2.TurmaId =
                           tp.TurmaId

                    INNER JOIN dbo.Aluno a
                        ON a.Id = at2.AlunoId

                    INNER JOIN dbo.AlunoEncarregado ae
                        ON ae.AlunoId = a.Id

                    INNER JOIN dbo.EncarregadoEducacao ee
                        ON ee.Id =
                           ae.EncarregadoEducacaoId

                    WHERE at2.Ate IS NULL
                      AND a.Ativo = 1
                      AND ae.Ativo = 1
                      AND ee.Ativo = 1
                      AND ee.UserId IS NOT NULL
                ),

                ContactosDistintos AS
                (
                    SELECT DISTINCT
                        UserId,
                        Nome,
                        Tipo,
                        AlunoId,
                        TurmaId,
                        Contexto

                    FROM ContactosBase
                ),

                Contactos AS
                (
                    SELECT
                        UserId,
                        Nome,
                        Tipo,

                        STRING_AGG(
                            Contexto,
                            N'; '
                        ) AS Contexto

                    FROM ContactosDistintos

                    GROUP BY
                        UserId,
                        Nome,
                        Tipo
                )

                SELECT
                    c.Id,
                    contacto.Nome,

                    CASE
                        WHEN contacto.Tipo =
                             N'encarregado'
                            THEN N'Encarregado'
                        ELSE N'Aluno'
                    END AS TipoTexto,

                    contacto.Contexto,

                    COALESCE(
                        ultima.Texto,
                        N'Sem mensagens'
                    ) AS UltimaMensagem,

                    ultima.CriadoEm AS DataUltima,

                    (
                        SELECT COUNT(1)

                        FROM dbo.MensagemDireta mensagem

                        WHERE mensagem.ConversaId =
                              c.Id

                          AND mensagem.RemetenteUserId <>
                              @UserIdAtual

                          AND mensagem.LidaEm IS NULL
                    ) AS NaoLidas

                FROM dbo.ConversaDireta c

                INNER JOIN Contactos contacto
                    ON contacto.UserId =
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

                    WHERE m.ConversaId =
                          c.Id

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
                    contacto.Nome;";

            DataTable tabela =
                ObterTabela(
                    sql,
                    Param(
                        "@ProfessorId",
                        ProfessorIdAtual
                    ),
                    Param(
                        "@UserIdAtual",
                        UserIdAtual
                    )
                );

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

            LimparMensagem();

            try
            {
                ContactoDados contacto;

                if (!ObterContactoAutorizadoDaConversa(
                        conversaId,
                        out contacto))
                {
                    MostrarMensagem(
                        "Não tem acesso a esta conversa.",
                        true
                    );

                    LimparConversaAtual();
                    CarregarConversas();

                    return;
                }

                GuardarConversaSelecionada(
                    conversaId,
                    contacto
                );

                MarcarMensagensComoLidas(
                    conversaId
                );

                CarregarConversas();

                CarregarConversaAtual(
                    conversaId,
                    contacto
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


        private bool ObterContactoAutorizadoDaConversa(
            int conversaId,
            out ContactoDados contacto)
        {
            contacto =
                null;

            Guid outroUserId;

            if (!ObterOutroUtilizadorDaConversa(
                    conversaId,
                    out outroUserId))
            {
                return false;
            }

            return ObterContactoPermitidoGeral(
                outroUserId,
                out contacto
            );
        }


        private bool ObterOutroUtilizadorDaConversa(
            int conversaId,
            out Guid outroUserId)
        {
            outroUserId =
                Guid.Empty;

            const string sql = @"
                SELECT TOP 1

                    CASE
                        WHEN Utilizador1Id =
                             @UserIdAtual
                            THEN Utilizador2Id
                        ELSE Utilizador1Id
                    END AS OutroUserId

                FROM dbo.ConversaDireta

                WHERE Id = @ConversaId
                  AND Ativa = 1

                  AND
                  (
                      Utilizador1Id =
                          @UserIdAtual

                      OR Utilizador2Id =
                          @UserIdAtual
                  );";

            object resultado =
                Escalar(
                    sql,
                    Param(
                        "@ConversaId",
                        conversaId
                    ),
                    Param(
                        "@UserIdAtual",
                        UserIdAtual
                    )
                );

            if (resultado == null ||
                resultado == DBNull.Value)
            {
                return false;
            }

            outroUserId =
                (Guid)resultado;

            return true;
        }


        private bool ObterContactoPermitidoGeral(
            Guid outroUserId,
            out ContactoDados contacto)
        {
            contacto =
                null;

            const string sql = @"
                WITH TurmasProfessor AS
                (
                    SELECT DISTINCT
                        t.Id AS TurmaId,

                        CAST(
                            t.AnoEscolaridade
                            AS NVARCHAR(3)
                        )
                        + N'.º '
                        + t.CodigoTurma
                        AS Turma

                    FROM dbo.TurmaDisciplinaProfessor tdp

                    INNER JOIN dbo.TurmaDisciplina td
                        ON td.Id =
                           tdp.TurmaDisciplinaId

                    INNER JOIN dbo.Turma t
                        ON t.Id = td.TurmaId

                    WHERE tdp.ProfessorId =
                          @ProfessorId

                      AND tdp.Ate IS NULL
                      AND t.Ativa = 1
                ),

                ContactosBase AS
                (
                    SELECT
                        a.UserId,
                        a.NomeCompleto AS Nome,
                        N'aluno' AS Tipo,
                        a.Id AS AlunoId,
                        tp.TurmaId,

                        N'Aluno — '
                        + tp.Turma
                        AS Contexto

                    FROM TurmasProfessor tp

                    INNER JOIN dbo.AlunoTurma at2
                        ON at2.TurmaId =
                           tp.TurmaId

                    INNER JOIN dbo.Aluno a
                        ON a.Id = at2.AlunoId

                    WHERE at2.Ate IS NULL
                      AND a.Ativo = 1
                      AND a.UserId IS NOT NULL

                    UNION ALL

                    SELECT
                        ee.UserId,
                        ee.NomeCompleto AS Nome,
                        N'encarregado' AS Tipo,
                        a.Id AS AlunoId,
                        tp.TurmaId,

                        N'Encarregado de '
                        + a.NomeCompleto
                        + N' — '
                        + tp.Turma
                        AS Contexto

                    FROM TurmasProfessor tp

                    INNER JOIN dbo.AlunoTurma at2
                        ON at2.TurmaId =
                           tp.TurmaId

                    INNER JOIN dbo.Aluno a
                        ON a.Id = at2.AlunoId

                    INNER JOIN dbo.AlunoEncarregado ae
                        ON ae.AlunoId = a.Id

                    INNER JOIN dbo.EncarregadoEducacao ee
                        ON ee.Id =
                           ae.EncarregadoEducacaoId

                    WHERE at2.Ate IS NULL
                      AND a.Ativo = 1
                      AND ae.Ativo = 1
                      AND ee.Ativo = 1
                      AND ee.UserId IS NOT NULL
                ),

                ContactosDistintos AS
                (
                    SELECT DISTINCT
                        UserId,
                        Nome,
                        Tipo,
                        AlunoId,
                        TurmaId,
                        Contexto

                    FROM ContactosBase
                )

                SELECT
                    UserId,
                    Nome,
                    Tipo,
                    MIN(AlunoId) AS AlunoId,
                    MIN(TurmaId) AS TurmaId,

                    STRING_AGG(
                        Contexto,
                        N'; '
                    ) AS Contexto

                FROM ContactosDistintos

                WHERE UserId = @OutroUserId

                GROUP BY
                    UserId,
                    Nome,
                    Tipo;";

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
                    .Value = ProfessorIdAtual;

                cmd.Parameters
                    .Add(
                        "@OutroUserId",
                        SqlDbType.UniqueIdentifier
                    )
                    .Value = outroUserId;

                conn.Open();

                using (SqlDataReader reader =
                    cmd.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        return false;
                    }

                    contacto =
                        LerContacto(
                            reader
                        );

                    return true;
                }
            }
        }

        #endregion

        #region Carregar conversa atual

        private void CarregarConversaAtual(
            int conversaId,
            ContactoDados contacto)
        {
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
                            ELSE @NomeContacto
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
                ObterTabela(
                    sql,
                    Param(
                        "@UserIdAtual",
                        UserIdAtual
                    ),
                    Param(
                        "@ConversaId",
                        conversaId
                    ),
                    Param(
                        "@NomeContacto",
                        contacto.Nome
                    )
                );

            RepeaterMensagens.DataSource =
                tabela;

            RepeaterMensagens.DataBind();

            PnlSemMensagens.Visible =
                tabela.Rows.Count == 0;

            LblConversaAtual.Text =
                contacto.Nome;

            LblContextoConversa.Text =
                contacto.Contexto;

            LblTipoConversa.Text =
                contacto.Tipo == "encarregado"
                    ? "Encarregado de Educação"
                    : "Aluno";

            LblTipoConversa.Visible =
                true;

            TxtMensagem.Enabled =
                true;

            BtnEnviarMensagem.Enabled =
                true;

            GuardarConversaSelecionada(
                conversaId,
                contacto
            );
        }


        private void GuardarConversaSelecionada(
            int conversaId,
            ContactoDados contacto)
        {
            HdnConversaId.Value =
                conversaId.ToString();

            HdnDestinatarioUserId.Value =
                contacto.UserId.ToString();

            HdnTipoDestinatario.Value =
                contacto.Tipo;

            HdnTurmaContextoId.Value =
                contacto.TurmaId.ToString();

            HdnAlunoContextoId.Value =
                contacto.AlunoId.ToString();
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

            try
            {
                ContactoDados contacto;

                if (!ObterContactoAutorizadoDaConversa(
                        conversaId,
                        out contacto))
                {
                    MostrarMensagem(
                        "Já não tem autorização para enviar " +
                        "mensagens nesta conversa.",
                        true
                    );

                    LimparConversaAtual();
                    CarregarConversas();

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

                Executar(
                    sql,
                    Param(
                        "@ConversaId",
                        conversaId
                    ),
                    Param(
                        "@RemetenteUserId",
                        UserIdAtual
                    ),
                    Param(
                        "@Texto",
                        texto
                    )
                );

                TxtMensagem.Text =
                    string.Empty;

                CarregarConversas();

                CarregarConversaAtual(
                    conversaId,
                    contacto
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

            Executar(
                sql,
                Param(
                    "@ConversaId",
                    conversaId
                ),
                Param(
                    "@UserIdAtual",
                    UserIdAtual
                )
            );
        }

        #endregion

        #region Classes auxiliares

        private ContactoDados LerContacto(
            SqlDataReader reader)
        {
            ContactoDados contacto =
                new ContactoDados();

            contacto.UserId =
                (Guid)reader["UserId"];

            contacto.Nome =
                ValorTexto(
                    reader["Nome"]
                );

            contacto.Tipo =
                ValorTexto(
                    reader["Tipo"]
                );

            contacto.Contexto =
                ValorTexto(
                    reader["Contexto"]
                );

            contacto.AlunoId =
                Convert.ToInt32(
                    reader["AlunoId"]
                );

            contacto.TurmaId =
                Convert.ToInt32(
                    reader["TurmaId"]
                );

            return contacto;
        }


        private class ContactoDados
        {
            public Guid UserId { get; set; }

            public string Nome { get; set; }

            public string Tipo { get; set; }

            public string Contexto { get; set; }

            public int AlunoId { get; set; }

            public int TurmaId { get; set; }
        }

        #endregion

        #region Acesso à base de dados

        private DataTable ObterTabela(
            string sql,
            params SqlParameter[] parametros)
        {
            DataTable tabela =
                new DataTable();

            using (SqlConnection conn =
                new SqlConnection(_connectionString))
            using (SqlCommand cmd =
                new SqlCommand(sql, conn))
            using (SqlDataAdapter adapter =
                new SqlDataAdapter(cmd))
            {
                if (parametros != null &&
                    parametros.Length > 0)
                {
                    cmd.Parameters.AddRange(
                        parametros
                    );
                }

                adapter.Fill(
                    tabela
                );
            }

            return tabela;
        }


        private object Escalar(
            string sql,
            params SqlParameter[] parametros)
        {
            using (SqlConnection conn =
                new SqlConnection(_connectionString))
            using (SqlCommand cmd =
                new SqlCommand(sql, conn))
            {
                if (parametros != null &&
                    parametros.Length > 0)
                {
                    cmd.Parameters.AddRange(
                        parametros
                    );
                }

                conn.Open();

                return cmd.ExecuteScalar();
            }
        }


        private void Executar(
            string sql,
            params SqlParameter[] parametros)
        {
            using (SqlConnection conn =
                new SqlConnection(_connectionString))
            using (SqlCommand cmd =
                new SqlCommand(sql, conn))
            {
                if (parametros != null &&
                    parametros.Length > 0)
                {
                    cmd.Parameters.AddRange(
                        parametros
                    );
                }

                conn.Open();

                cmd.ExecuteNonQuery();
            }
        }


        private SqlParameter Param(
            string nome,
            object valor)
        {
            return new SqlParameter(
                nome,
                valor ?? DBNull.Value
            );
        }

        #endregion

        #region Interface

        private void MostrarSemTurmas()
        {
            PnlSemTurmas.Visible =
                true;

            PnlConteudo.Visible =
                false;
        }


        private void LimparConversaAtual()
        {
            HdnConversaId.Value =
                string.Empty;

            HdnDestinatarioUserId.Value =
                string.Empty;

            HdnTipoDestinatario.Value =
                string.Empty;

            HdnTurmaContextoId.Value =
                string.Empty;

            HdnAlunoContextoId.Value =
                string.Empty;

            LblConversaAtual.Text =
                "Selecione uma conversa";

            LblContextoConversa.Text =
                "Nenhuma conversa aberta.";

            LblTipoConversa.Text =
                string.Empty;

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