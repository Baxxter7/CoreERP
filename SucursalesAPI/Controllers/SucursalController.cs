using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SucursalesAPI.Data;
using SucursalesAPI.DTOs;
using SucursalesAPI.Models;

namespace SucursalesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SucursalController : ControllerBase
    {
        private SucursaDbContext _context;

        public SucursalController(SucursaDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SucursalDto>>> GetAll()
        {
            var sucursales = await _context.Sucursales
                .AsNoTracking()
                .Select(s => new SucursalDto
                {
                    Id = s.Id,
                    Nombre = s.Nombre,
                    Ubicacion = s.Ubicacion,
                    Estado = s.Estado
                })
                .ToListAsync(); 

            return Ok(sucursales);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SucursalDto>> Get(int id)
        {
            var sucursal = await _context.Sucursales
                .AsNoTracking()
                .Where(s => s.Id == id)
                .Select(s => new SucursalDto
                {
                    Id = s.Id,
                    Nombre = s.Nombre,
                    Ubicacion = s.Ubicacion,
                    Estado = s.Estado
                })
                .FirstOrDefaultAsync();

            if (sucursal is null) return NotFound();

            return Ok(sucursal);
        }

        [HttpPost]
        public async Task<ActionResult<SucursalDto>> Create([FromBody] SucursalDto sucursalDto)
        {
            if (string.IsNullOrWhiteSpace(sucursalDto.Nombre))
                return BadRequest("El nombre de la sucursal es obligatorio.");

            var sucursal = new Sucursal
            {
                Nombre = sucursalDto.Nombre,
                Ubicacion = sucursalDto.Ubicacion,
                Estado = sucursalDto.Estado
            };

            _context.Sucursales.Add(sucursal);
            await _context.SaveChangesAsync();

            sucursalDto.Id = sucursal.Id;
            return CreatedAtAction(nameof(Get), new { id = sucursal.Id }, sucursalDto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] SucursalDto sucursalDto)
        {
            if (id != sucursalDto.Id) 
                return BadRequest("El Id de la sucursal no coincide.");
            
            var sucursal = await _context.Sucursales.FindAsync(id);

            if (sucursal is null)
                return NotFound("Sucursal no encontrada.");

            sucursal.Nombre = sucursalDto.Nombre;
            sucursal.Ubicacion = sucursalDto.Ubicacion;
            sucursal.Estado = sucursalDto.Estado;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id) 
        {
            var sucursal = await _context.Sucursales.FindAsync(id);

            if (sucursal is null)
                return NotFound("Sucursal no encontrada.");

            sucursal.Estado = false;

            await _context.SaveChangesAsync();

            return NoContent();
        }
    } 
}
