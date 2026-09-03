using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApi.Interface;
using WebApi.Models;

namespace WebApi.Helpers
{
    public static class DataScope
    {
        public static int? GetUserId(HttpContext context)
        {
            var claim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(claim, out var id) ? id : null;
        }

        public static bool EsAdministrador(HttpContext context) =>
            context.User.IsInRole("Administrador");

        public static async Task<HashSet<int>> ObtenerPacientesPermitidosAsync(HttpContext context)
        {
            var permitidos = new HashSet<int>();
            var idUsuario = GetUserId(context);
            if (idUsuario == null)
            {
                return permitidos;
            }

            var services = context.RequestServices;

            if (context.User.IsInRole("Paciente"))
            {
                var pacienteService = services.GetRequiredService<IPacienteService>();
                foreach (var p in await pacienteService.GetAllAsync())
                {
                    if (p.IdUsuario == idUsuario)
                    {
                        permitidos.Add(p.IdPaciente);
                    }
                }
                return permitidos;
            }

            if (context.User.IsInRole("ProfesionalSalud"))
            {
                 var profesionalService = services.GetRequiredService<IProfesionalSaludService>();
                 var tratamientoService = services.GetRequiredService<ITratamientoService>();

                 int? idProfesional = null;
                 foreach (var p in await profesionalService.GetAllAsync())
                 {
                     if (p.IdUsuario == idUsuario && p.Activo != false)
                     {
                         idProfesional = p.IdProfesional;
                         break;
                     }
                 }
                 if (idProfesional == null)
                 {
                     return permitidos;
                 }

                 foreach (var t in await tratamientoService.GetAllAsync())
                 {
                     if (t.IdProfesional == idProfesional.Value)
                     {
                         permitidos.Add(t.IdPaciente);
                     }
                 }
                 return permitidos;
            }

            if (context.User.IsInRole("Familiar"))
            {
                var familiarService = services.GetRequiredService<IFamiliarService>();
                var asistenteService = services.GetRequiredService<IAsistentePacienteService>();

                int? idFamiliar = null;
                foreach (var f in await familiarService.GetAllAsync())
                {
                    if (f.IdUsuario == idUsuario)
                    {
                        idFamiliar = f.IdFamiliar;
                        break;
                    }
                }
                if (idFamiliar == null)
                {
                    return permitidos;
                }

                foreach (var a in await asistenteService.GetAllAsync())
                {
                    if (a.IdFamiliar == idFamiliar.Value && a.Activo != false)
                    {
                        permitidos.Add(a.IdPaciente);
                    }
                }
                return permitidos;
            }

            return permitidos;
        }

        public static async Task<bool> PuedeAccederAPacienteAsync(HttpContext context, int idPaciente)
        {
            var permitidos = await ObtenerPacientesPermitidosAsync(context);
            return permitidos.Contains(idPaciente);
        }
    }
}