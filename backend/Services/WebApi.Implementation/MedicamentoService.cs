using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using WebApi.Interface;
using WebApi.Models;

namespace WebApi.Implementation
{
    public class MedicamentoService : IMedicamentoService
    {
        private readonly string _connectionString;

        public MedicamentoService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection")
                ?? throw new InvalidOperationException("La cadena de conexión no puede ser nula.");
        }

        public async Task<IEnumerable<Medicamento>> GetAllAsync()
        {
            var lista = new List<Medicamento>();

            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT IdMedicamento, NombreMedicamento, PrincipioActivo, Presentacion, Concentracion, Descripcion, Activo
                    FROM Medicamentos
                    ORDER BY IdMedicamento DESC";

                using (var command = new SqlCommand(query, connection))
                {
                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            lista.Add(
                    new Medicamento
                    {
                        IdMedicamento = reader.GetInt32(reader.GetOrdinal("IdMedicamento")),
                        NombreMedicamento = reader.IsDBNull(reader.GetOrdinal("NombreMedicamento")) ? null : reader.GetString(reader.GetOrdinal("NombreMedicamento")),
                        PrincipioActivo = reader.IsDBNull(reader.GetOrdinal("PrincipioActivo")) ? null : reader.GetString(reader.GetOrdinal("PrincipioActivo")),
                        Presentacion = reader.IsDBNull(reader.GetOrdinal("Presentacion")) ? null : reader.GetString(reader.GetOrdinal("Presentacion")),
                        Concentracion = reader.IsDBNull(reader.GetOrdinal("Concentracion")) ? null : reader.GetString(reader.GetOrdinal("Concentracion")),
                        Descripcion = reader.IsDBNull(reader.GetOrdinal("Descripcion")) ? null : reader.GetString(reader.GetOrdinal("Descripcion")),
                        Activo = reader.IsDBNull(reader.GetOrdinal("Activo")) ? null : reader.GetBoolean(reader.GetOrdinal("Activo")),
                    }
                            );
                        }
                    }
                }
            }

            return lista;
        }

        public async Task<Medicamento?> GetByIdAsync(int id)
        {
            Medicamento? resultado = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT IdMedicamento, NombreMedicamento, PrincipioActivo, Presentacion, Concentracion, Descripcion, Activo
                    FROM Medicamentos
                    WHERE IdMedicamento = @IdMedicamento";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdMedicamento", id);

                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            resultado =
                    new Medicamento
                    {
                        IdMedicamento = reader.GetInt32(reader.GetOrdinal("IdMedicamento")),
                        NombreMedicamento = reader.IsDBNull(reader.GetOrdinal("NombreMedicamento")) ? null : reader.GetString(reader.GetOrdinal("NombreMedicamento")),
                        PrincipioActivo = reader.IsDBNull(reader.GetOrdinal("PrincipioActivo")) ? null : reader.GetString(reader.GetOrdinal("PrincipioActivo")),
                        Presentacion = reader.IsDBNull(reader.GetOrdinal("Presentacion")) ? null : reader.GetString(reader.GetOrdinal("Presentacion")),
                        Concentracion = reader.IsDBNull(reader.GetOrdinal("Concentracion")) ? null : reader.GetString(reader.GetOrdinal("Concentracion")),
                        Descripcion = reader.IsDBNull(reader.GetOrdinal("Descripcion")) ? null : reader.GetString(reader.GetOrdinal("Descripcion")),
                        Activo = reader.IsDBNull(reader.GetOrdinal("Activo")) ? null : reader.GetBoolean(reader.GetOrdinal("Activo")),
                    };
                        }
                    }
                }
            }

            return resultado;
        }

        public async Task<Medicamento> CreateAsync(Medicamento entity)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    INSERT INTO Medicamentos (NombreMedicamento, PrincipioActivo, Presentacion, Concentracion, Descripcion, Activo)
                    OUTPUT INSERTED.IdMedicamento
                    VALUES (@NombreMedicamento, @PrincipioActivo, @Presentacion, @Concentracion, @Descripcion, @Activo)";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@NombreMedicamento", (object?)entity.NombreMedicamento ?? DBNull.Value);
                    command.Parameters.AddWithValue("@PrincipioActivo", (object?)entity.PrincipioActivo ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Presentacion", (object?)entity.Presentacion ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Concentracion", (object?)entity.Concentracion ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Descripcion", (object?)entity.Descripcion ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Activo", (object?)entity.Activo ?? DBNull.Value);

                    await connection.OpenAsync();
                    entity.IdMedicamento = Convert.ToInt32(await command.ExecuteScalarAsync());
                }
            }

            return entity;
        }

        public async Task<bool> UpdateAsync(int id, Medicamento entity)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    UPDATE Medicamentos
                    SET NombreMedicamento = @NombreMedicamento,
                        PrincipioActivo = @PrincipioActivo,
                        Presentacion = @Presentacion,
                        Concentracion = @Concentracion,
                        Descripcion = @Descripcion,
                        Activo = @Activo
                    WHERE IdMedicamento = @IdMedicamento";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdMedicamento", id);
                    command.Parameters.AddWithValue("@NombreMedicamento", (object?)entity.NombreMedicamento ?? DBNull.Value);
                    command.Parameters.AddWithValue("@PrincipioActivo", (object?)entity.PrincipioActivo ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Presentacion", (object?)entity.Presentacion ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Concentracion", (object?)entity.Concentracion ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Descripcion", (object?)entity.Descripcion ?? DBNull.Value);
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
                // Baja logica: la tabla maneja un indicador Activo
                var query = @"
                    UPDATE Medicamentos
                    SET Activo = 0
                    WHERE IdMedicamento = @IdMedicamento";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdMedicamento", id);

                    await connection.OpenAsync();
                    var rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
        }
    }
}
