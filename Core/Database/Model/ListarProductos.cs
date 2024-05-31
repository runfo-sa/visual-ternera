using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Database.Model
{
    public class ListarProductos
    {
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Senasa { get; set; } = string.Empty;

        [NotMapped]
        public ProductoError Error { get; set; } = ProductoError.Ninguno;

        [NotMapped]
        public List<Valor> Valores { get; set; } = [];
    }
}
