namespace ComprasAPI.Models
{
    public class DetalleRecepcion
    {
        public int Id { get; set; }
        public int RecepcionId { get; set; }
        public int ProductoId { get; set; }
        public int CantidadRecibida { get; set; }
    }
}
