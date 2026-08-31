using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Interface;
using WebApi.Models;
using WebApi.Dto;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AsistentePacienteController : ControllerBase
    {
        private readonly IAsistentePacienteService _service;

        public AsistentePacienteController(IAsistentePacienteService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        [Authorize(Roles = "Administrador,ProfesionalSalud")]

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var resultado = await _service.GetAllAsync();
            if (!resultado.Any())
            {
                return Ok(new { message = "No hay registros de AsistentePaciente actualmente." });
            }
            return Ok(resultado);
        }

        [Authorize(Roles = "Administrador,ProfesionalSalud")]

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var resultado = await _service.GetByIdAsync(id);
            if (resultado == null)
            {
                return NotFound(new { message = $"No se encontro el registro con Id {id}." });
            }
            return Ok(resultado);
        }

        [Authorize(Roles = "Administrador,ProfesionalSalud")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AsistentePacienteCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "La solicitud contiene datos invalidos." });
            }

            var entity = new AsistentePaciente
            {
                IdPaciente = dto.IdPaciente,
                IdFamiliar = dto.IdFamiliar,
                TipoRelacion = dto.TipoRelacion,
                PuedeVerCitas = dto.PuedeVerCitas,
                PuedeVerMedicamentos = dto.PuedeVerMedicamentos,
                PuedeRecibirAlertas = dto.PuedeRecibirAlertas,
                PuedeGestionarRecordatorios = dto.PuedeGestionarRecordatorios
            };

            var creado = await _service.CreateAsync(entity);
            return CreatedAtAction(nameof(GetById), new { id = creado.IdAsistentePaciente }, creado);
        }

        [Authorize(Roles = "Administrador,ProfesionalSalud")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] AsistentePacienteUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "La solicitud contiene datos invalidos." });
            }

            var entity = new AsistentePaciente
            {
                IdAsistentePaciente = id,
                TipoRelacion = dto.TipoRelacion,
                PuedeVerCitas = dto.PuedeVerCitas,
                PuedeVerMedicamentos = dto.PuedeVerMedicamentos,
                PuedeRecibirAlertas = dto.PuedeRecibirAlertas,
                PuedeGestionarRecordatorios = dto.PuedeGestionarRecordatorios,
                Activo = dto.Activo
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
