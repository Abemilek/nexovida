using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using WebApi.Dto;
using WebApi.Interface;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IUsuarioRolService _usuarioRolService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly ITotpService _totpService;
        private readonly IPacienteService _pacienteService;
        private readonly IFamiliarService _familiarService;
        private readonly IProfesionalSaludService _profesionalService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;

        private static readonly TimeSpan AccessTokenLifetime = TimeSpan.FromMinutes(15);
        private static readonly TimeSpan RefreshTokenLifetime = TimeSpan.FromDays(7);
        private static readonly byte[] DummyHash = new byte[32];
        private static readonly byte[] DummySalt = new byte[16];

        public AuthController(
            IUsuarioService usuarioService,
            IUsuarioRolService usuarioRolService,
            IRefreshTokenService refreshTokenService,
            ITotpService totpService,
            IPacienteService pacienteService,
            IFamiliarService familiarService,
            IProfesionalSaludService profesionalService,
            IConfiguration configuration,
            ILogger<AuthController> logger)
        {
            _usuarioService = usuarioService;
            _usuarioRolService = usuarioRolService;
            _refreshTokenService = refreshTokenService;
            _totpService = totpService;
            _pacienteService = pacienteService;
            _familiarService = familiarService;
            _profesionalService = profesionalService;
            _configuration = configuration;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        [EnableRateLimiting("auth")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "La solicitud contiene datos invalidos." });
            }

            var usuario = await _usuarioService.GetByCorreoAsync(request.Correo);

            const string genericError = "Correo o contrasena incorrectos.";

            if (usuario == null || usuario.Activo != true || usuario.Contrasena == null || usuario.Salt == null)
            {
                _usuarioService.VerifyPassword(request.Password, DummyHash, DummySalt);
                _logger.LogWarning("Intento de login rechazado para {Correo}", request.Correo);
                return Unauthorized(new { message = genericError });
            }

            if (!_usuarioService.VerifyPassword(request.Password, usuario.Contrasena, usuario.Salt))
            {
                _logger.LogWarning("Contrasena incorrecta para {Correo}", request.Correo);
                return Unauthorized(new { message = genericError });
            }

            if (usuario.TwoFactorEnabled == true)
            {
                if (string.IsNullOrWhiteSpace(request.TotpCode))
                {
                    return Ok(new LoginResponse { RequiresTwoFactor = true });
                }

                if (string.IsNullOrEmpty(usuario.TwoFactorSecret) ||
                    !_totpService.VerifyCode(usuario.TwoFactorSecret, request.TotpCode))
                {
                    return Unauthorized(new { message = "Codigo de doble factor invalido o expirado." });
                }
            }

            await _usuarioService.UpdateUltimoAccesoAsync(usuario.IdUsuario);

            var roles = await _usuarioRolService.GetRoleNamesForUserAsync(usuario.IdUsuario);
            var accessToken = GenerateAccessToken(usuario.IdUsuario, usuario.Correo!, roles);
            var refreshToken = await _refreshTokenService.CreateAsync(usuario.IdUsuario, RefreshTokenLifetime);

            return Ok(new LoginResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresInSeconds = (int)AccessTokenLifetime.TotalSeconds,
                RequiresTwoFactor = false,
            });
        }

        [AllowAnonymous]
        [HttpPost("refresh")]
        [EnableRateLimiting("auth")]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "La solicitud contiene datos invalidos." });
            }

            var idUsuario = await _refreshTokenService.ValidateAsync(request.RefreshToken);
            if (idUsuario == null)
            {
                return Unauthorized(new { message = "El refresh token no es valido o ya expiro." });
            }

            var usuario = await _usuarioService.GetByIdAsync(idUsuario.Value);
            if (usuario == null || usuario.Activo != true)
            {
                return Unauthorized(new { message = "El usuario ya no esta activo." });
            }

            await _refreshTokenService.RevokeAsync(request.RefreshToken);

            var roles = await _usuarioRolService.GetRoleNamesForUserAsync(usuario.IdUsuario);
            var accessToken = GenerateAccessToken(usuario.IdUsuario, usuario.Correo!, roles);
            var newRefreshToken = await _refreshTokenService.CreateAsync(usuario.IdUsuario, RefreshTokenLifetime);

            return Ok(new LoginResponse
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshToken,
                ExpiresInSeconds = (int)AccessTokenLifetime.TotalSeconds,
                RequiresTwoFactor = false,
            });
        }

        [AllowAnonymous]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] RefreshRequest request)
        {
            await _refreshTokenService.RevokeAsync(request.RefreshToken);
            return Ok(new { message = "Sesion cerrada correctamente." });
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            var idUsuario = GetAuthenticatedUserId();
            var usuario = await _usuarioService.GetByIdAsync(idUsuario);
            if (usuario == null)
            {
                return NotFound(new { message = "Usuario no encontrado." });
            }

            var roles = await _usuarioRolService.GetRoleNamesForUserAsync(idUsuario);

            int? idPaciente = null;
            int? idFamiliar = null;
            int? idProfesional = null;

            foreach (var p in await _pacienteService.GetAllAsync())
            {
                if (p.IdUsuario == idUsuario)
                {
                    idPaciente = p.IdPaciente;
                    break;
                }
            }
            foreach (var f in await _familiarService.GetAllAsync())
            {
                if (f.IdUsuario == idUsuario)
                {
                    idFamiliar = f.IdFamiliar;
                    break;
                }
            }
            foreach (var pr in await _profesionalService.GetAllAsync())
            {
                if (pr.IdUsuario == idUsuario && pr.Activo != false)
                {
                    idProfesional = pr.IdProfesional;
                    break;
                }
            }

            return Ok(new
            {
                idUsuario,
                nombreUsuario = usuario.NombreUsuario,
                correo = usuario.Correo,
                roles,
                twoFactorEnabled = usuario.TwoFactorEnabled == true,
                idPaciente,
                idFamiliar,
                idProfesional
            });
        }

        [HttpPost("2fa/setup")]
        [Authorize]
        public async Task<IActionResult> SetupTwoFactor()
        {
            var idUsuario = GetAuthenticatedUserId();
            var usuario = await _usuarioService.GetByIdAsync(idUsuario);
            if (usuario == null)
            {
                return NotFound(new { message = "Usuario no encontrado." });
            }

            if (usuario.TwoFactorEnabled == true)
            {
                return BadRequest(new { message = "El 2FA ya esta activo. Desactivalo primero para generar un secreto nuevo." });
            }

            var secret = _totpService.GenerateSecret();

            await _usuarioService.SetTwoFactorAsync(idUsuario, enabled: false, secret: secret);

            return Ok(new TwoFactorSetupResponse
            {
                Secret = secret,
                ProvisioningUri = _totpService.GenerateProvisioningUri(secret, usuario.Correo ?? "usuario"),
            });
        }

        [HttpPost("2fa/verify")]
        [Authorize]
        public async Task<IActionResult> VerifyTwoFactor([FromBody] TwoFactorVerifyRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "El codigo debe tener 6 digitos." });
            }

            var idUsuario = GetAuthenticatedUserId();
            var usuario = await _usuarioService.GetByIdAsync(idUsuario);
            if (usuario == null || string.IsNullOrEmpty(usuario.TwoFactorSecret))
            {
                return BadRequest(new { message = "Primero llama a /api/auth/2fa/setup." });
            }

            if (!_totpService.VerifyCode(usuario.TwoFactorSecret, request.Code))
            {
                return Unauthorized(new { message = "Codigo invalido." });
            }

            await _usuarioService.SetTwoFactorAsync(idUsuario, enabled: true, secret: usuario.TwoFactorSecret);
            return Ok(new { message = "2FA activado correctamente." });
        }

        [HttpPost("2fa/disable")]
        [Authorize]
        public async Task<IActionResult> DisableTwoFactor()
        {
            var idUsuario = GetAuthenticatedUserId();
            await _usuarioService.SetTwoFactorAsync(idUsuario, enabled: false, secret: null);
            return Ok(new { message = "2FA desactivado." });
        }

        private int GetAuthenticatedUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(claim!);
        }

        private string GenerateAccessToken(int idUsuario, string correo, IEnumerable<string> roles)
        {
            var jwtId = Guid.NewGuid().ToString("N");
            var issuer = _configuration["Jwt:Issuer"] ?? "NexoVida";
            var audience = _configuration["Jwt:Audience"] ?? "NexoVida";

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, idUsuario.ToString()),
                new(ClaimTypes.Email, correo),
                new(JwtRegisteredClaimNames.Jti, jwtId),
                new(JwtRegisteredClaimNames.Iss, issuer),
                new(JwtRegisteredClaimNames.Aud, audience),
            };
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.Add(AccessTokenLifetime),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
