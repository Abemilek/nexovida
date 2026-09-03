using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using WebApi.Interface;
using WebApi.Models;

namespace WebApi.Implementation
{
    public class RecordatorioService : IRecordatorioService
    {
        private readonly string _connectionString;

        public RecordatorioService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection")
                ?? throw new InvalidOperationException("La cadena de conexión no puede ser nula.");
        }

        public async Task<IEnumerable<Recordatorio>> GetAllAsync()
        {
            var lista = new List<Recordatorio>();

            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT IdRecordatorio, IdPaciente, IdTratamientoMedicamento, IdCita, Titulo, Descripcion, TipoRecordatorio, FechaHoraProgramada, Repetir, FrecuenciaRepeticion, EstadoRecordatorio, FechaCompletado, Activo
                    FROM Recordatorios
                    ORDER BY IdRecordatorio DESC";

                using (var command = new SqlCommand(query, connection))
                {
                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            lista.Add(
                    new Recordatorio
                    {
                        IdRecordatorio = reader.GetInt32(reader.GetOrdinal("IdRecordatorio")),
                        IdPaciente = reader.GetInt32(reader.GetOrdinal("IdPaciente")),
                        IdTratamientoMedicamento = reader.IsDBNull(reader.GetOrdinal("IdTratamientoMedicamento")) ? null : reader.GetInt32(reader.GetOrdinal("IdTratamientoMedicamento")),
                        IdCita = reader.IsDBNull(reader.GetOrdinal("IdCita")) ? null : reader.GetInt32(reader.GetOrdinal("IdCita")),
                        Titulo = reader.IsDBNull(reader.GetOrdinal("Titulo")) ? null : reader.GetString(reader.GetOrdinal("Titulo")),
                        Descripcion = reader.IsDBNull(reader.GetOrdinal("Descripcion")) ? null : reader.GetString(reader.GetOrdinal("Descripcion")),
                        TipoRecordatorio = reader.IsDBNull(reader.GetOrdinal("TipoRecordatorio")) ? null : reader.GetString(reader.GetOrdinal("TipoRecordatorio")),
                        FechaHoraProgramada = reader.IsDBNull(reader.GetOrdinal("FechaHoraProgramada")) ? null : reader.GetDateTime(reader.GetOrdinal("FechaHoraProgramada")),
                        Repetir = reader.IsDBNull(reader.GetOrdinal("Repetir")) ? null : reader.GetBoolean(reader.GetOrdinal("Repetir")),
                        FrecuenciaRepeticion = reader.IsDBNull(reader.GetOrdinal("FrecuenciaRepeticion")) ? null : reader.GetString(reader.GetOrdinal("FrecuenciaRepeticion")),
                        EstadoRecordatorio = reader.IsDBNull(reader.GetOrdinal("EstadoRecordatorio")) ? null : reader.GetString(reader.GetOrdinal("EstadoRecordatorio")),
                        FechaCompletado = reader.IsDBNull(reader.GetOrdinal("FechaCompletado")) ? null : reader.GetDateTime(reader.GetOrdinal("FechaCompletado")),
                        Activo = reader.IsDBNull(reader.GetOrdinal("Activo")) ? null : reader.GetBoolean(reader.GetOrdinal("Activo")),
                    }
                            );
                        }
                    }
                }
            }

            return lista;
        }

        public async Task<Recordatorio?> GetByIdAsync(int id)
        {
            Recordatorio? resultado = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT IdRecordatorio, IdPaciente, IdTratamientoMedicamento, IdCita, Titulo, Descripcion, TipoRecordatorio, FechaHoraProgramada, Repetir, FrecuenciaRepeticion, EstadoRecordatorio, FechaCompletado, Activo
                    FROM Recordatorios
                    WHERE IdRecordatorio = @IdRecordatorio";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdRecordatorio", id);

                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            resultado =
                    new Recordatorio
                    {
                        IdRecordatorio = reader.GetInt32(reader.GetOrdinal("IdRecordatorio")),
                        IdPaciente = reader.GetInt32(reader.GetOrdinal("IdPaciente")),
                        IdTratamientoMedicamento = reader.IsDBNull(reader.GetOrdinal("IdTratamientoMedicamento")) ? null : reader.GetInt32(reader.GetOrdinal("IdTratamientoMedicamento")),
                        IdCita = reader.IsDBNull(reader.GetOrdinal("IdCita")) ? null : reader.GetInt32(reader.GetOrdinal("IdCita")),
                        Titulo = reader.IsDBNull(reader.GetOrdinal("Titulo")) ? null : reader.GetString(reader.GetOrdinal("Titulo")),
                        Descripcion = reader.IsDBNull(reader.GetOrdinal("Descripcion")) ? null : reader.GetString(reader.GetOrdinal("Descripcion")),
                        TipoRecordatorio = reader.IsDBNull(reader.GetOrdinal("TipoRecordatorio")) ? null : reader.GetString(reader.GetOrdinal("TipoRecordatorio")),
                        FechaHoraProgramada = reader.IsDBNull(reader.GetOrdinal("FechaHoraProgramada")) ? null : reader.GetDateTime(reader.GetOrdinal("FechaHoraProgramada")),
                        Repetir = reader.IsDBNull(reader.GetOrdinal("Repetir")) ? null : reader.GetBoolean(reader.GetOrdinal("Repetir")),
                        FrecuenciaRepeticion = reader.IsDBNull(reader.GetOrdinal("FrecuenciaRepeticion")) ? null : reader.GetString(reader.GetOrdinal("FrecuenciaRepeticion")),
                        EstadoRecordatorio = reader.IsDBNull(reader.GetOrdinal("EstadoRecordatorio")) ? null : reader.GetString(reader.GetOrdinal("EstadoRecordatorio")),
                        FechaCompletado = reader.IsDBNull(reader.GetOrdinal("FechaCompletado")) ? null : reader.GetDateTime(reader.GetOrdinal("FechaCompletado")),
                        Activo = reader.IsDBNull(reader.GetOrdinal("Activo")) ? null : reader.GetBoolean(reader.GetOrdinal("Activo")),
                    };
                        }
                    }
                }
            }

            return resultado;
        }

        public async Task<Recordatorio> CreateAsync(Recordatorio entity)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    INSERT INTO Recordatorios (IdPaciente, IdTratamientoMedicamento, IdCita, Titulo, Descripcion, TipoRecordatorio, FechaHoraProgramada, Repetir, FrecuenciaRepeticion, EstadoRecordatorio, FechaCompletado, Activo)
                    OUTPUT INSERTED.IdRecordatorio
                    VALUES (@IdPaciente, @IdTratamientoMedicamento, @IdCita, @Titulo, @Descripcion, @TipoRecordatorio, @FechaHoraProgramada, @Repetir, @FrecuenciaRepeticion, @EstadoRecordatorio, @FechaCompletado, @Activo)";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdPaciente", entity.IdPaciente);
                    command.Parameters.AddWithValue("@IdTratamientoMedicamento", (object?)entity.IdTratamientoMedicamento ?? DBNull.Value);
                    command.Parameters.AddWithValue("@IdCita", (object?)entity.IdCita ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Titulo", (object?)entity.Titulo ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Descripcion", (object?)entity.Descripcion ?? DBNull.Value);
                    command.Parameters.AddWithValue("@TipoRecordatorio", (object?)entity.TipoRecordatorio ?? DBNull.Value);
                    command.Parameters.AddWithValue("@FechaHoraProgramada", (object?)entity.FechaHoraProgramada ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Repetir", (object?)entity.Repetir ?? DBNull.Value);
                    command.Parameters.AddWithValue("@FrecuenciaRepeticion", (object?)entity.FrecuenciaRepeticion ?? DBNull.Value);
                    command.Parameters.AddWithValue("@EstadoRecordatorio", (object?)entity.EstadoRecordatorio ?? DBNull.Value);
                    command.Parameters.AddWithValue("@FechaCompletado", (object?)entity.FechaCompletado ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Activo", (object?)entity.Activo ?? DBNull.Value);

                    await connection.OpenAsync();
                    entity.IdRecordatorio = Convert.ToInt32(await command.ExecuteScalarAsync());
                }
            }

            return entity;
        }

        public async Task<bool> UpdateAsync(int id, Recordatorio entity)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    UPDATE Recordatorios
                    SET IdPaciente = @IdPaciente,
                        IdTratamientoMedicamento = @IdTratamientoMedicamento,
                        IdCita = @IdCita,
                        Titulo = @Titulo,
                        Descripcion = @Descripcion,
                        TipoRecordatorio = @TipoRecordatorio,
                        FechaHoraProgramada = @FechaHoraProgramada,
                        Repetir = @Repetir,
                        FrecuenciaRepeticion = @FrecuenciaRepeticion,
                        EstadoRecordatorio = @EstadoRecordatorio,
                        FechaCompletado = @FechaCompletado,
                        Activo = @Activo
                    WHERE IdRecordatorio = @IdRecordatorio";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdRecordatorio", id);
                    command.Parameters.AddWithValue("@IdPaciente", entity.IdPaciente);
                    command.Parameters.AddWithValue("@IdTratamientoMedicamento", (object?)entity.IdTratamientoMedicamento ?? DBNull.Value);
                    command.Parameters.AddWithValue("@IdCita", (object?)entity.IdCita ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Titulo", (object?)entity.Titulo ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Descripcion", (object?)entity.Descripcion ?? DBNull.Value);
                    command.Parameters.AddWithValue("@TipoRecordatorio", (object?)entity.TipoRecordatorio ?? DBNull.Value);
                    command.Parameters.AddWithValue("@FechaHoraProgramada", (object?)entity.FechaHoraProgramada ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Repetir", (object?)entity.Repetir ?? DBNull.Value);
                    command.Parameters.AddWithValue("@FrecuenciaRepeticion", (object?)entity.FrecuenciaRepeticion ?? DBNull.Value);
                    command.Parameters.AddWithValue("@EstadoRecordatorio", (object?)entity.EstadoRecordatorio ?? DBNull.Value);
                    command.Parameters.AddWithValue("@FechaCompletado", (object?)entity.FechaCompletado ?? DBNull.Value);
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
                    UPDATE Recordatorios
                    SET Activo = 0
                    WHERE IdRecordatorio = @IdRecordatorio";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdRecordatorio", id);

                    await connection.OpenAsync();
                    var rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
        }
    }
}
