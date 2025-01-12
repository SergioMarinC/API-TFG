﻿    using API_TFG.Models.Domain;
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
                .OnDelete(DeleteBehavior.Cascade); // Archivos eliminados al eliminar usuario

            // Configuración de UserFile
            modelBuilder.Entity<UserFile>()
                .HasOne(uf => uf.File) // Relación con File
                .WithMany(f => f.SharedWithUsers) // Un archivo puede ser compartido con muchos usuarios
                .OnDelete(DeleteBehavior.Cascade); // Eliminar registros en cascada

            modelBuilder.Entity<UserFile>()
                .HasOne(uf => uf.User) // Relación con User
                .WithMany() // Un usuario puede compartir muchos archivos
                .OnDelete(DeleteBehavior.Cascade); // Eliminar registros en cascada

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
