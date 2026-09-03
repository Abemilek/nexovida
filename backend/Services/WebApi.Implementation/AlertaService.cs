using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using WebApi.Interface;
using WebApi.Models;

namespace WebApi.Implementation
{
    public class AlertaService : IAlertaService
    {
        private readonly string _connectionString;

        public AlertaService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection")
                ?? throw new InvalidOperationException("La cadena de conexión no puede ser nula.");
        }

        public async Task<IEnumerable<Alerta>> GetAllAsync()
        {
            var lista = new List<Alerta>();

            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT IdAlerta, IdPaciente, IdIndicadorSalud, IdRecordatorio, Titulo, Mensaje, TipoAlerta, NivelPrioridad, FechaGeneracion, FechaLectura, Atendida, FechaAtencion
                    FROM Alertas
                    ORDER BY IdAlerta DESC";

                using (var command = new SqlCommand(query, connection))
                {
                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            lista.Add(
                    new Alerta
                    {
                        IdAlerta = reader.GetInt64(reader.GetOrdinal("IdAlerta")),
                        IdPaciente = reader.GetInt32(reader.GetOrdinal("IdPaciente")),
                        IdIndicadorSalud = reader.IsDBNull(reader.GetOrdinal("IdIndicadorSalud")) ? null : reader.GetInt64(reader.GetOrdinal("IdIndicadorSalud")),
                        IdRecordatorio = reader.IsDBNull(reader.GetOrdinal("IdRecordatorio")) ? null : reader.GetInt32(reader.GetOrdinal("IdRecordatorio")),
                        Titulo = reader.IsDBNull(reader.GetOrdinal("Titulo")) ? null : reader.GetString(reader.GetOrdinal("Titulo")),
                        Mensaje = reader.IsDBNull(reader.GetOrdinal("Mensaje")) ? null : reader.GetString(reader.GetOrdinal("Mensaje")),
                        TipoAlerta = reader.IsDBNull(reader.GetOrdinal("TipoAlerta")) ? null : reader.GetString(reader.GetOrdinal("TipoAlerta")),
                        NivelPrioridad = reader.IsDBNull(reader.GetOrdinal("NivelPrioridad")) ? null : reader.GetString(reader.GetOrdinal("NivelPrioridad")),
                        FechaGeneracion = reader.IsDBNull(reader.GetOrdinal("FechaGeneracion")) ? null : reader.GetDateTime(reader.GetOrdinal("FechaGeneracion")),
                        FechaLectura = reader.IsDBNull(reader.GetOrdinal("FechaLectura")) ? null : reader.GetDateTime(reader.GetOrdinal("FechaLectura")),
                        Atendida = reader.IsDBNull(reader.GetOrdinal("Atendida")) ? null : reader.GetBoolean(reader.GetOrdinal("Atendida")),
                        FechaAtencion = reader.IsDBNull(reader.GetOrdinal("FechaAtencion")) ? null : reader.GetDateTime(reader.GetOrdinal("FechaAtencion")),
                    }
                            );
                        }
                    }
                }
            }

            return lista;
        }

        public async Task<Alerta?> GetByIdAsync(long id)
        {
            Alerta? resultado = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT IdAlerta, IdPaciente, IdIndicadorSalud, IdRecordatorio, Titulo, Mensaje, TipoAlerta, NivelPrioridad, FechaGeneracion, FechaLectura, Atendida, FechaAtencion
                    FROM Alertas
                    WHERE IdAlerta = @IdAlerta";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdAlerta", id);

                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            resultado =
                    new Alerta
                    {
                        IdAlerta = reader.GetInt64(reader.GetOrdinal("IdAlerta")),
                        IdPaciente = reader.GetInt32(reader.GetOrdinal("IdPaciente")),
                        IdIndicadorSalud = reader.IsDBNull(reader.GetOrdinal("IdIndicadorSalud")) ? null : reader.GetInt64(reader.GetOrdinal("IdIndicadorSalud")),
                        IdRecordatorio = reader.IsDBNull(reader.GetOrdinal("IdRecordatorio")) ? null : reader.GetInt32(reader.GetOrdinal("IdRecordatorio")),
                        Titulo = reader.IsDBNull(reader.GetOrdinal("Titulo")) ? null : reader.GetString(reader.GetOrdinal("Titulo")),
                        Mensaje = reader.IsDBNull(reader.GetOrdinal("Mensaje")) ? null : reader.GetString(reader.GetOrdinal("Mensaje")),
                        TipoAlerta = reader.IsDBNull(reader.GetOrdinal("TipoAlerta")) ? null : reader.GetString(reader.GetOrdinal("TipoAlerta")),
                        NivelPrioridad = reader.IsDBNull(reader.GetOrdinal("NivelPrioridad")) ? null : reader.GetString(reader.GetOrdinal("NivelPrioridad")),
                        FechaGeneracion = reader.IsDBNull(reader.GetOrdinal("FechaGeneracion")) ? null : reader.GetDateTime(reader.GetOrdinal("FechaGeneracion")),
                        FechaLectura = reader.IsDBNull(reader.GetOrdinal("FechaLectura")) ? null : reader.GetDateTime(reader.GetOrdinal("FechaLectura")),
                        Atendida = reader.IsDBNull(reader.GetOrdinal("Atendida")) ? null : reader.GetBoolean(reader.GetOrdinal("Atendida")),
                        FechaAtencion = reader.IsDBNull(reader.GetOrdinal("FechaAtencion")) ? null : reader.GetDateTime(reader.GetOrdinal("FechaAtencion")),
                    };
                        }
                    }
                }
            }

            return resultado;
        }

        public async Task<Alerta> CreateAsync(Alerta entity)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    INSERT INTO Alertas (IdPaciente, IdIndicadorSalud, IdRecordatorio, Titulo, Mensaje, TipoAlerta, NivelPrioridad, FechaGeneracion, FechaLectura, Atendida, FechaAtencion)
                    OUTPUT INSERTED.IdAlerta
                    VALUES (@IdPaciente, @IdIndicadorSalud, @IdRecordatorio, @Titulo, @Mensaje, @TipoAlerta, @NivelPrioridad, @FechaGeneracion, @FechaLectura, @Atendida, @FechaAtencion)";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdPaciente", entity.IdPaciente);
                    command.Parameters.AddWithValue("@IdIndicadorSalud", (object?)entity.IdIndicadorSalud ?? DBNull.Value);
                    command.Parameters.AddWithValue("@IdRecordatorio", (object?)entity.IdRecordatorio ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Titulo", (object?)entity.Titulo ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Mensaje", (object?)entity.Mensaje ?? DBNull.Value);
                    command.Parameters.AddWithValue("@TipoAlerta", (object?)entity.TipoAlerta ?? DBNull.Value);
                    command.Parameters.AddWithValue("@NivelPrioridad", (object?)entity.NivelPrioridad ?? DBNull.Value);
                    command.Parameters.AddWithValue("@FechaGeneracion", (object?)entity.FechaGeneracion ?? DBNull.Value);
                    command.Parameters.AddWithValue("@FechaLectura", (object?)entity.FechaLectura ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Atendida", (object?)entity.Atendida ?? DBNull.Value);
                    command.Parameters.AddWithValue("@FechaAtencion", (object?)entity.FechaAtencion ?? DBNull.Value);

                    await connection.OpenAsync();
                    entity.IdAlerta = Convert.ToInt64(await command.ExecuteScalarAsync());
                }
            }

            return entity;
        }

        public async Task<bool> UpdateAsync(long id, Alerta entity)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    UPDATE Alertas
                    SET IdPaciente = @IdPaciente,
                        IdIndicadorSalud = @IdIndicadorSalud,
                        IdRecordatorio = @IdRecordatorio,
                        Titulo = @Titulo,
                        Mensaje = @Mensaje,
                        TipoAlerta = @TipoAlerta,
                        NivelPrioridad = @NivelPrioridad,
                        FechaGeneracion = @FechaGeneracion,
                        FechaLectura = @FechaLectura,
                        Atendida = @Atendida,
                        FechaAtencion = @FechaAtencion
                    WHERE IdAlerta = @IdAlerta";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdAlerta", id);
                    command.Parameters.AddWithValue("@IdPaciente", entity.IdPaciente);
                    command.Parameters.AddWithValue("@IdIndicadorSalud", (object?)entity.IdIndicadorSalud ?? DBNull.Value);
                    command.Parameters.AddWithValue("@IdRecordatorio", (object?)entity.IdRecordatorio ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Titulo", (object?)entity.Titulo ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Mensaje", (object?)entity.Mensaje ?? DBNull.Value);
                    command.Parameters.AddWithValue("@TipoAlerta", (object?)entity.TipoAlerta ?? DBNull.Value);
                    command.Parameters.AddWithValue("@NivelPrioridad", (object?)entity.NivelPrioridad ?? DBNull.Value);
                    command.Parameters.AddWithValue("@FechaGeneracion", (object?)entity.FechaGeneracion ?? DBNull.Value);
                    command.Parameters.AddWithValue("@FechaLectura", (object?)entity.FechaLectura ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Atendida", (object?)entity.Atendida ?? DBNull.Value);
                    command.Parameters.AddWithValue("@FechaAtencion", (object?)entity.FechaAtencion ?? DBNull.Value);

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
                    DELETE FROM Alertas
                    WHERE IdAlerta = @IdAlerta";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdAlerta", id);

                    await connection.OpenAsync();
                    var rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
        }
    }
}
