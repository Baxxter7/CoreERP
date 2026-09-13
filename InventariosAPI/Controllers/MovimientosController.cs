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
    public class MovimientosController : ControllerBase
    {
        private readonly InventarioDbContext _context;

        public MovimientosController(InventarioDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MovimientoDto>>> Get()
        {
            var movimientos = await _context.Movimientos
                .AsNoTracking()
                .Select(m => new MovimientoDto
                {
                    Id = m.Id,
                    Tipo = m.Tipo,
                    ProductoId = m.ProductoId,
                    SucursalId = m.SucursalId,
                    Cantidad = m.Cantidad,
                    Fecha = m.Fecha,
                    Origen = m.Origen,
                    ReferenciaId = m.ReferenciaId,
                })
                .ToListAsync();

            return Ok(movimientos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MovimientoDto>> GetMovimiento(int id)
        {
            var movimiento = await _context.Movimientos
                .Where(m => m.Id == id)
                .Select(m => new MovimientoDto
                {
                    Id = m.Id,
                    Tipo = m.Tipo,
                    ProductoId = m.ProductoId,
                    SucursalId = m.SucursalId,
                    Cantidad = m.Cantidad,
                    Fecha = m.Fecha,
                    Origen = m.Origen,
                    ReferenciaId = m.ReferenciaId
                })
                .FirstOrDefaultAsync();

            if (movimiento is null)
                return NotFound();

            return Ok(movimiento);
        }

        [HttpPost]
        public async Task<ActionResult<MovimientoDto>> CreateMovimiento(MovimientoDto movimientoDto)
        {
            var movimiento = FromDto(movimientoDto);
            movimiento.Fecha = DateTime.UtcNow;

            _context.Movimientos.Add(movimiento);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetMovimiento),
                new { id = movimiento.Id },
                ToDto(movimiento)
            );
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMovimiento(int id, MovimientoDto movimientoDto)
        {
            if (id != movimientoDto.Id)
            {
                return BadRequest();
            }

            var movimiento = await _context.Movimientos.FindAsync(id);

            if (movimiento == null)
            {
                return NotFound();
            }

            movimiento.Tipo = movimientoDto.Tipo;
            movimiento.ProductoId = movimientoDto.ProductoId;
            movimiento.SucursalId = movimientoDto.SucursalId;
            movimiento.Cantidad = movimientoDto.Cantidad;
            movimiento.Fecha = movimientoDto.Fecha;
            movimiento.Origen = movimientoDto.Origen;
            movimiento.ReferenciaId = movimientoDto.ReferenciaId;

            _context.Entry(movimiento).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMovimiento(int id)
        {
            var movimiento = await _context.Movimientos.FindAsync(id);

            if (movimiento == null)
            {
                return NotFound();
            }

            _context.Movimientos.Remove(movimiento);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private MovimientoDto ToDto(Movimiento movimiento) =>
            new()
            {
                Id = movimiento.Id,
                Tipo = movimiento.Tipo,
                ProductoId = movimiento.ProductoId,
                SucursalId = movimiento.SucursalId,
                Cantidad = movimiento.Cantidad,
                Fecha = movimiento.Fecha,
                Origen = movimiento.Origen,
                ReferenciaId = movimiento.ReferenciaId,
            };

        private Movimiento FromDto(MovimientoDto dto) =>
            new()
            {
                Id = dto.Id,
                Tipo = dto.Tipo,
                ProductoId = dto.ProductoId,
                SucursalId = dto.SucursalId,
                Cantidad = dto.Cantidad,
                Fecha = dto.Fecha,
                Origen = dto.Origen,
                ReferenciaId = dto.ReferenciaId,
            };
    }
}
