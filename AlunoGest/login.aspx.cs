using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AlunoGest
{
    public partial class login : System.Web.UI.Page
    {
        private readonly string connectionString =
            ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void loginUtilizador_LoggedIn(object sender, EventArgs e)
        {
            MembershipUser user = Membership.GetUser(loginUtilizador.UserName);
            Session["UserId"] = user.ProviderUserKey.ToString();

            if (Roles.IsUserInRole(loginUtilizador.UserName, "agrupamento"))
            {
                //obter Id do agrupamento (int)
                Session["AgrupamentoID"] = GetIdAgrupamento(user.ProviderUserKey.ToString());
                Response.Redirect("~/agrupamento/dashboard.aspx");
            }
            if (Roles.IsUserInRole(loginUtilizador.UserName, "professor"))
                Response.Redirect("~/professor/dashboard.aspx");
            if (Roles.IsUserInRole(loginUtilizador.UserName, "aluno"))
                Response.Redirect("~/aluno/dashboard.aspx");
        }


        string GetIdAgrupamento(string userId)
        {
            const string query = @"
                    SELECT UserId
                    FROM dbo.Agrupamento
                    WHERE UserId = @Id;
                ";

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