using System.ComponentModel.DataAnnotations;
namespace WebApi.Models
{
    public class Familiar
    {
        public int IdFamiliar { get; set; }
        public int IdUsuario { get; set; }
        [Required(ErrorMessage = "Parentesco es obligatorio.")]
        [StringLength(100, ErrorMessage = "Parentesco no puede superar los 100 caracteres.")]
        public string? Parentesco { get; set; }
    }
}
