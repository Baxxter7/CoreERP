using Microsoft.EntityFrameworkCore;
using ProveedoresAPI.Models;

namespace ProveedoresAPI.Data
{
    public class ProveedoresDbContext : DbContext
    {
        public ProveedoresDbContext(DbContextOptions<ProveedoresDbContext> options) : base(options) { }

        public DbSet<Proveedor> Proveedores => Set<Proveedor>();
    }
}
