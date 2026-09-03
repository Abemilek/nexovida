using System.ComponentModel.DataAnnotations;
namespace WebApi.Models
{
    public class Medicamento
    {
        public int IdMedicamento { get; set; }
        [Required(ErrorMessage = "NombreMedicamento es obligatorio.")]
        [StringLength(200, ErrorMessage = "NombreMedicamento no puede superar los 200 caracteres.")]
        public string? NombreMedicamento { get; set; }
        [StringLength(200, ErrorMessage = "PrincipioActivo no puede superar los 200 caracteres.")]
        public string? PrincipioActivo { get; set; }
        [StringLength(100, ErrorMessage = "Presentacion no puede superar los 100 caracteres.")]
        public string? Presentacion { get; set; }
        [StringLength(100, ErrorMessage = "Concentracion no puede superar los 100 caracteres.")]
        public string? Concentracion { get; set; }
        [StringLength(500, ErrorMessage = "Descripcion no puede superar los 500 caracteres.")]
        public string? Descripcion { get; set; }
        public bool? Activo { get; set; }
    }
}
