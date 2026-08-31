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
    public class RecordatorioController : ControllerBase
    {
        private readonly IRecordatorioService _service;

        public RecordatorioController(IRecordatorioService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        // SECURITY (OWASP API1:2023): cada rol ve solo sus recordatorios -
        // Paciente los propios, ProfesionalSalud los de sus asignados, Familiar los
        // de quienes cuida. El Administrador no consume datos clinicos.
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
                return Ok(new { message = "No hay registros de Recordatorio para tu cuenta." });
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
        public async Task<IActionResult> Create([FromBody] RecordatorioCreateDto dto)
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

            var entity = new Recordatorio
            {
                IdPaciente = dto.IdPaciente,
                IdTratamientoMedicamento = dto.IdTratamientoMedicamento,
                IdCita = dto.IdCita,
                Titulo = dto.Titulo,
                Descripcion = dto.Descripcion,
                TipoRecordatorio = dto.TipoRecordatorio,
                FechaHoraProgramada = dto.FechaHoraProgramada,
                Repetir = dto.Repetir,
                FrecuenciaRepeticion = dto.FrecuenciaRepeticion
            };

            var creado = await _service.CreateAsync(entity);
            return CreatedAtAction(nameof(GetById), new { id = creado.IdRecordatorio }, creado);
        }

        [Authorize(Roles = "Administrador,ProfesionalSalud")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] RecordatorioUpdateDto dto)
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
            if (!await DataScope.PuedeAccederAPacienteAsync(HttpContext, existente.IdPaciente))
            {
                return Forbid();
            }

            var entity = new Recordatorio
            {
                IdRecordatorio = id,
                // Se conserva el IdPaciente del registro existente: la edicion no
                // transfiere el recordatorio de un paciente a otro, y sin esto el
                // UPDATE intentaria escribir IdPaciente = 0 (FK violada -> 500).
                IdPaciente = existente.IdPaciente,
                IdTratamientoMedicamento = dto.IdTratamientoMedicamento,
                IdCita = dto.IdCita,
                Titulo = dto.Titulo,
                Descripcion = dto.Descripcion,
                TipoRecordatorio = dto.TipoRecordatorio,
                FechaHoraProgramada = dto.FechaHoraProgramada,
                Repetir = dto.Repetir,
                FrecuenciaRepeticion = dto.FrecuenciaRepeticion,
                EstadoRecordatorio = dto.EstadoRecordatorio,
                FechaCompletado = dto.FechaCompletado,
                Activo = dto.Activo
            };

            var actualizado = await _service.UpdateAsync(id, entity);
            if (!actualizado)
            {
                return NotFound(new { message = $"No se encontro el registro con Id {id}." });
            }
            return Ok(new { message = "Registro actualizado correctamente." });
        }

        [Authorize(Roles = "Paciente,ProfesionalSalud")]
        [HttpPost("{id:int}/completar")]
        public async Task<IActionResult> Complete(int id)
        {
            var existente = await _service.GetByIdAsync(id);
            if (existente == null)
            {
                return NotFound(new { message = $"No se encontro el registro con Id {id}." });
            }
            if (!await WebApi.Helpers.BolaChecker.EsPropietario(HttpContext, existente.IdPaciente))
            {
                return Forbid();
            }

            existente.EstadoRecordatorio = "Completado";
            existente.FechaCompletado = DateTime.UtcNow;
            var actualizado = await _service.UpdateAsync(id, existente);
            if (!actualizado)
            {
                return NotFound(new { message = $"No se encontro el registro con Id {id}." });
            }
            return Ok(new { message = "Recordatorio completado correctamente." });
        }

        [Authorize(Roles = "Administrador,ProfesionalSalud")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
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
