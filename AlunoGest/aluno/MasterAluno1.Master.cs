using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Security;

namespace AlunoGest.aluno
{
    public partial class MasterAluno1 : System.Web.UI.MasterPage
    {
        private readonly string _connectionString =
            ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.User.Identity.IsAuthenticated)
            {
                Response.Redirect("~/login.aspx");
                return;
            }

            if (!Roles.IsUserInRole(Page.User.Identity.Name, "aluno"))
            {
                FormsAuthentication.SignOut();
                Session.Clear();
                Response.Redirect("~/login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LblAluno.Text = ObterNomeAluno();
            }
        }

        protected void LoginStatus1_LoggedOut(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
        }

        private string ObterNomeAluno()
        {
            try
            {
                if (Session["UserId"] == null)
                    return Page.User.Identity.Name;

                Guid userId = new Guid(Session["UserId"].ToString());

                string sql = @"
                    SELECT NomeCompleto
                    FROM dbo.Aluno
                    WHERE UserId = @UserId
                      AND Ativo = 1;";

                using (SqlConnection conn = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);

                    conn.Open();

                    object value = cmd.ExecuteScalar();

                    if (value == null || value == DBNull.Value)
                        return Page.User.Identity.Name;

                    return value.ToString();
                }
            }
            catch
            {
                return Page.User.Identity.Name;
            }
        }
    }
}