using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using WebApi.Interface;
using WebApi.Models;

namespace WebApi.Implementation
{
    public class PerfilService : IPerfilService
    {
        private readonly string _connectionString;

        public PerfilService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection")
                ?? throw new InvalidOperationException("La cadena de conexión no puede ser nula.");
        }

        public async Task<IEnumerable<Perfil>> GetAllAsync()
        {
            var lista = new List<Perfil>();

            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT IdPerfil, IdUsuario, Nombres, Apellidos, FechaNacimiento, Sexo, Telefono, Direccion, FotoPerfil, ContactoEmergenciaNombre, ContactoEmergenciaTelefono
                    FROM Perfil
                    ORDER BY IdPerfil DESC";

                using (var command = new SqlCommand(query, connection))
                {
                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            lista.Add(
                    new Perfil
                    {
                        IdPerfil = reader.GetInt32(reader.GetOrdinal("IdPerfil")),
                        IdUsuario = reader.GetInt32(reader.GetOrdinal("IdUsuario")),
                        Nombres = reader.IsDBNull(reader.GetOrdinal("Nombres")) ? null : reader.GetString(reader.GetOrdinal("Nombres")),
                        Apellidos = reader.IsDBNull(reader.GetOrdinal("Apellidos")) ? null : reader.GetString(reader.GetOrdinal("Apellidos")),
                        FechaNacimiento = reader.IsDBNull(reader.GetOrdinal("FechaNacimiento")) ? null : reader.GetDateTime(reader.GetOrdinal("FechaNacimiento")),
                        Sexo = reader.IsDBNull(reader.GetOrdinal("Sexo")) ? null : reader.GetString(reader.GetOrdinal("Sexo")),
                        Telefono = reader.IsDBNull(reader.GetOrdinal("Telefono")) ? null : reader.GetString(reader.GetOrdinal("Telefono")),
                        Direccion = reader.IsDBNull(reader.GetOrdinal("Direccion")) ? null : reader.GetString(reader.GetOrdinal("Direccion")),
                        FotoPerfil = reader.IsDBNull(reader.GetOrdinal("FotoPerfil")) ? null : reader.GetString(reader.GetOrdinal("FotoPerfil")),
                        ContactoEmergenciaNombre = reader.IsDBNull(reader.GetOrdinal("ContactoEmergenciaNombre")) ? null : reader.GetString(reader.GetOrdinal("ContactoEmergenciaNombre")),
                        ContactoEmergenciaTelefono = reader.IsDBNull(reader.GetOrdinal("ContactoEmergenciaTelefono")) ? null : reader.GetString(reader.GetOrdinal("ContactoEmergenciaTelefono")),
                    }
                            );
                        }
                    }
                }
            }

            return lista;
        }

        public async Task<Perfil?> GetByIdAsync(int id)
        {
            Perfil? resultado = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT IdPerfil, IdUsuario, Nombres, Apellidos, FechaNacimiento, Sexo, Telefono, Direccion, FotoPerfil, ContactoEmergenciaNombre, ContactoEmergenciaTelefono
                    FROM Perfil
                    WHERE IdPerfil = @IdPerfil";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdPerfil", id);

                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            resultado =
                    new Perfil
                    {
                        IdPerfil = reader.GetInt32(reader.GetOrdinal("IdPerfil")),
                        IdUsuario = reader.GetInt32(reader.GetOrdinal("IdUsuario")),
                        Nombres = reader.IsDBNull(reader.GetOrdinal("Nombres")) ? null : reader.GetString(reader.GetOrdinal("Nombres")),
                        Apellidos = reader.IsDBNull(reader.GetOrdinal("Apellidos")) ? null : reader.GetString(reader.GetOrdinal("Apellidos")),
                        FechaNacimiento = reader.IsDBNull(reader.GetOrdinal("FechaNacimiento")) ? null : reader.GetDateTime(reader.GetOrdinal("FechaNacimiento")),
                        Sexo = reader.IsDBNull(reader.GetOrdinal("Sexo")) ? null : reader.GetString(reader.GetOrdinal("Sexo")),
                        Telefono = reader.IsDBNull(reader.GetOrdinal("Telefono")) ? null : reader.GetString(reader.GetOrdinal("Telefono")),
                        Direccion = reader.IsDBNull(reader.GetOrdinal("Direccion")) ? null : reader.GetString(reader.GetOrdinal("Direccion")),
                        FotoPerfil = reader.IsDBNull(reader.GetOrdinal("FotoPerfil")) ? null : reader.GetString(reader.GetOrdinal("FotoPerfil")),
                        ContactoEmergenciaNombre = reader.IsDBNull(reader.GetOrdinal("ContactoEmergenciaNombre")) ? null : reader.GetString(reader.GetOrdinal("ContactoEmergenciaNombre")),
                        ContactoEmergenciaTelefono = reader.IsDBNull(reader.GetOrdinal("ContactoEmergenciaTelefono")) ? null : reader.GetString(reader.GetOrdinal("ContactoEmergenciaTelefono")),
                    };
                        }
                    }
                }
            }

            return resultado;
        }

        public async Task<Perfil> CreateAsync(Perfil entity)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    INSERT INTO Perfil (IdUsuario, Nombres, Apellidos, FechaNacimiento, Sexo, Telefono, Direccion, FotoPerfil, ContactoEmergenciaNombre, ContactoEmergenciaTelefono)
                    OUTPUT INSERTED.IdPerfil
                    VALUES (@IdUsuario, @Nombres, @Apellidos, @FechaNacimiento, @Sexo, @Telefono, @Direccion, @FotoPerfil, @ContactoEmergenciaNombre, @ContactoEmergenciaTelefono)";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdUsuario", (object?)entity.IdUsuario ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Nombres", (object?)entity.Nombres ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Apellidos", (object?)entity.Apellidos ?? DBNull.Value);
                    command.Parameters.AddWithValue("@FechaNacimiento", (object?)entity.FechaNacimiento ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Sexo", (object?)entity.Sexo ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Telefono", (object?)entity.Telefono ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Direccion", (object?)entity.Direccion ?? DBNull.Value);
                    command.Parameters.AddWithValue("@FotoPerfil", (object?)entity.FotoPerfil ?? DBNull.Value);
                    command.Parameters.AddWithValue("@ContactoEmergenciaNombre", (object?)entity.ContactoEmergenciaNombre ?? DBNull.Value);
                    command.Parameters.AddWithValue("@ContactoEmergenciaTelefono", (object?)entity.ContactoEmergenciaTelefono ?? DBNull.Value);

                    await connection.OpenAsync();
                    entity.IdPerfil = Convert.ToInt32(await command.ExecuteScalarAsync());
                }
            }

            return entity;
        }

        public async Task<bool> UpdateAsync(int id, Perfil entity)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    UPDATE Perfil
                    SET IdUsuario = @IdUsuario,
                        Nombres = @Nombres,
                        Apellidos = @Apellidos,
                        FechaNacimiento = @FechaNacimiento,
                        Sexo = @Sexo,
                        Telefono = @Telefono,
                        Direccion = @Direccion,
                        FotoPerfil = @FotoPerfil,
                        ContactoEmergenciaNombre = @ContactoEmergenciaNombre,
                        ContactoEmergenciaTelefono = @ContactoEmergenciaTelefono
                    WHERE IdPerfil = @IdPerfil";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdPerfil", id);
                    command.Parameters.AddWithValue("@IdUsuario", (object?)entity.IdUsuario ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Nombres", (object?)entity.Nombres ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Apellidos", (object?)entity.Apellidos ?? DBNull.Value);
                    command.Parameters.AddWithValue("@FechaNacimiento", (object?)entity.FechaNacimiento ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Sexo", (object?)entity.Sexo ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Telefono", (object?)entity.Telefono ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Direccion", (object?)entity.Direccion ?? DBNull.Value);
                    command.Parameters.AddWithValue("@FotoPerfil", (object?)entity.FotoPerfil ?? DBNull.Value);
                    command.Parameters.AddWithValue("@ContactoEmergenciaNombre", (object?)entity.ContactoEmergenciaNombre ?? DBNull.Value);
                    command.Parameters.AddWithValue("@ContactoEmergenciaTelefono", (object?)entity.ContactoEmergenciaTelefono ?? DBNull.Value);

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
                    DELETE FROM Perfil
                    WHERE IdPerfil = @IdPerfil";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdPerfil", id);

                    await connection.OpenAsync();
                    var rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
        }
    }
}
