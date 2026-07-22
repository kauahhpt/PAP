using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Net.Configuration;
using System.Net.Mail;
using System.Web;
using System.Web.Security;
using System.Web.UI;

namespace AlunoGest
{
    public partial class criarconta : Page
    {
        private readonly string connectionString =
            ConfigurationManager
                .ConnectionStrings["DefaultConnection"]
                .ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            /*
             * Esta página agora é pública.
             * Não deve verificar se o utilizador é administrador.
             */
        }

        protected void btnCriarConta_Click(
            object sender,
            EventArgs e)
        {
            lblMensagem.Visible = false;

            if (!Page.IsValid)
            {
                return;
            }

            string nomeAgrupamento =
                txtNome.Text.Trim();

            string nomeResponsavel =
                txtNomeResponsavel.Text.Trim();

            string email =
                txtEmail.Text
                    .Trim()
                    .ToLowerInvariant();

            string telefone =
                txtTelefone.Text.Trim();

            string nif =
                txtNIF.Text.Trim();

            string morada =
                txtMorada.Text.Trim();

            string codigoPostal =
                txtCodigoPostal.Text.Trim();

            string localidade =
                txtLocalidade.Text.Trim();

            string codigoMec =
                txtCodigoMEC.Text.Trim();

            try
            {
                /*
                 * Impede que seja usado um email que já pertence
                 * a uma conta existente no Membership.
                 */
                if (EmailJaPossuiConta(email))
                {
                    MostrarErro(
                        "Já existe uma conta associada a este email."
                    );

                    return;
                }

                /*
                 * Impede vários pedidos pendentes com o mesmo
                 * email ou o mesmo NIF.
                 */
                string problemaPedido =
                    VerificarPedidoPendente(
                        email,
                        nif
                    );

                if (!string.IsNullOrWhiteSpace(problemaPedido))
                {
                    MostrarErro(problemaPedido);
                    return;
                }

                InserirPedido(
                    nomeAgrupamento,
                    nomeResponsavel,
                    email,
                    telefone,
                    nif,
                    morada,
                    codigoPostal,
                    localidade,
                    codigoMec
                );

                bool emailEnviado =
                    EnviarEmailConfirmacao(
                        email,
                        nomeResponsavel,
                        nomeAgrupamento
                    );

                LimparCampos();

                if (emailEnviado)
                {
                    MostrarSucesso(
                        "O pedido foi enviado com sucesso. " +
                        "Foi enviada uma confirmação para o email indicado."
                    );
                }
                else
                {
                    MostrarAviso(
                        "O pedido foi guardado com sucesso, mas não foi " +
                        "possível enviar o email de confirmação. " +
                        "O pedido continua pendente para análise."
                    );
                }
            }
            catch (SqlException ex)
            {
                System.Diagnostics.Trace.TraceError(
                    "Erro SQL ao enviar pedido de agrupamento: " +
                    ex
                );

                /*
                 * 2601 e 2627 correspondem normalmente
                 * a violações de índices únicos.
                 */
                if (ex.Number == 2601 ||
                    ex.Number == 2627)
                {
                    MostrarErro(
                        "Já existe um pedido pendente com este email ou NIF."
                    );

                    return;
                }

                MostrarErro(
                    "Não foi possível guardar o pedido. " +
                    "Tente novamente mais tarde."
                );
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError(
                    "Erro ao enviar pedido de agrupamento: " +
                    ex
                );

                MostrarErro(
                    "Não foi possível processar o pedido. " +
                    "Tente novamente mais tarde."
                );
            }
        }

        private bool EmailJaPossuiConta(string email)
        {
            string username =
                Membership.GetUserNameByEmail(email);

            return !string.IsNullOrWhiteSpace(username);
        }

        private string VerificarPedidoPendente(
            string email,
            string nif)
        {
            const string sql = @"
                SELECT TOP 1
                    Email,
                    NIF
                FROM dbo.PedidoAgrupamento
                WHERE Estado = N'Pendente'
                  AND
                  (
                      LOWER(LTRIM(RTRIM(Email))) = @Email
                      OR NIF = @NIF
                  );";

            using (
                SqlConnection conn =
                    new SqlConnection(connectionString))
            using (
                SqlCommand cmd =
                    new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add(
                    "@Email",
                    SqlDbType.NVarChar,
                    150
                ).Value = email;

                cmd.Parameters.Add(
                    "@NIF",
                    SqlDbType.NVarChar,
                    9
                ).Value = nif;

                conn.Open();

                using (
                    SqlDataReader reader =
                        cmd.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        return null;
                    }

                    string emailEncontrado =
                        Convert.ToString(reader["Email"])
                            .Trim();

                    string nifEncontrado =
                        Convert.ToString(reader["NIF"])
                            .Trim();

                    if (emailEncontrado.Equals(
                        email,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        return
                            "Já existe um pedido pendente associado a este email.";
                    }

                    if (nifEncontrado == nif)
                    {
                        return
                            "Já existe um pedido pendente associado a este NIF.";
                    }

                    return
                        "Já existe um pedido pendente com estes dados.";
                }
            }
        }

