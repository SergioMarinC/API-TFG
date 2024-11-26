using API_TFG.Models.Domain;
using Microsoft.EntityFrameworkCore;
using File = API_TFG.Models.Domain.File;

namespace API_TFG.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions) {}

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<File> Files { get; set; } = null!;
        public DbSet<UserFile> UserFiles { get; set; } = null!;
        public DbSet<AuditLog> AuditLogs { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuración de File
            modelBuilder.Entity<File>()
                .HasOne(f => f.Owner)
                .WithMany(u => u.Files)
                .HasForeignKey(f => f.OwnerID)
                .OnDelete(DeleteBehavior.Cascade); // Archivos eliminados al eliminar usuario

            // Configuración de UserFile
            modelBuilder.Entity<UserFile>()
                .HasOne(uf => uf.File)
                .WithMany(f => f.SharedWithUsers)
                .HasForeignKey(uf => uf.FileID);

            modelBuilder.Entity<UserFile>()
                .HasOne(uf => uf.User)
                .WithMany()
                .HasForeignKey(uf => uf.UserID);

            // Configuración de AuditLog
            modelBuilder.Entity<AuditLog>()
                .Property(al => al.Action)
                .HasConversion<string>(); // Almacena ActionType como texto legible

            // Índices únicos en User
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            base.OnModelCreating(modelBuilder);
        }
    }
}
