using System.ComponentModel.DataAnnotations;
namespace WebApi.Models
{
    public class ProfesionalSalud
    {
        public int IdProfesional { get; set; }
        public int IdUsuario { get; set; }
        [Required(ErrorMessage = "Especialidad es obligatorio.")]
        [StringLength(150, ErrorMessage = "Especialidad no puede superar los 150 caracteres.")]
        public string? Especialidad { get; set; }
        [Required(ErrorMessage = "NumeroLicencia es obligatorio.")]
        [StringLength(100, ErrorMessage = "NumeroLicencia no puede superar los 100 caracteres.")]
        public string? NumeroLicencia { get; set; }
        [StringLength(200, ErrorMessage = "CentroSalud no puede superar los 200 caracteres.")]
        public string? CentroSalud { get; set; }
        [StringLength(30, ErrorMessage = "TelefonoProfesional no puede superar los 30 caracteres.")]
        public string? TelefonoProfesional { get; set; }
        public bool? Activo { get; set; }
    }
}
