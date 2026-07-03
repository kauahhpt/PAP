using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Security;

namespace AlunoGest.professor
{
    public partial class ModeloProfessor : System.Web.UI.MasterPage
    {
        private readonly string _connectionString =
            ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            MembershipUser user = Membership.GetUser();
            bool paginaConta = Request.AppRelativeCurrentExecutionFilePath.EndsWith("/conta.aspx", StringComparison.OrdinalIgnoreCase);
            if (user != null && !string.Equals(user.Comment, "CREDENCIAIS_ATUALIZADAS", StringComparison.Ordinal) && !paginaConta)
            {
                Response.Redirect("~/professor/conta.aspx?primeiro=1");
                return;
            }

            if (!IsPostBack)
                LblProfessor.Text = ObterNomeProfessor();
        }

        private string ObterNomeProfessor()
        {
            if (Session["UserId"] == null)
                return "Professor";

            const string sql = "SELECT Nome FROM dbo.Professor WHERE UserId = @UserId AND Ativo = 1;";
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@UserId", new Guid(Session["UserId"].ToString()));
                conn.Open();
                object value = cmd.ExecuteScalar();
                return value == null || value == DBNull.Value ? "Professor" : value.ToString();
            }
        }

        protected void LoginStatus1_LoggedOut(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
        }
    }
}
