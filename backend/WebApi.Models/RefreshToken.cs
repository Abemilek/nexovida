using System.Text.Json.Serialization;

namespace WebApi.Models
{
    public class RefreshToken
    {
        public int IdRefreshToken { get; set; }
        public int IdUsuario { get; set; }

        [JsonIgnore]
        public byte[]? TokenHash { get; set; }

        public DateTime FechaCreacion { get; set; }
        public DateTime FechaExpiracion { get; set; }
        public bool Revocado { get; set; }
    }
}
