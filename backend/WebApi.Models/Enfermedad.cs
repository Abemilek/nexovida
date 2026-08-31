using System.ComponentModel.DataAnnotations;
namespace WebApi.Models
{
    public class Enfermedad
    {
        public int IdEnfermedad { get; set; }
        [Required(ErrorMessage = "NombreEnfermedad es obligatorio.")]
        [StringLength(200, ErrorMessage = "NombreEnfermedad no puede superar los 200 caracteres.")]
        public string? NombreEnfermedad { get; set; }
        [StringLength(500, ErrorMessage = "Descripcion no puede superar los 500 caracteres.")]
        public string? Descripcion { get; set; }
        public bool? EsCronica { get; set; }
        public bool? Activa { get; set; }
    }
}
