using ReportesAPI.Models;

namespace ReportesAPI.Interfaces
{
    public interface IClienteService
    {
        Task<IEnumerable<ClienteDto>> GetAllAsync();
    }
}