        private void InserirPedido(
            string nomeAgrupamento,
            string nomeResponsavel,
            string email,
            string telefone,
            string nif,
            string morada,
            string codigoPostal,
            string localidade,
            string codigoMec)
        {
            const string sql = @"
                INSERT INTO dbo.PedidoAgrupamento
                (
                    NomeAgrupamento,
                    NomeResponsavel,
                    Email,
                    Telefone,
                    NIF,
                    Localidade,
                    Morada,
                    CodigoPostal,
                    CodigoMEC,
                    Estado,
                    DataPedido
                )
                VALUES
                (
                    @NomeAgrupamento,
                    @NomeResponsavel,
                    @Email,
                    @Telefone,
                    @NIF,
                    @Localidade,
                    @Morada,
                    @CodigoPostal,
                    @CodigoMEC,
                    N'Pendente',
                    SYSDATETIME()
                );";

            using (
                SqlConnection conn =
                    new SqlConnection(connectionString))
            using (
                SqlCommand cmd =
                    new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add(
                    "@NomeAgrupamento",
                    SqlDbType.NVarChar,
                    200
                ).Value = nomeAgrupamento;

                cmd.Parameters.Add(
                    "@NomeResponsavel",
                    SqlDbType.NVarChar,
                    200
                ).Value = nomeResponsavel;

                cmd.Parameters.Add(
                    "@Email",
                    SqlDbType.NVarChar,
                    150
                ).Value = email;

                cmd.Parameters.Add(
                    "@Telefone",
                    SqlDbType.NVarChar,
                    20
                ).Value = telefone;

                cmd.Parameters.Add(
                    "@NIF",
                    SqlDbType.NVarChar,
                    9
                ).Value = nif;

                cmd.Parameters.Add(
                    "@Localidade",
                    SqlDbType.NVarChar,
                    150
                ).Value = localidade;

                cmd.Parameters.Add(
                    "@Morada",
                    SqlDbType.NVarChar,
                    300
                ).Value = morada;

                cmd.Parameters.Add(
                    "@CodigoPostal",
                    SqlDbType.Char,
                    8
                ).Value = codigoPostal;

                cmd.Parameters.Add(
                    "@CodigoMEC",
                    SqlDbType.NVarChar,
                    20
                ).Value =
                    string.IsNullOrWhiteSpace(codigoMec)
                        ? (object)DBNull.Value
                        : codigoMec;

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private bool EnviarEmailConfirmacao(
            string emailDestino,
            string nomeResponsavel,
            string nomeAgrupamento)
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

                string nomeSeguro =
                    HttpUtility.HtmlEncode(nomeResponsavel);

                string agrupamentoSeguro =
                    HttpUtility.HtmlEncode(nomeAgrupamento);

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
                        "Pedido de criação de agrupamento recebido";

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
                                padding:22px;
                                border-radius:14px;
                                background:#f8fafc;
                                border:1px solid #e2e8f0;
                            "">

                                <h2 style=""
                                    margin-top:0;
                                    color:#123570;
                                "">
                                    Pedido recebido
                                </h2>

                                <p>
                                    Olá, <strong>" +
                                    nomeSeguro +
                                    @"</strong>.
                                </p>

                                <p>
                                    Recebemos o pedido de criação do agrupamento
                                    <strong>" +
                                    agrupamentoSeguro +
                                    @"</strong>.
                                </p>

                                <p>
                                    O pedido encontra-se pendente e será
                                    analisado pela administração da plataforma.
                                </p>

                                <p>
                                    A conta ainda não foi criada.
                                    Caso o pedido seja aprovado, receberá
                                    outro email com as credenciais de acesso.
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
                    "Erro ao enviar email de confirmação do pedido: " +
                    ex
                );

                return false;
            }
        }

        private void LimparCampos()
        {
            txtNome.Text = string.Empty;
            txtNomeResponsavel.Text = string.Empty;
            txtEmail.Text = string.Empty;
            txtTelefone.Text = string.Empty;
            txtNIF.Text = string.Empty;
            txtMorada.Text = string.Empty;
            txtCodigoPostal.Text = string.Empty;
            txtLocalidade.Text = string.Empty;
            txtCodigoMEC.Text = string.Empty;
        }

        private void MostrarSucesso(string mensagem)
        {
            lblMensagem.Text = mensagem;
            lblMensagem.CssClass = "message-success";
            lblMensagem.Visible = true;
        }

        private void MostrarAviso(string mensagem)
        {
            lblMensagem.Text = mensagem;
            lblMensagem.CssClass = "message-warning";
            lblMensagem.Visible = true;
        }

        private void MostrarErro(string mensagem)
        {
            lblMensagem.Text = mensagem;
            lblMensagem.CssClass = "message-error";
            lblMensagem.Visible = true;
        }
    }
}