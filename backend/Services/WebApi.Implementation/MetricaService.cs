using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using WebApi.Interface;

namespace WebApi.Implementation
{
    public class MetricaService : IMetricaService
    {
        private readonly string _connectionString;

        public MetricaService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection")
                ?? throw new InvalidOperationException("La cadena de conexión no puede ser nula.");
        }

        public async Task<object> GetGlobalMetricsAsync()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var totalPacientes = await CountAsync(connection, "Paciente");
                var totalProfesionales = await CountAsync(connection, "ProfesionalSalud");
                var totalRecordatorios = await CountAsync(connection, "Recordatorios");
                var totalIndicadores = await CountAsync(connection, "IndicadorSalud");

                return new
                {
                    TotalPacientes = totalPacientes,
                    TotalProfesionales = totalProfesionales,
                    TotalRecordatorios = totalRecordatorios,
                    TotalIndicadores = totalIndicadores
                };
            }
        }

        private static async Task<int> CountAsync(SqlConnection connection, string tableName)
        {
            using (var cmd = new SqlCommand($"SELECT COUNT(*) FROM {tableName}", connection))
            {
                var result = await cmd.ExecuteScalarAsync();
                return Convert.ToInt32(result);
            }
        }
    }
}
