using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Security;
using System.Web.UI.WebControls;
using AlunoGest.Services;

namespace AlunoGest.encarregado
{
    public partial class modeloEncarregado :
        System.Web.UI.MasterPage
    {
        private readonly string _connectionString =
            ConfigurationManager
                .ConnectionStrings["DefaultConnection"]
                .ConnectionString;
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DdlEducando.SelectedIndexChanged +=
                DdlEducando_SelectedIndexChanged;

            if (!ValidarAcesso())
            {
                return;
            }

            int encarregadoId;
            string nomeEncarregado;

            if (!ObterEncarregadoAtual(
                    out encarregadoId,
                    out nomeEncarregado))
            {
                TerminarSessao();
                return;
            }

            Session["EncarregadoEducacaoId"] =
                encarregadoId;

            LblEncarregado.Text =
                nomeEncarregado;

            if (!Page.IsPostBack)
            {
                CarregarEducandos(
                    encarregadoId
                );
            }
        }


        protected override void OnPreRender(
            EventArgs e)
        {
            base.OnPreRender(e);

            CarregarContadorMensagensNaoLidas();
        }
        private bool ValidarAcesso()
        {
            if (!Page.User.Identity.IsAuthenticated)
            {
                Response.Redirect("~/login.aspx");
                return false;
            }

            if (!Roles.IsUserInRole(
                    Page.User.Identity.Name,
                    "encarregado"))
            {
                TerminarSessao();
                return false;
            }

            return true;
        }

