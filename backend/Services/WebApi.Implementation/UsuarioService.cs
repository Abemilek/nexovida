using Microsoft.Extensions.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Security.Cryptography;
using WebApi.Interface;
using WebApi.Models;

namespace WebApi.Implementation
{
    public class UsuarioService : IUsuarioService
    {
        private readonly string _connectionString;

        private const int Pbkdf2Iterations = 600_000;
        private const int SaltSizeBytes = 16;
        private const int KeySizeBytes = 32;

        public UsuarioService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection")
                ?? throw new InvalidOperationException("La cadena de conexión no puede ser nula.");
        }

        private static (byte[] hash, byte[] salt) HashPassword(string plainTextPassword)
        {
            var salt = RandomNumberGenerator.GetBytes(SaltSizeBytes);
            var hash = Rfc2898DeriveBytes.Pbkdf2(
                password: plainTextPassword,
                salt: salt,
                iterations: Pbkdf2Iterations,
                hashAlgorithm: HashAlgorithmName.SHA256,
                outputLength: KeySizeBytes
            );
            return (hash, salt);
        }

        public bool VerifyPassword(string plainTextPassword, byte[] hash, byte[] salt)
        {
            var computedHash = Rfc2898DeriveBytes.Pbkdf2(
                password: plainTextPassword,
                salt: salt,
                iterations: Pbkdf2Iterations,
                hashAlgorithm: HashAlgorithmName.SHA256,
                outputLength: KeySizeBytes
            );
            return CryptographicOperations.FixedTimeEquals(computedHash, hash);
        }

        private static Usuario Map(SqlDataReader reader) => new Usuario
        {
            IdUsuario = reader.GetInt32(reader.GetOrdinal("IdUsuario")),
            NombreUsuario = reader.IsDBNull(reader.GetOrdinal("NombreUsuario")) ? null : reader.GetString(reader.GetOrdinal("NombreUsuario")),
            Correo = reader.IsDBNull(reader.GetOrdinal("Correo")) ? null : reader.GetString(reader.GetOrdinal("Correo")),
            Contrasena = reader.IsDBNull(reader.GetOrdinal("Contrasena")) ? null : (byte[])reader.GetValue(reader.GetOrdinal("Contrasena")),
            Salt = reader.IsDBNull(reader.GetOrdinal("Salt")) ? null : (byte[])reader.GetValue(reader.GetOrdinal("Salt")),
            FechaRegistro = reader.IsDBNull(reader.GetOrdinal("FechaRegistro")) ? null : reader.GetDateTime(reader.GetOrdinal("FechaRegistro")),
            UltimoAcceso = reader.IsDBNull(reader.GetOrdinal("UltimoAcceso")) ? null : reader.GetDateTime(reader.GetOrdinal("UltimoAcceso")),
            Activo = reader.IsDBNull(reader.GetOrdinal("Activo")) ? null : reader.GetBoolean(reader.GetOrdinal("Activo")),
            TwoFactorEnabled = reader.IsDBNull(reader.GetOrdinal("TwoFactorEnabled")) ? false : reader.GetBoolean(reader.GetOrdinal("TwoFactorEnabled")),
            TwoFactorSecret = reader.IsDBNull(reader.GetOrdinal("TwoFactorSecret")) ? null : reader.GetString(reader.GetOrdinal("TwoFactorSecret")),
        };

