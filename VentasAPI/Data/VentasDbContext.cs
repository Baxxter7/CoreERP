using Microsoft.EntityFrameworkCore;
using VentasAPI.Models;

namespace VentasAPI.Data
{
    public class VentasDbContext : DbContext
    {
        public VentasDbContext(DbContextOptions<VentasDbContext> options) : base(options) { }

        public DbSet<Venta> Ventas { get; set; } 
        public DbSet<DetalleVenta> DetallesVenta { get; set; } 

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Venta>()
                .HasMany(v => v.Detalles)
                .WithOne()
                .HasForeignKey(v => v.VentaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                .Entity<Venta>()
                .Property(d => d.Total)
                .HasPrecision(18, 2);

            modelBuilder
                .Entity<DetalleVenta>()
                .Property(d => d.PrecioUnitario)
                .HasPrecision(18, 2);
        }


    }


    
}
