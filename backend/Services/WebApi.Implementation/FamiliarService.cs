using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using WebApi.Interface;
using WebApi.Models;

namespace WebApi.Implementation
{
    public class FamiliarService : IFamiliarService
    {
        private readonly string _connectionString;

        public FamiliarService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection")
                ?? throw new InvalidOperationException("La cadena de conexión no puede ser nula.");
        }

        public async Task<IEnumerable<Familiar>> GetAllAsync()
        {
            var lista = new List<Familiar>();

            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT IdFamiliar, IdUsuario, Parentesco
                    FROM Familiares
                    ORDER BY IdFamiliar DESC";

                using (var command = new SqlCommand(query, connection))
                {
                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            lista.Add(
                    new Familiar
                    {
                        IdFamiliar = reader.GetInt32(reader.GetOrdinal("IdFamiliar")),
                        IdUsuario = reader.GetInt32(reader.GetOrdinal("IdUsuario")),
                        Parentesco = reader.IsDBNull(reader.GetOrdinal("Parentesco")) ? null : reader.GetString(reader.GetOrdinal("Parentesco")),
                    }
                            );
                        }
                    }
                }
            }

            return lista;
        }

        public async Task<Familiar?> GetByIdAsync(int id)
        {
            Familiar? resultado = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT IdFamiliar, IdUsuario, Parentesco
                    FROM Familiares
                    WHERE IdFamiliar = @IdFamiliar";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdFamiliar", id);

                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            resultado =
                    new Familiar
                    {
                        IdFamiliar = reader.GetInt32(reader.GetOrdinal("IdFamiliar")),
                        IdUsuario = reader.GetInt32(reader.GetOrdinal("IdUsuario")),
                        Parentesco = reader.IsDBNull(reader.GetOrdinal("Parentesco")) ? null : reader.GetString(reader.GetOrdinal("Parentesco")),
                    };
                        }
                    }
                }
            }

            return resultado;
        }

        public async Task<Familiar> CreateAsync(Familiar entity)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    INSERT INTO Familiares (IdUsuario, Parentesco)
                    OUTPUT INSERTED.IdFamiliar
                    VALUES (@IdUsuario, @Parentesco)";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdUsuario", (object?)entity.IdUsuario ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Parentesco", (object?)entity.Parentesco ?? DBNull.Value);

                    await connection.OpenAsync();
                    entity.IdFamiliar = Convert.ToInt32(await command.ExecuteScalarAsync());
                }
            }

            return entity;
        }

        public async Task<bool> UpdateAsync(int id, Familiar entity)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    UPDATE Familiares
                    SET IdUsuario = @IdUsuario,
                        Parentesco = @Parentesco
                    WHERE IdFamiliar = @IdFamiliar";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdFamiliar", id);
                    command.Parameters.AddWithValue("@IdUsuario", (object?)entity.IdUsuario ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Parentesco", (object?)entity.Parentesco ?? DBNull.Value);

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
                    DELETE FROM Familiares
                    WHERE IdFamiliar = @IdFamiliar";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdFamiliar", id);

                    await connection.OpenAsync();
                    var rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
        }
    }
}
