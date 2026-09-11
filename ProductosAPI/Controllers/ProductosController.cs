using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductosAPI.Data;
using ProductosAPI.DTO_s;
using ProductosAPI.Models;

namespace ProductosAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductosController : ControllerBase
    {
        private readonly ProductoDbContext _context;

        public ProductosController(ProductoDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductoDto>>> GetAll()
        {
            var productos = await _context.Productos
                .AsNoTracking()
                .Select(p => new ProductoDto
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    Codigo = p.Codigo,
                    Descripcion = p.Descripcion,
                    PrecioVenta = p.PrecioVenta,
                    CategoriaId = p.CategoriaId,
                    Estado = p.Estado,
                })
                .ToListAsync();

            return Ok(productos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductoDto>> Get(int id)
        {
            var producto = await _context.Productos
                .Select(producto => new ProductoDto
                {
                    Id = producto.Id,
                    Nombre = producto.Nombre,
                    Codigo = producto.Codigo,
                    Descripcion = producto.Descripcion,
                    PrecioVenta = producto.PrecioVenta,
                    CategoriaId = producto.CategoriaId,
                    Estado = producto.Estado,
                })
                .Where(p => p.Id == id)
                .FirstOrDefaultAsync();

            if (producto == null)
            {
                return NotFound();
            }

            return Ok(producto);
        }

        [HttpPost]
        public async Task<ActionResult<ProductoDto>> Post([FromBody] ProductoDto productoDTO)
        {
            if (string.IsNullOrWhiteSpace(productoDTO.Nombre))
            {
                return BadRequest("El nombre del producto es obligatorio.");
            }

            if (productoDTO.PrecioVenta <= 0)
            {
                return BadRequest("El precio de venta debe ser mayor que cero.");
            }

            var categoria = await _context.Categorias.FindAsync(productoDTO.CategoriaId);

            if (categoria is null) return BadRequest("La categoria especificada no existe o esta inactiva");

            var producto = new Producto
            {
                Nombre = productoDTO.Nombre,
                Codigo = productoDTO.Codigo,
                Descripcion = productoDTO.Descripcion,
                PrecioVenta = productoDTO.PrecioVenta,
                CategoriaId = productoDTO.CategoriaId,
                Estado = productoDTO.Estado,
            };

            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();

            productoDTO.Id = producto.Id;

            return CreatedAtAction(
                nameof(Get),
                new { id = producto.Id },
                productoDTO
            );
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] ProductoDto productoDTO)
        {
            if (id != productoDTO.Id)
            {
                return BadRequest("El ID del producto no coincide.");
            }

            var producto = await _context.Productos.FindAsync(id);

            if (producto == null)
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(productoDTO.Nombre))
            {
                return BadRequest("El nombre del producto es obligatorio.");
            }

            if (productoDTO.PrecioVenta <= 0)
            {
                return BadRequest("El precio de venta debe ser mayor que cero.");
            }

            var categoria = await _context.Categorias.FindAsync(productoDTO.CategoriaId);

            if (categoria == null)
            {
                return BadRequest("La categoría especificada no existe o está inactiva.");
            }

            producto.Nombre = productoDTO.Nombre;
            producto.Codigo = productoDTO.Codigo;
            producto.Descripcion = productoDTO.Descripcion;
            producto.PrecioVenta = productoDTO.PrecioVenta;
            producto.CategoriaId = productoDTO.CategoriaId;
            producto.Estado = productoDTO.Estado;

            _context.Entry(producto).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var producto = await _context.Productos.FindAsync(id);

            if (producto == null)
            {
                return NotFound();
            }

            producto.Estado = !producto.Estado;

            _context.Entry(producto).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
