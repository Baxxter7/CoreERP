using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VentasAPI.Data;
using VentasAPI.DTOs;
using VentasAPI.Models;

namespace VentasAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class VentasController : ControllerBase
    {
        private readonly VentasDbContext _context;
        private readonly InventoryServiceClient _inventoryServiceClient;

        public VentasController(VentasDbContext context, InventoryServiceClient inventoryServiceClient)
        {
            _context = context;
            _inventoryServiceClient = inventoryServiceClient;
        }

        [HttpGet]
        public async Task<ActionResult<List<VentaDto>>> GetVentas()
        {
            var ventas = await _context.Ventas
                .AsNoTracking()
                .Select(v => new VentaDto
                {
                    Id = v.Id,
                    ClienteId = v.ClienteId,
                    SucursalId = v.SucursalId,
                    Fecha = v.Fecha,
                    Total = v.Total,
                    Detalles = v
                    .Detalles.Select(d => new DetalleVentaDto
                    {
                        Id = d.Id,
                        VentaId = d.VentaId,
                        ProductoId = d.ProductoId,
                        Cantidad = d.Cantidad,
                        PrecioUnitario = d.PrecioUnitario,
                    })
                    .ToList()

                })
                .ToListAsync();

            return Ok(ventas);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<VentaDto>> GetVenta(int id)
        {
            var venta = await _context.Ventas
                .AsNoTracking()
                .Where(v => v.Id == id)
                .Select(v => new VentaDto
                {
                    Id = v.Id,
                    ClienteId = v.ClienteId,
                    SucursalId = v.SucursalId,
                    Fecha = v.Fecha,
                    Total = v.Total,
                    Detalles = v
                    .Detalles.Select(d => new DetalleVentaDto
                    {
                        Id = d.Id,
                        VentaId = d.VentaId,
                        ProductoId = d.ProductoId,
                        Cantidad = d.Cantidad,
                        PrecioUnitario = d.PrecioUnitario,
                    })
                    .ToList()

                })
                .FirstOrDefaultAsync();

            if (venta is null)
                return NotFound();

            return Ok(venta);
        }

        [HttpPost]
        public async Task<ActionResult<VentaDto>> CrearVenta(VentaDto ventaDto)
        {
            ventaDto.Total = ventaDto
                    .Detalles?
                    .Sum(d => d.Cantidad * d.PrecioUnitario) ?? 0;

            var venta = new Venta
            {
                ClienteId = ventaDto.ClienteId,
                SucursalId = ventaDto.SucursalId,
                Fecha = ventaDto.Fecha,
                Total = ventaDto.Total,
                Detalles = ventaDto
                            .Detalles?
                            .Select(d => new DetalleVenta
                            {
                                ProductoId = d.ProductoId,
                                Cantidad = d.Cantidad,
                                PrecioUnitario = d.PrecioUnitario,
                            })
                            .ToList(),
            };

            _context.Ventas.Add(venta);
            await _context.SaveChangesAsync();

            ventaDto.Id = venta.Id;

            if (venta.Detalles.Any())
            {
                for (int i = 0; i < venta.Detalles.Count; i++)
                {
                    var detalle = venta.Detalles[i];
                    ventaDto.Detalles[i].Id = detalle.Id;
                    ventaDto.Detalles[i].VentaId = venta.Id;

                    var movimiento = new MovimientoDto
                    {
                        Tipo = "Salida",
                        ProductoId = detalle.ProductoId,
                        SucursalId = venta.SucursalId,
                        Cantidad = detalle.Cantidad,
                        Fecha = venta.Fecha,
                        Origen = "Venta",
                        ReferenciaId = venta.Id,
                    };

                    await _inventoryServiceClient.RegistrarMovimientoYactualizarStockAsync(movimiento);
                }
            }

            return CreatedAtAction(nameof(GetVenta), new { id = ventaDto.Id }, ventaDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVenta(int id, VentaDto ventaDto)
        {
            if (id != ventaDto.Id)
                return BadRequest("El ID de la venta no coincide con el ID del DTO");

            var venta = await _context
                .Ventas.Include(v => v.Detalles)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (venta == null)
                return NotFound();

            int sucursalAnterior = venta.SucursalId;

            venta.SucursalId = ventaDto.SucursalId;
            venta.Fecha = ventaDto.Fecha;


            var detallesPrevios = venta.
                Detalles.Select(d => new DetalleVenta
                {
                    Id = d.Id,
                    ProductoId = d.ProductoId,
                    Cantidad = d.Cantidad,
                    PrecioUnitario = d.PrecioUnitario,
                }).ToList();

            var detallesAEliminar = venta
                .Detalles
                .Where(d => !ventaDto.Detalles
                                        .Any(dto => dto.Id == d.Id))
                                        .ToList();

            _context.DetallesVenta.RemoveRange(detallesAEliminar);

            foreach (var detalleDto in ventaDto.Detalles)
            {
                var detalleExistente = venta
                    .Detalles
                    .FirstOrDefault(d => d.Id == detalleDto.Id);

                if (detalleExistente is not null)
                {
                    detalleExistente.ProductoId = detalleDto.ProductoId;
                    detalleExistente.Cantidad = detalleDto.Cantidad;
                    detalleExistente.PrecioUnitario = detalleDto.PrecioUnitario;
                }
                else
                {
                    venta.Detalles.Add(new DetalleVenta
                    {
                        ProductoId = detalleDto.ProductoId,
                        Cantidad = detalleDto.Cantidad,
                        PrecioUnitario = detalleDto.PrecioUnitario
                    });
                }
            }

            venta.Total = ventaDto
                           .Detalles?
                           .Sum(d => d.Cantidad * d.PrecioUnitario) ?? 0;

            await _context.SaveChangesAsync();

            if (sucursalAnterior != ventaDto.SucursalId)
            {
                foreach (var detalle in venta.Detalles)
                {
                    var entrada = new MovimientoDto
                    {
                        Tipo = "Entrada",
                        ProductoId = detalle.ProductoId,
                        SucursalId = sucursalAnterior,
                        Cantidad = detalle.Cantidad,
                        Fecha = DateTime.Now,
                        Origen = "Reubicacion de sucursal (edicion de venta)",
                        ReferenciaId = venta.Id,
                    };

                    await _inventoryServiceClient.RegistrarMovimientoYactualizarStockAsync(entrada);

                    var salida = new MovimientoDto
                    {
                        Tipo = "Salida",
                        ProductoId = detalle.ProductoId,
                        SucursalId = venta.SucursalId,
                        Cantidad = detalle.Cantidad,
                        Fecha = DateTime.Now,
                        Origen = "Reubicacion de sucursal (edicion de venta)",
                        ReferenciaId = venta.Id,
                    };

                    await _inventoryServiceClient.RegistrarMovimientoYactualizarStockAsync(salida);
                }
            }
            else
            {
                foreach (var detalleActual in venta.Detalles)
                {
                    var detallePrevio = detallesPrevios
                                                       .FirstOrDefault(d => d.Id == detalleActual.Id);

                    int cantidadAnterior = detallePrevio?.Cantidad ?? 0;
                    int cantidadNueva = detalleActual.Cantidad;
                    int diferencia = cantidadNueva - cantidadAnterior;

                    if (diferencia > 0)
                    {
                        var salida = new MovimientoDto
                        {
                            Tipo = "Salida",
                            ProductoId = detalleActual.ProductoId,
                            SucursalId = venta.SucursalId,
                            Cantidad = diferencia,
                            Fecha = DateTime.Now,
                            Origen = "Ajuste por aumento de cantidad en venta.",
                            ReferenciaId = venta.Id,
                        };

                        await _inventoryServiceClient.RegistrarMovimientoYactualizarStockAsync(salida);
                    }
                    else if (diferencia < 0)
                    {
                        var entrada = new MovimientoDto
                        {
                            Tipo = "Entrada",
                            ProductoId = detalleActual.ProductoId,
                            SucursalId = venta.SucursalId,
                            Cantidad = -diferencia,
                            Fecha = DateTime.Now,
                            Origen = "Edicion de venta (reduccion de cantidad)",
                            ReferenciaId = venta.Id,
                        };

                        await _inventoryServiceClient.RegistrarMovimientoYactualizarStockAsync(entrada);
                    }
                }

                foreach (var eliminado in detallesAEliminar)
                {
                    var entrada = new MovimientoDto
                    {
                        Tipo = "Entrada",
                        ProductoId = eliminado.ProductoId,
                        SucursalId = venta.SucursalId,
                        Cantidad = eliminado.Cantidad,
                        Fecha = DateTime.Now,
                        Origen = "Devolucion por eliminacion de producto en edicion de venta.",
                        ReferenciaId = venta.Id,
                    };

                    await _inventoryServiceClient.RegistrarMovimientoYactualizarStockAsync(entrada);
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVenta(int id)
        {
            var venta = await _context
                .Ventas.Include(v => v.Detalles)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (venta == null)
                return NotFound();

            _context.DetallesVenta.RemoveRange(venta.Detalles);
            _context.Ventas.Remove(venta);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
