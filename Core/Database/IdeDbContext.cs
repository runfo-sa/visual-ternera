using Core.Database.Model;
using Microsoft.EntityFrameworkCore;

namespace Core.Database
{
    public class IdeDbContext(string sqlConnection) : DbContext()
    {
        public DbSet<Regla> Reglas { get; set; }
        public DbSet<ReglaAtributo> ReglasAtributos { get; set; }
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
