using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using WebApi.Interface;
using WebApi.Models;

namespace WebApi.Implementation
{
    public class UsuarioRolService : IUsuarioRolService
    {
        private readonly string _connectionString;

        public UsuarioRolService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection")
                ?? throw new InvalidOperationException("La cadena de conexión no puede ser nula.");
        }

        public async Task<IEnumerable<UsuarioRol>> GetAllAsync()
        {
            var lista = new List<UsuarioRol>();

            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT IdUsuarioRol, IdUsuario, IdRol, FechaAsignacion, Activo
                    FROM UsuarioRol
                    ORDER BY IdUsuarioRol DESC";

                using (var command = new SqlCommand(query, connection))
                {
                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            lista.Add(
                    new UsuarioRol
                    {
                        IdUsuarioRol = reader.GetInt32(reader.GetOrdinal("IdUsuarioRol")),
                        IdUsuario = reader.GetInt32(reader.GetOrdinal("IdUsuario")),
                        IdRol = reader.GetInt32(reader.GetOrdinal("IdRol")),
                        FechaAsignacion = reader.IsDBNull(reader.GetOrdinal("FechaAsignacion")) ? null : reader.GetDateTime(reader.GetOrdinal("FechaAsignacion")),
                        Activo = reader.IsDBNull(reader.GetOrdinal("Activo")) ? null : reader.GetBoolean(reader.GetOrdinal("Activo")),
                    }
                            );
                        }
                    }
                }
            }

            return lista;
        }

        public async Task<UsuarioRol?> GetByIdAsync(int id)
        {
            UsuarioRol? resultado = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT IdUsuarioRol, IdUsuario, IdRol, FechaAsignacion, Activo
                    FROM UsuarioRol
                    WHERE IdUsuarioRol = @IdUsuarioRol";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdUsuarioRol", id);

                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            resultado =
                    new UsuarioRol
                    {
                        IdUsuarioRol = reader.GetInt32(reader.GetOrdinal("IdUsuarioRol")),
                        IdUsuario = reader.GetInt32(reader.GetOrdinal("IdUsuario")),
                        IdRol = reader.GetInt32(reader.GetOrdinal("IdRol")),
                        FechaAsignacion = reader.IsDBNull(reader.GetOrdinal("FechaAsignacion")) ? null : reader.GetDateTime(reader.GetOrdinal("FechaAsignacion")),
                        Activo = reader.IsDBNull(reader.GetOrdinal("Activo")) ? null : reader.GetBoolean(reader.GetOrdinal("Activo")),
                    };
                        }
                    }
                }
            }

            return resultado;
        }

        public async Task<UsuarioRol> CreateAsync(UsuarioRol entity)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    INSERT INTO UsuarioRol (IdUsuario, IdRol, FechaAsignacion, Activo)
                    OUTPUT INSERTED.IdUsuarioRol
                    VALUES (@IdUsuario, @IdRol, @FechaAsignacion, @Activo)";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdUsuario", (object?)entity.IdUsuario ?? DBNull.Value);
                    command.Parameters.AddWithValue("@IdRol", (object?)entity.IdRol ?? DBNull.Value);
                    command.Parameters.AddWithValue("@FechaAsignacion", (object?)entity.FechaAsignacion ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Activo", (object?)entity.Activo ?? DBNull.Value);

                    await connection.OpenAsync();
                    entity.IdUsuarioRol = Convert.ToInt32(await command.ExecuteScalarAsync());
                }
            }

            return entity;
        }

        public async Task<bool> UpdateAsync(int id, UsuarioRol entity)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    UPDATE UsuarioRol
                    SET IdUsuario = @IdUsuario,
                        IdRol = @IdRol,
                        FechaAsignacion = @FechaAsignacion,
                        Activo = @Activo
                    WHERE IdUsuarioRol = @IdUsuarioRol";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdUsuarioRol", id);
                    command.Parameters.AddWithValue("@IdUsuario", (object?)entity.IdUsuario ?? DBNull.Value);
                    command.Parameters.AddWithValue("@IdRol", (object?)entity.IdRol ?? DBNull.Value);
                    command.Parameters.AddWithValue("@FechaAsignacion", (object?)entity.FechaAsignacion ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Activo", (object?)entity.Activo ?? DBNull.Value);

                    await connection.OpenAsync();
                    var rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    UPDATE UsuarioRol
                    SET Activo = 0
                    WHERE IdUsuarioRol = @IdUsuarioRol";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdUsuarioRol", id);

                    await connection.OpenAsync();
                    var rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
        }

        public async Task<IEnumerable<string>> GetRoleNamesForUserAsync(int idUsuario)
        {
            var roles = new List<string>();
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("sp_UsuarioRol_ObtenerRolesDeUsuario", connection) { CommandType = System.Data.CommandType.StoredProcedure })
                {
                    command.Parameters.AddWithValue("@IdUsuario", idUsuario);
                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            if (!reader.IsDBNull(0))
                            {
                                roles.Add(reader.GetString(0));
                            }
                        }
                    }
                }
            }
            return roles;
        }
    }
}
