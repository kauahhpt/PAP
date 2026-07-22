using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.Script.Services;
using System.Web.Security;
using System.Web.Services;
using System.Web.UI.WebControls;

namespace AlunoGest.aluno
{
    public partial class mensagens : System.Web.UI.Page
    {
        private readonly string _connectionString =
            ConfigurationManager
                .ConnectionStrings["DefaultConnection"]
                .ConnectionString;

        public class SugestaoAluno
        {
            public string NomeCompleto { get; set; }

            public string NumeroProcesso { get; set; }

            public string UserName { get; set; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                GarantirTabelas();

                if (!IsPostBack)
                {
                    CarregarTudo();
                }
            }
            catch (Exception ex)
            {
                MostrarMensagem(
                    "Não foi possível carregar as mensagens: " +
                    ex.Message,
                    true
                );
            }
        }

        #region Pesquisa de alunos
        private Guid UserIdAtual
        {
            get
            {
                Guid userId;

                if (Session["UserId"] != null &&
                    Guid.TryParse(
                        Session["UserId"].ToString(),
                        out userId))
                {
                    return userId;
                }

                MembershipUser utilizador =
                    Membership.GetUser(
                        User.Identity.Name,
                        false
                    );

                if (utilizador == null ||
                    utilizador.ProviderUserKey == null ||
                    !Guid.TryParse(
                        utilizador.ProviderUserKey.ToString(),
                        out userId))
                {
                    throw new InvalidOperationException(
                        "Não foi possível identificar o utilizador."
                    );
                }

                Session["UserId"] =
                    userId;

                return userId;
            }
        }
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static List<SugestaoAluno> PesquisarAlunos(
            string termo)
        {
            List<SugestaoAluno> resultados =
                new List<SugestaoAluno>();

            termo =
                (termo ?? string.Empty).Trim();

            if (termo.Length == 0)
                return resultados;

            HttpContext contexto =
                HttpContext.Current;

            if (contexto == null ||
                contexto.Session == null)
            {
                throw new InvalidOperationException(
                    "A sessão terminou."
                );
            }

            string connectionString =
                ConfigurationManager
                    .ConnectionStrings["DefaultConnection"]
                    .ConnectionString;

            using (SqlConnection conn =
                new SqlConnection(connectionString))
            {
                conn.Open();

                int alunoAtualId;

                if (contexto.Session["AlunoID"] != null &&
                    int.TryParse(
                        contexto.Session["AlunoID"].ToString(),
                        out alunoAtualId))
                {
                    // O ID do aluno já está guardado na sessão.
                }
                else
                {
                    if (contexto.Session["UserId"] == null)
                    {
                        throw new InvalidOperationException(
                            "A sessão terminou."
                        );
                    }

                    Guid userId;

                    if (!Guid.TryParse(
                            contexto.Session["UserId"].ToString(),
                            out userId))
                    {
                        throw new InvalidOperationException(
                            "O utilizador autenticado é inválido."
                        );
                    }

                    const string sqlAlunoAtual = @"
                        SELECT Id
                        FROM dbo.Aluno
                        WHERE UserId = @UserId
                          AND Ativo = 1;";

                    using (SqlCommand cmdAlunoAtual =
                        new SqlCommand(
                            sqlAlunoAtual,
                            conn))
                    {
                        cmdAlunoAtual.Parameters
                            .Add(
                                "@UserId",
                                SqlDbType.UniqueIdentifier
                            )
                            .Value = userId;

                        object resultadoAluno =
                            cmdAlunoAtual.ExecuteScalar();

                        if (resultadoAluno == null ||
                            resultadoAluno == DBNull.Value)
                        {
                            throw new InvalidOperationException(
                                "Aluno não encontrado."
                            );
                        }

                        alunoAtualId =
                            Convert.ToInt32(
                                resultadoAluno
                            );

                        contexto.Session["AlunoID"] =
                            alunoAtualId;
                    }
                }

                const string sql = @"
                    SELECT
                        al.NomeCompleto,
                        al.NumeroProcesso,
                        u.UserName

                    FROM dbo.Aluno al

                    INNER JOIN dbo.Users u
                        ON u.UserId = al.UserId

                    WHERE al.Ativo = 1

                      AND al.Id <> @AlunoAtualId

                      AND al.NomeCompleto
                            COLLATE Latin1_General_CI_AI
                            LIKE @Termo + N'%'

                      AND EXISTS
                      (
                          SELECT 1

                          FROM dbo.UsersInRoles ur

                          INNER JOIN dbo.Roles r
                              ON r.RoleId = ur.RoleId

                          WHERE ur.UserId = u.UserId
                            AND LOWER(r.RoleName) = N'aluno'
                      )

                      AND NOT EXISTS
                      (
                          SELECT 1

                          FROM dbo.AmizadeAluno amizade

                          WHERE
                              (
                                  amizade.SolicitanteAlunoId =
                                      @AlunoAtualId

                                  AND amizade.DestinatarioAlunoId =
                                      al.Id
                              )

                              OR

                              (
                                  amizade.SolicitanteAlunoId =
                                      al.Id

                                  AND amizade.DestinatarioAlunoId =
                                      @AlunoAtualId
                              )
                      )

                    ORDER BY
                        al.NomeCompleto,
                        al.NumeroProcesso;";

                using (SqlCommand cmd =
                    new SqlCommand(sql, conn))
                {
                    cmd.Parameters
                        .Add(
                            "@AlunoAtualId",
                            SqlDbType.Int
                        )
                        .Value = alunoAtualId;

                    cmd.Parameters
                        .Add(
                            "@Termo",
                            SqlDbType.NVarChar,
                            200
                        )
                        .Value = termo;

                    using (SqlDataReader reader =
                        cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            resultados.Add(
                                new SugestaoAluno
                                {
                                    NomeCompleto =
                                        reader["NomeCompleto"]
                                            .ToString(),

                                    NumeroProcesso =
                                        reader["NumeroProcesso"] ==
                                        DBNull.Value
                                            ? string.Empty
                                            : reader["NumeroProcesso"]
                                                .ToString(),

                                    UserName =
                                        reader["UserName"]
                                            .ToString()
                                }
                            );
                        }
                    }
                }
            }

