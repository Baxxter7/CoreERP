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
    public class OrdenComprasController : ControllerBase
    {
        private readonly ComprasDbContext _dbContext;

        public OrdenComprasController(ComprasDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrdenCompraDto>>> GetAll()
        {
            var compras = await _dbContext.OrdenCompras
                .AsNoTracking()
                .Include(c => c.Detalles)
                .ToListAsync();

            var ordenIds = compras.Select(c => c.Id)
                .ToList();

            var recepcionesPorOrden = await _dbContext.Recepciones
                .AsNoTracking()
                .Where(r => ordenIds.Contains(r.OrdenCompraId))
                .Include(r => r.Detalles)
                .ToListAsync();

            var comprasDto = compras
                .Select(c =>
                {
                    var recepciones = recepcionesPorOrden
                    .Where(r => r.OrdenCompraId == c.Id)
                    .ToList();

                    bool tieneRecepcion = recepciones.Any();
                    bool completa = true;

                    foreach (var detalle in c.Detalles)
                    {
                        var recibida = recepciones
                        .SelectMany(r => r.Detalles)
                        .Where(rd => rd.ProductoId == detalle.ProductoId)
                        .Sum(rd => rd.CantidadRecibida);

                        if (recibida < detalle.Cantidad)
                        {
                            completa = false;
                            break;
                        }
                    }

                    string estadoRecepcion;

                    if (!tieneRecepcion)
                    {
                        estadoRecepcion = "Sin Recibir";
                    }
                    else if (completa)
                    {
                        estadoRecepcion = "Completamente recibida";
                    }
                    else
                    {
                        estadoRecepcion = "Parcialmente recibida";
                    }

                    return new OrdenCompraDto
                    {
                        Id = c.Id,
                        ProveedorId = c.ProveedorId,
                        SucursalId = c.SucursalId,
                        Fecha = c.Fecha,
                        Estado = c.Estado,
                        Total = c.Total,
                        RecepcionEstado = estadoRecepcion,
                        Detalles = c
                        .Detalles.Select(d => new DetalleOrdenDto
                        {
                            Id = d.Id,
                            OrdenCompraId = d.OrdenCompraId,
                            ProductoId = d.ProductoId,
                            Cantidad = d.Cantidad,
                            PrecioUnitario = d.PrecioUnitario,
                        }).ToList()

                    };
                }).ToList();

            return Ok(comprasDto);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrdenCompraDto>> GetCompra(int id)
        {
            var compra = await _dbContext.OrdenCompras
                .AsNoTracking()
                .Include(c => c.Detalles)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (compra is null)
                return NotFound();

            var recepciones = await _dbContext
                .Recepciones.Where(r => r.OrdenCompraId == compra.Id)
                .Include(r => r.Detalles)
                .ToListAsync();

            bool tieneRecepcion = recepciones.Any();
            bool completa = true;

            foreach (var detalle in compra.Detalles)
            {
                var recibida = recepciones
                .SelectMany(r => r.Detalles)
                .Where(rd => rd.ProductoId == detalle.ProductoId)
                .Sum(rd => rd.CantidadRecibida);

                if (recibida < detalle.Cantidad)
                {
                    completa = false;
                    break;
                }
            }

            string estadoRecepcion;

            if (!tieneRecepcion)
            {
                estadoRecepcion = "Sin Recibir";
            }
            else if (completa)
            {
                estadoRecepcion = "Completamente recibida";
            }
            else
            {
                estadoRecepcion = "Parcialmente recibida";
            }

            var compraDto = new OrdenCompraDto
            {
                Id = compra.Id,
                ProveedorId = compra.ProveedorId,
                SucursalId = compra.SucursalId,
                Fecha = compra.Fecha,
                Estado = compra.Estado,
                Total = compra.Total,
                RecepcionEstado = estadoRecepcion,
                Detalles = compra
                 .Detalles.Select(d => new DetalleOrdenDto
                 {
                     Id = d.Id,
                     OrdenCompraId = d.OrdenCompraId,
                     ProductoId = d.ProductoId,
                     Cantidad = d.Cantidad,
                     PrecioUnitario = d.PrecioUnitario,
                 }).ToList()

            };

            return Ok(compraDto);
        }

        [HttpPost]
        public async Task<ActionResult<OrdenCompraDto>> CrearCompra([FromBody] OrdenCompraDto compraDto)
        {
            var compra = new OrdenCompra
            {
                ProveedorId = compraDto.ProveedorId,
                SucursalId = compraDto.SucursalId,
                Fecha = compraDto.Fecha,
                Estado = compraDto.Estado,
                Detalles = compraDto
                    .Detalles?
                    .Select(d => new DetalleOrden
                    {
                        ProductoId = d.ProductoId,
                        Cantidad = d.Cantidad,
                        PrecioUnitario = d.PrecioUnitario
                    })
                    .ToList()
            };

            compra.Total = compra.Detalles?.Sum(d => d.Cantidad * d.PrecioUnitario) ?? 0;
            _dbContext.Add(compra);
            await _dbContext.SaveChangesAsync();

            OrdenCompraDto dto = new OrdenCompraDto
            {
                Id = compra.Id,
                ProveedorId = compra.ProveedorId,
                SucursalId = compra.SucursalId,
                Fecha = compra.Fecha,
                Estado = compra.Estado,
                Total = compra.Total,
                Detalles = compra
                        .Detalles?.Select(d => new DetalleOrdenDto
                        {
                            Id = d.Id,
                            OrdenCompraId = d.OrdenCompraId,
                            ProductoId = d.ProductoId,
                            Cantidad = d.Cantidad,
                            PrecioUnitario = d.PrecioUnitario
                        }).ToList()
            };

            return CreatedAtAction(nameof(GetCompra), new {id = compra.Id}, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarCompra(int id, [FromBody] OrdenCompraDto compraDto)
        {
            var compra = await _dbContext.OrdenCompras
                .Include(c => c.Detalles)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (compra is null)
                return NotFound();

            compra.ProveedorId = compraDto.ProveedorId;
            compra.SucursalId = compraDto.SucursalId;
            compra.Fecha = compraDto.Fecha;
            compra.Estado = compraDto.Estado;

            _dbContext.RemoveRange(compra.Detalles);

            compra.Detalles = compraDto.Detalles?
                .Select(d => new DetalleOrden
                {
                    ProductoId = d.ProductoId,
                    Cantidad = d.Cantidad,
                    PrecioUnitario = d.PrecioUnitario,
                })
                .ToList();
      
            compra.Total = compra.Detalles?.Sum(d => d.Cantidad * d.PrecioUnitario) ?? 0;

            await _dbContext.SaveChangesAsync();

            return NoContent();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarCompra(int id)
        {
            var compra = await _dbContext
                .OrdenCompras
                .Include(c => c.Detalles)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (compra == null)
                return NotFound();

            _dbContext.OrdenCompras.Remove(compra);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
