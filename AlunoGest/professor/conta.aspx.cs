using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Security;

namespace AlunoGest.professor
{
    public partial class conta : System.Web.UI.Page
    {
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                MembershipUser user = Membership.GetUser();
                if (user == null) { FormsAuthentication.RedirectToLoginPage(); return; }
                TxtUsername.Text = user.UserName;
                PainelPrimeiroAcesso.Visible = !string.Equals(user.Comment, "CREDENCIAIS_ATUALIZADAS", StringComparison.Ordinal);
            }
        }

        protected void BtnGuardar_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;
            MembershipUser atual = Membership.GetUser();
            if (atual == null || !Membership.ValidateUser(atual.UserName, TxtPasswordAtual.Text))
            {
                MostrarMensagem("A palavra-passe atual está incorreta.", true);
                return;
            }

            string novoUsername = TxtUsername.Text.Trim().ToLowerInvariant();
            string novaPassword = TxtNovaPassword.Text;

            try
            {
                if (string.Equals(novoUsername, atual.UserName, StringComparison.OrdinalIgnoreCase))
                {
                    if (!atual.ChangePassword(TxtPasswordAtual.Text, novaPassword))
                        throw new InvalidOperationException("Não foi possível alterar a palavra-passe.");
                    atual.Comment = "CREDENCIAIS_ATUALIZADAS";
                    Membership.UpdateUser(atual);
                }
                else
                {
                    AlterarUsernameEPassword(atual, novoUsername, novaPassword);
                }

                Session.Clear();
                FormsAuthentication.SignOut();
                Response.Redirect("~/login.aspx?credenciais=alteradas");
            }
            catch (Exception ex)
            {
                MostrarMensagem("Não foi possível alterar as credenciais: " + ex.Message, true);
            }
        }

        private void AlterarUsernameEPassword(MembershipUser atual, string novoUsername, string novaPassword)
        {
            MembershipCreateStatus status;
            MembershipUser novo = Membership.CreateUser(novoUsername, novaPassword, atual.Email, null, null, true, null, out status);
            if (status != MembershipCreateStatus.Success || novo == null)
                throw new InvalidOperationException("O novo utilizador não está disponível (" + status + ").");

            try
            {
                Roles.AddUserToRole(novoUsername, "professor");
                novo.Comment = "CREDENCIAIS_ATUALIZADAS";
                Membership.UpdateUser(novo);

                const string sql = "UPDATE dbo.Professor SET UserId=@NovoUserId WHERE UserId=@UserId AND Id=@ProfessorId AND Ativo=1;";
                using (SqlConnection connection = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand(sql, connection))
                {
                    cmd.Parameters.Add("@NovoUserId", SqlDbType.UniqueIdentifier).Value = (Guid)novo.ProviderUserKey;
                    cmd.Parameters.Add("@UserId", SqlDbType.UniqueIdentifier).Value = (Guid)atual.ProviderUserKey;
                    cmd.Parameters.Add("@ProfessorId", SqlDbType.Int).Value = ProfessorId;
                    connection.Open();
                    if (cmd.ExecuteNonQuery() != 1)
                        throw new InvalidOperationException("O professor autenticado não foi encontrado.");
                }
            }
            catch
            {
                Membership.DeleteUser(novoUsername, true);
                throw;
            }

            try { Membership.DeleteUser(atual.UserName, true); }
            catch
            {
                atual.IsApproved = false;
                Membership.UpdateUser(atual);
            }
        }

        private int ProfessorId
        {
            get
            {
                int id;
                if (Session["ProfessorID"] != null && int.TryParse(Session["ProfessorID"].ToString(), out id)) return id;
                throw new InvalidOperationException("Sessão do professor inválida.");
            }
        }

        private void MostrarMensagem(string texto, bool erro)
        {
            LblMensagem.Text = texto;
            LblMensagem.CssClass = erro ? "alert alert-danger d-block" : "alert alert-success d-block";
            LblMensagem.Visible = true;
        }
    }
}
