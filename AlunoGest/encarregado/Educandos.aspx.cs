using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.Security;
using System.Web.UI;

namespace AlunoGest.encarregado
{
    public partial class Educandos : Page
    {
        #region Campos

        private readonly string _connectionString =
            ConfigurationManager
                .ConnectionStrings["DefaultConnection"]
                .ConnectionString;

        #endregion


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
                EducandoDados educando =
                    ObterEducando(
                        educandoId,
                        encarregadoId
                    );

                if (educando == null)
                {
                    Session["EducandoId"] = null;

                    MostrarSemEducando();

                    MostrarMensagem(
                        "O educando selecionado não está associado " +
                        "à sua conta ou já não se encontra disponível."
                    );

                    return;
                }

                PreencherInterface(educando);

                PnlSemEducando.Visible = false;
                PnlConteudo.Visible = true;

                LblEducandoSelecionado.Visible = true;
                LblEducandoSelecionado.Text =
                    Codificar(
                        educando.NomeCompleto
                    );
            }
            catch (SqlException ex)
            {
                System.Diagnostics.Trace.TraceError(
                    "Erro SQL ao carregar educando: " +
                    ex
                );

                MostrarSemEducando();

                MostrarMensagem(
                    "Não foi possível carregar os dados do educando."
                );
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError(
                    "Erro ao carregar educando: " +
                    ex
                );

                MostrarSemEducando();

                MostrarMensagem(
                    "Ocorreu um erro ao carregar as informações."
                );
            }
        }

        #endregion


        #region Consulta do educando

        private EducandoDados ObterEducando(
            int educandoId,
            int encarregadoId)
        {
            const string sql = @"
                SELECT TOP 1

                    a.Id,
                    a.NomeCompleto,
                    a.NumeroProcesso,
                    a.NIF,
                    a.Email,
                    a.Telefone,
                    a.Foto,
                    a.Ativo,

                    ae.Parentesco,
                    ae.Principal,
                    ae.Ativo AS AssociacaoAtiva,
                    ae.CreatedAt AS DataAssociacao,

                    ag.Nome AS Agrupamento,

                    t.Id AS TurmaId,

                    CASE
                        WHEN t.Id IS NULL
                        THEN NULL

                        ELSE
                            CAST(
                                t.AnoEscolaridade
                                AS NVARCHAR(3)
                            )
                            + N'.º '
                            + t.CodigoTurma
                    END AS Turma,

                    esc.Nome AS Escola,
                    al.Descricao AS AnoLetivo

                FROM dbo.Aluno a

                INNER JOIN dbo.AlunoEncarregado ae
                    ON ae.AlunoId = a.Id

                INNER JOIN dbo.EncarregadoEducacao enc
                    ON enc.Id =
                       ae.EncarregadoEducacaoId

                INNER JOIN dbo.Agrupamento ag
                    ON ag.Id = a.AgrupamentoId

                OUTER APPLY
                (
                    SELECT TOP 1
                        at2.TurmaId

                    FROM dbo.AlunoTurma at2

                    WHERE at2.AlunoId = a.Id
                      AND at2.Ate IS NULL

                    ORDER BY
                        at2.Desde DESC,
                        at2.Id DESC
                ) turmaAtual

                LEFT JOIN dbo.Turma t
                    ON t.Id = turmaAtual.TurmaId

                LEFT JOIN dbo.Escola esc
                    ON esc.Id = t.EscolaId

                LEFT JOIN dbo.AnoLetivo al
                    ON al.Id = t.AnoLetivoId

                WHERE a.Id = @AlunoId

                  AND ae.EncarregadoEducacaoId =
                      @EncarregadoEducacaoId

                  AND ae.Ativo = 1
                  AND enc.Ativo = 1;";

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

                using (SqlDataReader reader =
                    cmd.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        return null;
                    }

                    EducandoDados educando =
                        new EducandoDados();

                    educando.Id =
                        Convert.ToInt32(
                            reader["Id"]
                        );

                    educando.NomeCompleto =
                        ValorTexto(
                            reader["NomeCompleto"]
                        );

                    educando.NumeroProcesso =
                        ValorTexto(
                            reader["NumeroProcesso"]
                        );

                    educando.NIF =
                        ValorTexto(
                            reader["NIF"]
                        );

                    educando.Email =
                        ValorTexto(
                            reader["Email"]
                        );

                    educando.Telefone =
                        ValorTexto(
                            reader["Telefone"]
                        );

                    educando.Foto =
                        ValorTexto(
                            reader["Foto"]
                        );

                    educando.Ativo =
                        reader["Ativo"] != DBNull.Value &&
                        Convert.ToBoolean(
                            reader["Ativo"]
                        );

                    educando.Parentesco =
                        ValorTexto(
                            reader["Parentesco"]
                        );

                    educando.Principal =
                        reader["Principal"] != DBNull.Value &&
                        Convert.ToBoolean(
                            reader["Principal"]
                        );

                    educando.AssociacaoAtiva =
                        reader["AssociacaoAtiva"] != DBNull.Value &&
                        Convert.ToBoolean(
                            reader["AssociacaoAtiva"]
                        );

                    educando.Agrupamento =
                        ValorTexto(
                            reader["Agrupamento"]
                        );

                    educando.Turma =
                        ValorTexto(
                            reader["Turma"]
                        );

                    educando.Escola =
                        ValorTexto(
                            reader["Escola"]
                        );

                    educando.AnoLetivo =
                        ValorTexto(
                            reader["AnoLetivo"]
                        );

                    if (reader["TurmaId"] != DBNull.Value)
                    {
                        educando.TurmaId =
                            Convert.ToInt32(
                                reader["TurmaId"]
                            );
                    }

                    if (reader["DataAssociacao"] != DBNull.Value)
                    {
                        educando.DataAssociacao =
                            Convert.ToDateTime(
                                reader["DataAssociacao"]
                            );
                    }

                    return educando;
                }
            }
        }

        #endregion


        #region Preenchimento da interface

        private void PreencherInterface(
            EducandoDados educando)
        {
            LblNomeEducando.Text =
                Codificar(
                    TextoOuTraco(
                        educando.NomeCompleto
                    )
                );

            LblNumeroProcesso.Text =
                Codificar(
                    TextoOuTraco(
                        educando.NumeroProcesso
                    )
                );

            LblNIF.Text =
                Codificar(
                    TextoOuTraco(
                        educando.NIF
                    )
                );

            LblEmail.Text =
                Codificar(
                    TextoOuTraco(
                        educando.Email
                    )
                );

            LblTelefone.Text =
                Codificar(
                    TextoOuTraco(
                        educando.Telefone
                    )
                );

            LblTurma.Text =
                Codificar(
                    TextoOuTraco(
                        educando.Turma
                    )
                );

            LblAnoLetivo.Text =
                Codificar(
                    TextoOuTraco(
                        educando.AnoLetivo
                    )
                );

            LblEscola.Text =
                Codificar(
                    TextoOuTraco(
                        educando.Escola
                    )
                );

            LblAgrupamento.Text =
                Codificar(
                    TextoOuTraco(
                        educando.Agrupamento
                    )
                );

            LblParentesco.Text =
                Codificar(
                    string.IsNullOrWhiteSpace(
                        educando.Parentesco)
                        ? "Não definido"
                        : educando.Parentesco
                );

            LblPrincipal.Text =
                educando.Principal
                    ? "Encarregado principal"
                    : "Encarregado associado";

            LblAssociacaoAtiva.Text =
                educando.AssociacaoAtiva
                    ? "Sim"
                    : "Não";

            LblDataAssociacao.Text =
                educando.DataAssociacao.HasValue
                    ? educando.DataAssociacao.Value
                        .ToString("dd/MM/yyyy")
                    : "Não definido";

            ConfigurarEstadoAluno(
                educando.Ativo
            );

            ConfigurarTipoEncarregado(
                educando.Principal
            );

            LblDescricaoEducando.Text =
                Codificar(
                    CriarDescricaoEducando(
                        educando
                    )
                );

            CarregarFotografia(
                educando.Foto,
                educando.NomeCompleto
            );
        }

        private void ConfigurarEstadoAluno(
            bool ativo)
        {
            LblEstadoAluno.Text =
                ativo
                    ? "Aluno ativo"
                    : "Aluno inativo";

            LblEstadoAluno.CssClass =
                ativo
                    ? "etiqueta etiqueta-ativo"
                    : "etiqueta etiqueta-inativo";
        }

        private void ConfigurarTipoEncarregado(
            bool principal)
        {
            LblTipoEncarregado.Text =
                principal
                    ? "Encarregado principal"
                    : "Encarregado associado";

            LblTipoEncarregado.CssClass =
                principal
                    ? "etiqueta etiqueta-principal"
                    : "etiqueta etiqueta-secundario";
        }

        private string CriarDescricaoEducando(
            EducandoDados educando)
        {
            bool temTurma =
                !string.IsNullOrWhiteSpace(
                    educando.Turma
                );

            bool temEscola =
                !string.IsNullOrWhiteSpace(
                    educando.Escola
                );

            if (temTurma && temEscola)
            {
                return
                    educando.Turma +
                    " — " +
                    educando.Escola;
            }

            if (temTurma)
            {
                return educando.Turma;
            }

            if (temEscola)
            {
                return educando.Escola;
            }

            return "Aluno sem turma atual";
        }

        #endregion


        #region Fotografia

        private void CarregarFotografia(
            string caminhoFoto,
            string nomeCompleto)
        {
            if (string.IsNullOrWhiteSpace(
                    caminhoFoto))
            {
                MostrarIniciais(
                    nomeCompleto
                );

                return;
            }

            string urlFoto =
                NormalizarCaminhoFoto(
                    caminhoFoto
                );

            if (string.IsNullOrWhiteSpace(
                    urlFoto))
            {
                MostrarIniciais(
                    nomeCompleto
                );

                return;
            }

            ImgEducando.ImageUrl =
                urlFoto;

            ImgEducando.Visible =
                true;

            PnlFotoPlaceholder.Visible =
                false;
        }

        private string NormalizarCaminhoFoto(
            string caminhoFoto)
        {
            if (string.IsNullOrWhiteSpace(
                    caminhoFoto))
            {
                return null;
            }

            caminhoFoto =
                caminhoFoto.Trim();

            Uri urlAbsoluta;

            if (Uri.TryCreate(
                    caminhoFoto,
                    UriKind.Absolute,
                    out urlAbsoluta))
            {
                if (urlAbsoluta.Scheme ==
                        Uri.UriSchemeHttp ||
                    urlAbsoluta.Scheme ==
                        Uri.UriSchemeHttps)
                {
                    return caminhoFoto;
                }

                return null;
            }

            if (caminhoFoto.StartsWith(
                    "~/",
                    StringComparison.Ordinal))
            {
                return ResolveUrl(
                    caminhoFoto
                );
            }

            if (caminhoFoto.StartsWith(
                    "/",
                    StringComparison.Ordinal))
            {
                return ResolveUrl(
                    "~" + caminhoFoto
                );
            }

            return ResolveUrl(
                "~/" +
                caminhoFoto.TrimStart('/')
            );
        }

        private void MostrarIniciais(
            string nomeCompleto)
        {
            ImgEducando.Visible =
                false;

            PnlFotoPlaceholder.Visible =
                true;

            LblIniciais.Text =
                Codificar(
                    ObterIniciais(
                        nomeCompleto
                    )
                );
        }

        private string ObterIniciais(
            string nomeCompleto)
        {
            if (string.IsNullOrWhiteSpace(
                    nomeCompleto))
            {
                return "?";
            }

            string[] partes =
                nomeCompleto.Split(
                    new[] { ' ' },
                    StringSplitOptions.RemoveEmptyEntries
                );

            if (partes.Length == 0)
            {
                return "?";
            }

            if (partes.Length == 1)
            {
                return partes[0]
                    .Substring(0, 1)
                    .ToUpperInvariant();
            }

            return
                (
                    partes[0].Substring(0, 1) +
                    partes[partes.Length - 1]
                        .Substring(0, 1)
                )
                .ToUpperInvariant();
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
                    utilizador.ProviderUserKey.ToString(),
                    out userId))
            {
                return false;
            }

            const string sql = @"
                SELECT Id

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


        #region Interface e mensagens

        private void MostrarSemEducando()
        {
            PnlConteudo.Visible =
                false;

            PnlSemEducando.Visible =
                true;

            LblEducandoSelecionado.Visible =
                false;
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

        private string TextoOuTraco(
            string texto)
        {
            return string.IsNullOrWhiteSpace(
                    texto)
                ? "—"
                : texto.Trim();
        }

        private string Codificar(
            string texto)
        {
            return HttpUtility.HtmlEncode(
                texto ?? string.Empty
            );
        }

        private void TerminarSessao()
        {
            FormsAuthentication.SignOut();

            Session.Clear();
            Session.Abandon();

            Response.Redirect(
                "~/login.aspx"
            );
        }

        #endregion


        #region Classe auxiliar

        private class EducandoDados
        {
            public int Id { get; set; }

            public string NomeCompleto { get; set; }

            public string NumeroProcesso { get; set; }

            public string NIF { get; set; }

            public string Email { get; set; }

            public string Telefone { get; set; }

            public string Foto { get; set; }

            public bool Ativo { get; set; }

            public string Parentesco { get; set; }

            public bool Principal { get; set; }

            public bool AssociacaoAtiva { get; set; }

            public DateTime? DataAssociacao { get; set; }

            public int? TurmaId { get; set; }

            public string Turma { get; set; }

            public string Escola { get; set; }

            public string Agrupamento { get; set; }

            public string AnoLetivo { get; set; }
        }

        #endregion
    }
}