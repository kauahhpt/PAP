using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;

namespace AlunoGest.Util
{
    public enum EstadoValidacaoNifApi
    {
        Valido,
        Invalido,
        Indisponivel
    }


    public sealed class ResultadoValidacaoNifApi
    {
        public EstadoValidacaoNifApi Estado { get; set; }

        public string Mensagem { get; set; }
    }


    public static class ValidadorNif
    {
        #region Prefixos válidos

        /*
         * Prefixos aceites para o primeiro caractere (pessoas
         * singulares residentes) e para os dois primeiros
         * caracteres (categorias específicas, incluindo
         * não residentes e outras entidades associadas
         * a pessoas singulares).
         *
         * Referência: estrutura de números de identificação
         * fiscal (NIF) usada em Portugal.
         */

        private static readonly string[] PrefixosDoisDigitos =
        {
            "45", // Não residente sem estabelecimento estável
            "70", // Herança indivisa
            "71", // Não residente (pessoa singular)
            "72", // Não residente (pessoa singular)
            "74", // Não residente
            "75", // Não residente
            "77", // Atribuição oficiosa
            "78", // Não residente
            "79", // Não residente
            "90", // Condomínios / sociedades irregulares
            "91", // Condomínios / sociedades irregulares
            "98", // Não residente (pessoa singular)
            "99"  // Não residente (pessoa singular)
        };

        private static readonly char[] PrefixosUmDigito =
        {
            '1', // Pessoa singular
            '2', // Pessoa singular
            '3'  // Pessoa singular
        };

        #endregion


        #region Normalização

        public static string Normalizar(string nif)
        {
            if (string.IsNullOrWhiteSpace(nif))
            {
                return string.Empty;
            }

            return Regex.Replace(
                nif,
                @"\D",
                string.Empty
            );
        }

        #endregion


        #region Validação local

        public static bool ValidarLocalmente(
            string nif,
            out string mensagem)
        {
            mensagem = string.Empty;

            nif = Normalizar(nif);

            if (!Regex.IsMatch(nif, @"^\d{9}$"))
            {
                mensagem =
                    "O NIF deve conter exatamente 9 algarismos.";

                return false;
            }

            if (!PrefixoValido(nif))
            {
                mensagem =
                    "O NIF não corresponde a um formato válido " +
                    "(prefixo desconhecido).";

                return false;
            }

            int soma = 0;

            for (int indice = 0; indice < 8; indice++)
            {
                int digito = nif[indice] - '0';

                int peso = 9 - indice;

                soma += digito * peso;
            }

            int resto = soma % 11;

            int digitoControlo = 11 - resto;

            if (digitoControlo >= 10)
            {
                digitoControlo = 0;
            }

            int ultimoDigito = nif[8] - '0';

            if (digitoControlo != ultimoDigito)
            {
                mensagem =
                    "O NIF não possui um dígito de controlo válido.";

                return false;
            }

            return true;
        }


        /// <summary>
        /// Verifica se o NIF começa com um prefixo de um dígito
        /// (pessoa singular residente) ou com um dos prefixos de
        /// dois dígitos aceites (não residentes e outras entidades
        /// associadas a pessoas singulares).
        /// </summary>
        private static bool PrefixoValido(string nif)
        {
            if (PrefixosUmDigito.Contains(nif[0]))
            {
                return true;
            }

            string doisPrimeiros =
                nif.Substring(0, 2);

            return PrefixosDoisDigitos.Contains(
                doisPrimeiros
            );
        }

        #endregion


        #region Validação pela API

        public static ResultadoValidacaoNifApi ValidarNaApi(
            string nif)
        {
            nif = Normalizar(nif);

            string chaveApi =
                Convert.ToString(
                    ConfigurationManager
                        .AppSettings["NifPtApiKey"]
                )
                .Trim();

            if (string.IsNullOrWhiteSpace(chaveApi))
            {
                return CriarResultado(
                    EstadoValidacaoNifApi.Indisponivel,
                    "A chave da API do NIF não está configurada."
                );
            }

            string endereco =
                "https://www.nif.pt/" +
                "?json=1" +
                "&q=" +
                HttpUtility.UrlEncode(nif) +
                "&key=" +
                HttpUtility.UrlEncode(chaveApi);

            try
            {
                ServicePointManager.SecurityProtocol |=
                    SecurityProtocolType.Tls12;

                string respostaJson;

                using (WebClient cliente = new WebClient())
                {
                    cliente.Encoding = Encoding.UTF8;

                    cliente.Headers[
                        HttpRequestHeader.Accept
                    ] =
                        "application/json";

                    cliente.Headers[
                        HttpRequestHeader.UserAgent
                    ] =
                        "AlunoGest/1.0";

                    respostaJson =
                        cliente.DownloadString(endereco);
                }

                return InterpretarRespostaApi(
                    respostaJson
                );
            }
            catch (WebException ex)
            {
                string detalhe =
                    LerRespostaErro(ex);

                if (string.IsNullOrWhiteSpace(detalhe))
                {
                    detalhe =
                        ex.Message;
                }

                return CriarResultado(
                    EstadoValidacaoNifApi.Indisponivel,
                    "Não foi possível contactar a API do NIF. " +
                    detalhe
                );
            }
            catch (Exception ex)
            {
                return CriarResultado(
                    EstadoValidacaoNifApi.Indisponivel,
                    "Erro ao validar o NIF na API: " +
                    ex.Message
                );
            }
        }


