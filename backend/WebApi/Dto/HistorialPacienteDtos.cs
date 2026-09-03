using System.ComponentModel.DataAnnotations;

namespace WebApi.Dto
{
    public class HistorialPacienteCreateDto
    {
        [Required]
        public int IdPaciente { get; set; }

        public int? IdUsuario { get; set; }

        [Required]
        [StringLength(100)]
        public string TipoEvento { get; set; } = string.Empty;

        public DateTime? FechaEvento { get; set; }

        [Required]
        [StringLength(200)]
        public string Titulo { get; set; } = string.Empty;

        [StringLength(2000)]
        public string? Descripcion { get; set; }
    }

    public class HistorialPacienteUpdateDto
    {
        [Required]
        [StringLength(100)]
        public string TipoEvento { get; set; } = string.Empty;

        public DateTime? FechaEvento { get; set; }

        [Required]
        [StringLength(200)]
        public string Titulo { get; set; } = string.Empty;

        [StringLength(2000)]
        public string? Descripcion { get; set; }
    }
}
