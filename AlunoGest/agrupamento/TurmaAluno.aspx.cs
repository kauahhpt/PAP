using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace AlunoGest.agrupamento
{
    public partial class TurmaAlunos : System.Web.UI.Page
    {
        #region Campos

        private string _Ligacao = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        #endregion

        #region Eventos de Página

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LblMensagem.Visible = false;

                int IdTurma = GetIdTurma();

                if (IdTurma == 0)
                {
                    MostrarMensagem("Não foi indicada nenhuma turma.");
                    return;
                }

                Session["TurmaIdAlunos"] = IdTurma;

                CarregarNomeTurma(IdTurma);
                CarregarAlunosDisponiveis(IdTurma);

                TxtDesde.Text = DateTime.Today.ToString("yyyy-MM-dd");

                CarregarAlunos(IdTurma);
                CarregarHistorico(IdTurma);
            }
        }

        #endregion

        #region Eventos de Controlos

        protected void ButtonAdicionarAluno_Click(object sender, EventArgs e)
        {
            LblMensagem.Visible = false;

            int IdTurma = GetIdTurma();

            if (IdTurma == 0)
            {
                MostrarMensagem("Não foi possível determinar a turma.");
                return;
            }

            if (DdlAlunos.SelectedValue == "")
            {
                MostrarMensagem("Seleciona um aluno.");
                return;
            }

            if (TxtDesde.Text.Trim() == "")
            {
                MostrarMensagem("Indica a data de entrada do aluno.");
                return;
            }

            int IdAluno = Convert.ToInt32(DdlAlunos.SelectedValue);
            DateTime Desde;

            if (!DateTime.TryParse(TxtDesde.Text.Trim(), out Desde))
            {
                MostrarMensagem("Data de entrada inválida.");
                return;
            }

            bool TemPortugues = ChkTemPortugues.Checked;
            bool TemEMRC = ChkTemEMRC.Checked;

            // Um aluno só pode estar activo numa turma de cada vez,
            // independentemente da escola/ano.
            if (AlunoJaActivoNoutraTurma(IdAluno))
            {
                MostrarMensagem("Este aluno já está activo noutra turma.");
                return;
            }

            try
            {
                string Sql = @"
                    INSERT INTO dbo.AlunoTurma
                    (
                        AlunoId,
                        TurmaId,
                        Desde,
                        Ate,
                        TemPortugues,
                        TemEMRC
                    )
                    VALUES
                    (
                        @AlunoId,
                        @TurmaId,
                        @Desde,
                        NULL,
                        @TemPortugues,
                        @TemEMRC
                    );";

                using (SqlConnection Conn = new SqlConnection(_Ligacao))
                using (SqlCommand Cmd = new SqlCommand(Sql, Conn))
                {
                    Cmd.Parameters.AddWithValue("@AlunoId", IdAluno);
                    Cmd.Parameters.AddWithValue("@TurmaId", IdTurma);
                    Cmd.Parameters.AddWithValue("@Desde", Desde);
                    Cmd.Parameters.AddWithValue("@TemPortugues", TemPortugues);
                    Cmd.Parameters.AddWithValue("@TemEMRC", TemEMRC);

                    Conn.Open();
                    Cmd.ExecuteNonQuery();
                }

                MostrarMensagem("Aluno adicionado com sucesso.", false);

                CarregarAlunosDisponiveis(IdTurma);
                CarregarAlunos(IdTurma);
                CarregarHistorico(IdTurma);
            }
            catch (Exception Ex)
            {
                MostrarMensagem("Erro ao adicionar aluno: " + Ex.Message);
            }
        }

        protected void GridAlunos_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "RemoverAluno")
                return;

            int IdTurma = GetIdTurma();
            int IdRegistoAlunoTurma = Convert.ToInt32(e.CommandArgument);

            try
            {
                // Não apagamos — apenas preenchemos a data de saída
                // para manter o histórico.
                string Sql = @"
                    UPDATE dbo.AlunoTurma
                    SET Ate = @Hoje
                    WHERE Id      = @Id
                      AND TurmaId = @TurmaId
                      AND Ate IS NULL;";

                using (SqlConnection Conn = new SqlConnection(_Ligacao))
                using (SqlCommand Cmd = new SqlCommand(Sql, Conn))
                {
                    Cmd.Parameters.AddWithValue("@Hoje", DateTime.Today);
                    Cmd.Parameters.AddWithValue("@Id", IdRegistoAlunoTurma);
                    Cmd.Parameters.AddWithValue("@TurmaId", IdTurma);

                    Conn.Open();
                    Cmd.ExecuteNonQuery();
                }

                MostrarMensagem("Aluno removido da turma.", false);

                CarregarAlunosDisponiveis(IdTurma);
                CarregarAlunos(IdTurma);
                CarregarHistorico(IdTurma);
            }
            catch (Exception Ex)
            {
                MostrarMensagem("Erro ao remover aluno: " + Ex.Message);
            }
        }

        protected void ButtonVoltar_Click(object sender, EventArgs e)
        {
            int IdTurma = GetIdTurma();
            int IdEscola = GetIdEscolaDaTurma(IdTurma);

            Response.Redirect("~/agrupamento/turmas.aspx?id=" + IdEscola);
        }

        #endregion

        #region Carregamentos

        private void CarregarNomeTurma(int IdTurma)
        {
            string Sql = @"
                SELECT
                    CAST(t.AnoEscolaridade AS VARCHAR(2)) + '.º' + t.CodigoTurma AS CodigoTurma,
                    e.Nome       AS NomeEscola,
                    al.Descricao AS AnoLetivo
                FROM dbo.Turma t
                INNER JOIN dbo.Escola    e  ON e.Id  = t.EscolaId
                INNER JOIN dbo.AnoLetivo al ON al.Id = t.AnoLetivoId
                WHERE t.Id = @Id;";

            using (SqlConnection Conn = new SqlConnection(_Ligacao))
            using (SqlCommand Cmd = new SqlCommand(Sql, Conn))
            {
                Cmd.Parameters.AddWithValue("@Id", IdTurma);
                Conn.Open();

                using (SqlDataReader Dr = Cmd.ExecuteReader())
                {
                    if (Dr.Read())
                    {
                        LblTurma.Text = string.Format(
                            "Turma: {0} — {1} — {2}",
                            Dr["CodigoTurma"],
                            Dr["NomeEscola"],
                            Dr["AnoLetivo"]);
                    }
                }
            }
        }

        private void CarregarAlunosDisponiveis(int IdTurma)
        {
            // Mostra apenas alunos do mesmo agrupamento que NÃO estão
            // activos em NENHUMA turma neste momento.
            string Sql = @"
                SELECT
                    a.Id,
                    a.NomeCompleto + ' (' + ISNULL(a.NumeroProcesso, 'sem n.º') + ')' AS Nome
                FROM dbo.Aluno a
                INNER JOIN dbo.Turma t ON t.Id = @TurmaId
                WHERE a.AgrupamentoId =
                (
                    SELECT AgrupamentoId FROM dbo.Escola WHERE Id = t.EscolaId
                )
                  AND a.Ativo = 1
                  AND a.Id NOT IN
                  (
                      SELECT AlunoId
                      FROM dbo.AlunoTurma
                      WHERE Ate IS NULL
                  )
                ORDER BY a.NomeCompleto;";

            DataTable Dt = new DataTable();

            using (SqlConnection Conn = new SqlConnection(_Ligacao))
            using (SqlCommand Cmd = new SqlCommand(Sql, Conn))
            using (SqlDataAdapter Da = new SqlDataAdapter(Cmd))
            {
                Cmd.Parameters.AddWithValue("@TurmaId", IdTurma);
                Da.Fill(Dt);
            }

            DdlAlunos.DataSource = Dt;
            DdlAlunos.DataTextField = "Nome";
            DdlAlunos.DataValueField = "Id";
            DdlAlunos.DataBind();

            DdlAlunos.Items.Insert(0, new ListItem("-- selecionar --", ""));
        }

        private void CarregarAlunos(int IdTurma)
        {
            string Sql = @"
                SELECT
                    at2.Id,
                    a.NomeCompleto,
                    a.NumeroProcesso,
                    at2.Desde,
                    at2.TemPortugues,
                    at2.TemEMRC
                FROM dbo.AlunoTurma at2
                INNER JOIN dbo.Aluno a ON a.Id = at2.AlunoId
                WHERE at2.TurmaId = @TurmaId
                  AND at2.Ate IS NULL
                ORDER BY a.NomeCompleto;";

            DataTable Dt = new DataTable();

            using (SqlConnection Conn = new SqlConnection(_Ligacao))
            using (SqlCommand Cmd = new SqlCommand(Sql, Conn))
            using (SqlDataAdapter Da = new SqlDataAdapter(Cmd))
            {
                Cmd.Parameters.AddWithValue("@TurmaId", IdTurma);
                Da.Fill(Dt);
            }

            GridAlunos.DataSource = Dt;
            GridAlunos.DataBind();
        }

        private void CarregarHistorico(int IdTurma)
        {
            string Sql = @"
                SELECT
                    a.NomeCompleto,
                    a.NumeroProcesso,
                    at2.Desde,
                    at2.Ate,
                    at2.TemPortugues,
                    at2.TemEMRC
                FROM dbo.AlunoTurma at2
                INNER JOIN dbo.Aluno a ON a.Id = at2.AlunoId
                WHERE at2.TurmaId = @TurmaId
                  AND at2.Ate IS NOT NULL
                ORDER BY at2.Ate DESC, a.NomeCompleto;";

            DataTable Dt = new DataTable();

            using (SqlConnection Conn = new SqlConnection(_Ligacao))
            using (SqlCommand Cmd = new SqlCommand(Sql, Conn))
            using (SqlDataAdapter Da = new SqlDataAdapter(Cmd))
            {
                Cmd.Parameters.AddWithValue("@TurmaId", IdTurma);
                Da.Fill(Dt);
            }

            GridHistorico.DataSource = Dt;
            GridHistorico.DataBind();
        }

        #endregion
            
        #region Auxiliares

        private bool AlunoJaActivoNoutraTurma(int IdAluno)
        {
            // Verifica se o aluno está activo em QUALQUER turma neste momento.
            string Sql = @"
                SELECT COUNT(1)
                FROM dbo.AlunoTurma
                WHERE AlunoId = @AlunoId
                  AND Ate IS NULL;";

            using (SqlConnection Conn = new SqlConnection(_Ligacao))
            using (SqlCommand Cmd = new SqlCommand(Sql, Conn))
            {
                Cmd.Parameters.AddWithValue("@AlunoId", IdAluno);
                Conn.Open();
                return Convert.ToInt32(Cmd.ExecuteScalar()) > 0;
            }
        }

        private int GetIdEscolaDaTurma(int IdTurma)
        {
            string Sql = "SELECT EscolaId FROM dbo.Turma WHERE Id = @Id;";

            using (SqlConnection Conn = new SqlConnection(_Ligacao))
            using (SqlCommand Cmd = new SqlCommand(Sql, Conn))
            {
                Cmd.Parameters.AddWithValue("@Id", IdTurma);
                Conn.Open();

                object Valor = Cmd.ExecuteScalar();

                if (Valor == null || Valor == DBNull.Value)
                    return 0;

                return Convert.ToInt32(Valor);
            }
        }

        private int GetIdTurma()
        {
            int IdTurma = 0;

            if (Request.QueryString["idTurma"] != null)
                int.TryParse(Request.QueryString["idTurma"], out IdTurma);
            else if (Session["TurmaIdAlunos"] != null)
                int.TryParse(Session["TurmaIdAlunos"].ToString(), out IdTurma);

            return IdTurma;
        }

        private void MostrarMensagem(string Mensagem, bool Erro = true)
        {
            LblMensagem.Visible = true;
            LblMensagem.Text = Mensagem;
            LblMensagem.CssClass = Erro
                ? "alert alert-warning d-block"
                : "alert alert-success d-block";
        }

        #endregion
    }
}