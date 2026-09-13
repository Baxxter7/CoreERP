using InventariosAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace InventariosAPI.Data
{
    public class InventarioDbContext : DbContext
    {
        public InventarioDbContext(DbContextOptions<InventarioDbContext> options): base(options) { }

        public DbSet<Movimiento> Movimientos => Set<Movimiento>();

        public DbSet<Stock> Stocks => Set<Stock>();
    }
}
