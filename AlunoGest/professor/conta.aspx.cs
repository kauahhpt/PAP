using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;
using System.Web.Security;

namespace AlunoGest.professor
{
    public partial class conta : System.Web.UI.Page
    {
        private readonly string _connectionString =
            ConfigurationManager
                .ConnectionStrings["DefaultConnection"]
                .ConnectionString;


        #region Página

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LblMensagem.Visible = false;

                MembershipUser user = Membership.GetUser();

                if (user == null)
                {
                    FormsAuthentication.RedirectToLoginPage();
                    return;
                }

                PainelPrimeiroAcesso.Visible =
                    !string.Equals(
                        user.Comment,
                        "CREDENCIAIS_ATUALIZADAS",
                        StringComparison.Ordinal
                    );

                CarregarDadosProfessor();
            }
        }

        #endregion


        #region Professor autenticado

        private int ProfessorId
        {
            get
            {
                int professorId;

                if (Session["ProfessorID"] != null &&
                    int.TryParse(
                        Session["ProfessorID"].ToString(),
                        out professorId
                    ))
                {
                    return professorId;
                }


                if (Session["UserId"] == null)
                {
                    throw new InvalidOperationException(
                        "Sessão do professor inválida."
                    );
                }


                Guid userId;

                if (!Guid.TryParse(
                        Session["UserId"].ToString(),
                        out userId
                    ))
                {
                    throw new InvalidOperationException(
                        "O identificador do utilizador é inválido."
                    );
                }


                const string sql = @"
                    SELECT Id
                    FROM dbo.Professor
                    WHERE UserId = @UserId
                      AND Ativo = 1;";


                using (SqlConnection conn =
                    new SqlConnection(_connectionString))
                using (SqlCommand cmd =
                    new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add(
                        "@UserId",
                        SqlDbType.UniqueIdentifier
                    ).Value = userId;


                    conn.Open();


                    object resultado =
                        cmd.ExecuteScalar();


                    if (resultado == null ||
                        resultado == DBNull.Value)
                    {
                        throw new InvalidOperationException(
                            "Professor não encontrado."
                        );
                    }


                    professorId =
                        Convert.ToInt32(resultado);


                    Session["ProfessorID"] =
                        professorId;


                    return professorId;
                }
            }
        }

        #endregion


        #region Carregar dados

        private void CarregarDadosProfessor()
        {
            if (Session["UserId"] == null)
            {
                Response.Redirect("~/login.aspx");
                return;
            }

            Guid userId;

            if (!Guid.TryParse(
                    Session["UserId"].ToString(),
                    out userId))
            {
                MostrarMensagem(
                    "A sessão do utilizador é inválida.",
                    true
                );

                return;
            }

            const string sql = @"
        SELECT
            p.Nome,
            p.NumeroProcesso,
            p.Email,
            p.Foto,
            p.Telefone,
            p.NIF,
            u.UserName

        FROM dbo.Professor p

        INNER JOIN dbo.Users u
            ON u.UserId = p.UserId

        WHERE p.UserId = @UserId
          AND p.Ativo = 1;";

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
                        MostrarMensagem(
                            "Não foi possível encontrar os dados do professor.",
                            true
                        );

                        return;
                    }

                    LblNomeCompleto.Text =
                        reader["Nome"].ToString();

                    LblNumeroProcesso.Text =
                        reader["NumeroProcesso"] == DBNull.Value
                            ? "Não definido"
                            : reader["NumeroProcesso"].ToString();

                    LblEmail.Text =
                        reader["Email"] == DBNull.Value
                            ? "Não definido"
                            : reader["Email"].ToString();

                    LblTelefone.Text =
                        reader["Telefone"] == DBNull.Value ||
                        string.IsNullOrWhiteSpace(
                            reader["Telefone"].ToString())
                            ? "Não definido"
                            : reader["Telefone"].ToString();

                    LblNIF.Text =
                        reader["NIF"] == DBNull.Value ||
                        string.IsNullOrWhiteSpace(
                            reader["NIF"].ToString())
                            ? "Não definido"
                            : reader["NIF"].ToString();

                    LblUsername.Text =
                        reader["UserName"].ToString();

                    TxtUsername.Text =
                        reader["UserName"].ToString();

                    string foto =
                        reader["Foto"] == DBNull.Value
                            ? string.Empty
                            : reader["Foto"].ToString();

                    ImgFotoPerfil.ImageUrl =
                        string.IsNullOrWhiteSpace(foto)
                            ? "~/default-user.png"
                            : foto;
                }
            }
        }

        #endregion


        #region Alterar fotografia

        protected void BtnAlterarFoto_Click(
            object sender,
            EventArgs e)
        {
            LimparMensagem();


            if (!FileFoto.HasFile)
            {
                MostrarMensagem(
                    "Escolha uma imagem.",
                    true
                );

                return;
            }


            string extensao =
                Path
                    .GetExtension(FileFoto.FileName)
                    .ToLowerInvariant();


            if (extensao != ".jpg" &&
                extensao != ".jpeg" &&
                extensao != ".png")
            {
                MostrarMensagem(
                    "A fotografia deve ser JPG, JPEG ou PNG.",
                    true
                );

                return;
            }


            if (FileFoto.PostedFile.ContentLength >
                5 * 1024 * 1024)
            {
                MostrarMensagem(
                    "A imagem não pode ter mais de 5 MB.",
                    true
                );

                return;
            }


            try
            {
                string pastaFisica =
                    Server.MapPath(
                        "~/uploads/perfis/"
                    );


                if (!Directory.Exists(pastaFisica))
                {
                    Directory.CreateDirectory(
                        pastaFisica
                    );
                }


                string nomeFicheiro =
                    Guid.NewGuid().ToString("N") +
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


                const string sql = @"
                    UPDATE dbo.Professor

                    SET Foto = @Foto

                    WHERE Id = @ProfessorId
                      AND Ativo = 1;";


                using (SqlConnection conn =
                    new SqlConnection(_connectionString))
                using (SqlCommand cmd =
                    new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add(
                        "@Foto",
                        SqlDbType.NVarChar,
                        255
                    ).Value = caminhoRelativo;


                    cmd.Parameters.Add(
                        "@ProfessorId",
                        SqlDbType.Int
                    ).Value = ProfessorId;


                    conn.Open();


                    if (cmd.ExecuteNonQuery() != 1)
                    {
                        throw new InvalidOperationException(
                            "O professor autenticado não foi encontrado."
                        );
                    }
                }


                MostrarMensagem(
                    "Fotografia alterada com sucesso.",
                    false
                );


                CarregarDadosProfessor();
            }
            catch (Exception ex)
            {
                MostrarMensagem(
                    "Erro ao alterar a fotografia: " +
                    ex.Message,
                    true
                );
            }
        }

        #endregion


        #region Alterar credenciais

        protected void BtnGuardar_Click(
            object sender,
            EventArgs e)
        {
            LimparMensagem();


            if (!Page.IsValid)
            {
                return;
            }


            MembershipUser utilizadorAtual =
                Membership.GetUser();


            if (utilizadorAtual == null)
            {
                MostrarMensagem(
                    "Utilizador não encontrado.",
                    true
                );

                return;
            }


            if (!Membership.ValidateUser(
                    utilizadorAtual.UserName,
                    TxtPasswordAtual.Text
                ))
            {
                MostrarMensagem(
                    "A palavra-passe atual está incorreta.",
                    true
                );

                return;
            }


            string novoUsername =
                TxtUsername
                    .Text
                    .Trim()
                    .ToLowerInvariant();


            string novaPassword =
                TxtNovaPassword.Text;


            string mensagemSenha;


            if (!SenhaForte(
                    novaPassword,
                    out mensagemSenha
                ))
            {
                MostrarMensagem(
                    mensagemSenha,
                    true
                );

                return;
            }


            try
            {
                bool mesmoUsername =
                    string.Equals(
                        novoUsername,
                        utilizadorAtual.UserName,
                        StringComparison.OrdinalIgnoreCase
                    );


                if (mesmoUsername)
                {
                    AlterarApenasPassword(
                        utilizadorAtual,
                        novaPassword
                    );
                }
                else
                {
                    AlterarUsernameEPassword(
                        utilizadorAtual,
                        novoUsername,
                        novaPassword
                    );
                }


                Session.Clear();

                FormsAuthentication.SignOut();


                Response.Redirect(
                    "~/login.aspx?credenciais=alteradas"
                );
            }
            catch (Exception ex)
            {
                MostrarMensagem(
                    "Não foi possível alterar as credenciais: " +
                    ex.Message,
                    true
                );
            }
        }


        private void AlterarApenasPassword(
            MembershipUser utilizador,
            string novaPassword)
        {
            bool alterou =
                utilizador.ChangePassword(
                    TxtPasswordAtual.Text,
                    novaPassword
                );


            if (!alterou)
            {
                throw new InvalidOperationException(
                    "Não foi possível alterar a palavra-passe."
                );
            }


            utilizador.Comment =
                "CREDENCIAIS_ATUALIZADAS";


            Membership.UpdateUser(
                utilizador
            );
        }


        private void AlterarUsernameEPassword(
            MembershipUser utilizadorAtual,
            string novoUsername,
            string novaPassword)
        {
            MembershipCreateStatus status;


            MembershipUser novoUtilizador =
                Membership.CreateUser(
                    novoUsername,
                    novaPassword,
                    utilizadorAtual.Email,
                    null,
                    null,
                    true,
                    null,
                    out status
                );


            if (status != MembershipCreateStatus.Success ||
                novoUtilizador == null)
            {
                throw new InvalidOperationException(
                    "O novo utilizador não está disponível (" +
                    status +
                    ")."
                );
            }


            try
            {
                Roles.AddUserToRole(
                    novoUsername,
                    "professor"
                );


                novoUtilizador.Comment =
                    "CREDENCIAIS_ATUALIZADAS";


                Membership.UpdateUser(
                    novoUtilizador
                );


                const string sql = @"
                    UPDATE dbo.Professor

                    SET UserId = @NovoUserId

                    WHERE UserId = @UserId
                      AND Id = @ProfessorId
                      AND Ativo = 1;";


                using (SqlConnection conn =
                    new SqlConnection(_connectionString))
                using (SqlCommand cmd =
                    new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add(
                        "@NovoUserId",
                        SqlDbType.UniqueIdentifier
                    ).Value =
                        (Guid)novoUtilizador.ProviderUserKey;


                    cmd.Parameters.Add(
                        "@UserId",
                        SqlDbType.UniqueIdentifier
                    ).Value =
                        (Guid)utilizadorAtual.ProviderUserKey;


                    cmd.Parameters.Add(
                        "@ProfessorId",
                        SqlDbType.Int
                    ).Value =
                        ProfessorId;


                    conn.Open();


                    if (cmd.ExecuteNonQuery() != 1)
                    {
                        throw new InvalidOperationException(
                            "O professor autenticado não foi encontrado."
                        );
                    }
                }
            }
            catch
            {
                Membership.DeleteUser(
                    novoUsername,
                    true
                );

                throw;
            }


            try
            {
                Membership.DeleteUser(
                    utilizadorAtual.UserName,
                    true
                );
            }
            catch
            {
                utilizadorAtual.IsApproved =
                    false;


                Membership.UpdateUser(
                    utilizadorAtual
                );
            }
        }

        #endregion


        #region Validação da password

        private bool SenhaForte(
            string senha,
            out string mensagem)
        {
            mensagem = "";


            if (string.IsNullOrWhiteSpace(senha) ||
                senha.Length < 8)
            {
                mensagem =
                    "A palavra-passe deve ter pelo menos 8 caracteres.";

                return false;
            }


            if (!Regex.IsMatch(
                    senha,
                    "[A-Z]"
                ))
            {
                mensagem =
                    "A palavra-passe deve ter pelo menos uma letra maiúscula.";

                return false;
            }


            if (!Regex.IsMatch(
                    senha,
                    "[a-z]"
                ))
            {
                mensagem =
                    "A palavra-passe deve ter pelo menos uma letra minúscula.";

                return false;
            }


            if (!Regex.IsMatch(
                    senha,
                    "[0-9]"
                ))
            {
                mensagem =
                    "A palavra-passe deve ter pelo menos um número.";

                return false;
            }


            if (!Regex.IsMatch(
                    senha,
                    @"[\W_]"
                ))
            {
                mensagem =
                    "A palavra-passe deve ter pelo menos um caráter especial.";

                return false;
            }


            return true;
        }

        #endregion


        #region Mensagens

        private void MostrarMensagem(
            string texto,
            bool erro)
        {
            LblMensagem.Text =
                texto;


            LblMensagem.CssClass =
                erro
                    ? "alert alert-warning d-block"
                    : "alert alert-success d-block";


            LblMensagem.Visible =
                true;
        }


        private void LimparMensagem()
        {
            LblMensagem.Visible =
                false;


            LblMensagem.Text =
                "";
        }

        #endregion
    }
}   