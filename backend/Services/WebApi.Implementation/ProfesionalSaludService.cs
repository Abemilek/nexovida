using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using WebApi.Interface;
using WebApi.Models;

namespace WebApi.Implementation
{
    public class ProfesionalSaludService : IProfesionalSaludService
    {
        private readonly string _connectionString;

        public ProfesionalSaludService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection")
                ?? throw new InvalidOperationException("La cadena de conexión no puede ser nula.");
        }

        public async Task<IEnumerable<ProfesionalSalud>> GetAllAsync()
        {
            var lista = new List<ProfesionalSalud>();

            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT IdProfesional, IdUsuario, Especialidad, NumeroLicencia, CentroSalud, TelefonoProfesional, Activo
                    FROM ProfesionalSalud
                    ORDER BY IdProfesional DESC";

                using (var command = new SqlCommand(query, connection))
                {
                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            lista.Add(
                    new ProfesionalSalud
                    {
                        IdProfesional = reader.GetInt32(reader.GetOrdinal("IdProfesional")),
                        IdUsuario = reader.GetInt32(reader.GetOrdinal("IdUsuario")),
                        Especialidad = reader.IsDBNull(reader.GetOrdinal("Especialidad")) ? null : reader.GetString(reader.GetOrdinal("Especialidad")),
                        NumeroLicencia = reader.IsDBNull(reader.GetOrdinal("NumeroLicencia")) ? null : reader.GetString(reader.GetOrdinal("NumeroLicencia")),
                        CentroSalud = reader.IsDBNull(reader.GetOrdinal("CentroSalud")) ? null : reader.GetString(reader.GetOrdinal("CentroSalud")),
                        TelefonoProfesional = reader.IsDBNull(reader.GetOrdinal("TelefonoProfesional")) ? null : reader.GetString(reader.GetOrdinal("TelefonoProfesional")),
                        Activo = reader.IsDBNull(reader.GetOrdinal("Activo")) ? null : reader.GetBoolean(reader.GetOrdinal("Activo")),
                    }
                            );
                        }
                    }
                }
            }

            return lista;
        }

        public async Task<ProfesionalSalud?> GetByIdAsync(int id)
        {
            ProfesionalSalud? resultado = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT IdProfesional, IdUsuario, Especialidad, NumeroLicencia, CentroSalud, TelefonoProfesional, Activo
                    FROM ProfesionalSalud
                    WHERE IdProfesional = @IdProfesional";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdProfesional", id);

                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            resultado =
                    new ProfesionalSalud
                    {
                        IdProfesional = reader.GetInt32(reader.GetOrdinal("IdProfesional")),
                        IdUsuario = reader.GetInt32(reader.GetOrdinal("IdUsuario")),
                        Especialidad = reader.IsDBNull(reader.GetOrdinal("Especialidad")) ? null : reader.GetString(reader.GetOrdinal("Especialidad")),
                        NumeroLicencia = reader.IsDBNull(reader.GetOrdinal("NumeroLicencia")) ? null : reader.GetString(reader.GetOrdinal("NumeroLicencia")),
                        CentroSalud = reader.IsDBNull(reader.GetOrdinal("CentroSalud")) ? null : reader.GetString(reader.GetOrdinal("CentroSalud")),
                        TelefonoProfesional = reader.IsDBNull(reader.GetOrdinal("TelefonoProfesional")) ? null : reader.GetString(reader.GetOrdinal("TelefonoProfesional")),
                        Activo = reader.IsDBNull(reader.GetOrdinal("Activo")) ? null : reader.GetBoolean(reader.GetOrdinal("Activo")),
                    };
                        }
                    }
                }
            }

            return resultado;
        }

        public async Task<ProfesionalSalud> CreateAsync(ProfesionalSalud entity)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    INSERT INTO ProfesionalSalud (IdUsuario, Especialidad, NumeroLicencia, CentroSalud, TelefonoProfesional, Activo)
                    OUTPUT INSERTED.IdProfesional
                    VALUES (@IdUsuario, @Especialidad, @NumeroLicencia, @CentroSalud, @TelefonoProfesional, @Activo)";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdUsuario", (object?)entity.IdUsuario ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Especialidad", (object?)entity.Especialidad ?? DBNull.Value);
                    command.Parameters.AddWithValue("@NumeroLicencia", (object?)entity.NumeroLicencia ?? DBNull.Value);
                    command.Parameters.AddWithValue("@CentroSalud", (object?)entity.CentroSalud ?? DBNull.Value);
                    command.Parameters.AddWithValue("@TelefonoProfesional", (object?)entity.TelefonoProfesional ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Activo", (object?)entity.Activo ?? DBNull.Value);

                    await connection.OpenAsync();
                    entity.IdProfesional = Convert.ToInt32(await command.ExecuteScalarAsync());
                }
            }

            return entity;
        }

        public async Task<bool> UpdateAsync(int id, ProfesionalSalud entity)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    UPDATE ProfesionalSalud
                    SET IdUsuario = @IdUsuario,
                        Especialidad = @Especialidad,
                        NumeroLicencia = @NumeroLicencia,
                        CentroSalud = @CentroSalud,
                        TelefonoProfesional = @TelefonoProfesional,
                        Activo = @Activo
                    WHERE IdProfesional = @IdProfesional";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdProfesional", id);
                    command.Parameters.AddWithValue("@IdUsuario", (object?)entity.IdUsuario ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Especialidad", (object?)entity.Especialidad ?? DBNull.Value);
                    command.Parameters.AddWithValue("@NumeroLicencia", (object?)entity.NumeroLicencia ?? DBNull.Value);
                    command.Parameters.AddWithValue("@CentroSalud", (object?)entity.CentroSalud ?? DBNull.Value);
                    command.Parameters.AddWithValue("@TelefonoProfesional", (object?)entity.TelefonoProfesional ?? DBNull.Value);
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
                // Baja logica: la tabla maneja un indicador de estado
                var query = @"
                    UPDATE ProfesionalSalud
                    SET Activo = 0
                    WHERE IdProfesional = @IdProfesional";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdProfesional", id);

                    await connection.OpenAsync();
                    var rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
        }
    }
}
