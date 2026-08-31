using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Interface;
using WebApi.Models;
using WebApi.Dto;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolController : ControllerBase
    {
        private readonly IRolService _service;

        public RolController(IRolService service)
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
                return Ok(new { message = "No hay registros de Rol actualmente." });
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

        [Authorize(Roles = "Administrador")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RolCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "La solicitud contiene datos invalidos." });
            }

            var entity = new Rol
            {
                NombreRol = dto.NombreRol,
                Descripcion = dto.Descripcion,
                Activo = dto.Activo
            };

            var creado = await _service.CreateAsync(entity);
            return CreatedAtAction(nameof(GetById), new { id = creado.IdRol }, creado);
        }

        [Authorize(Roles = "Administrador")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] RolUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "La solicitud contiene datos invalidos." });
            }

            var entity = new Rol
            {
                IdRol = id,
                NombreRol = dto.NombreRol,
                Descripcion = dto.Descripcion,
                Activo = dto.Activo
            };

            var actualizado = await _service.UpdateAsync(id, entity);
            if (!actualizado)
            {
                return NotFound(new { message = $"No se encontro el registro con Id {id}." });
            }
            return Ok(new { message = "Registro actualizado correctamente." });
        }

        [Authorize(Roles = "Administrador")]
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
