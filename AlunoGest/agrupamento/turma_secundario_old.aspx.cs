using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AlunoGest.agrupamento
{
    public partial class Turma_secundario_old : System.Web.UI.Page
    {
        // Ligação à base de dados
        string ligacao = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Só executa este bloco na primeira vez que a página abre
            if (!IsPostBack)
            {
                lblMensagem.Visible = false;
                painelDisciplinas.Visible = false;

                int idEscola = GetIdEscola();

                // Guarda o id da escola em sessão
                Session["EscolaIdTurmaSecundario"] = idEscola;

                // Carrega os dados principais
                CarregarNomeEscola(idEscola);
                CarregarAnosLetivos();
                CarregarCursos();

                // O ano letivo vem da página anterior
                PreencherAnoLetivoVindoDaPaginaAnterior();

                // Vai buscar o modo: criar ou editar
                string modo = GetModo();

                if (modo == "editar")
                {
                    lblModo.Text = "";

                    int idTurma = GetIdTurma();

                    if (idTurma > 0)
                    {
                        // Carrega os dados principais da turma
                        CarregarTurma(idTurma);

                        // Mostra os blocos de opções
                        MostrarOpcoes();

                        // Marca as opções já guardadas
                        CarregarOpcoesDaTurma(idTurma);

                        // Mostra a preview
                        MostrarPreview();

                        // Em edição não deixamos mudar curso, ano e ano letivo
                        ddlPlano.Enabled = false;
                        ddlAno.Enabled = false;
                        ddlAnoLetivo.Enabled = false;
                    }
                    else
                    {
                        MostrarMensagem("Não foi indicada nenhuma turma para editar.");
                    }
                }
                else
                {
                    lblModo.Text = "";
                    chkAtiva.Checked = true;
                }
            }
        }

        protected void buttonGuardar_Click(object sender, EventArgs e)
        {
            lblMensagem.Visible = false;

            // Validações simples
            if (ddlAnoLetivo.SelectedValue == "")
            {
                MostrarMensagem("Não foi indicado o ano letivo.");
                return;
            }

            if (ddlPlano.SelectedValue == "")
            {
                MostrarMensagem("Selecione o curso.");
                return;
            }

            if (ddlAno.SelectedValue == "")
            {
                MostrarMensagem("Selecione o ano.");
                return;
            }

            if (txtCodigoTurma.Text.Trim() == "")
            {
                MostrarMensagem("Indique o código da turma.");
                return;
            }

            int idEscola = GetIdEscola();
            int idAnoLetivo = Convert.ToInt32(ddlAnoLetivo.SelectedValue);
            int idPlano = Convert.ToInt32(ddlPlano.SelectedValue);
            byte ano = Convert.ToByte(ddlAno.SelectedValue);
            string codigoTurma = txtCodigoTurma.Text.Trim();
            bool ativa = chkAtiva.Checked;

            string modo = GetModo();

            try
            {
                // Valida as escolhas curriculares
                ValidarSelecoesCurriculares(ano);

                // Vai buscar a lista dos ids do plano curricular
                List<int> listaPcd = ObterListaPcd(idPlano, ano);

                if (listaPcd.Count == 0)
                {
                    MostrarMensagem("Não foi possível obter as disciplinas da turma.");
                    return;
                }

                if (modo == "editar")
                {
                    int idTurma = GetIdTurma();

                    if (idTurma == 0)
                    {
                        MostrarMensagem("Não foi indicada nenhuma turma para editar.");
                        return;
                    }

                    if (TurmaJaExisteNoutra(idTurma, idEscola, idAnoLetivo, codigoTurma, idPlano))
                    {
                        MostrarMensagem("Já existe outra turma com esse código.");
                        return;
                    }

                    AtualizarTurma(idTurma, idEscola, idAnoLetivo, codigoTurma, ativa, listaPcd);

                    Response.Redirect("~/agrupamento/turmas.aspx?id=" + idEscola + "&anoLetivoId=" + idAnoLetivo);
                }
                else
                {
                    if (TurmaJaExiste(idEscola, idAnoLetivo, codigoTurma, idPlano))
                    {
                        MostrarMensagem("Já existe uma turma com esse código.");
                        return;
                    }

                    CriarTurma(idEscola, idAnoLetivo, idPlano, ano, codigoTurma, ativa, listaPcd);

                    Response.Redirect("~/agrupamento/turmas.aspx?id=" + idEscola + "&anoLetivoId=" + idAnoLetivo);
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
            string anoLetivoId = ddlAnoLetivo.SelectedValue;

            Response.Redirect("~/agrupamento/turmas.aspx?id=" + idEscola + "&anoLetivoId=" + anoLetivoId);
        }

        protected void ddlPlano_SelectedIndexChanged(object sender, EventArgs e)
        {
            MostrarOpcoes();
            MostrarPreview();
        }

        protected void ddlAno_SelectedIndexChanged(object sender, EventArgs e)
        {
            MostrarOpcoes();
            MostrarPreview();
        }

        protected void Opcoes_Changed(object sender, EventArgs e)
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

        private void PreencherAnoLetivoVindoDaPaginaAnterior()
        {
            // Vai buscar o ano letivo que veio do turmas.aspx
            if (Request.QueryString["anoLetivoId"] != null)
            {
                string valor = Request.QueryString["anoLetivoId"].ToString();

                if (ddlAnoLetivo.Items.FindByValue(valor) != null)
                {
                    ddlAnoLetivo.SelectedValue = valor;
                }
            }

            // O ano letivo fica bloqueado nesta página
            ddlAnoLetivo.Enabled = false;
        }

        private void CarregarCursos()
        {
            DataTable dt = new DataTable();

            string sql = @"
                SELECT pc.Id, pc.Nome
                FROM dbo.PlanoCurricular pc
                INNER JOIN dbo.OfertaFormativa ofo
                    ON ofo.Id = pc.OfertaFormativaId
                WHERE pc.Ativo = 1
                  AND ofo.NivelEnsino = 'Secundario'
                ORDER BY pc.Nome;";

            using (SqlConnection conn = new SqlConnection(ligacao))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                da.Fill(dt);
            }

            ddlPlano.DataSource = dt;
            ddlPlano.DataTextField = "Nome";
            ddlPlano.DataValueField = "Id";
            ddlPlano.DataBind();

            ddlPlano.Items.Insert(0, new ListItem("-- selecionar --", ""));
        }

        private void MostrarOpcoes()
        {
            // Limpa os controlos antes de voltar a carregar
            LimparOpcoes();

            if (ddlPlano.SelectedValue == "" || ddlAno.SelectedValue == "")
                return;

            int idPlano = Convert.ToInt32(ddlPlano.SelectedValue);
            byte ano = Convert.ToByte(ddlAno.SelectedValue);

            // Língua estrangeira
            CarregarDropDownBloco(ddlLE, idPlano, ano, "LE", out bool temLE);
            painelLE.Visible = temLE;

            // Bloco B
            CarregarCheckBoxListBloco(cblB, idPlano, ano, "B", out bool temB);
            painelB.Visible = temB;

            // Bloco C
            CarregarCheckBoxListBloco(cblC, idPlano, ano, "C", out bool temC);
            painelC.Visible = temC;

            // Bloco D
            CarregarCheckBoxListBloco(cblD, idPlano, ano, "D", out bool temD);
            painelD.Visible = temD;
        }

        private void MostrarPreview()
        {
            painelDisciplinas.Visible = true;

            if (ddlPlano.SelectedValue == "" || ddlAno.SelectedValue == "")
            {
                gridDisciplinas.DataSource = null;
                gridDisciplinas.DataBind();
                return;
            }

            int idPlano = Convert.ToInt32(ddlPlano.SelectedValue);
            byte ano = Convert.ToByte(ddlAno.SelectedValue);

            DataTable dt = CriarTabelaPreview();

            // Junta primeiro as obrigatórias
            DataTable dtObrigatorias = GetObrigatoriasPreview(idPlano, ano);
            dt.Merge(dtObrigatorias);

            // Junta depois as opções escolhidas
            DataTable dtOpcoes = GetOpcoesPreview(idPlano, ano);
            dt.Merge(dtOpcoes);

            // Ordena antes de mostrar
            DataView dv = dt.DefaultView;
            dv.Sort = "Ordem ASC, GrupoDisciplinar ASC, Disciplina ASC";

            gridDisciplinas.DataSource = dv;
            gridDisciplinas.DataBind();
        }

        private DataTable CriarTabelaPreview()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("GrupoDisciplinar", typeof(string));
            dt.Columns.Add("Disciplina", typeof(string));
            dt.Columns.Add("Natureza", typeof(string));
            dt.Columns.Add("Origem", typeof(string));
            dt.Columns.Add("Ordem", typeof(int));

            return dt;
        }

        private DataTable GetObrigatoriasPreview(int idPlano, byte ano)
        {
            DataTable dt = new DataTable();

            string sql = @"
                SELECT
                    gd.Nome AS GrupoDisciplinar,
                    d.Nome AS Disciplina,
                    pcd.Natureza,
                    'Obrigatória' AS Origem,
                    1 AS Ordem
                FROM dbo.PlanoCurricularDisciplina pcd
                INNER JOIN dbo.Disciplina d ON d.Id = pcd.DisciplinaId
                INNER JOIN dbo.GrupoDisciplinar gd ON gd.Id = d.GrupoDisciplinarId
                WHERE pcd.PlanoCurricularId = @Plano
                  AND pcd.AnoEscolaridade = @Ano
                  AND pcd.Ativa = 1
                  AND (pcd.Natureza = N'Obrigatória' OR pcd.Natureza = N'Obrigatoria');";

            using (SqlConnection conn = new SqlConnection(ligacao))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddWithValue("@Plano", idPlano);
                cmd.Parameters.AddWithValue("@Ano", ano);
                da.Fill(dt);
            }

            return dt;
        }

        private DataTable GetOpcoesPreview(int idPlano, byte ano)
        {
            DataTable dt = CriarTabelaPreview();

            // Língua estrangeira
            if (painelLE.Visible && ddlLE.SelectedValue != "")
            {
                DataTable temp = GetDisciplinaPorPcdId(Convert.ToInt32(ddlLE.SelectedValue), "Língua estrangeira", 2);
                dt.Merge(temp);
            }

            // Bloco B
            if (painelB.Visible)
            {
                for (int i = 0; i < cblB.Items.Count; i++)
                {
                    if (cblB.Items[i].Selected)
                    {
                        DataTable temp = GetDisciplinaPorPcdId(Convert.ToInt32(cblB.Items[i].Value), "Opção B", 3);
                        dt.Merge(temp);
                    }
                }
            }

            // Bloco C
            if (painelC.Visible)
            {
                for (int i = 0; i < cblC.Items.Count; i++)
                {
                    if (cblC.Items[i].Selected)
                    {
                        DataTable temp = GetDisciplinaPorPcdId(Convert.ToInt32(cblC.Items[i].Value), "Opção C", 4);
                        dt.Merge(temp);
                    }
                }
            }

            // Bloco D
            if (painelD.Visible)
            {
                for (int i = 0; i < cblD.Items.Count; i++)
                {
                    if (cblD.Items[i].Selected)
                    {
                        DataTable temp = GetDisciplinaPorPcdId(Convert.ToInt32(cblD.Items[i].Value), "Opção D", 5);
                        dt.Merge(temp);
                    }
                }
            }

            // EMR
            if (chkEMR.Checked)
            {
                DataTable temp = GetDisciplinaPorComponente(idPlano, ano, "EMR", "EMR", 6);
                dt.Merge(temp);
            }

            // PLNM
            if (chkPLNM.Checked)
            {
                DataTable temp = GetDisciplinaPorNome(idPlano, ano, "Português Língua Não Materna", "PLNM", 7);
                dt.Merge(temp);
            }

            return dt;
        }

        private DataTable GetDisciplinaPorPcdId(int idPcd, string origem, int ordem)
        {
            DataTable dt = new DataTable();

            string sql = @"
                SELECT
                    gd.Nome AS GrupoDisciplinar,
                    d.Nome AS Disciplina,
                    pcd.Natureza,
                    @Origem AS Origem,
                    @Ordem AS Ordem
                FROM dbo.PlanoCurricularDisciplina pcd
                INNER JOIN dbo.Disciplina d ON d.Id = pcd.DisciplinaId
                INNER JOIN dbo.GrupoDisciplinar gd ON gd.Id = d.GrupoDisciplinarId
                WHERE pcd.Id = @IdPcd;";

            using (SqlConnection conn = new SqlConnection(ligacao))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddWithValue("@IdPcd", idPcd);
                cmd.Parameters.AddWithValue("@Origem", origem);
                cmd.Parameters.AddWithValue("@Ordem", ordem);
                da.Fill(dt);
            }

            return dt;
        }

        private DataTable GetDisciplinaPorComponente(int idPlano, byte ano, string componente, string origem, int ordem)
        {
            DataTable dt = new DataTable();

            string sql = @"
                SELECT
                    gd.Nome AS GrupoDisciplinar,
                    d.Nome AS Disciplina,
                    pcd.Natureza,
                    @Origem AS Origem,
                    @Ordem AS Ordem
                FROM dbo.PlanoCurricularDisciplina pcd
                INNER JOIN dbo.Disciplina d ON d.Id = pcd.DisciplinaId
                INNER JOIN dbo.GrupoDisciplinar gd ON gd.Id = d.GrupoDisciplinarId
                WHERE pcd.PlanoCurricularId = @Plano
                  AND pcd.AnoEscolaridade = @Ano
                  AND pcd.Ativa = 1
                  AND pcd.Componente = @Componente;";

            using (SqlConnection conn = new SqlConnection(ligacao))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddWithValue("@Plano", idPlano);
                cmd.Parameters.AddWithValue("@Ano", ano);
                cmd.Parameters.AddWithValue("@Componente", componente);
                cmd.Parameters.AddWithValue("@Origem", origem);
                cmd.Parameters.AddWithValue("@Ordem", ordem);
                da.Fill(dt);
            }

            return dt;
        }

        private DataTable GetDisciplinaPorNome(int idPlano, byte ano, string nomeDisciplina, string origem, int ordem)
        {
            DataTable dt = new DataTable();

            string sql = @"
                SELECT
                    gd.Nome AS GrupoDisciplinar,
                    d.Nome AS Disciplina,
                    pcd.Natureza,
                    @Origem AS Origem,
                    @Ordem AS Ordem
                FROM dbo.PlanoCurricularDisciplina pcd
                INNER JOIN dbo.Disciplina d ON d.Id = pcd.DisciplinaId
                INNER JOIN dbo.GrupoDisciplinar gd ON gd.Id = d.GrupoDisciplinarId
                WHERE pcd.PlanoCurricularId = @Plano
                  AND pcd.AnoEscolaridade = @Ano
                  AND pcd.Ativa = 1
                  AND d.Nome = @NomeDisciplina;";

            using (SqlConnection conn = new SqlConnection(ligacao))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddWithValue("@Plano", idPlano);
                cmd.Parameters.AddWithValue("@Ano", ano);
                cmd.Parameters.AddWithValue("@NomeDisciplina", nomeDisciplina);
                cmd.Parameters.AddWithValue("@Origem", origem);
                cmd.Parameters.AddWithValue("@Ordem", ordem);
                da.Fill(dt);
            }

            return dt;
        }

        private void ValidarSelecoesCurriculares(byte ano)
        {
            // Valida a língua estrangeira, se existir
            if (painelLE.Visible)
            {
                if (ddlLE.SelectedValue == "")
                    throw new Exception("É necessário selecionar a língua estrangeira.");
            }

            // 10.º e 11.º ano: 2 disciplinas do bloco B
            if (ano == 10 || ano == 11)
            {
                if (painelB.Visible)
                {
                    int totalB = ContarSelecionados(cblB);

                    if (totalB != 2)
                        throw new Exception("É necessário selecionar duas disciplinas do bloco B.");
                }
            }

            // 12.º ano:
            // No total dos blocos C e D têm de ficar 2
            // e pelo menos uma tem de ser do bloco C
            if (ano == 12)
            {
                int totalC = ContarSelecionados(cblC);
                int totalD = ContarSelecionados(cblD);
                int total = totalC + totalD;

                if (total != 2)
                    throw new Exception("É necessário selecionar duas disciplinas dos blocos C e D.");

                if (totalC < 1)
                    throw new Exception("É necessário selecionar pelo menos uma disciplina do bloco C.");
            }
        }

        private int ContarSelecionados(CheckBoxList cbl)
        {
            int total = 0;

            for (int i = 0; i < cbl.Items.Count; i++)
            {
                if (cbl.Items[i].Selected)
                    total++;
            }

            return total;
        }

        private List<int> ObterListaPcd(int idPlano, byte ano)
        {
            List<int> lista = new List<int>();

            using (SqlConnection conn = new SqlConnection(ligacao))
            {
                conn.Open();

                // Primeiro vai buscar as obrigatórias
                string sqlObrigatorias = @"
                    SELECT Id
                    FROM dbo.PlanoCurricularDisciplina
                    WHERE PlanoCurricularId = @Plano
                      AND AnoEscolaridade = @Ano
                      AND Ativa = 1
                      AND (Natureza = N'Obrigatória' OR Natureza = N'Obrigatoria');";

                using (SqlCommand cmd = new SqlCommand(sqlObrigatorias, conn))
                {
                    cmd.Parameters.AddWithValue("@Plano", idPlano);
                    cmd.Parameters.AddWithValue("@Ano", ano);

                    SqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        lista.Add(Convert.ToInt32(dr["Id"]));
                    }

                    dr.Close();
                }

                // Língua estrangeira
                if (painelLE.Visible && ddlLE.SelectedValue != "")
                {
                    lista.Add(Convert.ToInt32(ddlLE.SelectedValue));
                }

                // Bloco B
                for (int i = 0; i < cblB.Items.Count; i++)
                {
                    if (cblB.Items[i].Selected)
                    {
                        lista.Add(Convert.ToInt32(cblB.Items[i].Value));
                    }
                }

                // Bloco C
                for (int i = 0; i < cblC.Items.Count; i++)
                {
                    if (cblC.Items[i].Selected)
                    {
                        lista.Add(Convert.ToInt32(cblC.Items[i].Value));
                    }
                }

                // Bloco D
                for (int i = 0; i < cblD.Items.Count; i++)
                {
                    if (cblD.Items[i].Selected)
                    {
                        lista.Add(Convert.ToInt32(cblD.Items[i].Value));
                    }
                }

                // EMR
                if (chkEMR.Checked)
                {
                    string sqlEmr = @"
                        SELECT Id
                        FROM dbo.PlanoCurricularDisciplina
                        WHERE PlanoCurricularId = @Plano
                          AND AnoEscolaridade = @Ano
                          AND Ativa = 1
                          AND Componente = 'EMR';";

                    using (SqlCommand cmd = new SqlCommand(sqlEmr, conn))
                    {
                        cmd.Parameters.AddWithValue("@Plano", idPlano);
                        cmd.Parameters.AddWithValue("@Ano", ano);

                        SqlDataReader dr = cmd.ExecuteReader();

                        while (dr.Read())
                        {
                            lista.Add(Convert.ToInt32(dr["Id"]));
                        }

                        dr.Close();
                    }
                }

                // PLNM
                if (chkPLNM.Checked)
                {
                    string sqlPlnm = @"
                        SELECT pcd.Id
                        FROM dbo.PlanoCurricularDisciplina pcd
                        INNER JOIN dbo.Disciplina d ON d.Id = pcd.DisciplinaId
                        WHERE pcd.PlanoCurricularId = @Plano
                          AND pcd.AnoEscolaridade = @Ano
                          AND pcd.Ativa = 1
                          AND d.Nome = 'Português Língua Não Materna';";

                    using (SqlCommand cmd = new SqlCommand(sqlPlnm, conn))
                    {
                        cmd.Parameters.AddWithValue("@Plano", idPlano);
                        cmd.Parameters.AddWithValue("@Ano", ano);

                        SqlDataReader dr = cmd.ExecuteReader();

                        while (dr.Read())
                        {
                            lista.Add(Convert.ToInt32(dr["Id"]));
                        }

                        dr.Close();
                    }
                }
            }

            // Remove repetições
            List<int> listaFinal = new List<int>();

            for (int i = 0; i < lista.Count; i++)
            {
                if (!listaFinal.Contains(lista[i]))
                    listaFinal.Add(lista[i]);
            }

            return listaFinal;
        }

        private void CarregarDropDownBloco(DropDownList ddl, int idPlano, byte ano, string bloco, out bool temDados)
        {
            DataTable dt = new DataTable();

            string sql = @"
                SELECT pcd.Id, d.Nome
                FROM dbo.PlanoCurricularDisciplina pcd
                INNER JOIN dbo.Disciplina d ON d.Id = pcd.DisciplinaId
                WHERE pcd.PlanoCurricularId = @Plano
                  AND pcd.AnoEscolaridade = @Ano
                  AND pcd.Ativa = 1
                  AND pcd.BlocoOpcao = @Bloco
                ORDER BY d.Nome;";

            using (SqlConnection conn = new SqlConnection(ligacao))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddWithValue("@Plano", idPlano);
                cmd.Parameters.AddWithValue("@Ano", ano);
                cmd.Parameters.AddWithValue("@Bloco", bloco);
                da.Fill(dt);
            }

            ddl.DataSource = dt;
            ddl.DataTextField = "Nome";
            ddl.DataValueField = "Id";
            ddl.DataBind();

            ddl.Items.Insert(0, new ListItem("-- selecionar --", ""));

            temDados = dt.Rows.Count > 0;
        }

        private void CarregarCheckBoxListBloco(CheckBoxList cbl, int idPlano, byte ano, string bloco, out bool temDados)
        {
            DataTable dt = new DataTable();

            string sql = @"
                SELECT pcd.Id, d.Nome
                FROM dbo.PlanoCurricularDisciplina pcd
                INNER JOIN dbo.Disciplina d ON d.Id = pcd.DisciplinaId
                WHERE pcd.PlanoCurricularId = @Plano
                  AND pcd.AnoEscolaridade = @Ano
                  AND pcd.Ativa = 1
                  AND pcd.BlocoOpcao = @Bloco
                ORDER BY d.Nome;";

            using (SqlConnection conn = new SqlConnection(ligacao))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddWithValue("@Plano", idPlano);
                cmd.Parameters.AddWithValue("@Ano", ano);
                cmd.Parameters.AddWithValue("@Bloco", bloco);
                da.Fill(dt);
            }

            cbl.DataSource = dt;
            cbl.DataTextField = "Nome";
            cbl.DataValueField = "Id";
            cbl.DataBind();

            temDados = dt.Rows.Count > 0;
        }

        private int CriarTurma(int idEscola, int idAnoLetivo, int idPlano, byte ano, string codigoTurma, bool ativa, List<int> listaPcd)
        {
            using (SqlConnection conn = new SqlConnection(ligacao))
            {
                conn.Open();
                SqlTransaction tr = conn.BeginTransaction();

                try
                {
                    int idTurma = 0;

                    // Cria a turma
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
                            @PlanoCurricularId,
                            @AnoEscolaridade,
                            @CodigoTurma,
                            @Ativa
                        );

                        SELECT CAST(SCOPE_IDENTITY() AS INT);";

                    using (SqlCommand cmdTurma = new SqlCommand(sqlTurma, conn, tr))
                    {
                        cmdTurma.Parameters.AddWithValue("@EscolaId", idEscola);
                        cmdTurma.Parameters.AddWithValue("@AnoLetivoId", idAnoLetivo);
                        cmdTurma.Parameters.AddWithValue("@PlanoCurricularId", idPlano);
                        cmdTurma.Parameters.AddWithValue("@AnoEscolaridade", ano);
                        cmdTurma.Parameters.AddWithValue("@CodigoTurma", codigoTurma);
                        cmdTurma.Parameters.AddWithValue("@Ativa", ativa);

                        idTurma = Convert.ToInt32(cmdTurma.ExecuteScalar());
                    }

                    // Insere as disciplinas da turma
                    for (int i = 0; i < listaPcd.Count; i++)
                    {
                        string sqlDisciplina = @"
                            INSERT INTO dbo.TurmaDisciplina
                            (
                                TurmaId,
                                DisciplinaId,
                                PlanoCurricularDisciplinaId
                            )
                            SELECT
                                @TurmaId,
                                DisciplinaId,
                                Id
                            FROM dbo.PlanoCurricularDisciplina
                            WHERE Id = @IdPcd;";

                        using (SqlCommand cmdDisciplina = new SqlCommand(sqlDisciplina, conn, tr))
                        {
                            cmdDisciplina.Parameters.AddWithValue("@TurmaId", idTurma);
                            cmdDisciplina.Parameters.AddWithValue("@IdPcd", listaPcd[i]);
                            cmdDisciplina.ExecuteNonQuery();
                        }
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

        private void AtualizarTurma(int idTurma, int idEscola, int idAnoLetivo, string codigoTurma, bool ativa, List<int> listaPcd)
        {
            using (SqlConnection conn = new SqlConnection(ligacao))
            {
                conn.Open();
                SqlTransaction tr = conn.BeginTransaction();

                try
                {
                    // Atualiza os dados principais da turma
                    string sqlTurma = @"
                        UPDATE dbo.Turma
                        SET
                            AnoLetivoId = @AnoLetivoId,
                            CodigoTurma = @CodigoTurma,
                            Ativa = @Ativa
                        WHERE Id = @Id
                          AND EscolaId = @EscolaId;";

                    using (SqlCommand cmdTurma = new SqlCommand(sqlTurma, conn, tr))
                    {
                        cmdTurma.Parameters.AddWithValue("@Id", idTurma);
                        cmdTurma.Parameters.AddWithValue("@EscolaId", idEscola);
                        cmdTurma.Parameters.AddWithValue("@AnoLetivoId", idAnoLetivo);
                        cmdTurma.Parameters.AddWithValue("@CodigoTurma", codigoTurma);
                        cmdTurma.Parameters.AddWithValue("@Ativa", ativa);
                        cmdTurma.ExecuteNonQuery();
                    }

                    // Apaga horários ligados às disciplinas atuais
                    string sqlApagarHorarios = @"
                        DELETE ht
                        FROM dbo.HorarioTurma ht
                        INNER JOIN dbo.TurmaDisciplina td ON td.Id = ht.TurmaDisciplinaId
                        WHERE td.TurmaId = @TurmaId;";

                    using (SqlCommand cmdHorarios = new SqlCommand(sqlApagarHorarios, conn, tr))
                    {
                        cmdHorarios.Parameters.AddWithValue("@TurmaId", idTurma);
                        cmdHorarios.ExecuteNonQuery();
                    }

                    // Apaga professores ligados às disciplinas atuais
                    string sqlApagarProfessores = @"
                        DELETE tdp
                        FROM dbo.TurmaDisciplinaProfessor tdp
                        INNER JOIN dbo.TurmaDisciplina td ON td.Id = tdp.TurmaDisciplinaId
                        WHERE td.TurmaId = @TurmaId;";

                    using (SqlCommand cmdProfessores = new SqlCommand(sqlApagarProfessores, conn, tr))
                    {
                        cmdProfessores.Parameters.AddWithValue("@TurmaId", idTurma);
                        cmdProfessores.ExecuteNonQuery();
                    }

                    // Apaga todas as disciplinas atuais
                    string sqlApagarDisciplinas = "DELETE FROM dbo.TurmaDisciplina WHERE TurmaId = @TurmaId;";

                    using (SqlCommand cmdApagar = new SqlCommand(sqlApagarDisciplinas, conn, tr))
                    {
                        cmdApagar.Parameters.AddWithValue("@TurmaId", idTurma);
                        cmdApagar.ExecuteNonQuery();
                    }

                    // Volta a inserir as disciplinas selecionadas
                    for (int i = 0; i < listaPcd.Count; i++)
                    {
                        string sqlDisciplina = @"
                            INSERT INTO dbo.TurmaDisciplina
                            (
                                TurmaId,
                                DisciplinaId,
                                PlanoCurricularDisciplinaId
                            )
                            SELECT
                                @TurmaId,
                                DisciplinaId,
                                Id
                            FROM dbo.PlanoCurricularDisciplina
                            WHERE Id = @IdPcd;";

                        using (SqlCommand cmdDisciplina = new SqlCommand(sqlDisciplina, conn, tr))
                        {
                            cmdDisciplina.Parameters.AddWithValue("@TurmaId", idTurma);
                            cmdDisciplina.Parameters.AddWithValue("@IdPcd", listaPcd[i]);
                            cmdDisciplina.ExecuteNonQuery();
                        }
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

        private bool TurmaJaExiste(int idEscola, int idAnoLetivo, string codigoTurma, int idPlano)
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

        private bool TurmaJaExisteNoutra(int idTurma, int idEscola, int idAnoLetivo, string codigoTurma, int idPlano)
        {
            string sql = @"
                SELECT COUNT(1)
                FROM dbo.Turma
                WHERE EscolaId = @EscolaId
                  AND AnoLetivoId = @AnoLetivoId
                  AND CodigoTurma = @CodigoTurma
                  AND PlanoCurricularId = @Plano
                  AND Id <> @IdTurma;";

            using (SqlConnection conn = new SqlConnection(ligacao))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@EscolaId", idEscola);
                cmd.Parameters.AddWithValue("@AnoLetivoId", idAnoLetivo);
                cmd.Parameters.AddWithValue("@CodigoTurma", codigoTurma);
                cmd.Parameters.AddWithValue("@Plano", idPlano);
                cmd.Parameters.AddWithValue("@IdTurma", idTurma);

                conn.Open();

                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        private void CarregarTurma(int idTurma)
        {
            int idEscola = GetIdEscola();

            string sql = @"
                SELECT
                    t.AnoLetivoId,
                    t.PlanoCurricularId,
                    t.AnoEscolaridade,
                    t.CodigoTurma,
                    t.Ativa
                FROM dbo.Turma t
                WHERE t.Id = @Id
                  AND t.EscolaId = @EscolaId;";

            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(ligacao))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddWithValue("@Id", idTurma);
                cmd.Parameters.AddWithValue("@EscolaId", idEscola);
                da.Fill(dt);
            }

            if (dt.Rows.Count == 0)
                return;

            DataRow dr = dt.Rows[0];

            ddlAnoLetivo.SelectedValue = dr["AnoLetivoId"].ToString();
            ddlPlano.SelectedValue = dr["PlanoCurricularId"].ToString();
            ddlAno.SelectedValue = dr["AnoEscolaridade"].ToString();
            txtCodigoTurma.Text = dr["CodigoTurma"].ToString();
            chkAtiva.Checked = Convert.ToBoolean(dr["Ativa"]);
        }

        private void CarregarOpcoesDaTurma(int idTurma)
        {
            string sql = @"
                SELECT
                    pcd.Id,
                    pcd.BlocoOpcao,
                    pcd.Componente,
                    d.Nome AS Disciplina
                FROM dbo.TurmaDisciplina td
                INNER JOIN dbo.PlanoCurricularDisciplina pcd ON pcd.Id = td.PlanoCurricularDisciplinaId
                INNER JOIN dbo.Disciplina d ON d.Id = pcd.DisciplinaId
                WHERE td.TurmaId = @TurmaId;";

            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(ligacao))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddWithValue("@TurmaId", idTurma);
                da.Fill(dt);
            }

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow dr = dt.Rows[i];

                string bloco = "";
                if (dr["BlocoOpcao"] != DBNull.Value)
                    bloco = dr["BlocoOpcao"].ToString();

                string disciplina = "";
                if (dr["Disciplina"] != DBNull.Value)
                    disciplina = dr["Disciplina"].ToString();

                string valor = dr["Id"].ToString();

                // Língua estrangeira
                if (bloco == "LE")
                {
                    if (ddlLE.Items.FindByValue(valor) != null)
                        ddlLE.SelectedValue = valor;
                }

                // Bloco B
                if (bloco == "B")
                {
                    ListItem item = cblB.Items.FindByValue(valor);
                    if (item != null)
                        item.Selected = true;
                }

                // Bloco C
                if (bloco == "C")
                {
                    ListItem item = cblC.Items.FindByValue(valor);
                    if (item != null)
                        item.Selected = true;
                }

                // Bloco D
                if (bloco == "D")
                {
                    ListItem item = cblD.Items.FindByValue(valor);
                    if (item != null)
                        item.Selected = true;
                }

                // EMR
                if (dr["Componente"] != DBNull.Value)
                {
                    if (dr["Componente"].ToString() == "EMR")
                        chkEMR.Checked = true;
                }

                // PLNM
                if (disciplina == "Português Língua Não Materna")
                    chkPLNM.Checked = true;
            }
        }

        private int GetIdEscola()
        {
            int idEscola = 0;

            if (Request.QueryString["id"] != null)
                int.TryParse(Request.QueryString["id"], out idEscola);
            else if (Session["EscolaIdTurmaSecundario"] != null)
                int.TryParse(Session["EscolaIdTurmaSecundario"].ToString(), out idEscola);

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

                if (nome == null || nome == DBNull.Value)
                    lblEscola.Text = "Escola: ";
                else
                    lblEscola.Text = "Escola: " + nome.ToString();
            }
        }

        private void LimparOpcoes()
        {
            // Limpa o conteúdo dos controlos
            ddlLE.Items.Clear();
            cblB.Items.Clear();
            cblC.Items.Clear();
            cblD.Items.Clear();

            // Esconde os painéis
            painelLE.Visible = false;
            painelB.Visible = false;
            painelC.Visible = false;
            painelD.Visible = false;
        }

        private void MostrarMensagem(string mensagem, bool erro = true)
        {
            lblMensagem.Visible = true;
            lblMensagem.Text = mensagem;

            if (erro)
                lblMensagem.CssClass = "alert alert-danger d-block";
            else
                lblMensagem.CssClass = "alert alert-success d-block";
        }
    }
}