            return resultados;
        }

        #endregion

        #region Amizades

        protected void BtnAdicionarAmigo_Click(
            object sender,
            EventArgs e)
        {
            string username =
                HdnUsernameAmigo.Value.Trim();

            if (username.Length == 0)
            {
                MostrarMensagem(
                    "Pesquisa pelo nome e seleciona um aluno " +
                    "na lista de sugestões.",
                    true
                );

                return;
            }

            try
            {
                int outroAlunoId =
                    ObterAlunoIdPorUsername(
                        username
                    );

                if (outroAlunoId == AlunoId)
                {
                    MostrarMensagem(
                        "Não podes adicionar a tua própria conta.",
                        true
                    );

                    return;
                }

                if (AmizadeExiste(
                        AlunoId,
                        outroAlunoId))
                {
                    MostrarMensagem(
                        "Já existe uma amizade ou pedido " +
                        "pendente com esse aluno.",
                        true
                    );

                    return;
                }

                const string sql = @"
                    INSERT INTO dbo.AmizadeAluno
                    (
                        SolicitanteAlunoId,
                        DestinatarioAlunoId,
                        Estado
                    )
                    VALUES
                    (
                        @SolicitanteAlunoId,
                        @DestinatarioAlunoId,
                        N'Pendente'
                    );";

                Executar(
                    sql,

                    Param(
                        "@SolicitanteAlunoId",
                        AlunoId
                    ),

                    Param(
                        "@DestinatarioAlunoId",
                        outroAlunoId
                    )
                );

                TxtUsernameAmigo.Text =
                    string.Empty;

                HdnUsernameAmigo.Value =
                    string.Empty;

                MostrarMensagem(
                    "Pedido de amizade enviado.",
                    false
                );

                CarregarTudo();
            }
            catch (Exception ex)
            {
                MostrarMensagem(
                    ex.Message,
                    true
                );
            }
        }

        protected void GridPedidosAmizade_RowCommand(
            object sender,
            GridViewCommandEventArgs e)
        {
            int index;

            if (e.CommandName != "AceitarAmizade" ||
                !int.TryParse(
                    e.CommandArgument.ToString(),
                    out index))
            {
                return;
            }

            if (index < 0 ||
                index >= GridPedidosAmizade.Rows.Count)
            {
                return;
            }

            int pedidoId =
                Convert.ToInt32(
                    GridPedidosAmizade
                        .DataKeys[index]
                        .Value
                );

            const string sql = @"
                UPDATE dbo.AmizadeAluno

                SET
                    Estado = N'Aceite',
                    RespondidoEm = SYSDATETIME()

                WHERE Id = @Id
                  AND DestinatarioAlunoId = @AlunoId
                  AND Estado = N'Pendente';";

            Executar(
                sql,
                Param("@Id", pedidoId),
                Param("@AlunoId", AlunoId)
            );

            MostrarMensagem(
                "Pedido de amizade aceite.",
                false
            );

            CarregarTudo();
        }

        #endregion

        #region Grupos

