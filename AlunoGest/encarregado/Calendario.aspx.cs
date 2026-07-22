using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Security;
using System.Web.Script.Serialization;
using System.Web.UI;

namespace AlunoGest.encarregado
{
    public partial class Calendario : Page
    {
        private readonly string _connectionString =
            ConfigurationManager
                .ConnectionStrings["DefaultConnection"]
                .ConnectionString;


        #region Página

        protected void Page_Load(
            object sender,
            EventArgs e)
        {
            if (!Request.IsAuthenticated ||
                !Roles.IsUserInRole(
                    User.Identity.Name,
                    "encarregado"))
            {
                TerminarSessao();
                return;
            }

            if (!IsPostBack)
            {
                CarregarPagina();
            }
        }

        #endregion


        #region Carregamento principal

        private void CarregarPagina()
        {
            LimparMensagem();

            int encarregadoId;

            if (!TryGetEncarregadoId(
                    out encarregadoId))
            {
                TerminarSessao();
                return;
            }

            int educandoId;

            if (!TryGetEducandoId(
                    out educandoId))
            {
                MostrarSemEducando();
                return;
            }

            try
            {
                string nomeEducando =
                    ObterNomeEducandoAssociado(
                        educandoId,
                        encarregadoId
                    );

                if (string.IsNullOrWhiteSpace(
                        nomeEducando))
                {
                    Session["EducandoId"] = null;

                    MostrarSemEducando();

                    MostrarMensagem(
                        "O educando selecionado não está associado " +
                        "à sua conta ou já não se encontra ativo."
                    );

                    return;
                }

                LblEducandoSelecionado.Text =
                    nomeEducando;

                LblEducandoSelecionado.Visible =
                    true;

                CarregarEventos(
                    educandoId
                );

                PnlSemEducando.Visible =
                    false;

                PnlConteudo.Visible =
                    true;
            }
            catch (SqlException ex)
            {
                System.Diagnostics.Trace.TraceError(
                    "Erro SQL ao carregar calendário: " +
                    ex
                );

                MostrarSemEducando();

                MostrarMensagem(
                    "Não foi possível carregar os eventos do educando."
                );
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError(
                    "Erro ao carregar calendário: " +
                    ex
                );

                MostrarSemEducando();

                MostrarMensagem(
                    "Ocorreu um erro ao carregar o calendário: " +
                    ex.Message
                );
            }
        }

        #endregion


        #region Educando associado

        private string ObterNomeEducandoAssociado(
            int educandoId,
            int encarregadoId)
        {
            const string sql = @"
                SELECT TOP 1
                    a.NomeCompleto

                FROM dbo.Aluno a

                INNER JOIN dbo.AlunoEncarregado ae
                    ON ae.AlunoId = a.Id

                INNER JOIN dbo.EncarregadoEducacao ee
                    ON ee.Id = ae.EncarregadoEducacaoId

                WHERE a.Id = @AlunoId

                  AND ae.EncarregadoEducacaoId =
                      @EncarregadoEducacaoId

                  AND a.Ativo = 1
                  AND ae.Ativo = 1
                  AND ee.Ativo = 1;";

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

                object resultado =
                    cmd.ExecuteScalar();

                if (resultado == null ||
                    resultado == DBNull.Value)
                {
                    return null;
                }

                return resultado
                    .ToString()
                    .Trim();
            }
        }

        #endregion


        #region Eventos

