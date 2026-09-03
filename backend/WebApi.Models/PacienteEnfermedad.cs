using System.ComponentModel.DataAnnotations;
namespace WebApi.Models
{
    public class PacienteEnfermedad
    {
        public int IdPacienteEnfermedad { get; set; }
        public int IdPaciente { get; set; }
        public int IdEnfermedad { get; set; }
        public DateTime? FechaDiagnostico { get; set; }
        [StringLength(500, ErrorMessage = "Observaciones no puede superar los 500 caracteres.")]
        public string? Observaciones { get; set; }
        public bool? Activa { get; set; }
    }
}
