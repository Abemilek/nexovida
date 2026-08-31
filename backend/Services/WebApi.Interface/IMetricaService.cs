using System.Threading.Tasks;

namespace WebApi.Interface
{
    public interface IMetricaService
    {
        Task<object> GetGlobalMetricsAsync();
    }
}
