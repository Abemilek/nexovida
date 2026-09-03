using System.ComponentModel.DataAnnotations;
namespace WebApi.Models
{
    public class TipoIndicadorSalud
    {
        public int IdTipoIndicador { get; set; }
        [Required(ErrorMessage = "NombreIndicador es obligatorio.")]
        [StringLength(100, ErrorMessage = "NombreIndicador no puede superar los 100 caracteres.")]
        public string? NombreIndicador { get; set; }
        [StringLength(50, ErrorMessage = "UnidadMedida no puede superar los 50 caracteres.")]
        public string? UnidadMedida { get; set; }
        [StringLength(300, ErrorMessage = "Descripcion no puede superar los 300 caracteres.")]
        public string? Descripcion { get; set; }
        public bool? Activo { get; set; }
    }
}
