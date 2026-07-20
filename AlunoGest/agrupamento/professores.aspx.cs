using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using AlunoGest.Util;

namespace AlunoGest.agrupamento
{
    public partial class professores : Page
    {
        #region Campos

        private readonly string _connectionString =
            ConfigurationManager
                .ConnectionStrings["DefaultConnection"]
                .ConnectionString;

        #endregion

        #region Página

        protected void Page_Load(
            object sender,
            EventArgs e)
        {
            if (!IsPostBack)
            {
                CarregarGrupoRecrutamento();
                GetProfessores();

                controlos.Visible = false;
                painelDisciplinasProfessor.Visible = false;
            }
        }

        #endregion

        #region Botões principais

        protected void buttonVer_Click(
            object sender,
            EventArgs e)
        {
            int idProfessor;

            if (!ProfessorSelecionado(out idProfessor))
            {
                MostrarAlert(
                    "Selecione um professor."
                );

                return;
            }

            CarregarProfessor(idProfessor);
            CarregarDisciplinasDoProfessor(idProfessor);

            controlos.Visible = true;
            painelDisciplinasProfessor.Visible = true;

            ViewState["op"] = "v";

            ActivarControlos(false);
        }

        protected void buttonCriar_Click(
            object sender,
            EventArgs e)
        {
            LimparFormulario();

            gridProfessores.SelectedIndex = -1;

            gridDisciplinasProfessor.DataSource = null;
            gridDisciplinasProfessor.DataBind();

            controlos.Visible = true;
            painelDisciplinasProfessor.Visible = false;

            ViewState["op"] = "i";

            ActivarControlos(true);
        }

        protected void buttonEditar_Click(
            object sender,
            EventArgs e)
        {
            int idProfessor;

            if (!ProfessorSelecionado(out idProfessor))
            {
                MostrarAlert(
                    "Selecione um professor."
                );

                return;
            }

            CarregarProfessor(idProfessor);
            CarregarDisciplinasDoProfessor(idProfessor);

            controlos.Visible = true;
            painelDisciplinasProfessor.Visible = true;

            ViewState["op"] = "u";

            ActivarControlos(true);
        }

        protected void buttonGuardar_Click(
            object sender,
            EventArgs e)
        {
            if (!Page.IsValid)
            {
                return;
            }

            string modo =
                Convert.ToString(
                    ViewState["op"]
                )
                .ToLowerInvariant();

            string nome =
                txtNome.Text.Trim();

            string email =
                txtEmail.Text.Trim();

            string telefone =
                txtTelefone.Text.Trim();

            string numeroProcesso =
                txtNumeroProcesso.Text.Trim();

            string nif =
                ValidadorNif.Normalizar(
                    txtNIF.Text
                );

            string mensagemNif;

            if (!ValidadorNif.ValidarLocalmente(
                    nif,
                    out mensagemNif))
            {
                MostrarAlert(mensagemNif);
                return;
            }

            //ResultadoValidacaoNifApi resultadoApi =
            //    ValidadorNif.ValidarNaApi(nif);

            //if (resultadoApi.Estado !=
            //    EstadoValidacaoNifApi.Valido)
            //{
            //    MostrarAlert(
            //        resultadoApi.Mensagem
            //    );

            //    return;
            //}

            int agrupamentoId;

            try
            {
                agrupamentoId =
                    GetAgrupamentoIdFromSession();
            }
            catch (Exception ex)
            {
                MostrarAlert(ex.Message);
                return;
            }

            int? grupoRecrutamentoId = null;

            if (!string.IsNullOrWhiteSpace(
                    ddlGrupoRecrutamento.SelectedValue))
            {
                grupoRecrutamentoId =
                    Convert.ToInt32(
                        ddlGrupoRecrutamento
                            .SelectedValue
                    );
            }

            try
            {
                if (modo == "i")
                {
                    if (NifJaExiste(
                            nif,
                            null,
                            null))
                    {
                        MostrarAlert(
                            "Já existe um aluno ou professor " +
                            "com este NIF."
                        );

                        return;
                    }

                    Guid userIdProfessor =
                        CriarContaProfessor();

                    int linhas =
                        InsertProfessor(
                            userIdProfessor,
                            agrupamentoId,
                            nome,
                            numeroProcesso,
                            email,
                            telefone,
                            nif,
                            grupoRecrutamentoId
                        );

                    if (linhas == 0)
                    {
                        MostrarAlert(
                            "Não foi possível criar o professor."
                        );

                        return;
                    }

                    MostrarAlert(
                        "Professor criado com sucesso."
                    );
                }
                else if (modo == "u")
                {
                    int idProfessor;

                    if (!ProfessorSelecionado(
                            out idProfessor))
                    {
                        MostrarAlert(
                            "Selecione um professor."
                        );

                        return;
                    }

                    if (NifJaExiste(
                            nif,
                            null,
                            idProfessor))
                    {
                        MostrarAlert(
                            "Já existe outro aluno ou professor " +
                            "com este NIF."
                        );

                        return;
                    }

                    int linhas =
                        UpdateProfessor(
                            idProfessor,
                            agrupamentoId,
                            nome,
                            numeroProcesso,
                            email,
                            telefone,
                            nif,
                            grupoRecrutamentoId
                        );

                    if (linhas == 0)
                    {
                        MostrarAlert(
                            "Não foi possível atualizar o professor."
                        );

                        return;
                    }

                    MostrarAlert(
                        "Professor atualizado com sucesso."
                    );
                }
                else
                {
                    FecharFormulario();
                    return;
                }

                GetProfessores();
                FecharFormulario();
            }
            catch (MembershipCreateUserException ex)
            {
                MostrarAlert(
                    "Não foi possível criar a conta do professor: " +
                    ex.Message
                );
            }
            catch (SqlException ex)
            {
                MostrarAlert(
                    "Erro na base de dados: " +
                    ex.Message
                );
            }
            catch (Exception ex)
            {
                MostrarAlert(
                    "Erro ao guardar o professor: " +
                    ex.Message
                );
            }
        }

