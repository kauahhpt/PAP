using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;
using System.Web.Security;

namespace AlunoGest.aluno
{
    public partial class InformacoesAluno : System.Web.UI.Page
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
            if (Session["UserId"] == null)
            {
                Response.Redirect(
                    "~/login.aspx"
                );

                return;
            }

            if (!IsPostBack)
            {
                LimparMensagem();

                try
                {
                    CarregarDadosAluno();
                }
                catch (Exception ex)
                {
                    MostrarMensagem(
                        "Não foi possível carregar os dados: " +
                        ex.Message
                    );
                }
            }
        }

        #endregion


        #region Sessão e utilizador autenticado

        private Guid UserIdAtual
        {
            get
            {
                if (Session["UserId"] == null)
                {
                    throw new InvalidOperationException(
                        "A sessão terminou. Faz login novamente."
                    );
                }

                Guid userId;

                if (!Guid.TryParse(
                        Session["UserId"].ToString(),
                        out userId))
                {
                    throw new InvalidOperationException(
                        "O utilizador guardado na sessão é inválido."
                    );
                }

                return userId;
            }
        }

        #endregion


        #region Carregar dados do aluno

        private void CarregarDadosAluno()
        {
            const string sql = @"
                SELECT
                    a.NomeCompleto,
                    a.NumeroProcesso,
                    a.Telefone,
                    a.NIF,
                    a.Foto,
                    u.UserName

                FROM dbo.Aluno a

                INNER JOIN dbo.Users u
                    ON u.UserId = a.UserId

                WHERE a.UserId = @UserId
                  AND a.Ativo = 1;";

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
                    .Value = UserIdAtual;

                conn.Open();

                using (SqlDataReader reader =
                    cmd.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        MostrarMensagem(
                            "Não foi possível encontrar os dados do aluno."
                        );

                        return;
                    }

                    LblNomeCompleto.Text =
                        ValorOuNaoDefinido(
                            reader["NomeCompleto"]
                        );

                    LblNumeroProcesso.Text =
                        ValorOuNaoDefinido(
                            reader["NumeroProcesso"]
                        );

                    LblUsername.Text =
                        ValorOuNaoDefinido(
                            reader["UserName"]
                        );

                    LblTelefone.Text =
                        ValorOuNaoDefinido(
                            reader["Telefone"]
                        );

                    LblNIF.Text =
                        ValorOuNaoDefinido(
                            reader["NIF"]
                        );

                    string foto =
                        reader["Foto"] == DBNull.Value
                            ? string.Empty
                            : reader["Foto"]
                                .ToString()
                                .Trim();

                    ImgFotoPerfil.ImageUrl =
                        string.IsNullOrWhiteSpace(foto)
                            ? ResolveUrl(
                                "~/default-user.png"
                            )
                            : ResolveUrl(foto);
                }
            }
        }

        #endregion


        #region Fotografia de perfil

        protected void BtnAlterarFoto_Click(
            object sender,
            EventArgs e)
        {
            LimparMensagem();

            if (!FileFoto.HasFile)
            {
                MostrarMensagem(
                    "Escolhe uma imagem."
                );

                return;
            }

            string extensao =
                Path.GetExtension(
                    FileFoto.FileName
                )
                .ToLowerInvariant();

            string[] extensoesPermitidas =
            {
                ".jpg",
                ".jpeg",
                ".png"
            };

            if (Array.IndexOf(
                    extensoesPermitidas,
                    extensao) < 0)
            {
                MostrarMensagem(
                    "A fotografia deve estar no formato JPG, JPEG ou PNG."
                );

                return;
            }

            const int tamanhoMaximo =
                5 * 1024 * 1024;

            if (FileFoto.PostedFile.ContentLength >
                tamanhoMaximo)
            {
                MostrarMensagem(
                    "A imagem não pode ter mais de 5 MB."
                );

                return;
            }

            try
            {
                string pastaFisica =
                    Server.MapPath(
                        "~/uploads/perfis/"
                    );

                if (!Directory.Exists(
                        pastaFisica))
                {
                    Directory.CreateDirectory(
                        pastaFisica
                    );
                }

                string nomeFicheiro =
                    Guid.NewGuid()
                        .ToString("N") +
                    extensao;

                string caminhoFisico =
                    Path.Combine(
                        pastaFisica,
                        nomeFicheiro
                    );

                string caminhoRelativo =
                    "~/uploads/perfis/" +
                    nomeFicheiro;

                FileFoto.SaveAs(
                    caminhoFisico
                );

                AtualizarFotoAluno(
                    caminhoRelativo
                );

                MostrarMensagem(
                    "Fotografia alterada com sucesso.",
                    false
                );

                CarregarDadosAluno();
            }
            catch (Exception ex)
            {
                MostrarMensagem(
                    "Erro ao alterar a fotografia: " +
                    ex.Message
                );
            }
        }


        private void AtualizarFotoAluno(
            string caminhoRelativo)
        {
            const string sql = @"
                UPDATE dbo.Aluno

                SET Foto = @Foto

                WHERE UserId = @UserId
                  AND Ativo = 1;";

            using (SqlConnection conn =
                new SqlConnection(_connectionString))
            using (SqlCommand cmd =
                new SqlCommand(sql, conn))
            {
                cmd.Parameters
                    .Add(
                        "@Foto",
                        SqlDbType.NVarChar,
                        500
                    )
                    .Value = caminhoRelativo;

                cmd.Parameters
                    .Add(
                        "@UserId",
                        SqlDbType.UniqueIdentifier
                    )
                    .Value = UserIdAtual;

                conn.Open();

                int linhasAlteradas =
                    cmd.ExecuteNonQuery();

                if (linhasAlteradas == 0)
                {
                    throw new InvalidOperationException(
                        "O aluno não foi encontrado."
                    );
                }
            }
        }

        #endregion


        #region Alteração da palavra-passe

        protected void BtnAlterarSenha_Click(
            object sender,
            EventArgs e)
        {
            LimparMensagem();

            if (string.IsNullOrWhiteSpace(
                    TxtSenhaAtual.Text) ||
                string.IsNullOrWhiteSpace(
                    TxtNovaSenha.Text) ||
                string.IsNullOrWhiteSpace(
                    TxtConfirmarSenha.Text))
            {
                MostrarMensagem(
                    "Preenche todos os campos da palavra-passe."
                );

                return;
            }

            if (TxtNovaSenha.Text !=
                TxtConfirmarSenha.Text)
            {
                MostrarMensagem(
                    "A nova palavra-passe e a confirmação não coincidem."
                );

                return;
            }

            string mensagemSenha;

            if (!SenhaForte(
                    TxtNovaSenha.Text,
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
                    Membership.GetUser();

                if (utilizador == null)
                {
                    MostrarMensagem(
                        "Utilizador não encontrado."
                    );

                    return;
                }

                bool alterou =
                    utilizador.ChangePassword(
                        TxtSenhaAtual.Text,
                        TxtNovaSenha.Text
                    );

                if (!alterou)
                {
                    MostrarMensagem(
                        "Não foi possível alterar a palavra-passe. " +
                        "Verifica a palavra-passe atual."
                    );

                    return;
                }

                LimparCamposSenha();

                MostrarMensagem(
                    "Palavra-passe alterada com sucesso.",
                    false
                );
            }
            catch (MembershipPasswordException)
            {
                MostrarMensagem(
                    "A palavra-passe atual está incorreta."
                );
            }
            catch (Exception ex)
            {
                MostrarMensagem(
                    "Erro ao alterar a palavra-passe: " +
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

            if (senha.Length < 8)
            {
                mensagem =
                    "A palavra-passe deve ter pelo menos 8 caracteres.";

                return false;
            }

            if (!Regex.IsMatch(
                    senha,
                    "[A-Z]"))
            {
                mensagem =
                    "A palavra-passe deve ter pelo menos uma letra maiúscula.";

                return false;
            }

            if (!Regex.IsMatch(
                    senha,
                    "[a-z]"))
            {
                mensagem =
                    "A palavra-passe deve ter pelo menos uma letra minúscula.";

                return false;
            }

            if (!Regex.IsMatch(
                    senha,
                    "[0-9]"))
            {
                mensagem =
                    "A palavra-passe deve ter pelo menos um número.";

                return false;
            }

            if (!Regex.IsMatch(
                    senha,
                    @"[\W_]"))
            {
                mensagem =
                    "A palavra-passe deve ter pelo menos um carácter especial.";

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


        #region Mensagens

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

        #endregion


        #region Utilidades

        private string ValorOuNaoDefinido(
            object valor)
        {
            if (valor == null ||
                valor == DBNull.Value)
            {
                return "Não definido";
            }

            string texto =
                valor.ToString().Trim();

            return string.IsNullOrWhiteSpace(
                texto)
                    ? "Não definido"
                    : texto;
        }

        #endregion
    }
}