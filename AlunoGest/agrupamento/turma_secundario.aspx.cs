using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AlunoGest.agrupamento
{
    public partial class turma_secundario : System.Web.UI.Page
    {
        // String de ligação à base de dados.
        private string ligacao = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Só carregamos os dados iniciais na primeira vez.
            if (!IsPostBack)
            {
                lblMensagem.Visible = false;
                painelDisciplinas.Visible = false;

                int idEscola = GetIdEscola();

                if (idEscola == 0)
                {
                    MostrarMensagem("Não foi indicada nenhuma escola.");
                    painelFormulario.Visible = false;
                    return;
                }

                // Guardamos o id da escola em sessão para reutilizar em postbacks.
                Session["EscolaIdTurmaSecundario"] = idEscola;

                CarregarNomeEscola(idEscola);
                CarregarAnosLetivos();
                CarregarCursos();
                PreencherAnoLetivoVindoDaPaginaAnterior();

                string modo = GetModo();

                if (modo == "editar")
                {
                    lblModo.Text = "Modo: editar turma";

                    int idTurma = GetIdTurma();

                    if (idTurma == 0)
                    {
                        MostrarMensagem("Não foi indicada nenhuma turma para editar.");
                        painelFormulario.Visible = false;
                        return;
                    }

                    // Primeiro carregamos os dados da turma.
                    CarregarTurma(idTurma);

                    // Depois mostramos os controlos adequados ao curso/ano.
                    MostrarOpcoes();

                    // Depois marcamos as opções já guardadas na turma.
                    CarregarOpcoesDaTurma(idTurma);

                    // E por fim mostramos a grelha final.
                    MostrarPreview();

                    // Em edição não deixamos mudar estes campos.
                    ddlAnoLetivo.Enabled = false;
                    ddlPlano.Enabled = false;
                    ddlAno.Enabled = false;
                }
                else
                {
                    lblModo.Text = "Modo: criar turma";
                    chkAtiva.Checked = true;
                }
            }
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

        protected void buttonGuardar_Click(object sender, EventArgs e)
        {
            lblMensagem.Visible = false;

            if (ddlAnoLetivo.SelectedValue == "")
            {
                MostrarMensagem("Não foi indicado o ano letivo.");
                return;
            }

            if (ddlPlano.SelectedValue == "")
            {
                MostrarMensagem("Seleciona o curso.");
                return;
            }

            if (ddlAno.SelectedValue == "")
            {
                MostrarMensagem("Seleciona o ano.");
                return;
            }

            if (txtCodigoTurma.Text.Trim() == "")
            {
                MostrarMensagem("Indica o código da turma.");
                return;
            }

            int idEscola = GetIdEscola();
            int idAnoLetivo = Convert.ToInt32(ddlAnoLetivo.SelectedValue);
            int idPlano = Convert.ToInt32(ddlPlano.SelectedValue);
            byte ano = Convert.ToByte(ddlAno.SelectedValue);
            string codigoTurma = txtCodigoTurma.Text.Trim();
            bool ativa = chkAtiva.Checked;

            try
            {
                // Valida as escolhas curriculares.
                ValidarSelecoesCurriculares(ano);

                // Cria a lista final das disciplinas que ficam na turma.
                List<int> listaPcd = ObterListaPcdSelecionadas(idPlano, ano);

                if (listaPcd.Count == 0)
                {
                    MostrarMensagem("Não foi possível determinar as disciplinas da turma.");
                    return;
                }

                if (GetModo() == "editar")
                {
                    int idTurma = GetIdTurma();

                    if (TurmaJaExisteNoutra(idTurma, idEscola, idAnoLetivo, codigoTurma, idPlano))
                    {
                        MostrarMensagem("Já existe outra turma com esse código.");
                        return;
                    }

                    AtualizarTurma(idTurma, idEscola, idAnoLetivo, codigoTurma, ativa, listaPcd);
                }
                else
                {
                    if (TurmaJaExiste(idEscola, idAnoLetivo, codigoTurma, idPlano))
                    {
                        MostrarMensagem("Já existe uma turma com esse código.");
                        return;
                    }

                    CriarTurma(idEscola, idAnoLetivo, idPlano, ano, codigoTurma, ativa, listaPcd);
                }

                Response.Redirect("~/agrupamento/turmas.aspx?id=" + idEscola + "&anoLetivoId=" + idAnoLetivo);
            }
            catch (Exception ex)
            {
                MostrarMensagem(ex.Message);
            }
        }

        protected void buttonCancelar_Click(object sender, EventArgs e)
        {
            int idEscola = GetIdEscola();
            string anoLetivoId = ddlAnoLetivo.SelectedValue;

            Response.Redirect("~/agrupamento/turmas.aspx?id=" + idEscola + "&anoLetivoId=" + anoLetivoId);
        }

        private void CarregarAnosLetivos()
        {
            string sql = @"
                SELECT Id, Descricao
                FROM dbo.AnoLetivo
                WHERE Ativo = 1
                ORDER BY Descricao DESC;";

            DataTable dt = ExecutarTabela(sql);

            ddlAnoLetivo.DataSource = dt;
            ddlAnoLetivo.DataTextField = "Descricao";
            ddlAnoLetivo.DataValueField = "Id";
            ddlAnoLetivo.DataBind();

            ddlAnoLetivo.Items.Insert(0, new ListItem("-- selecionar --", ""));
        }

        private void CarregarCursos()
        {
            string sql = @"
                SELECT pc.Id, pc.Nome
                FROM dbo.PlanoCurricular pc
                INNER JOIN dbo.OfertaFormativa ofo ON ofo.Id = pc.OfertaFormativaId
                WHERE pc.Ativo = 1
                  AND ofo.NivelEnsino = N'Secundario'
                ORDER BY pc.Nome;";

            DataTable dt = ExecutarTabela(sql);

            ddlPlano.DataSource = dt;
            ddlPlano.DataTextField = "Nome";
            ddlPlano.DataValueField = "Id";
            ddlPlano.DataBind();

            ddlPlano.Items.Insert(0, new ListItem("-- selecionar --", ""));
        }

        private void PreencherAnoLetivoVindoDaPaginaAnterior()
        {
            // O utilizador chega aqui vindo do turmas.aspx.
            if (Request.QueryString["anoLetivoId"] != null)
            {
                string valor = Request.QueryString["anoLetivoId"].ToString();

                if (ddlAnoLetivo.Items.FindByValue(valor) != null)
                    ddlAnoLetivo.SelectedValue = valor;
            }

            // Mantemos o contexto vindo da página anterior.
            ddlAnoLetivo.Enabled = false;
        }

        private void MostrarOpcoes()
        {
            LimparOpcoes();

            if (ddlPlano.SelectedValue == "" || ddlAno.SelectedValue == "")
                return;

            int idPlano = Convert.ToInt32(ddlPlano.SelectedValue);
            byte ano = Convert.ToByte(ddlAno.SelectedValue);

            // 10.º e 11.º anos:
            // - língua estrangeira no bloco A
            // - duas disciplinas no bloco B
            if (ano == 10 || ano == 11)
            {
                CarregarLinguasEstrangeiras(idPlano, ano);
                CarregarCheckBoxListBloco(cblB, idPlano, ano, "B", out bool temB);

                painelLE.Visible = ddlLE.Items.Count > 1;
                painelB.Visible = temB;
            }

            // 12.º ano:
            // - grupo C
            // - grupo D
            if (ano == 12)
            {
                CarregarCheckBoxListBloco(cblC, idPlano, ano, "C", out bool temC);
                CarregarCheckBoxListBloco(cblD, idPlano, ano, "D", out bool temD);

                painelC.Visible = temC;
                painelD.Visible = temD;
            }
        }

        private void LimparOpcoes()
        {
            ddlLE.Items.Clear();
            cblB.Items.Clear();
            cblC.Items.Clear();
            cblD.Items.Clear();

            chkPLNM.Checked = false;
            chkEMR.Checked = false;

            painelLE.Visible = false;
            painelB.Visible = false;
            painelC.Visible = false;
            painelD.Visible = false;
        }

        private void CarregarLinguasEstrangeiras(int idPlano, byte ano)
        {
            // A língua estrangeira dos 10.º e 11.º anos tem de vir do bloco A.
            string sql = @"
                SELECT pcd.Id, d.Nome
                FROM dbo.PlanoCurricularDisciplina pcd
                INNER JOIN dbo.Disciplina d ON d.Id = pcd.DisciplinaId
                WHERE pcd.PlanoCurricularId = @Plano
                  AND pcd.AnoEscolaridade = @Ano
                  AND pcd.Ativa = 1
                  AND pcd.BlocoOpcao = N'A'
                ORDER BY d.Nome;";

            DataTable dt = ExecutarTabela(sql,
                new SqlParameter("@Plano", idPlano),
                new SqlParameter("@Ano", ano));

            ddlLE.DataSource = dt;
            ddlLE.DataTextField = "Nome";
            ddlLE.DataValueField = "Id";
            ddlLE.DataBind();

            ddlLE.Items.Insert(0, new ListItem("-- selecionar --", ""));
        }

        private void CarregarCheckBoxListBloco(CheckBoxList cbl, int idPlano, byte ano, string bloco, out bool temDados)
        {
            string sql = @"
                SELECT pcd.Id, d.Nome
                FROM dbo.PlanoCurricularDisciplina pcd
                INNER JOIN dbo.Disciplina d ON d.Id = pcd.DisciplinaId
                WHERE pcd.PlanoCurricularId = @Plano
                  AND pcd.AnoEscolaridade = @Ano
                  AND pcd.Ativa = 1
                  AND pcd.BlocoOpcao = @Bloco
                ORDER BY d.Nome;";

            DataTable dt = ExecutarTabela(sql,
                new SqlParameter("@Plano", idPlano),
                new SqlParameter("@Ano", ano),
                new SqlParameter("@Bloco", bloco));

            cbl.DataSource = dt;
            cbl.DataTextField = "Nome";
            cbl.DataValueField = "Id";
            cbl.DataBind();

            temDados = dt.Rows.Count > 0;
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

            DataTable dtPreview = CriarTabelaPreview();

            AdicionarObrigatoriasPreview(dtPreview, idPlano, ano);
            AdicionarOpcoesPreview(dtPreview, idPlano, ano);

            DataView dv = dtPreview.DefaultView;
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

        private void AdicionarObrigatoriasPreview(DataTable dtPreview, int idPlano, byte ano)
        {
            string sql = @"
                SELECT
                    pcd.Id,
                    gd.Nome AS GrupoDisciplinar,
                    d.Nome AS Disciplina,
                    pcd.Natureza
                FROM dbo.PlanoCurricularDisciplina pcd
                INNER JOIN dbo.Disciplina d ON d.Id = pcd.DisciplinaId
                INNER JOIN dbo.GrupoDisciplinar gd ON gd.Id = d.GrupoDisciplinarId
                WHERE pcd.PlanoCurricularId = @Plano
                  AND pcd.AnoEscolaridade = @Ano
                  AND pcd.Ativa = 1
                  AND (pcd.Natureza = N'Obrigatória' OR pcd.Natureza = N'Obrigatoria')
                ORDER BY gd.Nome, d.Nome;";

            DataTable dt = ExecutarTabela(sql,
                new SqlParameter("@Plano", idPlano),
                new SqlParameter("@Ano", ano));

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                AdicionarLinhaPreview(
                    dtPreview,
                    dt.Rows[i]["GrupoDisciplinar"].ToString(),
                    dt.Rows[i]["Disciplina"].ToString(),
                    "Obrigatória",
                    "Obrigatória",
                    1);
            }
        }

        private void AdicionarOpcoesPreview(DataTable dtPreview, int idPlano, byte ano)
        {
            // 10.º e 11.º anos: língua estrangeira do bloco A
            if ((ano == 10 || ano == 11) && ddlLE.SelectedValue != "")
            {
                AdicionarPreviewPorPcdId(dtPreview, Convert.ToInt32(ddlLE.SelectedValue), "Opção - grupo A", 2);
            }

            // Bloco B
            for (int i = 0; i < cblB.Items.Count; i++)
            {
                if (cblB.Items[i].Selected)
                    AdicionarPreviewPorPcdId(dtPreview, Convert.ToInt32(cblB.Items[i].Value), "Opção - grupo B", 2);
            }

            // Bloco C
            for (int i = 0; i < cblC.Items.Count; i++)
            {
                if (cblC.Items[i].Selected)
                    AdicionarPreviewPorPcdId(dtPreview, Convert.ToInt32(cblC.Items[i].Value), "Opção - grupo C", 2);
            }

            // Bloco D
            for (int i = 0; i < cblD.Items.Count; i++)
            {
                if (cblD.Items[i].Selected)
                    AdicionarPreviewPorPcdId(dtPreview, Convert.ToInt32(cblD.Items[i].Value), "Opção - grupo D", 2);
            }

            // PLNM
            if (chkPLNM.Checked)
            {
                int idPcdPlnm = GetIdPcdPlnm(idPlano, ano);

                if (idPcdPlnm > 0)
                    AdicionarPreviewPorPcdId(dtPreview, idPcdPlnm, "PLNM", 3);
            }

            // EMR
            if (chkEMR.Checked)
            {
                int idPcdEmr = GetIdPcdEmr(idPlano, ano);

                if (idPcdEmr > 0)
                    AdicionarPreviewPorPcdId(dtPreview, idPcdEmr, "EMR", 3);
            }
        }

        private void AdicionarPreviewPorPcdId(DataTable dtPreview, int idPcd, string origem, int ordem)
        {
            string sql = @"
                SELECT
                    gd.Nome AS GrupoDisciplinar,
                    d.Nome AS Disciplina
                FROM dbo.PlanoCurricularDisciplina pcd
                INNER JOIN dbo.Disciplina d ON d.Id = pcd.DisciplinaId
                INNER JOIN dbo.GrupoDisciplinar gd ON gd.Id = d.GrupoDisciplinarId
                WHERE pcd.Id = @Id;";

            DataTable dt = ExecutarTabela(sql, new SqlParameter("@Id", idPcd));

            if (dt.Rows.Count > 0)
            {
                AdicionarLinhaPreview(
                    dtPreview,
                    dt.Rows[0]["GrupoDisciplinar"].ToString(),
                    dt.Rows[0]["Disciplina"].ToString(),
                    "Opção",
                    origem,
                    ordem);
            }
        }

        private void AdicionarLinhaPreview(DataTable dtPreview, string grupo, string disciplina, string natureza, string origem, int ordem)
        {
            // Evita repetir a mesma disciplina na grelha.
            for (int i = 0; i < dtPreview.Rows.Count; i++)
            {
                if (dtPreview.Rows[i]["Disciplina"].ToString() == disciplina)
                    return;
            }

            DataRow nova = dtPreview.NewRow();
            nova["GrupoDisciplinar"] = grupo;
            nova["Disciplina"] = disciplina;
            nova["Natureza"] = natureza;
            nova["Origem"] = origem;
            nova["Ordem"] = ordem;
            dtPreview.Rows.Add(nova);
        }

        private void ValidarSelecoesCurriculares(byte ano)
        {
            // 10.º e 11.º anos:
            // - 1 língua estrangeira do grupo A
            // - 2 disciplinas do grupo B
            if (ano == 10 || ano == 11)
            {
                if (ddlLE.SelectedValue == "")
                    throw new Exception("É necessário selecionar uma língua estrangeira do grupo A.");

                int totalB = ContarSelecionados(cblB);

                if (totalB != 2)
                    throw new Exception("É necessário selecionar duas disciplinas do grupo B.");
            }

            // 12.º ano:
            // - 2 disciplinas no total dos grupos C e D
            // - pelo menos 1 do grupo C
            if (ano == 12)
            {
                int totalC = ContarSelecionados(cblC);
                int totalD = ContarSelecionados(cblD);
                int total = totalC + totalD;

                if (total != 2)
                    throw new Exception("É necessário selecionar duas disciplinas dos grupos C e D.");

                if (totalC < 1)
                    throw new Exception("É necessário selecionar pelo menos uma disciplina do grupo C.");
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

        private List<int> ObterListaPcdSelecionadas(int idPlano, byte ano)
        {
            List<int> lista = new List<int>();

            // 1) Primeiro juntamos as obrigatórias.
            string sqlObrigatorias = @"
                SELECT Id
                FROM dbo.PlanoCurricularDisciplina
                WHERE PlanoCurricularId = @Plano
                  AND AnoEscolaridade = @Ano
                  AND Ativa = 1
                  AND (Natureza = N'Obrigatória' OR Natureza = N'Obrigatoria');";

            DataTable dtObrigatorias = ExecutarTabela(sqlObrigatorias,
                new SqlParameter("@Plano", idPlano),
                new SqlParameter("@Ano", ano));

            for (int i = 0; i < dtObrigatorias.Rows.Count; i++)
            {
                lista.Add(Convert.ToInt32(dtObrigatorias.Rows[i]["Id"]));
            }

            // 2) Grupo A - língua estrangeira
            if (ddlLE.SelectedValue != "")
                lista.Add(Convert.ToInt32(ddlLE.SelectedValue));

            // 3) Grupo B
            for (int i = 0; i < cblB.Items.Count; i++)
            {
                if (cblB.Items[i].Selected)
                    lista.Add(Convert.ToInt32(cblB.Items[i].Value));
            }

            // 4) Grupo C
            for (int i = 0; i < cblC.Items.Count; i++)
            {
                if (cblC.Items[i].Selected)
                    lista.Add(Convert.ToInt32(cblC.Items[i].Value));
            }

            // 5) Grupo D
            for (int i = 0; i < cblD.Items.Count; i++)
            {
                if (cblD.Items[i].Selected)
                    lista.Add(Convert.ToInt32(cblD.Items[i].Value));
            }

            // 6) PLNM
            if (chkPLNM.Checked)
            {
                int idPcdPlnm = GetIdPcdPlnm(idPlano, ano);

                if (idPcdPlnm > 0)
                    lista.Add(idPcdPlnm);
            }

            // 7) EMR
            if (chkEMR.Checked)
            {
                int idPcdEmr = GetIdPcdEmr(idPlano, ano);

                if (idPcdEmr > 0)
                    lista.Add(idPcdEmr);
            }

            // 8) Remove repetições
            List<int> listaFinal = new List<int>();

            for (int i = 0; i < lista.Count; i++)
            {
                if (!listaFinal.Contains(lista[i]))
                    listaFinal.Add(lista[i]);
            }

            return listaFinal;
        }

        private int GetIdPcdPlnm(int idPlano, byte ano)
        {
            string sql = @"
                SELECT TOP 1 pcd.Id
                FROM dbo.PlanoCurricularDisciplina pcd
                INNER JOIN dbo.Disciplina d ON d.Id = pcd.DisciplinaId
                WHERE pcd.PlanoCurricularId = @Plano
                  AND pcd.AnoEscolaridade = @Ano
                  AND pcd.Ativa = 1
                  AND d.Nome = N'Português Língua Não Materna';";

            return ExecutarEscalarInt(sql,
                new SqlParameter("@Plano", idPlano),
                new SqlParameter("@Ano", ano));
        }

        private int GetIdPcdEmr(int idPlano, byte ano)
        {
            string sql = @"
                SELECT TOP 1 Id
                FROM dbo.PlanoCurricularDisciplina
                WHERE PlanoCurricularId = @Plano
                  AND AnoEscolaridade = @Ano
                  AND Ativa = 1
                  AND Componente = N'EMR';";

            return ExecutarEscalarInt(sql,
                new SqlParameter("@Plano", idPlano),
                new SqlParameter("@Ano", ano));
        }

        private void CarregarTurma(int idTurma)
        {
            int idEscola = GetIdEscola();

            string sql = @"
                SELECT
                    AnoLetivoId,
                    PlanoCurricularId,
                    AnoEscolaridade,
                    CodigoTurma,
                    Ativa
                FROM dbo.Turma
                WHERE Id = @Id
                  AND EscolaId = @EscolaId;";

            DataTable dt = ExecutarTabela(sql,
                new SqlParameter("@Id", idTurma),
                new SqlParameter("@EscolaId", idEscola));

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

            DataTable dt = ExecutarTabela(sql, new SqlParameter("@TurmaId", idTurma));

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow dr = dt.Rows[i];

                string valor = dr["Id"].ToString();

                string bloco = "";
                if (dr["BlocoOpcao"] != DBNull.Value)
                    bloco = dr["BlocoOpcao"].ToString();

                string componente = "";
                if (dr["Componente"] != DBNull.Value)
                    componente = dr["Componente"].ToString();

                string disciplina = "";
                if (dr["Disciplina"] != DBNull.Value)
                    disciplina = dr["Disciplina"].ToString();

                // Grupo A - língua estrangeira
                if (bloco == "A")
                {
                    if (ddlLE.Items.FindByValue(valor) != null)
                        ddlLE.SelectedValue = valor;
                }

                // Grupo B
                if (bloco == "B")
                {
                    ListItem itemB = cblB.Items.FindByValue(valor);
                    if (itemB != null)
                        itemB.Selected = true;
                }

                // Grupo C
                if (bloco == "C")
                {
                    ListItem itemC = cblC.Items.FindByValue(valor);
                    if (itemC != null)
                        itemC.Selected = true;
                }

                // Grupo D
                if (bloco == "D")
                {
                    ListItem itemD = cblD.Items.FindByValue(valor);
                    if (itemD != null)
                        itemD.Selected = true;
                }

                // EMR
                if (componente == "EMR")
                    chkEMR.Checked = true;

                // PLNM
                if (disciplina == "Português Língua Não Materna")
                    chkPLNM.Checked = true;
            }
        }

        private int CriarTurma(int idEscola, int idAnoLetivo, int idPlano, byte ano, string codigoTurma, bool ativa, List<int> listaPcd)
        {
            using (SqlConnection conn = new SqlConnection(ligacao))
            {
                conn.Open();
                SqlTransaction tr = conn.BeginTransaction();

                try
                {
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

                    int idTurma = 0;

                    using (SqlCommand cmd = new SqlCommand(sqlTurma, conn, tr))
                    {
                        cmd.Parameters.AddWithValue("@EscolaId", idEscola);
                        cmd.Parameters.AddWithValue("@AnoLetivoId", idAnoLetivo);
                        cmd.Parameters.AddWithValue("@PlanoCurricularId", idPlano);
                        cmd.Parameters.AddWithValue("@AnoEscolaridade", ano);
                        cmd.Parameters.AddWithValue("@CodigoTurma", codigoTurma);
                        cmd.Parameters.AddWithValue("@Ativa", ativa);

                        idTurma = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    InserirTurmaDisciplinas(conn, tr, idTurma, listaPcd);

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
                    string sqlTurma = @"
                        UPDATE dbo.Turma
                        SET
                            AnoLetivoId = @AnoLetivoId,
                            CodigoTurma = @CodigoTurma,
                            Ativa = @Ativa
                        WHERE Id = @Id
                          AND EscolaId = @EscolaId;";

                    using (SqlCommand cmd = new SqlCommand(sqlTurma, conn, tr))
                    {
                        cmd.Parameters.AddWithValue("@Id", idTurma);
                        cmd.Parameters.AddWithValue("@EscolaId", idEscola);
                        cmd.Parameters.AddWithValue("@AnoLetivoId", idAnoLetivo);
                        cmd.Parameters.AddWithValue("@CodigoTurma", codigoTurma);
                        cmd.Parameters.AddWithValue("@Ativa", ativa);
                        cmd.ExecuteNonQuery();
                    }

                    // Apaga dependências antes de voltar a inserir as disciplinas da turma.
                    string sqlApagarHorarios = @"
                        DELETE ht
                        FROM dbo.HorarioTurma ht
                        INNER JOIN dbo.TurmaDisciplina td ON td.Id = ht.TurmaDisciplinaId
                        WHERE td.TurmaId = @TurmaId;";

                    using (SqlCommand cmd = new SqlCommand(sqlApagarHorarios, conn, tr))
                    {
                        cmd.Parameters.AddWithValue("@TurmaId", idTurma);
                        cmd.ExecuteNonQuery();
                    }

                    string sqlApagarProfessores = @"
                        DELETE tdp
                        FROM dbo.TurmaDisciplinaProfessor tdp
                        INNER JOIN dbo.TurmaDisciplina td ON td.Id = tdp.TurmaDisciplinaId
                        WHERE td.TurmaId = @TurmaId;";

                    using (SqlCommand cmd = new SqlCommand(sqlApagarProfessores, conn, tr))
                    {
                        cmd.Parameters.AddWithValue("@TurmaId", idTurma);
                        cmd.ExecuteNonQuery();
                    }

                    string sqlApagarDisciplinas = "DELETE FROM dbo.TurmaDisciplina WHERE TurmaId = @TurmaId;";

                    using (SqlCommand cmd = new SqlCommand(sqlApagarDisciplinas, conn, tr))
                    {
                        cmd.Parameters.AddWithValue("@TurmaId", idTurma);
                        cmd.ExecuteNonQuery();
                    }

                    InserirTurmaDisciplinas(conn, tr, idTurma, listaPcd);

                    tr.Commit();
                }
                catch
                {
                    tr.Rollback();
                    throw;
                }
            }
        }

        private void InserirTurmaDisciplinas(SqlConnection conn, SqlTransaction tr, int idTurma, List<int> listaPcd)
        {
            for (int i = 0; i < listaPcd.Count; i++)
            {
                string sql = @"
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
                        NULL
                    FROM dbo.PlanoCurricularDisciplina pcd
                    WHERE pcd.Id = @IdPcd;";

                using (SqlCommand cmd = new SqlCommand(sql, conn, tr))
                {
                    cmd.Parameters.AddWithValue("@TurmaId", idTurma);
                    cmd.Parameters.AddWithValue("@IdPcd", listaPcd[i]);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private bool TurmaJaExiste(int idEscola, int idAnoLetivo, string codigoTurma, int idPlano)
        {
            string sql = @"
                SELECT COUNT(*)
                FROM dbo.Turma
                WHERE EscolaId = @EscolaId
                  AND AnoLetivoId = @AnoLetivoId
                  AND PlanoCurricularId = @PlanoCurricularId
                  AND CodigoTurma = @CodigoTurma;";

            return ExecutarEscalarInt(sql,
                new SqlParameter("@EscolaId", idEscola),
                new SqlParameter("@AnoLetivoId", idAnoLetivo),
                new SqlParameter("@PlanoCurricularId", idPlano),
                new SqlParameter("@CodigoTurma", codigoTurma)) > 0;
        }

        private bool TurmaJaExisteNoutra(int idTurma, int idEscola, int idAnoLetivo, string codigoTurma, int idPlano)
        {
            string sql = @"
                SELECT COUNT(*)
                FROM dbo.Turma
                WHERE EscolaId = @EscolaId
                  AND AnoLetivoId = @AnoLetivoId
                  AND PlanoCurricularId = @PlanoCurricularId
                  AND CodigoTurma = @CodigoTurma
                  AND Id <> @Id;";

            return ExecutarEscalarInt(sql,
                new SqlParameter("@EscolaId", idEscola),
                new SqlParameter("@AnoLetivoId", idAnoLetivo),
                new SqlParameter("@PlanoCurricularId", idPlano),
                new SqlParameter("@CodigoTurma", codigoTurma),
                new SqlParameter("@Id", idTurma)) > 0;
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
                return Request.QueryString["modo"].ToString().ToLower();

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

        private void MostrarMensagem(string mensagem)
        {
            lblMensagem.Visible = true;
            lblMensagem.Text = mensagem;
            lblMensagem.CssClass = "alert alert-danger d-block";
        }

        private DataTable ExecutarTabela(string sql, params SqlParameter[] parametros)
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(ligacao))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                if (parametros != null)
                {
                    for (int i = 0; i < parametros.Length; i++)
                        cmd.Parameters.Add(parametros[i]);
                }

                da.Fill(dt);
            }

            return dt;
        }

        private int ExecutarEscalarInt(string sql, params SqlParameter[] parametros)
        {
            using (SqlConnection conn = new SqlConnection(ligacao))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                if (parametros != null)
                {
                    for (int i = 0; i < parametros.Length; i++)
                        cmd.Parameters.Add(parametros[i]);
                }

                conn.Open();

                object valor = cmd.ExecuteScalar();

                if (valor == null || valor == DBNull.Value)
                    return 0;

                return Convert.ToInt32(valor);
            }
        }
    }
}