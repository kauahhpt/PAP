using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AlunoGest.encarregado
{
    public partial class MinhaConta : Page
    {
        private readonly string _connectionString =
            ConfigurationManager
                .ConnectionStrings["DefaultConnection"]
                .ConnectionString;


        #region Página

        protected void Page_Load(
            object sender,
            EventArgs e)
        {
            if (!Request.IsAuthenticated ||
                !Roles.IsUserInRole(
                    User.Identity.Name,
                    "encarregado"))
            {
                TerminarSessao();
                return;
            }

            if (!IsPostBack)
            {
                LimparMensagem();

                try
                {
                    CarregarDadosConta();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.TraceError(
                        "Erro ao carregar a conta do encarregado: " +
                        ex
                    );

                    MostrarMensagem(
                        "Não foi possível carregar os dados da conta."
                    );
                }
            }
        }

        #endregion


        #region Utilizador atual

        private MembershipUser ObterUtilizadorAtual()
        {
            MembershipUser utilizador =
                Membership.GetUser(
                    User.Identity.Name,
                    false
                );

            if (utilizador == null ||
                utilizador.ProviderUserKey == null)
            {
                throw new InvalidOperationException(
                    "Não foi possível identificar o utilizador."
                );
            }

            return utilizador;
        }

        private Guid ObterUserIdAtual()
        {
            MembershipUser utilizador =
                ObterUtilizadorAtual();

            Guid userId;

            if (!Guid.TryParse(
                    utilizador
                        .ProviderUserKey
                        .ToString(),
                    out userId))
            {
                throw new InvalidOperationException(
                    "O identificador da conta não é válido."
                );
            }

            Session["UserId"] = userId;

            return userId;
        }

        #endregion


        #region Carregar dados

        private void CarregarDadosConta()
        {
            Guid userId =
                ObterUserIdAtual();

            const string sql = @"
                SELECT TOP 1
                    e.Id,
                    e.NomeCompleto,
                    e.Email,
                    e.Telefone,
                    e.NIF,
                    e.Ativo,
                    e.CreatedAt,
                    u.UserName

                FROM dbo.EncarregadoEducacao e

                INNER JOIN dbo.Users u
                    ON u.UserId = e.UserId

                WHERE e.UserId = @UserId;";

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

                using (SqlDataReader reader =
                    cmd.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        throw new InvalidOperationException(
                            "Os dados do encarregado não foram encontrados."
                        );
                    }

                    string nomeCompleto =
                        ValorTexto(
                            reader["NomeCompleto"]
                        );

                    string email =
                        ValorTexto(
                            reader["Email"]
                        );

                    string telefone =
                        ValorTexto(
                            reader["Telefone"]
                        );

                    string nif =
                        ValorTexto(
                            reader["NIF"]
                        );

                    string username =
                        ValorTexto(
                            reader["UserName"]
                        );

                    bool ativo =
                        reader["Ativo"] != DBNull.Value &&
                        Convert.ToBoolean(
                            reader["Ativo"]
                        );

                    DateTime? dataCriacao =
                        reader["CreatedAt"] == DBNull.Value
                            ? (DateTime?)null
                            : Convert.ToDateTime(
                                reader["CreatedAt"]
                            );

                    PreencherInterface(
                        nomeCompleto,
                        email,
                        telefone,
                        nif,
                        username,
                        ativo,
                        dataCriacao
                    );
                }
            }
        }

        private void PreencherInterface(
            string nomeCompleto,
            string email,
            string telefone,
            string nif,
            string username,
            bool ativo,
            DateTime? dataCriacao)
        {
            LblNomeCabecalho.Text =
                Codificar(
                    TextoOuNaoDefinido(
                        nomeCompleto
                    )
                );

            LblIniciais.Text =
                Codificar(
                    ObterIniciais(
                        nomeCompleto
                    )
                );

            LblUsername.Text =
                Codificar(
                    TextoOuNaoDefinido(
                        username
                    )
                );

            LblNIF.Text =
                Codificar(
                    TextoOuNaoDefinido(
                        nif
                    )
                );

            LblEmail.Text =
                Codificar(
                    TextoOuNaoDefinido(
                        email
                    )
                );

            LblTelefone.Text =
                Codificar(
                    TextoOuNaoDefinido(
                        telefone
                    )
                );

            LblEstado.Text =
                ativo
                    ? "Ativo"
                    : "Inativo";

            LblEstado.CssClass =
                ativo
                    ? "estado-ativo"
                    : "estado-inativo";

            LblDataCriacao.Text =
                dataCriacao.HasValue
                    ? dataCriacao.Value
                        .ToString(
                            "dd/MM/yyyy HH:mm"
                        )
                    : "Não definido";

            TxtNomeCompleto.Text =
                nomeCompleto;

            TxtEmail.Text =
                email;

            TxtTelefone.Text =
                telefone;
        }

        #endregion


        #region Alterar dados pessoais

        protected void BtnGuardarDados_Click(
            object sender,
            EventArgs e)
        {
            LimparMensagem();

            Page.Validate("dados");

            if (!Page.IsValid)
            {
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

            try
            {
                MembershipUser utilizador =
                    ObterUtilizadorAtual();

                Guid userId =
                    ObterUserIdAtual();

                if (EmailPertenceAOutroUtilizador(
                        email,
                        utilizador.UserName))
                {
                    MostrarMensagem(
                        "Já existe outra conta associada a este email."
                    );

                    return;
                }

                if (EmailPertenceAOutroEncarregado(
                        email,
                        userId))
                {
                    MostrarMensagem(
                        "Já existe outro encarregado de educação " +
                        "com este email."
                    );

                    return;
                }

                AtualizarDados(
                    utilizador,
                    userId,
                    nomeCompleto,
                    email,
                    telefone
                );

                CarregarDadosConta();

                AtualizarNomeNaMaster(
                    nomeCompleto
                );

                MostrarMensagem(
                    "Os seus dados foram atualizados com sucesso.",
                    false
                );
            }
            catch (SqlException ex)
            {
                System.Diagnostics.Trace.TraceError(
                    "Erro SQL ao atualizar encarregado: " +
                    ex
                );

                MostrarMensagem(
                    "Não foi possível atualizar os dados na base de dados."
                );
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError(
                    "Erro ao atualizar encarregado: " +
                    ex
                );

                MostrarMensagem(
                    "Não foi possível atualizar os dados da conta: " +
                    ex.Message
                );
            }
        }

        private void AtualizarDados(
            MembershipUser utilizador,
            Guid userId,
            string nomeCompleto,
            string email,
            string telefone)
        {
            string emailAnterior =
                utilizador.Email;

            utilizador.Email =
                email;

            Membership.UpdateUser(
                utilizador
            );

            try
            {
                const string sql = @"
                    UPDATE dbo.EncarregadoEducacao

                    SET
                        NomeCompleto = @NomeCompleto,
                        Email = @Email,
                        Telefone = @Telefone

                    WHERE UserId = @UserId;";

                using (SqlConnection conn =
                    new SqlConnection(_connectionString))
                using (SqlCommand cmd =
                    new SqlCommand(sql, conn))
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
                        .Value = telefone;

                    cmd.Parameters
                        .Add(
                            "@UserId",
                            SqlDbType.UniqueIdentifier
                        )
                        .Value = userId;

                    conn.Open();

                    int linhasAlteradas =
                        cmd.ExecuteNonQuery();

                    if (linhasAlteradas != 1)
                    {
                        throw new InvalidOperationException(
                            "O encarregado não foi encontrado."
                        );
                    }
                }
            }
            catch
            {
                /*
                 * Se a atualização da tabela falhar,
                 * tentamos repor o email anterior no Membership.
                 */
                try
                {
                    utilizador.Email =
                        emailAnterior;

                    Membership.UpdateUser(
                        utilizador
                    );
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.TraceError(
                        "Erro ao repor email do Membership: " +
                        ex
                    );
                }

                throw;
            }
        }

        private bool EmailPertenceAOutroUtilizador(
            string email,
            string usernameAtual)
        {
            string usernameEncontrado =
                Membership.GetUserNameByEmail(
                    email
                );

            if (string.IsNullOrWhiteSpace(
                    usernameEncontrado))
            {
                return false;
            }

            return !string.Equals(
                usernameEncontrado,
                usernameAtual,
                StringComparison.OrdinalIgnoreCase
            );
        }

        private bool EmailPertenceAOutroEncarregado(
            string email,
            Guid userIdAtual)
        {
            const string sql = @"
                SELECT
                    CASE
                        WHEN EXISTS
                        (
                            SELECT 1

                            FROM dbo.EncarregadoEducacao

                            WHERE Email = @Email
                              AND UserId <> @UserId
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
                        "@UserId",
                        SqlDbType.UniqueIdentifier
                    )
                    .Value = userIdAtual;

                conn.Open();

                return Convert.ToInt32(
                    cmd.ExecuteScalar()
                ) == 1;
            }
        }

        protected void BtnCancelarDados_Click(
            object sender,
            EventArgs e)
        {
            LimparMensagem();

            try
            {
                CarregarDadosConta();
            }
            catch (Exception ex)
            {
                MostrarMensagem(
                    "Não foi possível repor os dados: " +
                    ex.Message
                );
            }
        }

        private void AtualizarNomeNaMaster(
            string nomeCompleto)
        {
            if (Master == null)
            {
                return;
            }

            Label labelEncarregado =
                Master.FindControl(
                    "LblEncarregado"
                ) as Label;

            if (labelEncarregado != null)
            {
                labelEncarregado.Text =
                    Codificar(
                        nomeCompleto
                    );
            }
        }

        #endregion


        #region Alterar palavra-passe

        protected void BtnAlterarSenha_Click(
            object sender,
            EventArgs e)
        {
            LimparMensagem();

            Page.Validate("senha");

            if (!Page.IsValid)
            {
                return;
            }

            string senhaAtual =
                TxtSenhaAtual.Text;

            string novaSenha =
                TxtNovaSenha.Text;

            string confirmacao =
                TxtConfirmarSenha.Text;

            if (novaSenha != confirmacao)
            {
                MostrarMensagem(
                    "A nova palavra-passe e a confirmação não coincidem."
                );

                return;
            }

            string mensagemSenha;

            if (!SenhaForte(
                    novaSenha,
                    out mensagemSenha))
            {
                MostrarMensagem(
                    mensagemSenha
                );

                return;
            }

            try
            {
                MembershipUser utilizador =
                    ObterUtilizadorAtual();

                if (utilizador.IsLockedOut)
                {
                    utilizador.UnlockUser();
                }

                bool alterou =
                    utilizador.ChangePassword(
                        senhaAtual,
                        novaSenha
                    );

                if (!alterou)
                {
                    MostrarMensagem(
                        "Não foi possível alterar a palavra-passe."
                    );

                    return;
                }

                LimparCamposSenha();

                MostrarMensagem(
                    "A palavra-passe foi alterada com sucesso.",
                    false
                );
            }
            catch (MembershipPasswordException)
            {
                MostrarMensagem(
                    "A palavra-passe atual está incorreta " +
                    "ou a nova palavra-passe não cumpre os requisitos."
                );
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError(
                    "Erro ao alterar palavra-passe: " +
                    ex
                );

                MostrarMensagem(
                    "Não foi possível alterar a palavra-passe: " +
                    ex.Message
                );
            }
        }

        private bool SenhaForte(
            string senha,
            out string mensagem)
        {
            mensagem =
                string.Empty;

            int tamanhoMinimo =
                Math.Max(
                    8,
                    Membership.MinRequiredPasswordLength
                );

            if (string.IsNullOrWhiteSpace(senha) ||
                senha.Length < tamanhoMinimo)
            {
                mensagem =
                    "A palavra-passe deve ter pelo menos " +
                    tamanhoMinimo +
                    " caracteres.";

                return false;
            }

            if (!Regex.IsMatch(
                    senha,
                    "[A-Z]"))
            {
                mensagem =
                    "A palavra-passe deve conter pelo menos " +
                    "uma letra maiúscula.";

                return false;
            }

            if (!Regex.IsMatch(
                    senha,
                    "[a-z]"))
            {
                mensagem =
                    "A palavra-passe deve conter pelo menos " +
                    "uma letra minúscula.";

                return false;
            }

            if (!Regex.IsMatch(
                    senha,
                    "[0-9]"))
            {
                mensagem =
                    "A palavra-passe deve conter pelo menos um número.";

                return false;
            }

            if (!Regex.IsMatch(
                    senha,
                    @"[\W_]"))
            {
                mensagem =
                    "A palavra-passe deve conter pelo menos " +
                    "um carácter especial.";

                return false;
            }

            return true;
        }

        private void LimparCamposSenha()
        {
            TxtSenhaAtual.Text =
                string.Empty;

            TxtNovaSenha.Text =
                string.Empty;

            TxtConfirmarSenha.Text =
                string.Empty;
        }

        #endregion


        #region Utilidades

        private string ValorTexto(
            object valor)
        {
            if (valor == null ||
                valor == DBNull.Value)
            {
                return string.Empty;
            }

            return valor
                .ToString()
                .Trim();
        }

        private string TextoOuNaoDefinido(
            string valor)
        {
            return string.IsNullOrWhiteSpace(
                    valor)
                ? "Não definido"
                : valor.Trim();
        }

        private string Codificar(
            string valor)
        {
            return HttpUtility.HtmlEncode(
                valor ?? string.Empty
            );
        }

        private string ObterIniciais(
            string nomeCompleto)
        {
            if (string.IsNullOrWhiteSpace(
                    nomeCompleto))
            {
                return "?";
            }

            string[] partes =
                nomeCompleto.Split(
                    new[] { ' ' },
                    StringSplitOptions.RemoveEmptyEntries
                );

            if (partes.Length == 0)
            {
                return "?";
            }

            if (partes.Length == 1)
            {
                return partes[0]
                    .Substring(0, 1)
                    .ToUpperInvariant();
            }

            return
                (
                    partes[0].Substring(0, 1) +
                    partes[partes.Length - 1]
                        .Substring(0, 1)
                )
                .ToUpperInvariant();
        }

        private void MostrarMensagem(
            string mensagem,
            bool erro = true)
        {
            LblMensagem.Visible =
                true;

            LblMensagem.Text =
                mensagem;

            LblMensagem.CssClass =
                erro
                    ? "alert alert-warning d-block"
                    : "alert alert-success d-block";
        }

        private void LimparMensagem()
        {
            LblMensagem.Visible =
                false;

            LblMensagem.Text =
                string.Empty;

            LblMensagem.CssClass =
                string.Empty;
        }

        private void TerminarSessao()
        {
            FormsAuthentication.SignOut();

            Session.Clear();
            Session.Abandon();

            Response.Redirect(
                "~/login.aspx"
            );
        }

        #endregion
    }
}