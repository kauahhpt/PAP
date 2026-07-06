using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace AlunoGest.aluno
{
    public partial class MinhaTurma : System.Web.UI.Page
    {
        private readonly string _connectionString =
            ConfigurationManager
                .ConnectionStrings["DefaultConnection"]
                .ConnectionString;


        #region Página

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    CarregarPagina();
                }
            }
            catch (Exception ex)
            {
                MostrarMensagem(
                    "Não foi possível carregar a turma: " + ex.Message,
                    true
                );
            }
        }

        private void CarregarPagina()
        {
            int? turmaId = ObterTurmaAtualAluno();

            if (!turmaId.HasValue)
            {
                LblResumoTurma.Text =
                    "Não estás associado a nenhuma turma.";

                LblTotalAlunos.Text = "0";
                LblTotalProfessores.Text = "0";

                PainelSemAlunos.Visible = true;
                PainelSemProfessores.Visible = true;

                return;
            }

            CarregarResumoTurma(turmaId.Value);
            CarregarAlunos(turmaId.Value);
            CarregarProfessores(turmaId.Value);
        }

        #endregion


        #region Aluno autenticado

        private int AlunoId
        {
            get
            {
                int idAluno;

                /*
                 * Se o ID do aluno já estiver guardado
                 * na sessão, usa-o diretamente.
                 */
                if (Session["AlunoID"] != null &&
                    int.TryParse(
                        Session["AlunoID"].ToString(),
                        out idAluno))
                {
                    return idAluno;
                }


                /*
                 * Caso não exista AlunoID na sessão,
                 * procura o aluno através do UserId.
                 */
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
                        "O utilizador da sessão é inválido."
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
                    cmd.Parameters.Add(
                        "@UserId",
                        SqlDbType.UniqueIdentifier
                    ).Value = userId;


                    conn.Open();


                    object resultado =
                        cmd.ExecuteScalar();


                    if (resultado == null ||
                        resultado == DBNull.Value)
                    {
                        throw new InvalidOperationException(
                            "Não foi encontrado um aluno associado a esta conta."
                        );
                    }


                    idAluno =
                        Convert.ToInt32(resultado);


                    Session["AlunoID"] =
                        idAluno;


                    return idAluno;
                }
            }
        }


        private int? ObterTurmaAtualAluno()
        {
            const string sql = @"
                SELECT TOP 1
                    TurmaId

                FROM dbo.AlunoTurma

                WHERE AlunoId = @AlunoId
                  AND Ate IS NULL

                ORDER BY Desde DESC;";


            using (SqlConnection conn =
                new SqlConnection(_connectionString))
            using (SqlCommand cmd =
                new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add(
                    "@AlunoId",
                    SqlDbType.Int
                ).Value = AlunoId;


                conn.Open();


                object resultado =
                    cmd.ExecuteScalar();


                if (resultado == null ||
                    resultado == DBNull.Value)
                {
                    return null;
                }


                return Convert.ToInt32(resultado);
            }
        }

        #endregion


        #region Resumo da turma

        private void CarregarResumoTurma(int turmaId)
        {
            const string sql = @"
                SELECT
                    CAST(
                        t.AnoEscolaridade
                        AS varchar(2)
                    )
                    + '.º '
                    + t.CodigoTurma
                    AS Turma,

                    e.Nome AS Escola,

                    al.Descricao AS AnoLetivo

                FROM dbo.Turma t

                INNER JOIN dbo.Escola e
                    ON e.Id = t.EscolaId

                INNER JOIN dbo.AnoLetivo al
                    ON al.Id = t.AnoLetivoId

                WHERE t.Id = @TurmaId;";


            using (SqlConnection conn =
                new SqlConnection(_connectionString))
            using (SqlCommand cmd =
                new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add(
                    "@TurmaId",
                    SqlDbType.Int
                ).Value = turmaId;


                conn.Open();


                using (SqlDataReader reader =
                    cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string turma =
                            reader["Turma"].ToString();

                        string escola =
                            reader["Escola"].ToString();

                        string anoLetivo =
                            reader["AnoLetivo"].ToString();


                        LblResumoTurma.Text =
                            turma +
                            " | " +
                            escola +
                            " | Ano letivo: " +
                            anoLetivo;
                    }
                }
            }
        }

        #endregion


        #region Alunos da turma

        private void CarregarAlunos(int turmaId)
        {
            const string sql = @"
                SELECT
                    a.Id,
                    a.NomeCompleto,
                    a.NumeroProcesso,
                    a.Email,
                    a.Foto

                FROM dbo.AlunoTurma at2

                INNER JOIN dbo.Aluno a
                    ON a.Id = at2.AlunoId

                WHERE at2.TurmaId = @TurmaId
                  AND at2.Ate IS NULL
                  AND a.Ativo = 1

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
                cmd.Parameters.Add(
                    "@TurmaId",
                    SqlDbType.Int
                ).Value = turmaId;


                adapter.Fill(tabela);
            }


            RepeaterAlunos.DataSource =
                tabela;

            RepeaterAlunos.DataBind();


            LblTotalAlunos.Text =
                tabela.Rows.Count.ToString();


            PainelSemAlunos.Visible =
                tabela.Rows.Count == 0;
        }

        #endregion


        #region Professores da turma

        private void CarregarProfessores(int turmaId)
        {
            /*
             * A tabela Professor da BD atual ainda não tem
             * uma coluna de fotografia.
             *
             * Por isso devolvemos FotoPerfil como NULL.
             * O front-end irá mostrar a inicial do professor.
             */
            const string sql = @"
                SELECT
                    p.Id,
                    p.Nome,

                    CAST(
                        NULL AS nvarchar(255)
                    ) AS FotoPerfil,

                    STRING_AGG(
                        disciplinas.Nome,
                        ', '
                    ) AS Disciplina

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

                GROUP BY
                    p.Id,
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
                cmd.Parameters.Add(
                    "@TurmaId",
                    SqlDbType.Int
                ).Value = turmaId;


                adapter.Fill(tabela);
            }


            RepeaterProfessores.DataSource =
                tabela;

            RepeaterProfessores.DataBind();


            LblTotalProfessores.Text =
                tabela.Rows.Count.ToString();


            PainelSemProfessores.Visible =
                tabela.Rows.Count == 0;
        }

        #endregion


        #region Fotografias e iniciais

        protected bool TemFoto(object foto)
        {
            if (foto == null ||
                foto == DBNull.Value)
            {
                return false;
            }


            string caminho =
                foto.ToString().Trim();


            return !string.IsNullOrWhiteSpace(caminho);
        }


        protected string ObterFoto(object foto)
        {
            if (!TemFoto(foto))
            {
                return "";
            }


            string caminho =
                foto.ToString().Trim();


            /*
             * URL externa.
             */
            if (caminho.StartsWith(
                    "http://",
                    StringComparison.OrdinalIgnoreCase) ||
                caminho.StartsWith(
                    "https://",
                    StringComparison.OrdinalIgnoreCase))
            {
                return caminho;
            }


            /*
             * Exemplo:
             * ~/uploads/perfis/foto.jpg
             */
            if (caminho.StartsWith("~/"))
            {
                return ResolveUrl(caminho);
            }


            /*
             * Exemplo:
             * /uploads/perfis/foto.jpg
             */
            if (caminho.StartsWith("/"))
            {
                return ResolveUrl(
                    "~" + caminho
                );
            }


            /*
             * Exemplo:
             * uploads/perfis/foto.jpg
             */
            return ResolveUrl(
                "~/" + caminho
            );
        }


        protected string ObterInicial(object nome)
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


        #region Mensagens

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