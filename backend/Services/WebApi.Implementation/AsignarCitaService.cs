using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using WebApi.Interface;
using WebApi.Models;

namespace WebApi.Implementation
{
    public class AsignarCitaService : IAsignarCitaService
    {
        private readonly string _connectionString;

        public AsignarCitaService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection")
                ?? throw new InvalidOperationException("La cadena de conexión no puede ser nula.");
        }

        public async Task<IEnumerable<AsignarCita>> GetAllAsync()
        {
            var lista = new List<AsignarCita>();

            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT IdAsignarCita, IdCita, IdProfesional, FechaAsignacion, EsPrincipal, EstadoAsignacion
                    FROM AsignarCitas
                    ORDER BY IdAsignarCita DESC";

                using (var command = new SqlCommand(query, connection))
                {
                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            lista.Add(
                    new AsignarCita
                    {
                        IdAsignarCita = reader.GetInt32(reader.GetOrdinal("IdAsignarCita")),
                        IdCita = reader.GetInt32(reader.GetOrdinal("IdCita")),
                        IdProfesional = reader.GetInt32(reader.GetOrdinal("IdProfesional")),
                        FechaAsignacion = reader.IsDBNull(reader.GetOrdinal("FechaAsignacion")) ? null : reader.GetDateTime(reader.GetOrdinal("FechaAsignacion")),
                        EsPrincipal = reader.IsDBNull(reader.GetOrdinal("EsPrincipal")) ? null : reader.GetBoolean(reader.GetOrdinal("EsPrincipal")),
                        EstadoAsignacion = reader.IsDBNull(reader.GetOrdinal("EstadoAsignacion")) ? null : reader.GetString(reader.GetOrdinal("EstadoAsignacion")),
                    }
                            );
                        }
                    }
                }
            }

            return lista;
        }

        public async Task<AsignarCita?> GetByIdAsync(int id)
        {
            AsignarCita? resultado = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT IdAsignarCita, IdCita, IdProfesional, FechaAsignacion, EsPrincipal, EstadoAsignacion
                    FROM AsignarCitas
                    WHERE IdAsignarCita = @IdAsignarCita";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdAsignarCita", id);

                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            resultado =
                    new AsignarCita
                    {
                        IdAsignarCita = reader.GetInt32(reader.GetOrdinal("IdAsignarCita")),
                        IdCita = reader.GetInt32(reader.GetOrdinal("IdCita")),
                        IdProfesional = reader.GetInt32(reader.GetOrdinal("IdProfesional")),
                        FechaAsignacion = reader.IsDBNull(reader.GetOrdinal("FechaAsignacion")) ? null : reader.GetDateTime(reader.GetOrdinal("FechaAsignacion")),
                        EsPrincipal = reader.IsDBNull(reader.GetOrdinal("EsPrincipal")) ? null : reader.GetBoolean(reader.GetOrdinal("EsPrincipal")),
                        EstadoAsignacion = reader.IsDBNull(reader.GetOrdinal("EstadoAsignacion")) ? null : reader.GetString(reader.GetOrdinal("EstadoAsignacion")),
                    };
                        }
                    }
                }
            }

            return resultado;
        }

        public async Task<AsignarCita> CreateAsync(AsignarCita entity)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    INSERT INTO AsignarCitas (IdCita, IdProfesional, FechaAsignacion, EsPrincipal, EstadoAsignacion)
                    OUTPUT INSERTED.IdAsignarCita
                    VALUES (@IdCita, @IdProfesional, @FechaAsignacion, @EsPrincipal, @EstadoAsignacion)";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdCita", entity.IdCita);
                    command.Parameters.AddWithValue("@IdProfesional", entity.IdProfesional);
                    command.Parameters.AddWithValue("@FechaAsignacion", (object?)entity.FechaAsignacion ?? DBNull.Value);
                    command.Parameters.AddWithValue("@EsPrincipal", (object?)entity.EsPrincipal ?? DBNull.Value);
                    command.Parameters.AddWithValue("@EstadoAsignacion", (object?)entity.EstadoAsignacion ?? DBNull.Value);

                    await connection.OpenAsync();
                    entity.IdAsignarCita = Convert.ToInt32(await command.ExecuteScalarAsync());
                }
            }

            return entity;
        }

        public async Task<bool> UpdateAsync(int id, AsignarCita entity)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    UPDATE AsignarCitas
                    SET IdCita = @IdCita,
                        IdProfesional = @IdProfesional,
                        FechaAsignacion = @FechaAsignacion,
                        EsPrincipal = @EsPrincipal,
                        EstadoAsignacion = @EstadoAsignacion
                    WHERE IdAsignarCita = @IdAsignarCita";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdAsignarCita", id);
                    command.Parameters.AddWithValue("@IdCita", entity.IdCita);
                    command.Parameters.AddWithValue("@IdProfesional", entity.IdProfesional);
                    command.Parameters.AddWithValue("@FechaAsignacion", (object?)entity.FechaAsignacion ?? DBNull.Value);
                    command.Parameters.AddWithValue("@EsPrincipal", (object?)entity.EsPrincipal ?? DBNull.Value);
                    command.Parameters.AddWithValue("@EstadoAsignacion", (object?)entity.EstadoAsignacion ?? DBNull.Value);

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
                    DELETE FROM AsignarCitas
                    WHERE IdAsignarCita = @IdAsignarCita";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdAsignarCita", id);

                    await connection.OpenAsync();
                    var rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
        }
    }
}
