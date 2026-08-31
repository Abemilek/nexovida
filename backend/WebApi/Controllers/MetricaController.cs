using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebApi.Interface;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MetricaController : ControllerBase
    {
        private readonly IMetricaService _service;

        public MetricaController(IMetricaService service)
        {
            _service = service;
        }

        [Authorize(Roles = "Administrador")]
        [HttpGet]
        public async Task<IActionResult> GetMetrics()
        {
            var metrics = await _service.GetGlobalMetricsAsync();
            return Ok(metrics);
        }
    }
}
