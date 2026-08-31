using System.ComponentModel.DataAnnotations;

namespace WebApi.Dto
{
    public class AlertaCreateDto
    {
        [Required]
        public int IdPaciente { get; set; }

        public long? IdIndicadorSalud { get; set; }
        public int? IdRecordatorio { get; set; }

        [Required]
        [StringLength(200)]
        public string Titulo { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string Mensaje { get; set; } = string.Empty;

        [StringLength(50)]
        public string? TipoAlerta { get; set; }

        [StringLength(30)]
        public string? NivelPrioridad { get; set; }
    }

    public class AlertaUpdateDto
    {
        [Required]
        [StringLength(200)]
        public string Titulo { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string Mensaje { get; set; } = string.Empty;

        [StringLength(50)]
        public string? TipoAlerta { get; set; }

        [StringLength(30)]
        public string? NivelPrioridad { get; set; }
        
        public bool? Atendida { get; set; }
        public DateTime? FechaLectura { get; set; }
        public DateTime? FechaAtencion { get; set; }
    }
}