        protected void buttonCancelar_Click(
            object sender,
            EventArgs e)
        {
            FecharFormulario();
        }

        protected void buttonDisciplinasProfessor_Click(
            object sender,
            EventArgs e)
        {
            int idProfessor;

            if (!ProfessorSelecionado(
                    out idProfessor))
            {
                MostrarAlert(
                    "Selecione o professor."
                );

                return;
            }

            Session["ProfessorId"] =
                idProfessor;

            Response.Redirect(
                "~/agrupamento/professor_disciplinas.aspx"
            );
        }

        #endregion

        #region Paginação

        protected void gridProfessores_PageIndexChanging(
            object sender,
            GridViewPageEventArgs e)
        {
            gridProfessores.PageIndex =
                e.NewPageIndex;

            GetProfessores();

            gridProfessores.SelectedIndex = -1;

            controlos.Visible = false;
            painelDisciplinasProfessor.Visible = false;
        }

        #endregion

        #region Listagem e leitura

        private void GetProfessores()
        {
            int agrupamentoId =
                GetAgrupamentoIdFromSession();

            DataTable tabela =
                new DataTable();

            const string sql = @"
                SELECT
                    p.Id,
                    p.Nome,
                    gr.Nome AS GrupoRecrutamento,
                    p.NIF,
                    p.Telefone,
                    p.Email

                FROM dbo.Professor p

                LEFT JOIN dbo.GrupoRecrutamento gr
                    ON p.GrupoRecrutamentoId = gr.Id

                WHERE p.AgrupamentoId = @AgrupamentoId
                  AND p.Ativo = 1

                ORDER BY p.Nome;";

            using (SqlConnection conn =
                new SqlConnection(_connectionString))
            using (SqlCommand cmd =
                new SqlCommand(sql, conn))
            using (SqlDataAdapter adapter =
                new SqlDataAdapter(cmd))
            {
                cmd.Parameters
                    .Add(
                        "@AgrupamentoId",
                        SqlDbType.Int
                    )
                    .Value = agrupamentoId;

                adapter.Fill(tabela);
            }

            gridProfessores.DataSource = tabela;
            gridProfessores.DataBind();
        }

        private DataRow GetProfessorById(
            int idProfessor)
        {
            int agrupamentoId =
                GetAgrupamentoIdFromSession();

            DataTable tabela =
                new DataTable();

            const string sql = @"
                SELECT
                    Id,
                    Nome,
                    Email,
                    Telefone,
                    NIF,
                    NumeroProcesso,
                    GrupoRecrutamentoId

                FROM dbo.Professor

                WHERE Id = @Id
                  AND AgrupamentoId = @AgrupamentoId;";

            using (SqlConnection conn =
                new SqlConnection(_connectionString))
            using (SqlCommand cmd =
                new SqlCommand(sql, conn))
            using (SqlDataAdapter adapter =
                new SqlDataAdapter(cmd))
            {
                cmd.Parameters
                    .Add(
                        "@Id",
                        SqlDbType.Int
                    )
                    .Value = idProfessor;

                cmd.Parameters
                    .Add(
                        "@AgrupamentoId",
                        SqlDbType.Int
                    )
                    .Value = agrupamentoId;

                adapter.Fill(tabela);
            }

            return tabela.Rows.Count > 0
                ? tabela.Rows[0]
                : null;
        }

