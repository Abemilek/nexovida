using System.ComponentModel.DataAnnotations;
namespace WebApi.Models
{
    public class HistorialPaciente
    {
        public long IdHistorialPaciente { get; set; }
        public int IdPaciente { get; set; }
        public int? IdUsuario { get; set; }
        [Required(ErrorMessage = "TipoEvento es obligatorio.")]
        [StringLength(100, ErrorMessage = "TipoEvento no puede superar los 100 caracteres.")]
        public string? TipoEvento { get; set; }
        public DateTime? FechaEvento { get; set; }
        [Required(ErrorMessage = "Titulo es obligatorio.")]
        [StringLength(200, ErrorMessage = "Titulo no puede superar los 200 caracteres.")]
        public string? Titulo { get; set; }
        [StringLength(2000, ErrorMessage = "Descripcion no puede superar los 2000 caracteres.")]
        public string? Descripcion { get; set; }
    }
}
