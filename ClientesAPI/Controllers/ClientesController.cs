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

        [HttpGet("{id}")]
        public async Task<ActionResult<ClienteDto>> GetCliente(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);

            if(cliente is null)
                return NotFound();

            var clienteDto = new ClienteDto { 
                Id = id,
                Nombre = cliente.Nombre,
                Telefono = cliente.Telefono,
                Direccion= cliente.Direccion
            };

            return Ok(clienteDto);
        }
    }
}