        public async Task<IEnumerable<Usuario>> GetAllAsync()
        {
            var lista = new List<Usuario>();
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_Usuario_ObtenerTodos", connection) { CommandType = CommandType.StoredProcedure };
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                lista.Add(Map(reader));
            }
            return lista;
        }

        public async Task<Usuario?> GetByIdAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_Usuario_ObtenerPorId", connection) { CommandType = CommandType.StoredProcedure };
            command.Parameters.AddWithValue("@IdUsuario", id);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            return await reader.ReadAsync() ? Map(reader) : null;
        }

        public async Task<Usuario?> GetByCorreoAsync(string correo)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_Usuario_ObtenerPorCorreo", connection) { CommandType = CommandType.StoredProcedure };
            command.Parameters.AddWithValue("@Correo", correo);
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            return await reader.ReadAsync() ? Map(reader) : null;
        }

        public async Task<Usuario> CreateAsync(Usuario entity)
        {
            if (string.IsNullOrWhiteSpace(entity.Password))
            {
                throw new ArgumentException("El campo Password es obligatorio para crear un usuario.");
            }

            var (hash, salt) = HashPassword(entity.Password);
            entity.Contrasena = hash;
            entity.Salt = salt;
            entity.Password = null;
            entity.TwoFactorEnabled = false;
            entity.TwoFactorSecret = null;

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_Usuario_Crear", connection) { CommandType = CommandType.StoredProcedure };
            command.Parameters.AddWithValue("@NombreUsuario", (object?)entity.NombreUsuario ?? DBNull.Value);
            command.Parameters.AddWithValue("@Correo", (object?)entity.Correo ?? DBNull.Value);
            command.Parameters.AddWithValue("@Contrasena", entity.Contrasena);
            command.Parameters.AddWithValue("@Salt", entity.Salt);
            command.Parameters.AddWithValue("@FechaRegistro", (object?)entity.FechaRegistro ?? DBNull.Value);
            command.Parameters.AddWithValue("@UltimoAcceso", (object?)entity.UltimoAcceso ?? DBNull.Value);
            command.Parameters.AddWithValue("@Activo", (object?)entity.Activo ?? DBNull.Value);

            await connection.OpenAsync();
            entity.IdUsuario = Convert.ToInt32(await command.ExecuteScalarAsync());
            return entity;
        }

        public async Task<bool> UpdateAsync(int id, Usuario entity)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            if (!string.IsNullOrWhiteSpace(entity.Password))
            {
                var (hash, salt) = HashPassword(entity.Password);
                entity.Contrasena = hash;
                entity.Salt = salt;
                entity.Password = null;
            }
            else
            {
                using var credCommand = new SqlCommand("sp_Usuario_ObtenerCredenciales", connection) { CommandType = CommandType.StoredProcedure };
                credCommand.Parameters.AddWithValue("@IdUsuario", id);
                using var reader = await credCommand.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    entity.Contrasena = reader.IsDBNull(0) ? null : (byte[])reader.GetValue(0);
                    entity.Salt = reader.IsDBNull(1) ? null : (byte[])reader.GetValue(1);
                }
            }

            using var command = new SqlCommand("sp_Usuario_Actualizar", connection) { CommandType = CommandType.StoredProcedure };
            command.Parameters.AddWithValue("@IdUsuario", id);
            command.Parameters.AddWithValue("@NombreUsuario", (object?)entity.NombreUsuario ?? DBNull.Value);
            command.Parameters.AddWithValue("@Correo", (object?)entity.Correo ?? DBNull.Value);
            command.Parameters.AddWithValue("@Contrasena", (object?)entity.Contrasena ?? DBNull.Value);
            command.Parameters.AddWithValue("@Salt", (object?)entity.Salt ?? DBNull.Value);
            command.Parameters.AddWithValue("@FechaRegistro", (object?)entity.FechaRegistro ?? DBNull.Value);
            command.Parameters.AddWithValue("@UltimoAcceso", (object?)entity.UltimoAcceso ?? DBNull.Value);
            command.Parameters.AddWithValue("@Activo", (object?)entity.Activo ?? DBNull.Value);

            var filasAfectadas = Convert.ToInt32(await command.ExecuteScalarAsync());
            return filasAfectadas > 0;
        }

        public async Task<bool> SetTwoFactorAsync(int id, bool enabled, string? secret)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_UsuarioSeguridad_Establecer", connection) { CommandType = CommandType.StoredProcedure };
            command.Parameters.AddWithValue("@IdUsuario", id);
            command.Parameters.AddWithValue("@TwoFactorEnabled", enabled);
            command.Parameters.AddWithValue("@TwoFactorSecret", (object?)secret ?? DBNull.Value);
            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
            return true;
        }

        public async Task UpdateUltimoAccesoAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_Usuario_ActualizarUltimoAcceso", connection) { CommandType = CommandType.StoredProcedure };
            command.Parameters.AddWithValue("@IdUsuario", id);
            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_Usuario_Desactivar", connection) { CommandType = CommandType.StoredProcedure };
            command.Parameters.AddWithValue("@IdUsuario", id);
            await connection.OpenAsync();
            var filasAfectadas = Convert.ToInt32(await command.ExecuteScalarAsync());
            return filasAfectadas > 0;
        }
    }
}
