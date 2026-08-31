using System.ComponentModel.DataAnnotations;
namespace WebApi.Models
{
    public class Rol
    {
        public int IdRol { get; set; }
        [Required(ErrorMessage = "NombreRol es obligatorio.")]
        [StringLength(50, ErrorMessage = "NombreRol no puede superar los 50 caracteres.")]
        public string? NombreRol { get; set; }
        [StringLength(250, ErrorMessage = "Descripcion no puede superar los 250 caracteres.")]
        public string? Descripcion { get; set; }
        public bool? Activo { get; set; }
    }
}
