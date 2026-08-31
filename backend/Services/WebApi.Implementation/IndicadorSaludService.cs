using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using WebApi.Interface;
using WebApi.Models;

namespace WebApi.Implementation
{
    public class IndicadorSaludService : IIndicadorSaludService
    {
        private readonly string _connectionString;

        public IndicadorSaludService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection")
                ?? throw new InvalidOperationException("La cadena de conexión no puede ser nula.");
        }

        public async Task<IEnumerable<IndicadorSalud>> GetAllAsync()
        {
            var lista = new List<IndicadorSalud>();

            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT IdIndicadorSalud, IdPaciente, IdTipoIndicador, Valor, ValorSecundario, FechaHoraMedicion, IdUsuarioRegistro, Observaciones, Fuente
                    FROM IndicadorSalud
                    ORDER BY IdIndicadorSalud DESC";

                using (var command = new SqlCommand(query, connection))
                {
                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            lista.Add(
                    new IndicadorSalud
                    {
                        IdIndicadorSalud = reader.GetInt64(reader.GetOrdinal("IdIndicadorSalud")),
                        IdPaciente = reader.GetInt32(reader.GetOrdinal("IdPaciente")),
                        IdTipoIndicador = reader.GetInt32(reader.GetOrdinal("IdTipoIndicador")),
                        Valor = reader.IsDBNull(reader.GetOrdinal("Valor")) ? null : reader.GetDecimal(reader.GetOrdinal("Valor")),
                        ValorSecundario = reader.IsDBNull(reader.GetOrdinal("ValorSecundario")) ? null : reader.GetDecimal(reader.GetOrdinal("ValorSecundario")),
                        FechaHoraMedicion = reader.IsDBNull(reader.GetOrdinal("FechaHoraMedicion")) ? null : reader.GetDateTime(reader.GetOrdinal("FechaHoraMedicion")),
                        IdUsuarioRegistro = reader.IsDBNull(reader.GetOrdinal("IdUsuarioRegistro")) ? null : reader.GetInt32(reader.GetOrdinal("IdUsuarioRegistro")),
                        Observaciones = reader.IsDBNull(reader.GetOrdinal("Observaciones")) ? null : reader.GetString(reader.GetOrdinal("Observaciones")),
                        Fuente = reader.IsDBNull(reader.GetOrdinal("Fuente")) ? null : reader.GetString(reader.GetOrdinal("Fuente")),
                    }
                            );
                        }
                    }
                }
            }

            return lista;
        }

        public async Task<IndicadorSalud?> GetByIdAsync(long id)
        {
            IndicadorSalud? resultado = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT IdIndicadorSalud, IdPaciente, IdTipoIndicador, Valor, ValorSecundario, FechaHoraMedicion, IdUsuarioRegistro, Observaciones, Fuente
                    FROM IndicadorSalud
                    WHERE IdIndicadorSalud = @IdIndicadorSalud";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdIndicadorSalud", id);

                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            resultado =
                    new IndicadorSalud
                    {
                        IdIndicadorSalud = reader.GetInt64(reader.GetOrdinal("IdIndicadorSalud")),
                        IdPaciente = reader.GetInt32(reader.GetOrdinal("IdPaciente")),
                        IdTipoIndicador = reader.GetInt32(reader.GetOrdinal("IdTipoIndicador")),
                        Valor = reader.IsDBNull(reader.GetOrdinal("Valor")) ? null : reader.GetDecimal(reader.GetOrdinal("Valor")),
                        ValorSecundario = reader.IsDBNull(reader.GetOrdinal("ValorSecundario")) ? null : reader.GetDecimal(reader.GetOrdinal("ValorSecundario")),
                        FechaHoraMedicion = reader.IsDBNull(reader.GetOrdinal("FechaHoraMedicion")) ? null : reader.GetDateTime(reader.GetOrdinal("FechaHoraMedicion")),
                        IdUsuarioRegistro = reader.IsDBNull(reader.GetOrdinal("IdUsuarioRegistro")) ? null : reader.GetInt32(reader.GetOrdinal("IdUsuarioRegistro")),
                        Observaciones = reader.IsDBNull(reader.GetOrdinal("Observaciones")) ? null : reader.GetString(reader.GetOrdinal("Observaciones")),
                        Fuente = reader.IsDBNull(reader.GetOrdinal("Fuente")) ? null : reader.GetString(reader.GetOrdinal("Fuente")),
                    };
                        }
                    }
                }
            }

            return resultado;
        }

        public async Task<IndicadorSalud> CreateAsync(IndicadorSalud entity)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    INSERT INTO IndicadorSalud (IdPaciente, IdTipoIndicador, Valor, ValorSecundario, FechaHoraMedicion, IdUsuarioRegistro, Observaciones, Fuente)
                    OUTPUT INSERTED.IdIndicadorSalud
                    VALUES (@IdPaciente, @IdTipoIndicador, @Valor, @ValorSecundario, @FechaHoraMedicion, @IdUsuarioRegistro, @Observaciones, @Fuente)";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdPaciente", entity.IdPaciente);
                    command.Parameters.AddWithValue("@IdTipoIndicador", entity.IdTipoIndicador);
                    command.Parameters.AddWithValue("@Valor", (object?)entity.Valor ?? DBNull.Value);
                    command.Parameters.AddWithValue("@ValorSecundario", (object?)entity.ValorSecundario ?? DBNull.Value);
                    command.Parameters.AddWithValue("@FechaHoraMedicion", (object?)entity.FechaHoraMedicion ?? DBNull.Value);
                    command.Parameters.AddWithValue("@IdUsuarioRegistro", (object?)entity.IdUsuarioRegistro ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Observaciones", (object?)entity.Observaciones ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Fuente", (object?)entity.Fuente ?? DBNull.Value);

                    await connection.OpenAsync();
                    entity.IdIndicadorSalud = Convert.ToInt64(await command.ExecuteScalarAsync());
                }
            }

            return entity;
        }

        public async Task<bool> UpdateAsync(long id, IndicadorSalud entity)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    UPDATE IndicadorSalud
                    SET IdPaciente = @IdPaciente,
                        IdTipoIndicador = @IdTipoIndicador,
                        Valor = @Valor,
                        ValorSecundario = @ValorSecundario,
                        FechaHoraMedicion = @FechaHoraMedicion,
                        IdUsuarioRegistro = @IdUsuarioRegistro,
                        Observaciones = @Observaciones,
                        Fuente = @Fuente
                    WHERE IdIndicadorSalud = @IdIndicadorSalud";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdIndicadorSalud", id);
                    command.Parameters.AddWithValue("@IdPaciente", entity.IdPaciente);
                    command.Parameters.AddWithValue("@IdTipoIndicador", entity.IdTipoIndicador);
                    command.Parameters.AddWithValue("@Valor", (object?)entity.Valor ?? DBNull.Value);
                    command.Parameters.AddWithValue("@ValorSecundario", (object?)entity.ValorSecundario ?? DBNull.Value);
                    command.Parameters.AddWithValue("@FechaHoraMedicion", (object?)entity.FechaHoraMedicion ?? DBNull.Value);
                    command.Parameters.AddWithValue("@IdUsuarioRegistro", (object?)entity.IdUsuarioRegistro ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Observaciones", (object?)entity.Observaciones ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Fuente", (object?)entity.Fuente ?? DBNull.Value);

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
                    DELETE FROM IndicadorSalud
                    WHERE IdIndicadorSalud = @IdIndicadorSalud";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdIndicadorSalud", id);

                    await connection.OpenAsync();
                    var rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
        }
    }
}
