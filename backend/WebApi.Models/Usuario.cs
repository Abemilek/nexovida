using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebApi.Models
{
    public class Usuario
    {
        public int IdUsuario { get; set; }
        [Required(ErrorMessage = "NombreUsuario es obligatorio.")]
        [StringLength(100, ErrorMessage = "NombreUsuario no puede superar los 100 caracteres.")]
        public string? NombreUsuario { get; set; }
        [Required(ErrorMessage = "Correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "Correo no tiene un formato valido.")]
        [StringLength(200, ErrorMessage = "Correo no puede superar los 200 caracteres.")]
        public string? Correo { get; set; }

        [MinLength(8, ErrorMessage = "Password debe tener al menos 8 caracteres.")]
        [JsonIgnore]
        public string? Password { get; set; }

        [JsonIgnore]
        public byte[]? Contrasena { get; set; }

        [JsonIgnore]
        public byte[]? Salt { get; set; }

        public DateTime? FechaRegistro { get; set; }
        public DateTime? UltimoAcceso { get; set; }
        public bool? Activo { get; set; }

        public bool? TwoFactorEnabled { get; set; }

        [JsonIgnore]
        public string? TwoFactorSecret { get; set; }
    }
}
