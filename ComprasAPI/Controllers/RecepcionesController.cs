using ComprasAPI.Data;
using ComprasAPI.DTOs;
using ComprasAPI.Models;
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

        [HttpGet("{id}")]
        public async Task<ActionResult<RecepcionDto>> GetRecepcion(int id)
        {
            var recepcion = await _dbContext.Recepciones
                .AsNoTracking()
                .Where(r => r.Id == id)
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
                .FirstOrDefaultAsync();

            if (recepcion is null)
                return NotFound();

            return Ok(recepcion);
        }

        [HttpPost]
        public async Task<ActionResult<RecepcionDto>> CrearRecepcion([FromBody] RecepcionDto recepcionDto)
        {
            var recepcion = new Recepcion
            {
                OrdenCompraId = recepcionDto.OrdenCompraId,
                SucursalId = recepcionDto.SucursalId,
                Fecha = recepcionDto.Fecha,
                Observaciones = recepcionDto.Observaciones,
                Detalles = recepcionDto
                .Detalles.Select(d => new DetalleRecepcion
                {
                    ProductoId = d.ProductoId,
                    CantidadRecibida = d.CantidadRecibida,
                }).ToList()
            };

            _dbContext.Recepciones.Add(recepcion);
            await _dbContext.SaveChangesAsync();

            recepcionDto.Id = recepcion.Id;

            if (recepcion.Detalles.Any())
            {
                for (int i = 0; i < recepcion.Detalles.Count; i++)
                {
                    var detalle = recepcion.Detalles[i];
                    recepcionDto.Detalles[i].Id = detalle.Id;
                    recepcionDto.Detalles[i].RecepcionId = detalle.RecepcionId;

                    MovimientoDto movimiento = new MovimientoDto
                    {
                        Tipo = "Entrada",
                        ProductoId = detalle.ProductoId,
                        SucursalId = recepcion.SucursalId,
                        Cantidad = detalle.CantidadRecibida,
                        Fecha = recepcion.Fecha,
                        Origen = "Recepción",
                        ReferenciaId = recepcion.Id,
                    };

                    await _inventoryServiceClient.RegistrarMovimientoYactualizarStockAsync(movimiento);
                }
            }

            await VerificarYDesactivarOrdenSiCompleta(recepcion.OrdenCompraId);
            return CreatedAtAction(nameof(GetRecepcion), new { id = recepcion.Id }, recepcionDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarRecepcion(int id, [FromBody] RecepcionDto recepcionDto)
        {
            var recepcion = await _dbContext
                .Recepciones
                .Include(r => r.Detalles)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (recepcion is null)
            {
                return NotFound();
            }

            recepcion.OrdenCompraId = recepcionDto.OrdenCompraId;
            recepcion.SucursalId = recepcionDto.SucursalId;
            recepcion.Fecha = recepcionDto.Fecha;
            recepcion.Observaciones = recepcionDto.Observaciones;

            _dbContext.DetalleRecepciones.RemoveRange(recepcion.Detalles);

            recepcion.Detalles = recepcionDto.Detalles.Select(d => new DetalleRecepcion
            {
                ProductoId = d.ProductoId,
                CantidadRecibida = d.CantidadRecibida
            }).ToList();

            await _dbContext.SaveChangesAsync();

            recepcionDto.Id = recepcion.Id;

            if (recepcion.Detalles.Any())
            {
                for (int i = 0; i < recepcion.Detalles.Count; i++)
                {
                    var detalle = recepcion.Detalles[i];
                    recepcionDto.Detalles[i].Id = detalle.Id;
                    recepcionDto.Detalles[i].RecepcionId = detalle.RecepcionId;

                    MovimientoDto movimiento = new MovimientoDto
                    {
                        Tipo = "Entrada",
                        ProductoId = detalle.ProductoId,
                        SucursalId = recepcion.SucursalId,
                        Cantidad = detalle.CantidadRecibida,
                        Fecha = recepcion.Fecha,
                        Origen = "Recepción",
                        ReferenciaId = recepcion.Id,
                    };

                    await _inventoryServiceClient.RegistrarMovimientoYactualizarStockAsync(movimiento);
                }
            }

            await VerificarYDesactivarOrdenSiCompleta(recepcion.OrdenCompraId);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarRecepcion(int id)
        {
            var recepcion = await _dbContext.Recepciones
                .Include(r => r.Detalles)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (recepcion is null)
            {
                return NotFound();
            }

            int ordenCompraId = recepcion.OrdenCompraId;

            _dbContext.Remove(recepcion);

            await _dbContext.SaveChangesAsync();

            await VerificarYDesactivarOrdenSiCompleta(ordenCompraId);

            return NoContent();

        }

        private async Task VerificarYDesactivarOrdenSiCompleta(int ordenCompraId)
        {
            var orden = await _dbContext
                .OrdenCompras
                .Include(o => o.Detalles)
                .FirstOrDefaultAsync(o => o.Id == ordenCompraId);

            if (orden is null)
                return;

            bool estaCompleta = true;

            foreach (var detalle in orden.Detalles)
            {
                var recepcionIds = await _dbContext
                    .Recepciones.Where(r => r.OrdenCompraId == ordenCompraId)
                    .Select(r => r.Id)
                    .ToListAsync();

                var cantidadRecibidaTotal = await _dbContext
                    .DetalleRecepciones
                    .Where(d => recepcionIds.Contains(d.RecepcionId)
                                && d.ProductoId == detalle.ProductoId)
                    .SumAsync(d => (int?)d.CantidadRecibida) ?? 0;

                if (cantidadRecibidaTotal < detalle.Cantidad)
                {
                    estaCompleta = false;
                    break;
                }

                orden.Estado = estaCompleta;
                await _dbContext.SaveChangesAsync();
            }
        }

    }
}
