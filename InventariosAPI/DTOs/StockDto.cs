namespace InventariosAPI.DTOs
{
    public class StockDto
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public int SucursalId { get; set; }
        public int Cantidad { get; set; }
    }
}
