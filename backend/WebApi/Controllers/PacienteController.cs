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
    public class PacienteController : ControllerBase
    {
        private readonly IPacienteService _service;

        public PacienteController(IPacienteService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var resultado = await _service.GetAllAsync();
            if (DataScope.EsAdministrador(HttpContext))
            {
                var identidades = resultado.Select(p => new Paciente
                {
                    IdPaciente = p.IdPaciente,
                    IdUsuario = p.IdUsuario,
                    EstadoPaciente = p.EstadoPaciente
                }).ToList();
                return identidades.Any()
                    ? Ok(identidades)
                    : Ok(new { message = "No hay registros de Paciente actualmente." });
            }

            var permitidos = await DataScope.ObtenerPacientesPermitidosAsync(HttpContext);
            var filtrado = resultado.Where(p => permitidos.Contains(p.IdPaciente)).ToList();
            if (!filtrado.Any())
            {
                return Ok(new { message = "No hay pacientes asignados a tu cuenta." });
            }
            return Ok(filtrado);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
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
        public async Task<IActionResult> Create([FromBody] PacienteCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "La solicitud contiene datos invalidos." });
            }

            var entity = new Paciente
            {
                IdUsuario = dto.IdUsuario,
                TipoPaciente = dto.TipoPaciente,
                PorcentajeDiscapacidad = dto.PorcentajeDiscapacidad,
                NecesidadesEspeciales = dto.NecesidadesEspeciales,
                EstadoPaciente = dto.EstadoPaciente
            };

            var creado = await _service.CreateAsync(entity);
            return CreatedAtAction(nameof(GetById), new { id = creado.IdPaciente }, creado);
        }

        [Authorize(Roles = "Administrador,ProfesionalSalud")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] PacienteUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "La solicitud contiene datos invalidos." });
            }

            var entity = new Paciente
            {
                IdPaciente = id,
                TipoPaciente = dto.TipoPaciente,
                PorcentajeDiscapacidad = dto.PorcentajeDiscapacidad,
                NecesidadesEspeciales = dto.NecesidadesEspeciales,
                EstadoPaciente = dto.EstadoPaciente
            };

            var actualizado = await _service.UpdateAsync(id, entity);
            if (!actualizado)
            {
                return NotFound(new { message = $"No se encontro el registro con Id {id}." });
            }
            return Ok(new { message = "Registro actualizado correctamente." });
        }

        [Authorize(Roles = "Administrador,ProfesionalSalud")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var eliminado = await _service.DeleteAsync(id);
            if (!eliminado)
            {
                return NotFound(new { message = $"No se encontro el registro con Id {id}." });
            }
            return Ok(new { message = "Registro eliminado correctamente." });
        }
    }
}
