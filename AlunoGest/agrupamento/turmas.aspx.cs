using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AlunoGest.agrupamento
{
    public partial class turmas : System.Web.UI.Page
    {
        // String de ligação à base de dados.
        private string ligacao = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Só carregamos os dados iniciais na primeira vez.
            if (!IsPostBack)
            {
                lblMensagem.Visible = false;
                painelEditarOfertaEscola.Visible = false;
                painelResumoOfertaEscola.Visible = false;

                int idEscola = GetIdEscola();

                if (idEscola == 0)
                {
                    MostrarMensagem("Não foi indicada nenhuma escola.");
                    return;
                }

                // Guardamos o id da escola em sessão.
                Session["EscolaIdTurmas"] = idEscola;

                CarregarNomeEscola(idEscola);
                CarregarAnosLetivos();
                CarregarGruposDisciplinaresOfertaEscola();

                // Se vier anoLetivoId na query string, tentamos selecioná-lo.
                string anoLetivoQuery = Request.QueryString["anoLetivoId"];
                if (!string.IsNullOrEmpty(anoLetivoQuery))
                {
                    if (ddlAnoLetivo.Items.FindByValue(anoLetivoQuery) != null)
                        ddlAnoLetivo.SelectedValue = anoLetivoQuery;
                }

                CarregarResumoOfertaEscola();
                CarregarTurmas();
            }
        }

        protected void ddlAnoLetivo_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblMensagem.Visible = false;

            // Quando muda o ano letivo:
            // 1) fechamos o painel de edição
            // 2) atualizamos o resumo da oferta
            // 3) atualizamos a lista de turmas
            painelEditarOfertaEscola.Visible = false;

            CarregarResumoOfertaEscola();
            CarregarTurmas();
        }

        protected void buttonDefinirOfertaEscola_Click(object sender, EventArgs e)
        {
            lblMensagem.Visible = false;

            int idEscola = GetIdEscola();

            if (idEscola == 0)
            {
                MostrarMensagem("Não foi indicada nenhuma escola.");
                return;
            }

            if (ddlAnoLetivo.SelectedValue == "")
            {
                MostrarMensagem("Seleciona o ano letivo.");
                return;
            }

            // Mostra o painel e carrega o que já existir.
            painelEditarOfertaEscola.Visible = true;
            CarregarOfertaEscolaNoFormulario();
        }

        protected void buttonCancelarOfertaEscola_Click(object sender, EventArgs e)
        {
            // Apenas fecha o painel.
            painelEditarOfertaEscola.Visible = false;
            lblMensagem.Visible = false;
        }

        protected void buttonGuardarOfertaEscola_Click(object sender, EventArgs e)
        {
            lblMensagem.Visible = false;

            int idEscola = GetIdEscola();

            if (idEscola == 0)
            {
                MostrarMensagem("Não foi indicada nenhuma escola.");
                return;
            }

            if (ddlAnoLetivo.SelectedValue == "")
            {
                MostrarMensagem("Seleciona o ano letivo.");
                return;
            }

            if (txtNomeOfertaEscola.Text.Trim() == "")
            {
                MostrarMensagem("Indica o nome da disciplina de Oferta de Escola.");
                return;
            }

            if (!TemGrupoSelecionado())
            {
                MostrarMensagem("Seleciona pelo menos um grupo disciplinar.");
                return;
            }

            int idAgrupamento = GetIdAgrupamentoDaEscola(idEscola);

            if (idAgrupamento == 0)
            {
                MostrarMensagem("Não foi possível determinar o agrupamento da escola.");
                return;
            }

            int idAnoLetivo = Convert.ToInt32(ddlAnoLetivo.SelectedValue);
            string nomeOferta = txtNomeOfertaEscola.Text.Trim();

            using (SqlConnection conn = new SqlConnection(ligacao))
            {
                conn.Open();
                SqlTransaction tr = conn.BeginTransaction();

                try
                {
                    int idOferta = ObterOuCriarOfertaEscola(conn, tr, idAgrupamento, idAnoLetivo, nomeOferta);

                    // Apaga os grupos antigos.
                    string sqlDeleteGrupos = @"
                        DELETE FROM dbo.OfertaEscolaAgrupamentoGrupoDisciplinar
                        WHERE OfertaEscolaAgrupamentoId = @OfertaId;";

                    using (SqlCommand cmd = new SqlCommand(sqlDeleteGrupos, conn, tr))
                    {
                        cmd.Parameters.AddWithValue("@OfertaId", idOferta);
                        cmd.ExecuteNonQuery();
                    }

                    // Insere os grupos atualmente selecionados.
                    for (int i = 0; i < cblGruposOfertaEscola.Items.Count; i++)
                    {
                        if (cblGruposOfertaEscola.Items[i].Selected)
                        {
                            string sqlInsertGrupo = @"
                                INSERT INTO dbo.OfertaEscolaAgrupamentoGrupoDisciplinar
                                (
                                    OfertaEscolaAgrupamentoId,
                                    GrupoDisciplinarId,
                                    Ativo
                                )
                                VALUES
                                (
                                    @OfertaId,
                                    @GrupoDisciplinarId,
                                    1
                                );";

                            using (SqlCommand cmd = new SqlCommand(sqlInsertGrupo, conn, tr))
                            {
                                cmd.Parameters.AddWithValue("@OfertaId", idOferta);
                                cmd.Parameters.AddWithValue("@GrupoDisciplinarId", Convert.ToInt32(cblGruposOfertaEscola.Items[i].Value));
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }

                    tr.Commit();

                    // Fecha o painel e atualiza o resumo.
                    painelEditarOfertaEscola.Visible = false;
                    CarregarResumoOfertaEscola();

                    MostrarMensagem("Oferta de Escola guardada com sucesso.", false);
                }
                catch (Exception ex)
                {
                    tr.Rollback();
                    MostrarMensagem("Erro ao guardar a Oferta de Escola: " + ex.Message);
                }
            }
        }

        protected void buttonCriarTurma_Click(object sender, EventArgs e)
        {
            int idEscola = GetIdEscola();

            if (idEscola == 0)
            {
                MostrarMensagem("Não foi indicada nenhuma escola.");
                return;
            }

            if (ddlAnoLetivo.SelectedValue == "")
            {
                MostrarMensagem("Seleciona o ano letivo.");
                return;
            }

            if (ddlTipoTurma.SelectedValue == "")
            {
                MostrarMensagem("Seleciona o tipo de turma.");
                return;
            }

            string anoLetivoId = ddlAnoLetivo.SelectedValue;

            // Redireciona para o formulário adequado.
            if (ddlTipoTurma.SelectedValue == "basico")
            {
                Response.Redirect("~/agrupamento/turma_basico.aspx?id=" + idEscola
                    + "&modo=criar&anoLetivoId=" + anoLetivoId);
            }
            else
            {
                Response.Redirect("~/agrupamento/turma_secundario.aspx?id=" + idEscola
                    + "&modo=criar&anoLetivoId=" + anoLetivoId);
            }
        }
        protected void gridTurmas_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int idEscola = GetIdEscola();

            if (e.CommandName == "editarTurma")
            {
                int idTurma = Convert.ToInt32(e.CommandArgument);
                string tipoTurma = GetTipoTurma(idTurma);

                if (tipoTurma == "basico")
                {
                    Response.Redirect("~/agrupamento/turma_basico.aspx?id=" + idEscola
                        + "&modo=editar&idTurma=" + idTurma);
                }
                else
                {
                    Response.Redirect("~/agrupamento/turma_secundario.aspx?id=" + idEscola
                        + "&modo=editar&idTurma=" + idTurma);
                }
            }

            if (e.CommandName == "professoresTurma")
            {
                int idTurma = Convert.ToInt32(e.CommandArgument);
                Response.Redirect("~/agrupamento/turma_professores.aspx?idTurma=" + idTurma);
            }

            // ← Adiciona aqui
            if (e.CommandName == "alunosTurma")
            {
                int idTurma = Convert.ToInt32(e.CommandArgument);
                Response.Redirect("~/agrupamento/TurmaAluno.aspx?idTurma=" + idTurma);
            }
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

        private void CarregarGruposDisciplinaresOfertaEscola()
        {
            DataTable dt = new DataTable();

            string sql = @"
                SELECT Id, Nome
                FROM dbo.GrupoDisciplinar
                WHERE Ativo = 1
                ORDER BY Nome;";

            using (SqlConnection conn = new SqlConnection(ligacao))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                da.Fill(dt);
            }

            cblGruposOfertaEscola.DataSource = dt;
            cblGruposOfertaEscola.DataTextField = "Nome";
            cblGruposOfertaEscola.DataValueField = "Id";
            cblGruposOfertaEscola.DataBind();
        }

        private void CarregarOfertaEscolaNoFormulario()
        {
            // Limpa primeiro.
            LimparFormularioOfertaEscola();

            int idEscola = GetIdEscola();

            if (idEscola == 0)
                return;

            if (ddlAnoLetivo.SelectedValue == "")
                return;

            int idAgrupamento = GetIdAgrupamentoDaEscola(idEscola);

            if (idAgrupamento == 0)
                return;

            int idAnoLetivo = Convert.ToInt32(ddlAnoLetivo.SelectedValue);

            string sqlOferta = @"
                SELECT TOP 1 Id, Nome
                FROM dbo.OfertaEscolaAgrupamento
                WHERE AgrupamentoId = @AgrupamentoId
                  AND AnoLetivoId = @AnoLetivoId
                  AND Ativa = 1
                ORDER BY Id DESC;";

            int idOferta = 0;

            using (SqlConnection conn = new SqlConnection(ligacao))
            using (SqlCommand cmd = new SqlCommand(sqlOferta, conn))
            {
                cmd.Parameters.AddWithValue("@AgrupamentoId", idAgrupamento);
                cmd.Parameters.AddWithValue("@AnoLetivoId", idAnoLetivo);

                conn.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        idOferta = Convert.ToInt32(dr["Id"]);
                        txtNomeOfertaEscola.Text = dr["Nome"].ToString();
                    }
                }
            }

            if (idOferta == 0)
                return;

            string sqlGrupos = @"
                SELECT GrupoDisciplinarId
                FROM dbo.OfertaEscolaAgrupamentoGrupoDisciplinar
                WHERE OfertaEscolaAgrupamentoId = @OfertaId
                  AND Ativo = 1;";

            using (SqlConnection conn = new SqlConnection(ligacao))
            using (SqlCommand cmd = new SqlCommand(sqlGrupos, conn))
            {
                cmd.Parameters.AddWithValue("@OfertaId", idOferta);

                conn.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        string idGrupo = dr["GrupoDisciplinarId"].ToString();

                        if (cblGruposOfertaEscola.Items.FindByValue(idGrupo) != null)
                            cblGruposOfertaEscola.Items.FindByValue(idGrupo).Selected = true;
                    }
                }
            }
        }

        private void CarregarResumoOfertaEscola()
        {
            lblOfertaEscolaAtual.Text = "";
            lblGruposOfertaEscolaAtual.Text = "";
            painelResumoOfertaEscola.Visible = false;

            int idEscola = GetIdEscola();

            if (idEscola == 0)
                return;

            if (ddlAnoLetivo.SelectedValue == "")
                return;

            int idAgrupamento = GetIdAgrupamentoDaEscola(idEscola);

            if (idAgrupamento == 0)
                return;

            int idAnoLetivo = Convert.ToInt32(ddlAnoLetivo.SelectedValue);

            string sql = @"
                SELECT TOP 1 Id, Nome
                FROM dbo.OfertaEscolaAgrupamento
                WHERE AgrupamentoId = @AgrupamentoId
                  AND AnoLetivoId = @AnoLetivoId
                  AND Ativa = 1
                ORDER BY Id DESC;";

            int idOferta = 0;

            using (SqlConnection conn = new SqlConnection(ligacao))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@AgrupamentoId", idAgrupamento);
                cmd.Parameters.AddWithValue("@AnoLetivoId", idAnoLetivo);

                conn.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        idOferta = Convert.ToInt32(dr["Id"]);
                        lblOfertaEscolaAtual.Text = dr["Nome"].ToString();
                    }
                }
            }

            if (idOferta == 0)
                return;

            lblGruposOfertaEscolaAtual.Text = ObterTextoGruposOfertaEscola(idOferta);
            painelResumoOfertaEscola.Visible = true;
        }

        private string ObterTextoGruposOfertaEscola(int idOferta)
        {
            StringBuilder sb = new StringBuilder();

            string sql = @"
                SELECT gd.Nome
                FROM dbo.OfertaEscolaAgrupamentoGrupoDisciplinar oegd
                INNER JOIN dbo.GrupoDisciplinar gd ON gd.Id = oegd.GrupoDisciplinarId
                WHERE oegd.OfertaEscolaAgrupamentoId = @OfertaId
                  AND oegd.Ativo = 1
                ORDER BY gd.Nome;";

            using (SqlConnection conn = new SqlConnection(ligacao))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@OfertaId", idOferta);
                conn.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        if (sb.Length > 0)
                            sb.Append(", ");

                        sb.Append(dr["Nome"].ToString());
                    }
                }
            }

            return sb.ToString();
        }

        private void LimparFormularioOfertaEscola()
        {
            txtNomeOfertaEscola.Text = "";

            for (int i = 0; i < cblGruposOfertaEscola.Items.Count; i++)
                cblGruposOfertaEscola.Items[i].Selected = false;
        }

        private int ObterOuCriarOfertaEscola(SqlConnection conn, SqlTransaction tr, int idAgrupamento, int idAnoLetivo, string nomeOferta)
        {
            // Primeiro verifica se já existe.
            string sqlSelect = @"
                SELECT TOP 1 Id
                FROM dbo.OfertaEscolaAgrupamento
                WHERE AgrupamentoId = @AgrupamentoId
                  AND AnoLetivoId = @AnoLetivoId;";

            using (SqlCommand cmd = new SqlCommand(sqlSelect, conn, tr))
            {
                cmd.Parameters.AddWithValue("@AgrupamentoId", idAgrupamento);
                cmd.Parameters.AddWithValue("@AnoLetivoId", idAnoLetivo);

                object valor = cmd.ExecuteScalar();

                // Se já existe, atualiza.
                if (valor != null && valor != DBNull.Value)
                {
                    int idOfertaExistente = Convert.ToInt32(valor);

                    string sqlUpdate = @"
                        UPDATE dbo.OfertaEscolaAgrupamento
                        SET Nome = @Nome,
                            Ativa = 1
                        WHERE Id = @Id;";

                    using (SqlCommand cmdUpdate = new SqlCommand(sqlUpdate, conn, tr))
                    {
                        cmdUpdate.Parameters.AddWithValue("@Nome", nomeOferta);
                        cmdUpdate.Parameters.AddWithValue("@Id", idOfertaExistente);
                        cmdUpdate.ExecuteNonQuery();
                    }

                    return idOfertaExistente;
                }
            }

            // Se não existe, insere.
            string sqlInsert = @"
                INSERT INTO dbo.OfertaEscolaAgrupamento
                (
                    AgrupamentoId,
                    AnoLetivoId,
                    Nome,
                    Ativa
                )
                VALUES
                (
                    @AgrupamentoId,
                    @AnoLetivoId,
                    @Nome,
                    1
                );

                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using (SqlCommand cmd = new SqlCommand(sqlInsert, conn, tr))
            {
                cmd.Parameters.AddWithValue("@AgrupamentoId", idAgrupamento);
                cmd.Parameters.AddWithValue("@AnoLetivoId", idAnoLetivo);
                cmd.Parameters.AddWithValue("@Nome", nomeOferta);

                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        private bool TemGrupoSelecionado()
        {
            for (int i = 0; i < cblGruposOfertaEscola.Items.Count; i++)
            {
                if (cblGruposOfertaEscola.Items[i].Selected)
                    return true;
            }

            return false;
        }

        private void CarregarTurmas()
        {
            int idEscola = GetIdEscola();

            if (idEscola == 0)
            {
                gridTurmas.DataSource = null;
                gridTurmas.DataBind();
                return;
            }

            DataTable dt = new DataTable();

            string sql = @"
                SELECT
                    t.Id,
                    t.CodigoTurma,
                    CAST(t.AnoEscolaridade AS VARCHAR(2)) + '.º' AS AnoEscolaridade,
                    al.Descricao AS AnoLetivo,
                    pc.Nome AS PlanoCurricular,
                    t.Ativa
                FROM dbo.Turma t
                INNER JOIN dbo.AnoLetivo al ON al.Id = t.AnoLetivoId
                INNER JOIN dbo.PlanoCurricular pc ON pc.Id = t.PlanoCurricularId
                WHERE t.EscolaId = @EscolaId
                  AND (@AnoLetivoId = 0 OR t.AnoLetivoId = @AnoLetivoId)
                ORDER BY
                    al.Descricao DESC,
                    t.AnoEscolaridade ASC,
                    t.CodigoTurma ASC;";

            int idAnoLetivo = 0;

            if (ddlAnoLetivo.SelectedValue != "")
                idAnoLetivo = Convert.ToInt32(ddlAnoLetivo.SelectedValue);

            using (SqlConnection conn = new SqlConnection(ligacao))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddWithValue("@EscolaId", idEscola);
                cmd.Parameters.AddWithValue("@AnoLetivoId", idAnoLetivo);

                da.Fill(dt);
            }

            gridTurmas.DataSource = dt;
            gridTurmas.DataBind();
        }

        private string GetTipoTurma(int idTurma)
        {
            // Plano 1 = básico; restantes = secundário.
            string sql = @"
                SELECT PlanoCurricularId
                FROM dbo.Turma
                WHERE Id = @Id;";

            using (SqlConnection conn = new SqlConnection(ligacao))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Id", idTurma);
                conn.Open();

                object valor = cmd.ExecuteScalar();

                if (valor == null || valor == DBNull.Value)
                    return "";

                int idPlano = Convert.ToInt32(valor);

                if (idPlano == 1)
                    return "basico";

                return "secundario";
            }
        }

        private int GetIdEscola()
        {
            int idEscola = 0;

            if (Request.QueryString["id"] != null)
                int.TryParse(Request.QueryString["id"], out idEscola);
            else if (Session["EscolaIdTurmas"] != null)
                int.TryParse(Session["EscolaIdTurmas"].ToString(), out idEscola);

            return idEscola;
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

        private void CarregarNomeEscola(int idEscola)
        {
            string sql = "SELECT Nome FROM dbo.Escola WHERE Id = @Id;";

            using (SqlConnection conn = new SqlConnection(ligacao))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Id", idEscola);
                conn.Open();

                object nome = cmd.ExecuteScalar();

                if (nome == null || nome == DBNull.Value)
                    lblEscola.Text = "Escola: ";
                else
                    lblEscola.Text = "Escola: " + nome.ToString();
            }
        }

        private void MostrarMensagem(string mensagem, bool erro = true)
        {
            lblMensagem.Visible = true;
            lblMensagem.Text = mensagem;

            if (erro)
                lblMensagem.CssClass = "alert alert-warning d-block";
            else
                lblMensagem.CssClass = "alert alert-success d-block";
        }
    }
}