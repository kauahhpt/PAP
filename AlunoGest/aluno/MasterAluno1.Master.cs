using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Security;

namespace AlunoGest.aluno
{
    public partial class MasterAluno1 : System.Web.UI.MasterPage
    {
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.User.Identity.IsAuthenticated || !Roles.IsUserInRole(Page.User.Identity.Name, "aluno"))
            {
                Response.Redirect("~/login.aspx");
                return;
            }

            if (!IsPostBack)
                LblAluno.Text = ObterNomeAluno();
        }

        protected void LoginStatus1_LoggedOut(object sender, EventArgs e)
        {
            Session.Clear();
        }

        private string ObterNomeAluno()
        {
            if (Session["UserId"] == null) return Page.User.Identity.Name;
            using (SqlConnection c = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("SELECT NomeCompleto FROM dbo.Aluno WHERE UserId=@UserId AND Ativo=1", c))
            {
                cmd.Parameters.AddWithValue("@UserId", new Guid(Session["UserId"].ToString()));
                c.Open();
                object value = cmd.ExecuteScalar();
                return value == null ? Page.User.Identity.Name : value.ToString();
            }
        }
    }
}
