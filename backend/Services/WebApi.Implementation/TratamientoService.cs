using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using WebApi.Interface;
using WebApi.Models;

namespace WebApi.Implementation
{
    public class TratamientoService : ITratamientoService
    {
        private readonly string _connectionString;

        public TratamientoService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection")
                ?? throw new InvalidOperationException("La cadena de conexión no puede ser nula.");
        }

        public async Task<IEnumerable<Tratamiento>> GetAllAsync()
        {
            var lista = new List<Tratamiento>();

            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT IdTratamiento, IdPaciente, IdProfesional, IdEnfermedad, NombreTratamiento, Indicaciones, FechaInicio, FechaFin, EstadoTratamiento, Observaciones
                    FROM Tratamiento
                    ORDER BY IdTratamiento DESC";

                using (var command = new SqlCommand(query, connection))
                {
                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            lista.Add(
                    new Tratamiento
                    {
                        IdTratamiento = reader.GetInt32(reader.GetOrdinal("IdTratamiento")),
                        IdPaciente = reader.GetInt32(reader.GetOrdinal("IdPaciente")),
                        IdProfesional = reader.GetInt32(reader.GetOrdinal("IdProfesional")),
                        IdEnfermedad = reader.GetInt32(reader.GetOrdinal("IdEnfermedad")),
                        NombreTratamiento = reader.IsDBNull(reader.GetOrdinal("NombreTratamiento")) ? null : reader.GetString(reader.GetOrdinal("NombreTratamiento")),
                        Indicaciones = reader.IsDBNull(reader.GetOrdinal("Indicaciones")) ? null : reader.GetString(reader.GetOrdinal("Indicaciones")),
                        FechaInicio = reader.IsDBNull(reader.GetOrdinal("FechaInicio")) ? null : reader.GetDateTime(reader.GetOrdinal("FechaInicio")),
                        FechaFin = reader.IsDBNull(reader.GetOrdinal("FechaFin")) ? null : reader.GetDateTime(reader.GetOrdinal("FechaFin")),
                        EstadoTratamiento = reader.IsDBNull(reader.GetOrdinal("EstadoTratamiento")) ? null : reader.GetString(reader.GetOrdinal("EstadoTratamiento")),
                        Observaciones = reader.IsDBNull(reader.GetOrdinal("Observaciones")) ? null : reader.GetString(reader.GetOrdinal("Observaciones")),
                    }
                            );
                        }
                    }
                }
            }

            return lista;
        }

        public async Task<Tratamiento?> GetByIdAsync(int id)
        {
            Tratamiento? resultado = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT IdTratamiento, IdPaciente, IdProfesional, IdEnfermedad, NombreTratamiento, Indicaciones, FechaInicio, FechaFin, EstadoTratamiento, Observaciones
                    FROM Tratamiento
                    WHERE IdTratamiento = @IdTratamiento";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdTratamiento", id);

                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            resultado =
                    new Tratamiento
                    {
                        IdTratamiento = reader.GetInt32(reader.GetOrdinal("IdTratamiento")),
                        IdPaciente = reader.GetInt32(reader.GetOrdinal("IdPaciente")),
                        IdProfesional = reader.GetInt32(reader.GetOrdinal("IdProfesional")),
                        IdEnfermedad = reader.GetInt32(reader.GetOrdinal("IdEnfermedad")),
                        NombreTratamiento = reader.IsDBNull(reader.GetOrdinal("NombreTratamiento")) ? null : reader.GetString(reader.GetOrdinal("NombreTratamiento")),
                        Indicaciones = reader.IsDBNull(reader.GetOrdinal("Indicaciones")) ? null : reader.GetString(reader.GetOrdinal("Indicaciones")),
                        FechaInicio = reader.IsDBNull(reader.GetOrdinal("FechaInicio")) ? null : reader.GetDateTime(reader.GetOrdinal("FechaInicio")),
                        FechaFin = reader.IsDBNull(reader.GetOrdinal("FechaFin")) ? null : reader.GetDateTime(reader.GetOrdinal("FechaFin")),
                        EstadoTratamiento = reader.IsDBNull(reader.GetOrdinal("EstadoTratamiento")) ? null : reader.GetString(reader.GetOrdinal("EstadoTratamiento")),
                        Observaciones = reader.IsDBNull(reader.GetOrdinal("Observaciones")) ? null : reader.GetString(reader.GetOrdinal("Observaciones")),
                    };
                        }
                    }
                }
            }

            return resultado;
        }

        public async Task<Tratamiento> CreateAsync(Tratamiento entity)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    INSERT INTO Tratamiento (IdPaciente, IdProfesional, IdEnfermedad, NombreTratamiento, Indicaciones, FechaInicio, FechaFin, EstadoTratamiento, Observaciones)
                    OUTPUT INSERTED.IdTratamiento
                    VALUES (@IdPaciente, @IdProfesional, @IdEnfermedad, @NombreTratamiento, @Indicaciones, @FechaInicio, @FechaFin, @EstadoTratamiento, @Observaciones)";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdPaciente", entity.IdPaciente);
                    command.Parameters.AddWithValue("@IdProfesional", entity.IdProfesional);
                    command.Parameters.AddWithValue("@IdEnfermedad", entity.IdEnfermedad);
                    command.Parameters.AddWithValue("@NombreTratamiento", (object?)entity.NombreTratamiento ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Indicaciones", (object?)entity.Indicaciones ?? DBNull.Value);
                    command.Parameters.AddWithValue("@FechaInicio", (object?)entity.FechaInicio ?? DBNull.Value);
                    command.Parameters.AddWithValue("@FechaFin", (object?)entity.FechaFin ?? DBNull.Value);
                    command.Parameters.AddWithValue("@EstadoTratamiento", (object?)entity.EstadoTratamiento ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Observaciones", (object?)entity.Observaciones ?? DBNull.Value);

                    await connection.OpenAsync();
                    entity.IdTratamiento = Convert.ToInt32(await command.ExecuteScalarAsync());
                }
            }

            return entity;
        }

        public async Task<bool> UpdateAsync(int id, Tratamiento entity)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    UPDATE Tratamiento
                    SET IdPaciente = @IdPaciente,
                        IdProfesional = @IdProfesional,
                        IdEnfermedad = @IdEnfermedad,
                        NombreTratamiento = @NombreTratamiento,
                        Indicaciones = @Indicaciones,
                        FechaInicio = @FechaInicio,
                        FechaFin = @FechaFin,
                        EstadoTratamiento = @EstadoTratamiento,
                        Observaciones = @Observaciones
                    WHERE IdTratamiento = @IdTratamiento";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdTratamiento", id);
                    command.Parameters.AddWithValue("@IdPaciente", entity.IdPaciente);
                    command.Parameters.AddWithValue("@IdProfesional", entity.IdProfesional);
                    command.Parameters.AddWithValue("@IdEnfermedad", entity.IdEnfermedad);
                    command.Parameters.AddWithValue("@NombreTratamiento", (object?)entity.NombreTratamiento ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Indicaciones", (object?)entity.Indicaciones ?? DBNull.Value);
                    command.Parameters.AddWithValue("@FechaInicio", (object?)entity.FechaInicio ?? DBNull.Value);
                    command.Parameters.AddWithValue("@FechaFin", (object?)entity.FechaFin ?? DBNull.Value);
                    command.Parameters.AddWithValue("@EstadoTratamiento", (object?)entity.EstadoTratamiento ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Observaciones", (object?)entity.Observaciones ?? DBNull.Value);

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
                    DELETE FROM Tratamiento
                    WHERE IdTratamiento = @IdTratamiento";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdTratamiento", id);

                    await connection.OpenAsync();
                    var rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
        }
    }
}