        private void CarregarEventos(
            int educandoId)
        {
            /*
             * São apresentados:
             *
             * 1. Eventos pessoais, cujo AlunoId corresponde
             *    ao educando selecionado.
             *
             * 2. Eventos gerais das turmas atuais do educando,
             *    onde o Evento.AlunoId está vazio.
             */
            const string sql = @"
                SELECT
                    ev.Id,
                    ev.Titulo,
                    ev.Tipo,
                    ev.DataHora,

                    CASE
                        WHEN ev.AlunoId = @AlunoId
                            THEN N'Pessoal'

                        ELSE N'Turma'
                    END AS Origem

                FROM dbo.Evento ev

                WHERE
                    ev.AlunoId = @AlunoId

                    OR
                    (
                        ev.AlunoId IS NULL

                        AND EXISTS
                        (
                            SELECT 1

                            FROM dbo.AlunoTurma at2

                            WHERE at2.AlunoId = @AlunoId
                              AND at2.TurmaId = ev.TurmaId
                              AND at2.Ate IS NULL
                        )
                    )

                ORDER BY
                    ev.DataHora,
                    ev.Id;";

            List<object> eventosJson =
                new List<object>();

            int totalProximos = 0;
            int totalTestes = 0;
            int totalTrabalhos = 0;

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

                conn.Open();

                using (SqlDataReader reader =
                    cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int eventoId =
                            Convert.ToInt32(
                                reader["Id"]
                            );

                        string titulo =
                            ValorTexto(
                                reader["Titulo"]
                            );

                        string tipo =
                            ValorTexto(
                                reader["Tipo"]
                            );

                        string origem =
                            ValorTexto(
                                reader["Origem"]
                            );

                        DateTime dataHora =
                            Convert.ToDateTime(
                                reader["DataHora"]
                            );

                        string cor =
                            ObterCorEvento(
                                tipo
                            );

                        string descricao =
                            CriarDescricaoEvento(
                                origem
                            );

                        eventosJson.Add(
                            new
                            {
                                id = eventoId,

                                title =
                                    string.IsNullOrWhiteSpace(
                                        titulo)
                                        ? "Evento sem título"
                                        : titulo,

                                start =
                                    dataHora.ToString(
                                        "yyyy-MM-ddTHH:mm:ss"
                                    ),

                                allDay = false,

                                color = cor,

                                tipo =
                                    string.IsNullOrWhiteSpace(
                                        tipo)
                                        ? "Outro"
                                        : tipo,

                                origem = origem,

                                descricao = descricao
                            }
                        );

                        if (dataHora >= DateTime.Today)
                        {
                            totalProximos++;

                            if (string.Equals(
                                    tipo,
                                    "Teste",
                                    StringComparison
                                        .OrdinalIgnoreCase))
                            {
                                totalTestes++;
                            }
                            else if (string.Equals(
                                    tipo,
                                    "Trabalho",
                                    StringComparison
                                        .OrdinalIgnoreCase))
                            {
                                totalTrabalhos++;
                            }
                        }
                    }
                }
            }

            JavaScriptSerializer serializer =
                new JavaScriptSerializer();

            HfEventosJson.Value =
                serializer.Serialize(
                    eventosJson
                );

            LblTotalProximos.Text =
                totalProximos.ToString();

            LblTotalTestes.Text =
                totalTestes.ToString();

            LblTotalTrabalhos.Text =
                totalTrabalhos.ToString();
        }


        private string ObterCorEvento(
            string tipo)
        {
            if (string.Equals(
                    tipo,
                    "Teste",
                    StringComparison.OrdinalIgnoreCase))
            {
                return "#dc2626";
            }

            if (string.Equals(
                    tipo,
                    "Trabalho",
                    StringComparison.OrdinalIgnoreCase))
            {
                return "#2563eb";
            }

            if (string.Equals(
                    tipo,
                    "Evento",
                    StringComparison.OrdinalIgnoreCase))
            {
                return "#16a34a";
            }

            return "#7c3aed";
        }


        private string CriarDescricaoEvento(
            string origem)
        {
            if (string.Equals(
                    origem,
                    "Pessoal",
                    StringComparison.OrdinalIgnoreCase))
            {
                return
                    "Evento atribuído especificamente " +
                    "ao educando selecionado.";
            }

            return
                "Evento geral atribuído à turma atual " +
                "do educando selecionado.";
        }

        #endregion


        #region Sessão

        private bool TryGetEducandoId(
            out int educandoId)
        {
            educandoId = 0;

            if (Session["EducandoId"] == null)
            {
                return false;
            }

            return int.TryParse(
                Session["EducandoId"].ToString(),
                out educandoId
            );
        }


        private bool TryGetEncarregadoId(
            out int encarregadoId)
        {
            encarregadoId = 0;

            if (Session[
                    "EncarregadoEducacaoId"
                ] != null &&
                int.TryParse(
                    Session[
                        "EncarregadoEducacaoId"
                    ].ToString(),
                    out encarregadoId))
            {
                return true;
            }

            MembershipUser utilizador =
                Membership.GetUser(
                    User.Identity.Name,
                    false
                );

            if (utilizador == null ||
                utilizador.ProviderUserKey == null)
            {
                return false;
            }

            Guid userId;

            if (!Guid.TryParse(
                    utilizador
                        .ProviderUserKey
                        .ToString(),
                    out userId))
            {
                return false;
            }

            const string sql = @"
                SELECT TOP 1
                    Id

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

                object resultado =
                    cmd.ExecuteScalar();

                if (resultado == null ||
                    resultado == DBNull.Value)
                {
                    return false;
                }

                encarregadoId =
                    Convert.ToInt32(
                        resultado
                    );

                Session[
                    "EncarregadoEducacaoId"
                ] = encarregadoId;

                return true;
            }
        }

        #endregion


        #region Interface

        private void MostrarSemEducando()
        {
            PnlConteudo.Visible =
                false;

            PnlSemEducando.Visible =
                true;

            LblEducandoSelecionado.Visible =
                false;

            HfEventosJson.Value =
                "[]";

            LblTotalProximos.Text =
                "0";

            LblTotalTestes.Text =
                "0";

            LblTotalTrabalhos.Text =
                "0";
        }


        private void MostrarMensagem(
            string mensagem,
            bool erro = true)
        {
            LblMensagem.Visible =
                true;

            LblMensagem.Text =
                mensagem;

            LblMensagem.CssClass =
                erro
                    ? "alert alert-warning d-block"
                    : "alert alert-success d-block";
        }


        private void LimparMensagem()
        {
            LblMensagem.Visible =
                false;

            LblMensagem.Text =
                string.Empty;

            LblMensagem.CssClass =
                string.Empty;
        }


        private string ValorTexto(
            object valor)
        {
            if (valor == null ||
                valor == DBNull.Value)
            {
                return string.Empty;
            }

            return valor
                .ToString()
                .Trim();
        }

        #endregion


        #region Terminar sessão

        private void TerminarSessao()
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
        }

        #endregion
    }
}