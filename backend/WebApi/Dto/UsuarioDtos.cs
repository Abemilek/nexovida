using System.ComponentModel.DataAnnotations;

namespace WebApi.Dto
{
    public class UsuarioCreateDto
    {
        [Required]
        [StringLength(100)]
        public string NombreUsuario { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(200)]
        public string Correo { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$", ErrorMessage = "La contraseña debe tener al menos 8 caracteres, e incluir mayúsculas, minúsculas, números y caracteres especiales.")]
        public string Password { get; set; } = string.Empty;
    }

    public class UsuarioUpdateDto
    {
        [Required]
        [StringLength(100)]
        public string NombreUsuario { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(200)]
        public string Correo { get; set; } = string.Empty;

        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$", ErrorMessage = "La contraseña debe tener al menos 8 caracteres, e incluir mayúsculas, minúsculas, números y caracteres especiales.")]
        public string? Password { get; set; }

        public bool? Activo { get; set; }
    }
}
