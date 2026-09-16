using ComprasAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ComprasAPI.Data
{
    public class ComprasDbContext : DbContext
    {
        public ComprasDbContext(DbContextOptions<ComprasDbContext> options) : base(options) { }

        public DbSet<OrdenCompra> OrdenCompras { get; set; }
        public DbSet<DetalleOrden> DetalleOrdenes { get; set; }
        public DbSet<Recepcion> Recepciones { get; set; }
        public DbSet<DetalleRecepcion> DetalleRecepciones { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<OrdenCompra>()
                .HasMany(c => c.Detalles)
                .WithOne()
                .HasForeignKey(d => d.OrdenCompraId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                .Entity<Recepcion>()
                .HasMany(r => r.Detalles)
                .WithOne()
                .HasForeignKey(d => d.RecepcionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
