namespace ComprasAPI.Models
{
    public class OrdenCompra
    {
        public int Id { get; set; }

        public int ProveedorId { get; set; }

        public int SucursalId { get; set; }

        public DateTime Fecha { get; set; }

        public bool Estado { get; set; }

        public decimal Total { get; set; }

        public List<DetalleOrden>? Detalles { get; set; }
    }
}
