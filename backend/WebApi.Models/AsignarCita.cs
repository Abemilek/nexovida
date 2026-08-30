using System.ComponentModel.DataAnnotations;
namespace WebApi.Models
{
    public class AsignarCita
    {
        public int IdAsignarCita { get; set; }
        public int IdCita { get; set; }
        public int IdProfesional { get; set; }
        public DateTime? FechaAsignacion { get; set; }
        public bool? EsPrincipal { get; set; }
        [StringLength(30, ErrorMessage = "EstadoAsignacion no puede superar los 30 caracteres.")]
        public string? EstadoAsignacion { get; set; }
    }
}
