using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Database.Model
{
    [PrimaryKey(nameof(Id))]
    public class Regla
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(Order = 1)]
        public int Id { get; set; }

        public string Nombre { get; set; } = string.Empty;
        public string Etiqueta { get; set; } = string.Empty;
    }
}
