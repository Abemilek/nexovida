using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Interface;
using WebApi.Models;
using WebApi.Dto;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _service;

        public UsuarioController(IUsuarioService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        [Authorize(Roles = "Administrador")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var resultado = (await _service.GetAllAsync()).ToList();
            if (!resultado.Any())
            {
                return Ok(new { message = "No hay registros de Usuario actualmente." });
            }

            var rolService = HttpContext.RequestServices.GetRequiredService<IRolService>();
            var usuarioRolService = HttpContext.RequestServices.GetRequiredService<IUsuarioRolService>();
            var roles = (await rolService.GetAllAsync()).ToDictionary(r => r.IdRol, r => r.NombreRol);
            var asignaciones = await usuarioRolService.GetAllAsync();

            var respuesta = resultado.Select(u =>
            {
                var asignacion = asignaciones
                    .Where(a => a.IdUsuario == u.IdUsuario)
                    .OrderByDescending(a => a.Activo == true)
                    .FirstOrDefault();
                return new
                {
                    u.IdUsuario,
                    u.NombreUsuario,
                    u.Correo,
                    u.FechaRegistro,
                    u.UltimoAcceso,
                    u.Activo,
                    u.TwoFactorEnabled,
                    IdRol = asignacion?.IdRol,
                    Rol = asignacion != null && roles.TryGetValue(asignacion.IdRol, out var nombre) ? nombre : null
                };
            }).ToList();

            return Ok(respuesta);
        }

        [Authorize]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (!EsPropioOAdministrador(id))
            {
                return Forbid();
            }

            var resultado = await _service.GetByIdAsync(id);
            if (resultado == null)
            {
                return NotFound(new { message = $"No se encontro el registro con Id {id}." });
            }
            return Ok(resultado);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UsuarioCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "La solicitud contiene datos invalidos." });
            }

            var existente = await _service.GetByCorreoAsync(dto.Correo!);
            if (existente != null)
            {
                return Conflict(new { message = "Ya existe una cuenta registrada con ese correo." });
            }

            var entity = new Usuario
            {
                NombreUsuario = dto.NombreUsuario,
                Correo = dto.Correo,
                Password = dto.Password,
                FechaRegistro = DateTime.UtcNow,
                UltimoAcceso = DateTime.UtcNow,
                Activo = true
            };

            var creado = await _service.CreateAsync(entity);
            return CreatedAtAction(nameof(GetById), new { id = creado.IdUsuario }, creado);
        }

        [Authorize]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UsuarioUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "La solicitud contiene datos invalidos." });
            }

            if (!EsPropioOAdministrador(id))
            {
                return Forbid();
            }

            var existente = await _service.GetByIdAsync(id);
            if (existente == null)
            {
                return NotFound(new { message = $"No se encontro el registro con Id {id}." });
            }

            var idAutenticado = int.TryParse(
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var idClaim) ? idClaim : 0;
            if (dto.Activo == false && idAutenticado == id)
            {
                return BadRequest(new { message = "No puedes desactivar tu propia cuenta de Administrador." });
            }

            var entity = new Usuario
            {
                NombreUsuario = dto.NombreUsuario,
                Correo = dto.Correo,
                Password = dto.Password,
                Activo = dto.Activo ?? existente.Activo
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

        private bool EsPropioOAdministrador(int idSolicitado)
        {
            if (User.IsInRole("Administrador"))
            {
                return true;
            }

            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(claim, out var idAutenticado) && idAutenticado == idSolicitado;
        }
    }
}
