using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Main.Model
{
    /// <summary>
    /// Representación del estado de un cliente del servicio
    /// </summary>
    [PrimaryKey(nameof(Id))]
    [Index(nameof(Cliente), IsUnique = true)]
    public class Client
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(Order = 1)]
        public int Id { get; set; }

        /// <summary>
        /// IPv4 de un Cliente del servicio
        /// </summary>
        public string Cliente { get; set; } = String.Empty;

        /// <summary>
        /// Estado en el que se encuentra el cliente
        /// </summary>
        public ClientStatus Estado { get; set; }

        /// <summary>
        /// Ultima conexión del cliente con el servicio
        /// </summary>
        public DateTime UltimaConexion { get; set; }
    }

    /// <summary>
    /// Instancia de conexión con la base de datos del servicio
    /// </summary>
    public class ServiceDbContext(string sqlConnection) : DbContext()
    {
        /// <summary>
        /// Tabla que almacena el estado de todos los clientes
        /// </summary>
        public DbSet<Client> EstadoCliente { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Client>().ToTable(b => b.IsMemoryOptimized());
            modelBuilder.HasDefaultSchema("service");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(sqlConnection);
        }
    }
}
