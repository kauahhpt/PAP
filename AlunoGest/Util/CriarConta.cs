using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web.Security;

namespace AlunoGest.Util
{
    public static class CriarConta
    {
        #region Palavras a Ignorar no Username

        private static readonly HashSet<string> _PalavrasIgnorar = new HashSet<string>
        {
            "de", "da", "do", "das", "dos", "e", "a", "o"
        };

        #endregion

        #region Geração de Username

        public static string GerarUsername(string NomeCompleto)
        {
            if (string.IsNullOrWhiteSpace(NomeCompleto))
                throw new ArgumentException("Nome inválido.");

            string[] Partes = NomeCompleto.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            List<string> PalavrasValidas = new List<string>();

            foreach (string Parte in Partes)
            {
                string Normalizado = NormalizarTexto(Parte);

                if (!_PalavrasIgnorar.Contains(Normalizado))
                    PalavrasValidas.Add(Normalizado);
            }

            if (PalavrasValidas.Count == 0)
                throw new InvalidOperationException("Não foi possível gerar username a partir do nome.");

            string PrimeiroNome = CapitalizarPrimeiraLetra(PalavrasValidas[0]);

            List<string> Iniciais = new List<string>();

            for (int I = 1; I < PalavrasValidas.Count; I++)
                Iniciais.Add(PalavrasValidas[I].Substring(0, 1).ToUpperInvariant());

            string Username = PrimeiroNome;

            if (Iniciais.Count > 0)
                Username += "." + string.Join(".", Iniciais);

            return Username;
        }

        public static string GarantirUsernameUnico(string UsernameBase)
        {
            string Username = UsernameBase;
            int Sufixo = 1;

            while (Membership.GetUser(Username) != null)
            {
                Sufixo++;
                Username = UsernameBase + Sufixo;
            }

            return Username;
        }

        #endregion

        #region Geração de Password

        public static string GerarPassword()
        {
            return Membership.GeneratePassword(10, 2);
        }

        #endregion

        #region Envio de Email

        public static void EnviarEmailCredenciais(string EmailDestino, string NomeCompleto, string Username, string Password, string UrlLogin)
        {
            if (string.IsNullOrWhiteSpace(EmailDestino))
                return;

            string Assunto = "As suas credenciais de acesso — AlunoGest";

            string Corpo = $@"
                <p>Olá {NomeCompleto},</p>

                <p>A sua conta foi criada com sucesso. Eis as suas credenciais de acesso:</p>

                <table style='border-collapse: collapse;'>
                    <tr>
                        <td style='padding: 6px 12px; font-weight: bold;'>Utilizador:</td>
                        <td style='padding: 6px 12px;'>{Username}</td>
                    </tr>
                    <tr>
                        <td style='padding: 6px 12px; font-weight: bold;'>Password:</td>
                        <td style='padding: 6px 12px;'>{Password}</td>
                    </tr>
                </table>

                <p style='margin-top: 16px;'>
                    Pode aceder através do seguinte link:
                    <a href='{UrlLogin}'>{UrlLogin}</a>
                </p>

                <p>Por segurança, recomendamos que altere a password após o primeiro acesso.</p>

                <p>Cumprimentos,<br />Equipa AlunoGest</p>";

            using (MailMessage Mensagem = new MailMessage())
            {
                Mensagem.To.Add(EmailDestino);
                Mensagem.Subject = Assunto;
                Mensagem.Body = Corpo;
                Mensagem.IsBodyHtml = true;

                using (SmtpClient Cliente = new SmtpClient())
                {
                    Cliente.Send(Mensagem);
                }
            }
        }

        #endregion

        #region Auxiliares Privados

        private static string NormalizarTexto(string Texto)
        {
            if (string.IsNullOrWhiteSpace(Texto))
                return string.Empty;

            Texto = Texto.ToLowerInvariant();

            Texto = Texto.Replace("á", "a").Replace("à", "a").Replace("ã", "a").Replace("â", "a");
            Texto = Texto.Replace("é", "e").Replace("ê", "e");
            Texto = Texto.Replace("í", "i");
            Texto = Texto.Replace("ó", "o").Replace("ô", "o").Replace("õ", "o");
            Texto = Texto.Replace("ú", "u");
            Texto = Texto.Replace("ç", "c");

            Texto = Texto.Replace("-", "")
                         .Replace("_", "")
                         .Replace(".", "")
                         .Replace(",", "");

            return Texto;
        }

        private static string CapitalizarPrimeiraLetra(string Texto)
        {
            if (string.IsNullOrEmpty(Texto))
                return Texto;

            return char.ToUpperInvariant(Texto[0]) + Texto.Substring(1);
        }

        #endregion
    }
}