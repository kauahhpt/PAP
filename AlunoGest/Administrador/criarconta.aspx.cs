using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Security;
using System.Web.UI;

namespace AlunoGest
{
    public partial class criarconta : Page
    {
        private readonly string connectionString =
            ConfigurationManager.ConnectionStrings["DefaultConnection"]
                .ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Proteção adicional, para além do Web.config.
            if (!Request.IsAuthenticated ||
                !Roles.IsUserInRole(User.Identity.Name, "administrador"))
            {
                Response.Redirect("~/login.aspx");
                return;
            }
        }

        protected void btnCriarConta_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            lblMensagem.Visible = false;

            string username = txtUsername.Text.Trim();
            MembershipCreateStatus status;

            MembershipUser user = Membership.CreateUser(
                username,
                txtPassword.Text,
                txtEmail.Text.Trim(),
                null,
                null,
                true,
                out status
            );

            if (status != MembershipCreateStatus.Success || user == null)
            {
                MostrarErro(ObterMensagemMembership(status));
                return;
            }

            try
            {
                Roles.AddUserToRole(username, "agrupamento");

                string sql = @"
                    INSERT INTO dbo.Agrupamento
                    (
                        UserId,
                        Nome,
                        Morada,
                        CodigoPostal,
                        Localidade,
                        Telefone,
                        Email,
                        CodigoMEC
                    )
                    VALUES
                    (
                        @UserId,
                        @Nome,
                        @Morada,
                        @CodigoPostal,
                        @Localidade,
                        @Telefone,
                        @Email,
                        @CodigoMEC
                    );";

                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add(
                        "@UserId",
                        SqlDbType.VarChar,
                        50
                    ).Value = user.ProviderUserKey.ToString();

                    cmd.Parameters.Add(
                        "@Nome",
                        SqlDbType.NVarChar,
                        150
                    ).Value = txtNome.Text.Trim();

                    cmd.Parameters.Add(
                        "@Morada",
                        SqlDbType.NVarChar,
                        500
                    ).Value = txtMorada.Text.Trim();

                    cmd.Parameters.Add(
                        "@CodigoPostal",
                        SqlDbType.VarChar,
                        8
                    ).Value = txtCodigoPostal.Text.Trim();

                    cmd.Parameters.Add(
                        "@Localidade",
                        SqlDbType.NVarChar,
                        100
                    ).Value = txtLocalidade.Text.Trim();

                    cmd.Parameters.Add(
                        "@Telefone",
                        SqlDbType.VarChar,
                        20
                    ).Value = string.IsNullOrWhiteSpace(txtTelefone.Text)
                        ? (object)DBNull.Value
                        : txtTelefone.Text.Trim();

                    cmd.Parameters.Add(
                        "@Email",
                        SqlDbType.NVarChar,
                        120
                    ).Value = string.IsNullOrWhiteSpace(txtEmail.Text)
                        ? (object)DBNull.Value
                        : txtEmail.Text.Trim();

                    cmd.Parameters.Add(
                        "@CodigoMEC",
                        SqlDbType.VarChar,
                        50
                    ).Value = string.IsNullOrWhiteSpace(txtCodigoMEC.Text)
                        ? (object)DBNull.Value
                        : txtCodigoMEC.Text.Trim();

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                LimparCampos();

                lblMensagem.Text =
                    "O agrupamento foi criado com sucesso.";

                lblMensagem.CssClass = "message-success";
                lblMensagem.Visible = true;
            }
            catch (Exception)
            {
                // Evita deixar uma conta sem agrupamento caso o INSERT falhe.
                RemoverUtilizadorCriado(username);

                MostrarErro(
                    "Não foi possível criar o agrupamento. " +
                    "Nenhuma conta foi mantida."
                );
            }
        }

        private void RemoverUtilizadorCriado(string username)
        {
            try
            {
                if (Roles.IsUserInRole(username, "agrupamento"))
                {
                    Roles.RemoveUserFromRole(username, "agrupamento");
                }

                Membership.DeleteUser(username, true);
            }
            catch
            {
                // Não substitui o erro original.
            }
        }

        private void LimparCampos()
        {
            txtNome.Text = string.Empty;
            txtEmail.Text = string.Empty;
            txtTelefone.Text = string.Empty;
            txtMorada.Text = string.Empty;
            txtCodigoPostal.Text = string.Empty;
            txtLocalidade.Text = string.Empty;
            txtCodigoMEC.Text = string.Empty;
            txtUsername.Text = string.Empty;
            txtPassword.Text = string.Empty;
        }

        private void MostrarErro(string mensagem)
        {
            lblMensagem.Text = mensagem;
            lblMensagem.CssClass = "message-error";
            lblMensagem.Visible = true;
        }

        private string ObterMensagemMembership(
            MembershipCreateStatus status)
        {
            switch (status)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "Já existe uma conta com esse nome de utilizador.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "Já existe uma conta associada a esse email.";

                case MembershipCreateStatus.InvalidPassword:
                    return "A palavra-passe não cumpre os requisitos definidos.";

                case MembershipCreateStatus.InvalidEmail:
                    return "O endereço de email não é válido.";

                case MembershipCreateStatus.InvalidUserName:
                    return "O nome de utilizador não é válido.";

                case MembershipCreateStatus.UserRejected:
                    return "A criação do utilizador foi recusada.";

                default:
                    return "Não foi possível criar o utilizador.";
            }
        }
    }
}