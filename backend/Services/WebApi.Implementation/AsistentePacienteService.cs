using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using WebApi.Interface;
using WebApi.Models;

namespace WebApi.Implementation
{
    public class AsistentePacienteService : IAsistentePacienteService
    {
        private readonly string _connectionString;

        public AsistentePacienteService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection")
                ?? throw new InvalidOperationException("La cadena de conexión no puede ser nula.");
        }

        public async Task<IEnumerable<AsistentePaciente>> GetAllAsync()
        {
            var lista = new List<AsistentePaciente>();

            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT IdAsistentePaciente, IdPaciente, IdFamiliar, TipoRelacion, PuedeVerCitas, PuedeVerMedicamentos, PuedeRecibirAlertas, PuedeGestionarRecordatorios, FechaAsignacion, Activo
                    FROM AsistentePaciente
                    ORDER BY IdAsistentePaciente DESC";

                using (var command = new SqlCommand(query, connection))
                {
                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            lista.Add(
                    new AsistentePaciente
                    {
                        IdAsistentePaciente = reader.GetInt32(reader.GetOrdinal("IdAsistentePaciente")),
                        IdPaciente = reader.GetInt32(reader.GetOrdinal("IdPaciente")),
                        IdFamiliar = reader.GetInt32(reader.GetOrdinal("IdFamiliar")),
                        TipoRelacion = reader.IsDBNull(reader.GetOrdinal("TipoRelacion")) ? null : reader.GetString(reader.GetOrdinal("TipoRelacion")),
                        PuedeVerCitas = reader.IsDBNull(reader.GetOrdinal("PuedeVerCitas")) ? null : reader.GetBoolean(reader.GetOrdinal("PuedeVerCitas")),
                        PuedeVerMedicamentos = reader.IsDBNull(reader.GetOrdinal("PuedeVerMedicamentos")) ? null : reader.GetBoolean(reader.GetOrdinal("PuedeVerMedicamentos")),
                        PuedeRecibirAlertas = reader.IsDBNull(reader.GetOrdinal("PuedeRecibirAlertas")) ? null : reader.GetBoolean(reader.GetOrdinal("PuedeRecibirAlertas")),
                        PuedeGestionarRecordatorios = reader.IsDBNull(reader.GetOrdinal("PuedeGestionarRecordatorios")) ? null : reader.GetBoolean(reader.GetOrdinal("PuedeGestionarRecordatorios")),
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

        public async Task<AsistentePaciente?> GetByIdAsync(int id)
        {
            AsistentePaciente? resultado = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT IdAsistentePaciente, IdPaciente, IdFamiliar, TipoRelacion, PuedeVerCitas, PuedeVerMedicamentos, PuedeRecibirAlertas, PuedeGestionarRecordatorios, FechaAsignacion, Activo
                    FROM AsistentePaciente
                    WHERE IdAsistentePaciente = @IdAsistentePaciente";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdAsistentePaciente", id);

                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            resultado =
                    new AsistentePaciente
                    {
                        IdAsistentePaciente = reader.GetInt32(reader.GetOrdinal("IdAsistentePaciente")),
                        IdPaciente = reader.GetInt32(reader.GetOrdinal("IdPaciente")),
                        IdFamiliar = reader.GetInt32(reader.GetOrdinal("IdFamiliar")),
                        TipoRelacion = reader.IsDBNull(reader.GetOrdinal("TipoRelacion")) ? null : reader.GetString(reader.GetOrdinal("TipoRelacion")),
                        PuedeVerCitas = reader.IsDBNull(reader.GetOrdinal("PuedeVerCitas")) ? null : reader.GetBoolean(reader.GetOrdinal("PuedeVerCitas")),
                        PuedeVerMedicamentos = reader.IsDBNull(reader.GetOrdinal("PuedeVerMedicamentos")) ? null : reader.GetBoolean(reader.GetOrdinal("PuedeVerMedicamentos")),
                        PuedeRecibirAlertas = reader.IsDBNull(reader.GetOrdinal("PuedeRecibirAlertas")) ? null : reader.GetBoolean(reader.GetOrdinal("PuedeRecibirAlertas")),
                        PuedeGestionarRecordatorios = reader.IsDBNull(reader.GetOrdinal("PuedeGestionarRecordatorios")) ? null : reader.GetBoolean(reader.GetOrdinal("PuedeGestionarRecordatorios")),
                        FechaAsignacion = reader.IsDBNull(reader.GetOrdinal("FechaAsignacion")) ? null : reader.GetDateTime(reader.GetOrdinal("FechaAsignacion")),
                        Activo = reader.IsDBNull(reader.GetOrdinal("Activo")) ? null : reader.GetBoolean(reader.GetOrdinal("Activo")),
                    };
                        }
                    }
                }
            }

