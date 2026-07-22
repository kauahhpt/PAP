using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using AlunoGest.Util;

namespace AlunoGest.agrupamento
{
    public partial class Encarregados : Page
    {
        #region Campos

        private readonly string _connectionString =
            ConfigurationManager
                .ConnectionStrings["DefaultConnection"]
                .ConnectionString;

        #endregion


        #region Eventos

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ButtonCriar.Click += ButtonCriar_Click;
            ButtonEditar.Click += ButtonEditar_Click;
            ButtonGerirEducandos.Click += ButtonGerirEducandos_Click;

            ButtonGuardar.Click += ButtonGuardar_Click;
            ButtonCancelar.Click += ButtonCancelar_Click;

            ButtonAssociarAluno.Click += ButtonAssociarAluno_Click;
            ButtonFecharEducandos.Click += ButtonFecharEducandos_Click;

            GridEducandos.RowCommand += GridEducandos_RowCommand;
        }

        #endregion


        #region Página

        protected void Page_Load(
            object sender,
            EventArgs e)
        {
            if (!Request.IsAuthenticated ||
                !Roles.IsUserInRole(
                    User.Identity.Name,
                    "agrupamento"))
            {
                FormsAuthentication.SignOut();
                Session.Clear();

                Response.Redirect("~/login.aspx");
                return;
            }

            int agrupamentoId;

            if (!TryGetAgrupamentoId(out agrupamentoId))
            {
                Response.Redirect("~/login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                PnlFormulario.Visible = false;
                PnlEducandos.Visible = false;
                PnlAssociacaoInicial.Visible = false;

                RfvAlunoInicial.Enabled = false;

                LimparMensagem();
                GetEncarregados();
            }
        }

        #endregion


        #region Botões principais

        protected void ButtonCriar_Click(
            object sender,
            EventArgs e)
        {
            int agrupamentoId;

            if (!TryGetAgrupamentoId(out agrupamentoId))
            {
                Response.Redirect("~/login.aspx");
                return;
            }

            LimparMensagem();
            LimparFormulario();

            GridEncarregados.SelectedIndex = -1;

            ViewState["Op"] = "criar";
            ViewState["EncarregadoId"] = null;

            ChkAtivo.Checked = true;
            ChkPrincipalInicial.Checked = true;

            PnlFormulario.Visible = true;
            PnlAssociacaoInicial.Visible = true;
            PnlEducandos.Visible = false;

            RfvAlunoInicial.Enabled = true;

            CarregarAlunosDropDown(
                DdlAlunoInicial,
                agrupamentoId,
                null
            );
        }

        protected void ButtonEditar_Click(
            object sender,
            EventArgs e)
        {
            LimparMensagem();

            int encarregadoId;

            if (!EncarregadoSelecionado(
                    out encarregadoId))
            {
                MostrarMensagem(
                    "Selecione um encarregado de educação."
                );

                return;
            }

            EncarregadoDados encarregado =
                GetEncarregadoById(encarregadoId);

            if (encarregado == null)
            {
                MostrarMensagem(
                    "Não foi possível encontrar o encarregado selecionado."
                );

                GetEncarregados();
                return;
            }

            CarregarFormulario(encarregado);

            ViewState["Op"] = "editar";
            ViewState["EncarregadoId"] = encarregadoId;

            PnlFormulario.Visible = true;
            PnlAssociacaoInicial.Visible = false;
            PnlEducandos.Visible = false;

            RfvAlunoInicial.Enabled = false;
        }

        protected void ButtonGerirEducandos_Click(
            object sender,
            EventArgs e)
        {
            LimparMensagem();

            int encarregadoId;

            if (!EncarregadoSelecionado(
                    out encarregadoId))
            {
                MostrarMensagem(
                    "Selecione um encarregado de educação."
                );

                return;
            }

            EncarregadoDados encarregado =
                GetEncarregadoById(encarregadoId);

            if (encarregado == null)
            {
                MostrarMensagem(
                    "Não foi possível encontrar o encarregado selecionado."
                );

                GetEncarregados();
                return;
            }

            ViewState["EncarregadoId"] =
                encarregadoId;

            LblEncarregadoSelecionado.Text =
                "Encarregado selecionado: " +
                encarregado.NomeCompleto;

            TxtParentesco.Text = string.Empty;
            ChkPrincipal.Checked = false;

            CarregarEducandos(encarregadoId);
            CarregarAlunosDisponiveis(encarregadoId);

            PnlFormulario.Visible = false;
            PnlEducandos.Visible = true;
        }

        #endregion


        #region Guardar encarregado

        protected void ButtonGuardar_Click(
            object sender,
            EventArgs e)
        {
            LimparMensagem();

            string operacao =
                Convert.ToString(ViewState["Op"]);

            bool criar =
                operacao == "criar";

            bool editar =
                operacao == "editar";

            if (!criar && !editar)
            {
                MostrarMensagem(
                    "A operação selecionada não é válida."
                );

                return;
            }

            RfvAlunoInicial.Enabled = criar;

            Page.Validate("encarregado");

            if (!Page.IsValid)
            {
                PnlFormulario.Visible = true;
                PnlAssociacaoInicial.Visible = criar;

                return;
            }

            int agrupamentoId;

            if (!TryGetAgrupamentoId(out agrupamentoId))
            {
                Response.Redirect("~/login.aspx");
                return;
            }

            string nomeCompleto =
                TxtNomeCompleto.Text.Trim();

            string email =
                TxtEmail.Text
                    .Trim()
                    .ToLowerInvariant();

            string telefone =
                TxtTelefone.Text.Trim();

            string nif =
                ValidadorNif.Normalizar(
                    TxtNIF.Text
                );

            string mensagemNif;

            if (!ValidadorNif.ValidarLocalmente(
                    nif,
                    out mensagemNif))
            {
                MostrarMensagem(mensagemNif);

                PnlFormulario.Visible = true;
                PnlAssociacaoInicial.Visible = criar;

                return;
            }

            if (criar)
            {
                CriarNovoEncarregado(
                    agrupamentoId,
                    nomeCompleto,
                    email,
                    telefone,
                    nif
                );

                return;
            }

            AtualizarEncarregadoExistente(
                nomeCompleto,
                email,
                telefone,
                nif
            );
        }

        private void CriarNovoEncarregado(
            int agrupamentoId,
            string nomeCompleto,
            string email,
            string telefone,
            string nif)
        {
            int alunoId;

            if (!int.TryParse(
                    DdlAlunoInicial.SelectedValue,
                    out alunoId))
            {
                MostrarMensagem(
                    "Selecione o aluno que ficará associado ao encarregado."
                );

                PnlFormulario.Visible = true;
                PnlAssociacaoInicial.Visible = true;

                return;
            }

            if (!AlunoPertenceAoAgrupamento(
                    alunoId,
                    agrupamentoId))
            {
                MostrarMensagem(
                    "O aluno selecionado não pertence ao agrupamento."
                );

                return;
            }

            if (NifJaExiste(nif, null))
            {
                MostrarMensagem(
                    "Já existe um aluno, professor ou encarregado " +
                    "de educação com este NIF."
                );

                return;
            }

            if (EmailJaExisteNaTabela(
                    email,
                    null))
            {
                MostrarMensagem(
                    "Já existe um encarregado de educação com este email."
                );

                return;
            }

            if (EmailJaExisteNoMembership(
                    email,
                    null))
            {
                MostrarMensagem(
                    "Já existe uma conta de utilizador associada a este email."
                );

                return;
            }

            string parentesco =
                TxtParentescoInicial.Text.Trim();

            bool principal =
                ChkPrincipalInicial.Checked;

            string username =
                null;

            string password =
                null;

            try
            {
                CriarContaEncarregado(
                    nomeCompleto,
                    email,
                    telefone,
                    nif,
                    alunoId,
                    parentesco,
                    principal,
                    ChkAtivo.Checked,
                    out username,
                    out password
                );
            }
            catch (MembershipCreateUserException ex)
            {
                MostrarMensagem(
                    "Não foi possível criar a conta do encarregado: " +
                    ex.Message
                );

                return;
            }
            catch (SqlException ex)
            {
                MostrarMensagem(
                    "Erro na base de dados ao criar o encarregado: " +
                    ex.Message
                );

                return;
            }
            catch (Exception ex)
            {
                MostrarMensagem(
                    "Não foi possível criar o encarregado: " +
                    ex.Message
                );

                return;
            }

            bool emailEnviado =
                EnviarCredenciais(
                    email,
                    nomeCompleto,
                    username,
                    password
                );

            GetEncarregados();
            LimparFormulario();

            GridEncarregados.SelectedIndex = -1;

            ViewState["Op"] = null;
            ViewState["EncarregadoId"] = null;

            PnlFormulario.Visible = false;
            PnlAssociacaoInicial.Visible = false;

            if (emailEnviado)
            {
                MostrarMensagem(
                    "Encarregado criado com sucesso. " +
                    "As credenciais foram enviadas por email.",
                    false
                );
            }
            else
            {
                MostrarMensagem(
                    "O encarregado e a respetiva conta foram criados, " +
                    "mas não foi possível enviar o email com as credenciais."
                );
            }
        }

        private void AtualizarEncarregadoExistente(
            string nomeCompleto,
            string email,
            string telefone,
            string nif)
        {
            int encarregadoId;

            if (!TryGetEncarregadoIdViewState(
                    out encarregadoId))
            {
                MostrarMensagem(
                    "Não foi possível identificar o encarregado."
                );

                return;
            }

            EncarregadoDados dadosAtuais =
                GetEncarregadoById(encarregadoId);

            if (dadosAtuais == null)
            {
                MostrarMensagem(
                    "O encarregado já não está disponível."
                );

                GetEncarregados();
                return;
            }

            if (NifJaExiste(
                    nif,
                    encarregadoId))
            {
                MostrarMensagem(
                    "Já existe outro aluno, professor ou encarregado " +
                    "de educação com este NIF."
                );

                return;
            }

            if (EmailJaExisteNaTabela(
                    email,
                    encarregadoId))
            {
                MostrarMensagem(
                    "Já existe outro encarregado de educação com este email."
                );

                return;
            }

            if (EmailJaExisteNoMembership(
                    email,
                    dadosAtuais.UserId))
            {
                MostrarMensagem(
                    "Já existe outra conta associada a este email."
                );

                return;
            }

            try
            {
                AtualizarEncarregadoEMembership(
                    dadosAtuais,
                    nomeCompleto,
                    email,
                    telefone,
                    nif,
                    ChkAtivo.Checked
                );
            }
            catch (SqlException ex)
            {
                MostrarMensagem(
                    "Erro na base de dados ao atualizar o encarregado: " +
                    ex.Message
                );

                return;
            }
            catch (Exception ex)
            {
                MostrarMensagem(
                    "Não foi possível atualizar o encarregado: " +
                    ex.Message
                );

                return;
            }

            GetEncarregados();
            LimparFormulario();

            GridEncarregados.SelectedIndex = -1;

            ViewState["Op"] = null;
            ViewState["EncarregadoId"] = null;

            PnlFormulario.Visible = false;
            PnlAssociacaoInicial.Visible = false;

            MostrarMensagem(
                "Encarregado atualizado com sucesso.",
                false
            );
        }

        protected void ButtonCancelar_Click(
            object sender,
            EventArgs e)
        {
            LimparMensagem();
            LimparFormulario();

            GridEncarregados.SelectedIndex = -1;

            ViewState["Op"] = null;
            ViewState["EncarregadoId"] = null;

            PnlFormulario.Visible = false;
            PnlAssociacaoInicial.Visible = false;

            RfvAlunoInicial.Enabled = false;
        }

        #endregion


        #region Criação da conta

        private void CriarContaEncarregado(
            string nomeCompleto,
            string email,
            string telefone,
            string nif,
            int alunoId,
            string parentesco,
            bool principal,
            bool ativo,
            out string username,
            out string password)
        {
            username = null;
            password = null;

            MembershipUser utilizadorCriado =
                null;

            string usernameBase =
                CriarConta.GerarUsername(
                    nomeCompleto
                );

            username =
                CriarConta.GarantirUsernameUnico(
                    usernameBase
                );

            password =
                CriarConta.GerarPassword();

            try
            {
                utilizadorCriado =
                    Membership.CreateUser(
                        username,
                        password,
                        email
                    );

                if (utilizadorCriado == null)
                {
                    throw new InvalidOperationException(
                        "O Membership não devolveu o utilizador criado."
                    );
                }

                Roles.AddUserToRole(
                    username,
                    "encarregado"
                );

                Guid userId =
                    (Guid)utilizadorCriado.ProviderUserKey;

                InserirEncarregadoEAssociacao(
                    userId,
                    nomeCompleto,
                    email,
                    telefone,
                    nif,
                    alunoId,
                    parentesco,
                    principal,
                    ativo
                );
            }
            catch
            {
                if (!string.IsNullOrWhiteSpace(username))
                {
                    RemoverContaCriada(username);
                }

                throw;
            }
        }

        private bool EnviarCredenciais(
            string email,
            string nomeCompleto,
            string username,
            string password)
        {
            try
            {
                string urlLogin =
                    Request.Url.GetLeftPart(
                        UriPartial.Authority
                    ) +
                    ResolveUrl("~/login.aspx");

                CriarConta.EnviarEmailCredenciais(
                    email,
                    nomeCompleto,
                    username,
                    password,
                    urlLogin
                );

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError(
                    "Erro ao enviar credenciais do encarregado: " +
                    ex
                );

                return false;
            }
        }

        private void RemoverContaCriada(
            string username)
        {
            try
            {
                if (Roles.IsUserInRole(
                        username,
                        "encarregado"))
                {
                    Roles.RemoveUserFromRole(
                        username,
                        "encarregado"
                    );
                }

                Membership.DeleteUser(
                    username,
                    true
                );
            }
            catch (Exception ex)
            {
               System.Diagnostics.Trace.TraceError(
                    "Erro ao remover conta incompleta do encarregado: " +
                    ex
                );
            }
        }

        #endregion


        #region Inserção e atualização

        private void InserirEncarregadoEAssociacao(
            Guid userId,
            string nomeCompleto,
            string email,
            string telefone,
            string nif,
            int alunoId,
            string parentesco,
            bool principal,
            bool ativo)
        {
            using (SqlConnection conn =
                new SqlConnection(_connectionString))
            {
                conn.Open();

                using (SqlTransaction transaction =
                    conn.BeginTransaction())
                {
                    try
                    {
                        const string inserirEncarregado = @"
                            INSERT INTO dbo.EncarregadoEducacao
                            (
                                UserId,
                                NomeCompleto,
                                Email,
                                Telefone,
                                NIF,
                                Ativo
                            )
                            VALUES
                            (
                                @UserId,
                                @NomeCompleto,
                                @Email,
                                @Telefone,
                                @NIF,
                                @Ativo
                            );

                            SELECT
                                CAST(SCOPE_IDENTITY() AS INT);";

                        int encarregadoId;

                        using (SqlCommand cmd =
                            new SqlCommand(
                                inserirEncarregado,
                                conn,
                                transaction))
                        {
                            AdicionarParametrosEncarregado(
                                cmd,
                                nomeCompleto,
                                email,
                                telefone,
                                nif,
                                ativo
                            );

                            cmd.Parameters
                                .Add(
                                    "@UserId",
                                    SqlDbType.UniqueIdentifier
                                )
                                .Value = userId;

                            encarregadoId =
                                Convert.ToInt32(
                                    cmd.ExecuteScalar()
                                );
                        }

                        if (principal)
                        {
                            DesmarcarPrincipalAtual(
                                alunoId,
                                conn,
                                transaction
                            );
                        }

                        const string inserirAssociacao = @"
                            INSERT INTO dbo.AlunoEncarregado
                            (
                                AlunoId,
                                EncarregadoEducacaoId,
                                Parentesco,
                                Principal,
                                Ativo
                            )
                            VALUES
                            (
                                @AlunoId,
                                @EncarregadoEducacaoId,
                                @Parentesco,
                                @Principal,
                                1
                            );";

                        using (SqlCommand cmd =
                            new SqlCommand(
                                inserirAssociacao,
                                conn,
                                transaction))
                        {
                            cmd.Parameters
                                .Add(
                                    "@AlunoId",
                                    SqlDbType.Int
                                )
                                .Value = alunoId;

                            cmd.Parameters
                                .Add(
                                    "@EncarregadoEducacaoId",
                                    SqlDbType.Int
                                )
                                .Value = encarregadoId;

                            cmd.Parameters
                                .Add(
                                    "@Parentesco",
                                    SqlDbType.NVarChar,
                                    50
                                )
                                .Value =
                                ValorOuDbNull(parentesco);

                            cmd.Parameters
                                .Add(
                                    "@Principal",
                                    SqlDbType.Bit
                                )
                                .Value = principal;

                            cmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        try
                        {
                            transaction.Rollback();
                        }
                        catch
                        {
                        }

                        throw;
                    }
                }
            }
        }

        private void AtualizarEncarregadoEMembership(
            EncarregadoDados dadosAtuais,
            string nomeCompleto,
            string email,
            string telefone,
            string nif,
            bool ativo)
        {
            MembershipUser utilizador =
                Membership.GetUser(
                    dadosAtuais.UserId,
                    false
                );

            if (utilizador == null)
            {
                throw new InvalidOperationException(
                    "Não foi possível encontrar a conta do encarregado."
                );
            }

            string emailAnterior =
                utilizador.Email;

            bool estadoAnterior =
                utilizador.IsApproved;

            utilizador.Email = email;
            utilizador.IsApproved = ativo;

            Membership.UpdateUser(utilizador);

            try
            {
                const string sql = @"
                    UPDATE dbo.EncarregadoEducacao

                    SET
                        NomeCompleto = @NomeCompleto,
                        Email = @Email,
                        Telefone = @Telefone,
                        NIF = @NIF,
                        Ativo = @Ativo

                    WHERE Id = @Id;";

                using (SqlConnection conn =
                    new SqlConnection(_connectionString))
                using (SqlCommand cmd =
                    new SqlCommand(sql, conn))
                {
                    AdicionarParametrosEncarregado(
                        cmd,
                        nomeCompleto,
                        email,
                        telefone,
                        nif,
                        ativo
                    );

                    cmd.Parameters
                        .Add(
                            "@Id",
                            SqlDbType.Int
                        )
                        .Value = dadosAtuais.Id;

                    conn.Open();

                    int linhas =
                        cmd.ExecuteNonQuery();

                    if (linhas != 1)
                    {
                        throw new InvalidOperationException(
                            "O registo do encarregado não foi atualizado."
                        );
                    }
                }
            }
            catch
            {
                try
                {
                    utilizador.Email =
                        emailAnterior;

                    utilizador.IsApproved =
                        estadoAnterior;

                    Membership.UpdateUser(utilizador);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.TraceError(
                        "Erro ao repor os dados do Membership: " +
                        ex
                    );
                }

                throw;
            }
        }

        private void AdicionarParametrosEncarregado(
            SqlCommand cmd,
            string nomeCompleto,
            string email,
            string telefone,
            string nif,
            bool ativo)
        {
            cmd.Parameters
                .Add(
                    "@NomeCompleto",
                    SqlDbType.NVarChar,
                    200
                )
                .Value = nomeCompleto;

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
                .Value =
                ValorOuDbNull(telefone);

            cmd.Parameters
                .Add(
                    "@NIF",
                    SqlDbType.NVarChar,
                    9
                )
                .Value = nif;

            cmd.Parameters
                .Add(
                    "@Ativo",
                    SqlDbType.Bit
                )
                .Value = ativo;
        }

        #endregion


        #region Gestão dos educandos

        protected void ButtonAssociarAluno_Click(
            object sender,
            EventArgs e)
        {
            LimparMensagem();

            Page.Validate("associacao");

            if (!Page.IsValid)
            {
                PnlEducandos.Visible = true;
                return;
            }

            int encarregadoId;

            if (!TryGetEncarregadoIdViewState(
                    out encarregadoId))
            {
                MostrarMensagem(
                    "Não foi possível identificar o encarregado."
                );

                return;
            }

            EncarregadoDados encarregado =
                GetEncarregadoById(encarregadoId);

            if (encarregado == null)
            {
                MostrarMensagem(
                    "O encarregado selecionado já não está disponível."
                );

                GetEncarregados();
                PnlEducandos.Visible = false;

                return;
            }

            int agrupamentoId;

            if (!TryGetAgrupamentoId(out agrupamentoId))
            {
                Response.Redirect("~/login.aspx");
                return;
            }

            int alunoId;

            if (!int.TryParse(
                    DdlAlunoAssociar.SelectedValue,
                    out alunoId))
            {
                MostrarMensagem(
                    "Selecione um aluno."
                );

                PnlEducandos.Visible = true;
                return;
            }

            if (!AlunoPertenceAoAgrupamento(
                    alunoId,
                    agrupamentoId))
            {
                MostrarMensagem(
                    "O aluno selecionado não pertence ao agrupamento."
                );

                return;
            }

            string parentesco =
                TxtParentesco.Text.Trim();

            bool principal =
                ChkPrincipal.Checked;

            try
            {
                AssociarAluno(
                    encarregadoId,
                    alunoId,
                    parentesco,
                    principal
                );
            }
            catch (SqlException ex)
            {
                MostrarMensagem(
                    "Erro ao associar o aluno: " +
                    ex.Message
                );

                return;
            }
            catch (Exception ex)
            {
                MostrarMensagem(
                    "Não foi possível associar o aluno: " +
                    ex.Message
                );

                return;
            }

            TxtParentesco.Text = string.Empty;
            ChkPrincipal.Checked = false;

            CarregarEducandos(encarregadoId);
            CarregarAlunosDisponiveis(encarregadoId);
            GetEncarregados();

            PnlEducandos.Visible = true;

            MostrarMensagem(
                "Aluno associado com sucesso.",
                false
            );
        }

        private void AssociarAluno(
            int encarregadoId,
            int alunoId,
            string parentesco,
            bool principal)
        {
            using (SqlConnection conn =
                new SqlConnection(_connectionString))
            {
                conn.Open();

                using (SqlTransaction transaction =
                    conn.BeginTransaction())
                {
                    try
                    {
                        if (principal)
                        {
                            DesmarcarPrincipalAtual(
                                alunoId,
                                conn,
                                transaction
                            );
                        }

                        const string atualizarAssociacao = @"
                            UPDATE dbo.AlunoEncarregado

                            SET
                                Parentesco = @Parentesco,
                                Principal = @Principal,
                                Ativo = 1

                            WHERE AlunoId = @AlunoId
                              AND EncarregadoEducacaoId =
                                  @EncarregadoEducacaoId;";

                        int linhas;

                        using (SqlCommand cmd =
                            new SqlCommand(
                                atualizarAssociacao,
                                conn,
                                transaction))
                        {
                            AdicionarParametrosAssociacao(
                                cmd,
                                encarregadoId,
                                alunoId,
                                parentesco,
                                principal
                            );

                            linhas =
                                cmd.ExecuteNonQuery();
                        }

                        if (linhas == 0)
                        {
                            const string inserirAssociacao = @"
                                INSERT INTO dbo.AlunoEncarregado
                                (
                                    AlunoId,
                                    EncarregadoEducacaoId,
                                    Parentesco,
                                    Principal,
                                    Ativo
                                )
                                VALUES
                                (
                                    @AlunoId,
                                    @EncarregadoEducacaoId,
                                    @Parentesco,
                                    @Principal,
                                    1
                                );";

                            using (SqlCommand cmd =
                                new SqlCommand(
                                    inserirAssociacao,
                                    conn,
                                    transaction))
                            {
                                AdicionarParametrosAssociacao(
                                    cmd,
                                    encarregadoId,
                                    alunoId,
                                    parentesco,
                                    principal
                                );

                                cmd.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        try
                        {
                            transaction.Rollback();
                        }
                        catch
                        {
                        }

                        throw;
                    }
                }
            }
        }

        private void AdicionarParametrosAssociacao(
            SqlCommand cmd,
            int encarregadoId,
            int alunoId,
            string parentesco,
            bool principal)
        {
            cmd.Parameters
                .Add(
                    "@EncarregadoEducacaoId",
                    SqlDbType.Int
                )
                .Value = encarregadoId;

            cmd.Parameters
                .Add(
                    "@AlunoId",
                    SqlDbType.Int
                )
                .Value = alunoId;

            cmd.Parameters
                .Add(
                    "@Parentesco",
                    SqlDbType.NVarChar,
                    50
                )
                .Value =
                ValorOuDbNull(parentesco);

            cmd.Parameters
                .Add(
                    "@Principal",
                    SqlDbType.Bit
                )
                .Value = principal;
        }

        private void DesmarcarPrincipalAtual(
            int alunoId,
            SqlConnection conn,
            SqlTransaction transaction)
        {
            const string sql = @"
                UPDATE dbo.AlunoEncarregado

                SET Principal = 0

                WHERE AlunoId = @AlunoId
                  AND Principal = 1;";

            using (SqlCommand cmd =
                new SqlCommand(
                    sql,
                    conn,
                    transaction))
            {
                cmd.Parameters
                    .Add(
                        "@AlunoId",
                        SqlDbType.Int
                    )
                    .Value = alunoId;

                cmd.ExecuteNonQuery();
            }
        }

        protected void GridEducandos_RowCommand(
            object sender,
            GridViewCommandEventArgs e)
        {
            if (e.CommandName != "RemoverAssociacao")
            {
                return;
            }

            int associacaoId;

            if (!int.TryParse(
                    Convert.ToString(e.CommandArgument),
                    out associacaoId))
            {
                MostrarMensagem(
                    "Não foi possível identificar a associação."
                );

                return;
            }

            int encarregadoId;

            if (!TryGetEncarregadoIdViewState(
                    out encarregadoId))
            {
                MostrarMensagem(
                    "Não foi possível identificar o encarregado."
                );

                return;
            }

            int agrupamentoId;

            if (!TryGetAgrupamentoId(out agrupamentoId))
            {
                Response.Redirect("~/login.aspx");
                return;
            }

            try
            {
                int linhas =
                    RemoverAssociacao(
                        associacaoId,
                        encarregadoId,
                        agrupamentoId
                    );

                if (linhas == 0)
                {
                    MostrarMensagem(
                        "A associação não existe ou já foi removida."
                    );

                    return;
                }
            }
            catch (SqlException ex)
            {
                MostrarMensagem(
                    "Erro ao remover a associação: " +
                    ex.Message
                );

                return;
            }

            CarregarEducandos(encarregadoId);
            CarregarAlunosDisponiveis(encarregadoId);
            GetEncarregados();

            PnlEducandos.Visible = true;

            MostrarMensagem(
                "Associação removida com sucesso.",
                false
            );
        }

        private int RemoverAssociacao(
            int associacaoId,
            int encarregadoId,
            int agrupamentoId)
        {
            const string sql = @"
                UPDATE ae

                SET
                    ae.Ativo = 0,
                    ae.Principal = 0

                FROM dbo.AlunoEncarregado ae

                INNER JOIN dbo.Aluno a
                    ON a.Id = ae.AlunoId

                WHERE ae.Id = @AssociacaoId
                  AND ae.EncarregadoEducacaoId =
                      @EncarregadoEducacaoId
                  AND a.AgrupamentoId =
                      @AgrupamentoId;";

            using (SqlConnection conn =
                new SqlConnection(_connectionString))
            using (SqlCommand cmd =
                new SqlCommand(sql, conn))
            {
                cmd.Parameters
                    .Add(
                        "@AssociacaoId",
                        SqlDbType.Int
                    )
                    .Value = associacaoId;

                cmd.Parameters
                    .Add(
                        "@EncarregadoEducacaoId",
                        SqlDbType.Int
                    )
                    .Value = encarregadoId;

                cmd.Parameters
                    .Add(
                        "@AgrupamentoId",
                        SqlDbType.Int
                    )
                    .Value = agrupamentoId;

                conn.Open();

                return cmd.ExecuteNonQuery();
            }
        }

        protected void ButtonFecharEducandos_Click(
            object sender,
            EventArgs e)
        {
            PnlEducandos.Visible = false;

            ViewState["EncarregadoId"] = null;

            TxtParentesco.Text = string.Empty;
            ChkPrincipal.Checked = false;

            LimparMensagem();
        }

        #endregion


        #region Listagem dos encarregados

        private void GetEncarregados()
        {
            int agrupamentoId;

            if (!TryGetAgrupamentoId(out agrupamentoId))
            {
                Response.Redirect("~/login.aspx");
                return;
            }

            const string sql = @"
                SELECT
                    e.Id,
                    e.NomeCompleto,
                    e.NIF,
                    e.Email,
                    e.Telefone,
                    e.Ativo,

                    COUNT
                    (
                        DISTINCT
                        CASE
                            WHEN ae.Ativo = 1
                            THEN ae.AlunoId
                            ELSE NULL
                        END
                    ) AS NumeroEducandos

                FROM dbo.EncarregadoEducacao e

                INNER JOIN dbo.AlunoEncarregado ae
                    ON ae.EncarregadoEducacaoId = e.Id

                INNER JOIN dbo.Aluno a
                    ON a.Id = ae.AlunoId

                WHERE a.AgrupamentoId = @AgrupamentoId

                GROUP BY
                    e.Id,
                    e.NomeCompleto,
                    e.NIF,
                    e.Email,
                    e.Telefone,
                    e.Ativo

                ORDER BY e.NomeCompleto;";

            DataTable tabela =
                new DataTable();

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

            GridEncarregados.DataSource =
                tabela;

            GridEncarregados.DataBind();
        }

        private EncarregadoDados GetEncarregadoById(
            int encarregadoId)
        {
            int agrupamentoId;

            if (!TryGetAgrupamentoId(out agrupamentoId))
            {
                return null;
            }

            const string sql = @"
                SELECT TOP 1
                    e.Id,
                    e.UserId,
                    e.NomeCompleto,
                    e.Email,
                    e.Telefone,
                    e.NIF,
                    e.Ativo

                FROM dbo.EncarregadoEducacao e

                WHERE e.Id = @EncarregadoEducacaoId

                  AND EXISTS
                  (
                      SELECT 1

                      FROM dbo.AlunoEncarregado ae

                      INNER JOIN dbo.Aluno a
                          ON a.Id = ae.AlunoId

                      WHERE ae.EncarregadoEducacaoId = e.Id
                        AND a.AgrupamentoId = @AgrupamentoId
                  );";

            using (SqlConnection conn =
                new SqlConnection(_connectionString))
            using (SqlCommand cmd =
                new SqlCommand(sql, conn))
            {
                cmd.Parameters
                    .Add(
                        "@EncarregadoEducacaoId",
                        SqlDbType.Int
                    )
                    .Value = encarregadoId;

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
                        return null;
                    }

                    return new EncarregadoDados
                    {
                        Id =
                            Convert.ToInt32(
                                reader["Id"]
                            ),

                        UserId =
                            (Guid)reader["UserId"],

                        NomeCompleto =
                            Convert.ToString(
                                reader["NomeCompleto"]
                            ),

                        Email =
                            Convert.ToString(
                                reader["Email"]
                            ),

                        Telefone =
                            ValorTexto(
                                reader["Telefone"]
                            ),

                        NIF =
                            ValorTexto(
                                reader["NIF"]
                            ),

                        Ativo =
                            Convert.ToBoolean(
                                reader["Ativo"]
                            )
                    };
                }
            }
        }

        private void CarregarFormulario(
            EncarregadoDados encarregado)
        {
            TxtNomeCompleto.Text =
                encarregado.NomeCompleto;

            TxtNIF.Text =
                encarregado.NIF;

            TxtEmail.Text =
                encarregado.Email;

            TxtTelefone.Text =
                encarregado.Telefone;

            ChkAtivo.Checked =
                encarregado.Ativo;

            TxtParentescoInicial.Text =
                string.Empty;

            ChkPrincipalInicial.Checked =
                false;
        }

        #endregion


        #region Listagem dos alunos

        private void CarregarAlunosDisponiveis(
            int encarregadoId)
        {
            int agrupamentoId;

            if (!TryGetAgrupamentoId(out agrupamentoId))
            {
                return;
            }

            CarregarAlunosDropDown(
                DdlAlunoAssociar,
                agrupamentoId,
                encarregadoId
            );
        }

        private void CarregarAlunosDropDown(
            DropDownList dropDown,
            int agrupamentoId,
            int? encarregadoId)
        {
            const string sql = @"
                SELECT
                    a.Id,

                    a.NomeCompleto +
                    CASE
                        WHEN a.NumeroProcesso IS NULL
                             OR LTRIM(RTRIM(a.NumeroProcesso)) = ''
                        THEN ''
                        ELSE
                            N' — Processo ' +
                            a.NumeroProcesso
                    END AS Descricao

                FROM dbo.Aluno a

                WHERE a.AgrupamentoId = @AgrupamentoId
                  AND a.Ativo = 1

                  AND
                  (
                      @EncarregadoEducacaoId IS NULL

                      OR NOT EXISTS
                      (
                          SELECT 1

                          FROM dbo.AlunoEncarregado ae

                          WHERE ae.AlunoId = a.Id
                            AND ae.EncarregadoEducacaoId =
                                @EncarregadoEducacaoId
                            AND ae.Ativo = 1
                      )
                  )

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
                cmd.Parameters
                    .Add(
                        "@AgrupamentoId",
                        SqlDbType.Int
                    )
                    .Value = agrupamentoId;

                cmd.Parameters
                    .Add(
                        "@EncarregadoEducacaoId",
                        SqlDbType.Int
                    )
                    .Value =
                    encarregadoId.HasValue
                        ? (object)encarregadoId.Value
                        : DBNull.Value;

                adapter.Fill(tabela);
            }

            dropDown.Items.Clear();

            dropDown.Items.Add(
                new ListItem(
                    "Selecione um aluno...",
                    ""
                )
            );

            dropDown.DataSource =
                tabela;

            dropDown.DataTextField =
                "Descricao";

            dropDown.DataValueField =
                "Id";

            dropDown.DataBind();
        }

        private void CarregarEducandos(
            int encarregadoId)
        {
            int agrupamentoId;

            if (!TryGetAgrupamentoId(out agrupamentoId))
            {
                return;
            }

            const string sql = @"
                SELECT
                    ae.Id,
                    a.NomeCompleto,
                    a.NumeroProcesso,
                    ae.Parentesco,
                    ae.Principal

                FROM dbo.AlunoEncarregado ae

                INNER JOIN dbo.Aluno a
                    ON a.Id = ae.AlunoId

                WHERE ae.EncarregadoEducacaoId =
                      @EncarregadoEducacaoId

                  AND ae.Ativo = 1
                  AND a.AgrupamentoId = @AgrupamentoId

                ORDER BY
                    ae.Principal DESC,
                    a.NomeCompleto;";

            DataTable tabela =
                new DataTable();

            using (SqlConnection conn =
                new SqlConnection(_connectionString))
            using (SqlCommand cmd =
                new SqlCommand(sql, conn))
            using (SqlDataAdapter adapter =
                new SqlDataAdapter(cmd))
            {
                cmd.Parameters
                    .Add(
                        "@EncarregadoEducacaoId",
                        SqlDbType.Int
                    )
                    .Value = encarregadoId;

                cmd.Parameters
                    .Add(
                        "@AgrupamentoId",
                        SqlDbType.Int
                    )
                    .Value = agrupamentoId;

                adapter.Fill(tabela);
            }

            GridEducandos.DataSource =
                tabela;

            GridEducandos.DataBind();
        }

        private bool AlunoPertenceAoAgrupamento(
            int alunoId,
            int agrupamentoId)
        {
            const string sql = @"
                SELECT
                    CASE
                        WHEN EXISTS
                        (
                            SELECT 1

                            FROM dbo.Aluno

                            WHERE Id = @AlunoId
                              AND AgrupamentoId =
                                  @AgrupamentoId
                              AND Ativo = 1
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
                        "@AlunoId",
                        SqlDbType.Int
                    )
                    .Value = alunoId;

                cmd.Parameters
                    .Add(
                        "@AgrupamentoId",
                        SqlDbType.Int
                    )
                    .Value = agrupamentoId;

                conn.Open();

                return Convert.ToInt32(
                    cmd.ExecuteScalar()
                ) == 1;
            }
        }

