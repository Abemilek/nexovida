using System;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace WebApi.Helpers
{
    public static class DatabaseInitializer
    {
        public static void Initialize(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            using var connection = new SqlConnection(connectionString);
            connection.Open();

            var checkCmd = new SqlCommand("SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'UsuarioSeguridad'", connection);
            int exists = (int)checkCmd.ExecuteScalar();
            
            if (exists == 0)
            {
                var createTablesCmd = new SqlCommand(@"
                    CREATE TABLE UsuarioSeguridad (
                        IdUsuario INT PRIMARY KEY,
                        PasswordHash VARCHAR(255) NOT NULL,
                        PasswordSalt VARCHAR(255) NOT NULL,
                        TwoFactorEnabled BIT DEFAULT 0,
                        TwoFactorSecret VARCHAR(255),
                        FOREIGN KEY (IdUsuario) REFERENCES Usuario(IdUsuario)
                    );

                    CREATE TABLE RefreshTokens (
                        Id INT IDENTITY(1,1) PRIMARY KEY,
                        IdUsuario INT NOT NULL,
                        TokenHash VARCHAR(255) NOT NULL,
                        ExpiresUtc DATETIME NOT NULL,
                        CreatedUtc DATETIME DEFAULT GETUTCDATE(),
                        RevokedUtc DATETIME,
                        FOREIGN KEY (IdUsuario) REFERENCES Usuario(IdUsuario)
                    );
                ", connection);
                createTablesCmd.ExecuteNonQuery();

                var createSpsCmd = new SqlCommand(@"
                    CREATE PROCEDURE sp_Usuario_ObtenerTodos AS BEGIN SELECT u.IdUsuario, u.NombreUsuario, u.Correo, u.Activo, u.FechaRegistro, u.UltimoAcceso FROM Usuario u; END;
                ", connection);
            }
        }
    }
}
