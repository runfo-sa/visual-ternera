using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Database.Model
{
    [PrimaryKey(nameof(Id))]
    public class ReglaAtributo
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(Order = 1)]
        public int Id { get; set; }

        [ForeignKey(nameof(Reglas_Id))]
        public int Reglas_Id { get; set; }

        public string AtributoNombre { get; set; } = string.Empty;
        public bool EsAtributoEstatico { get; set; } = false;
        public string? ValorEstatico { get; set; } = string.Empty;
    }
}
