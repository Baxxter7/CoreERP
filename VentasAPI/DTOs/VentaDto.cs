namespace VentasAPI.DTOs
{
    public class VentaDto
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public int SucursalId { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }
        public List<DetalleVentaDto>? Detalles { get; set; }
    }
}
