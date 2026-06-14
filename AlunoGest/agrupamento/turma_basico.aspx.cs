using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AlunoGest.agrupamento
{
    public partial class turma_basico : System.Web.UI.Page
    {
        // String de ligação à base de dados
        string ligacao = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        // Plano 1 = 3.º ciclo
        int idPlano = 1;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Só carregamos os dados iniciais na primeira vez
            if (!IsPostBack)
            {
                lblMensagem.Visible = false;
                painelDisciplinas.Visible = false;

                int idEscola = GetIdEscola();
                Session["EscolaIdTurmaBasico"] = idEscola;

                CarregarNomeEscola(idEscola);
                CarregarAnosLetivos();
                CarregarLE2();

                // Se vier anoLetivoId na query string, tenta selecionar logo esse ano
                string anoLetivoQuery = Request.QueryString["anoLetivoId"];
                if (!string.IsNullOrEmpty(anoLetivoQuery))
                {
                    if (ddlAnoLetivo.Items.FindByValue(anoLetivoQuery) != null)
                        ddlAnoLetivo.SelectedValue = anoLetivoQuery;
                }

                string modo = GetModo();

                if (modo == "editar")
                {
                    int idTurma = GetIdTurma();

                    if (idTurma > 0)
                    {
                        CarregarTurma(idTurma);

                        // Em edição não deixamos mudar o ano de escolaridade,
                        // porque isso mudaria a estrutura da turma
                        ddlAnoEscolaridade.Enabled = false;

                        MostrarPreview();
                    }
                    else
                    {
                        MostrarMensagem("Não foi indicada nenhuma turma para editar.");
                    }
                }
                else
                {
                    chkAtiva.Checked = true;
                }
            }
        }

        protected void buttonGuardar_Click(object sender, EventArgs e)
        {
            lblMensagem.Visible = false;

            if (ddlAnoLetivo.SelectedValue == "")
            {
                MostrarMensagem("Selecione o ano letivo.");
                return;
            }

            if (ddlAnoEscolaridade.SelectedValue == "")
            {
                MostrarMensagem("Selecione o ano de escolaridade.");
                return;
            }

            if (txtCodigoTurma.Text.Trim() == "")
            {
                MostrarMensagem("Indique o código da turma.");
                return;
            }

            if (ddlLE2.SelectedValue == "")
            {
                MostrarMensagem("Selecione a LE II.");
                return;
            }

            int idEscola = GetIdEscola();
            int idAnoLetivo = Convert.ToInt32(ddlAnoLetivo.SelectedValue);
            byte ano = Convert.ToByte(ddlAnoEscolaridade.SelectedValue);
            string codigoTurma = txtCodigoTurma.Text.Trim();
            int idLE2 = Convert.ToInt32(ddlLE2.SelectedValue);
            bool ativa = chkAtiva.Checked;
            bool emr = chkEMR.Checked;
            bool plnm = chkPLNM.Checked;

            int idAgrupamento = GetIdAgrupamentoDaEscola(idEscola);
            int idOfertaEscola = GetIdOfertaEscolaAgrupamento(idAgrupamento, idAnoLetivo);

            // A Oferta de Escola é obrigatória no 3.º ciclo.
            // Por isso, tem de existir configuração no turmas.aspx.
            if (idOfertaEscola == 0)
            {
                MostrarMensagem("Antes de criar a turma, defina a Oferta de Escola no ecrã das turmas.");
                return;
            }

            string modo = GetModo();

            try
            {
                if (modo == "editar")
                {
                    int idTurma = GetIdTurma();

                    if (idTurma == 0)
                    {
                        MostrarMensagem("Não foi indicada nenhuma turma para editar.");
                        return;
                    }

                    AtualizarTurma(idTurma, idEscola, idAnoLetivo, codigoTurma, ativa, ano, idLE2, emr, plnm, idOfertaEscola);

                    Response.Redirect("~/agrupamento/turmas.aspx?id=" + idEscola + "&anoLetivoId=" + idAnoLetivo);
                }
                else
                {
                    if (TurmaJaExiste(idEscola, idAnoLetivo, codigoTurma))
                    {
                        MostrarMensagem("Já existe uma turma com esse código.");
                        return;
                    }

                    int idTurma = CriarTurma(idEscola, idAnoLetivo, ano, codigoTurma, ativa, idLE2, emr, plnm, idOfertaEscola);

                    if (idTurma > 0)
                    {
                        Response.Redirect("~/agrupamento/turmas.aspx?id=" + idEscola + "&anoLetivoId=" + idAnoLetivo);
                    }
                    else
                    {
                        MostrarMensagem("Não foi possível criar a turma.");
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarMensagem("Erro: " + ex.Message);
            }
        }

        protected void buttonCancelar_Click(object sender, EventArgs e)
        {
            int idEscola = GetIdEscola();
            Response.Redirect("~/agrupamento/turmas.aspx?id=" + idEscola);
        }

        protected void SelecaoCurricular_Changed(object sender, EventArgs e)
        {
            MostrarPreview();
        }

        private void CarregarAnosLetivos()
        {
            DataTable dt = new DataTable();

            string sql = @"
                SELECT Id, Descricao
                FROM dbo.AnoLetivo
                WHERE Ativo = 1
                ORDER BY Descricao DESC;";

            using (SqlConnection conn = new SqlConnection(ligacao))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                da.Fill(dt);
            }

            ddlAnoLetivo.DataSource = dt;
            ddlAnoLetivo.DataTextField = "Descricao";
            ddlAnoLetivo.DataValueField = "Id";
            ddlAnoLetivo.DataBind();
            ddlAnoLetivo.Items.Insert(0, new ListItem("-- selecionar --", ""));
        }

        private void CarregarLE2()
        {
            // As LE2 disponíveis são Francês, Espanhol e Alemão
            DataTable dt = new DataTable();

            string sql = @"
                SELECT Id, Nome
                FROM dbo.Disciplina
                WHERE Id IN (3, 4, 5)
                ORDER BY Nome;";

            using (SqlConnection conn = new SqlConnection(ligacao))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                da.Fill(dt);
            }

            ddlLE2.DataSource = dt;
            ddlLE2.DataTextField = "Nome";
            ddlLE2.DataValueField = "Id";
            ddlLE2.DataBind();
            ddlLE2.Items.Insert(0, new ListItem("-- selecionar --", ""));
        }

        private int GetIdDisciplinaPLNM()
        {
            string sql = @"
                SELECT TOP 1 Id
                FROM dbo.Disciplina
                WHERE Nome = 'Português Língua Não Materna';";

            using (SqlConnection conn = new SqlConnection(ligacao))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                conn.Open();
                object valor = cmd.ExecuteScalar();

                if (valor == null || valor == DBNull.Value)
                    return 0;

                return Convert.ToInt32(valor);
            }
        }

        private int GetIdDisciplinaOfertaEscola()
        {
            // Na tabela Disciplina continua a existir a disciplina base "Oferta de Escola"
            string sql = @"
                SELECT TOP 1 Id
                FROM dbo.Disciplina
                WHERE Nome = 'Oferta de Escola';";

            using (SqlConnection conn = new SqlConnection(ligacao))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                conn.Open();
                object valor = cmd.ExecuteScalar();

                if (valor == null || valor == DBNull.Value)
                    return 0;

                return Convert.ToInt32(valor);
            }
        }

        private int GetIdAgrupamentoDaEscola(int idEscola)
        {
            string sql = @"
                SELECT AgrupamentoId
                FROM dbo.Escola
                WHERE Id = @Id;";

            using (SqlConnection conn = new SqlConnection(ligacao))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Id", idEscola);

                conn.Open();

                object valor = cmd.ExecuteScalar();

                if (valor == null || valor == DBNull.Value)
                    return 0;

                return Convert.ToInt32(valor);
            }
        }

        private int GetIdOfertaEscolaAgrupamento(int idAgrupamento, int idAnoLetivo)
        {
            string sql = @"
                SELECT TOP 1 Id
                FROM dbo.OfertaEscolaAgrupamento
                WHERE AgrupamentoId = @AgrupamentoId
                  AND AnoLetivoId = @AnoLetivoId
                  AND Ativa = 1
                ORDER BY Id DESC;";

            using (SqlConnection conn = new SqlConnection(ligacao))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@AgrupamentoId", idAgrupamento);
                cmd.Parameters.AddWithValue("@AnoLetivoId", idAnoLetivo);

                conn.Open();

                object valor = cmd.ExecuteScalar();

                if (valor == null || valor == DBNull.Value)
                    return 0;

                return Convert.ToInt32(valor);
            }
        }

        private void MostrarPreview()
        {
            painelDisciplinas.Visible = true;

            if (ddlAnoEscolaridade.SelectedValue == "" || ddlLE2.SelectedValue == "")
            {
                gridDisciplinas.DataSource = null;
                gridDisciplinas.DataBind();
                return;
            }

            int idEscola = GetIdEscola();

            if (idEscola == 0 || ddlAnoLetivo.SelectedValue == "")
            {
                gridDisciplinas.DataSource = null;
                gridDisciplinas.DataBind();
                return;
            }

            int idAgrupamento = GetIdAgrupamentoDaEscola(idEscola);
            int idAnoLetivo = Convert.ToInt32(ddlAnoLetivo.SelectedValue);
            int idOfertaEscola = GetIdOfertaEscolaAgrupamento(idAgrupamento, idAnoLetivo);

            if (idOfertaEscola == 0)
            {
                MostrarMensagem("Antes de criar a turma, defina a Oferta de Escola no ecrã das turmas.");
                gridDisciplinas.DataSource = null;
                gridDisciplinas.DataBind();
                return;
            }

            byte ano = Convert.ToByte(ddlAnoEscolaridade.SelectedValue);
            int idLE2 = Convert.ToInt32(ddlLE2.SelectedValue);
            bool emr = chkEMR.Checked;
            bool plnm = chkPLNM.Checked;
            int idDisciplinaPLNM = GetIdDisciplinaPLNM();
            int idDisciplinaOfertaEscola = GetIdDisciplinaOfertaEscola();

            DataTable dt = new DataTable();

            string sql = @"
                SELECT
                    CASE
                        WHEN pcd.DisciplinaId = @DisciplinaOfertaEscola THEN 'Oferta de Escola'
                        ELSE gd.Nome
                    END AS GrupoDisciplinar,
                    CASE
                        WHEN pcd.DisciplinaId = @DisciplinaOfertaEscola THEN ISNULL(oea.Nome, d.Nome)
                        ELSE d.Nome
                    END AS Disciplina,
                    pcd.Natureza,
                    CASE
                        WHEN pcd.DisciplinaId = @DisciplinaOfertaEscola THEN 'Oferta de Escola'
                        WHEN pcd.BlocoOpcao = 'LE2' THEN 'Língua Estrangeira II'
                        WHEN pcd.Componente = 'EMR' THEN 'EMR'
                        WHEN pcd.DisciplinaId = @PLNMId THEN 'PLNM'
                        ELSE 'Obrigatória'
                    END AS Origem
                FROM dbo.PlanoCurricularDisciplina pcd
                INNER JOIN dbo.Disciplina d ON d.Id = pcd.DisciplinaId
                INNER JOIN dbo.GrupoDisciplinar gd ON gd.Id = d.GrupoDisciplinarId
                LEFT JOIN dbo.OfertaEscolaAgrupamento oea ON oea.Id = @OfertaEscolaId
                WHERE pcd.PlanoCurricularId = @Plano
                  AND pcd.AnoEscolaridade = @Ano
                  AND pcd.Ativa = 1
                  AND
                  (
                        pcd.Natureza IN (N'Obrigatória', N'Obrigatoria')
                        OR (pcd.BlocoOpcao = 'LE2' AND pcd.DisciplinaId = @LE2)
                        OR (pcd.Componente = 'EMR' AND @EMR = 1)
                        OR (pcd.DisciplinaId = @PLNMId AND @PLNM = 1)
                  )
                ORDER BY
                    CASE
                        WHEN pcd.Natureza IN (N'Obrigatória', N'Obrigatoria') THEN 1
                        WHEN pcd.BlocoOpcao = 'LE2' THEN 2
                        WHEN pcd.Componente = 'EMR' THEN 3
                        WHEN pcd.DisciplinaId = @PLNMId THEN 4
                        ELSE 5
                    END,
                    GrupoDisciplinar,
                    Disciplina;";

            using (SqlConnection conn = new SqlConnection(ligacao))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddWithValue("@Plano", idPlano);
                cmd.Parameters.AddWithValue("@Ano", ano);
                cmd.Parameters.AddWithValue("@LE2", idLE2);
                cmd.Parameters.AddWithValue("@EMR", emr);
                cmd.Parameters.AddWithValue("@PLNM", plnm);
                cmd.Parameters.AddWithValue("@PLNMId", idDisciplinaPLNM);
                cmd.Parameters.AddWithValue("@DisciplinaOfertaEscola", idDisciplinaOfertaEscola);
                cmd.Parameters.AddWithValue("@OfertaEscolaId", idOfertaEscola);

                da.Fill(dt);
            }

            gridDisciplinas.DataSource = dt;
            gridDisciplinas.DataBind();
        }

        private int CriarTurma(int idEscola, int idAnoLetivo, byte ano, string codigoTurma, bool ativa, int idLE2, bool emr, bool plnm, int idOfertaEscola)
        {
            int idDisciplinaPLNM = GetIdDisciplinaPLNM();
            int idDisciplinaOfertaEscola = GetIdDisciplinaOfertaEscola();

            using (SqlConnection conn = new SqlConnection(ligacao))
            {
                conn.Open();
                SqlTransaction tr = conn.BeginTransaction();

                try
                {
                    int idTurma = 0;

                    string sqlTurma = @"
                        INSERT INTO dbo.Turma
                        (
                            EscolaId,
                            AnoLetivoId,
                            PlanoCurricularId,
                            AnoEscolaridade,
                            CodigoTurma,
                            Ativa
                        )
                        VALUES
                        (
                            @EscolaId,
                            @AnoLetivoId,
                            @Plano,
                            @Ano,
                            @CodigoTurma,
                            @Ativa
                        );

                        SELECT CAST(SCOPE_IDENTITY() AS INT);";

                    using (SqlCommand cmdTurma = new SqlCommand(sqlTurma, conn, tr))
                    {
                        cmdTurma.Parameters.AddWithValue("@EscolaId", idEscola);
                        cmdTurma.Parameters.AddWithValue("@AnoLetivoId", idAnoLetivo);
                        cmdTurma.Parameters.AddWithValue("@Plano", idPlano);
                        cmdTurma.Parameters.AddWithValue("@Ano", ano);
                        cmdTurma.Parameters.AddWithValue("@CodigoTurma", codigoTurma);
                        cmdTurma.Parameters.AddWithValue("@Ativa", ativa);

                        idTurma = Convert.ToInt32(cmdTurma.ExecuteScalar());
                    }

                    string sqlDisciplinas = @"
                        INSERT INTO dbo.TurmaDisciplina
                        (
                            TurmaId,
                            DisciplinaId,
                            PlanoCurricularDisciplinaId,
                            OfertaEscolaAgrupamentoId
                        )
                        SELECT
                            @TurmaId,
                            pcd.DisciplinaId,
                            pcd.Id,
                            CASE
                                WHEN pcd.DisciplinaId = @DisciplinaOfertaEscola THEN @OfertaEscolaId
                                ELSE NULL
                            END
                        FROM dbo.PlanoCurricularDisciplina pcd
                        WHERE pcd.PlanoCurricularId = @Plano
                          AND pcd.AnoEscolaridade = @Ano
                          AND pcd.Ativa = 1
                          AND
                          (
                                pcd.Natureza IN (N'Obrigatória', N'Obrigatoria')
                                OR (pcd.BlocoOpcao = 'LE2' AND pcd.DisciplinaId = @LE2)
                                OR (pcd.Componente = 'EMR' AND @EMR = 1)
                                OR (pcd.DisciplinaId = @PLNMId AND @PLNM = 1)
                          );";

                    using (SqlCommand cmdDisciplinas = new SqlCommand(sqlDisciplinas, conn, tr))
                    {
                        cmdDisciplinas.Parameters.AddWithValue("@TurmaId", idTurma);
                        cmdDisciplinas.Parameters.AddWithValue("@Plano", idPlano);
                        cmdDisciplinas.Parameters.AddWithValue("@Ano", ano);
                        cmdDisciplinas.Parameters.AddWithValue("@LE2", idLE2);
                        cmdDisciplinas.Parameters.AddWithValue("@EMR", emr);
                        cmdDisciplinas.Parameters.AddWithValue("@PLNM", plnm);
                        cmdDisciplinas.Parameters.AddWithValue("@PLNMId", idDisciplinaPLNM);
                        cmdDisciplinas.Parameters.AddWithValue("@DisciplinaOfertaEscola", idDisciplinaOfertaEscola);
                        cmdDisciplinas.Parameters.AddWithValue("@OfertaEscolaId", idOfertaEscola);

                        cmdDisciplinas.ExecuteNonQuery();
                    }

                    tr.Commit();
                    return idTurma;
                }
                catch
                {
                    tr.Rollback();
                    throw;
                }
            }
        }

        private void AtualizarTurma(int idTurma, int idEscola, int idAnoLetivo, string codigoTurma, bool ativa, byte ano, int idLE2, bool emr, bool plnm, int idOfertaEscola)
        {
            int idDisciplinaPLNM = GetIdDisciplinaPLNM();
            int idDisciplinaOfertaEscola = GetIdDisciplinaOfertaEscola();

            using (SqlConnection conn = new SqlConnection(ligacao))
            {
                conn.Open();
                SqlTransaction tr = conn.BeginTransaction();

                try
                {
                    string sqlUpdateTurma = @"
                        UPDATE dbo.Turma
                        SET
                            AnoLetivoId = @AnoLetivoId,
                            CodigoTurma = @CodigoTurma,
                            Ativa = @Ativa
                        WHERE Id = @Id
                          AND EscolaId = @EscolaId
                          AND PlanoCurricularId = @Plano;";

                    using (SqlCommand cmdUpdateTurma = new SqlCommand(sqlUpdateTurma, conn, tr))
                    {
                        cmdUpdateTurma.Parameters.AddWithValue("@Id", idTurma);
                        cmdUpdateTurma.Parameters.AddWithValue("@EscolaId", idEscola);
                        cmdUpdateTurma.Parameters.AddWithValue("@AnoLetivoId", idAnoLetivo);
                        cmdUpdateTurma.Parameters.AddWithValue("@CodigoTurma", codigoTurma);
                        cmdUpdateTurma.Parameters.AddWithValue("@Ativa", ativa);
                        cmdUpdateTurma.Parameters.AddWithValue("@Plano", idPlano);

                        cmdUpdateTurma.ExecuteNonQuery();
                    }

                    // Apaga opcionais/facultativas e também volta a atualizar o registo da Oferta de Escola,
                    // porque pode mudar consoante o ano letivo
                    string sqlApagar = @"
                        DELETE td
                        FROM dbo.TurmaDisciplina td
                        INNER JOIN dbo.PlanoCurricularDisciplina pcd ON pcd.Id = td.PlanoCurricularDisciplinaId
                        WHERE td.TurmaId = @TurmaId
                          AND
                          (
                                pcd.BlocoOpcao = 'LE2'
                                OR pcd.Componente = 'EMR'
                                OR pcd.DisciplinaId = @PLNMId
                                OR pcd.DisciplinaId = @DisciplinaOfertaEscola
                          );";

                    using (SqlCommand cmdApagar = new SqlCommand(sqlApagar, conn, tr))
                    {
                        cmdApagar.Parameters.AddWithValue("@TurmaId", idTurma);
                        cmdApagar.Parameters.AddWithValue("@PLNMId", idDisciplinaPLNM);
                        cmdApagar.Parameters.AddWithValue("@DisciplinaOfertaEscola", idDisciplinaOfertaEscola);
                        cmdApagar.ExecuteNonQuery();
                    }

                    string sqlInserir = @"
                        INSERT INTO dbo.TurmaDisciplina
                        (
                            TurmaId,
                            DisciplinaId,
                            PlanoCurricularDisciplinaId,
                            OfertaEscolaAgrupamentoId
                        )
                        SELECT
                            @TurmaId,
                            pcd.DisciplinaId,
                            pcd.Id,
                            CASE
                                WHEN pcd.DisciplinaId = @DisciplinaOfertaEscola THEN @OfertaEscolaId
                                ELSE NULL
                            END
                        FROM dbo.PlanoCurricularDisciplina pcd
                        WHERE pcd.PlanoCurricularId = @Plano
                          AND pcd.AnoEscolaridade = @Ano
                          AND pcd.Ativa = 1
                          AND
                          (
                                (pcd.BlocoOpcao = 'LE2' AND pcd.DisciplinaId = @LE2)
                                OR (pcd.Componente = 'EMR' AND @EMR = 1)
                                OR (pcd.DisciplinaId = @PLNMId AND @PLNM = 1)
                                OR (pcd.DisciplinaId = @DisciplinaOfertaEscola)
                          );";

                    using (SqlCommand cmdInserir = new SqlCommand(sqlInserir, conn, tr))
                    {
                        cmdInserir.Parameters.AddWithValue("@TurmaId", idTurma);
                        cmdInserir.Parameters.AddWithValue("@Plano", idPlano);
                        cmdInserir.Parameters.AddWithValue("@Ano", ano);
                        cmdInserir.Parameters.AddWithValue("@LE2", idLE2);
                        cmdInserir.Parameters.AddWithValue("@EMR", emr);
                        cmdInserir.Parameters.AddWithValue("@PLNM", plnm);
                        cmdInserir.Parameters.AddWithValue("@PLNMId", idDisciplinaPLNM);
                        cmdInserir.Parameters.AddWithValue("@DisciplinaOfertaEscola", idDisciplinaOfertaEscola);
                        cmdInserir.Parameters.AddWithValue("@OfertaEscolaId", idOfertaEscola);

                        cmdInserir.ExecuteNonQuery();
                    }

                    tr.Commit();
                }
                catch
                {
                    tr.Rollback();
                    throw;
                }
            }
        }

        private bool TurmaJaExiste(int idEscola, int idAnoLetivo, string codigoTurma)
        {
            string sql = @"
                SELECT COUNT(1)
                FROM dbo.Turma
                WHERE EscolaId = @EscolaId
                  AND AnoLetivoId = @AnoLetivoId
                  AND CodigoTurma = @CodigoTurma
                  AND PlanoCurricularId = @Plano;";

            using (SqlConnection conn = new SqlConnection(ligacao))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@EscolaId", idEscola);
                cmd.Parameters.AddWithValue("@AnoLetivoId", idAnoLetivo);
                cmd.Parameters.AddWithValue("@CodigoTurma", codigoTurma);
                cmd.Parameters.AddWithValue("@Plano", idPlano);

                conn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        private void CarregarTurma(int idTurma)
        {
            int idEscola = GetIdEscola();
            int idDisciplinaPLNM = GetIdDisciplinaPLNM();

            string sql = @"
                SELECT
                    t.AnoLetivoId,
                    t.AnoEscolaridade,
                    t.CodigoTurma,
                    t.Ativa,
                    le2.DisciplinaId AS LE2DisciplinaId,
                    emr.ExisteEMR,
                    plnm.ExistePLNM
                FROM dbo.Turma t
                OUTER APPLY
                (
                    SELECT TOP 1 td.DisciplinaId
                    FROM dbo.TurmaDisciplina td
                    INNER JOIN dbo.PlanoCurricularDisciplina pcd ON pcd.Id = td.PlanoCurricularDisciplinaId
                    WHERE td.TurmaId = t.Id
                      AND pcd.BlocoOpcao = 'LE2'
                ) le2
                OUTER APPLY
                (
                    SELECT CASE WHEN COUNT(1) > 0 THEN 1 ELSE 0 END AS ExisteEMR
                    FROM dbo.TurmaDisciplina td
                    INNER JOIN dbo.PlanoCurricularDisciplina pcd ON pcd.Id = td.PlanoCurricularDisciplinaId
                    WHERE td.TurmaId = t.Id
                      AND pcd.Componente = 'EMR'
                ) emr
                OUTER APPLY
                (
                    SELECT CASE WHEN COUNT(1) > 0 THEN 1 ELSE 0 END AS ExistePLNM
                    FROM dbo.TurmaDisciplina td
                    WHERE td.TurmaId = t.Id
                      AND td.DisciplinaId = @DisciplinaPLNM
                ) plnm
                WHERE t.Id = @Id
                  AND t.EscolaId = @EscolaId
                  AND t.PlanoCurricularId = @Plano;";

            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(ligacao))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddWithValue("@Id", idTurma);
                cmd.Parameters.AddWithValue("@EscolaId", idEscola);
                cmd.Parameters.AddWithValue("@Plano", idPlano);
                cmd.Parameters.AddWithValue("@DisciplinaPLNM", idDisciplinaPLNM);
                da.Fill(dt);
            }

            if (dt.Rows.Count == 0)
                return;

            DataRow dr = dt.Rows[0];

            ddlAnoLetivo.SelectedValue = dr["AnoLetivoId"].ToString();
            ddlAnoEscolaridade.SelectedValue = dr["AnoEscolaridade"].ToString();
            txtCodigoTurma.Text = dr["CodigoTurma"].ToString();
            chkAtiva.Checked = Convert.ToBoolean(dr["Ativa"]);

            if (dr["LE2DisciplinaId"] != DBNull.Value)
            {
                string valor = dr["LE2DisciplinaId"].ToString();
                if (ddlLE2.Items.FindByValue(valor) != null)
                    ddlLE2.SelectedValue = valor;
            }

            chkEMR.Checked = dr["ExisteEMR"] != DBNull.Value && Convert.ToInt32(dr["ExisteEMR"]) == 1;
            chkPLNM.Checked = dr["ExistePLNM"] != DBNull.Value && Convert.ToInt32(dr["ExistePLNM"]) == 1;
        }

        private int GetIdEscola()
        {
            int idEscola = 0;

            if (Request.QueryString["id"] != null)
                int.TryParse(Request.QueryString["id"], out idEscola);
            else if (Session["EscolaIdTurmaBasico"] != null)
                int.TryParse(Session["EscolaIdTurmaBasico"].ToString(), out idEscola);

            return idEscola;
        }

        private int GetIdTurma()
        {
            int idTurma = 0;

            if (Request.QueryString["idTurma"] != null)
                int.TryParse(Request.QueryString["idTurma"], out idTurma);

            return idTurma;
        }

        private string GetModo()
        {
            if (Request.QueryString["modo"] != null)
                return Request.QueryString["modo"].ToLower();

            return "criar";
        }

        private void CarregarNomeEscola(int idEscola)
        {
            string sql = "SELECT Nome FROM dbo.Escola WHERE Id = @Id;";

            using (SqlConnection conn = new SqlConnection(ligacao))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Id", idEscola);
                conn.Open();

                object nome = cmd.ExecuteScalar();
                lblEscola.Text = "Escola: " + (nome == null ? "" : nome.ToString());
            }
        }

        private void MostrarMensagem(string mensagem, bool erro = true)
        {
            lblMensagem.Visible = true;
            lblMensagem.Text = mensagem;
            lblMensagem.CssClass = erro ? "alert alert-warning d-block" : "alert alert-success d-block";
        }
    }
}