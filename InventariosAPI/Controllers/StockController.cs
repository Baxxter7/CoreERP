using InventariosAPI.Data;
using InventariosAPI.DTOs;
using InventariosAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventariosAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StockController : ControllerBase
    {
        private readonly InventarioDbContext _context;

        public StockController(InventarioDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<StockDto>>> GetAll()
        {
            var stocks = await _context.Stocks
                .AsNoTracking()
                .Select(s => new StockDto
                {
                    Id = s.Id,
                    ProductoId = s.ProductoId,
                    SucursalId = s.SucursalId,
                    Cantidad = s.Cantidad,
                })
                .ToListAsync();

            return Ok(stocks);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<StockDto>> GetStock(int id)
        {
            var stock = await _context.Stocks
                .Where(s => s.Id == id)
                .Select(s => new StockDto
                {
                    Id = s.Id,
                    ProductoId = s.ProductoId,
                    SucursalId = s.SucursalId,
                    Cantidad = s.Cantidad,
                })
                .FirstOrDefaultAsync();

            if (stock is null)
                return NotFound();

            return Ok(stock);
        }

        [HttpPost]
        public async Task<ActionResult<StockDto>> CreateStock(StockDto stockDto)
        {
            var stock = FromDto(stockDto);

            _context.Stocks.Add(stock);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetStock),
                new { id = stock.Id },
                ToDto(stock)
            );
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStock(int id, StockDto dto)
        {
            if (id != dto.Id)
            {
                return BadRequest();
            }

            var stock = await _context.Stocks.FindAsync(id);

            if (stock == null)
            {
                return NotFound();
            }

            stock.ProductoId = dto.ProductoId;
            stock.SucursalId = dto.SucursalId;
            stock.Cantidad = dto.Cantidad;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStock(int id)
        {
            var stock = await _context.Stocks.FindAsync(id);

            if (stock == null)
            {
                return NotFound();
            }

            _context.Stocks.Remove(stock);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("StockPorSucursal")]
        public async Task<ActionResult<IEnumerable<StockDto>>> GetProductosConStockPorSucursal(int sucursalId)
        {
            var stock = await _context.Stocks
                        .AsNoTracking()
                        .Where(s => s.SucursalId == sucursalId && s.Cantidad > 0)
                        .Select(s => new StockDto
                        {
                            Id = s.Id,
                            ProductoId = s.ProductoId,
                            SucursalId = s.SucursalId,
                            Cantidad = s.Cantidad,
                        })
                        .ToListAsync();

            if (!stock.Any())
            {
                return NotFound();
            }

            return Ok(stock);
        }

        [HttpPost("registrar-movimiento")]
        public async Task<IActionResult> RegistrarMovimientoStock([FromBody] MovimientoDto movimientoDto)
        {
            if (movimientoDto.Cantidad <= 0)
                return BadRequest("La cantidad debe ser mayor que cero.");

            if (movimientoDto.Tipo != "Entrada" &&
                movimientoDto.Tipo != "Salida")
            {
                return BadRequest("El tipo de movimiento debe ser Entrada o Salida.");
            }

            using var transaccion = await _context.Database.BeginTransactionAsync();

            var stock = await _context.Stocks
                        .FirstOrDefaultAsync(s =>
                            s.ProductoId == movimientoDto.ProductoId &&
                            s.SucursalId == movimientoDto.SucursalId);

            if (stock is null)
            {
                if (movimientoDto.Tipo == "Salida")
                {
                    return BadRequest(
                        "No existe stock disponible para realizar la salida.");
                }

                stock = new Stock
                {
                    ProductoId = movimientoDto.ProductoId,
                    SucursalId = movimientoDto.SucursalId,
                    Cantidad = movimientoDto.Cantidad
                };

                _context.Stocks.Add(stock);
            }
            else
            {
                if (movimientoDto.Tipo == "Entrada")
                {
                    stock.Cantidad += movimientoDto.Cantidad;
                }
                else
                {
                    if (stock.Cantidad < movimientoDto.Cantidad)
                    {
                        return BadRequest(
                            "No hay suficiente stock para realizar la salida.");
                    }

                    stock.Cantidad -= movimientoDto.Cantidad;
                }
            }

            var movimiento = new Movimiento
            {
                ProductoId = movimientoDto.ProductoId,
                SucursalId = movimientoDto.SucursalId,
                Cantidad = movimientoDto.Cantidad,
                Tipo = movimientoDto.Tipo,
                Fecha = DateTime.UtcNow,
                Origen = movimientoDto.Origen,
                ReferenciaId = movimientoDto.ReferenciaId
            };

            _context.Movimientos.Add(movimiento);

            await _context.SaveChangesAsync();
            await transaccion.CommitAsync();
            return Ok();
        }

        private StockDto ToDto(Stock stock) => new()
        {
            Id = stock.Id,
            ProductoId = stock.ProductoId,
            SucursalId = stock.SucursalId,
            Cantidad = stock.Cantidad,
        };

        private Stock FromDto(StockDto stock) => new()
        {
            Id = stock.Id,
            ProductoId = stock.ProductoId,
            SucursalId = stock.SucursalId,
            Cantidad = stock.Cantidad,
        };
    }
}
