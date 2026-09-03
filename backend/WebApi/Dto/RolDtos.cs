using System.ComponentModel.DataAnnotations;

namespace WebApi.Dto
{
    public class RolCreateDto
    {
        [Required]
        [StringLength(50)]
        public string NombreRol { get; set; } = string.Empty;

        [StringLength(250)]
        public string? Descripcion { get; set; }

        public bool? Activo { get; set; } = true;
    }

    public class RolUpdateDto
    {
        [Required]
        [StringLength(50)]
        public string NombreRol { get; set; } = string.Empty;

        [StringLength(250)]
        public string? Descripcion { get; set; }

        public bool? Activo { get; set; }
    }
}
