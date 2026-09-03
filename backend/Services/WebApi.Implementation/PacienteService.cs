using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using WebApi.Interface;
using WebApi.Models;

namespace WebApi.Implementation
{
    public class PacienteService : IPacienteService
    {
        private readonly string _connectionString;

        public PacienteService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection")
                ?? throw new InvalidOperationException("La cadena de conexión no puede ser nula.");
        }

        public async Task<IEnumerable<Paciente>> GetAllAsync()
        {
            var lista = new List<Paciente>();

            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT p.IdPaciente, p.IdUsuario, p.TipoPaciente, p.PorcentajeDiscapacidad, p.NecesidadesEspeciales, p.FechaIngreso, p.EstadoPaciente, u.NombreUsuario
                    FROM Paciente p
                    LEFT JOIN Usuario u ON u.IdUsuario = p.IdUsuario
                    ORDER BY p.IdPaciente DESC";

                using (var command = new SqlCommand(query, connection))
                {
                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            lista.Add(
                    new Paciente
                    {
                        IdPaciente = reader.GetInt32(reader.GetOrdinal("IdPaciente")),
                        IdUsuario = reader.GetInt32(reader.GetOrdinal("IdUsuario")),
                        TipoPaciente = reader.IsDBNull(reader.GetOrdinal("TipoPaciente")) ? null : reader.GetString(reader.GetOrdinal("TipoPaciente")),
                        PorcentajeDiscapacidad = reader.IsDBNull(reader.GetOrdinal("PorcentajeDiscapacidad")) ? null : reader.GetDecimal(reader.GetOrdinal("PorcentajeDiscapacidad")),
                        NecesidadesEspeciales = reader.IsDBNull(reader.GetOrdinal("NecesidadesEspeciales")) ? null : reader.GetString(reader.GetOrdinal("NecesidadesEspeciales")),
                        FechaIngreso = reader.IsDBNull(reader.GetOrdinal("FechaIngreso")) ? null : reader.GetDateTime(reader.GetOrdinal("FechaIngreso")),
                        EstadoPaciente = reader.IsDBNull(reader.GetOrdinal("EstadoPaciente")) ? null : reader.GetString(reader.GetOrdinal("EstadoPaciente")),
                        NombreUsuario = reader.IsDBNull(reader.GetOrdinal("NombreUsuario")) ? null : reader.GetString(reader.GetOrdinal("NombreUsuario")),
                    }
                            );
                        }
                    }
                }
            }

            return lista;
        }

        public async Task<Paciente?> GetByIdAsync(int id)
        {
            Paciente? resultado = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT p.IdPaciente, p.IdUsuario, p.TipoPaciente, p.PorcentajeDiscapacidad, p.NecesidadesEspeciales, p.FechaIngreso, p.EstadoPaciente, u.NombreUsuario
                    FROM Paciente p
                    LEFT JOIN Usuario u ON u.IdUsuario = p.IdUsuario
                    WHERE p.IdPaciente = @IdPaciente";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdPaciente", id);

                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            resultado =
                    new Paciente
                    {
                        IdPaciente = reader.GetInt32(reader.GetOrdinal("IdPaciente")),
                        IdUsuario = reader.GetInt32(reader.GetOrdinal("IdUsuario")),
                        TipoPaciente = reader.IsDBNull(reader.GetOrdinal("TipoPaciente")) ? null : reader.GetString(reader.GetOrdinal("TipoPaciente")),
                        PorcentajeDiscapacidad = reader.IsDBNull(reader.GetOrdinal("PorcentajeDiscapacidad")) ? null : reader.GetDecimal(reader.GetOrdinal("PorcentajeDiscapacidad")),
                        NecesidadesEspeciales = reader.IsDBNull(reader.GetOrdinal("NecesidadesEspeciales")) ? null : reader.GetString(reader.GetOrdinal("NecesidadesEspeciales")),
                        FechaIngreso = reader.IsDBNull(reader.GetOrdinal("FechaIngreso")) ? null : reader.GetDateTime(reader.GetOrdinal("FechaIngreso")),
                        EstadoPaciente = reader.IsDBNull(reader.GetOrdinal("EstadoPaciente")) ? null : reader.GetString(reader.GetOrdinal("EstadoPaciente")),
                        NombreUsuario = reader.IsDBNull(reader.GetOrdinal("NombreUsuario")) ? null : reader.GetString(reader.GetOrdinal("NombreUsuario")),
                    };
                        }
                    }
                }
            }

            return resultado;
        }

        public async Task<Paciente> CreateAsync(Paciente entity)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    INSERT INTO Paciente (IdUsuario, TipoPaciente, PorcentajeDiscapacidad, NecesidadesEspeciales, FechaIngreso, EstadoPaciente)
                    OUTPUT INSERTED.IdPaciente
                    VALUES (@IdUsuario, @TipoPaciente, @PorcentajeDiscapacidad, @NecesidadesEspeciales, @FechaIngreso, @EstadoPaciente)";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdUsuario", (object?)entity.IdUsuario ?? DBNull.Value);
                    command.Parameters.AddWithValue("@TipoPaciente", (object?)entity.TipoPaciente ?? DBNull.Value);
                    command.Parameters.AddWithValue("@PorcentajeDiscapacidad", (object?)entity.PorcentajeDiscapacidad ?? DBNull.Value);
                    command.Parameters.AddWithValue("@NecesidadesEspeciales", (object?)entity.NecesidadesEspeciales ?? DBNull.Value);
                    command.Parameters.AddWithValue("@FechaIngreso", (object?)entity.FechaIngreso ?? DBNull.Value);
                    command.Parameters.AddWithValue("@EstadoPaciente", (object?)entity.EstadoPaciente ?? DBNull.Value);

                    await connection.OpenAsync();
                    entity.IdPaciente = Convert.ToInt32(await command.ExecuteScalarAsync());
                }
            }

            return entity;
        }

        public async Task<bool> UpdateAsync(int id, Paciente entity)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"
                    UPDATE Paciente
                    SET IdUsuario = @IdUsuario,
                        TipoPaciente = @TipoPaciente,
                        PorcentajeDiscapacidad = @PorcentajeDiscapacidad,
                        NecesidadesEspeciales = @NecesidadesEspeciales,
                        FechaIngreso = @FechaIngreso,
                        EstadoPaciente = @EstadoPaciente
                    WHERE IdPaciente = @IdPaciente";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdPaciente", id);
                    command.Parameters.AddWithValue("@IdUsuario", (object?)entity.IdUsuario ?? DBNull.Value);
                    command.Parameters.AddWithValue("@TipoPaciente", (object?)entity.TipoPaciente ?? DBNull.Value);
                    command.Parameters.AddWithValue("@PorcentajeDiscapacidad", (object?)entity.PorcentajeDiscapacidad ?? DBNull.Value);
                    command.Parameters.AddWithValue("@NecesidadesEspeciales", (object?)entity.NecesidadesEspeciales ?? DBNull.Value);
                    command.Parameters.AddWithValue("@FechaIngreso", (object?)entity.FechaIngreso ?? DBNull.Value);
                    command.Parameters.AddWithValue("@EstadoPaciente", (object?)entity.EstadoPaciente ?? DBNull.Value);

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
                    DELETE FROM Paciente
                    WHERE IdPaciente = @IdPaciente";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdPaciente", id);

                    await connection.OpenAsync();
                    var rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
        }
    }
}
