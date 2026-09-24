using ReportesAPI.Interfaces;
using ReportesAPI.Models;

namespace ReportesAPI.Services
{
    public class ClienteService : IClienteService
    {
        private readonly HttpClient _httpClient;

        public ClienteService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<ClienteDto>> GetAllAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<ClienteDto>>("api/clientes");
        }
    }
}
