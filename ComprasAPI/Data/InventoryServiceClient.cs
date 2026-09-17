using ComprasAPI.DTOs;

namespace ComprasAPI.Data
{
    public class InventoryServiceClient
    {
        private readonly HttpClient _httpClient;

        public InventoryServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task RegistrarMovimientoYactualizarStockAsync(MovimientoDto movimientoDto)
        {
            var response = await _httpClient.PostAsJsonAsync(
                "api/stock/registrar-movimiento",
                movimientoDto
            );

            response.EnsureSuccessStatusCode();
        }

    }
}
