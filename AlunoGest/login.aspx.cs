using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Security;

namespace AlunoGest
{
    public partial class login : System.Web.UI.Page
    {
        private readonly string connectionString =
            ConfigurationManager.ConnectionStrings["DefaultConnection"]
                .ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void loginUtilizador_LoggedIn(object sender, EventArgs e)
        {
            string username = loginUtilizador.UserName;

            MembershipUser user = Membership.GetUser(username);

            if (user == null)
            {
                FormsAuthentication.SignOut();
                Response.Redirect("~/login.aspx");
                return;
            }

            Session["UserId"] = user.ProviderUserKey.ToString();

            // Administrador da plataforma
            if (Roles.IsUserInRole(username, "administrador"))
            {
                Response.Redirect("~/administrador/criarconta.aspx");
                return;
            }

            // Agrupamento
            if (Roles.IsUserInRole(username, "agrupamento"))
            {
                Session["AgrupamentoID"] =
                    GetIdAgrupamento(user.ProviderUserKey.ToString());

                Response.Redirect("~/agrupamento/dashboard.aspx");
                return;
            }

            // Professor
            if (Roles.IsUserInRole(username, "professor"))
            {
                Response.Redirect("~/professor/Home.aspx");
                return;
            }

            // Aluno
            if (Roles.IsUserInRole(username, "aluno"))
            {
                Response.Redirect("~/aluno/dashboard.aspx");
                return;
            }

            // Utilizador autenticado, mas sem uma role reconhecida
            FormsAuthentication.SignOut();
            Session.Clear();

            Response.Redirect("~/login.aspx");
        }

        private string GetIdAgrupamento(string userId)
        {
            const string query = @"
                SELECT UserId
                FROM dbo.Agrupamento
                WHERE UserId = @Id;";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Id", userId);

                conn.Open();

                object result = cmd.ExecuteScalar();

                return result?.ToString();
            }
        }
    }
}