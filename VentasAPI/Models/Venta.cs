namespace VentasAPI.Models
{
    public class Venta
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public int SucursalId { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }
        public List<DetalleVenta> Detalles { get; set; } = new();
    }
}
