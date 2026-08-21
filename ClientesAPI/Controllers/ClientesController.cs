using ClientesAPI.Data;
using ClientesAPI.DTOs;
using ClientesAPI.Models;
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
        public async Task<ActionResult<IEnumerable<ClienteDto>>> GetAll()
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
        public async Task<ActionResult<ClienteDto>> GetById(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);

            if (cliente is null)
                return NotFound();

            var clienteDto = new ClienteDto
            {
                Id = id,
                Nombre = cliente.Nombre,
                Telefono = cliente.Telefono,
                Direccion = cliente.Direccion
            };

            return Ok(clienteDto);
        }

        [HttpPost]
        public async Task<ActionResult<ClienteDto>> AddClient([FromBody]ClienteDto clienteDto)
        {
            if (string.IsNullOrWhiteSpace(clienteDto.Nombre))
                return BadRequest("La propiedad nombre es obligatoria.");

            Cliente cliente = new Cliente
            {
                Nombre = clienteDto.Nombre,
                Telefono = clienteDto.Telefono,
                Direccion = clienteDto.Direccion
            };

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            clienteDto.Id = cliente.Id;
            return CreatedAtAction(nameof(GetById), new { Id = clienteDto.Id}, clienteDto);
        }
    }
}
