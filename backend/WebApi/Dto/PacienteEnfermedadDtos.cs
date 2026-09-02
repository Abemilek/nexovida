using System.ComponentModel.DataAnnotations;

namespace WebApi.Dto
{
    public class PacienteEnfermedadCreateDto
    {
        [Required]
        public int IdPaciente { get; set; }

        [Required]
        public int IdEnfermedad { get; set; }

        public DateTime? FechaDiagnostico { get; set; }

        [StringLength(500)]
        public string? Observaciones { get; set; }
    }

    public class PacienteEnfermedadUpdateDto
    {
        public DateTime? FechaDiagnostico { get; set; }

        [StringLength(500)]
        public string? Observaciones { get; set; }

        public bool? Activa { get; set; }
    }
}
