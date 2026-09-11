namespace ProductosAPI.DTO_s
{
    public class ProductoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Codigo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public decimal PrecioVenta { get; set; }
        public int CategoriaId { get; set; }
        public bool Estado { get; set; } = true;
    }
}
