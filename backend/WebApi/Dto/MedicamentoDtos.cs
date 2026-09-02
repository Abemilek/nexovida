using System.ComponentModel.DataAnnotations;

namespace WebApi.Dto
{
    public class MedicamentoCreateDto
    {
        [Required]
        [StringLength(200)]
        public string NombreMedicamento { get; set; } = string.Empty;

        [StringLength(200)]
        public string? PrincipioActivo { get; set; }

        [StringLength(100)]
        public string? Presentacion { get; set; }

        [StringLength(100)]
        public string? Concentracion { get; set; }

        [StringLength(500)]
        public string? Descripcion { get; set; }
    }

    public class MedicamentoUpdateDto
    {
        [Required]
        [StringLength(200)]
        public string NombreMedicamento { get; set; } = string.Empty;

        [StringLength(200)]
        public string? PrincipioActivo { get; set; }

        [StringLength(100)]
        public string? Presentacion { get; set; }

        [StringLength(100)]
        public string? Concentracion { get; set; }

        [StringLength(500)]
        public string? Descripcion { get; set; }
        
        public bool? Activo { get; set; }
    }
}
