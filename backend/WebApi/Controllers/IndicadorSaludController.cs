using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Interface;
using WebApi.Models;
using WebApi.Dto;
using WebApi.Helpers;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IndicadorSaludController : ControllerBase
    {
        private readonly IIndicadorSaludService _service;
        private readonly IAlertaService _alertaService;

        public IndicadorSaludController(IIndicadorSaludService service, IAlertaService alertaService)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _alertaService = alertaService ?? throw new ArgumentNullException(nameof(alertaService));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            if (DataScope.EsAdministrador(HttpContext))
            {
                return Forbid();
            }

            var resultado = await _service.GetAllAsync();
            var permitidos = await DataScope.ObtenerPacientesPermitidosAsync(HttpContext);
            var filtrado = resultado.Where(x => permitidos.Contains(x.IdPaciente)).ToList();
            if (!filtrado.Any())
            {
                return Ok(new { message = "No hay registros de IndicadorSalud para tu cuenta." });
            }
            return Ok(filtrado);
        }

        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetById(long id)
        {
            var resultado = await _service.GetByIdAsync(id);
            if (resultado == null)
            {
                return NotFound(new { message = $"No se encontro el registro con Id {id}." });
            }
            if (!await WebApi.Helpers.BolaChecker.EsPropietario(HttpContext, resultado.IdPaciente))
            {
                return Forbid();
            }
            return Ok(resultado);
        }

        [Authorize(Roles = "Paciente,ProfesionalSalud")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] IndicadorSaludCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "La solicitud contiene datos invalidos." });
            }
            if (DataScope.EsAdministrador(HttpContext))
            {
                return Forbid();
            }
            if (!await WebApi.Helpers.BolaChecker.EsPropietario(HttpContext, dto.IdPaciente))
            {
                return Forbid();
            }

            var entity = new IndicadorSalud
            {
                IdPaciente = dto.IdPaciente,
                IdTipoIndicador = dto.IdTipoIndicador,
                Valor = dto.Valor,
                ValorSecundario = dto.ValorSecundario,
                FechaHoraMedicion = dto.FechaHoraMedicion,
                IdUsuarioRegistro = dto.IdUsuarioRegistro,
                Observaciones = dto.Observaciones,
                Fuente = dto.Fuente
            };

            var creado = await _service.CreateAsync(entity);
            await CrearAlertaSiFueraDeRangoAsync(creado);
            return CreatedAtAction(nameof(GetById), new { id = creado.IdIndicadorSalud }, creado);
        }

        private async Task CrearAlertaSiFueraDeRangoAsync(IndicadorSalud indicador)
        {
            var fueraDeRango = indicador.IdTipoIndicador switch
            {
                
                1 => indicador.Valor >= 140 || (indicador.ValorSecundario >= 90),
              
                2 => indicador.Valor >= 180,
               
                4 => indicador.Valor < 92,
                _ => false
            };
            if (!fueraDeRango)
            {
                return;
            }

            try
            {
                var alerta = new Alerta
                {
                    IdPaciente = indicador.IdPaciente,
                    IdIndicadorSalud = indicador.IdIndicadorSalud,
                    Titulo = "Indicador fuera de rango",
                    Mensaje = $"Se registro una medicion fuera del rango esperado. Revisa el plan de seguimiento.",
                    TipoAlerta = "Indicador Anormal",
                    NivelPrioridad = indicador.IdTipoIndicador == 2 ? "Media" : "Alta",
                };
                await _alertaService.CreateAsync(alerta);
            }
            catch
            {
                
            }
        }

        [Authorize(Roles = "Administrador,ProfesionalSalud")]
        [HttpPut("{id:long}")]
        public async Task<IActionResult> Update(long id, [FromBody] IndicadorSaludUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "La solicitud contiene datos invalidos." });
            }

            var existente = await _service.GetByIdAsync(id);
            if (existente == null)
            {
                return NotFound(new { message = $"No se encontro el registro con Id {id}." });
            }
            if (DataScope.EsAdministrador(HttpContext)
                || !await DataScope.PuedeAccederAPacienteAsync(HttpContext, existente.IdPaciente))
            {
                return Forbid();
            }

            var entity = new IndicadorSalud
            {
                IdIndicadorSalud = id,
                IdPaciente = existente.IdPaciente,
                IdTipoIndicador = dto.IdTipoIndicador,
                Valor = dto.Valor,
                ValorSecundario = dto.ValorSecundario,
                FechaHoraMedicion = dto.FechaHoraMedicion,
                IdUsuarioRegistro = existente.IdUsuarioRegistro,
                Observaciones = dto.Observaciones,
                Fuente = dto.Fuente
            };

            var actualizado = await _service.UpdateAsync(id, entity);
            if (!actualizado)
            {
                return NotFound(new { message = $"No se encontro el registro con Id {id}." });
            }
            return Ok(new { message = "Registro actualizado correctamente." });
        }

        [Authorize(Roles = "Administrador,ProfesionalSalud")]
        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id)
        {
            var existente = await _service.GetByIdAsync(id);
            if (existente == null)
            {
                return NotFound(new { message = $"No se encontro el registro con Id {id}." });
            }
            if (DataScope.EsAdministrador(HttpContext)
                || !await DataScope.PuedeAccederAPacienteAsync(HttpContext, existente.IdPaciente))
            {
                return Forbid();
            }

            var eliminado = await _service.DeleteAsync(id);
            if (!eliminado)
            {
                return NotFound(new { message = $"No se encontro el registro con Id {id}." });
            }
            return Ok(new { message = "Registro eliminado correctamente." });
        }
    }
}
