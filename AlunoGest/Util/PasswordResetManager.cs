using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Web.Security;

namespace AlunoGest.Util
{
    public static class PasswordResetManager
    {
        #region Campos

        private const int DuracaoTokenMinutos = 30;

        private static readonly string _connectionString =
            ConfigurationManager
                .ConnectionStrings["DefaultConnection"]
                .ConnectionString;

        #endregion


        #region Gerar token

        public static string GenerateAndStoreToken(
            string username,
            string email,
            string requestedIp)
        {
            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(email))
            {
                return null;
            }

            username =
                username.Trim();

            email =
                email.Trim();

            /*
             * Procura diretamente pelo username.
             * Isto evita a ambiguidade causada por emails repetidos.
             */
            MembershipUser user =
                Membership.GetUser(
                    username,
                    false
                );

            if (user == null ||
                user.ProviderUserKey == null)
            {
                return null;
            }

            /*
             * Confirma se o email introduzido corresponde
             * ao email guardado na conta encontrada.
             */
            if (string.IsNullOrWhiteSpace(user.Email) ||
                !string.Equals(
                    user.Email.Trim(),
                    email,
                    StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            Guid userId =
                (Guid)user.ProviderUserKey;

            string token =
                GerarTokenSeguro();

            string tokenHash =
                CalcularHashToken(token);

            DateTime expirationUtc =
                DateTime.UtcNow.AddMinutes(
                    DuracaoTokenMinutos
                );

            using (SqlConnection conn =
                new SqlConnection(_connectionString))
            {
                conn.Open();

                using (SqlTransaction transaction =
                    conn.BeginTransaction())
                {
                    try
                    {
                        InvalidarTokensAnteriores(
                            conn,
                            transaction,
                            userId
                        );

                        InserirToken(
                            conn,
                            transaction,
                            tokenHash,
                            userId,
                            expirationUtc,
                            requestedIp
                        );

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }

            return token;
        }

        private static string GerarTokenSeguro()
        {
            byte[] bytes =
                new byte[32];

            using (RandomNumberGenerator generator =
                RandomNumberGenerator.Create())
            {
                generator.GetBytes(bytes);
            }

            return Convert
                .ToBase64String(bytes)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
        }


        private static void InserirToken(
            SqlConnection conn,
            SqlTransaction transaction,
            string tokenHash,
            Guid userId,
            DateTime expirationUtc,
            string requestedIp)
        {
            const string sql = @"
                INSERT INTO dbo.PasswordResetTokens
                (
                    TokenHash,
                    UserId,
                    ExpirationUtc,
                    RequestedIp
                )
                VALUES
                (
                    @TokenHash,
                    @UserId,
                    @ExpirationUtc,
                    @RequestedIp
                );";

            using (SqlCommand cmd =
                new SqlCommand(
                    sql,
                    conn,
                    transaction
                ))
            {
                cmd.Parameters
                    .Add(
                        "@TokenHash",
                        SqlDbType.Char,
                        64
                    )
                    .Value = tokenHash;

                cmd.Parameters
                    .Add(
                        "@UserId",
                        SqlDbType.UniqueIdentifier
                    )
                    .Value = userId;

                cmd.Parameters
                    .Add(
                        "@ExpirationUtc",
                        SqlDbType.DateTime2
                    )
                    .Value = expirationUtc;

                cmd.Parameters
                    .Add(
                        "@RequestedIp",
                        SqlDbType.NVarChar,
                        45
                    )
                    .Value =
                    string.IsNullOrWhiteSpace(
                        requestedIp
                    )
                        ? (object)DBNull.Value
                        : requestedIp;

                cmd.ExecuteNonQuery();
            }
        }


        private static void InvalidarTokensAnteriores(
            SqlConnection conn,
            SqlTransaction transaction,
            Guid userId)
        {
            const string sql = @"
                UPDATE dbo.PasswordResetTokens

                SET UsedAtUtc = SYSUTCDATETIME()

                WHERE UserId = @UserId
                  AND UsedAtUtc IS NULL;


                DELETE FROM dbo.PasswordResetTokens

                WHERE ExpirationUtc <
                      DATEADD(
                          DAY,
                          -1,
                          SYSUTCDATETIME()
                      );";

            using (SqlCommand cmd =
                new SqlCommand(
                    sql,
                    conn,
                    transaction
                ))
            {
                cmd.Parameters
                    .Add(
                        "@UserId",
                        SqlDbType.UniqueIdentifier
                    )
                    .Value = userId;

                cmd.ExecuteNonQuery();
            }
        }

        #endregion


        #region Validar token

        public static string GetUsernameByToken(
            string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return null;
            }

            string tokenHash =
                CalcularHashToken(token);

            const string sql = @"
                SELECT TOP 1
                    u.UserName

                FROM dbo.PasswordResetTokens t

                INNER JOIN dbo.Users u
                    ON u.UserId = t.UserId

                WHERE t.TokenHash = @TokenHash
                  AND t.UsedAtUtc IS NULL
                  AND t.ExpirationUtc > SYSUTCDATETIME();";

            using (SqlConnection conn =
                new SqlConnection(_connectionString))
            using (SqlCommand cmd =
                new SqlCommand(sql, conn))
            {
                cmd.Parameters
                    .Add(
                        "@TokenHash",
                        SqlDbType.Char,
                        64
                    )
                    .Value = tokenHash;

                conn.Open();

                object resultado =
                    cmd.ExecuteScalar();

                if (resultado == null ||
                    resultado == DBNull.Value)
                {
                    return null;
                }

                return resultado.ToString();
            }
        }

        #endregion


        #region Invalidar token

        public static void InvalidateToken(
            string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return;
            }

            string tokenHash =
                CalcularHashToken(token);

            const string sql = @"
                UPDATE dbo.PasswordResetTokens

                SET UsedAtUtc = SYSUTCDATETIME()

                WHERE TokenHash = @TokenHash
                  AND UsedAtUtc IS NULL;";

            using (SqlConnection conn =
                new SqlConnection(_connectionString))
            using (SqlCommand cmd =
                new SqlCommand(sql, conn))
            {
                cmd.Parameters
                    .Add(
                        "@TokenHash",
                        SqlDbType.Char,
                        64
                    )
                    .Value = tokenHash;

                conn.Open();

                cmd.ExecuteNonQuery();
            }
        }

        #endregion


        #region Hash

        private static string CalcularHashToken(
            string token)
        {
            byte[] tokenBytes =
                Encoding.UTF8.GetBytes(token);

            byte[] hashBytes;

            using (SHA256 sha256 =
                SHA256.Create())
            {
                hashBytes =
                    sha256.ComputeHash(
                        tokenBytes
                    );
            }

            StringBuilder resultado =
                new StringBuilder(
                    hashBytes.Length * 2
                );

            foreach (byte valor in hashBytes)
            {
                resultado.Append(
                    valor.ToString("x2")
                );
            }

            return resultado.ToString();
        }

        #endregion
    }
}