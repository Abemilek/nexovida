using System.ComponentModel.DataAnnotations;
namespace WebApi.Models
{
    public class Perfil
    {
        public int IdPerfil { get; set; }
        public int IdUsuario { get; set; }
        [Required(ErrorMessage = "Nombres es obligatorio.")]
        [StringLength(100, ErrorMessage = "Nombres no puede superar los 100 caracteres.")]
        public string? Nombres { get; set; }
        [Required(ErrorMessage = "Apellidos es obligatorio.")]
        [StringLength(100, ErrorMessage = "Apellidos no puede superar los 100 caracteres.")]
        public string? Apellidos { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        [StringLength(20, ErrorMessage = "Sexo no puede superar los 20 caracteres.")]
        public string? Sexo { get; set; }
        [StringLength(30, ErrorMessage = "Telefono no puede superar los 30 caracteres.")]
        public string? Telefono { get; set; }
        [StringLength(300, ErrorMessage = "Direccion no puede superar los 300 caracteres.")]
        public string? Direccion { get; set; }
        [StringLength(500, ErrorMessage = "FotoPerfil no puede superar los 500 caracteres.")]
        public string? FotoPerfil { get; set; }
        [StringLength(200, ErrorMessage = "ContactoEmergenciaNombre no puede superar los 200 caracteres.")]
        public string? ContactoEmergenciaNombre { get; set; }
        [StringLength(30, ErrorMessage = "ContactoEmergenciaTelefono no puede superar los 30 caracteres.")]
        public string? ContactoEmergenciaTelefono { get; set; }
    }
}
