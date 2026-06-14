using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AlunoGest
{
    public partial class criarconta : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //Roles.CreateRole("aluno");
            //Roles.CreateRole("professor");
            //Roles.CreateRole("agrupamento");
        }

        protected void btnCriarConta_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            string connectionString =
                ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            // 1) Criar utilizador

            Membership.CreateUser(txtUsername.Text, txtPassword.Text, txtEmail.Text);

            MembershipUser user = Membership.GetUser(txtUsername.Text);

            // 2) Role
            Roles.AddUserToRole(user.UserName, "agrupamento"); // ou "agrupamento"

            // 3) Inserir Agrupamento
            string sql = @"
                INSERT INTO Agrupamento
                    (UserId, Nome, Morada, CodigoPostal, Localidade, Telefone, Email, CodigoMEC)
                VALUES
                    (@UserId, @Nome, @Morada, @CodigoPostal, @Localidade, @Telefone, @Email, @CodigoMEC);";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                // Id é varchar(50) - guardar como string
                cmd.Parameters.Add("@UserId", SqlDbType.VarChar, 50).Value = user.ProviderUserKey.ToString();

                cmd.Parameters.Add("@Nome", SqlDbType.NVarChar, 150).Value = txtNome.Text.Trim();
                cmd.Parameters.Add("@Morada", SqlDbType.NVarChar, 500).Value = txtMorada.Text.Trim();
                cmd.Parameters.Add("@CodigoPostal", SqlDbType.VarChar, 8).Value = txtCodigoPostal.Text.Trim();
                cmd.Parameters.Add("@Localidade", SqlDbType.NVarChar, 100).Value = txtLocalidade.Text.Trim();

                cmd.Parameters.Add("@Telefone", SqlDbType.VarChar, 20).Value =
                    string.IsNullOrWhiteSpace(txtTelefone.Text) ? (object)DBNull.Value : txtTelefone.Text.Trim();

                cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 120).Value =
                    string.IsNullOrWhiteSpace(txtEmail.Text) ? (object)DBNull.Value : txtEmail.Text.Trim();

                cmd.Parameters.Add("@CodigoMEC", SqlDbType.VarChar, 50).Value =
                    string.IsNullOrWhiteSpace(txtCodigoMEC.Text) ? (object)DBNull.Value : txtCodigoMEC.Text.Trim();

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            Response.Redirect("~/login.aspx");
        }

    }
}