            return resultado;
        }

        public async Task<AsistentePaciente> CreateAsync(AsistentePaciente entity)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    INSERT INTO AsistentePaciente (IdPaciente, IdFamiliar, TipoRelacion, PuedeVerCitas, PuedeVerMedicamentos, PuedeRecibirAlertas, PuedeGestionarRecordatorios, FechaAsignacion, Activo)
                    OUTPUT INSERTED.IdAsistentePaciente
                    VALUES (@IdPaciente, @IdFamiliar, @TipoRelacion, @PuedeVerCitas, @PuedeVerMedicamentos, @PuedeRecibirAlertas, @PuedeGestionarRecordatorios, @FechaAsignacion, @Activo)";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdPaciente", (object?)entity.IdPaciente ?? DBNull.Value);
                    command.Parameters.AddWithValue("@IdFamiliar", (object?)entity.IdFamiliar ?? DBNull.Value);
                    command.Parameters.AddWithValue("@TipoRelacion", (object?)entity.TipoRelacion ?? DBNull.Value);
                    command.Parameters.AddWithValue("@PuedeVerCitas", (object?)entity.PuedeVerCitas ?? DBNull.Value);
                    command.Parameters.AddWithValue("@PuedeVerMedicamentos", (object?)entity.PuedeVerMedicamentos ?? DBNull.Value);
                    command.Parameters.AddWithValue("@PuedeRecibirAlertas", (object?)entity.PuedeRecibirAlertas ?? DBNull.Value);
                    command.Parameters.AddWithValue("@PuedeGestionarRecordatorios", (object?)entity.PuedeGestionarRecordatorios ?? DBNull.Value);
                    command.Parameters.AddWithValue("@FechaAsignacion", (object?)entity.FechaAsignacion ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Activo", (object?)entity.Activo ?? DBNull.Value);

                    await connection.OpenAsync();
                    entity.IdAsistentePaciente = Convert.ToInt32(await command.ExecuteScalarAsync());
                }
            }

            return entity;
        }

        public async Task<bool> UpdateAsync(int id, AsistentePaciente entity)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    UPDATE AsistentePaciente
                    SET IdPaciente = @IdPaciente,
                        IdFamiliar = @IdFamiliar,
                        TipoRelacion = @TipoRelacion,
                        PuedeVerCitas = @PuedeVerCitas,
                        PuedeVerMedicamentos = @PuedeVerMedicamentos,
                        PuedeRecibirAlertas = @PuedeRecibirAlertas,
                        PuedeGestionarRecordatorios = @PuedeGestionarRecordatorios,
                        FechaAsignacion = @FechaAsignacion,
                        Activo = @Activo
                    WHERE IdAsistentePaciente = @IdAsistentePaciente";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdAsistentePaciente", id);
                    command.Parameters.AddWithValue("@IdPaciente", (object?)entity.IdPaciente ?? DBNull.Value);
                    command.Parameters.AddWithValue("@IdFamiliar", (object?)entity.IdFamiliar ?? DBNull.Value);
                    command.Parameters.AddWithValue("@TipoRelacion", (object?)entity.TipoRelacion ?? DBNull.Value);
                    command.Parameters.AddWithValue("@PuedeVerCitas", (object?)entity.PuedeVerCitas ?? DBNull.Value);
                    command.Parameters.AddWithValue("@PuedeVerMedicamentos", (object?)entity.PuedeVerMedicamentos ?? DBNull.Value);
                    command.Parameters.AddWithValue("@PuedeRecibirAlertas", (object?)entity.PuedeRecibirAlertas ?? DBNull.Value);
                    command.Parameters.AddWithValue("@PuedeGestionarRecordatorios", (object?)entity.PuedeGestionarRecordatorios ?? DBNull.Value);
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
                    UPDATE AsistentePaciente
                    SET Activo = 0
                    WHERE IdAsistentePaciente = @IdAsistentePaciente";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdAsistentePaciente", id);

                    await connection.OpenAsync();
                    var rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
        }
    }
}
