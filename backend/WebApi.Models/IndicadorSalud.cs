using System.ComponentModel.DataAnnotations;
namespace WebApi.Models
{
    public class IndicadorSalud
    {
        public long IdIndicadorSalud { get; set; }
        public int IdPaciente { get; set; }
        public int IdTipoIndicador { get; set; }
        [Range(0, 9999, ErrorMessage = "Valor debe estar entre 0 y 9999.")]
        public decimal? Valor { get; set; }
        public decimal? ValorSecundario { get; set; }
        public DateTime? FechaHoraMedicion { get; set; }
        public int? IdUsuarioRegistro { get; set; }
        [StringLength(500, ErrorMessage = "Observaciones no puede superar los 500 caracteres.")]
        public string? Observaciones { get; set; }
        [StringLength(100, ErrorMessage = "Fuente no puede superar los 100 caracteres.")]
        public string? Fuente { get; set; }
    }
}
