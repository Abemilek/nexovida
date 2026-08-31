namespace WebApi.Models
{
    public class UsuarioRol
    {
        public int IdUsuarioRol { get; set; }
        public int IdUsuario { get; set; }
        public int IdRol { get; set; }
        public DateTime? FechaAsignacion { get; set; }
        public bool? Activo { get; set; }
    }
}