        protected void BtnCriarGrupo_Click(
            object sender,
            EventArgs e)
        {
            string nome =
                TxtNomeGrupo.Text.Trim();

            if (nome.Length == 0)
            {
                MostrarMensagem(
                    "Indica o nome do grupo.",
                    true
                );

                return;
            }

            List<int> convidados =
                new List<int>();

            foreach (ListItem item in
                CheckAmigosGrupo.Items)
            {
                if (item.Selected)
                {
                    convidados.Add(
                        Convert.ToInt32(
                            item.Value
                        )
                    );
                }
            }

            if (convidados.Count == 0)
            {
                MostrarMensagem(
                    "Escolhe pelo menos um amigo para convidar.",
                    true
                );

                return;
            }

            using (SqlConnection c =
                new SqlConnection(_connectionString))
            {
                c.Open();

                using (SqlTransaction tx =
                    c.BeginTransaction())
                {
                    try
                    {
                        int grupoId;

                        using (SqlCommand cmd =
                            new SqlCommand(
                                @"
                                    INSERT INTO dbo.GrupoAluno
                                    (
                                        Nome,
                                        CriadorAlunoId
                                    )
                                    VALUES
                                    (
                                        @Nome,
                                        @CriadorAlunoId
                                    );

                                    SELECT CAST(
                                        SCOPE_IDENTITY()
                                        AS int
                                    );",
                                c,
                                tx))
                        {
                            cmd.Parameters
                                .AddWithValue(
                                    "@Nome",
                                    nome
                                );

                            cmd.Parameters
                                .AddWithValue(
                                    "@CriadorAlunoId",
                                    AlunoId
                                );

                            grupoId =
                                Convert.ToInt32(
                                    cmd.ExecuteScalar()
                                );
                        }

                        using (SqlCommand cmd =
                            new SqlCommand(
                                @"
                                    INSERT INTO dbo.GrupoAlunoMembro
                                    (
                                        GrupoId,
                                        AlunoId,
                                        Estado,
                                        RespondidoEm
                                    )
                                    VALUES
                                    (
                                        @GrupoId,
                                        @AlunoId,
                                        N'Aceite',
                                        SYSDATETIME()
                                    );",
                                c,
                                tx))
                        {
                            cmd.Parameters
                                .AddWithValue(
                                    "@GrupoId",
                                    grupoId
                                );

                            cmd.Parameters
                                .AddWithValue(
                                    "@AlunoId",
                                    AlunoId
                                );

                            cmd.ExecuteNonQuery();
                        }

                        foreach (
                            int alunoConvidadoId
                            in convidados)
                        {
                            if (!SaoAmigos(
                                    AlunoId,
                                    alunoConvidadoId,
                                    c,
                                    tx))
                            {
                                continue;
                            }

                            using (SqlCommand cmd =
                                new SqlCommand(
                                    @"
                                        INSERT INTO dbo.GrupoAlunoMembro
                                        (
                                            GrupoId,
                                            AlunoId,
                                            Estado
                                        )
                                        VALUES
                                        (
                                            @GrupoId,
                                            @AlunoId,
                                            N'Pendente'
                                        );",
                                    c,
                                    tx))
                            {
                                cmd.Parameters
                                    .AddWithValue(
                                        "@GrupoId",
                                        grupoId
                                    );

                                cmd.Parameters
                                    .AddWithValue(
                                        "@AlunoId",
                                        alunoConvidadoId
                                    );

                                cmd.ExecuteNonQuery();
                            }
                        }

                        tx.Commit();
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }

            TxtNomeGrupo.Text =
                string.Empty;

            MostrarMensagem(
                "Grupo criado. Os amigos convidados " +
                "precisam aceitar para entrar.",
                false
            );

            CarregarTudo();
        }

        protected void GridConvitesGrupo_RowCommand(
            object sender,
            GridViewCommandEventArgs e)
        {
            int index;

            if (e.CommandName != "AceitarGrupo" ||
                !int.TryParse(
                    e.CommandArgument.ToString(),
                    out index))
            {
                return;
            }

            if (index < 0 ||
                index >= GridConvitesGrupo.Rows.Count)
            {
                return;
            }

            int grupoId =
                Convert.ToInt32(
                    GridConvitesGrupo
                        .DataKeys[index]
                        .Value
                );

            const string sql = @"
                UPDATE dbo.GrupoAlunoMembro

                SET
                    Estado = N'Aceite',
                    RespondidoEm = SYSDATETIME()

                WHERE GrupoId = @GrupoId
                  AND AlunoId = @AlunoId
                  AND Estado = N'Pendente';";

            Executar(
                sql,
                Param("@GrupoId", grupoId),
                Param("@AlunoId", AlunoId)
            );

            MostrarMensagem(
                "Convite aceite. Já podes conversar no grupo.",
                false
            );

            CarregarTudo();
        }

        #endregion

        #region Conversas e mensagens

        protected void RepeaterConversas_ItemCommand(
            object source,
            RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "AbrirConversa")
                return;

            string[] partes =
                e.CommandArgument
                    .ToString()
                    .Split(':');

            int conversaId;

            if (partes.Length != 2 ||
                !int.TryParse(
                    partes[1],
                    out conversaId))
            {
                return;
            }

            HdnTipoConversa.Value =
                partes[0];

            HdnConversaId.Value =
                conversaId.ToString();

            MarcarConversaComoLida(
                partes[0],
                conversaId
            );

            CarregarConversas();
            CarregarConversaAtual();
        }

        protected void BtnEnviarMensagem_Click(
    object sender,
    EventArgs e)
        {
            string texto =
                TxtMensagem.Text.Trim();

            if (texto.Length == 0)
            {
                MostrarMensagem(
                    "Escreve uma mensagem antes de enviar.",
                    true
                );

                return;
            }

            int conversaId;

            if (!int.TryParse(
                    HdnConversaId.Value,
                    out conversaId) ||
                string.IsNullOrEmpty(
                    HdnTipoConversa.Value))
            {
                MostrarMensagem(
                    "Escolhe uma conversa primeiro.",
                    true
                );

                return;
            }

            if (HdnTipoConversa.Value == "amigo")
            {
                if (!SaoAmigos(
                        AlunoId,
                        conversaId))
                {
                    MostrarMensagem(
                        "Só podes conversar com alunos que já aceitaram a amizade.",
                        true
                    );

                    return;
                }

                const string sql = @"
            INSERT INTO dbo.MensagemAlunoPrivada
            (
                DeAlunoId,
                ParaAlunoId,
                Texto
            )
            VALUES
            (
                @DeAlunoId,
                @ParaAlunoId,
                @Texto
            );";

                Executar(
                    sql,
                    Param("@DeAlunoId", AlunoId),
                    Param("@ParaAlunoId", conversaId),
                    Param("@Texto", texto)
                );
            }
            else if (HdnTipoConversa.Value == "grupo")
            {
                if (!AlunoEstaNoGrupo(
                        conversaId))
                {
                    MostrarMensagem(
                        "Ainda não tens acesso a este grupo.",
                        true
                    );

                    return;
                }

                const string sql = @"
            INSERT INTO dbo.MensagemAlunoGrupo
            (
                GrupoId,
                DeAlunoId,
                Texto
            )
            VALUES
            (
                @GrupoId,
                @DeAlunoId,
                @Texto
            );";

                Executar(
                    sql,
                    Param("@GrupoId", conversaId),
                    Param("@DeAlunoId", AlunoId),
                    Param("@Texto", texto)
                );
            }
            else if (HdnTipoConversa.Value == "escolar")
            {
                string nomeProfessor;

                if (!ConversaEscolarPermitida(
                        conversaId,
                        out nomeProfessor))
                {
                    MostrarMensagem(
                        "Já não tens acesso a esta conversa escolar.",
                        true
                    );

                    return;
                }

                const string sql = @"
            INSERT INTO dbo.MensagemDireta
            (
                ConversaId,
                RemetenteUserId,
                Texto
            )
            VALUES
            (
                @ConversaId,
                @RemetenteUserId,
                @Texto
            );";

                Executar(
                    sql,
                    Param("@ConversaId", conversaId),
                    Param("@RemetenteUserId", UserIdAtual),
                    Param("@Texto", texto)
                );
            }
            else
            {
                MostrarMensagem(
                    "O tipo de conversa é inválido.",
                    true
                );

                return;
            }

            TxtMensagem.Text =
                string.Empty;

            CarregarConversas();
            CarregarConversaAtual();
        }

        #endregion

        #region Aluno autenticado

        private int AlunoId
        {
            get
            {
                int id;

                if (Session["AlunoID"] != null &&
                    int.TryParse(
                        Session["AlunoID"].ToString(),
                        out id))
                {
                    return id;
                }

                if (Session["UserId"] == null)
                {
                    throw new InvalidOperationException(
                        "Sessão terminada. Faz login novamente."
                    );
                }

                using (SqlConnection c =
                    new SqlConnection(_connectionString))
                using (SqlCommand cmd =
                    new SqlCommand(
                        @"
                            SELECT Id
                            FROM dbo.Aluno
                            WHERE UserId = @UserId
                              AND Ativo = 1;",
                        c))
                {
                    cmd.Parameters
                        .Add(
                            "@UserId",
                            SqlDbType.UniqueIdentifier
                        )
                        .Value =
                        new Guid(
                            Session["UserId"].ToString()
                        );

                    c.Open();

                    object value =
                        cmd.ExecuteScalar();

                    if (value == null ||
                        value == DBNull.Value)
                    {
                        throw new InvalidOperationException(
                            "Aluno não encontrado."
                        );
                    }

                    id =
                        Convert.ToInt32(value);

                    Session["AlunoID"] =
                        id;

                    return id;
                }
            }
        }

        #endregion

        #region Carregamento da página

        private void CarregarTudo()
        {
            CarregarPedidosAmizade();
            CarregarAmigos();
            CarregarConvitesGrupo();
            CarregarConversas();
            CarregarConversaAtual();
        }

        private void CarregarPedidosAmizade()
        {
            const string sql = @"
                SELECT
                    am.Id,
                    al.NomeCompleto,
                    u.UserName

                FROM dbo.AmizadeAluno am

                INNER JOIN dbo.Aluno al
                    ON al.Id = am.SolicitanteAlunoId

                INNER JOIN dbo.Users u
                    ON u.UserId = al.UserId

                WHERE am.DestinatarioAlunoId = @AlunoId
                  AND am.Estado = N'Pendente'

                ORDER BY am.CriadoEm DESC;";

            BindGrid(
                GridPedidosAmizade,
                sql,
                Param("@AlunoId", AlunoId)
            );
        }

        private void CarregarAmigos()
        {
            const string sql = @"
                SELECT
                    amigo.Id,
                    amigo.NomeCompleto,
                    u.UserName

                FROM dbo.AmizadeAluno am

                INNER JOIN dbo.Aluno amigo
                    ON amigo.Id =
                        CASE
                            WHEN am.SolicitanteAlunoId = @AlunoId
                                THEN am.DestinatarioAlunoId
                            ELSE am.SolicitanteAlunoId
                        END

                INNER JOIN dbo.Users u
                    ON u.UserId = amigo.UserId

                WHERE am.Estado = N'Aceite'
                  AND @AlunoId IN
                  (
                      am.SolicitanteAlunoId,
                      am.DestinatarioAlunoId
                  )

                ORDER BY amigo.NomeCompleto;";

            DataTable dt =
                ObterTabela(
                    sql,
                    Param("@AlunoId", AlunoId)
                );

            CheckAmigosGrupo.Items.Clear();

            foreach (DataRow row in dt.Rows)
            {
                string texto =
                    row["NomeCompleto"] +
                    " (" +
                    row["UserName"] +
                    ")";

                string valor =
                    row["Id"].ToString();

                CheckAmigosGrupo.Items.Add(
                    new ListItem(
                        texto,
                        valor
                    )
                );
            }
        }

        private void CarregarConvitesGrupo()
        {
            const string sql = @"
                SELECT
                    g.Id AS GrupoId,
                    g.Nome,
                    criador.NomeCompleto AS Criador

                FROM dbo.GrupoAlunoMembro gm

                INNER JOIN dbo.GrupoAluno g
                    ON g.Id = gm.GrupoId

                INNER JOIN dbo.Aluno criador
                    ON criador.Id = g.CriadorAlunoId

                WHERE gm.AlunoId = @AlunoId
                  AND gm.Estado = N'Pendente'

                ORDER BY gm.CriadoEm DESC;";

            BindGrid(
                GridConvitesGrupo,
                sql,
                Param("@AlunoId", AlunoId)
            );
        }

        private void CarregarConversas()
        {
            const string sql = @"
        SELECT
            'amigo' AS Tipo,
            amigo.Id,
            amigo.NomeCompleto AS Nome,
            N'privado' AS TipoTexto,

            COALESCE(
                ultima.Texto,
                N'Sem mensagens'
            ) AS UltimaMensagem,

            ultima.CriadoEm AS DataUltima,

            (
                SELECT COUNT(1)

                FROM dbo.MensagemAlunoPrivada mpNaoLida

                LEFT JOIN dbo.ConversaAlunoLeitura leitura
                    ON leitura.AlunoId = @AlunoId
                   AND leitura.TipoConversa = N'amigo'
                   AND leitura.ConversaId = amigo.Id

                WHERE mpNaoLida.DeAlunoId = amigo.Id
                  AND mpNaoLida.ParaAlunoId = @AlunoId

                  AND
                  (
                      leitura.LidaEm IS NULL
                      OR mpNaoLida.CriadoEm > leitura.LidaEm
                  )
            ) AS NaoLidas

        FROM dbo.AmizadeAluno am

        INNER JOIN dbo.Aluno amigo
            ON amigo.Id =
                CASE
                    WHEN am.SolicitanteAlunoId = @AlunoId
                        THEN am.DestinatarioAlunoId
                    ELSE am.SolicitanteAlunoId
                END

        OUTER APPLY
        (
            SELECT TOP 1
                Texto,
                CriadoEm

            FROM dbo.MensagemAlunoPrivada mp

            WHERE
                (
                    mp.DeAlunoId = @AlunoId
                    AND mp.ParaAlunoId = amigo.Id
                )

                OR

                (
                    mp.DeAlunoId = amigo.Id
                    AND mp.ParaAlunoId = @AlunoId
                )

            ORDER BY
                mp.CriadoEm DESC,
                mp.Id DESC
        ) ultima

        WHERE am.Estado = N'Aceite'

          AND @AlunoId IN
          (
              am.SolicitanteAlunoId,
              am.DestinatarioAlunoId
          )


        UNION ALL


        SELECT
            'grupo' AS Tipo,
            g.Id,
            g.Nome,
            N'grupo' AS TipoTexto,

            COALESCE(
                ultima.Texto,
                N'Sem mensagens'
            ) AS UltimaMensagem,

            ultima.CriadoEm AS DataUltima,

            (
                SELECT COUNT(1)

                FROM dbo.MensagemAlunoGrupo mgNaoLida

                LEFT JOIN dbo.ConversaAlunoLeitura leitura
                    ON leitura.AlunoId = @AlunoId
                   AND leitura.TipoConversa = N'grupo'
                   AND leitura.ConversaId = g.Id

                WHERE mgNaoLida.GrupoId = g.Id
                  AND mgNaoLida.DeAlunoId <> @AlunoId

                  AND
                  (
                      leitura.LidaEm IS NULL
                      OR mgNaoLida.CriadoEm > leitura.LidaEm
                  )
            ) AS NaoLidas

        FROM dbo.GrupoAlunoMembro gm

        INNER JOIN dbo.GrupoAluno g
            ON g.Id = gm.GrupoId

        OUTER APPLY
        (
            SELECT TOP 1
                Texto,
                CriadoEm

            FROM dbo.MensagemAlunoGrupo mg

            WHERE mg.GrupoId = g.Id

            ORDER BY
                mg.CriadoEm DESC,
                mg.Id DESC
        ) ultima

        WHERE gm.AlunoId = @AlunoId
          AND gm.Estado = N'Aceite'


        UNION ALL


        SELECT
            'escolar' AS Tipo,
            conversa.Id,
            professor.Nome,
            N'professor' AS TipoTexto,

            COALESCE(
                ultima.Texto,
                N'Sem mensagens'
            ) AS UltimaMensagem,

            ultima.CriadoEm AS DataUltima,

            (
                SELECT COUNT(1)

                FROM dbo.MensagemDireta mensagemNaoLida

                WHERE mensagemNaoLida.ConversaId =
                      conversa.Id

                  AND mensagemNaoLida.RemetenteUserId <>
                      @UserIdAtual

                  AND mensagemNaoLida.LidaEm IS NULL
            ) AS NaoLidas

        FROM dbo.ConversaDireta conversa

        INNER JOIN dbo.Professor professor
            ON professor.UserId =
                CASE
                    WHEN conversa.Utilizador1Id =
                         @UserIdAtual
                        THEN conversa.Utilizador2Id
                    ELSE conversa.Utilizador1Id
                END

        OUTER APPLY
        (
            SELECT TOP 1
                mensagem.Texto,
                mensagem.CriadoEm

            FROM dbo.MensagemDireta mensagem

            WHERE mensagem.ConversaId =
                  conversa.Id

            ORDER BY
                mensagem.CriadoEm DESC,
                mensagem.Id DESC
        ) ultima

        WHERE conversa.Ativa = 1

          AND
          (
              conversa.Utilizador1Id =
                  @UserIdAtual

              OR conversa.Utilizador2Id =
                  @UserIdAtual
          )

          AND professor.Ativo = 1

          AND EXISTS
          (
              SELECT 1

              FROM dbo.AlunoTurma alunoTurma

              INNER JOIN dbo.TurmaDisciplina turmaDisciplina
                  ON turmaDisciplina.TurmaId =
                     alunoTurma.TurmaId

              INNER JOIN dbo.TurmaDisciplinaProfessor atribuicao
                  ON atribuicao.TurmaDisciplinaId =
                     turmaDisciplina.Id

              WHERE alunoTurma.AlunoId =
                    @AlunoId

                AND atribuicao.ProfessorId =
                    professor.Id

                AND alunoTurma.Ate IS NULL
                AND atribuicao.Ate IS NULL
          )

        ORDER BY
            DataUltima DESC,
            Nome;";

            DataTable dt =
                ObterTabela(
                    sql,
                    Param("@AlunoId", AlunoId),
                    Param("@UserIdAtual", UserIdAtual)
                );

            if (dt.Columns["Ativa"] == null)
            {
                dt.Columns.Add(
                    "Ativa",
                    typeof(bool)
                );
            }

            foreach (DataRow row in dt.Rows)
            {
                row["Ativa"] =
                    row["Tipo"].ToString() ==
                        HdnTipoConversa.Value

                    &&

                    row["Id"].ToString() ==
                        HdnConversaId.Value;
            }

            RepeaterConversas.DataSource =
                dt;

            RepeaterConversas.DataBind();

            PainelSemConversas.Visible =
                dt.Rows.Count == 0;
        }

        private void CarregarConversaAtual()
        {
            int conversaId;

            if (!int.TryParse(
                    HdnConversaId.Value,
                    out conversaId) ||
                string.IsNullOrEmpty(
                    HdnTipoConversa.Value))
            {
                RepeaterMensagens.DataSource =
                    null;

                RepeaterMensagens.DataBind();

                PainelSemMensagens.Visible =
                    true;

                LblConversaAtual.Text =
                    "Escolhe uma conversa";

                LblTipoConversa.Text =
                    string.Empty;

                return;
            }

            DataTable dt;

            if (HdnTipoConversa.Value == "amigo")
            {
                if (!SaoAmigos(
                        AlunoId,
                        conversaId))
                {
                    return;
                }

                LblConversaAtual.Text =
                    ObterNomeAluno(
                        conversaId
                    );

                LblTipoConversa.Text =
                    "privado";

                const string sql = @"
            SELECT TOP 100

                CASE
                    WHEN m.DeAlunoId = @AlunoId
                        THEN CAST(1 AS bit)
                    ELSE CAST(0 AS bit)
                END AS Minha,

                autor.NomeCompleto AS Autor,
                m.Texto,
                m.CriadoEm

            FROM dbo.MensagemAlunoPrivada m

            INNER JOIN dbo.Aluno autor
                ON autor.Id = m.DeAlunoId

            WHERE
                (
                    m.DeAlunoId = @AlunoId
                    AND m.ParaAlunoId = @OutroAlunoId
                )

                OR

                (
                    m.DeAlunoId = @OutroAlunoId
                    AND m.ParaAlunoId = @AlunoId
                )

            ORDER BY m.CriadoEm;";

                dt =
                    ObterTabela(
                        sql,
                        Param("@AlunoId", AlunoId),
                        Param("@OutroAlunoId", conversaId)
                    );

                MarcarConversaComoLida(
                    "amigo",
                    conversaId
                );
            }
            else if (HdnTipoConversa.Value == "grupo")
            {
                if (!AlunoEstaNoGrupo(
                        conversaId))
                {
                    return;
                }

                LblConversaAtual.Text =
                    ObterNomeGrupo(
                        conversaId
                    );

                LblTipoConversa.Text =
                    "grupo";

                const string sql = @"
            SELECT TOP 100

                CASE
                    WHEN m.DeAlunoId = @AlunoId
                        THEN CAST(1 AS bit)
                    ELSE CAST(0 AS bit)
                END AS Minha,

                autor.NomeCompleto AS Autor,
                m.Texto,
                m.CriadoEm

            FROM dbo.MensagemAlunoGrupo m

            INNER JOIN dbo.Aluno autor
                ON autor.Id = m.DeAlunoId

            WHERE m.GrupoId = @GrupoId

            ORDER BY m.CriadoEm;";

                dt =
                    ObterTabela(
                        sql,
                        Param("@AlunoId", AlunoId),
                        Param("@GrupoId", conversaId)
                    );

                MarcarConversaComoLida(
                    "grupo",
                    conversaId
                );
            }
            else if (HdnTipoConversa.Value == "escolar")
            {
                string nomeProfessor;

                if (!ConversaEscolarPermitida(
                        conversaId,
                        out nomeProfessor))
                {
                    HdnTipoConversa.Value =
                        string.Empty;

                    HdnConversaId.Value =
                        string.Empty;

                    return;
                }

                LblConversaAtual.Text =
                    nomeProfessor;

                LblTipoConversa.Text =
                    "professor";

                const string sql = @"
            SELECT
                mensagens.Minha,
                mensagens.Autor,
                mensagens.Texto,
                mensagens.CriadoEm

            FROM
            (
                SELECT TOP 100

                    CASE
                        WHEN m.RemetenteUserId =
                             @UserIdAtual
                            THEN CAST(1 AS BIT)
                        ELSE CAST(0 AS BIT)
                    END AS Minha,

                    CASE
                        WHEN m.RemetenteUserId =
                             @UserIdAtual
                            THEN N'Você'
                        ELSE @NomeProfessor
                    END AS Autor,

                    m.Texto,
                    m.CriadoEm,
                    m.Id

                FROM dbo.MensagemDireta m

                WHERE m.ConversaId =
                      @ConversaId

                ORDER BY
                    m.CriadoEm DESC,
                    m.Id DESC

            ) AS mensagens

            ORDER BY
                mensagens.CriadoEm,
                mensagens.Id;";

                dt =
                    ObterTabela(
                        sql,
                        Param("@UserIdAtual", UserIdAtual),
                        Param("@NomeProfessor", nomeProfessor),
                        Param("@ConversaId", conversaId)
                    );

                MarcarConversaEscolarComoLida(
                    conversaId
                );
            }
            else
            {
                return;
            }

            RepeaterMensagens.DataSource =
                dt;

            RepeaterMensagens.DataBind();

            PainelSemMensagens.Visible =
                dt.Rows.Count == 0;

            CarregarConversas();
        }
        #endregion

        #region Métodos de apoio às conversas

        private void MarcarConversaComoLida(
            string tipoConversa,
            int conversaId)
        {
            if (tipoConversa != "amigo" &&
                tipoConversa != "grupo")
            {
                return;
            }

            const string sql = @"
                MERGE dbo.ConversaAlunoLeitura AS alvo

                USING
                (
                    SELECT
                        @AlunoId AS AlunoId,
                        @TipoConversa AS TipoConversa,
                        @ConversaId AS ConversaId
                ) AS origem

                ON alvo.AlunoId = origem.AlunoId
                   AND alvo.TipoConversa = origem.TipoConversa
                   AND alvo.ConversaId = origem.ConversaId

                WHEN MATCHED THEN

                    UPDATE SET
                        LidaEm = SYSDATETIME()

                WHEN NOT MATCHED THEN

                    INSERT
                    (
                        AlunoId,
                        TipoConversa,
                        ConversaId,
                        LidaEm
                    )
                    VALUES
                    (
                        origem.AlunoId,
                        origem.TipoConversa,
                        origem.ConversaId,
                        SYSDATETIME()
                    );";

            Executar(
                sql,
                Param("@AlunoId", AlunoId),
                Param("@TipoConversa", tipoConversa),
                Param("@ConversaId", conversaId)
            );
        }

        private string ObterNomeAluno(
            int alunoId)
        {
            object value =
                Escalar(
                    @"
                        SELECT NomeCompleto
                        FROM dbo.Aluno
                        WHERE Id = @Id;",
                    Param("@Id", alunoId)
                );

            return value == null ||
                   value == DBNull.Value
                ? "Conversa privada"
                : value.ToString();
        }

        private string ObterNomeGrupo(
            int grupoId)
        {
            object value =
                Escalar(
                    @"
                        SELECT Nome
                        FROM dbo.GrupoAluno
                        WHERE Id = @Id;",
                    Param("@Id", grupoId)
                );

            return value == null ||
                   value == DBNull.Value
                ? "Conversa de grupo"
                : value.ToString();
        }

        private int ObterAlunoIdPorUsername(
            string username)
        {
            const string sql = @"
                SELECT TOP 1
                    al.Id

                FROM dbo.Users u

                INNER JOIN dbo.UsersInRoles ur
                    ON ur.UserId = u.UserId

                INNER JOIN dbo.Roles r
                    ON r.RoleId = ur.RoleId

                INNER JOIN dbo.Aluno al
                    ON al.UserId = u.UserId
                   AND al.Ativo = 1

                WHERE LOWER(u.UserName) =
                        LOWER(@UserName)

                  AND LOWER(r.RoleName) =
                        N'aluno';";

            object value =
                Escalar(
                    sql,
                    Param("@UserName", username)
                );

            if (value == null ||
                value == DBNull.Value)
            {
                throw new InvalidOperationException(
                    "Aluno não encontrado com esse nome de utilizador."
                );
            }

            return Convert.ToInt32(value);
        }

        private bool AmizadeExiste(
            int alunoA,
            int alunoB)
        {
            const string sql = @"
                SELECT COUNT(1)

                FROM dbo.AmizadeAluno

                WHERE
                    (
                        SolicitanteAlunoId = @AlunoA
                        AND DestinatarioAlunoId = @AlunoB
                    )

                    OR

                    (
                        SolicitanteAlunoId = @AlunoB
                        AND DestinatarioAlunoId = @AlunoA
                    );";

            return Convert.ToInt32(
                Escalar(
                    sql,
                    Param("@AlunoA", alunoA),
                    Param("@AlunoB", alunoB)
                )
            ) > 0;
        }

        private bool SaoAmigos(
            int alunoA,
            int alunoB)
        {
            const string sql = @"
                SELECT COUNT(1)

                FROM dbo.AmizadeAluno

                WHERE Estado = N'Aceite'

                  AND
                  (
                      (
                          SolicitanteAlunoId = @AlunoA
                          AND DestinatarioAlunoId = @AlunoB
                      )

                      OR

                      (
                          SolicitanteAlunoId = @AlunoB
                          AND DestinatarioAlunoId = @AlunoA
                      )
                  );";

            return Convert.ToInt32(
                Escalar(
                    sql,
                    Param("@AlunoA", alunoA),
                    Param("@AlunoB", alunoB)
                )
            ) > 0;
        }

        private bool SaoAmigos(
            int alunoA,
            int alunoB,
            SqlConnection c,
            SqlTransaction tx)
        {
            using (SqlCommand cmd =
                new SqlCommand(
                    @"
                        SELECT COUNT(1)

                        FROM dbo.AmizadeAluno

                        WHERE Estado = N'Aceite'

                          AND
                          (
                              (
                                  SolicitanteAlunoId = @AlunoA
                                  AND DestinatarioAlunoId = @AlunoB
                              )

                              OR

                              (
                                  SolicitanteAlunoId = @AlunoB
                                  AND DestinatarioAlunoId = @AlunoA
                              )
                          );",
                    c,
                    tx))
            {
                cmd.Parameters
                    .AddWithValue(
                        "@AlunoA",
                        alunoA
                    );

                cmd.Parameters
                    .AddWithValue(
                        "@AlunoB",
                        alunoB
                    );

                return Convert.ToInt32(
                    cmd.ExecuteScalar()
                ) > 0;
            }
        }

        private bool AlunoEstaNoGrupo(
            int grupoId)
        {
            const string sql = @"
                SELECT COUNT(1)

                FROM dbo.GrupoAlunoMembro

                WHERE GrupoId = @GrupoId
                  AND AlunoId = @AlunoId
                  AND Estado = N'Aceite';";

            return Convert.ToInt32(
                Escalar(
                    sql,
                    Param("@GrupoId", grupoId),
                    Param("@AlunoId", AlunoId)
                )
            ) > 0;
        }

        #endregion

        #region Criação das tabelas

        private void GarantirTabelas()
        {
            const string sql = @"
                IF OBJECT_ID(
                    'dbo.AmizadeAluno',
                    'U'
                ) IS NULL
                BEGIN

                    CREATE TABLE dbo.AmizadeAluno
                    (
                        Id int IDENTITY(1,1)
                            NOT NULL
                            PRIMARY KEY,

                        SolicitanteAlunoId int
                            NOT NULL,

                        DestinatarioAlunoId int
                            NOT NULL,

                        AlunoMenorId AS
                        (
                            CASE
                                WHEN SolicitanteAlunoId <
                                     DestinatarioAlunoId
                                    THEN SolicitanteAlunoId
                                ELSE DestinatarioAlunoId
                            END
                        ) PERSISTED,

                        AlunoMaiorId AS
                        (
                            CASE
                                WHEN SolicitanteAlunoId <
                                     DestinatarioAlunoId
                                    THEN DestinatarioAlunoId
                                ELSE SolicitanteAlunoId
                            END
                        ) PERSISTED,

                        Estado nvarchar(20)
                            NOT NULL,

                        CriadoEm datetime2
                            NOT NULL
                            CONSTRAINT DF_AmizadeAluno_CriadoEm
                            DEFAULT SYSDATETIME(),

                        RespondidoEm datetime2
                            NULL,

                        CONSTRAINT CK_AmizadeAluno_Alunos
                            CHECK
                            (
                                SolicitanteAlunoId <>
                                DestinatarioAlunoId
                            )
                    );

                    CREATE UNIQUE INDEX
                        UX_AmizadeAluno_Par

                    ON dbo.AmizadeAluno
                    (
                        AlunoMenorId,
                        AlunoMaiorId
                    );

                END;

                IF OBJECT_ID(
                    'dbo.GrupoAluno',
                    'U'
                ) IS NULL
                BEGIN

                    CREATE TABLE dbo.GrupoAluno
                    (
                        Id int IDENTITY(1,1)
                            NOT NULL
                            PRIMARY KEY,

                        Nome nvarchar(120)
                            NOT NULL,

                        CriadorAlunoId int
                            NOT NULL,

                        CriadoEm datetime2
                            NOT NULL
                            CONSTRAINT DF_GrupoAluno_CriadoEm
                            DEFAULT SYSDATETIME()
                    );

                END;

                IF OBJECT_ID(
                    'dbo.GrupoAlunoMembro',
                    'U'
                ) IS NULL
                BEGIN

                    CREATE TABLE dbo.GrupoAlunoMembro
                    (
                        GrupoId int
                            NOT NULL,

                        AlunoId int
                            NOT NULL,

                        Estado nvarchar(20)
                            NOT NULL,

                        CriadoEm datetime2
                            NOT NULL
                            CONSTRAINT DF_GrupoAlunoMembro_CriadoEm
                            DEFAULT SYSDATETIME(),

                        RespondidoEm datetime2
                            NULL,

                        CONSTRAINT PK_GrupoAlunoMembro
                            PRIMARY KEY
                            (
                                GrupoId,
                                AlunoId
                            )
                    );

                END;

                IF OBJECT_ID(
                    'dbo.MensagemAlunoPrivada',
                    'U'
                ) IS NULL
                BEGIN

                    CREATE TABLE dbo.MensagemAlunoPrivada
                    (
                        Id int IDENTITY(1,1)
                            NOT NULL
                            PRIMARY KEY,

                        DeAlunoId int
                            NOT NULL,

                        ParaAlunoId int
                            NOT NULL,

                        Texto nvarchar(2000)
                            NOT NULL,

                        CriadoEm datetime2
                            NOT NULL
                            CONSTRAINT DF_MensagemAlunoPrivada_CriadoEm
                            DEFAULT SYSDATETIME()
                    );

                END;

                IF OBJECT_ID(
                    'dbo.MensagemAlunoGrupo',
                    'U'
                ) IS NULL
                BEGIN

                    CREATE TABLE dbo.MensagemAlunoGrupo
                    (
                        Id int IDENTITY(1,1)
                            NOT NULL
                            PRIMARY KEY,

                        GrupoId int
                            NOT NULL,

                        DeAlunoId int
                            NOT NULL,

                        Texto nvarchar(2000)
                            NOT NULL,

                        CriadoEm datetime2
                            NOT NULL
                            CONSTRAINT DF_MensagemAlunoGrupo_CriadoEm
                            DEFAULT SYSDATETIME()
                    );

                END;

                IF OBJECT_ID(
                    'dbo.ConversaAlunoLeitura',
                    'U'
                ) IS NULL
                BEGIN

                    CREATE TABLE dbo.ConversaAlunoLeitura
                    (
                        AlunoId int
                            NOT NULL,

                        TipoConversa nvarchar(20)
                            NOT NULL,

                        ConversaId int
                            NOT NULL,

                        LidaEm datetime2
                            NOT NULL,

                        CONSTRAINT PK_ConversaAlunoLeitura
                            PRIMARY KEY
                            (
                                AlunoId,
                                TipoConversa,
                                ConversaId
                            )
                    );

                END;";

            Executar(sql);
        }

        #endregion

        #region Acesso à base de dados

        private void BindGrid(
            GridView grid,
            string sql,
            params SqlParameter[] parametros)
        {
            grid.DataSource =
                ObterTabela(
                    sql,
                    parametros
                );

            grid.DataBind();
        }

        private DataTable ObterTabela(
            string sql,
            params SqlParameter[] parametros)
        {
            DataTable dt =
                new DataTable();

            using (SqlConnection c =
                new SqlConnection(_connectionString))
            using (SqlCommand cmd =
                new SqlCommand(sql, c))
            using (SqlDataAdapter da =
                new SqlDataAdapter(cmd))
            {
                if (parametros != null)
                {
                    cmd.Parameters.AddRange(
                        parametros
                    );
                }

                da.Fill(dt);
            }

            return dt;
        }

        private object Escalar(
            string sql,
            params SqlParameter[] parametros)
        {
            using (SqlConnection c =
                new SqlConnection(_connectionString))
            using (SqlCommand cmd =
                new SqlCommand(sql, c))
            {
                if (parametros != null)
                {
                    cmd.Parameters.AddRange(
                        parametros
                    );
                }

                c.Open();

                return cmd.ExecuteScalar();
            }
        }

        private void Executar(
            string sql,
            params SqlParameter[] parametros)
        {
            using (SqlConnection c =
                new SqlConnection(_connectionString))
            using (SqlCommand cmd =
                new SqlCommand(sql, c))
            {
                if (parametros != null)
                {
                    cmd.Parameters.AddRange(
                        parametros
                    );
                }

                c.Open();

                cmd.ExecuteNonQuery();
            }
        }

        private SqlParameter Param(
            string nome,
            object valor)
        {
            return new SqlParameter(
                nome,
                valor ?? DBNull.Value
            );
        }

        #endregion

        #region Mensagens da interface

        private void MostrarMensagem(
            string texto,
            bool erro)
        {
            LblMensagem.Text =
                texto;

            LblMensagem.CssClass =
                erro
                    ? "alert alert-warning d-block"
                    : "alert alert-success d-block";

            LblMensagem.Visible =
                true;
        }

        #endregion

        private bool ConversaEscolarPermitida(
    int conversaId,
    out string nomeProfessor)
        {
            nomeProfessor =
                string.Empty;

            const string sql = @"
        SELECT TOP 1
            professor.Nome

        FROM dbo.ConversaDireta conversa

        INNER JOIN dbo.Professor professor
            ON professor.UserId =
                CASE
                    WHEN conversa.Utilizador1Id =
                         @UserIdAtual
                        THEN conversa.Utilizador2Id
                    ELSE conversa.Utilizador1Id
                END

        WHERE conversa.Id =
              @ConversaId

          AND conversa.Ativa = 1

          AND
          (
              conversa.Utilizador1Id =
                  @UserIdAtual

              OR conversa.Utilizador2Id =
                  @UserIdAtual
          )

          AND professor.Ativo = 1

          AND EXISTS
          (
              SELECT 1

              FROM dbo.AlunoTurma alunoTurma

              INNER JOIN dbo.TurmaDisciplina turmaDisciplina
                  ON turmaDisciplina.TurmaId =
                     alunoTurma.TurmaId

              INNER JOIN dbo.TurmaDisciplinaProfessor atribuicao
                  ON atribuicao.TurmaDisciplinaId =
                     turmaDisciplina.Id

              WHERE alunoTurma.AlunoId =
                    @AlunoId

                AND atribuicao.ProfessorId =
                    professor.Id

                AND alunoTurma.Ate IS NULL
                AND atribuicao.Ate IS NULL
          );";

            object resultado =
                Escalar(
                    sql,
                    Param("@ConversaId", conversaId),
                    Param("@UserIdAtual", UserIdAtual),
                    Param("@AlunoId", AlunoId)
                );

            if (resultado == null ||
                resultado == DBNull.Value)
            {
                return false;
            }

            nomeProfessor =
                resultado.ToString().Trim();

            return true;
        }


        private void MarcarConversaEscolarComoLida(
            int conversaId)
        {
            const string sql = @"
        UPDATE dbo.MensagemDireta

        SET LidaEm = SYSDATETIME()

        WHERE ConversaId = @ConversaId

          AND RemetenteUserId <>
              @UserIdAtual

          AND LidaEm IS NULL;";

            Executar(
                sql,
                Param("@ConversaId", conversaId),
                Param("@UserIdAtual", UserIdAtual)
            );
        }
    }
}
