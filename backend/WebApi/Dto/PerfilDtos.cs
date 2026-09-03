using System.ComponentModel.DataAnnotations;

namespace WebApi.Dto
{
    public class PerfilCreateDto
    {
        [Required]
        public int IdUsuario { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombres { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Apellidos { get; set; } = string.Empty;

        public DateTime? FechaNacimiento { get; set; }

        [StringLength(20)]
        public string? Sexo { get; set; }

        [StringLength(30)]
        public string? Telefono { get; set; }

        [StringLength(300)]
        public string? Direccion { get; set; }

        [StringLength(500)]
        public string? FotoPerfil { get; set; }

        [StringLength(200)]
        public string? ContactoEmergenciaNombre { get; set; }

        [StringLength(30)]
        public string? ContactoEmergenciaTelefono { get; set; }
    }

    public class PerfilUpdateDto
    {
        [Required]
        [StringLength(100)]
        public string Nombres { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Apellidos { get; set; } = string.Empty;

        public DateTime? FechaNacimiento { get; set; }

        [StringLength(20)]
        public string? Sexo { get; set; }

        [StringLength(30)]
        public string? Telefono { get; set; }

        [StringLength(300)]
        public string? Direccion { get; set; }

        [StringLength(500)]
        public string? FotoPerfil { get; set; }

        [StringLength(200)]
        public string? ContactoEmergenciaNombre { get; set; }

        [StringLength(30)]
        public string? ContactoEmergenciaTelefono { get; set; }
    }
}
