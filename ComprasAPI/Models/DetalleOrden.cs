namespace ComprasAPI.Models
{
    public class DetalleOrden
    {
        public int Id { get; set; }
        public int OrdenCompraId { get; set; }
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
    }
}
