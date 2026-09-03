using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using WebApi.Interface;
using WebApi.Models;

namespace WebApi.Implementation
{
    public class RolService : IRolService
    {
        private readonly string _connectionString;

        public RolService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection")
                ?? throw new InvalidOperationException("La cadena de conexión no puede ser nula.");
        }

        public async Task<IEnumerable<Rol>> GetAllAsync()
        {
            var lista = new List<Rol>();

            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT IdRol, NombreRol, Descripcion, Activo
                    FROM Roles
                    ORDER BY IdRol DESC";

                using (var command = new SqlCommand(query, connection))
                {
                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            lista.Add(
                    new Rol
                    {
                        IdRol = reader.GetInt32(reader.GetOrdinal("IdRol")),
                        NombreRol = reader.IsDBNull(reader.GetOrdinal("NombreRol")) ? null : reader.GetString(reader.GetOrdinal("NombreRol")),
                        Descripcion = reader.IsDBNull(reader.GetOrdinal("Descripcion")) ? null : reader.GetString(reader.GetOrdinal("Descripcion")),
                        Activo = reader.IsDBNull(reader.GetOrdinal("Activo")) ? null : reader.GetBoolean(reader.GetOrdinal("Activo")),
                    }
                            );
                        }
                    }
                }
            }

            return lista;
        }

        public async Task<Rol?> GetByIdAsync(int id)
        {
            Rol? resultado = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT IdRol, NombreRol, Descripcion, Activo
                    FROM Roles
                    WHERE IdRol = @IdRol";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdRol", id);

                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            resultado =
                    new Rol
                    {
                        IdRol = reader.GetInt32(reader.GetOrdinal("IdRol")),
                        NombreRol = reader.IsDBNull(reader.GetOrdinal("NombreRol")) ? null : reader.GetString(reader.GetOrdinal("NombreRol")),
                        Descripcion = reader.IsDBNull(reader.GetOrdinal("Descripcion")) ? null : reader.GetString(reader.GetOrdinal("Descripcion")),
                        Activo = reader.IsDBNull(reader.GetOrdinal("Activo")) ? null : reader.GetBoolean(reader.GetOrdinal("Activo")),
                    };
                        }
                    }
                }
            }

            return resultado;
        }

        public async Task<Rol> CreateAsync(Rol entity)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    INSERT INTO Roles (NombreRol, Descripcion, Activo)
                    OUTPUT INSERTED.IdRol
                    VALUES (@NombreRol, @Descripcion, @Activo)";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@NombreRol", (object?)entity.NombreRol ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Descripcion", (object?)entity.Descripcion ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Activo", (object?)entity.Activo ?? DBNull.Value);

                    await connection.OpenAsync();
                    entity.IdRol = Convert.ToInt32(await command.ExecuteScalarAsync());
                }
            }

            return entity;
        }

        public async Task<bool> UpdateAsync(int id, Rol entity)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    UPDATE Roles
                    SET NombreRol = @NombreRol,
                        Descripcion = @Descripcion,
                        Activo = @Activo
                    WHERE IdRol = @IdRol";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdRol", id);
                    command.Parameters.AddWithValue("@NombreRol", (object?)entity.NombreRol ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Descripcion", (object?)entity.Descripcion ?? DBNull.Value);
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
                // Baja logica: la tabla maneja un indicador de estado
                var query = @"
                    UPDATE Roles
                    SET Activo = 0
                    WHERE IdRol = @IdRol";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdRol", id);

                    await connection.OpenAsync();
                    var rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
        }
    }
}
