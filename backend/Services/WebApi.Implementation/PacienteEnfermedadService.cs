using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using WebApi.Interface;
using WebApi.Models;

namespace WebApi.Implementation
{
    public class PacienteEnfermedadService : IPacienteEnfermedadService
    {
        private readonly string _connectionString;

        public PacienteEnfermedadService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection")
                ?? throw new InvalidOperationException("La cadena de conexión no puede ser nula.");
        }

        public async Task<IEnumerable<PacienteEnfermedad>> GetAllAsync()
        {
            var lista = new List<PacienteEnfermedad>();

            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT IdPacienteEnfermedad, IdPaciente, IdEnfermedad, FechaDiagnostico, Observaciones, Activa
                    FROM PacienteEnfermedad
                    ORDER BY IdPacienteEnfermedad DESC";

                using (var command = new SqlCommand(query, connection))
                {
                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            lista.Add(
                    new PacienteEnfermedad
                    {
                        IdPacienteEnfermedad = reader.GetInt32(reader.GetOrdinal("IdPacienteEnfermedad")),
                        IdPaciente = reader.GetInt32(reader.GetOrdinal("IdPaciente")),
                        IdEnfermedad = reader.GetInt32(reader.GetOrdinal("IdEnfermedad")),
                        FechaDiagnostico = reader.IsDBNull(reader.GetOrdinal("FechaDiagnostico")) ? null : reader.GetDateTime(reader.GetOrdinal("FechaDiagnostico")),
                        Observaciones = reader.IsDBNull(reader.GetOrdinal("Observaciones")) ? null : reader.GetString(reader.GetOrdinal("Observaciones")),
                        Activa = reader.IsDBNull(reader.GetOrdinal("Activa")) ? null : reader.GetBoolean(reader.GetOrdinal("Activa")),
                    }
                            );
                        }
                    }
                }
            }

            return lista;
        }

        public async Task<PacienteEnfermedad?> GetByIdAsync(int id)
        {
            PacienteEnfermedad? resultado = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT IdPacienteEnfermedad, IdPaciente, IdEnfermedad, FechaDiagnostico, Observaciones, Activa
                    FROM PacienteEnfermedad
                    WHERE IdPacienteEnfermedad = @IdPacienteEnfermedad";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdPacienteEnfermedad", id);

                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            resultado =
                    new PacienteEnfermedad
                    {
                        IdPacienteEnfermedad = reader.GetInt32(reader.GetOrdinal("IdPacienteEnfermedad")),
                        IdPaciente = reader.GetInt32(reader.GetOrdinal("IdPaciente")),
                        IdEnfermedad = reader.GetInt32(reader.GetOrdinal("IdEnfermedad")),
                        FechaDiagnostico = reader.IsDBNull(reader.GetOrdinal("FechaDiagnostico")) ? null : reader.GetDateTime(reader.GetOrdinal("FechaDiagnostico")),
                        Observaciones = reader.IsDBNull(reader.GetOrdinal("Observaciones")) ? null : reader.GetString(reader.GetOrdinal("Observaciones")),
                        Activa = reader.IsDBNull(reader.GetOrdinal("Activa")) ? null : reader.GetBoolean(reader.GetOrdinal("Activa")),
                    };
                        }
                    }
                }
            }

            return resultado;
        }

        public async Task<PacienteEnfermedad> CreateAsync(PacienteEnfermedad entity)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    INSERT INTO PacienteEnfermedad (IdPaciente, IdEnfermedad, FechaDiagnostico, Observaciones, Activa)
                    OUTPUT INSERTED.IdPacienteEnfermedad
                    VALUES (@IdPaciente, @IdEnfermedad, @FechaDiagnostico, @Observaciones, @Activa)";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdPaciente", (object?)entity.IdPaciente ?? DBNull.Value);
                    command.Parameters.AddWithValue("@IdEnfermedad", (object?)entity.IdEnfermedad ?? DBNull.Value);
                    command.Parameters.AddWithValue("@FechaDiagnostico", (object?)entity.FechaDiagnostico ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Observaciones", (object?)entity.Observaciones ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Activa", (object?)entity.Activa ?? DBNull.Value);

                    await connection.OpenAsync();
                    entity.IdPacienteEnfermedad = Convert.ToInt32(await command.ExecuteScalarAsync());
                }
            }

            return entity;
        }

        public async Task<bool> UpdateAsync(int id, PacienteEnfermedad entity)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    UPDATE PacienteEnfermedad
                    SET IdPaciente = @IdPaciente,
                        IdEnfermedad = @IdEnfermedad,
                        FechaDiagnostico = @FechaDiagnostico,
                        Observaciones = @Observaciones,
                        Activa = @Activa
                    WHERE IdPacienteEnfermedad = @IdPacienteEnfermedad";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdPacienteEnfermedad", id);
                    command.Parameters.AddWithValue("@IdPaciente", (object?)entity.IdPaciente ?? DBNull.Value);
                    command.Parameters.AddWithValue("@IdEnfermedad", (object?)entity.IdEnfermedad ?? DBNull.Value);
                    command.Parameters.AddWithValue("@FechaDiagnostico", (object?)entity.FechaDiagnostico ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Observaciones", (object?)entity.Observaciones ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Activa", (object?)entity.Activa ?? DBNull.Value);

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
                    UPDATE PacienteEnfermedad
                    SET Activa = 0
                    WHERE IdPacienteEnfermedad = @IdPacienteEnfermedad";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdPacienteEnfermedad", id);

                    await connection.OpenAsync();
                    var rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
        }
    }
}