        #endregion


        #region Validações de duplicação

        private bool NifJaExiste(
            string nif,
            int? encarregadoIdIgnorar)
        {
            const string sql = @"
                SELECT
                    CASE

                        WHEN EXISTS
                        (
                            SELECT 1

                            FROM dbo.Aluno

                            WHERE NIF = @NIF
                        )

                        OR EXISTS
                        (
                            SELECT 1

                            FROM dbo.Professor

                            WHERE NIF = @NIF
                        )

                        OR EXISTS
                        (
                            SELECT 1

                            FROM dbo.EncarregadoEducacao

                            WHERE NIF = @NIF

                              AND
                              (
                                  @EncarregadoIdIgnorar IS NULL
                                  OR Id <> @EncarregadoIdIgnorar
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
                        "@EncarregadoIdIgnorar",
                        SqlDbType.Int
                    )
                    .Value =
                    encarregadoIdIgnorar.HasValue
                        ? (object)encarregadoIdIgnorar.Value
                        : DBNull.Value;

                conn.Open();

                return Convert.ToInt32(
                    cmd.ExecuteScalar()
                ) == 1;
            }
        }

        private bool EmailJaExisteNaTabela(
            string email,
            int? encarregadoIdIgnorar)
        {
            const string sql = @"
                SELECT
                    CASE
                        WHEN EXISTS
                        (
                            SELECT 1

                            FROM dbo.EncarregadoEducacao

                            WHERE Email = @Email

                              AND
                              (
                                  @EncarregadoIdIgnorar IS NULL
                                  OR Id <> @EncarregadoIdIgnorar
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
                        "@Email",
                        SqlDbType.NVarChar,
                        150
                    )
                    .Value = email;

                cmd.Parameters
                    .Add(
                        "@EncarregadoIdIgnorar",
                        SqlDbType.Int
                    )
                    .Value =
                    encarregadoIdIgnorar.HasValue
                        ? (object)encarregadoIdIgnorar.Value
                        : DBNull.Value;

                conn.Open();

                return Convert.ToInt32(
                    cmd.ExecuteScalar()
                ) == 1;
            }
        }

