using ComprasAPI.Data;
using ComprasAPI.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ComprasAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RecepcionesController : ControllerBase
    {
        private readonly ComprasDbContext _dbContext;
        private readonly InventoryServiceClient _inventoryServiceClient;

        public RecepcionesController(ComprasDbContext dbContext, InventoryServiceClient inventoryServiceClient)
        {
            _dbContext = dbContext;
            _inventoryServiceClient = inventoryServiceClient;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RecepcionDto>>> GetRecepciones()
        {
            var recepciones = await _dbContext.Recepciones
                .AsNoTracking()
                //.Include(r => r.Detalles)
                .Select(r => new RecepcionDto
                {
                    Id = r.Id,
                    OrdenCompraId = r.OrdenCompraId,
                    SucursalId = r.SucursalId,
                    Fecha = r.Fecha,
                    Observaciones = r.Observaciones,
                    Detalles =
                        r.Detalles.Select(d => new DetalleRecepcionDto
                        {
                            Id = d.Id,
                            RecepcionId = d.RecepcionId,
                            ProductoId = d.ProductoId,
                            CantidadRecibida = d.CantidadRecibida
                        }).ToList()
                })
                .ToListAsync();

            return Ok(recepciones);
        }

    }
}