        private bool ObterEncarregadoAtual(
            out int encarregadoId,
            out string nomeEncarregado)
        {
            encarregadoId = 0;
            nomeEncarregado =
                Page.User.Identity.Name;

            Guid userId;

            if (!ObterUserIdAtual(out userId))
            {
                return false;
            }

            const string sql = @"
                SELECT TOP 1
                    Id,
                    NomeCompleto

                FROM dbo.EncarregadoEducacao

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

                using (SqlDataReader reader =
                    cmd.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        return false;
                    }

                    encarregadoId =
                        Convert.ToInt32(
                            reader["Id"]
                        );

                    nomeEncarregado =
                        Convert.ToString(
                            reader["NomeCompleto"]
                        );

                    return true;
                }
            }
        }

        private bool ObterUserIdAtual(
            out Guid userId)
        {
            userId = Guid.Empty;

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

            Session["UserId"] = userId;

            return true;
        }

        private void CarregarEducandos(
            int encarregadoId)
        {
            const string sql = @"
                SELECT
                    a.Id,

                    a.NomeCompleto +
                    CASE

                        WHEN a.NumeroProcesso IS NULL
                             OR LTRIM(RTRIM(
                                 a.NumeroProcesso
                             )) = ''

                        THEN ''

                        ELSE
                            N' — Processo ' +
                            a.NumeroProcesso

                    END AS Descricao

                FROM dbo.AlunoEncarregado ae

                INNER JOIN dbo.Aluno a
                    ON a.Id = ae.AlunoId

                WHERE ae.EncarregadoEducacaoId =
                      @EncarregadoEducacaoId

                  AND ae.Ativo = 1
                  AND a.Ativo = 1

                ORDER BY
                    ae.Principal DESC,
                    a.NomeCompleto;";

            DataTable tabela =
                new DataTable();

            using (SqlConnection conn =
                new SqlConnection(_connectionString))
            using (SqlCommand cmd =
                new SqlCommand(sql, conn))
            using (SqlDataAdapter adapter =
                new SqlDataAdapter(cmd))
            {
                cmd.Parameters
                    .Add(
                        "@EncarregadoEducacaoId",
                        SqlDbType.Int
                    )
                    .Value = encarregadoId;

                adapter.Fill(tabela);
            }

            DdlEducando.Items.Clear();

            if (tabela.Rows.Count == 0)
            {
                DdlEducando.Items.Add(
                    new ListItem(
                        "Nenhum educando associado",
                        ""
                    )
                );

                DdlEducando.Enabled = false;

                Session["EducandoId"] = null;

                return;
            }

            DdlEducando.Enabled = true;

            DdlEducando.DataSource =
                tabela;

            DdlEducando.DataTextField =
                "Descricao";

            DdlEducando.DataValueField =
                "Id";

            DdlEducando.DataBind();

            SelecionarEducandoAtual();
        }

        private void SelecionarEducandoAtual()
        {
            int educandoId;

            if (Session["EducandoId"] != null &&
                int.TryParse(
                    Session["EducandoId"].ToString(),
                    out educandoId))
            {
                ListItem item =
                    DdlEducando.Items.FindByValue(
                        educandoId.ToString()
                    );

                if (item != null)
                {
                    DdlEducando.ClearSelection();
                    item.Selected = true;

                    return;
                }
            }

            if (DdlEducando.Items.Count > 0)
            {
                DdlEducando.SelectedIndex = 0;

                Session["EducandoId"] =
                    Convert.ToInt32(
                        DdlEducando.SelectedValue
                    );
            }
        }

        protected void DdlEducando_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            int educandoId;
            int encarregadoId;

            if (!int.TryParse(
                    DdlEducando.SelectedValue,
                    out educandoId))
            {
                Session["EducandoId"] = null;
                return;
            }

            if (Session["EncarregadoEducacaoId"] == null ||
                !int.TryParse(
                    Session[
                        "EncarregadoEducacaoId"
                    ].ToString(),
                    out encarregadoId))
            {
                TerminarSessao();
                return;
            }

            if (!EducandoPertenceAoEncarregado(
                    educandoId,
                    encarregadoId))
            {
                Session["EducandoId"] = null;

                CarregarEducandos(
                    encarregadoId
                );

                return;
            }

            Session["EducandoId"] =
                educandoId;

            /*
             * Faz uma nova abertura da página para que
             * o dashboard carregue os dados do novo educando.
             */

            Response.Redirect(
                Request.RawUrl,
                false
            );

            Context.ApplicationInstance
                .CompleteRequest();
        }

        private bool EducandoPertenceAoEncarregado(
            int educandoId,
            int encarregadoId)
        {
            const string sql = @"
                SELECT
                    CASE

                        WHEN EXISTS
                        (
                            SELECT 1

                            FROM dbo.AlunoEncarregado ae

                            INNER JOIN dbo.Aluno a
                                ON a.Id = ae.AlunoId

                            WHERE ae.AlunoId = @AlunoId

                              AND
                              ae.EncarregadoEducacaoId =
                                  @EncarregadoEducacaoId

                              AND ae.Ativo = 1
                              AND a.Ativo = 1
                        )

                        THEN 1
                        ELSE 0

                    END;";

            using (SqlConnection conn =
                new SqlConnection(_connectionString))
            using (SqlCommand cmd =
                new SqlCommand(sql, conn))
            {
                cmd.Parameters
                    .Add(
                        "@AlunoId",
                        SqlDbType.Int
                    )
                    .Value = educandoId;

                cmd.Parameters
                    .Add(
                        "@EncarregadoEducacaoId",
                        SqlDbType.Int
                    )
                    .Value = encarregadoId;

                conn.Open();

                return Convert.ToInt32(
                    cmd.ExecuteScalar()
                ) == 1;
            }
        }
        private void CarregarContadorMensagensNaoLidas()
        {
            try
            {
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
                        .ContarMensagensNaoLidas(
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
                    "Mensagens";
            }
        }
        protected void LoginStatus1_LoggedOut(
            object sender,
            EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
        }

        private void TerminarSessao()
        {
            FormsAuthentication.SignOut();

            Session.Clear();
            Session.Abandon();

            Response.Redirect("~/login.aspx");
        }

        public int? EducandoSelecionadoId
        {
            get
            {
                int educandoId;

                if (Session["EducandoId"] != null &&
                    int.TryParse(
                        Session["EducandoId"].ToString(),
                        out educandoId))
                {
                    return educandoId;
                }

                return null;
            }
        }
    }
}