        private bool EmailJaExisteNoMembership(
            string email,
            Guid? userIdIgnorar)
        {
            string username =
                Membership.GetUserNameByEmail(
                    email
                );

            if (string.IsNullOrWhiteSpace(username))
            {
                return false;
            }

            if (!userIdIgnorar.HasValue)
            {
                return true;
            }

            MembershipUser utilizador =
                Membership.GetUser(
                    username,
                    false
                );

            if (utilizador == null ||
                utilizador.ProviderUserKey == null)
            {
                return true;
            }

            Guid userIdEncontrado;

            if (!Guid.TryParse(
                    utilizador.ProviderUserKey.ToString(),
                    out userIdEncontrado))
            {
                return true;
            }

            return userIdEncontrado !=
                   userIdIgnorar.Value;
        }

        #endregion


        #region Agrupamento e sessão

        private bool TryGetAgrupamentoId(
            out int agrupamentoId)
        {
            agrupamentoId = 0;

            if (Session["AgrupamentoID"] != null &&
                int.TryParse(
                    Session["AgrupamentoID"].ToString(),
                    out agrupamentoId))
            {
                return true;
            }

            if (Session["UserId"] == null)
            {
                return false;
            }

            Guid userId;

            if (!Guid.TryParse(
                    Session["UserId"].ToString(),
                    out userId))
            {
                return false;
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
                    return false;
                }

                agrupamentoId =
                    Convert.ToInt32(resultado);

                Session["AgrupamentoID"] =
                    agrupamentoId;

                return true;
            }
        }

        #endregion


        #region Utilidades

        private bool EncarregadoSelecionado(
            out int encarregadoId)
        {
            encarregadoId = 0;

            if (GridEncarregados.SelectedDataKey == null ||
                GridEncarregados.SelectedDataKey.Value == null)
            {
                return false;
            }

            return int.TryParse(
                GridEncarregados
                    .SelectedDataKey
                    .Value
                    .ToString(),
                out encarregadoId
            );
        }

        private bool TryGetEncarregadoIdViewState(
            out int encarregadoId)
        {
            encarregadoId = 0;

            if (ViewState["EncarregadoId"] == null)
            {
                return false;
            }

            return int.TryParse(
                ViewState["EncarregadoId"].ToString(),
                out encarregadoId
            );
        }

        private void LimparFormulario()
        {
            TxtNomeCompleto.Text = string.Empty;
            TxtNIF.Text = string.Empty;
            TxtEmail.Text = string.Empty;
            TxtTelefone.Text = string.Empty;

            TxtParentescoInicial.Text =
                string.Empty;

            TxtParentesco.Text =
                string.Empty;

            ChkAtivo.Checked = true;
            ChkPrincipalInicial.Checked = true;
            ChkPrincipal.Checked = false;

            DdlAlunoInicial.Items.Clear();
            DdlAlunoAssociar.Items.Clear();
        }

        private object ValorOuDbNull(
            string valor)
        {
            return string.IsNullOrWhiteSpace(valor)
                ? (object)DBNull.Value
                : valor.Trim();
        }

        private string ValorTexto(
            object valor)
        {
            return valor == null ||
                   valor == DBNull.Value
                ? string.Empty
                : valor.ToString();
        }

        private void MostrarMensagem(
            string mensagem,
            bool erro = true)
        {
            LblMensagem.Visible = true;
            LblMensagem.Text = mensagem;

            LblMensagem.CssClass =
                erro
                    ? "alert alert-warning d-block"
                    : "alert alert-success d-block";
        }

        private void LimparMensagem()
        {
            LblMensagem.Visible = false;
            LblMensagem.Text = string.Empty;
            LblMensagem.CssClass = string.Empty;
        }

        #endregion


        #region Classe auxiliar

        private class EncarregadoDados
        {
            public int Id { get; set; }

            public Guid UserId { get; set; }

            public string NomeCompleto { get; set; }

            public string Email { get; set; }

            public string Telefone { get; set; }

            public string NIF { get; set; }

            public bool Ativo { get; set; }
        }

        #endregion
    }
}