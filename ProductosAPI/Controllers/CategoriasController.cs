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
    public class CategoriasController : ControllerBase
    {
        private readonly ProductoDbContext _context;

        public CategoriasController(ProductoDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoriaDto>>> GetAll()
        {
            var categorias = await _context.Categorias
                .AsNoTracking()
                .Select(c => new CategoriaDto
                {
                    Id = c.Id,
                    Nombre = c.Nombre,
                    Descripcion = c.Descripcion,
                    Estado = c.Estado,
                })
                .ToListAsync();

            return Ok(categorias);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoriaDto>> Get(int id)
        {
            var categoria = await _context.Categorias
                .Select(c => new CategoriaDto
                {
                    Id = c.Id,
                    Nombre = c.Nombre,
                    Descripcion = c.Descripcion,
                    Estado = c.Estado,
                })
                .Where(c => c.Id == id)
                .FirstOrDefaultAsync();

            if (categoria == null)           
                return NotFound();

           return Ok(categoria);
        }

        [HttpPost]
        public async Task<ActionResult<CategoriaDto>> Post([FromBody] CategoriaDto categoriaDTO)
        {
            if (string.IsNullOrWhiteSpace(categoriaDTO.Nombre))
            {
                return BadRequest("El nombre de la categoría es obligatorio.");
            }

            var categoria = new Categoria
            {
                Nombre = categoriaDTO.Nombre,
                Descripcion = categoriaDTO.Descripcion,
                Estado = categoriaDTO.Estado,
            };

            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();

            categoriaDTO.Id = categoria.Id;

            return CreatedAtAction(
                nameof(Get),
                new { id = categoria.Id },
                categoriaDTO
            );
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] CategoriaDto categoriaDTO)
        {
            if (id != categoriaDTO.Id)
            {
                return BadRequest("El ID de la categoría no coincide.");
            }

            var categoria = await _context.Categorias.FindAsync(id);

            if (categoria == null)
            {
                return NotFound();
            }

            categoria.Nombre = categoriaDTO.Nombre;
            categoria.Descripcion = categoriaDTO.Descripcion;
            categoria.Estado = categoriaDTO.Estado;

            _context.Entry(categoria).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var categoria = await _context.Categorias.FindAsync(id);

            if (categoria == null)
            {
                return NotFound();
            }

            categoria.Estado = !categoria.Estado;

            _context.Entry(categoria).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
