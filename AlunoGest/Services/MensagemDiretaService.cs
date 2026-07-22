using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace AlunoGest.Services
{
    public static class MensagemDiretaService
    {
        private static string ObterConnectionString()
        {
            ConnectionStringSettings configuracao =
                ConfigurationManager
                    .ConnectionStrings["DefaultConnection"];

            if (configuracao == null ||
                string.IsNullOrWhiteSpace(
                    configuracao.ConnectionString))
            {
                throw new InvalidOperationException(
                    "A ligação DefaultConnection não foi encontrada."
                );
            }

            return configuracao.ConnectionString;
        }


        public static int ContarMensagensNaoLidas(
            Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return 0;
            }

            const string sql = @"
                SELECT COUNT(1)

                FROM dbo.MensagemDireta mensagem

                INNER JOIN dbo.ConversaDireta conversa
                    ON conversa.Id =
                       mensagem.ConversaId

                WHERE conversa.Ativa = 1

                  AND
                  (
                      conversa.Utilizador1Id =
                          @UserId

                      OR conversa.Utilizador2Id =
                          @UserId
                  )

                  AND mensagem.RemetenteUserId <>
                      @UserId

                  AND mensagem.LidaEm IS NULL;";

            using (SqlConnection conn =
                new SqlConnection(
                    ObterConnectionString()))
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

                return Convert.ToInt32(
                    cmd.ExecuteScalar()
                );
            }
        }


        public static int ContarMensagensNaoLidasAluno(
            Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return 0;
            }

            const string sql = @"
                DECLARE @AlunoId INT;

                SELECT TOP 1
                    @AlunoId = Id

                FROM dbo.Aluno

                WHERE UserId = @UserId
                  AND Ativo = 1;


                IF @AlunoId IS NULL
                BEGIN

                    SELECT 0;
                    RETURN;

                END;


                SELECT

                    /* Mensagens enviadas por professores */

                    (
                        SELECT COUNT(1)

                        FROM dbo.MensagemDireta mensagem

                        INNER JOIN dbo.ConversaDireta conversa
                            ON conversa.Id =
                               mensagem.ConversaId

                        WHERE conversa.Ativa = 1

                          AND
                          (
                              conversa.Utilizador1Id =
                                  @UserId

                              OR conversa.Utilizador2Id =
                                  @UserId
                          )

                          AND mensagem.RemetenteUserId <>
                              @UserId

                          AND mensagem.LidaEm IS NULL
                    )

                    +

                    /* Mensagens privadas dos amigos */

                    (
                        SELECT COUNT(1)

                        FROM dbo.MensagemAlunoPrivada mensagem

                        LEFT JOIN dbo.ConversaAlunoLeitura leitura
                            ON leitura.AlunoId =
                               @AlunoId

                           AND leitura.TipoConversa =
                               N'amigo'

                           AND leitura.ConversaId =
                               mensagem.DeAlunoId

                        WHERE mensagem.ParaAlunoId =
                              @AlunoId

                          AND
                          (
                              leitura.LidaEm IS NULL

                              OR mensagem.CriadoEm >
                                 leitura.LidaEm
                          )
                    )

                    +

                    /* Mensagens recebidas em grupos */

                    (
                        SELECT COUNT(1)

                        FROM dbo.MensagemAlunoGrupo mensagem

                        INNER JOIN dbo.GrupoAlunoMembro membro
                            ON membro.GrupoId =
                               mensagem.GrupoId

                           AND membro.AlunoId =
                               @AlunoId

                           AND membro.Estado =
                               N'Aceite'

                        LEFT JOIN dbo.ConversaAlunoLeitura leitura
                            ON leitura.AlunoId =
                               @AlunoId

                           AND leitura.TipoConversa =
                               N'grupo'

                           AND leitura.ConversaId =
                               mensagem.GrupoId

                        WHERE mensagem.DeAlunoId <>
                              @AlunoId

                          AND
                          (
                              leitura.LidaEm IS NULL

                              OR mensagem.CriadoEm >
                                 leitura.LidaEm
                          )
                    );";

            using (SqlConnection conn =
                new SqlConnection(
                    ObterConnectionString()))
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

                return Convert.ToInt32(
                    cmd.ExecuteScalar()
                );
            }
        }


        public static string FormatarQuantidade(
            int quantidade)
        {
            if (quantidade <= 0)
            {
                return "0";
            }

            return quantidade > 99
                ? "99+"
                : quantidade.ToString();
        }


        public static string FormatarToolTip(
            int quantidade)
        {
            if (quantidade <= 0)
            {
                return "Mensagens";
            }

            if (quantidade == 1)
            {
                return "1 mensagem não lida";
            }

            return quantidade +
                   " mensagens não lidas";
        }
    }
}