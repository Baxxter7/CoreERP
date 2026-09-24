using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;
using ReportesAPI.Data;
using ReportesAPI.Interfaces;

namespace ReportesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReportesController : ControllerBase
    {
        private readonly IClienteService _clienteService;

        public ReportesController(IClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        [HttpGet("clientes/pdf")]
        public async Task<IActionResult> GenerarReporteClientes()
        {
            var clientes = (await _clienteService.GetAllAsync()).ToList();
            var documento = new ReporteClientesDocument(clientes);
            var pdf = documento.GeneratePdf();

            return File(
                pdf,
                "application/pdf",
                $"ReportesClientes_{DateTime.Now:yyyyMMddHHmmss}.pdf"
            );
        }
    }
}
