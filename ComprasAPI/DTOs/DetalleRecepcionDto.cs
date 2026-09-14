namespace ComprasAPI.DTOs
{
    public class DetalleRecepcionDto
    {
        public int Id { get; set; }
        public int RecepcionId { get; set; }
        public int ProductoId { get; set; }
        public int CantidadRecibida { get; set; }
    }
}
