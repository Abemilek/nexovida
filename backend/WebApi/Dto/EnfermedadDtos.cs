using System.ComponentModel.DataAnnotations;

namespace WebApi.Dto
{
    public class EnfermedadCreateDto
    {
        [Required]
        [StringLength(200)]
        public string NombreEnfermedad { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Descripcion { get; set; }

        public bool? EsCronica { get; set; }
    }

    public class EnfermedadUpdateDto
    {
        [Required]
        [StringLength(200)]
        public string NombreEnfermedad { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Descripcion { get; set; }

        public bool? EsCronica { get; set; }
        public bool? Activa { get; set; }
    }
}
