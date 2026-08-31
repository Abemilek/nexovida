using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using WebApi.Interface;
using WebApi.Models;

namespace WebApi.Implementation
{
    public class TipoIndicadorSaludService : ITipoIndicadorSaludService
    {
        private readonly string _connectionString;

        public TipoIndicadorSaludService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection")
                ?? throw new InvalidOperationException("La cadena de conexión no puede ser nula.");
        }

        public async Task<IEnumerable<TipoIndicadorSalud>> GetAllAsync()
        {
            var lista = new List<TipoIndicadorSalud>();

            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT IdTipoIndicador, NombreIndicador, UnidadMedida, Descripcion, Activo
                    FROM TipoIndicadorSalud
                    ORDER BY IdTipoIndicador DESC";

                using (var command = new SqlCommand(query, connection))
                {
                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            lista.Add(
                    new TipoIndicadorSalud
                    {
                        IdTipoIndicador = reader.GetInt32(reader.GetOrdinal("IdTipoIndicador")),
                        NombreIndicador = reader.IsDBNull(reader.GetOrdinal("NombreIndicador")) ? null : reader.GetString(reader.GetOrdinal("NombreIndicador")),
                        UnidadMedida = reader.IsDBNull(reader.GetOrdinal("UnidadMedida")) ? null : reader.GetString(reader.GetOrdinal("UnidadMedida")),
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

        public async Task<TipoIndicadorSalud?> GetByIdAsync(int id)
        {
            TipoIndicadorSalud? resultado = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT IdTipoIndicador, NombreIndicador, UnidadMedida, Descripcion, Activo
                    FROM TipoIndicadorSalud
                    WHERE IdTipoIndicador = @IdTipoIndicador";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdTipoIndicador", id);

                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            resultado =
                    new TipoIndicadorSalud
                    {
                        IdTipoIndicador = reader.GetInt32(reader.GetOrdinal("IdTipoIndicador")),
                        NombreIndicador = reader.IsDBNull(reader.GetOrdinal("NombreIndicador")) ? null : reader.GetString(reader.GetOrdinal("NombreIndicador")),
                        UnidadMedida = reader.IsDBNull(reader.GetOrdinal("UnidadMedida")) ? null : reader.GetString(reader.GetOrdinal("UnidadMedida")),
                        Descripcion = reader.IsDBNull(reader.GetOrdinal("Descripcion")) ? null : reader.GetString(reader.GetOrdinal("Descripcion")),
                        Activo = reader.IsDBNull(reader.GetOrdinal("Activo")) ? null : reader.GetBoolean(reader.GetOrdinal("Activo")),
                    };
                        }
                    }
                }
            }

            return resultado;
        }

        public async Task<TipoIndicadorSalud> CreateAsync(TipoIndicadorSalud entity)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    INSERT INTO TipoIndicadorSalud (NombreIndicador, UnidadMedida, Descripcion, Activo)
                    OUTPUT INSERTED.IdTipoIndicador
                    VALUES (@NombreIndicador, @UnidadMedida, @Descripcion, @Activo)";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@NombreIndicador", (object?)entity.NombreIndicador ?? DBNull.Value);
                    command.Parameters.AddWithValue("@UnidadMedida", (object?)entity.UnidadMedida ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Descripcion", (object?)entity.Descripcion ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Activo", (object?)entity.Activo ?? DBNull.Value);

                    await connection.OpenAsync();
                    entity.IdTipoIndicador = Convert.ToInt32(await command.ExecuteScalarAsync());
                }
            }

            return entity;
        }

        public async Task<bool> UpdateAsync(int id, TipoIndicadorSalud entity)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    UPDATE TipoIndicadorSalud
                    SET NombreIndicador = @NombreIndicador,
                        UnidadMedida = @UnidadMedida,
                        Descripcion = @Descripcion,
                        Activo = @Activo
                    WHERE IdTipoIndicador = @IdTipoIndicador";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdTipoIndicador", id);
                    command.Parameters.AddWithValue("@NombreIndicador", (object?)entity.NombreIndicador ?? DBNull.Value);
                    command.Parameters.AddWithValue("@UnidadMedida", (object?)entity.UnidadMedida ?? DBNull.Value);
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
               
                var query = @"
                    UPDATE TipoIndicadorSalud
                    SET Activo = 0
                    WHERE IdTipoIndicador = @IdTipoIndicador";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdTipoIndicador", id);

                    await connection.OpenAsync();
                    var rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
        }
    }
}
