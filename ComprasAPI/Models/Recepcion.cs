using ComprasAPI.DTOs;

namespace ComprasAPI.Models
{
    public class Recepcion
    {
        public int Id { get; set; }
        public int OrdenCompraId { get; set; }
        public int SucursalId { get; set; }
        public DateTime Fecha { get; set; }
        public string Observaciones { get; set; } = string.Empty;
        public List<DetalleRecepcion> Detalles { get; set; } = new();
    }
}
