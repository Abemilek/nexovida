using System.ComponentModel.DataAnnotations;
namespace WebApi.Models
{
    public class Recordatorio
    {
        public int IdRecordatorio { get; set; }
        public int IdPaciente { get; set; }
        public int? IdTratamientoMedicamento { get; set; }
        public int? IdCita { get; set; }
        [Required(ErrorMessage = "Titulo es obligatorio.")]
        [StringLength(200, ErrorMessage = "Titulo no puede superar los 200 caracteres.")]
        public string? Titulo { get; set; }
        [StringLength(500, ErrorMessage = "Descripcion no puede superar los 500 caracteres.")]
        public string? Descripcion { get; set; }
        [StringLength(50, ErrorMessage = "TipoRecordatorio no puede superar los 50 caracteres.")]
        public string? TipoRecordatorio { get; set; }
        public DateTime? FechaHoraProgramada { get; set; }
        public bool? Repetir { get; set; }
        [StringLength(100, ErrorMessage = "FrecuenciaRepeticion no puede superar los 100 caracteres.")]
        public string? FrecuenciaRepeticion { get; set; }
        [StringLength(30, ErrorMessage = "EstadoRecordatorio no puede superar los 30 caracteres.")]
        public string? EstadoRecordatorio { get; set; }
        public DateTime? FechaCompletado { get; set; }
        public bool? Activo { get; set; }
    }
}
