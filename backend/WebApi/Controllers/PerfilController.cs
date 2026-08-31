using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Interface;
using WebApi.Models;
using WebApi.Dto;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PerfilController : ControllerBase
    {
        private readonly IPerfilService _service;

        public PerfilController(IPerfilService service)
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
                return Ok(new { message = "No hay registros de Perfil actualmente." });
            }
            return Ok(resultado);
        }

        [Authorize]

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var resultado = await _service.GetByIdAsync(id);
            if (resultado == null)
            {
                return NotFound(new { message = $"No se encontro el registro con Id {id}." });
            }
            
            var claim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            int.TryParse(claim, out var idAutenticado);
            if (!User.IsInRole("Administrador") && !User.IsInRole("ProfesionalSalud") && resultado.IdUsuario != idAutenticado)
            {
                return Forbid();
            }
            return Ok(resultado);
        }

        [Authorize(Roles = "Administrador,ProfesionalSalud")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PerfilCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "La solicitud contiene datos invalidos." });
            }

            var entity = new Perfil
            {
                IdUsuario = dto.IdUsuario,
                Nombres = dto.Nombres,
                Apellidos = dto.Apellidos,
                FechaNacimiento = dto.FechaNacimiento,
                Sexo = dto.Sexo,
                Telefono = dto.Telefono,
                Direccion = dto.Direccion,
                FotoPerfil = dto.FotoPerfil,
                ContactoEmergenciaNombre = dto.ContactoEmergenciaNombre,
                ContactoEmergenciaTelefono = dto.ContactoEmergenciaTelefono
            };

            var creado = await _service.CreateAsync(entity);
            return CreatedAtAction(nameof(GetById), new { id = creado.IdPerfil }, creado);
        }

        [Authorize(Roles = "Administrador,ProfesionalSalud")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] PerfilUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "La solicitud contiene datos invalidos." });
            }

            var entity = new Perfil
            {
                IdPerfil = id,
                Nombres = dto.Nombres,
                Apellidos = dto.Apellidos,
                FechaNacimiento = dto.FechaNacimiento,
                Sexo = dto.Sexo,
                Telefono = dto.Telefono,
                Direccion = dto.Direccion,
                FotoPerfil = dto.FotoPerfil,
                ContactoEmergenciaNombre = dto.ContactoEmergenciaNombre,
                ContactoEmergenciaTelefono = dto.ContactoEmergenciaTelefono
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