        private void CarregarProfessor(
            int idProfessor)
        {
            DataRow professor =
                GetProfessorById(idProfessor);

            if (professor == null)
            {
                MostrarAlert(
                    "Não foi possível encontrar o professor."
                );

                return;
            }

            txtNome.Text =
                ValorTexto(
                    professor["Nome"]
                );

            txtEmail.Text =
                ValorTexto(
                    professor["Email"]
                );

            txtTelefone.Text =
                ValorTexto(
                    professor["Telefone"]
                );

            txtNIF.Text =
                ValorTexto(
                    professor["NIF"]
                );

            txtNumeroProcesso.Text =
                ValorTexto(
                    professor["NumeroProcesso"]
                );

            if (professor["GrupoRecrutamentoId"] ==
                DBNull.Value)
            {
                ddlGrupoRecrutamento.SelectedIndex = 0;
            }
            else
            {
                string valor =
                    professor[
                        "GrupoRecrutamentoId"
                    ]
                    .ToString();

                ListItem item =
                    ddlGrupoRecrutamento
                        .Items
                        .FindByValue(valor);

                if (item != null)
                {
                    ddlGrupoRecrutamento.SelectedValue =
                        valor;
                }
            }
        }

        #endregion

        #region Inserção e atualização

        private int InsertProfessor(
            Guid userId,
            int agrupamentoId,
            string nome,
            string numeroProcesso,
            string email,
            string telefone,
            string nif,
            int? grupoRecrutamentoId)
        {
            const string sql = @"
                INSERT INTO dbo.Professor
                (
                    AgrupamentoId,
                    UserId,
                    Nome,
                    Email,
                    Telefone,
                    NIF,
                    NumeroProcesso,
                    GrupoRecrutamentoId,
                    Ativo,
                    CreatedAt
                )
                VALUES
                (
                    @AgrupamentoId,
                    @UserId,
                    @Nome,
                    @Email,
                    @Telefone,
                    @NIF,
                    @NumeroProcesso,
                    @GrupoRecrutamentoId,
                    1,
                    SYSDATETIME()
                );";

            using (SqlConnection conn =
                new SqlConnection(_connectionString))
            using (SqlCommand cmd =
                new SqlCommand(sql, conn))
            {
                AdicionarParametrosProfessor(
                    cmd,
                    agrupamentoId,
                    nome,
                    numeroProcesso,
                    email,
                    telefone,
                    nif,
                    grupoRecrutamentoId
                );

                cmd.Parameters
                    .Add(
                        "@UserId",
                        SqlDbType.UniqueIdentifier
                    )
                    .Value = userId;

                conn.Open();

                return cmd.ExecuteNonQuery();
            }
        }

        private int UpdateProfessor(
            int idProfessor,
            int agrupamentoId,
            string nome,
            string numeroProcesso,
            string email,
            string telefone,
            string nif,
            int? grupoRecrutamentoId)
        {
            const string sql = @"
                UPDATE dbo.Professor

                SET
                    Nome = @Nome,
                    Email = @Email,
                    Telefone = @Telefone,
                    NIF = @NIF,
                    NumeroProcesso = @NumeroProcesso,
                    GrupoRecrutamentoId = @GrupoRecrutamentoId

                WHERE Id = @Id
                  AND AgrupamentoId = @AgrupamentoId;";

            using (SqlConnection conn =
                new SqlConnection(_connectionString))
            using (SqlCommand cmd =
                new SqlCommand(sql, conn))
            {
                AdicionarParametrosProfessor(
                    cmd,
                    agrupamentoId,
                    nome,
                    numeroProcesso,
                    email,
                    telefone,
                    nif,
                    grupoRecrutamentoId
                );

                cmd.Parameters
                    .Add(
                        "@Id",
                        SqlDbType.Int
                    )
                    .Value = idProfessor;

                conn.Open();

                return cmd.ExecuteNonQuery();
            }
        }

