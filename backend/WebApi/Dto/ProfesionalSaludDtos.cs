using System.ComponentModel.DataAnnotations;

namespace WebApi.Dto
{
    public class ProfesionalSaludCreateDto
    {
        [Required]
        public int IdUsuario { get; set; }

        [Required]
        [StringLength(150)]
        public string Especialidad { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string NumeroLicencia { get; set; } = string.Empty;

        [StringLength(200)]
        public string? CentroSalud { get; set; }

        [StringLength(30)]
        public string? TelefonoProfesional { get; set; }
    }

    public class ProfesionalSaludUpdateDto
    {
        [Required]
        [StringLength(150)]
        public string Especialidad { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string NumeroLicencia { get; set; } = string.Empty;

        [StringLength(200)]
        public string? CentroSalud { get; set; }

        [StringLength(30)]
        public string? TelefonoProfesional { get; set; }
    }
}
