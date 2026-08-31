using System.ComponentModel.DataAnnotations;

namespace WebApi.Dto
{
    public class RecordatorioCreateDto
    {
        [Required]
        public int IdPaciente { get; set; }

        public int? IdTratamientoMedicamento { get; set; }
        public int? IdCita { get; set; }

        [Required]
        [StringLength(200)]
        public string Titulo { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Descripcion { get; set; }

        [StringLength(50)]
        public string? TipoRecordatorio { get; set; }

        public DateTime? FechaHoraProgramada { get; set; }
        public bool? Repetir { get; set; }

        [StringLength(100)]
        public string? FrecuenciaRepeticion { get; set; }
    }

    public class RecordatorioUpdateDto
    {
        public int? IdTratamientoMedicamento { get; set; }
        public int? IdCita { get; set; }

        [Required]
        [StringLength(200)]
        public string Titulo { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Descripcion { get; set; }

        [StringLength(50)]
        public string? TipoRecordatorio { get; set; }

        public DateTime? FechaHoraProgramada { get; set; }
        public bool? Repetir { get; set; }

        [StringLength(100)]
        public string? FrecuenciaRepeticion { get; set; }

        [StringLength(30)]
        public string? EstadoRecordatorio { get; set; }

        public DateTime? FechaCompletado { get; set; }
        public bool? Activo { get; set; }
    }
}
