using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AlunoGest.agrupamento
{
    public partial class turma_professores : System.Web.UI.Page
    {
        // Ligação à base de dados
        string ligacao = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Só carregamos os dados na primeira abertura da página
            if (!IsPostBack)
            {
                lblMensagem.Visible = false;
                painelDisciplina.Visible = false;

                int idTurma = GetIdTurma();
                Session["IdTurmaProfessores"] = idTurma;

                if (idTurma == 0)
                {
                    MostrarMensagem("Não foi indicada nenhuma turma.");
                    return;
                }

                CarregarPagina(idTurma);
            }
        }

        protected void GridTurmaDisciplinas_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblMensagem.Visible = false;

            // Se não existir linha selecionada, não faz nada
            if (GridTurmaDisciplinas.SelectedDataKey == null)
                return;

            // Guardamos a disciplina selecionada em ViewState
            ViewState["TurmaDisciplinaIdSelecionada"] =
                Convert.ToInt32(GridTurmaDisciplinas.SelectedDataKey.Values["TurmaDisciplinaId"]);

            ViewState["DisciplinaIdSelecionada"] =
                Convert.ToInt32(GridTurmaDisciplinas.SelectedDataKey.Values["DisciplinaId"]);

            ViewState["OfertaEscolaSelecionada"] =
                Convert.ToInt32(GridTurmaDisciplinas.SelectedDataKey.Values["OfertaEscola"]);

            ViewState["NomeDisciplinaSelecionada"] = GetNomeDisciplinaSelecionada();

            MostrarPainelDisciplina();
        }

        protected void buttonOcultarPainelDisciplina_Click(object sender, EventArgs e)
        {
            // Oculta o painel e limpa a seleção atual
            painelDisciplina.Visible = false;
            lblMensagem.Visible = false;

            ViewState["TurmaDisciplinaIdSelecionada"] = null;
            ViewState["DisciplinaIdSelecionada"] = null;
            ViewState["OfertaEscolaSelecionada"] = null;
            ViewState["NomeDisciplinaSelecionada"] = null;

            GridTurmaDisciplinas.SelectedIndex = -1;
        }

        protected void buttonAssociarProfessor_Click(object sender, EventArgs e)
        {
            lblMensagem.Visible = false;

            int turmaDisciplinaId = GetTurmaDisciplinaIdSelecionada();

            if (turmaDisciplinaId == 0)
            {
                MostrarMensagem("Selecione uma disciplina.");
                return;
            }

            if (GridProfessores.SelectedDataKey == null)
            {
                MostrarMensagem("Selecione um professor disponível.");
                return;
            }

            int professorId = Convert.ToInt32(GridProfessores.SelectedDataKey.Value);

            using (SqlConnection conn = new SqlConnection(ligacao))
            {
                conn.Open();
                SqlTransaction tr = conn.BeginTransaction();

                try
                {
                    // Fecha o professor atual da disciplina, se existir
                    string sqlFecharAtual = @"
                        UPDATE dbo.TurmaDisciplinaProfessor
                        SET Ate = CAST(GETDATE() AS date)
                        WHERE TurmaDisciplinaId = @TurmaDisciplinaId
                          AND Ate IS NULL;";

                    using (SqlCommand cmd = new SqlCommand(sqlFecharAtual, conn, tr))
                    {
                        cmd.Parameters.AddWithValue("@TurmaDisciplinaId", turmaDisciplinaId);
                        cmd.ExecuteNonQuery();
                    }

                    // Insere o novo professor como atual
                    string sqlInserir = @"
                        INSERT INTO dbo.TurmaDisciplinaProfessor
                        (
                            TurmaDisciplinaId,
                            ProfessorId,
                            Desde,
                            Ate
                        )
                        VALUES
                        (
                            @TurmaDisciplinaId,
                            @ProfessorId,
                            CAST(GETDATE() AS date),
                            NULL
                        );";

                    using (SqlCommand cmd = new SqlCommand(sqlInserir, conn, tr))
                    {
                        cmd.Parameters.AddWithValue("@TurmaDisciplinaId", turmaDisciplinaId);
                        cmd.Parameters.AddWithValue("@ProfessorId", professorId);
                        cmd.ExecuteNonQuery();
                    }

                    tr.Commit();
                }
                catch
                {
                    tr.Rollback();
                    throw;
                }
            }

            int idTurma = GetIdTurma();

            // Recarrega a informação da página
            CarregarDisciplinasTurma(idTurma);
            RecarregarZonaDisciplina(turmaDisciplinaId);
            CarregarProfessoresParaDiretorTurma(idTurma);
            CarregarDiretorAtual(idTurma);
            CarregarHistoricoDiretores(idTurma);

            MostrarMensagem("Professor associado com sucesso.", false);
        }

        protected void GridTurmaProfessores_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "TerminarProfessor")
            {
                lblMensagem.Visible = false;

                int index = Convert.ToInt32(e.CommandArgument);
                int id = Convert.ToInt32(GridTurmaProfessores.DataKeys[index].Values["Id"]);
                int professorId = Convert.ToInt32(GridTurmaProfessores.DataKeys[index].Values["ProfessorId"]);
                int turmaDisciplinaId = GetTurmaDisciplinaIdSelecionada();
                int idTurma = GetIdTurma();

                using (SqlConnection conn = new SqlConnection(ligacao))
                {
                    conn.Open();
                    SqlTransaction tr = conn.BeginTransaction();

                    try
                    {
                        // Fecha a associação do professor à disciplina
                        string sqlFecharProfessor = @"
                            UPDATE dbo.TurmaDisciplinaProfessor
                            SET Ate = CAST(GETDATE() AS date)
                            WHERE Id = @Id
                              AND Ate IS NULL;";

                        using (SqlCommand cmd = new SqlCommand(sqlFecharProfessor, conn, tr))
                        {
                            cmd.Parameters.AddWithValue("@Id", id);
                            cmd.ExecuteNonQuery();
                        }

                        // Se esse professor era o diretor de turma atual, fecha também o diretor
                        string sqlFecharDiretor = @"
                            UPDATE dbo.TurmaDiretor
                            SET Ate = CAST(GETDATE() AS date)
                            WHERE TurmaId = @TurmaId
                              AND ProfessorId = @ProfessorId
                              AND Ate IS NULL;";

                        using (SqlCommand cmd = new SqlCommand(sqlFecharDiretor, conn, tr))
                        {
                            cmd.Parameters.AddWithValue("@TurmaId", idTurma);
                            cmd.Parameters.AddWithValue("@ProfessorId", professorId);
                            cmd.ExecuteNonQuery();
                        }

                        tr.Commit();
                    }
                    catch
                    {
                        tr.Rollback();
                        throw;
                    }
                }

                CarregarDisciplinasTurma(idTurma);
                RecarregarZonaDisciplina(turmaDisciplinaId);
                CarregarProfessoresParaDiretorTurma(idTurma);
                CarregarDiretorAtual(idTurma);
                CarregarHistoricoDiretores(idTurma);

                MostrarMensagem("Associação terminada com sucesso.", false);
            }
        }

        protected void buttonDefinirDiretorTurma_Click(object sender, EventArgs e)
        {
            lblMensagem.Visible = false;

            int idTurma = GetIdTurma();

            if (ddlDiretorTurma.SelectedValue == "")
            {
                MostrarMensagem("Selecione um professor para diretor de turma.");
                return;
            }

            int professorId = Convert.ToInt32(ddlDiretorTurma.SelectedValue);

            using (SqlConnection conn = new SqlConnection(ligacao))
            {
                conn.Open();
                SqlTransaction tr = conn.BeginTransaction();

                try
                {
                    // Fecha o diretor atual
                    string sqlFecharAtual = @"
                        UPDATE dbo.TurmaDiretor
                        SET Ate = CAST(GETDATE() AS date)
                        WHERE TurmaId = @TurmaId
                          AND Ate IS NULL;";

                    using (SqlCommand cmd = new SqlCommand(sqlFecharAtual, conn, tr))
                    {
                        cmd.Parameters.AddWithValue("@TurmaId", idTurma);
                        cmd.ExecuteNonQuery();
                    }

                    // Insere o novo diretor
                    string sqlInserir = @"
                        INSERT INTO dbo.TurmaDiretor
                        (
                            TurmaId,
                            ProfessorId,
                            Desde,
                            Ate
                        )
                        VALUES
                        (
                            @TurmaId,
                            @ProfessorId,
                            CAST(GETDATE() AS date),
                            NULL
                        );";

                    using (SqlCommand cmd = new SqlCommand(sqlInserir, conn, tr))
                    {
                        cmd.Parameters.AddWithValue("@TurmaId", idTurma);
                        cmd.Parameters.AddWithValue("@ProfessorId", professorId);
                        cmd.ExecuteNonQuery();
                    }

                    tr.Commit();
                }
                catch
                {
                    tr.Rollback();
                    throw;
                }
            }

            CarregarDiretorAtual(idTurma);
            CarregarHistoricoDiretores(idTurma);

            MostrarMensagem("Diretor de turma definido com sucesso.", false);
        }

        protected void GridDiretorAtual_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "TerminarDiretor")
            {
                lblMensagem.Visible = false;

                int index = Convert.ToInt32(e.CommandArgument);
                int id = Convert.ToInt32(GridDiretorAtual.DataKeys[index].Values["Id"]);
                int idTurma = GetIdTurma();

                string sql = @"
                    UPDATE dbo.TurmaDiretor
                    SET Ate = CAST(GETDATE() AS date)
                    WHERE Id = @Id
                      AND Ate IS NULL;";

                using (SqlConnection conn = new SqlConnection(ligacao))
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                CarregarDiretorAtual(idTurma);
                CarregarHistoricoDiretores(idTurma);

                MostrarMensagem("Diretor de turma terminado com sucesso.", false);
            }
        }

        protected void buttonVoltar_Click(object sender, EventArgs e)
        {
            int idEscola = GetIdEscolaDaTurma(GetIdTurma());
            Response.Redirect("~/agrupamento/turmas.aspx?id=" + idEscola);
        }

        private void CarregarPagina(int idTurma)
        {
            // Carrega tudo o que aparece ao abrir a página
            CarregarDadosCabecalhoTurma(idTurma);
            CarregarDisciplinasTurma(idTurma);
            CarregarProfessoresParaDiretorTurma(idTurma);
            CarregarDiretorAtual(idTurma);
            CarregarHistoricoDiretores(idTurma);
        }

        private void MostrarPainelDisciplina()
        {
            int turmaDisciplinaId = GetTurmaDisciplinaIdSelecionada();

            if (turmaDisciplinaId == 0)
            {
                painelDisciplina.Visible = false;
                return;
            }

            painelDisciplina.Visible = true;
            lblDisciplinaSelecionada.Text = "Disciplina selecionada: " + GetNomeDisciplinaSelecionadaGuardada();

            RecarregarZonaDisciplina(turmaDisciplinaId);
        }

        private void RecarregarZonaDisciplina(int turmaDisciplinaId)
        {
            // Recarrega as três grelhas ligadas à disciplina escolhida
            CarregarProfessoresDisponiveis(turmaDisciplinaId);
            CarregarProfessoresAtuaisDaDisciplina(turmaDisciplinaId);
            CarregarHistoricoProfessoresDaDisciplina(turmaDisciplinaId);
        }

        private void CarregarDadosCabecalhoTurma(int idTurma)
        {
            // Mostra uma identificação simples da turma
            string sql = @"
                SELECT
                    e.Nome AS Escola,
                    al.Descricao AS AnoLetivo,
                    t.CodigoTurma,
                    pc.Nome AS Curso
                FROM dbo.Turma t
                INNER JOIN dbo.Escola e ON e.Id = t.EscolaId
                INNER JOIN dbo.AnoLetivo al ON al.Id = t.AnoLetivoId
                INNER JOIN dbo.PlanoCurricular pc ON pc.Id = t.PlanoCurricularId
                WHERE t.Id = @IdTurma;";

            using (SqlConnection conn = new SqlConnection(ligacao))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@IdTurma", idTurma);
                conn.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        lblTurma.Text =
                            "Turma: " + dr["CodigoTurma"].ToString() +
                            " | Ano letivo: " + dr["AnoLetivo"].ToString() +
                            " | Curso: " + dr["Curso"].ToString() +
                            " | Escola: " + dr["Escola"].ToString();
                    }
                }
            }
        }

        private void CarregarDisciplinasTurma(int idTurma)
        {
            // Carrega as disciplinas da turma.
            // Se for a disciplina genérica "Oferta de Escola",
            // mostra o nome definido pelo agrupamento.
            DataTable dt = new DataTable();

            string sql = @"
                SELECT
                    td.Id AS TurmaDisciplinaId,
                    td.DisciplinaId,

                    CASE
                        WHEN d.Nome = N'Oferta de Escola'
                             AND ISNULL(oea.Nome, N'') <> N''
                        THEN N'Oferta de Escola'
                        ELSE gd.Nome
                    END AS GrupoDisciplinar,

                    CASE
                        WHEN d.Nome = N'Oferta de Escola'
                             AND ISNULL(oea.Nome, N'') <> N''
                        THEN oea.Nome
                        ELSE d.Nome
                    END AS Disciplina,

                    pcd.Natureza,

                    CASE
                        WHEN d.Nome = N'Oferta de Escola' THEN 1
                        ELSE 0
                    END AS OfertaEscola,

                    ISNULL(pAtual.Nome, '-') AS ProfessorAtual

                FROM dbo.TurmaDisciplina td
                INNER JOIN dbo.Turma t
                    ON t.Id = td.TurmaId
                INNER JOIN dbo.Escola e
                    ON e.Id = t.EscolaId
                INNER JOIN dbo.Disciplina d
                    ON d.Id = td.DisciplinaId
                INNER JOIN dbo.GrupoDisciplinar gd
                    ON gd.Id = d.GrupoDisciplinarId
                INNER JOIN dbo.PlanoCurricularDisciplina pcd
                    ON pcd.Id = td.PlanoCurricularDisciplinaId

                OUTER APPLY
                (
                    SELECT TOP 1 oe.Nome
                    FROM dbo.OfertaEscolaAgrupamento oe
                    WHERE oe.AgrupamentoId = e.AgrupamentoId
                      AND oe.AnoLetivoId = t.AnoLetivoId
                      AND oe.Ativa = 1
                    ORDER BY oe.Id DESC
                ) oea

                OUTER APPLY
                (
                    SELECT TOP 1 p2.Nome
                    FROM dbo.TurmaDisciplinaProfessor tdp2
                    INNER JOIN dbo.Professor p2
                        ON p2.Id = tdp2.ProfessorId
                    WHERE tdp2.TurmaDisciplinaId = td.Id
                      AND tdp2.Ate IS NULL
                    ORDER BY tdp2.Desde DESC, p2.Nome
                ) pAtual

                WHERE td.TurmaId = @TurmaId
                ORDER BY
                    CASE
                        WHEN d.Nome = N'Oferta de Escola'
                             AND ISNULL(oea.Nome, N'') <> N''
                        THEN oea.Nome
                        ELSE d.Nome
                    END;";

            using (SqlConnection conn = new SqlConnection(ligacao))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddWithValue("@TurmaId", idTurma);
                da.Fill(dt);
            }

            GridTurmaDisciplinas.DataSource = dt;
            GridTurmaDisciplinas.DataBind();
        }

        private void CarregarProfessoresDisponiveis(int turmaDisciplinaId)
        {
            // Se a disciplina for uma disciplina normal,
            // os professores vêm de ProfessorDisciplina.
            //
            // Se for a Oferta de Escola,
            // os professores vêm dos grupos disciplinares associados
            // em OfertaEscolaAgrupamentoGrupoDisciplinar.
            DataTable dt = new DataTable();

            if (EhOfertaEscolaSelecionada())
            {
                string sqlOfertaEscola = @"
                    SELECT DISTINCT
                        p.Id AS ProfessorId,
                        p.Nome,
                        p.NumeroProcesso,
                        p.Email
                    FROM dbo.TurmaDisciplina td
                    INNER JOIN dbo.Turma t
                        ON t.Id = td.TurmaId
                    INNER JOIN dbo.Escola e
                        ON e.Id = t.EscolaId
                    INNER JOIN dbo.OfertaEscolaAgrupamento oe
                        ON oe.AgrupamentoId = e.AgrupamentoId
                       AND oe.AnoLetivoId = t.AnoLetivoId
                       AND oe.Ativa = 1
                    INNER JOIN dbo.OfertaEscolaAgrupamentoGrupoDisciplinar oegd
                        ON oegd.OfertaEscolaAgrupamentoId = oe.Id
                    INNER JOIN dbo.Disciplina dProf
                        ON dProf.GrupoDisciplinarId = oegd.GrupoDisciplinarId
                    INNER JOIN dbo.ProfessorDisciplina pd
                        ON pd.DisciplinaId = dProf.Id
                    INNER JOIN dbo.Professor p
                        ON p.Id = pd.ProfessorId
                    WHERE td.Id = @TurmaDisciplinaId
                      AND p.Ativo = 1
                      AND (pd.Ate IS NULL OR pd.Ate >= CAST(GETDATE() AS date))
                      AND p.Id NOT IN
                      (
                          SELECT tdp.ProfessorId
                          FROM dbo.TurmaDisciplinaProfessor tdp
                          WHERE tdp.TurmaDisciplinaId = @TurmaDisciplinaId
                            AND tdp.Ate IS NULL
                      )
                    ORDER BY p.Nome;";

                using (SqlConnection conn = new SqlConnection(ligacao))
                using (SqlCommand cmd = new SqlCommand(sqlOfertaEscola, conn))
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    cmd.Parameters.AddWithValue("@TurmaDisciplinaId", turmaDisciplinaId);
                    da.Fill(dt);
                }
            }
            else
            {
                string sqlNormal = @"
                    SELECT DISTINCT
                        p.Id AS ProfessorId,
                        p.Nome,
                        p.NumeroProcesso,
                        p.Email
                    FROM dbo.TurmaDisciplina td
                    INNER JOIN dbo.ProfessorDisciplina pd
                        ON pd.DisciplinaId = td.DisciplinaId
                    INNER JOIN dbo.Professor p
                        ON p.Id = pd.ProfessorId
                    WHERE td.Id = @TurmaDisciplinaId
                      AND p.Ativo = 1
                      AND (pd.Ate IS NULL OR pd.Ate >= CAST(GETDATE() AS date))
                      AND p.Id NOT IN
                      (
                          SELECT tdp.ProfessorId
                          FROM dbo.TurmaDisciplinaProfessor tdp
                          WHERE tdp.TurmaDisciplinaId = @TurmaDisciplinaId
                            AND tdp.Ate IS NULL
                      )
                    ORDER BY p.Nome;";

                using (SqlConnection conn = new SqlConnection(ligacao))
                using (SqlCommand cmd = new SqlCommand(sqlNormal, conn))
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    cmd.Parameters.AddWithValue("@TurmaDisciplinaId", turmaDisciplinaId);
                    da.Fill(dt);
                }
            }

            GridProfessores.DataSource = dt;
            GridProfessores.DataBind();
        }

        private void CarregarProfessoresAtuaisDaDisciplina(int turmaDisciplinaId)
        {
            // Professor atual = registo com Ate a NULL
            DataTable dt = new DataTable();

            string sql = @"
                SELECT
                    tdp.Id,
                    p.Id AS ProfessorId,
                    p.Nome,
                    p.NumeroProcesso,
                    tdp.Desde
                FROM dbo.TurmaDisciplinaProfessor tdp
                INNER JOIN dbo.Professor p
                    ON p.Id = tdp.ProfessorId
                WHERE tdp.TurmaDisciplinaId = @TurmaDisciplinaId
                  AND tdp.Ate IS NULL
                ORDER BY p.Nome;";

            using (SqlConnection conn = new SqlConnection(ligacao))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddWithValue("@TurmaDisciplinaId", turmaDisciplinaId);
                da.Fill(dt);
            }

            GridTurmaProfessores.DataSource = dt;
            GridTurmaProfessores.DataBind();
        }

        private void CarregarHistoricoProfessoresDaDisciplina(int turmaDisciplinaId)
        {
            // Histórico completo dos professores da disciplina
            DataTable dt = new DataTable();

            string sql = @"
                SELECT
                    p.Nome,
                    p.NumeroProcesso,
                    tdp.Desde,
                    tdp.Ate,
                    CASE
                        WHEN tdp.Ate IS NULL THEN 'Atual'
                        ELSE 'Histórico'
                    END AS Estado
                FROM dbo.TurmaDisciplinaProfessor tdp
                INNER JOIN dbo.Professor p
                    ON p.Id = tdp.ProfessorId
                WHERE tdp.TurmaDisciplinaId = @TurmaDisciplinaId
                ORDER BY
                    CASE WHEN tdp.Ate IS NULL THEN 0 ELSE 1 END,
                    tdp.Desde DESC,
                    p.Nome;";

            using (SqlConnection conn = new SqlConnection(ligacao))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddWithValue("@TurmaDisciplinaId", turmaDisciplinaId);
                da.Fill(dt);
            }

            GridHistoricoProfessores.DataSource = dt;
            GridHistoricoProfessores.DataBind();
        }

        private void CarregarProfessoresParaDiretorTurma(int idTurma)
        {
            // O diretor de turma só pode ser um professor atual da turma
            DataTable dt = new DataTable();

            string sql = @"
                SELECT DISTINCT
                    p.Id,
                    p.Nome
                FROM dbo.TurmaDisciplina td
                INNER JOIN dbo.TurmaDisciplinaProfessor tdp
                    ON tdp.TurmaDisciplinaId = td.Id
                INNER JOIN dbo.Professor p
                    ON p.Id = tdp.ProfessorId
                WHERE td.TurmaId = @TurmaId
                  AND tdp.Ate IS NULL
                ORDER BY p.Nome;";

            using (SqlConnection conn = new SqlConnection(ligacao))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddWithValue("@TurmaId", idTurma);
                da.Fill(dt);
            }

            ddlDiretorTurma.DataSource = dt;
            ddlDiretorTurma.DataTextField = "Nome";
            ddlDiretorTurma.DataValueField = "Id";
            ddlDiretorTurma.DataBind();
            ddlDiretorTurma.Items.Insert(0, new ListItem("-- selecionar --", ""));
        }

        private void CarregarDiretorAtual(int idTurma)
        {
            // Diretor atual = registo com Ate a NULL
            DataTable dt = new DataTable();

            string sql = @"
                SELECT
                    td.Id,
                    p.Id AS ProfessorId,
                    p.Nome,
                    p.NumeroProcesso,
                    td.Desde
                FROM dbo.TurmaDiretor td
                INNER JOIN dbo.Professor p
                    ON p.Id = td.ProfessorId
                WHERE td.TurmaId = @TurmaId
                  AND td.Ate IS NULL
                ORDER BY p.Nome;";

            using (SqlConnection conn = new SqlConnection(ligacao))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddWithValue("@TurmaId", idTurma);
                da.Fill(dt);
            }

            GridDiretorAtual.DataSource = dt;
            GridDiretorAtual.DataBind();
        }

        private void CarregarHistoricoDiretores(int idTurma)
        {
            // Histórico completo dos diretores de turma
            DataTable dt = new DataTable();

            string sql = @"
                SELECT
                    p.Nome,
                    p.NumeroProcesso,
                    td.Desde,
                    td.Ate,
                    CASE
                        WHEN td.Ate IS NULL THEN 'Atual'
                        ELSE 'Histórico'
                    END AS Estado
                FROM dbo.TurmaDiretor td
                INNER JOIN dbo.Professor p
                    ON p.Id = td.ProfessorId
                WHERE td.TurmaId = @TurmaId
                ORDER BY
                    CASE WHEN td.Ate IS NULL THEN 0 ELSE 1 END,
                    td.Desde DESC,
                    p.Nome;";

            using (SqlConnection conn = new SqlConnection(ligacao))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddWithValue("@TurmaId", idTurma);
                da.Fill(dt);
            }

            GridHistoricoDiretores.DataSource = dt;
            GridHistoricoDiretores.DataBind();
        }

        private int GetIdTurma()
        {
            // Vai buscar o id da turma da querystring ou da sessão
            int idTurma = 0;

            if (Request.QueryString["idTurma"] != null)
                int.TryParse(Request.QueryString["idTurma"], out idTurma);
            else if (Session["IdTurmaProfessores"] != null)
                int.TryParse(Session["IdTurmaProfessores"].ToString(), out idTurma);

            return idTurma;
        }

        private int GetTurmaDisciplinaIdSelecionada()
        {
            // Vai buscar o id da disciplina selecionada guardado em ViewState
            if (ViewState["TurmaDisciplinaIdSelecionada"] == null)
                return 0;

            return Convert.ToInt32(ViewState["TurmaDisciplinaIdSelecionada"]);
        }

        private bool EhOfertaEscolaSelecionada()
        {
            // Diz se a disciplina escolhida é a Oferta de Escola
            if (ViewState["OfertaEscolaSelecionada"] == null)
                return false;

            return Convert.ToInt32(ViewState["OfertaEscolaSelecionada"]) == 1;
        }

        private string GetNomeDisciplinaSelecionada()
        {
            // Vai buscar o nome da disciplina diretamente da linha selecionada
            if (GridTurmaDisciplinas.SelectedRow == null)
                return "";

            return GridTurmaDisciplinas.SelectedRow.Cells[2].Text;
        }

        private string GetNomeDisciplinaSelecionadaGuardada()
        {
            // Vai buscar o nome guardado em ViewState
            if (ViewState["NomeDisciplinaSelecionada"] == null)
                return "";

            return ViewState["NomeDisciplinaSelecionada"].ToString();
        }

        private int GetIdEscolaDaTurma(int idTurma)
        {
            // Necessário para o botão Voltar
            string sql = "SELECT EscolaId FROM dbo.Turma WHERE Id = @IdTurma;";

            using (SqlConnection conn = new SqlConnection(ligacao))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@IdTurma", idTurma);
                conn.Open();

                object valor = cmd.ExecuteScalar();

                if (valor == null || valor == DBNull.Value)
                    return 0;

                return Convert.ToInt32(valor);
            }
        }

        private void MostrarMensagem(string mensagem, bool erro = true)
        {
            // Mostra mensagem no topo do formulário
            lblMensagem.Visible = true;
            lblMensagem.Text = mensagem;
            lblMensagem.CssClass = erro
                ? "alert alert-warning d-block"
                : "alert alert-success d-block";
        }
    }
}