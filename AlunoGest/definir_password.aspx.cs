using System;
using System.Text.RegularExpressions;
using System.Web.Security;
using AlunoGest.Util;

namespace AlunoGest
{
    public partial class definir_password :
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
                ValidarTokenInicial();
            }
        }


        private void ValidarTokenInicial()
        {
            string token =
                Request.QueryString["token"];

            string username =
                PasswordResetManager
                    .GetUsernameByToken(token);

            if (string.IsNullOrWhiteSpace(username))
            {
                MostrarMensagem(
                    "O link de recuperação é inválido, " +
                    "já foi utilizado ou expirou.",
                    true
                );

                DesativarFormulario();
            }
        }

        #endregion


        #region Redefinir palavra-passe

        protected void btnReset_Click(
            object sender,
            EventArgs e)
        {
            LimparMensagem();

            if (!Page.IsValid)
            {
                return;
            }

            string novaPassword =
                textPassword.Text;

            string mensagemPassword;

            if (!PasswordForte(
                    novaPassword,
                    out mensagemPassword))
            {
                MostrarMensagem(
                    mensagemPassword,
                    true
                );

                return;
            }

            string token =
                Request.QueryString["token"];

            string username =
                PasswordResetManager
                    .GetUsernameByToken(token);

            if (string.IsNullOrWhiteSpace(username))
            {
                MostrarMensagem(
                    "O link de recuperação é inválido, " +
                    "já foi utilizado ou expirou.",
                    true
                );

                DesativarFormulario();

                return;
            }

            try
            {
                MembershipUser user =
                    Membership.GetUser(username);

                if (user == null)
                {
                    MostrarMensagem(
                        "Não foi possível encontrar a conta.",
                        true
                    );

                    return;
                }

                string passwordTemporaria =
                    user.ResetPassword();

                bool alterou =
                    user.ChangePassword(
                        passwordTemporaria,
                        novaPassword
                    );

                if (!alterou)
                {
                    MostrarMensagem(
                        "Não foi possível alterar a palavra-passe.",
                        true
                    );

                    return;
                }

                PasswordResetManager
                    .InvalidateToken(token);

                MostrarMensagem(
                    "Palavra-passe alterada com sucesso. " +
                    "Será redirecionado para o início de sessão " +
                    "dentro de cinco segundos.",
                    false
                );

                DesativarFormulario();

                string loginUrl =
                    ResolveUrl(
                        "~/login.aspx"
                    );

                ltlRedirect.Text =
                    "<meta http-equiv='refresh' " +
                    "content='5;url=" +
                    loginUrl +
                    "' />";
            }
            catch (MembershipPasswordException ex)
            {
                System.Diagnostics.Trace.TraceError(
    "Erro no pedido de recuperação: {0}",
    ex
);

                MostrarMensagem(
                    "A nova palavra-passe não cumpre " +
                    "as regras configuradas.",
                    true
                );
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError(
                    "Erro ao redefinir password: {0}",
                    ex
                );

                MostrarMensagem(
                    "Não foi possível alterar a palavra-passe. " +
                    "Tente novamente mais tarde.",
                    true
                );
            }
        }

        #endregion


        #region Validação da palavra-passe

        private bool PasswordForte(
            string password,
            out string mensagem)
        {
            mensagem =
                string.Empty;

            if (string.IsNullOrWhiteSpace(password) ||
                password.Length < 8)
            {
                mensagem =
                    "A palavra-passe deve ter pelo menos " +
                    "oito caracteres.";

                return false;
            }

            if (!Regex.IsMatch(
                    password,
                    "[A-Z]"))
            {
                mensagem =
                    "A palavra-passe deve conter pelo menos " +
                    "uma letra maiúscula.";

                return false;
            }

            if (!Regex.IsMatch(
                    password,
                    "[a-z]"))
            {
                mensagem =
                    "A palavra-passe deve conter pelo menos " +
                    "uma letra minúscula.";

                return false;
            }

            if (!Regex.IsMatch(
                    password,
                    "[0-9]"))
            {
                mensagem =
                    "A palavra-passe deve conter pelo menos " +
                    "um número.";

                return false;
            }

            if (!Regex.IsMatch(
                    password,
                    @"[\W_]"))
            {
                mensagem =
                    "A palavra-passe deve conter pelo menos " +
                    "um carácter especial.";

                return false;
            }

            return true;
        }

        #endregion


        #region Formulário e mensagens

        private void DesativarFormulario()
        {
            btnReset.Enabled =
                false;

            textPassword.Enabled =
                false;

            textPasswordRepeticao.Enabled =
                false;
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