using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Security;
using AlunoGest.Services;

namespace AlunoGest.aluno
{
    public partial class MasterAluno1 :
        System.Web.UI.MasterPage
    {
        private readonly string _connectionString =
            ConfigurationManager
                .ConnectionStrings["DefaultConnection"]
                .ConnectionString;


        protected void Page_Load(
            object sender,
            EventArgs e)
        {
            if (!Page.User.Identity.IsAuthenticated)
            {
                Response.Redirect(
                    "~/login.aspx",
                    false
                );

                Context.ApplicationInstance
                    .CompleteRequest();

                return;
            }

            if (!Roles.IsUserInRole(
                    Page.User.Identity.Name,
                    "aluno"))
            {
                FormsAuthentication.SignOut();

                Session.Clear();
                Session.Abandon();

                Response.Redirect(
                    "~/login.aspx",
                    false
                );

                Context.ApplicationInstance
                    .CompleteRequest();

                return;
            }

            Guid userId;

            if (!ObterUserIdAtual(
                    out userId))
            {
                FormsAuthentication.SignOut();

                Session.Clear();
                Session.Abandon();

                Response.Redirect(
                    "~/login.aspx",
                    false
                );

                Context.ApplicationInstance
                    .CompleteRequest();

                return;
            }

            if (!IsPostBack)
            {
                LblAluno.Text =
                    ObterNomeAluno(
                        userId
                    );
            }
        }


        protected override void OnPreRender(
            EventArgs e)
        {
            base.OnPreRender(e);

            CarregarContadorMensagensNaoLidas();
        }


        private void CarregarContadorMensagensNaoLidas()
        {
            try
            {
                if (!Page.User.Identity.IsAuthenticated)
                {
                    LblMensagensNaoLidas.Visible =
                        false;

                    return;
                }

                Guid userId;

                if (!ObterUserIdAtual(
                        out userId))
                {
                    LblMensagensNaoLidas.Visible =
                        false;

                    return;
                }

                int quantidade =
                    MensagemDiretaService
                        .ContarMensagensNaoLidasAluno(
                            userId
                        );

                LblMensagensNaoLidas.Text =
                    MensagemDiretaService
                        .FormatarQuantidade(
                            quantidade
                        );

                LblMensagensNaoLidas.Visible =
                    quantidade > 0;

                LnkMensagens.ToolTip =
                    MensagemDiretaService
                        .FormatarToolTip(
                            quantidade
                        );
            }
            catch
            {
                LblMensagensNaoLidas.Visible =
                    false;

                LnkMensagens.ToolTip =
                    "Chat";
            }
        }


        private bool ObterUserIdAtual(
            out Guid userId)
        {
            userId = Guid.Empty;

            if (Session["UserId"] != null &&
                Guid.TryParse(
                    Session["UserId"].ToString(),
                    out userId))
            {
                return true;
            }

            MembershipUser utilizador =
                Membership.GetUser(
                    Page.User.Identity.Name,
                    false
                );

            if (utilizador == null ||
                utilizador.ProviderUserKey == null)
            {
                return false;
            }

            if (!Guid.TryParse(
                    utilizador.ProviderUserKey.ToString(),
                    out userId))
            {
                return false;
            }

            Session["UserId"] =
                userId;

            return true;
        }


        private string ObterNomeAluno(
            Guid userId)
        {
            try
            {
                const string sql = @"
                    SELECT TOP 1
                        NomeCompleto

                    FROM dbo.Aluno

                    WHERE UserId = @UserId
                      AND Ativo = 1;";

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

                    object value =
                        cmd.ExecuteScalar();

                    if (value == null ||
                        value == DBNull.Value)
                    {
                        return Page.User.Identity.Name;
                    }

                    return value
                        .ToString()
                        .Trim();
                }
            }
            catch
            {
                return Page.User.Identity.Name;
            }
        }


        protected void LoginStatus1_LoggedOut(
            object sender,
            EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
        }
    }
}