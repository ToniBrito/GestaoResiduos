using GestaoResiduos.Models;
using Microsoft.EntityFrameworkCore;

namespace GestaoResiduos.Data.Contexts
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<TipoResiduo> TipoResiduos { get; set; }
        public DbSet<Coleta> Coletas { get; set; }
        public DbSet<Processamento> Processamentos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Ignorar a propriedade Senha do mapeamento de banco caso esteja usando autenticação customizada
            modelBuilder.Entity<Usuario>().Ignore(u => u.Senha);

            // Relacionamento Coleta -> Usuario (N:1)
            modelBuilder.Entity<Coleta>()
                .HasOne(c => c.Usuario)
                .WithMany(u => u.Coletas)
                .HasForeignKey(c => c.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relacionamento Coleta -> TipoResiduo (N:1)
            modelBuilder.Entity<Coleta>()
                .HasOne(c => c.TipoResiduo)
                .WithMany(t => t.Coletas)
                .HasForeignKey(c => c.TipoResiduoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relacionamento Processamento -> Coleta (1:1)
            modelBuilder.Entity<Processamento>()
                .HasOne(p => p.Coleta)
                .WithOne(c => c.Processamento)
                .HasForeignKey<Processamento>(p => p.ColetaId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relacionamento Processamento -> Usuario (N:1)
            modelBuilder.Entity<Processamento>()
                .HasOne(p => p.Usuario)
                .WithMany(u => u.Processamentos)
                .HasForeignKey(p => p.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relacionamento Processamento -> TipoResiduo (N:1)
            modelBuilder.Entity<Processamento>()
                .HasOne(p => p.TipoResiduo)
                .WithMany(t => t.Processamentos)
                .HasForeignKey(p => p.TipoResiduoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
