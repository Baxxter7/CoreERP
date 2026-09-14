using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProveedoresAPI.Data;
using ProveedoresAPI.DTOs;
using ProveedoresAPI.Models;

namespace ProveedoresAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProveedoresController : ControllerBase
    {
        private readonly ProveedoresDbContext _context;
        public ProveedoresController(ProveedoresDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProveedorDto>>> GetAll()
        {
            var proveedores = await _context.Proveedores
                .AsNoTracking()
                .Select(p => new ProveedorDto
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    Telefono = p.Telefono,
                    Direccion = p.Direccion,
                    Estado = p.Estado,
                })
                .ToListAsync();

            return Ok(proveedores);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProveedorDto>> Get(int id)
        {
            var proveedor = await _context.Proveedores
                .Select(p => new ProveedorDto
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    Telefono = p.Telefono,
                    Direccion = p.Direccion,
                    Estado = p.Estado,
                })
                .Where(c => c.Id == id)
                .FirstOrDefaultAsync();

            if (proveedor == null)
                return NotFound();

            return Ok(proveedor);
        }

        [HttpPost]
        public async Task<ActionResult> Create(ProveedorDto proveedorDto)
        {
            var proveedor = new Proveedor
            {
                Nombre = proveedorDto.Nombre,
                Telefono = proveedorDto.Telefono,
                Direccion = proveedorDto.Direccion,
                Estado = true,
            };

            _context.Proveedores.Add(proveedor);
            await _context.SaveChangesAsync();

            proveedorDto.Id = proveedor.Id;

            return CreatedAtAction(
                nameof(Get),
                new { id = proveedor.Id },
                proveedorDto
            );
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ProveedorDto dto)
        {
            if (id != dto.Id)
            {
                return BadRequest();
            }

            var proveedor = await _context.Proveedores.FindAsync(id);

            if (proveedor == null)
                return NotFound();

            proveedor.Nombre = dto.Nombre;
            proveedor.Telefono = dto.Telefono;
            proveedor.Direccion = dto.Direccion;
            proveedor.Estado = dto.Estado;

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
