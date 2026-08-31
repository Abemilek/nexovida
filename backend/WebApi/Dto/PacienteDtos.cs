using System.ComponentModel.DataAnnotations;

namespace WebApi.Dto
{
    public class PacienteCreateDto
    {
        [Required]
        public int IdUsuario { get; set; }

        [Required]
        [StringLength(50)]
        public string TipoPaciente { get; set; } = string.Empty;

        [Range(0, 100)]
        public decimal? PorcentajeDiscapacidad { get; set; }

        [StringLength(500)]
        public string? NecesidadesEspeciales { get; set; }

        [StringLength(30)]
        public string? EstadoPaciente { get; set; }
    }

    public class PacienteUpdateDto
    {
        [Required]
        [StringLength(50)]
        public string TipoPaciente { get; set; } = string.Empty;

        [Range(0, 100)]
        public decimal? PorcentajeDiscapacidad { get; set; }

        [StringLength(500)]
        public string? NecesidadesEspeciales { get; set; }

        [StringLength(30)]
        public string? EstadoPaciente { get; set; }
    }
}
