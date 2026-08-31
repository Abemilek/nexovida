using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using WebApi.Interface;
using WebApi.Models;

namespace WebApi.Implementation
{
    public class HistorialPacienteService : IHistorialPacienteService
    {
        private readonly string _connectionString;

        public HistorialPacienteService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection")
                ?? throw new InvalidOperationException("La cadena de conexión no puede ser nula.");
        }

        public async Task<IEnumerable<HistorialPaciente>> GetAllAsync()
        {
            var lista = new List<HistorialPaciente>();

            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT IdHistorialPaciente, IdPaciente, IdUsuario, TipoEvento, FechaEvento, Titulo, Descripcion
                    FROM HistorialPaciente
                    ORDER BY IdHistorialPaciente DESC";

                using (var command = new SqlCommand(query, connection))
                {
                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            lista.Add(
                    new HistorialPaciente
                    {
                        IdHistorialPaciente = reader.GetInt64(reader.GetOrdinal("IdHistorialPaciente")),
                        IdPaciente = reader.GetInt32(reader.GetOrdinal("IdPaciente")),
                        IdUsuario = reader.IsDBNull(reader.GetOrdinal("IdUsuario")) ? null : reader.GetInt32(reader.GetOrdinal("IdUsuario")),
                        TipoEvento = reader.IsDBNull(reader.GetOrdinal("TipoEvento")) ? null : reader.GetString(reader.GetOrdinal("TipoEvento")),
                        FechaEvento = reader.IsDBNull(reader.GetOrdinal("FechaEvento")) ? null : reader.GetDateTime(reader.GetOrdinal("FechaEvento")),
                        Titulo = reader.IsDBNull(reader.GetOrdinal("Titulo")) ? null : reader.GetString(reader.GetOrdinal("Titulo")),
                        Descripcion = reader.IsDBNull(reader.GetOrdinal("Descripcion")) ? null : reader.GetString(reader.GetOrdinal("Descripcion")),
                    }
                            );
                        }
                    }
                }
            }

            return lista;
        }

        public async Task<HistorialPaciente?> GetByIdAsync(long id)
        {
            HistorialPaciente? resultado = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT IdHistorialPaciente, IdPaciente, IdUsuario, TipoEvento, FechaEvento, Titulo, Descripcion
                    FROM HistorialPaciente
                    WHERE IdHistorialPaciente = @IdHistorialPaciente";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdHistorialPaciente", id);

                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            resultado =
                    new HistorialPaciente
                    {
                        IdHistorialPaciente = reader.GetInt64(reader.GetOrdinal("IdHistorialPaciente")),
                        IdPaciente = reader.GetInt32(reader.GetOrdinal("IdPaciente")),
                        IdUsuario = reader.IsDBNull(reader.GetOrdinal("IdUsuario")) ? null : reader.GetInt32(reader.GetOrdinal("IdUsuario")),
                        TipoEvento = reader.IsDBNull(reader.GetOrdinal("TipoEvento")) ? null : reader.GetString(reader.GetOrdinal("TipoEvento")),
                        FechaEvento = reader.IsDBNull(reader.GetOrdinal("FechaEvento")) ? null : reader.GetDateTime(reader.GetOrdinal("FechaEvento")),
                        Titulo = reader.IsDBNull(reader.GetOrdinal("Titulo")) ? null : reader.GetString(reader.GetOrdinal("Titulo")),
                        Descripcion = reader.IsDBNull(reader.GetOrdinal("Descripcion")) ? null : reader.GetString(reader.GetOrdinal("Descripcion")),
                    };
                        }
                    }
                }
            }

            return resultado;
        }

        public async Task<HistorialPaciente> CreateAsync(HistorialPaciente entity)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    INSERT INTO HistorialPaciente (IdPaciente, IdUsuario, TipoEvento, FechaEvento, Titulo, Descripcion)
                    OUTPUT INSERTED.IdHistorialPaciente
                    VALUES (@IdPaciente, @IdUsuario, @TipoEvento, @FechaEvento, @Titulo, @Descripcion)";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdPaciente", entity.IdPaciente);
                    command.Parameters.AddWithValue("@IdUsuario", (object?)entity.IdUsuario ?? DBNull.Value);
                    command.Parameters.AddWithValue("@TipoEvento", (object?)entity.TipoEvento ?? DBNull.Value);
                    command.Parameters.AddWithValue("@FechaEvento", (object?)entity.FechaEvento ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Titulo", (object?)entity.Titulo ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Descripcion", (object?)entity.Descripcion ?? DBNull.Value);

                    await connection.OpenAsync();
                    entity.IdHistorialPaciente = Convert.ToInt64(await command.ExecuteScalarAsync());
                }
            }

            return entity;
        }

        public async Task<bool> UpdateAsync(long id, HistorialPaciente entity)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    UPDATE HistorialPaciente
                    SET IdPaciente = @IdPaciente,
                        IdUsuario = @IdUsuario,
                        TipoEvento = @TipoEvento,
                        FechaEvento = @FechaEvento,
                        Titulo = @Titulo,
                        Descripcion = @Descripcion
                    WHERE IdHistorialPaciente = @IdHistorialPaciente";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdHistorialPaciente", id);
                    command.Parameters.AddWithValue("@IdPaciente", entity.IdPaciente);
                    command.Parameters.AddWithValue("@IdUsuario", (object?)entity.IdUsuario ?? DBNull.Value);
                    command.Parameters.AddWithValue("@TipoEvento", (object?)entity.TipoEvento ?? DBNull.Value);
                    command.Parameters.AddWithValue("@FechaEvento", (object?)entity.FechaEvento ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Titulo", (object?)entity.Titulo ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Descripcion", (object?)entity.Descripcion ?? DBNull.Value);

                    await connection.OpenAsync();
                    var rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
        }

        public async Task<bool> DeleteAsync(long id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    DELETE FROM HistorialPaciente
                    WHERE IdHistorialPaciente = @IdHistorialPaciente";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdHistorialPaciente", id);

                    await connection.OpenAsync();
                    var rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
        }
    }
}
