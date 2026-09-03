using System.ComponentModel.DataAnnotations;

namespace WebApi.Dto
{
    public class UsuarioRolCreateDto
    {
        [Required]
        public int IdUsuario { get; set; }

        [Required]
        public int IdRol { get; set; }
    }

    public class UsuarioRolUpdateDto
    {
        public int IdUsuario { get; set; }
        public int IdRol { get; set; }
        public bool? Activo { get; set; }
    }
}
