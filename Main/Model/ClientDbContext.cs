using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Main.Model
{
    /// <summary>
    /// Representación del estado de un cliente en una tabla.
    /// </summary>
    [PrimaryKey(nameof(Id))]
    [Index(nameof(Cliente), IsUnique = true)]
    public class Client
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(Order = 1)]
        public int Id { get; set; }

        public string Cliente { get; set; } = String.Empty;
        public Status Estado { get; set; }
        public DateTime UltimaConexion { get; set; }
    }

    /// <summary>
    /// Instancia de conexión con una base de datos para la tabla de <see cref="Client"/>
    /// </summary>
    public class ClientDbContext() : DbContext()
    {
        public DbSet<Client> EstadoCliente { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Client>().ToTable(b => b.IsMemoryOptimized());
            modelBuilder.HasDefaultSchema("service");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=rafatest;Database=VisualTernera;Trusted_Connection=true;Encrypt=True;TrustServerCertificate=True");
        }
    }
}
