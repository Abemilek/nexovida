using System.ComponentModel.DataAnnotations;
namespace WebApi.Models
{
    public class Paciente
    {
        public int IdPaciente { get; set; }
        public int IdUsuario { get; set; }
        [Required(ErrorMessage = "TipoPaciente es obligatorio.")]
        [StringLength(50, ErrorMessage = "TipoPaciente no puede superar los 50 caracteres.")]
        public string? TipoPaciente { get; set; }
        [Range(0, 100, ErrorMessage = "PorcentajeDiscapacidad debe estar entre 0 y 100.")]
        public decimal? PorcentajeDiscapacidad { get; set; }
        [StringLength(500, ErrorMessage = "NecesidadesEspeciales no puede superar los 500 caracteres.")]
        public string? NecesidadesEspeciales { get; set; }
        public DateTime? FechaIngreso { get; set; }
        [StringLength(30, ErrorMessage = "EstadoPaciente no puede superar los 30 caracteres.")]
        public string? EstadoPaciente { get; set; }

        [System.Text.Json.Serialization.JsonIgnore(Condition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull)]
        public string? NombreUsuario { get; set; }
    }
}
