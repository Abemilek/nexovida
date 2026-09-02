using System.ComponentModel.DataAnnotations;

namespace WebApi.Dto
{
    public class TratamientoCreateDto
    {
        [Required]
        public int IdPaciente { get; set; }

        [Required]
        public int IdProfesional { get; set; }

        [Required]
        public int IdEnfermedad { get; set; }

        [Required]
        [StringLength(200)]
        public string NombreTratamiento { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Indicaciones { get; set; }

        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }

        [StringLength(30)]
        public string? EstadoTratamiento { get; set; }

        [StringLength(1000)]
        public string? Observaciones { get; set; }
    }

    public class TratamientoUpdateDto
    {
        [Required]
        [StringLength(200)]
        public string NombreTratamiento { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Indicaciones { get; set; }

        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }

        [StringLength(30)]
        public string? EstadoTratamiento { get; set; }

        [StringLength(1000)]
        public string? Observaciones { get; set; }
    }
}
