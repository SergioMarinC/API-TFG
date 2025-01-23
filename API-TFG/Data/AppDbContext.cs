using API_TFG.Models.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using File = API_TFG.Models.Domain.File;

namespace API_TFG.Data
{
    public class AppDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> dbContextOptions) : base(dbContextOptions)
        {

        }

        public DbSet<File> Files { get; set; }
        public DbSet<UserFile> UserFiles { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuración de File
            modelBuilder.Entity<File>()
                .HasOne(f => f.Owner)
                .WithMany(u => u.Files)
                .OnDelete(DeleteBehavior.Restrict); // Archivos eliminados al eliminar usuario

            // Configuración de UserFile
            modelBuilder.Entity<UserFile>()
                .HasOne(uf => uf.File) // Relación con File
                .WithMany(f => f.SharedWithUsers) // Un archivo puede ser compartido con muchos usuarios
                .OnDelete(DeleteBehavior.Cascade); // Eliminar registros en cascada

            modelBuilder.Entity<UserFile>()
                .HasOne(uf => uf.User) // Relación con User
                .WithMany() // Un usuario puede compartir muchos archivos
                .OnDelete(DeleteBehavior.Restrict);

            // Configuración de AuditLog
            modelBuilder.Entity<AuditLog>()
                .Property(al => al.Action)
                .HasConversion<string>(); // Almacena ActionType como texto legible

            // Configuración única para Email
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Configuración única para Username
            modelBuilder.Entity<User>()
                .HasIndex(u => u.UserName)
                .IsUnique();

            base.OnModelCreating(modelBuilder);

            var roles = new List<IdentityRole<Guid>>
            {
                new IdentityRole<Guid>
                {
                    Id = Guid.Parse("d579c31f-478f-4027-a5d9-141bba4bf886"),
                    ConcurrencyStamp = "d579c31f-478f-4027-a5d9-141bba4bf886",
                    Name = "User",
                    NormalizedName = "USER"
                },
                new IdentityRole<Guid>
                {
                    Id = Guid.Parse("a382f791-7544-4920-ad30-138446a0816d"),
                    ConcurrencyStamp = "a382f791-7544-4920-ad30-138446a0816d",
                    Name = "Administrator",
                    NormalizedName = "ADMINISTRATOR"
                }
            };

            modelBuilder.Entity<IdentityRole<Guid>>().HasData(roles);
        }
    }
}
