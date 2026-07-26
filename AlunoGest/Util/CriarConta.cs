using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Web;
using System.Web.Security;

namespace AlunoGest.Util
{
    public static class CriarConta
    {
        #region Palavras a ignorar no username

        private static readonly HashSet<string> _PalavrasIgnorar =
            new HashSet<string>
            {
                "de",
                "da",
                "do",
                "das",
                "dos",
                "e",
                "a",
                "o"
            };

        #endregion


        #region Geração de username

        public static string GerarUsername(
            string nomeCompleto)
        {
            if (string.IsNullOrWhiteSpace(nomeCompleto))
            {
                throw new ArgumentException(
                    "Nome inválido.",
                    "nomeCompleto"
                );
            }

            string[] partes =
                nomeCompleto.Split(
                    new[] { ' ' },
                    StringSplitOptions.RemoveEmptyEntries
                );

            List<string> palavrasValidas =
                new List<string>();

            foreach (string parte in partes)
            {
                string normalizado =
                    NormalizarTexto(parte);

                if (!string.IsNullOrWhiteSpace(normalizado) &&
                    !_PalavrasIgnorar.Contains(normalizado))
                {
                    palavrasValidas.Add(normalizado);
                }
            }

            if (palavrasValidas.Count == 0)
            {
                throw new InvalidOperationException(
                    "Não foi possível gerar o nome de utilizador " +
                    "a partir do nome indicado."
                );
            }

            string primeiroNome =
                CapitalizarPrimeiraLetra(
                    palavrasValidas[0]
                );

            List<string> iniciais =
                new List<string>();

            for (int i = 1;
                 i < palavrasValidas.Count;
                 i++)
            {
                iniciais.Add(
                    palavrasValidas[i]
                        .Substring(0, 1)
                        .ToUpperInvariant()
                );
            }

            string username =
                primeiroNome;

            if (iniciais.Count > 0)
            {
                username +=
                    "." +
                    string.Join(
                        ".",
                        iniciais
                    );
            }

            return username;
        }

        public static string GarantirUsernameUnico(
            string usernameBase)
        {
            if (string.IsNullOrWhiteSpace(usernameBase))
            {
                throw new ArgumentException(
                    "O nome de utilizador base é obrigatório.",
                    "usernameBase"
                );
            }

            string username =
                usernameBase;

            int sufixo =
                1;

            while (Membership.GetUser(
                       username,
                       false) != null)
            {
                sufixo++;

                username =
                    usernameBase +
                    sufixo;
            }

            return username;
        }

        #endregion


        #region Palavra-passe

        public static string GerarPassword()
        {
            return Membership.GeneratePassword(
                10,
                2
            );
        }

        public static string RedefinirPassword(
            string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException(
                    "O nome de utilizador é obrigatório.",
                    "username"
                );
            }

            MembershipUser utilizador =
                Membership.GetUser(
                    username,
                    false
                );

            if (utilizador == null)
            {
                throw new InvalidOperationException(
                    "Não foi possível encontrar a conta de utilizador."
                );
            }

            if (utilizador.IsLockedOut)
            {
                bool desbloqueado =
                    utilizador.UnlockUser();

                if (!desbloqueado)
                {
                    throw new InvalidOperationException(
                        "Não foi possível desbloquear a conta de utilizador."
                    );
                }
            }

            /*
             * ResetPassword gera imediatamente uma palavra-passe
             * temporária e invalida a palavra-passe anterior.
             */
            string passwordTemporaria =
                utilizador.ResetPassword();

            string novaPassword =
                GerarPassword();

            try
            {
                bool alterada =
                    utilizador.ChangePassword(
                        passwordTemporaria,
                        novaPassword
                    );

                if (alterada)
                {
                    return novaPassword;
                }
            }
            catch
            {
                /*
                 * Caso o provider não aceite a segunda alteração,
                 * a palavra-passe temporária devolvida pelo
                 * ResetPassword continua válida.
                 */
            }

            return passwordTemporaria;
        }

        #endregion


        #region Envio de credenciais iniciais

        public static void EnviarEmailCredenciais(
            string emailDestino,
            string nomeCompleto,
            string username,
            string password,
            string urlLogin)
        {
            ValidarDadosEmail(
                emailDestino,
                username,
                password,
                urlLogin
            );

            string nomeSeguro =
                HttpUtility.HtmlEncode(
                    nomeCompleto ?? string.Empty
                );

            string usernameSeguro =
                HttpUtility.HtmlEncode(username);

            string passwordSegura =
                HttpUtility.HtmlEncode(password);

            string urlAtributoSeguro =
                HttpUtility.HtmlAttributeEncode(
                    urlLogin
                );

            string urlTextoSeguro =
                HttpUtility.HtmlEncode(
                    urlLogin
                );

            string assunto =
                "As suas credenciais de acesso — AlunoGest";

            string corpo = @"
                <p>Olá " + nomeSeguro + @",</p>

                <p>
                    A sua conta foi criada com sucesso.
                    Eis as suas credenciais de acesso:
                </p>

                <table style='border-collapse: collapse;'>
                    <tr>
                        <td style='padding: 6px 12px; font-weight: bold;'>
                            Utilizador:
                        </td>

                        <td style='padding: 6px 12px;'>" +
                            usernameSeguro + @"
                        </td>
                    </tr>

                    <tr>
                        <td style='padding: 6px 12px; font-weight: bold;'>
                            Palavra-passe:
                        </td>

                        <td style='padding: 6px 12px;'>" +
                            passwordSegura + @"
                        </td>
                    </tr>
                </table>

                <p style='margin-top: 16px;'>
                    Pode aceder através do seguinte link:

                    <a href='" + urlAtributoSeguro + @"'>" +
                        urlTextoSeguro + @"
                    </a>
                </p>

                <p>
                    Por segurança, recomendamos que altere a
                    palavra-passe após o primeiro acesso.
                </p>

                <p>
                    Cumprimentos,<br />
                    Equipa AlunoGest
                </p>";

            EnviarEmail(
                emailDestino,
                assunto,
                corpo
            );
        }

