using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using WebApi.Interface;
using WebApi.Models;

namespace WebApi.Implementation
{
    public class EnfermedadService : IEnfermedadService
    {
        private readonly string _connectionString;

        public EnfermedadService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection")
                ?? throw new InvalidOperationException("La cadena de conexión no puede ser nula.");
        }

        public async Task<IEnumerable<Enfermedad>> GetAllAsync()
        {
            var lista = new List<Enfermedad>();

            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT IdEnfermedad, NombreEnfermedad, Descripcion, EsCronica, Activa
                    FROM Enfermedades
                    ORDER BY IdEnfermedad DESC";

                using (var command = new SqlCommand(query, connection))
                {
                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            lista.Add(
                    new Enfermedad
                    {
                        IdEnfermedad = reader.GetInt32(reader.GetOrdinal("IdEnfermedad")),
                        NombreEnfermedad = reader.IsDBNull(reader.GetOrdinal("NombreEnfermedad")) ? null : reader.GetString(reader.GetOrdinal("NombreEnfermedad")),
                        Descripcion = reader.IsDBNull(reader.GetOrdinal("Descripcion")) ? null : reader.GetString(reader.GetOrdinal("Descripcion")),
                        EsCronica = reader.IsDBNull(reader.GetOrdinal("EsCronica")) ? null : reader.GetBoolean(reader.GetOrdinal("EsCronica")),
                        Activa = reader.IsDBNull(reader.GetOrdinal("Activa")) ? null : reader.GetBoolean(reader.GetOrdinal("Activa")),
                    }
                            );
                        }
                    }
                }
            }

            return lista;
        }

        public async Task<Enfermedad?> GetByIdAsync(int id)
        {
            Enfermedad? resultado = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT IdEnfermedad, NombreEnfermedad, Descripcion, EsCronica, Activa
                    FROM Enfermedades
                    WHERE IdEnfermedad = @IdEnfermedad";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdEnfermedad", id);

                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            resultado =
                    new Enfermedad
                    {
                        IdEnfermedad = reader.GetInt32(reader.GetOrdinal("IdEnfermedad")),
                        NombreEnfermedad = reader.IsDBNull(reader.GetOrdinal("NombreEnfermedad")) ? null : reader.GetString(reader.GetOrdinal("NombreEnfermedad")),
                        Descripcion = reader.IsDBNull(reader.GetOrdinal("Descripcion")) ? null : reader.GetString(reader.GetOrdinal("Descripcion")),
                        EsCronica = reader.IsDBNull(reader.GetOrdinal("EsCronica")) ? null : reader.GetBoolean(reader.GetOrdinal("EsCronica")),
                        Activa = reader.IsDBNull(reader.GetOrdinal("Activa")) ? null : reader.GetBoolean(reader.GetOrdinal("Activa")),
                    };
                        }
                    }
                }
            }

            return resultado;
        }

        public async Task<Enfermedad> CreateAsync(Enfermedad entity)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    INSERT INTO Enfermedades (NombreEnfermedad, Descripcion, EsCronica, Activa)
                    OUTPUT INSERTED.IdEnfermedad
                    VALUES (@NombreEnfermedad, @Descripcion, @EsCronica, @Activa)";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@NombreEnfermedad", (object?)entity.NombreEnfermedad ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Descripcion", (object?)entity.Descripcion ?? DBNull.Value);
                    command.Parameters.AddWithValue("@EsCronica", (object?)entity.EsCronica ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Activa", (object?)entity.Activa ?? DBNull.Value);

                    await connection.OpenAsync();
                    entity.IdEnfermedad = Convert.ToInt32(await command.ExecuteScalarAsync());
                }
            }

            return entity;
        }

        public async Task<bool> UpdateAsync(int id, Enfermedad entity)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    UPDATE Enfermedades
                    SET NombreEnfermedad = @NombreEnfermedad,
                        Descripcion = @Descripcion,
                        EsCronica = @EsCronica,
                        Activa = @Activa
                    WHERE IdEnfermedad = @IdEnfermedad";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdEnfermedad", id);
                    command.Parameters.AddWithValue("@NombreEnfermedad", (object?)entity.NombreEnfermedad ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Descripcion", (object?)entity.Descripcion ?? DBNull.Value);
                    command.Parameters.AddWithValue("@EsCronica", (object?)entity.EsCronica ?? DBNull.Value);
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
                    UPDATE Enfermedades
                    SET Activa = 0
                    WHERE IdEnfermedad = @IdEnfermedad";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdEnfermedad", id);

                    await connection.OpenAsync();
                    var rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
        }
    }
}
