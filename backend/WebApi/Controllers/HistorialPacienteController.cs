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
    public class HistorialPacienteController : ControllerBase
    {
        private readonly IHistorialPacienteService _service;

        public HistorialPacienteController(IHistorialPacienteService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
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
                return Ok(new { message = "No hay registros de HistorialPaciente para tu cuenta." });
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

        [Authorize(Roles = "Administrador,ProfesionalSalud")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] HistorialPacienteCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "La solicitud contiene datos invalidos." });
            }
            if (DataScope.EsAdministrador(HttpContext))
            {
                return Forbid();
            }
            if (!await DataScope.PuedeAccederAPacienteAsync(HttpContext, dto.IdPaciente))
            {
                return Forbid();
            }

            var entity = new HistorialPaciente
            {
                IdPaciente = dto.IdPaciente,
                IdUsuario = dto.IdUsuario,
                TipoEvento = dto.TipoEvento,
                FechaEvento = dto.FechaEvento,
                Titulo = dto.Titulo,
                Descripcion = dto.Descripcion
            };

            var creado = await _service.CreateAsync(entity);
            return CreatedAtAction(nameof(GetById), new { id = creado.IdHistorialPaciente }, creado);
        }

        [Authorize(Roles = "Administrador,ProfesionalSalud")]
        [HttpPut("{id:long}")]
        public async Task<IActionResult> Update(long id, [FromBody] HistorialPacienteUpdateDto dto)
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

            var entity = new HistorialPaciente
            {
                IdHistorialPaciente = id,
                IdPaciente = existente.IdPaciente,
                IdUsuario = existente.IdUsuario,
                TipoEvento = dto.TipoEvento,
                FechaEvento = dto.FechaEvento,
                Titulo = dto.Titulo,
                Descripcion = dto.Descripcion
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
