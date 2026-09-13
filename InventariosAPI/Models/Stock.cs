namespace InventariosAPI.Models
{
    public class Stock
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public int SucursalId { get; set; }
        public int Cantidad { get; set; }
    }
}
