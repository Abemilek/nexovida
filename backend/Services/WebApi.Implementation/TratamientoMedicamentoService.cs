using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using WebApi.Interface;
using WebApi.Models;

namespace WebApi.Implementation
{
    public class TratamientoMedicamentoService : ITratamientoMedicamentoService
    {
        private readonly string _connectionString;

        public TratamientoMedicamentoService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection")
                ?? throw new InvalidOperationException("La cadena de conexión no puede ser nula.");
        }

        public async Task<IEnumerable<TratamientoMedicamento>> GetAllAsync()
        {
            var lista = new List<TratamientoMedicamento>();

            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT IdTratamientoMedicamento, IdTratamiento, IdMedicamento, Dosis, Frecuencia, ViaAdministracion, Horarios, Instrucciones
                    FROM TratamientoMedicamento
                    ORDER BY IdTratamientoMedicamento DESC";

                using (var command = new SqlCommand(query, connection))
                {
                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            lista.Add(
                    new TratamientoMedicamento
                    {
                        IdTratamientoMedicamento = reader.GetInt32(reader.GetOrdinal("IdTratamientoMedicamento")),
                        IdTratamiento = reader.GetInt32(reader.GetOrdinal("IdTratamiento")),
                        IdMedicamento = reader.GetInt32(reader.GetOrdinal("IdMedicamento")),
                        Dosis = reader.IsDBNull(reader.GetOrdinal("Dosis")) ? null : reader.GetString(reader.GetOrdinal("Dosis")),
                        Frecuencia = reader.IsDBNull(reader.GetOrdinal("Frecuencia")) ? null : reader.GetString(reader.GetOrdinal("Frecuencia")),
                        ViaAdministracion = reader.IsDBNull(reader.GetOrdinal("ViaAdministracion")) ? null : reader.GetString(reader.GetOrdinal("ViaAdministracion")),
                        Horarios = reader.IsDBNull(reader.GetOrdinal("Horarios")) ? null : reader.GetString(reader.GetOrdinal("Horarios")),
                        Instrucciones = reader.IsDBNull(reader.GetOrdinal("Instrucciones")) ? null : reader.GetString(reader.GetOrdinal("Instrucciones")),
                    }
                            );
                        }
                    }
                }
            }

            return lista;
        }

        public async Task<TratamientoMedicamento?> GetByIdAsync(int id)
        {
            TratamientoMedicamento? resultado = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT IdTratamientoMedicamento, IdTratamiento, IdMedicamento, Dosis, Frecuencia, ViaAdministracion, Horarios, Instrucciones
                    FROM TratamientoMedicamento
                    WHERE IdTratamientoMedicamento = @IdTratamientoMedicamento";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdTratamientoMedicamento", id);

                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            resultado =
                    new TratamientoMedicamento
                    {
                        IdTratamientoMedicamento = reader.GetInt32(reader.GetOrdinal("IdTratamientoMedicamento")),
                        IdTratamiento = reader.GetInt32(reader.GetOrdinal("IdTratamiento")),
                        IdMedicamento = reader.GetInt32(reader.GetOrdinal("IdMedicamento")),
                        Dosis = reader.IsDBNull(reader.GetOrdinal("Dosis")) ? null : reader.GetString(reader.GetOrdinal("Dosis")),
                        Frecuencia = reader.IsDBNull(reader.GetOrdinal("Frecuencia")) ? null : reader.GetString(reader.GetOrdinal("Frecuencia")),
                        ViaAdministracion = reader.IsDBNull(reader.GetOrdinal("ViaAdministracion")) ? null : reader.GetString(reader.GetOrdinal("ViaAdministracion")),
                        Horarios = reader.IsDBNull(reader.GetOrdinal("Horarios")) ? null : reader.GetString(reader.GetOrdinal("Horarios")),
                        Instrucciones = reader.IsDBNull(reader.GetOrdinal("Instrucciones")) ? null : reader.GetString(reader.GetOrdinal("Instrucciones")),
                    };
                        }
                    }
                }
            }

            return resultado;
        }

        public async Task<TratamientoMedicamento> CreateAsync(TratamientoMedicamento entity)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    INSERT INTO TratamientoMedicamento (IdTratamiento, IdMedicamento, Dosis, Frecuencia, ViaAdministracion, Horarios, Instrucciones)
                    OUTPUT INSERTED.IdTratamientoMedicamento
                    VALUES (@IdTratamiento, @IdMedicamento, @Dosis, @Frecuencia, @ViaAdministracion, @Horarios, @Instrucciones)";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdTratamiento", entity.IdTratamiento);
                    command.Parameters.AddWithValue("@IdMedicamento", entity.IdMedicamento);
                    command.Parameters.AddWithValue("@Dosis", (object?)entity.Dosis ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Frecuencia", (object?)entity.Frecuencia ?? DBNull.Value);
                    command.Parameters.AddWithValue("@ViaAdministracion", (object?)entity.ViaAdministracion ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Horarios", (object?)entity.Horarios ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Instrucciones", (object?)entity.Instrucciones ?? DBNull.Value);

                    await connection.OpenAsync();
                    entity.IdTratamientoMedicamento = Convert.ToInt32(await command.ExecuteScalarAsync());
                }
            }

            return entity;
        }

        public async Task<bool> UpdateAsync(int id, TratamientoMedicamento entity)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    UPDATE TratamientoMedicamento
                    SET IdTratamiento = @IdTratamiento,
                        IdMedicamento = @IdMedicamento,
                        Dosis = @Dosis,
                        Frecuencia = @Frecuencia,
                        ViaAdministracion = @ViaAdministracion,
                        Horarios = @Horarios,
                        Instrucciones = @Instrucciones
                    WHERE IdTratamientoMedicamento = @IdTratamientoMedicamento";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdTratamientoMedicamento", id);
                    command.Parameters.AddWithValue("@IdTratamiento", entity.IdTratamiento);
                    command.Parameters.AddWithValue("@IdMedicamento", entity.IdMedicamento);
                    command.Parameters.AddWithValue("@Dosis", (object?)entity.Dosis ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Frecuencia", (object?)entity.Frecuencia ?? DBNull.Value);
                    command.Parameters.AddWithValue("@ViaAdministracion", (object?)entity.ViaAdministracion ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Horarios", (object?)entity.Horarios ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Instrucciones", (object?)entity.Instrucciones ?? DBNull.Value);

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
                    DELETE FROM TratamientoMedicamento
                    WHERE IdTratamientoMedicamento = @IdTratamientoMedicamento";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdTratamientoMedicamento", id);

                    await connection.OpenAsync();
                    var rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
        }
    }
}
