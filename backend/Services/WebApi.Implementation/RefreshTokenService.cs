using Microsoft.Extensions.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Security.Cryptography;
using WebApi.Interface;

namespace WebApi.Implementation
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly string _connectionString;

        public RefreshTokenService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection")
                ?? throw new InvalidOperationException("La cadena de conexión no puede ser nula.");
        }

        private static byte[] Hash(string plainTextToken) => SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(plainTextToken));

        public async Task<string> CreateAsync(int idUsuario, TimeSpan lifetime)
        {
            var plainTextToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
            var tokenHash = Hash(plainTextToken);

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_RefreshToken_Crear", connection) { CommandType = CommandType.StoredProcedure };
            command.Parameters.AddWithValue("@IdUsuario", idUsuario);
            command.Parameters.AddWithValue("@TokenHash", tokenHash);
            command.Parameters.AddWithValue("@FechaExpiracion", DateTime.UtcNow.Add(lifetime));
            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();

            return plainTextToken;
        }

        public async Task<int?> ValidateAsync(string plainTextToken)
        {
            var tokenHash = Hash(plainTextToken);
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_RefreshToken_Validar", connection) { CommandType = CommandType.StoredProcedure };
            command.Parameters.AddWithValue("@TokenHash", tokenHash);
            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            return result == null ? null : Convert.ToInt32(result);
        }

        public async Task RevokeAsync(string plainTextToken)
        {
            var tokenHash = Hash(plainTextToken);
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_RefreshToken_Revocar", connection) { CommandType = CommandType.StoredProcedure };
            command.Parameters.AddWithValue("@TokenHash", tokenHash);
            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task RevokeAllForUserAsync(int idUsuario)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_RefreshToken_RevocarTodosDeUsuario", connection) { CommandType = CommandType.StoredProcedure };
            command.Parameters.AddWithValue("@IdUsuario", idUsuario);
            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }
    }
}
