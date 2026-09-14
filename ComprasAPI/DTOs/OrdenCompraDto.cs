namespace ComprasAPI.DTOs
{
    public class OrdenCompraDto
    {
        public int Id { get; set; }
        public int ProveedorId { get; set; }
        public int SucursalId { get; set; }
        public DateTime Fecha { get; set; }
        public bool Estado { get; set; }
        public string RecepcionEstado { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public List<DetalleOrdenDto>? Detalles { get; set; }
    }
}
