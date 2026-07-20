using System;
using System.Net.Mail;
using System.Web;
using AlunoGest.Util;

namespace AlunoGest
{
    public partial class recuperar_password :
        System.Web.UI.Page
    {
        #region Página

        protected void Page_Load(
            object sender,
            EventArgs e)
        {
            if (!IsPostBack)
            {
                LimparMensagem();
            }
        }

        #endregion


        #region Enviar recuperação

        protected void btnSend_Click(
            object sender,
            EventArgs e)
        {
            LimparMensagem();

            if (!Page.IsValid)
            {
                return;
            }

            string username =
                textUsername.Text.Trim();

            string email =
                textEmail.Text.Trim();

            try
            {
                string token =
                    PasswordResetManager
                        .GenerateAndStoreToken(
                            username,
                            email,
                            Request.UserHostAddress
                        );

                if (!string.IsNullOrWhiteSpace(token))
                {
                    string link =
                        CriarLinkRecuperacao(token);

                    EnviarEmailRecuperacao(
                        email,
                        link
                    );
                }

                MostrarMensagemGenerica();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError(
                    "Erro no pedido de recuperação: {0}",
                    ex
                );

                MostrarMensagem(
                    "Não foi possível processar o pedido neste momento. " +
                    "Tente novamente mais tarde.",
                    true
                );
            }
        }

        private string CriarLinkRecuperacao(
            string token)
        {
            string enderecoBase =
                Request.Url.GetLeftPart(
                    UriPartial.Authority
                );

            string pagina =
                ResolveUrl(
                    "~/definir_password.aspx"
                );

            return enderecoBase +
                   pagina +
                   "?token=" +
                   HttpUtility.UrlEncode(token);
        }


        private void EnviarEmailRecuperacao(
            string email,
            string link)
        {
            string linkSeguro =
                HttpUtility.HtmlAttributeEncode(
                    link
                );

            using (MailMessage mail =
                new MailMessage())
            {
                mail.From =
                    new MailAddress(
                        "inovarescola2526@gmail.com",
                        "Inovar"
                    );

                mail.To.Add(email);

                mail.Subject =
                    "Recuperação de palavra-passe";

                mail.IsBodyHtml =
                    true;

                mail.Body = string.Format(
                    @"
                    <div style='font-family:Segoe UI,Arial,sans-serif;
                                max-width:600px;
                                margin:0 auto;
                                color:#334155;'>

                        <h2 style='color:#1d4ed8;'>
                            Recuperação de palavra-passe
                        </h2>

                        <p>
                            Foi solicitado um novo acesso à sua conta.
                        </p>

                        <p>
                            Clique no botão abaixo para definir
                            uma nova palavra-passe:
                        </p>

                        <p style='margin:28px 0;'>
                            <a
                                href='{0}'
                                style='display:inline-block;
                                       padding:12px 20px;
                                       border-radius:8px;
                                       background:#1d4ed8;
                                       color:#ffffff;
                                       font-weight:700;
                                       text-decoration:none;'>

                                Definir nova palavra-passe

                            </a>
                        </p>

                        <p>
                            Este link é válido durante 30 minutos
                            e só pode ser utilizado uma vez.
                        </p>

                        <p style='color:#64748b;font-size:13px;'>
                            Caso não tenha solicitado esta alteração,
                            ignore esta mensagem.
                        </p>

                        <hr style='border:0;
                                   border-top:1px solid #e2e8f0;
                                   margin:25px 0;' />

                        <p style='color:#64748b;font-size:12px;'>
                            Equipa Inovar
                        </p>

                    </div>",
                    linkSeguro
                );

                using (SmtpClient smtp =
                    new SmtpClient())
                {
                    smtp.Send(mail);
                }
            }
        }

        #endregion


        #region Mensagens

        private void MostrarMensagemGenerica()
        {
            MostrarMensagem(
                "Caso exista uma conta associada ao email indicado, " +
                "será enviado um link de recuperação.",
                false
            );
        }


        private void MostrarMensagem(
            string mensagem,
            bool erro)
        {
            lblMessage.Visible =
                true;

            lblMessage.Text =
                mensagem;

            lblMessage.CssClass =
                erro
                    ? "alert alert-warning d-block"
                    : "alert alert-success d-block";
        }


        private void LimparMensagem()
        {
            lblMessage.Visible =
                false;

            lblMessage.Text =
                string.Empty;

            lblMessage.CssClass =
                string.Empty;
        }

        #endregion
    }
}