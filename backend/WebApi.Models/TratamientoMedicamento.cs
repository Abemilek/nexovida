using System.ComponentModel.DataAnnotations;
namespace WebApi.Models
{
    public class TratamientoMedicamento
    {
        public int IdTratamientoMedicamento { get; set; }
        public int IdTratamiento { get; set; }
        public int IdMedicamento { get; set; }
        [StringLength(100, ErrorMessage = "Dosis no puede superar los 100 caracteres.")]
        public string? Dosis { get; set; }
        [StringLength(100, ErrorMessage = "Frecuencia no puede superar los 100 caracteres.")]
        public string? Frecuencia { get; set; }
        [StringLength(100, ErrorMessage = "ViaAdministracion no puede superar los 100 caracteres.")]
        public string? ViaAdministracion { get; set; }
        [StringLength(300, ErrorMessage = "Horarios no puede superar los 300 caracteres.")]
        public string? Horarios { get; set; }
        [StringLength(500, ErrorMessage = "Instrucciones no puede superar los 500 caracteres.")]
        public string? Instrucciones { get; set; }
    }
}
