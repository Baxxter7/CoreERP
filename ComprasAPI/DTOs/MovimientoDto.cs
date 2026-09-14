namespace ComprasAPI.DTOs
{
    public class MovimientoDto
    {
        public int Id { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public int ProductoId { get; set; }
        public int SucursalId { get; set; }
        public int Cantidad { get; set; }
        public DateTime Fecha { get; set; }
        public string Origen { get; set; } = string.Empty;
        public int ReferenciaId { get; set; }
    }
}
