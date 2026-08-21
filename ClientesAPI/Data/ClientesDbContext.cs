using ClientesAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ClientesAPI.Data;
public class ClientesDbContext:DbContext
{
    public ClientesDbContext(DbContextOptions<ClientesDbContext> options) : base(options) { }

    public DbSet<Cliente> Clientes => Set<Cliente>();
}
