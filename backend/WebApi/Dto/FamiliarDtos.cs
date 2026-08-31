using System.ComponentModel.DataAnnotations;

namespace WebApi.Dto
{
    public class FamiliarCreateDto
    {
        [Required]
        public int IdUsuario { get; set; }

        [Required]
        [StringLength(100)]
        public string Parentesco { get; set; } = string.Empty;
    }

    public class FamiliarUpdateDto
    {
        [Required]
        [StringLength(100)]
        public string Parentesco { get; set; } = string.Empty;
    }
}
