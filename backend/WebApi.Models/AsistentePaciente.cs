using System.ComponentModel.DataAnnotations;
namespace WebApi.Models
{
    public class AsistentePaciente
    {
        public int IdAsistentePaciente { get; set; }
        public int IdPaciente { get; set; }
        public int IdFamiliar { get; set; }
        [StringLength(100, ErrorMessage = "TipoRelacion no puede superar los 100 caracteres.")]
        public string? TipoRelacion { get; set; }
        public bool? PuedeVerCitas { get; set; }
        public bool? PuedeVerMedicamentos { get; set; }
        public bool? PuedeRecibirAlertas { get; set; }
        public bool? PuedeGestionarRecordatorios { get; set; }
        public DateTime? FechaAsignacion { get; set; }
        public bool? Activo { get; set; }
    }
}
