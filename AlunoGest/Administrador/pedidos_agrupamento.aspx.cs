using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Net.Configuration;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AlunoGest.administrador
{
    public partial class pedidos_agrupamento : Page
    {
        private readonly string connectionString =
            ConfigurationManager
                .ConnectionStrings["DefaultConnection"]
                .ConnectionString;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            btnAtualizar.Click += btnAtualizar_Click;
            GridPedidos.RowCommand += GridPedidos_RowCommand;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Request.IsAuthenticated ||
                !Roles.IsUserInRole(
                    User.Identity.Name,
                    "administrador"
                ))
            {
                FormsAuthentication.SignOut();
                Session.Clear();

                Response.Redirect("~/login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                CarregarPedidos();
            }
        }

        private void CarregarPedidos()
        {
            const string sql = @"
                SELECT
                    IdPedido,
                    NomeAgrupamento,
                    NomeResponsavel,
                    Email,
                    Telefone,
                    NIF,
                    Localidade,
                    Estado,
                    DataPedido
                FROM dbo.PedidoAgrupamento
                WHERE Estado = N'Pendente'
                ORDER BY DataPedido ASC;";

            try
            {
                DataTable tabela = new DataTable();

                using (
                    SqlConnection conn =
                        new SqlConnection(connectionString))
                using (
                    SqlCommand cmd =
                        new SqlCommand(sql, conn))
                using (
                    SqlDataAdapter adapter =
                        new SqlDataAdapter(cmd))
                {
                    adapter.Fill(tabela);
                }

                GridPedidos.DataSource = tabela;
                GridPedidos.DataBind();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError(
                    "Erro ao carregar pedidos: " + ex
                );

                MostrarErro(
                    "Não foi possível carregar os pedidos de agrupamento."
                );
            }
        }

        protected void btnAtualizar_Click(
            object sender,
            EventArgs e)
        {
            lblMensagem.Visible = false;

            CarregarPedidos();
        }

        protected void GridPedidos_RowCommand(
            object sender,
            GridViewCommandEventArgs e)
        {
            if (e.CommandName != "AprovarPedido" &&
                e.CommandName != "RejeitarPedido")
            {
                return;
            }

            int idPedido;

            if (!int.TryParse(
                Convert.ToString(e.CommandArgument),
                out idPedido))
            {
                MostrarErro(
                    "Não foi possível identificar o pedido selecionado."
                );

                return;
            }

            if (e.CommandName == "AprovarPedido")
            {
                AprovarPedido(idPedido);
                return;
            }

            AbrirJanelaRejeicao(idPedido);
        }
        private void AbrirJanelaRejeicao(int idPedido)
        {
            PedidoAgrupamento pedido =
                ObterPedidoPendente(idPedido);

            if (pedido == null)
            {
                MostrarErro(
                    "O pedido não existe ou já foi analisado."
                );

                CarregarPedidos();
                return;
            }

            hdnPedidoRejeitar.Value =
                idPedido.ToString();

            txtMotivoRejeicao.Text =
                string.Empty;

            pnlRejeicao.Visible = true;
        }

        protected void btnCancelarRejeicao_Click(
            object sender,
            EventArgs e)
        {
            FecharJanelaRejeicao();
        }

        protected void btnConfirmarRejeicao_Click(
            object sender,
            EventArgs e)
        {
            Page.Validate("rejeicao");

            if (!Page.IsValid)
            {
                pnlRejeicao.Visible = true;
                return;
            }

            int idPedido;

            if (!int.TryParse(
                hdnPedidoRejeitar.Value,
                out idPedido))
            {
                FecharJanelaRejeicao();

                MostrarErro(
                    "Não foi possível identificar o pedido."
                );

                return;
            }

            string motivo =
                txtMotivoRejeicao.Text.Trim();

            if (string.IsNullOrWhiteSpace(motivo))
            {
                pnlRejeicao.Visible = true;

                MostrarErro(
                    "Indique o motivo da rejeição."
                );

                return;
            }

            if (motivo.Length > 500)
            {
                pnlRejeicao.Visible = true;

                MostrarErro(
                    "O motivo não pode ultrapassar 500 caracteres."
                );

                return;
            }

            PedidoAgrupamento pedido =
                ObterPedidoPendente(idPedido);

            if (pedido == null)
            {
                FecharJanelaRejeicao();

                MostrarErro(
                    "O pedido não existe ou já foi analisado."
                );

                CarregarPedidos();
                return;
            }

            MembershipUser administrador =
                Membership.GetUser(
                    User.Identity.Name,
                    false
                );

            if (administrador == null)
            {
                pnlRejeicao.Visible = true;

                MostrarErro(
                    "Não foi possível identificar o administrador."
                );

                return;
            }

            Guid administradorUserId =
                new Guid(
                    administrador
                        .ProviderUserKey
                        .ToString()
                );

            try
            {
                bool rejeitado =
                    MarcarPedidoComoRejeitado(
                        idPedido,
                        motivo,
                        administradorUserId
                    );

                if (!rejeitado)
                {
                    FecharJanelaRejeicao();

                    MostrarErro(
                        "O pedido já foi analisado por outro administrador."
                    );

                    CarregarPedidos();
                    return;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError(
                    "Erro ao rejeitar pedido de agrupamento: " +
                    ex
                );

                pnlRejeicao.Visible = true;

                MostrarErro(
                    "Não foi possível rejeitar o pedido."
                );

                return;
            }

            bool emailEnviado =
                EnviarEmailRejeicao(
                    pedido.Email,
                    pedido.NomeResponsavel,
                    pedido.NomeAgrupamento,
                    motivo
                );

            FecharJanelaRejeicao();
            CarregarPedidos();

            if (emailEnviado)
            {
                MostrarSucesso(
                    "O pedido foi rejeitado e o motivo foi enviado para " +
                    pedido.Email +
                    "."
                );
            }
            else
            {
                MostrarAviso(
                    "O pedido foi rejeitado, mas não foi possível " +
                    "enviar o email ao responsável."
                );
            }
        }

        private bool MarcarPedidoComoRejeitado(
            int idPedido,
            string motivo,
            Guid administradorUserId)
        {
            const string sql = @"
        UPDATE dbo.PedidoAgrupamento

        SET
            Estado = N'Rejeitado',
            DataDecisao = SYSDATETIME(),
            ObservacaoAdministrador = @Motivo,
            AdministradorUserId = @AdministradorUserId,
            AgrupamentoId = NULL

        WHERE IdPedido = @IdPedido
          AND Estado = N'Pendente';";

            using (
                SqlConnection conn =
                    new SqlConnection(connectionString))
            using (
                SqlCommand cmd =
                    new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add(
                    "@Motivo",
                    SqlDbType.NVarChar,
                    500
                ).Value = motivo;

                cmd.Parameters.Add(
                    "@AdministradorUserId",
                    SqlDbType.UniqueIdentifier
                ).Value = administradorUserId;

                cmd.Parameters.Add(
                    "@IdPedido",
                    SqlDbType.Int
                ).Value = idPedido;

                conn.Open();

                int linhasAlteradas =
                    cmd.ExecuteNonQuery();

                return linhasAlteradas == 1;
            }
        }

        private bool EnviarEmailRejeicao(
            string emailDestino,
            string nomeResponsavel,
            string nomeAgrupamento,
            string motivo)
        {
            try
            {
                SmtpSection configuracaoSmtp =
                    ConfigurationManager.GetSection(
                        "system.net/mailSettings/smtp"
                    ) as SmtpSection;

                string emailRemetente =
                    configuracaoSmtp?.From;

                if (string.IsNullOrWhiteSpace(emailRemetente) &&
                    configuracaoSmtp?.Network != null)
                {
                    emailRemetente =
                        configuracaoSmtp.Network.UserName;
                }

                if (string.IsNullOrWhiteSpace(emailRemetente))
                {
                    throw new InvalidOperationException(
                        "O email remetente não está configurado."
                    );
                }

                string responsavelSeguro =
                    HttpUtility.HtmlEncode(
                        nomeResponsavel
                    );

                string agrupamentoSeguro =
                    HttpUtility.HtmlEncode(
                        nomeAgrupamento
                    );

                string motivoSeguro =
                    HttpUtility.HtmlEncode(
                        motivo
                    )
                    .Replace("\r\n", "<br />")
                    .Replace("\n", "<br />");

                using (
                    MailMessage mensagem =
                        new MailMessage())
                {
                    mensagem.From =
                        new MailAddress(
                            emailRemetente,
                            "Inovar Inovado"
                        );

                    mensagem.To.Add(emailDestino);

                    mensagem.Subject =
                        "Pedido de agrupamento rejeitado";

                    mensagem.IsBodyHtml = true;

                    mensagem.Body = @"
                <div style=""
                    max-width:600px;
                    margin:0 auto;
                    padding:24px;
                    font-family:Segoe UI,Arial,sans-serif;
                    color:#1f2937;
                    line-height:1.6;
                "">

                    <div style=""
                        padding:24px;
                        border:1px solid #fecaca;
                        border-radius:14px;
                        background:#fffafa;
                    "">

                        <h2 style=""
                            margin-top:0;
                            color:#991b1b;
                        "">
                            Pedido não aprovado
                        </h2>

                        <p>
                            Olá,
                            <strong>" +
                                    responsavelSeguro +
                                    @"</strong>.
                        </p>

                        <p>
                            O pedido de criação do agrupamento
                            <strong>" +
                                    agrupamentoSeguro +
                                    @"</strong>
                            foi analisado pela administração.
                        </p>

                        <p>
                            Neste momento, o pedido não pôde ser aprovado.
                        </p>

                        <div style=""
                            margin:20px 0;
                            padding:16px;
                            border-radius:10px;
                            background:#fef2f2;
                            border:1px solid #fecaca;
                        "">

                            <strong style=""color:#991b1b;"">
                                Motivo:
                            </strong>

                            <div style=""margin-top:7px;"">
                                " + motivoSeguro + @"
                            </div>

                        </div>

                        <p>
                            Poderá corrigir as informações indicadas
                            e enviar um novo pedido.
                        </p>

                        <p style=""
                            margin-top:24px;
                            color:#64748b;
                            font-size:13px;
                        "">
                            Inovar Inovado —
                            Plataforma de Gestão Escolar
                        </p>

                    </div>

                </div>";

                    using (
                        SmtpClient smtp =
                            new SmtpClient())
                    {
                        smtp.Send(mensagem);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError(
                    "Erro ao enviar email de rejeição: " +
                    ex
                );

                return false;
            }
        }

        private void FecharJanelaRejeicao()
        {
            pnlRejeicao.Visible = false;

            hdnPedidoRejeitar.Value =
                string.Empty;

            txtMotivoRejeicao.Text =
                string.Empty;
        }
        private void AprovarPedido(int idPedido)
        {
            lblMensagem.Visible = false;

            PedidoAgrupamento pedido =
                ObterPedidoPendente(idPedido);

            if (pedido == null)
            {
                MostrarErro(
                    "O pedido não existe ou já foi analisado."
                );

                CarregarPedidos();
                return;
            }

            /*
             * Confirma novamente que o email não pertence
             * a nenhuma conta já existente.
             */
            string utilizadorDoEmail =
                Membership.GetUserNameByEmail(pedido.Email);

            if (!string.IsNullOrWhiteSpace(utilizadorDoEmail))
            {
                MostrarErro(
                    "Já existe uma conta associada ao email deste pedido."
                );

                return;
            }

            MembershipUser administrador =
                Membership.GetUser(
                    User.Identity.Name,
                    false
                );

            if (administrador == null)
            {
                MostrarErro(
                    "Não foi possível identificar o administrador."
                );

                return;
            }

            Guid administradorUserId =
                new Guid(
                    administrador.ProviderUserKey.ToString()
                );

            string username =
                GerarUsername(
                    pedido.NomeAgrupamento,
                    pedido.IdPedido
                );

            string password =
                GerarPasswordTemporaria();

            MembershipCreateStatus status;

            MembershipUser novoUtilizador =
                Membership.CreateUser(
                    username,
                    password,
                    pedido.Email,
                    null,
                    null,
                    true,
                    out status
                );

            if (status != MembershipCreateStatus.Success ||
                novoUtilizador == null)
            {
                MostrarErro(
                    ObterMensagemMembership(status)
                );

                return;
            }

            try
            {
                Roles.AddUserToRole(
                    username,
                    "agrupamento"
                );

                Guid novoUserId =
                    new Guid(
                        novoUtilizador
                            .ProviderUserKey
                            .ToString()
                    );

                CriarAgrupamentoEAprovarPedido(
                    pedido,
                    novoUserId,
                    administradorUserId
                );
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError(
                    "Erro ao criar conta do agrupamento: " +
                    ex
                );

                RemoverUtilizadorCriado(username);

                MostrarErro(
                    "Não foi possível concluir a aprovação. " +
                    "Nenhuma conta foi mantida."
                );

                CarregarPedidos();
                return;
            }

            bool emailEnviado =
                EnviarEmailAprovacao(
                    pedido.Email,
                    pedido.NomeResponsavel,
                    pedido.NomeAgrupamento,
                    username,
                    password
                );

            CarregarPedidos();

            if (emailEnviado)
            {
                MostrarSucesso(
                    "O pedido foi aprovado. A conta foi criada " +
                    "e as credenciais foram enviadas para " +
                    pedido.Email +
                    "."
                );
            }
            else
            {
                MostrarAviso(
                    "O pedido foi aprovado e a conta foi criada, " +
                    "mas não foi possível enviar o email com as credenciais."
                );
            }
        }

        private PedidoAgrupamento ObterPedidoPendente(
            int idPedido)
        {
            const string sql = @"
                SELECT
                    IdPedido,
                    NomeAgrupamento,
                    NomeResponsavel,
                    Email,
                    Telefone,
                    NIF,
                    Localidade,
                    Morada,
                    CodigoPostal,
                    CodigoMEC
                FROM dbo.PedidoAgrupamento
                WHERE IdPedido = @IdPedido
                  AND Estado = N'Pendente';";

            using (
                SqlConnection conn =
                    new SqlConnection(connectionString))
            using (
                SqlCommand cmd =
                    new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add(
                    "@IdPedido",
                    SqlDbType.Int
                ).Value = idPedido;

                conn.Open();

                using (
                    SqlDataReader reader =
                        cmd.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        return null;
                    }

                    return new PedidoAgrupamento
                    {
                        IdPedido =
                            Convert.ToInt32(
                                reader["IdPedido"]
                            ),

                        NomeAgrupamento =
                            Convert.ToString(
                                reader["NomeAgrupamento"]
                            ),

                        NomeResponsavel =
                            Convert.ToString(
                                reader["NomeResponsavel"]
                            ),

                        Email =
                            Convert.ToString(
                                reader["Email"]
                            ).Trim(),

                        Telefone =
                            Convert.ToString(
                                reader["Telefone"]
                            ).Trim(),

                        NIF =
                            Convert.ToString(
                                reader["NIF"]
                            ).Trim(),

                        Localidade =
                            Convert.ToString(
                                reader["Localidade"]
                            ).Trim(),

                        Morada =
                            Convert.ToString(
                                reader["Morada"]
                            ).Trim(),

                        CodigoPostal =
                            Convert.ToString(
                                reader["CodigoPostal"]
                            ).Trim(),

                        CodigoMEC =
                            reader["CodigoMEC"] == DBNull.Value
                                ? null
                                : Convert.ToString(
                                    reader["CodigoMEC"]
                                ).Trim()
                    };
                }
            }
        }

        private void CriarAgrupamentoEAprovarPedido(
            PedidoAgrupamento pedido,
            Guid novoUserId,
            Guid administradorUserId)
        {
            using (
                SqlConnection conn =
                    new SqlConnection(connectionString))
            {
                conn.Open();

                using (
                    SqlTransaction transaction =
                        conn.BeginTransaction())
                {
                    try
                    {
                        const string inserirAgrupamento = @"
                            INSERT INTO dbo.Agrupamento
                            (
                                UserId,
                                Nome,
                                CodigoMEC,
                                Morada,
                                CodigoPostal,
                                Localidade,
                                Email,
                                Telefone
                            )
                            VALUES
                            (
                                @UserId,
                                @Nome,
                                @CodigoMEC,
                                @Morada,
                                @CodigoPostal,
                                @Localidade,
                                @Email,
                                @Telefone
                            );

                            SELECT
                                CAST(SCOPE_IDENTITY() AS INT);";

                        int agrupamentoId;

                        using (
                            SqlCommand cmd =
                                new SqlCommand(
                                    inserirAgrupamento,
                                    conn,
                                    transaction
                                ))
                        {
                            cmd.Parameters.Add(
                                "@UserId",
                                SqlDbType.UniqueIdentifier
                            ).Value = novoUserId;

                            cmd.Parameters.Add(
                                "@Nome",
                                SqlDbType.NVarChar,
                                200
                            ).Value = pedido.NomeAgrupamento;

                            cmd.Parameters.Add(
                                "@CodigoMEC",
                                SqlDbType.NVarChar,
                                20
                            ).Value =
                                string.IsNullOrWhiteSpace(
                                    pedido.CodigoMEC
                                )
                                    ? (object)DBNull.Value
                                    : pedido.CodigoMEC;

                            cmd.Parameters.Add(
                                "@Morada",
                                SqlDbType.NVarChar,
                                300
                            ).Value = pedido.Morada;

                            cmd.Parameters.Add(
                                "@CodigoPostal",
                                SqlDbType.Char,
                                8
                            ).Value = pedido.CodigoPostal;

                            cmd.Parameters.Add(
                                "@Localidade",
                                SqlDbType.NVarChar,
                                100
                            ).Value = pedido.Localidade;

                            cmd.Parameters.Add(
                                "@Email",
                                SqlDbType.NVarChar,
                                150
                            ).Value = pedido.Email;

                            cmd.Parameters.Add(
                                "@Telefone",
                                SqlDbType.NVarChar,
                                30
                            ).Value = pedido.Telefone;

                            agrupamentoId =
                                Convert.ToInt32(
                                    cmd.ExecuteScalar()
                                );
                        }

                        const string aprovarPedido = @"
                            UPDATE dbo.PedidoAgrupamento

                            SET
                                Estado = N'Aprovado',
                                DataDecisao = SYSDATETIME(),
                                AdministradorUserId =
                                    @AdministradorUserId,
                                AgrupamentoId =
                                    @AgrupamentoId

                            WHERE IdPedido = @IdPedido
                              AND Estado = N'Pendente';";

                        using (
                            SqlCommand cmd =
                                new SqlCommand(
                                    aprovarPedido,
                                    conn,
                                    transaction
                                ))
                        {
                            cmd.Parameters.Add(
                                "@AdministradorUserId",
                                SqlDbType.UniqueIdentifier
                            ).Value = administradorUserId;

                            cmd.Parameters.Add(
                                "@AgrupamentoId",
                                SqlDbType.Int
                            ).Value = agrupamentoId;

                            cmd.Parameters.Add(
                                "@IdPedido",
                                SqlDbType.Int
                            ).Value = pedido.IdPedido;

                            int linhas =
                                cmd.ExecuteNonQuery();

                            if (linhas != 1)
                            {
                                throw new InvalidOperationException(
                                    "O pedido já foi analisado."
                                );
                            }
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        private string GerarUsername(
            string nomeAgrupamento,
            int idPedido)
        {
            string textoNormalizado =
                nomeAgrupamento.Normalize(
                    NormalizationForm.FormD
                );

            StringBuilder username =
                new StringBuilder();

            bool ultimoFoiPonto = false;

            foreach (char caractere in textoNormalizado)
            {
                UnicodeCategory categoria =
                    CharUnicodeInfo.GetUnicodeCategory(
                        caractere
                    );

                if (categoria ==
                    UnicodeCategory.NonSpacingMark)
                {
                    continue;
                }

                if (char.IsLetterOrDigit(caractere))
                {
                    username.Append(
                        char.ToLowerInvariant(caractere)
                    );

                    ultimoFoiPonto = false;
                }
                else if (
                    username.Length > 0 &&
                    !ultimoFoiPonto)
                {
                    username.Append('.');
                    ultimoFoiPonto = true;
                }
            }

            string usernameBase =
                username
                    .ToString()
                    .Trim('.');

            if (string.IsNullOrWhiteSpace(usernameBase))
            {
                usernameBase = "agrupamento";
            }

            const string identificador = ".agr";

            int tamanhoMaximoBase =
                50 - identificador.Length;

            if (usernameBase.Length >
                tamanhoMaximoBase)
            {
                usernameBase =
                    usernameBase.Substring(
                        0,
                        tamanhoMaximoBase
                    ).Trim('.');
            }

            string resultado =
                usernameBase +
                identificador;

            if (Membership.GetUser(
                resultado,
                false) == null)
            {
                return resultado;
            }

            string sufixo =
                "." + idPedido;

            tamanhoMaximoBase =
                50 -
                identificador.Length -
                sufixo.Length;

            if (usernameBase.Length >
                tamanhoMaximoBase)
            {
                usernameBase =
                    usernameBase.Substring(
                        0,
                        tamanhoMaximoBase
                    ).Trim('.');
            }

            return
                usernameBase +
                identificador +
                sufixo;
        }

        private string GerarPasswordTemporaria()
        {
            int tamanho =
                Math.Max(
                    Membership.MinRequiredPasswordLength,
                    12
                );

            int caracteresEspeciais =
                Math.Max(
                    Membership
                        .MinRequiredNonAlphanumericCharacters,
                    2
                );

            return Membership.GeneratePassword(
                tamanho,
                caracteresEspeciais
            );
        }

        private bool EnviarEmailAprovacao(
            string emailDestino,
            string nomeResponsavel,
            string nomeAgrupamento,
            string username,
            string password)
        {
            try
            {
                SmtpSection configuracaoSmtp =
                    ConfigurationManager.GetSection(
                        "system.net/mailSettings/smtp"
                    ) as SmtpSection;

                string emailRemetente =
                    configuracaoSmtp?.From;

                if (string.IsNullOrWhiteSpace(emailRemetente) &&
                    configuracaoSmtp?.Network != null)
                {
                    emailRemetente =
                        configuracaoSmtp.Network.UserName;
                }

                if (string.IsNullOrWhiteSpace(emailRemetente))
                {
                    throw new InvalidOperationException(
                        "O remetente não está configurado."
                    );
                }

                string responsavelSeguro =
                    HttpUtility.HtmlEncode(
                        nomeResponsavel
                    );

                string agrupamentoSeguro =
                    HttpUtility.HtmlEncode(
                        nomeAgrupamento
                    );

                string usernameSeguro =
                    HttpUtility.HtmlEncode(
                        username
                    );

                string passwordSegura =
                    HttpUtility.HtmlEncode(
                        password
                    );

                using (
                    MailMessage mensagem =
                        new MailMessage())
                {
                    mensagem.From =
                        new MailAddress(
                            emailRemetente,
                            "Inovar Inovado"
                        );

                    mensagem.To.Add(emailDestino);

                    mensagem.Subject =
                        "Pedido de agrupamento aprovado";

                    mensagem.IsBodyHtml = true;

                    mensagem.Body = @"
                        <div style=""
                            max-width:600px;
                            margin:0 auto;
                            padding:24px;
                            font-family:Segoe UI,Arial,sans-serif;
                            color:#1f2937;
                            line-height:1.6;
                        "">

                            <div style=""
                                padding:24px;
                                border:1px solid #dbeafe;
                                border-radius:14px;
                                background:#f8fafc;
                            "">

                                <h2 style=""
                                    margin-top:0;
                                    color:#123570;
                                "">
                                    Pedido aprovado
                                </h2>

                                <p>
                                    Olá,
                                    <strong>" +
                                    responsavelSeguro +
                                    @"</strong>.
                                </p>

                                <p>
                                    O pedido de criação do agrupamento
                                    <strong>" +
                                    agrupamentoSeguro +
                                    @"</strong>
                                    foi aprovado.
                                </p>

                                <div style=""
                                    margin:20px 0;
                                    padding:16px;
                                    border-radius:10px;
                                    background:#eff6ff;
                                "">

                                    <p style=""margin:0 0 8px;"">
                                        <strong>
                                            Nome de utilizador:
                                        </strong>

                                        " + usernameSeguro + @"
                                    </p>

                                    <p style=""margin:0;"">
                                        <strong>
                                            Palavra-passe:
                                        </strong>

                                        " + passwordSegura + @"
                                    </p>

                                </div>

                                <p>
                                    Utilize estas credenciais para iniciar
                                    sessão na plataforma.
                                </p>

                                <p style=""
                                    margin-top:24px;
                                    color:#64748b;
                                    font-size:13px;
                                "">
                                    Inovar Inovado —
                                    Plataforma de Gestão Escolar
                                </p>

                            </div>

                        </div>";

                    using (
                        SmtpClient smtp =
                            new SmtpClient())
                    {
                        smtp.Send(mensagem);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError(
                    "Erro ao enviar credenciais: " +
                    ex
                );

                return false;
            }
        }

        private void RemoverUtilizadorCriado(
            string username)
        {
            try
            {
                if (Roles.IsUserInRole(
                    username,
                    "agrupamento"))
                {
                    Roles.RemoveUserFromRole(
                        username,
                        "agrupamento"
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
                    "Erro ao remover utilizador incompleto: " +
                    ex
                );
            }
        }

        private string ObterMensagemMembership(
            MembershipCreateStatus status)
        {
            switch (status)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return
                        "Já existe uma conta com o nome de utilizador gerado.";

                case MembershipCreateStatus.DuplicateEmail:
                    return
                        "Já existe uma conta associada ao email do pedido.";

                case MembershipCreateStatus.InvalidPassword:
                    return
                        "Não foi possível gerar uma palavra-passe válida.";

                case MembershipCreateStatus.InvalidEmail:
                    return
                        "O email do pedido não é válido.";

                case MembershipCreateStatus.InvalidUserName:
                    return
                        "Não foi possível gerar um nome de utilizador válido.";

                case MembershipCreateStatus.UserRejected:
                    return
                        "A criação da conta foi recusada pelo sistema.";

                default:
                    return
                        "Não foi possível criar a conta do agrupamento.";
            }
        }

        private void MostrarSucesso(string mensagem)
        {
            lblMensagem.Text = mensagem;
            lblMensagem.CssClass = "message-success";
            lblMensagem.Visible = true;
        }

        private void MostrarErro(string mensagem)
        {
            lblMensagem.Text = mensagem;
            lblMensagem.CssClass = "message-error";
            lblMensagem.Visible = true;
        }

        private void MostrarAviso(string mensagem)
        {
            lblMensagem.Text = mensagem;
            lblMensagem.CssClass = "message-warning";
            lblMensagem.Visible = true;
        }

        private class PedidoAgrupamento
        {
            public int IdPedido { get; set; }

            public string NomeAgrupamento { get; set; }

            public string NomeResponsavel { get; set; }

            public string Email { get; set; }

            public string Telefone { get; set; }

            public string NIF { get; set; }

            public string Localidade { get; set; }

            public string Morada { get; set; }

            public string CodigoPostal { get; set; }

            public string CodigoMEC { get; set; }
        }
    }
}