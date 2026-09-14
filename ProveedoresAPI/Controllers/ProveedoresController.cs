using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProveedoresAPI.Data;
using ProveedoresAPI.DTOs;

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


    }
}