        private void AdicionarParametrosProfessor(
            SqlCommand cmd,
            int agrupamentoId,
            string nome,
            string numeroProcesso,
            string email,
            string telefone,
            string nif,
            int? grupoRecrutamentoId)
        {
            cmd.Parameters
                .Add(
                    "@AgrupamentoId",
                    SqlDbType.Int
                )
                .Value = agrupamentoId;

            cmd.Parameters
                .Add(
                    "@Nome",
                    SqlDbType.NVarChar,
                    200
                )
                .Value = nome;

            cmd.Parameters
                .Add(
                    "@Email",
                    SqlDbType.NVarChar,
                    150
                )
                .Value = email;

            cmd.Parameters
                .Add(
                    "@Telefone",
                    SqlDbType.NVarChar,
                    20
                )
                .Value = telefone;

            cmd.Parameters
                .Add(
                    "@NIF",
                    SqlDbType.NVarChar,
                    9
                )
                .Value = nif;

            cmd.Parameters
                .Add(
                    "@NumeroProcesso",
                    SqlDbType.NVarChar,
                    50
                )
                .Value = numeroProcesso;

            cmd.Parameters
                .Add(
                    "@GrupoRecrutamentoId",
                    SqlDbType.Int
                )
                .Value =
                grupoRecrutamentoId.HasValue
                    ? (object)grupoRecrutamentoId.Value
                    : DBNull.Value;
        }

        #endregion

        #region Validação de duplicação do NIF

        private bool NifJaExiste(
            string nif,
            int? alunoIdIgnorar,
            int? professorIdIgnorar)
        {
            const string sql = @"
                SELECT
                    CASE
                        WHEN EXISTS
                        (
                            SELECT 1

                            FROM dbo.Aluno

                            WHERE NIF = @NIF
                              AND
                              (
                                  @AlunoIdIgnorar IS NULL
                                  OR Id <> @AlunoIdIgnorar
                              )
                        )
                        OR EXISTS
                        (
                            SELECT 1

                            FROM dbo.Professor

                            WHERE NIF = @NIF
                              AND
                              (
                                  @ProfessorIdIgnorar IS NULL
                                  OR Id <> @ProfessorIdIgnorar
                              )
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
                        "@NIF",
                        SqlDbType.NVarChar,
                        9
                    )
                    .Value = nif;

                cmd.Parameters
                    .Add(
                        "@AlunoIdIgnorar",
                        SqlDbType.Int
                    )
                    .Value =
                    alunoIdIgnorar.HasValue
                        ? (object)alunoIdIgnorar.Value
                        : DBNull.Value;

                cmd.Parameters
                    .Add(
                        "@ProfessorIdIgnorar",
                        SqlDbType.Int
                    )
                    .Value =
                    professorIdIgnorar.HasValue
                        ? (object)professorIdIgnorar.Value
                        : DBNull.Value;

                conn.Open();

                return Convert.ToInt32(
                    cmd.ExecuteScalar()
                ) == 1;
            }
        }

        #endregion

        #region Disciplinas e grupos

        private void CarregarDisciplinasDoProfessor(
            int professorId)
        {
            DataTable tabela =
                new DataTable();

            const string sql = @"
                SELECT
                    gd.Nome AS GrupoDisciplinar,
                    d.Nome AS Disciplina,
                    pd.Desde,
                    pd.Ate,

                    CASE
                        WHEN pd.Ate IS NULL
                            THEN 'Ativa'
                        ELSE 'Terminada'
                    END AS Estado

                FROM dbo.ProfessorDisciplina pd

                INNER JOIN dbo.Disciplina d
                    ON pd.DisciplinaId = d.Id

                INNER JOIN dbo.GrupoDisciplinar gd
                    ON d.GrupoDisciplinarId = gd.Id

                WHERE pd.ProfessorId = @ProfessorId

                ORDER BY
                    CASE
                        WHEN pd.Ate IS NULL THEN 0
                        ELSE 1
                    END,
                    gd.Nome,
                    d.Nome,
                    pd.Desde DESC;";

            using (SqlConnection conn =
                new SqlConnection(_connectionString))
            using (SqlCommand cmd =
                new SqlCommand(sql, conn))
            using (SqlDataAdapter adapter =
                new SqlDataAdapter(cmd))
            {
                cmd.Parameters
                    .Add(
                        "@ProfessorId",
                        SqlDbType.Int
                    )
                    .Value = professorId;

                adapter.Fill(tabela);
            }

            gridDisciplinasProfessor.DataSource =
                tabela;

            gridDisciplinasProfessor.DataBind();
        }

