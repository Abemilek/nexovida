using System.ComponentModel.DataAnnotations;
namespace WebApi.Models
{
    public class Alerta
    {
        public long IdAlerta { get; set; }
        public int IdPaciente { get; set; }
        public long? IdIndicadorSalud { get; set; }
        public int? IdRecordatorio { get; set; }
        [Required(ErrorMessage = "Titulo es obligatorio.")]
        [StringLength(200, ErrorMessage = "Titulo no puede superar los 200 caracteres.")]
        public string? Titulo { get; set; }
        [Required(ErrorMessage = "Mensaje es obligatorio.")]
        [StringLength(1000, ErrorMessage = "Mensaje no puede superar los 1000 caracteres.")]
        public string? Mensaje { get; set; }
        [StringLength(50, ErrorMessage = "TipoAlerta no puede superar los 50 caracteres.")]
        public string? TipoAlerta { get; set; }
        [StringLength(30, ErrorMessage = "NivelPrioridad no puede superar los 30 caracteres.")]
        public string? NivelPrioridad { get; set; }
        public DateTime? FechaGeneracion { get; set; }
        public DateTime? FechaLectura { get; set; }
        public bool? Atendida { get; set; }
        public DateTime? FechaAtencion { get; set; }
    }
}