        private static ResultadoValidacaoNifApi
            InterpretarRespostaApi(string respostaJson)
        {
            if (string.IsNullOrWhiteSpace(respostaJson))
            {
                return CriarResultado(
                    EstadoValidacaoNifApi.Indisponivel,
                    "A API devolveu uma resposta vazia."
                );
            }

            JavaScriptSerializer serializer =
                new JavaScriptSerializer();

            Dictionary<string, object> resposta;

            try
            {
                resposta =
                    serializer.Deserialize<
                        Dictionary<string, object>
                    >(respostaJson);
            }
            catch (Exception)
            {
                return CriarResultado(
                    EstadoValidacaoNifApi.Indisponivel,
                    "A resposta da API não está num formato válido."
                );
            }

            if (resposta == null)
            {
                return CriarResultado(
                    EstadoValidacaoNifApi.Indisponivel,
                    "Não foi possível interpretar a resposta da API."
                );
            }

            string resultado =
                ObterTexto(
                    resposta,
                    "result"
                );

            if (!string.Equals(
                    resultado,
                    "success",
                    StringComparison.OrdinalIgnoreCase))
            {
                string erro =
                    ObterTexto(
                        resposta,
                        "message"
                    );

                if (string.IsNullOrWhiteSpace(erro))
                {
                    erro =
                        ObterTexto(
                            resposta,
                            "error"
                        );
                }

                if (string.IsNullOrWhiteSpace(erro))
                {
                    erro =
                        ObterTexto(
                            resposta,
                            "reason"
                        );
                }

                if (string.IsNullOrWhiteSpace(erro))
                {
                    erro =
                        "A API recusou o pedido. " +
                        "Confirma se a chave está ativada e se " +
                        "não ultrapassaste o limite de pedidos.";
                }

                return CriarResultado(
                    EstadoValidacaoNifApi.Indisponivel,
                    erro
                );
            }

            bool nifValidation;
            bool isNif;

            bool encontrouValidacao =
                TentarObterBooleano(
                    resposta,
                    "nif_validation",
                    out nifValidation
                );

            bool encontrouIsNif =
                TentarObterBooleano(
                    resposta,
                    "is_nif",
                    out isNif
                );

            if (!encontrouValidacao ||
                !encontrouIsNif)
            {
                return CriarResultado(
                    EstadoValidacaoNifApi.Indisponivel,
                    "A API respondeu, mas não devolveu os campos " +
                    "nif_validation e is_nif."
                );
            }

            if (!nifValidation || !isNif)
            {
                return CriarResultado(
                    EstadoValidacaoNifApi.Invalido,
                    "A API indicou que o NIF não é válido."
                );
            }

            return CriarResultado(
                EstadoValidacaoNifApi.Valido,
                "NIF validado com sucesso."
            );
        }

        #endregion


        #region Leitura da resposta

        private static bool TentarObterBooleano(
            Dictionary<string, object> resposta,
            string campo,
            out bool valor)
        {
            valor = false;

            object valorObjeto;

            if (!resposta.TryGetValue(
                    campo,
                    out valorObjeto) ||
                valorObjeto == null)
            {
                return false;
            }

            if (valorObjeto is bool)
            {
                valor = (bool)valorObjeto;

                return true;
            }

            string texto =
                Convert.ToString(valorObjeto);

            if (bool.TryParse(
                    texto,
                    out valor))
            {
                return true;
            }

            int numero;

            if (int.TryParse(
                    texto,
                    out numero))
            {
                valor = numero == 1;

                return true;
            }

            return false;
        }


        private static string ObterTexto(
            Dictionary<string, object> resposta,
            string campo)
        {
            object valor;

            if (!resposta.TryGetValue(
                    campo,
                    out valor) ||
                valor == null)
            {
                return string.Empty;
            }

            return Convert
                .ToString(valor)
                .Trim();
        }


        private static string LerRespostaErro(
            WebException ex)
        {
            if (ex.Response == null)
            {
                return string.Empty;
            }

            try
            {
                using (Stream stream =
                    ex.Response.GetResponseStream())
                {
                    if (stream == null)
                    {
                        return string.Empty;
                    }

                    using (StreamReader reader =
                        new StreamReader(stream))
                    {
                        string resposta =
                            reader.ReadToEnd();

                        if (resposta.Length > 500)
                        {
                            resposta =
                                resposta.Substring(
                                    0,
                                    500
                                );
                        }

                        return resposta;
                    }
                }
            }
            catch
            {
                return string.Empty;
            }
        }


        private static ResultadoValidacaoNifApi
            CriarResultado(
                EstadoValidacaoNifApi estado,
                string mensagem)
        {
            return new ResultadoValidacaoNifApi
            {
                Estado = estado,
                Mensagem = mensagem
            };
        }

        #endregion
    }
}