using System.ComponentModel.DataAnnotations;

namespace WebApi.Dto
{
    public class TratamientoMedicamentoCreateDto
    {
        [Required]
        public int IdTratamiento { get; set; }

        [Required]
        public int IdMedicamento { get; set; }

        [StringLength(100)]
        public string? Dosis { get; set; }

        [StringLength(100)]
        public string? Frecuencia { get; set; }

        [StringLength(100)]
        public string? ViaAdministracion { get; set; }

        [StringLength(300)]
        public string? Horarios { get; set; }

        [StringLength(500)]
        public string? Instrucciones { get; set; }
    }

    public class TratamientoMedicamentoUpdateDto
    {
        [StringLength(100)]
        public string? Dosis { get; set; }

        [StringLength(100)]
        public string? Frecuencia { get; set; }

        [StringLength(100)]
        public string? ViaAdministracion { get; set; }

        [StringLength(300)]
        public string? Horarios { get; set; }

        [StringLength(500)]
        public string? Instrucciones { get; set; }
    }
}
