using ClientesAPI.Data;
using ClientesAPI.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClientesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ClientesController : ControllerBase
    {
        private readonly ClientesDbContext _context;

        public ClientesController(ClientesDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClienteDto>>> GetClientes()
        {
            var clientesDb = await _context.Clientes.ToListAsync();

            var listaClientes = clientesDb.Select(c => new ClienteDto
            {
                Id = c.Id,
                Nombre = c.Nombre,
                Telefono = c.Telefono,
                Direccion = c.Direccion
            }).ToList();


            return Ok(listaClientes);
        }
    }
}
