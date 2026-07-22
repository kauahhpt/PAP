using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Security;

namespace AlunoGest.agrupamento
{
    public partial class dashboard :
        System.Web.UI.Page
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
            if (!Page.User.Identity.IsAuthenticated)
            {
                TerminarSessao();
                return;
            }

            if (!Roles.IsUserInRole(
                    Page.User.Identity.Name,
                    "agrupamento"))
            {
                TerminarSessao();
                return;
            }

            if (!IsPostBack)
            {
                int agrupamentoId;

                if (!TryGetAgrupamentoId(
                        out agrupamentoId))
                {
                    TerminarSessao();
                    return;
                }

                CarregarDashboard(
                    agrupamentoId
                );
            }
        }

        #endregion


        #region Carregamento do dashboard

        private void CarregarDashboard(
            int agrupamentoId)
        {
            const string sql = @"
                SELECT

                    /* =========================================
                       INFORMAÇÃO PRINCIPAL
                    ========================================== */

                    agrupamento.Nome
                        AS NomeAgrupamento,

                    COALESCE
                    (
                        (
                            SELECT TOP 1
                                anoLetivo.Descricao

                            FROM dbo.AnoLetivo anoLetivo

                            WHERE anoLetivo.Ativo = 1

                            ORDER BY
                                CASE
                                    WHEN anoLetivo.DataInicio
                                         IS NULL
                                        THEN 1
                                    ELSE 0
                                END,

                                anoLetivo.DataInicio DESC,
                                anoLetivo.Id DESC
                        ),

                        N'Não definido'
                    ) AS AnoLetivo,


                    /* =========================================
                       ESCOLAS ATIVAS
                    ========================================== */

                    (
                        SELECT COUNT(1)

                        FROM dbo.Escola escola

                        WHERE escola.AgrupamentoId =
                              @AgrupamentoId

                          AND escola.Ativa = 1
                    ) AS TotalEscolas,


                    /* =========================================
                       TURMAS ATIVAS
                    ========================================== */

                    (
                        SELECT COUNT(1)

                        FROM dbo.Turma turma

                        INNER JOIN dbo.Escola escola
                            ON escola.Id =
                               turma.EscolaId

                        WHERE escola.AgrupamentoId =
                              @AgrupamentoId

                          AND escola.Ativa = 1
                          AND turma.Ativa = 1
                    ) AS TotalTurmas,


                    /* =========================================
                       ALUNOS ATIVOS
                    ========================================== */

                    (
                        SELECT COUNT(1)

                        FROM dbo.Aluno aluno

                        WHERE aluno.AgrupamentoId =
                              @AgrupamentoId

                          AND aluno.Ativo = 1
                    ) AS TotalAlunos,


                    /* =========================================
                       PROFESSORES ATIVOS
                    ========================================== */

                    (
                        SELECT COUNT(1)

                        FROM dbo.Professor professor

                        WHERE professor.AgrupamentoId =
                              @AgrupamentoId

                          AND professor.Ativo = 1
                    ) AS TotalProfessores,


                    /* =========================================
                       ENCARREGADOS ATIVOS
                    ========================================== */

                    (
                        SELECT COUNT(
                            DISTINCT encarregado.Id
                        )

                        FROM dbo.EncarregadoEducacao
                            encarregado

                        INNER JOIN dbo.AlunoEncarregado
                            associacao

                            ON associacao
                                .EncarregadoEducacaoId =
                               encarregado.Id

                        INNER JOIN dbo.Aluno aluno
                            ON aluno.Id =
                               associacao.AlunoId

                        WHERE aluno.AgrupamentoId =
                              @AgrupamentoId

                          AND aluno.Ativo = 1
                          AND associacao.Ativo = 1
                          AND encarregado.Ativo = 1
                    ) AS TotalEncarregados,


                    /* =========================================
                       TOTAL DE CONTAS DE UTILIZADOR
                    ========================================== */

                    (
                        SELECT COUNT(1)

                        FROM
                        (
                            SELECT aluno.UserId

                            FROM dbo.Aluno aluno

                            WHERE aluno.AgrupamentoId =
                                  @AgrupamentoId

                              AND aluno.Ativo = 1
                              AND aluno.UserId IS NOT NULL


                            UNION


                            SELECT professor.UserId

                            FROM dbo.Professor professor

                            WHERE professor.AgrupamentoId =
                                  @AgrupamentoId

                              AND professor.Ativo = 1
                              AND professor.UserId IS NOT NULL


                            UNION


                            SELECT encarregado.UserId

                            FROM dbo.EncarregadoEducacao
                                encarregado

                            INNER JOIN dbo.AlunoEncarregado
                                associacao

                                ON associacao
                                    .EncarregadoEducacaoId =
                                   encarregado.Id

                            INNER JOIN dbo.Aluno aluno
                                ON aluno.Id =
                                   associacao.AlunoId

                            WHERE aluno.AgrupamentoId =
                                  @AgrupamentoId

                              AND aluno.Ativo = 1
                              AND associacao.Ativo = 1
                              AND encarregado.Ativo = 1

                              AND encarregado.UserId
                                  IS NOT NULL


                            UNION


                            SELECT agrupamentoConta.UserId

                            FROM dbo.Agrupamento
                                agrupamentoConta

                            WHERE agrupamentoConta.Id =
                                  @AgrupamentoId

                              AND agrupamentoConta.Ativo = 1

                              AND agrupamentoConta.UserId
                                  IS NOT NULL

                        ) AS utilizadores

                    ) AS TotalUtilizadores,


                    /* =========================================
                       ALUNOS SEM ENCARREGADO
                    ========================================== */

                    (
                        SELECT COUNT(1)

                        FROM dbo.Aluno aluno

                        WHERE aluno.AgrupamentoId =
                              @AgrupamentoId

                          AND aluno.Ativo = 1

                          AND NOT EXISTS
                          (
                              SELECT 1

                              FROM dbo.AlunoEncarregado
                                  associacao

                              INNER JOIN
                                  dbo.EncarregadoEducacao
                                  encarregado

                                  ON encarregado.Id =
                                     associacao
                                        .EncarregadoEducacaoId

                              WHERE associacao.AlunoId =
                                    aluno.Id

                                AND associacao.Ativo = 1
                                AND encarregado.Ativo = 1
                          )
                    ) AS AlunosSemEncarregado,


                    /* =========================================
                       ALUNOS SEM TURMA ATIVA
                    ========================================== */

                    (
                        SELECT COUNT(1)

                        FROM dbo.Aluno aluno

                        WHERE aluno.AgrupamentoId =
                              @AgrupamentoId

                          AND aluno.Ativo = 1

                          AND NOT EXISTS
                          (
                              SELECT 1

                              FROM dbo.AlunoTurma
                                  alunoTurma

                              INNER JOIN dbo.Turma turma
                                  ON turma.Id =
                                     alunoTurma.TurmaId

                              INNER JOIN dbo.Escola escola
                                  ON escola.Id =
                                     turma.EscolaId

                              WHERE alunoTurma.AlunoId =
                                    aluno.Id

                                AND alunoTurma.Ate IS NULL

                                AND turma.Ativa = 1
                                AND escola.Ativa = 1

                                AND escola.AgrupamentoId =
                                    @AgrupamentoId
                          )
                    ) AS AlunosSemTurma,


                    /* =========================================
                       PROFESSORES SEM ATRIBUIÇÃO
                    ========================================== */

                    (
                        SELECT COUNT(1)

                        FROM dbo.Professor professor

                        WHERE professor.AgrupamentoId =
                              @AgrupamentoId

                          AND professor.Ativo = 1

                          AND NOT EXISTS
                          (
                              SELECT 1

                              FROM
                                  dbo.TurmaDisciplinaProfessor
                                  atribuicao

                              INNER JOIN
                                  dbo.TurmaDisciplina
                                  turmaDisciplina

                                  ON turmaDisciplina.Id =
                                     atribuicao
                                        .TurmaDisciplinaId

                              INNER JOIN dbo.Turma turma
                                  ON turma.Id =
                                     turmaDisciplina.TurmaId

                              INNER JOIN dbo.Escola escola
                                  ON escola.Id =
                                     turma.EscolaId

                              WHERE atribuicao.ProfessorId =
                                    professor.Id

                                AND atribuicao.Ate IS NULL

                                AND turma.Ativa = 1
                                AND escola.Ativa = 1

                                AND escola.AgrupamentoId =
                                    @AgrupamentoId
                          )
                    ) AS ProfessoresSemDisciplina,


                    /* =========================================
                       TURMAS SEM ALUNOS
                    ========================================== */

                    (
                        SELECT COUNT(1)

                        FROM dbo.Turma turma

                        INNER JOIN dbo.Escola escola
                            ON escola.Id =
                               turma.EscolaId

                        WHERE escola.AgrupamentoId =
                              @AgrupamentoId

                          AND escola.Ativa = 1
                          AND turma.Ativa = 1

                          AND NOT EXISTS
                          (
                              SELECT 1

                              FROM dbo.AlunoTurma
                                  alunoTurma

                              INNER JOIN dbo.Aluno aluno
                                  ON aluno.Id =
                                     alunoTurma.AlunoId

                              WHERE alunoTurma.TurmaId =
                                    turma.Id

                                AND alunoTurma.Ate IS NULL

                                AND aluno.Ativo = 1

                                AND aluno.AgrupamentoId =
                                    @AgrupamentoId
                          )
                    ) AS TurmasSemAlunos

                FROM dbo.Agrupamento agrupamento

                WHERE agrupamento.Id =
                      @AgrupamentoId

                  AND agrupamento.Ativo = 1;";

            using (SqlConnection conn =
                new SqlConnection(_connectionString))
            using (SqlCommand cmd =
                new SqlCommand(sql, conn))
            {
                cmd.Parameters
                    .Add(
                        "@AgrupamentoId",
                        SqlDbType.Int
                    )
                    .Value = agrupamentoId;

                conn.Open();

                using (SqlDataReader reader =
                    cmd.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        throw new InvalidOperationException(
                            "Não foi possível encontrar o agrupamento ativo."
                        );
                    }

                    PreencherCabecalho(
                        reader
                    );

                    PreencherEstatisticas(
                        reader
                    );

                    PreencherAtencoes(
                        reader
                    );

                    PreencherResumo(
                        reader
                    );
                }
            }
        }

        #endregion


        #region Preenchimento dos controlos

        private void PreencherCabecalho(
            SqlDataReader reader)
        {
            LblAgrupamento.Text =
                ValorTexto(
                    reader["NomeAgrupamento"],
                    "Agrupamento"
                );

            LblAnoLetivo.Text =
                ValorTexto(
                    reader["AnoLetivo"],
                    "Não definido"
                );
        }


        private void PreencherEstatisticas(
            SqlDataReader reader)
        {
            LblTotalEscolas.Text =
                ValorNumero(
                    reader["TotalEscolas"]
                );

            LblTotalTurmas.Text =
                ValorNumero(
                    reader["TotalTurmas"]
                );

            LblTotalAlunos.Text =
                ValorNumero(
                    reader["TotalAlunos"]
                );

            LblTotalProfessores.Text =
                ValorNumero(
                    reader["TotalProfessores"]
                );

            LblTotalEncarregados.Text =
                ValorNumero(
                    reader["TotalEncarregados"]
                );
        }


        private void PreencherAtencoes(
    SqlDataReader reader)
        {
            int alunosSemEncarregado =
                ValorInteiro(
                    reader["AlunosSemEncarregado"]
                );

            int alunosSemTurma =
                ValorInteiro(
                    reader["AlunosSemTurma"]
                );

            int professoresSemDisciplina =
                ValorInteiro(
                    reader["ProfessoresSemDisciplina"]
                );

            int turmasSemAlunos =
                ValorInteiro(
                    reader["TurmasSemAlunos"]
                );


            LblAlunosSemEncarregado.Text =
                alunosSemEncarregado.ToString();

            LblAlunosSemTurma.Text =
                alunosSemTurma.ToString();

            LblProfessoresSemDisciplina.Text =
                professoresSemDisciplina.ToString();

            LblTurmasSemAlunos.Text =
                turmasSemAlunos.ToString();


            LnkAlunosSemEncarregado.Visible =
                alunosSemEncarregado > 0;

            LnkAlunosSemTurma.Visible =
                alunosSemTurma > 0;

            LnkProfessoresSemDisciplina.Visible =
                professoresSemDisciplina > 0;

            LnkTurmasSemAlunos.Visible =
                turmasSemAlunos > 0;


            PnlAtencaoNecessaria.Visible =
                alunosSemEncarregado > 0
                ||
                alunosSemTurma > 0
                ||
                professoresSemDisciplina > 0
                ||
                turmasSemAlunos > 0;
        }


        private void PreencherResumo(
            SqlDataReader reader)
        {
            LblTotalUtilizadores.Text =
                ValorNumero(
                    reader["TotalUtilizadores"]
                );

            LblResumoEscolas.Text =
                ValorNumero(
                    reader["TotalEscolas"]
                );

            LblResumoTurmas.Text =
                ValorNumero(
                    reader["TotalTurmas"]
                );

            LblResumoAlunos.Text =
                ValorNumero(
                    reader["TotalAlunos"]
                );

            LblResumoProfessores.Text =
                ValorNumero(
                    reader["TotalProfessores"]
                );

            LblResumoEncarregados.Text =
                ValorNumero(
                    reader["TotalEncarregados"]
                );
        }

        #endregion


        #region Agrupamento autenticado

        private bool TryGetAgrupamentoId(
            out int agrupamentoId)
        {
            agrupamentoId = 0;

            if (Session["AgrupamentoID"] != null &&
                int.TryParse(
                    Session["AgrupamentoID"]
                        .ToString(),
                    out agrupamentoId))
            {
                return true;
            }

            Guid userId;

            if (!TryGetUserIdAtual(
                    out userId))
            {
                return false;
            }

            const string sql = @"
                SELECT TOP 1
                    Id

                FROM dbo.Agrupamento

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
                    .Value = userId;

                conn.Open();

                object resultado =
                    cmd.ExecuteScalar();

                if (resultado == null ||
                    resultado == DBNull.Value)
                {
                    return false;
                }

                agrupamentoId =
                    Convert.ToInt32(
                        resultado
                    );

                Session["AgrupamentoID"] =
                    agrupamentoId;

                return true;
            }
        }


        private bool TryGetUserIdAtual(
            out Guid userId)
        {
            userId = Guid.Empty;

            if (Session["UserId"] != null &&
                Guid.TryParse(
                    Session["UserId"].ToString(),
                    out userId))
            {
                return true;
            }

            MembershipUser utilizador =
                Membership.GetUser(
                    Page.User.Identity.Name,
                    false
                );

            if (utilizador == null ||
                utilizador.ProviderUserKey == null)
            {
                return false;
            }

            if (!Guid.TryParse(
                    utilizador.ProviderUserKey
                        .ToString(),
                    out userId))
            {
                return false;
            }

            Session["UserId"] =
                userId;

            return true;
        }

        #endregion


        #region Utilidades

        private string ValorNumero(
            object valor)
        {
            if (valor == null ||
                valor == DBNull.Value)
            {
                return "0";
            }

            return Convert
                .ToInt32(valor)
                .ToString();
        }

        private int ValorInteiro(
    object valor)
        {
            if (valor == null ||
                valor == DBNull.Value)
            {
                return 0;
            }

            return Convert.ToInt32(
                valor
            );
        }
        private string ValorTexto(
            object valor,
            string valorPadrao)
        {
            if (valor == null ||
                valor == DBNull.Value)
            {
                return valorPadrao;
            }

            string texto =
                valor.ToString().Trim();

            return string.IsNullOrWhiteSpace(
                texto)
                    ? valorPadrao
                    : texto;
        }


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