namespace ComprasAPI.DTOs
{
    public class DetalleOrdenDto
    {
        public int Id { get; set; }
        public int OrdenCompraId { get; set; }
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
    }
}
