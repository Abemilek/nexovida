using System.ComponentModel.DataAnnotations;

namespace WebApi.Dto
{
    public class AsignarCitaCreateDto
    {
        [Required]
        public int IdCita { get; set; }

        [Required]
        public int IdProfesional { get; set; }

        public bool? EsPrincipal { get; set; }

        [StringLength(30)]
        public string? EstadoAsignacion { get; set; }
    }

    public class AsignarCitaUpdateDto
    {
        public bool? EsPrincipal { get; set; }

        [StringLength(30)]
        public string? EstadoAsignacion { get; set; }
    }
}