        private void CarregarGrupoRecrutamento()
        {
            DataTable tabela =
                new DataTable();

            const string sql = @"
                SELECT
                    Id,
                    Nome

                FROM dbo.GrupoRecrutamento

                WHERE Ativo = 1

                ORDER BY Nome;";

            using (SqlConnection conn =
                new SqlConnection(_connectionString))
            using (SqlCommand cmd =
                new SqlCommand(sql, conn))
            using (SqlDataAdapter adapter =
                new SqlDataAdapter(cmd))
            {
                adapter.Fill(tabela);
            }

            ddlGrupoRecrutamento.DataSource =
                tabela;

            ddlGrupoRecrutamento.DataTextField =
                "Nome";

            ddlGrupoRecrutamento.DataValueField =
                "Id";

            ddlGrupoRecrutamento.DataBind();

            ddlGrupoRecrutamento.Items.Insert(
                0,
                new ListItem(
                    "-- selecionar --",
                    string.Empty
                )
            );
        }

        #endregion

        #region Membership

        private Guid CriarContaProfessor()
        {
            string usernameBase =
                CriarConta.GerarUsername(
                    txtNome.Text
                );

            string username =
                CriarConta.GarantirUsernameUnico(
                    usernameBase
                );

            string password =
                CriarConta.GerarPassword();

            Membership.CreateUser(
                username,
                password,
                txtEmail.Text.Trim()
            );

            Roles.AddUserToRole(
                username,
                "Professor"
            );

            MembershipUser utilizador =
                Membership.GetUser(username);

            if (utilizador == null)
            {
                throw new InvalidOperationException(
                    "A conta do professor foi criada, " +
                    "mas não foi possível obter os seus dados."
                );
            }

            CriarConta.EnviarEmailCredenciais(
                txtEmail.Text.Trim(),
                txtNome.Text.Trim(),
                username,
                password,
                "http://localhost/login.aspx"
            );

            return (Guid)utilizador.ProviderUserKey;
        }

        #endregion

        #region Sessão e agrupamento

        private int GetAgrupamentoIdFromSession()
        {
            object sessionUserId =
                Session["UserId"];

            if (sessionUserId == null)
            {
                throw new InvalidOperationException(
                    "A sessão terminou. Inicie sessão novamente."
                );
            }

            Guid userId;

            if (!Guid.TryParse(
                    sessionUserId.ToString(),
                    out userId))
            {
                throw new InvalidOperationException(
                    "O utilizador guardado na sessão é inválido."
                );
            }

            const string sql = @"
                SELECT Id

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
                    throw new InvalidOperationException(
                        "Não foi encontrado um agrupamento " +
                        "associado ao utilizador autenticado."
                    );
                }

                return Convert.ToInt32(resultado);
            }
        }

        #endregion

        #region Formulário e utilidades

        private void ActivarControlos(
            bool ativo)
        {
            txtNome.Enabled = ativo;
            txtEmail.Enabled = ativo;
            txtTelefone.Enabled = ativo;
            txtNIF.Enabled = ativo;
            txtNumeroProcesso.Enabled = ativo;
            ddlGrupoRecrutamento.Enabled = ativo;

            buttonGuardar.Visible = ativo;
        }

        private void LimparFormulario()
        {
            txtNome.Text = string.Empty;
            txtEmail.Text = string.Empty;
            txtTelefone.Text = string.Empty;
            txtNIF.Text = string.Empty;
            txtNumeroProcesso.Text = string.Empty;

            if (ddlGrupoRecrutamento.Items.Count > 0)
            {
                ddlGrupoRecrutamento.SelectedIndex = 0;
            }
        }

        private void FecharFormulario()
        {
            LimparFormulario();

            ViewState["op"] = null;

            gridProfessores.SelectedIndex = -1;

            controlos.Visible = false;
            painelDisciplinasProfessor.Visible = false;

            gridDisciplinasProfessor.DataSource = null;
            gridDisciplinasProfessor.DataBind();
        }

        private bool ProfessorSelecionado(
            out int idProfessor)
        {
            idProfessor = 0;

            if (gridProfessores.SelectedDataKey == null ||
                gridProfessores.SelectedDataKey.Value == null)
            {
                return false;
            }

            return int.TryParse(
                gridProfessores.SelectedDataKey
                    .Value
                    .ToString(),
                out idProfessor
            );
        }

        private string ValorTexto(
            object valor)
        {
            return valor == null ||
                   valor == DBNull.Value
                ? string.Empty
                : valor.ToString();
        }

        private void MostrarAlert(
            string mensagem)
        {
            string mensagemSegura =
                mensagem
                    .Replace("\\", "\\\\")
                    .Replace("'", "\\'")
                    .Replace(
                        Environment.NewLine,
                        "\\n"
                    );

            string script =
                string.Format(
                    "alert('{0}');",
                    mensagemSegura
                );

            ClientScript.RegisterStartupScript(
                GetType(),
                Guid.NewGuid().ToString(),
                script,
                true
            );
        }

        #endregion
    }
}
