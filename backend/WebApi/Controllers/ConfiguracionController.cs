using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConfiguracionController : ControllerBase
    {
        private static object _currentConfig = new
        {
            NotificacionesPush = true,
            FrecuenciaRecordatorios = "Cada 8 horas",
            MantenimientoActivo = false,
            VersionSistema = "1.0.0"
        };

        [Authorize(Roles = "Administrador")]
        [HttpGet]
        public IActionResult GetConfig()
        {
            return Ok(_currentConfig);
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost]
        public IActionResult UpdateConfig([FromBody] object newConfig)
        {
            _currentConfig = newConfig;
            return Ok(new { message = "Configuración actualizada con éxito" });
        }
    }
}