        #endregion


        #region Reenvio de novas credenciais

        public static void EnviarEmailCredenciaisRedefinidas(
            string emailDestino,
            string nomeCompleto,
            string username,
            string password,
            string urlLogin)
        {
            ValidarDadosEmail(
                emailDestino,
                username,
                password,
                urlLogin
            );

            string nomeSeguro =
                HttpUtility.HtmlEncode(
                    nomeCompleto ?? string.Empty
                );

            string usernameSeguro =
                HttpUtility.HtmlEncode(username);

            string passwordSegura =
                HttpUtility.HtmlEncode(password);

            string urlAtributoSeguro =
                HttpUtility.HtmlAttributeEncode(
                    urlLogin
                );

            string urlTextoSeguro =
                HttpUtility.HtmlEncode(
                    urlLogin
                );

            string assunto =
                "Novas credenciais de acesso — AlunoGest";

            string corpo = @"
                <p>Olá " + nomeSeguro + @",</p>

                <p>
                    Foram geradas novas credenciais de acesso
                    à sua conta AlunoGest.
                </p>

                <p>
                    A palavra-passe anterior deixou de funcionar.
                    Utilize os dados abaixo no próximo acesso:
                </p>

                <table style='border-collapse: collapse;'>
                    <tr>
                        <td style='padding: 6px 12px; font-weight: bold;'>
                            Utilizador:
                        </td>

                        <td style='padding: 6px 12px;'>" +
                            usernameSeguro + @"
                        </td>
                    </tr>

                    <tr>
                        <td style='padding: 6px 12px; font-weight: bold;'>
                            Nova palavra-passe:
                        </td>

                        <td style='padding: 6px 12px;'>" +
                            passwordSegura + @"
                        </td>
                    </tr>
                </table>

                <p style='margin-top: 16px;'>
                    Pode aceder através do seguinte link:

                    <a href='" + urlAtributoSeguro + @"'>" +
                        urlTextoSeguro + @"
                    </a>
                </p>

                <p>
                    Por segurança, recomendamos que altere a
                    palavra-passe após iniciar sessão.
                </p>

                <p>
                    Caso não tenha solicitado este reenvio,
                    contacte o agrupamento.
                </p>

                <p>
                    Cumprimentos,<br />
                    Equipa AlunoGest
                </p>";

            EnviarEmail(
                emailDestino,
                assunto,
                corpo
            );
        }

        #endregion


        #region Envio SMTP

        private static void EnviarEmail(
            string emailDestino,
            string assunto,
            string corpo)
        {
            using (MailMessage mensagem =
                new MailMessage())
            {
                mensagem.To.Add(
                    emailDestino
                );

                mensagem.Subject =
                    assunto;

                mensagem.Body =
                    corpo;

                mensagem.IsBodyHtml =
                    true;

                using (SmtpClient cliente =
                    new SmtpClient())
                {
                    /*
                     * O servidor, porta, utilizador, palavra-passe,
                     * SSL e remetente são obtidos do Web.config.
                     */
                    cliente.Send(mensagem);
                }
            }
        }

        private static void ValidarDadosEmail(
            string emailDestino,
            string username,
            string password,
            string urlLogin)
        {
            if (string.IsNullOrWhiteSpace(emailDestino))
            {
                throw new ArgumentException(
                    "O email de destino é obrigatório.",
                    "emailDestino"
                );
            }

            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException(
                    "O nome de utilizador é obrigatório.",
                    "username"
                );
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException(
                    "A palavra-passe é obrigatória.",
                    "password"
                );
            }

            if (string.IsNullOrWhiteSpace(urlLogin))
            {
                throw new ArgumentException(
                    "O endereço da página de login é obrigatório.",
                    "urlLogin"
                );
            }
        }

        #endregion


        #region Auxiliares privados

        private static string NormalizarTexto(
            string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
            {
                return string.Empty;
            }

            texto =
                texto.ToLowerInvariant();

            texto =
                texto.Replace("á", "a")
                     .Replace("à", "a")
                     .Replace("ã", "a")
                     .Replace("â", "a")
                     .Replace("é", "e")
                     .Replace("ê", "e")
                     .Replace("í", "i")
                     .Replace("ó", "o")
                     .Replace("ô", "o")
                     .Replace("õ", "o")
                     .Replace("ú", "u")
                     .Replace("ç", "c")
                     .Replace("-", string.Empty)
                     .Replace("_", string.Empty)
                     .Replace(".", string.Empty)
                     .Replace(",", string.Empty);

            return texto;
        }

        private static string CapitalizarPrimeiraLetra(
            string texto)
        {
            if (string.IsNullOrEmpty(texto))
            {
                return texto;
            }

            return char.ToUpperInvariant(
                       texto[0]
                   ) +
                   texto.Substring(1);
        }

        #endregion
    }
}