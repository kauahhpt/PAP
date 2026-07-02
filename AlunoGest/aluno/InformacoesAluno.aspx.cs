using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Web.Security;
using System.Text.RegularExpressions;

namespace AlunoGest.aluno
{
    public partial class InformacoesAluno : System.Web.UI.Page
    {
        private readonly string _ConnectionString =
            ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        private bool SenhaForte(string senha, out string mensagem)
        {
            mensagem = "";

            if (senha.Length < 8)
            {
                mensagem = "A senha deve ter pelo menos 8 caracteres.";
                return false;
            }

            if (!Regex.IsMatch(senha, "[A-Z]"))
            {
                mensagem = "A senha deve ter pelo menos uma letra maiúscula.";
                return false;
            }

            if (!Regex.IsMatch(senha, "[a-z]"))
            {
                mensagem = "A senha deve ter pelo menos uma letra minúscula.";
                return false;
            }

            if (!Regex.IsMatch(senha, "[0-9]"))
            {
                mensagem = "A senha deve ter pelo menos um número.";
                return false;
            }

            if (!Regex.IsMatch(senha, @"[\W_]"))
            {
                mensagem = "A senha deve ter pelo menos um caractere especial.";
                return false;
            }

            return true;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LblMensagem.Visible = false;
                CarregarDadosAluno();
            }
        }

        private void CarregarDadosAluno()
        {
            if (Session["UserId"] == null)
            {
                Response.Redirect("~/login.aspx");
                return;
            }

            Guid userId = new Guid(Session["UserId"].ToString());

            string sql = @"
                SELECT 
                    a.NomeCompleto,
                    a.NumeroProcesso,
                    a.Foto,
                    u.UserName
                FROM dbo.Aluno a
                INNER JOIN dbo.Users u ON u.UserId = a.UserId
                WHERE a.UserId = @UserId;";

            using (SqlConnection conn = new SqlConnection(_ConnectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@UserId", userId);
                conn.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        LblNomeCompleto.Text = dr["NomeCompleto"].ToString();
                        LblNumeroProcesso.Text = dr["NumeroProcesso"].ToString();
                        LblUsername.Text = dr["UserName"].ToString();

                        string foto = dr["Foto"].ToString();

                        ImgFotoPerfil.ImageUrl = string.IsNullOrWhiteSpace(foto)
                            ? "~/default-user.png"
                            : foto;
                    }
                }
            }
        }

        protected void BtnAlterarFoto_Click(object sender, EventArgs e)
        {
            LimparMensagem();

            if (!FileFoto.HasFile)
            {
                MostrarMensagem("Escolhe uma imagem.");
                return;
            }

            string extensao = Path.GetExtension(FileFoto.FileName).ToLower();

            if (extensao != ".jpg" && extensao != ".jpeg" && extensao != ".png")
            {
                MostrarMensagem("A foto deve ser JPG, JPEG ou PNG.");
                return;
            }

            if (FileFoto.PostedFile.ContentLength > 5 * 1024 * 1024)
            {
                MostrarMensagem("A imagem não pode ter mais de 5 MB.");
                return;
            }

            try
            {
                string pasta = Server.MapPath("~/uploads/perfis/");

                if (!Directory.Exists(pasta))
                    Directory.CreateDirectory(pasta);

                string nomeFicheiro = Guid.NewGuid().ToString("N") + extensao;
                string caminhoFisico = Path.Combine(pasta, nomeFicheiro);
                string caminhoRelativo = "~/uploads/perfis/" + nomeFicheiro;

                FileFoto.SaveAs(caminhoFisico);

                Guid userId = new Guid(Session["UserId"].ToString());

                string sql = @"
                    UPDATE dbo.Aluno
                    SET Foto = @Foto
                    WHERE UserId = @UserId;";

                using (SqlConnection conn = new SqlConnection(_ConnectionString))
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Foto", caminhoRelativo);
                    cmd.Parameters.AddWithValue("@UserId", userId);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                MostrarMensagem("Foto alterada com sucesso.", false);
                CarregarDadosAluno();
            }
            catch (Exception ex)
            {
                MostrarMensagem("Erro ao alterar foto: " + ex.Message);
            }
        }

        protected void BtnAlterarSenha_Click(object sender, EventArgs e)
        {
            LimparMensagem();

            if (TxtSenhaAtual.Text.Trim() == "" ||
                TxtNovaSenha.Text.Trim() == "" ||
                TxtConfirmarSenha.Text.Trim() == "")
            {
                MostrarMensagem("Preenche todos os campos da senha.");
                return;
            }

            if (TxtNovaSenha.Text != TxtConfirmarSenha.Text)
            {
                MostrarMensagem("A nova senha e a confirmação não coincidem.");
                return;
            }

            string mensagemSenha;

            if (!SenhaForte(TxtNovaSenha.Text, out mensagemSenha))
            {
                MostrarMensagem(mensagemSenha);
                return;
            }

            try
            {
                MembershipUser user = Membership.GetUser();

                if (user == null)
                {
                    MostrarMensagem("Utilizador não encontrado.");
                    return;
                }

                bool alterou = user.ChangePassword(TxtSenhaAtual.Text, TxtNovaSenha.Text);

                if (alterou)
                {
                    MostrarMensagem("Senha alterada com sucesso.", false);
                    TxtSenhaAtual.Text = "";
                    TxtNovaSenha.Text = "";
                    TxtConfirmarSenha.Text = "";
                }
                else
                {
                    MostrarMensagem("Não foi possível alterar a senha. Verifica a senha atual.");
                }
            }
            catch (Exception ex)
            {
                MostrarMensagem("Erro ao alterar senha: " + ex.Message);
            }
        }

        private void MostrarMensagem(string mensagem, bool erro = true)
        {
            LblMensagem.Visible = true;
            LblMensagem.Text = mensagem;
            LblMensagem.CssClass = erro
                ? "alert alert-warning d-block"
                : "alert alert-success d-block";
        }

        private void LimparMensagem()
        {
            LblMensagem.Visible = false;
            LblMensagem.Text = "";
        }
    }
}