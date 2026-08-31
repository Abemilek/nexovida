using System.ComponentModel.DataAnnotations;

namespace WebApi.Dto
{
    public class TipoIndicadorSaludCreateDto
    {
        [Required]
        [StringLength(100)]
        public string NombreIndicador { get; set; } = string.Empty;

        [StringLength(50)]
        public string? UnidadMedida { get; set; }

        [StringLength(300)]
        public string? Descripcion { get; set; }
    }

    public class TipoIndicadorSaludUpdateDto
    {
        [Required]
        [StringLength(100)]
        public string NombreIndicador { get; set; } = string.Empty;

        [StringLength(50)]
        public string? UnidadMedida { get; set; }

        [StringLength(300)]
        public string? Descripcion { get; set; }
        
        public bool? Activo { get; set; }
    }
}
