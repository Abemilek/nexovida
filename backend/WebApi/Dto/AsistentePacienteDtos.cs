using System.ComponentModel.DataAnnotations;

namespace WebApi.Dto
{
    public class AsistentePacienteCreateDto
    {
        [Required]
        public int IdPaciente { get; set; }

        [Required]
        public int IdFamiliar { get; set; }

        [StringLength(100)]
        public string? TipoRelacion { get; set; }

        public bool? PuedeVerCitas { get; set; }
        public bool? PuedeVerMedicamentos { get; set; }
        public bool? PuedeRecibirAlertas { get; set; }
        public bool? PuedeGestionarRecordatorios { get; set; }
    }

    public class AsistentePacienteUpdateDto
    {
        [StringLength(100)]
        public string? TipoRelacion { get; set; }

        public bool? PuedeVerCitas { get; set; }
        public bool? PuedeVerMedicamentos { get; set; }
        public bool? PuedeRecibirAlertas { get; set; }
        public bool? PuedeGestionarRecordatorios { get; set; }
        public bool? Activo { get; set; }
    }
}
