using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.Security;

namespace AlunoGest.encarregado
{
    public partial class dashboard :
        System.Web.UI.Page
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
                CarregarDashboard();
            }
        }

        #endregion


        #region Carregamento principal

        private void CarregarDashboard()
        {
            LimparMensagem();

            int encarregadoId;
            int educandoId;

            if (!TryGetEncarregadoId(
                    out encarregadoId))
            {
                TerminarSessao();
                return;
            }

            if (!TryGetEducandoId(
                    out educandoId))
            {
                MostrarSemEducando();
                return;
            }

            /*
             * Nunca confiamos apenas no valor guardado
             * na sessão.
             *
             * Confirmamos novamente que o aluno selecionado
             * está realmente associado ao encarregado.
             */
            if (!EducandoPertenceAoEncarregado(
                    educandoId,
                    encarregadoId))
            {
                Session["EducandoId"] = null;

                MostrarSemEducando();

                MostrarMensagem(
                    "O educando selecionado já não se encontra " +
                    "associado à sua conta.",
                    true
                );

                return;
            }

            try
            {
                EducandoDados educando =
                    ObterDadosEducando(
                        educandoId,
                        encarregadoId
                    );

                if (educando == null)
                {
                    MostrarSemEducando();

                    MostrarMensagem(
                        "Não foi possível carregar os dados do educando.",
                        true
                    );

                    return;
                }

                PreencherDadosEducando(educando);
                CarregarEventos(educando);

                PnlSemEducando.Visible = false;
                PnlDashboard.Visible = true;

                PnlBadgeEducando.Visible = true;
                LblBadgeEducando.Text =
                    educando.NomeCompleto;
            }
            catch (SqlException ex)
            {
                System.Diagnostics.Trace.TraceError(
                    "Erro de base de dados no dashboard " +
                    "do encarregado: " +
                    ex
                );

                MostrarSemEducando();

                MostrarMensagem(
                    "Não foi possível carregar as informações " +
                    "do educando.",
                    true
                );
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError(
                    "Erro no dashboard do encarregado: " +
                    ex
                );

                MostrarSemEducando();

                MostrarMensagem(
                    "Ocorreu um erro ao carregar o dashboard.",
                    true
                );
            }
        }

        #endregion


        #region Dados do educando

        private EducandoDados ObterDadosEducando(
            int educandoId,
            int encarregadoId)
        {
            const string sql = @"
                SELECT TOP 1

                    a.Id,
                    a.NomeCompleto,
                    a.NumeroProcesso,
                    a.Email,
                    a.Telefone,
                    a.Foto,

                    at2.TurmaId,

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

                    e.Nome AS Escola,

                    al.Descricao AS AnoLetivo

                FROM dbo.Aluno a

                INNER JOIN dbo.AlunoEncarregado ae
                    ON ae.AlunoId = a.Id

                LEFT JOIN dbo.AlunoTurma at2
                    ON at2.AlunoId = a.Id
                   AND at2.Ate IS NULL

                LEFT JOIN dbo.Turma t
                    ON t.Id = at2.TurmaId
                   AND t.Ativa = 1

                LEFT JOIN dbo.Escola e
                    ON e.Id = t.EscolaId

                LEFT JOIN dbo.AnoLetivo al
                    ON al.Id = t.AnoLetivoId

                WHERE a.Id = @AlunoId

                  AND ae.EncarregadoEducacaoId =
                      @EncarregadoEducacaoId

                  AND ae.Ativo = 1
                  AND a.Ativo = 1

                ORDER BY
                    at2.Desde DESC;";

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

                    EducandoDados dados =
                        new EducandoDados();

                    dados.Id =
                        Convert.ToInt32(
                            reader["Id"]
                        );

                    dados.NomeCompleto =
                        ValorTexto(
                            reader["NomeCompleto"]
                        );

                    dados.NumeroProcesso =
                        ValorTexto(
                            reader["NumeroProcesso"]
                        );

                    dados.Email =
                        ValorTexto(
                            reader["Email"]
                        );

                    dados.Telefone =
                        ValorTexto(
                            reader["Telefone"]
                        );

                    dados.Foto =
                        ValorTexto(
                            reader["Foto"]
                        );

                    dados.Turma =
                        ValorTexto(
                            reader["Turma"]
                        );

                    dados.Escola =
                        ValorTexto(
                            reader["Escola"]
                        );

                    dados.AnoLetivo =
                        ValorTexto(
                            reader["AnoLetivo"]
                        );

                    if (reader["TurmaId"] != DBNull.Value)
                    {
                        dados.TurmaId =
                            Convert.ToInt32(
                                reader["TurmaId"]
                            );
                    }

                    return dados;
                }
            }
        }

        private void PreencherDadosEducando(
            EducandoDados educando)
        {
            string numeroProcesso =
                TextoOuTraco(
                    educando.NumeroProcesso
                );

            string turma =
                TextoOuTraco(
                    educando.Turma
                );

            string email =
                TextoOuTraco(
                    educando.Email
                );

            string telefone =
                TextoOuTraco(
                    educando.Telefone
                );

            LblResumoNome.Text =
                educando.NomeCompleto;

            LblResumoProcesso.Text =
                numeroProcesso;

            LblResumoTurma.Text =
                turma;

            LblResumoAnoLetivo.Text =
                string.IsNullOrWhiteSpace(
                    educando.AnoLetivo)
                    ? "Ano letivo não definido"
                    : "Ano letivo: " +
                      educando.AnoLetivo;

            LblNomeEducando.Text =
                educando.NomeCompleto;

            LblNumeroProcesso.Text =
                numeroProcesso;

            LblTurma.Text =
                turma;

            LblEmail.Text =
                email;

            LblTelefone.Text =
                telefone;

            LblDescricaoEducando.Text =
                CriarDescricaoEducando(
                    educando
                );

            CarregarFotografia(
                educando
            );
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
            EducandoDados educando)
        {
            string caminhoFoto =
                educando.Foto;

            if (string.IsNullOrWhiteSpace(
                    caminhoFoto))
            {
                MostrarIniciais(
                    educando.NomeCompleto
                );

                return;
            }

            try
            {
                string urlFoto =
                    NormalizarCaminhoFoto(
                        caminhoFoto
                    );

                if (string.IsNullOrWhiteSpace(
                        urlFoto))
                {
                    MostrarIniciais(
                        educando.NomeCompleto
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
            catch
            {
                MostrarIniciais(
                    educando.NomeCompleto
                );
            }
        }

        private string NormalizarCaminhoFoto(
            string caminho)
        {
            if (string.IsNullOrWhiteSpace(caminho))
            {
                return null;
            }

            caminho = caminho.Trim();

            Uri urlAbsoluta;

            if (Uri.TryCreate(
                    caminho,
                    UriKind.Absolute,
                    out urlAbsoluta))
            {
                if (urlAbsoluta.Scheme ==
                        Uri.UriSchemeHttp ||
                    urlAbsoluta.Scheme ==
                        Uri.UriSchemeHttps)
                {
                    return caminho;
                }

                return null;
            }

            if (caminho.StartsWith(
                    "~/",
                    StringComparison.Ordinal))
            {
                return ResolveUrl(caminho);
            }

            if (caminho.StartsWith(
                    "/",
                    StringComparison.Ordinal))
            {
                return ResolveUrl(
                    "~" + caminho
                );
            }

            return ResolveUrl(
                "~/" + caminho.TrimStart('/')
            );
        }

        private void MostrarIniciais(
            string nomeCompleto)
        {
            ImgEducando.Visible = false;

            PnlFotoPlaceholder.Visible =
                true;

            LblIniciaisEducando.Text =
                ObterIniciais(
                    nomeCompleto
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
                    StringSplitOptions
                        .RemoveEmptyEntries
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

            string primeira =
                partes[0]
                    .Substring(0, 1);

            string ultima =
                partes[partes.Length - 1]
                    .Substring(0, 1);

            return
                (primeira + ultima)
                    .ToUpperInvariant();
        }

        #endregion


        #region Eventos

        private void CarregarEventos(
            EducandoDados educando)
        {
            const string sql = @"
                SELECT TOP 20

                    ev.Id,
                    ev.Titulo,
                    ev.Tipo,
                    ev.DataHora

                FROM dbo.Evento ev

                WHERE ev.DataHora >= SYSDATETIME()

                  AND
                  (
                      /*
                       * Evento criado especificamente
                       * para este aluno.
                       */
                      ev.AlunoId = @AlunoId

                      OR

                      /*
                       * Evento geral da turma atual.
                       */
                      (
                          ev.AlunoId IS NULL
                          AND @TurmaId IS NOT NULL
                          AND ev.TurmaId = @TurmaId
                      )
                  )

                ORDER BY
                    ev.DataHora ASC,
                    ev.Titulo ASC;";

            DataTable eventos =
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
                        "@AlunoId",
                        SqlDbType.Int
                    )
                    .Value = educando.Id;

                cmd.Parameters
                    .Add(
                        "@TurmaId",
                        SqlDbType.Int
                    )
                    .Value =
                    educando.TurmaId.HasValue
                        ? (object)educando.TurmaId.Value
                        : DBNull.Value;

                adapter.Fill(eventos);
            }

            GridEventos.DataSource =
                eventos;

            GridEventos.DataBind();

            LblNumeroEventos.Text =
                eventos.Rows.Count.ToString();

            CarregarProximosEventos(
                eventos
            );
        }

        private void CarregarProximosEventos(
            DataTable eventos)
        {
            DataTable proximos =
                eventos.Clone();

            int quantidade =
                Math.Min(
                    eventos.Rows.Count,
                    5
                );

            for (int i = 0;
                i < quantidade;
                i++)
            {
                proximos.ImportRow(
                    eventos.Rows[i]
                );
            }

            RptProximosEventos.DataSource =
                proximos;

            RptProximosEventos.DataBind();

            PnlSemProximosEventos.Visible =
                proximos.Rows.Count == 0;
        }

        #endregion


        #region Segurança da associação

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

                            INNER JOIN dbo.EncarregadoEducacao e
                                ON e.Id =
                                   ae.EncarregadoEducacaoId

                            WHERE ae.AlunoId =
                                  @AlunoId

                              AND ae.EncarregadoEducacaoId =
                                  @EncarregadoEducacaoId

                              AND ae.Ativo = 1
                              AND a.Ativo = 1
                              AND e.Ativo = 1
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

        #endregion


        #region Sessão

        private bool TryGetEncarregadoId(
            out int encarregadoId)
        {
            encarregadoId = 0;

            if (Session[
                    "EncarregadoEducacaoId"
                ] == null)
            {
                return false;
            }

            return int.TryParse(
                Session[
                    "EncarregadoEducacaoId"
                ].ToString(),
                out encarregadoId
            );
        }

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


        #region Interface e utilidades

        private void MostrarSemEducando()
        {
            PnlDashboard.Visible =
                false;

            PnlSemEducando.Visible =
                true;

            PnlBadgeEducando.Visible =
                false;

            GridEventos.DataSource =
                null;

            GridEventos.DataBind();

            RptProximosEventos.DataSource =
                null;

            RptProximosEventos.DataBind();

            LblNumeroEventos.Text =
                "0";
        }

        private string TextoOuTraco(
            string texto)
        {
            return string.IsNullOrWhiteSpace(
                    texto)
                ? "—"
                : texto.Trim();
        }

        private string ValorTexto(
            object valor)
        {
            return valor == null ||
                   valor == DBNull.Value
                ? string.Empty
                : valor.ToString().Trim();
        }

        private void MostrarMensagem(
            string mensagem,
            bool erro)
        {
            LblMensagem.Text =
                mensagem;

            LblMensagem.CssClass =
                erro
                    ? "mensagem-erro"
                    : "mensagem-sucesso";

            LblMensagem.Visible =
                true;
        }

        private void LimparMensagem()
        {
            LblMensagem.Text =
                string.Empty;

            LblMensagem.CssClass =
                string.Empty;

            LblMensagem.Visible =
                false;
        }

        #endregion


        #region Classe auxiliar

        private class EducandoDados
        {
            public int Id { get; set; }

            public string NomeCompleto { get; set; }

            public string NumeroProcesso { get; set; }

            public string Email { get; set; }

            public string Telefone { get; set; }

            public string Foto { get; set; }

            public int? TurmaId { get; set; }

            public string Turma { get; set; }

            public string Escola { get; set; }

            public string AnoLetivo { get; set; }
        }

        #endregion
    }
}