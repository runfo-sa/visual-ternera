using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core
{
    public enum ProductoError
    {
        Incompleto,
        Incoherente,
        Ninguno
    }

    public class ListarProductos
    {
        public string Codigo { get; set; } = String.Empty;
        public string Nombre { get; set; } = String.Empty;
        public string Senasa { get; set; } = String.Empty;

        [NotMapped]
        public ProductoError Error { get; set; } = ProductoError.Ninguno;

        [NotMapped]
        public List<Valor> Valores { get; set; } = [];
    }

    public record struct Valor(string Atributo, string? Valores, ProductoError Error);

    [PrimaryKey(nameof(Id))]
    public class Regla
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(Order = 1)]
        public int Id { get; set; }

        public string Nombre { get; set; } = String.Empty;
        public string Etiqueta { get; set; } = String.Empty;
    }

    [PrimaryKey(nameof(Id))]
    public class ReglaAtributo
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(Order = 1)]
        public int Id { get; set; }

        [ForeignKey(nameof(Reglas_Id))]
        public int Reglas_Id { get; set; }

        public string AtributoNombre { get; set; } = String.Empty;
        public bool EsAtributoEstatico { get; set; } = false;
        public string? ValorEstatico { get; set; } = String.Empty;
    }

    public class KeyValue
    {
        public string Codigo { get; set; } = String.Empty;
        public string? Valor { get; set; } = String.Empty;
    }

    public class IdeDbContext(string sqlConnection) : DbContext()
    {
        public DbSet<Regla> Reglas { get; set; }
        public DbSet<ReglaAtributo> ReglasAtributos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TestVar>().ToTable(b => b.IsMemoryOptimized());
            modelBuilder.HasDefaultSchema("ide");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(sqlConnection);
        }
    }
}
