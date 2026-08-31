using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace WebApi.Helpers
{
    public static class BolaChecker
    {
        public static Task<bool> EsPropietario(HttpContext context, int idPaciente)
            => DataScope.PuedeAccederAPacienteAsync(context, idPaciente);
    }
}