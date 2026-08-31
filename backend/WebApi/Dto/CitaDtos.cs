using System.ComponentModel.DataAnnotations;

namespace WebApi.Dto
{
    public class CitaCreateDto
    {
        [Required]
        public int IdPaciente { get; set; }

        public DateTime? FechaHoraInicio { get; set; }
        public DateTime? FechaHoraFin { get; set; }

        [Required]
        [StringLength(100)]
        public string TipoCita { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Motivo { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Modalidad { get; set; }

        [StringLength(300)]
        public string? Lugar { get; set; }

        [StringLength(30)]
        public string? EstadoCita { get; set; }

        [StringLength(1000)]
        public string? Observaciones { get; set; }
    }

    public class CitaUpdateDto
    {
        public DateTime? FechaHoraInicio { get; set; }
        public DateTime? FechaHoraFin { get; set; }

        [Required]
        [StringLength(100)]
        public string TipoCita { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Motivo { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Modalidad { get; set; }

        [StringLength(300)]
        public string? Lugar { get; set; }

        [StringLength(30)]
        public string? EstadoCita { get; set; }

        [StringLength(1000)]
        public string? Observaciones { get; set; }
    }
}
