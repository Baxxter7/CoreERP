namespace SucursalesAPI.Models
{
    public class Sucursal
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Ubicacion { get; set; } = string.Empty;
        public bool Estado { get; set; }
    }
}
