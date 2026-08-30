using System.ComponentModel.DataAnnotations;
namespace WebApi.Models
{
    public class Cita
    {
        public int IdCita { get; set; }
        public int IdPaciente { get; set; }
        public DateTime? FechaHoraInicio { get; set; }
        public DateTime? FechaHoraFin { get; set; }
        [Required(ErrorMessage = "TipoCita es obligatorio.")]
        [StringLength(100, ErrorMessage = "TipoCita no puede superar los 100 caracteres.")]
        public string? TipoCita { get; set; }
        [Required(ErrorMessage = "Motivo es obligatorio.")]
        [StringLength(500, ErrorMessage = "Motivo no puede superar los 500 caracteres.")]
        public string? Motivo { get; set; }
        [StringLength(50, ErrorMessage = "Modalidad no puede superar los 50 caracteres.")]
        public string? Modalidad { get; set; }
        [StringLength(300, ErrorMessage = "Lugar no puede superar los 300 caracteres.")]
        public string? Lugar { get; set; }
        [StringLength(30, ErrorMessage = "EstadoCita no puede superar los 30 caracteres.")]
        public string? EstadoCita { get; set; }
        [StringLength(1000, ErrorMessage = "Observaciones no puede superar los 1000 caracteres.")]
        public string? Observaciones { get; set; }
        public DateTime? FechaCreacion { get; set; }
    }
}
