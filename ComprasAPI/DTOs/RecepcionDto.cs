namespace ComprasAPI.DTOs
{
    public class RecepcionDto
    {
        public int Id { get; set; }
        public int OrdenCompraId { get; set; }
        public int SucursalId { get; set; }
        public DateTime Fecha { get; set; }
        public string Observaciones { get; set; } = string.Empty;
        public List<DetalleRecepcionDto>? Detalles { get; set; }
    }
}
