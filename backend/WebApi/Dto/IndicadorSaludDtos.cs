using System.ComponentModel.DataAnnotations;

namespace WebApi.Dto
{
    public class IndicadorSaludCreateDto
    {
        [Required]
        public int IdPaciente { get; set; }

        [Required]
        public int IdTipoIndicador { get; set; }

        [Range(0, 9999)]
        public decimal? Valor { get; set; }
        
        public decimal? ValorSecundario { get; set; }
        public DateTime? FechaHoraMedicion { get; set; }
        public int? IdUsuarioRegistro { get; set; }

        [StringLength(500)]
        public string? Observaciones { get; set; }

        [StringLength(100)]
        public string? Fuente { get; set; }
    }

    public class IndicadorSaludUpdateDto
    {
        [Required]
        public int IdTipoIndicador { get; set; }

        [Range(0, 9999)]
        public decimal? Valor { get; set; }
        
        public decimal? ValorSecundario { get; set; }
        public DateTime? FechaHoraMedicion { get; set; }
        public int? IdUsuarioRegistro { get; set; }

        [StringLength(500)]
        public string? Observaciones { get; set; }

        [StringLength(100)]
        public string? Fuente { get; set; }
    }
}
