using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using WebApi.Interface;
using WebApi.Models;

namespace WebApi.Implementation
{
    public class CitaService : ICitaService
    {
        private readonly string _connectionString;

        public CitaService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection")
                ?? throw new InvalidOperationException("La cadena de conexión no puede ser nula.");
        }

        public async Task<IEnumerable<Cita>> GetAllAsync()
        {
            var lista = new List<Cita>();

            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT IdCita, IdPaciente, FechaHoraInicio, FechaHoraFin, TipoCita, Motivo, Modalidad, Lugar, EstadoCita, Observaciones, FechaCreacion
                    FROM Citas
                    ORDER BY IdCita DESC";

                using (var command = new SqlCommand(query, connection))
                {
                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            lista.Add(
                    new Cita
                    {
                        IdCita = reader.GetInt32(reader.GetOrdinal("IdCita")),
                        IdPaciente = reader.GetInt32(reader.GetOrdinal("IdPaciente")),
                        FechaHoraInicio = reader.IsDBNull(reader.GetOrdinal("FechaHoraInicio")) ? null : reader.GetDateTime(reader.GetOrdinal("FechaHoraInicio")),
                        FechaHoraFin = reader.IsDBNull(reader.GetOrdinal("FechaHoraFin")) ? null : reader.GetDateTime(reader.GetOrdinal("FechaHoraFin")),
                        TipoCita = reader.IsDBNull(reader.GetOrdinal("TipoCita")) ? null : reader.GetString(reader.GetOrdinal("TipoCita")),
                        Motivo = reader.IsDBNull(reader.GetOrdinal("Motivo")) ? null : reader.GetString(reader.GetOrdinal("Motivo")),
                        Modalidad = reader.IsDBNull(reader.GetOrdinal("Modalidad")) ? null : reader.GetString(reader.GetOrdinal("Modalidad")),
                        Lugar = reader.IsDBNull(reader.GetOrdinal("Lugar")) ? null : reader.GetString(reader.GetOrdinal("Lugar")),
                        EstadoCita = reader.IsDBNull(reader.GetOrdinal("EstadoCita")) ? null : reader.GetString(reader.GetOrdinal("EstadoCita")),
                        Observaciones = reader.IsDBNull(reader.GetOrdinal("Observaciones")) ? null : reader.GetString(reader.GetOrdinal("Observaciones")),
                        FechaCreacion = reader.IsDBNull(reader.GetOrdinal("FechaCreacion")) ? null : reader.GetDateTime(reader.GetOrdinal("FechaCreacion")),
                    }
                            );
                        }
                    }
                }
            }

            return lista;
        }

        public async Task<Cita?> GetByIdAsync(int id)
        {
            Cita? resultado = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT IdCita, IdPaciente, FechaHoraInicio, FechaHoraFin, TipoCita, Motivo, Modalidad, Lugar, EstadoCita, Observaciones, FechaCreacion
                    FROM Citas
                    WHERE IdCita = @IdCita";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdCita", id);

                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            resultado =
                    new Cita
                    {
                        IdCita = reader.GetInt32(reader.GetOrdinal("IdCita")),
                        IdPaciente = reader.GetInt32(reader.GetOrdinal("IdPaciente")),
                        FechaHoraInicio = reader.IsDBNull(reader.GetOrdinal("FechaHoraInicio")) ? null : reader.GetDateTime(reader.GetOrdinal("FechaHoraInicio")),
                        FechaHoraFin = reader.IsDBNull(reader.GetOrdinal("FechaHoraFin")) ? null : reader.GetDateTime(reader.GetOrdinal("FechaHoraFin")),
                        TipoCita = reader.IsDBNull(reader.GetOrdinal("TipoCita")) ? null : reader.GetString(reader.GetOrdinal("TipoCita")),
                        Motivo = reader.IsDBNull(reader.GetOrdinal("Motivo")) ? null : reader.GetString(reader.GetOrdinal("Motivo")),
                        Modalidad = reader.IsDBNull(reader.GetOrdinal("Modalidad")) ? null : reader.GetString(reader.GetOrdinal("Modalidad")),
                        Lugar = reader.IsDBNull(reader.GetOrdinal("Lugar")) ? null : reader.GetString(reader.GetOrdinal("Lugar")),
                        EstadoCita = reader.IsDBNull(reader.GetOrdinal("EstadoCita")) ? null : reader.GetString(reader.GetOrdinal("EstadoCita")),
                        Observaciones = reader.IsDBNull(reader.GetOrdinal("Observaciones")) ? null : reader.GetString(reader.GetOrdinal("Observaciones")),
                        FechaCreacion = reader.IsDBNull(reader.GetOrdinal("FechaCreacion")) ? null : reader.GetDateTime(reader.GetOrdinal("FechaCreacion")),
                    };
                        }
                    }
                }
            }

            return resultado;
        }

        public async Task<Cita> CreateAsync(Cita entity)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    INSERT INTO Citas (IdPaciente, FechaHoraInicio, FechaHoraFin, TipoCita, Motivo, Modalidad, Lugar, EstadoCita, Observaciones, FechaCreacion)
                    OUTPUT INSERTED.IdCita
                    VALUES (@IdPaciente, @FechaHoraInicio, @FechaHoraFin, @TipoCita, @Motivo, @Modalidad, @Lugar, @EstadoCita, @Observaciones, @FechaCreacion)";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdPaciente", entity.IdPaciente);
                    command.Parameters.AddWithValue("@FechaHoraInicio", (object?)entity.FechaHoraInicio ?? DBNull.Value);
                    command.Parameters.AddWithValue("@FechaHoraFin", (object?)entity.FechaHoraFin ?? DBNull.Value);
                    command.Parameters.AddWithValue("@TipoCita", (object?)entity.TipoCita ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Motivo", (object?)entity.Motivo ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Modalidad", (object?)entity.Modalidad ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Lugar", (object?)entity.Lugar ?? DBNull.Value);
                    command.Parameters.AddWithValue("@EstadoCita", (object?)entity.EstadoCita ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Observaciones", (object?)entity.Observaciones ?? DBNull.Value);
                    command.Parameters.AddWithValue("@FechaCreacion", (object?)entity.FechaCreacion ?? DBNull.Value);

                    await connection.OpenAsync();
                    entity.IdCita = Convert.ToInt32(await command.ExecuteScalarAsync());
                }
            }

            return entity;
        }

        public async Task<bool> UpdateAsync(int id, Cita entity)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    UPDATE Citas
                    SET IdPaciente = @IdPaciente,
                        FechaHoraInicio = @FechaHoraInicio,
                        FechaHoraFin = @FechaHoraFin,
                        TipoCita = @TipoCita,
                        Motivo = @Motivo,
                        Modalidad = @Modalidad,
                        Lugar = @Lugar,
                        EstadoCita = @EstadoCita,
                        Observaciones = @Observaciones,
                        FechaCreacion = @FechaCreacion
                    WHERE IdCita = @IdCita";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdCita", id);
                    command.Parameters.AddWithValue("@IdPaciente", entity.IdPaciente);
                    command.Parameters.AddWithValue("@FechaHoraInicio", (object?)entity.FechaHoraInicio ?? DBNull.Value);
                    command.Parameters.AddWithValue("@FechaHoraFin", (object?)entity.FechaHoraFin ?? DBNull.Value);
                    command.Parameters.AddWithValue("@TipoCita", (object?)entity.TipoCita ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Motivo", (object?)entity.Motivo ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Modalidad", (object?)entity.Modalidad ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Lugar", (object?)entity.Lugar ?? DBNull.Value);
                    command.Parameters.AddWithValue("@EstadoCita", (object?)entity.EstadoCita ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Observaciones", (object?)entity.Observaciones ?? DBNull.Value);
                    command.Parameters.AddWithValue("@FechaCreacion", (object?)entity.FechaCreacion ?? DBNull.Value);

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
                    DELETE FROM Citas
                    WHERE IdCita = @IdCita";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdCita", id);

                    await connection.OpenAsync();
                    var rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
        }
    }
}
