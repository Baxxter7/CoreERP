using Microsoft.EntityFrameworkCore;
using SucursalesAPI.Models;

namespace SucursalesAPI.Data
{
    public class SucursaDbContext : DbContext
    {
        public SucursaDbContext(DbContextOptions<SucursaDbContext> options) : base(options) { }

        public DbSet<Sucursal> Sucursales => Set<Sucursal>();
    }
}
