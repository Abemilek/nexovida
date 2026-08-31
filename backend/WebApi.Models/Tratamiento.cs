using System.ComponentModel.DataAnnotations;
namespace WebApi.Models
{
    public class Tratamiento
    {
        public int IdTratamiento { get; set; }
        public int IdPaciente { get; set; }
        public int IdProfesional { get; set; }
        public int IdEnfermedad { get; set; }
        [Required(ErrorMessage = "NombreTratamiento es obligatorio.")]
        [StringLength(200, ErrorMessage = "NombreTratamiento no puede superar los 200 caracteres.")]
        public string? NombreTratamiento { get; set; }
        [StringLength(1000, ErrorMessage = "Indicaciones no puede superar los 1000 caracteres.")]
        public string? Indicaciones { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        [StringLength(30, ErrorMessage = "EstadoTratamiento no puede superar los 30 caracteres.")]
        public string? EstadoTratamiento { get; set; }
        [StringLength(1000, ErrorMessage = "Observaciones no puede superar los 1000 caracteres.")]
        public string? Observaciones { get; set; }
    }
}
