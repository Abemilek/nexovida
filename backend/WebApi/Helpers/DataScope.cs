using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WebApi.Helpers
{
    public static class DataScope
    {
        public static int? GetUserId(HttpContext context)
        {
            var claim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(claim, out var id) ? id : null;
        }

        public static bool EsAdministrador(HttpContext context) =>
            context.User.IsInRole("Administrador");

        private static string ConnectionString(HttpContext context) =>
            context.RequestServices.GetRequiredService<IConfiguration>()
                .GetConnectionString("DatabaseConnection")
                ?? throw new InvalidOperationException("La cadena de conexión no puede ser nula.");

        public static async Task<HashSet<int>> ObtenerPacientesPermitidosAsync(HttpContext context)
        {
            var permitidos = new HashSet<int>();
            var idUsuario = GetUserId(context);
            if (idUsuario == null)
            {
                return permitidos;
            }

            var connectionString = ConnectionString(context);

            if (context.User.IsInRole("Paciente"))
            {
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();
                using var cmd = new SqlCommand(
                    "SELECT IdPaciente FROM Paciente WHERE IdUsuario = @idUsuario",
                    connection);
                cmd.Parameters.AddWithValue("@idUsuario", idUsuario.Value);
                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    permitidos.Add(reader.GetInt32(0));
                }
                return permitidos;
            }

            if (context.User.IsInRole("ProfesionalSalud"))
            {
                int? idProfesional = null;
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    using var cmd = new SqlCommand(
                        "SELECT IdProfesional FROM ProfesionalSalud WHERE IdUsuario = @idUsuario AND Activo = 1",
                        connection);
                    cmd.Parameters.AddWithValue("@idUsuario", idUsuario.Value);
                    var result = await cmd.ExecuteScalarAsync();
                    if (result != null)
                    {
                        idProfesional = Convert.ToInt32(result);
                    }
                }
                if (idProfesional == null)
                {
                    return permitidos;
                }

                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    using var cmd = new SqlCommand(
                        "SELECT DISTINCT IdPaciente FROM Tratamiento WHERE IdProfesional = @idProfesional",
                        connection);
                    cmd.Parameters.AddWithValue("@idProfesional", idProfesional.Value);
                    using var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        permitidos.Add(reader.GetInt32(0));
                    }
                }
                return permitidos;
            }

            if (context.User.IsInRole("Familiar"))
            {
                int? idFamiliar = null;
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    using var cmd = new SqlCommand(
                        "SELECT IdFamiliar FROM Familiares WHERE IdUsuario = @idUsuario",
                        connection);
                    cmd.Parameters.AddWithValue("@idUsuario", idUsuario.Value);
                    var result = await cmd.ExecuteScalarAsync();
                    if (result != null)
                    {
                        idFamiliar = Convert.ToInt32(result);
                    }
                }
                if (idFamiliar == null)
                {
                    return permitidos;
                }

                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    using var cmd = new SqlCommand(
                        "SELECT IdPaciente FROM AsistentePaciente WHERE IdFamiliar = @idFamiliar AND Activo = 1",
                        connection);
                    cmd.Parameters.AddWithValue("@idFamiliar", idFamiliar.Value);
                    using var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        permitidos.Add(reader.GetInt32(0));
                    }
                }
                return permitidos;
            }

            return permitidos;
        }

        public static async Task<bool> PuedeAccederAPacienteAsync(HttpContext context, int idPaciente)
        {
            var permitidos = await ObtenerPacientesPermitidosAsync(context);
            return permitidos.Contains(idPaciente);
        }
    }
}