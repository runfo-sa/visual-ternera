using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Editor.Model
{
    /// <summary>
    /// Representación del estado de un cliente en una tabla.
    /// </summary>
    [PrimaryKey(nameof(Id))]
    [Index(nameof(Key), IsUnique = true)]
    public class TestVar
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(Order = 1)]
        public int Id { get; set; }

        public string Key { get; set; } = String.Empty;
        public string Value { get; set; } = String.Empty;
    }

    /// <summary>
    /// Instancia de conexión con una base de datos para la tabla de <see cref="TestVar"/>
    /// </summary>
    public class TestVarsDbContext(string sqlConnection) : DbContext()
    {
        public DbSet<TestVar> EtiquetasDatosPrueba { get; set; }

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
