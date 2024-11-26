using System.ComponentModel.DataAnnotations;

namespace API_TFG.Models.Domain
{
    public class User
    {
        [Key]
        public Guid UserID { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = null!;

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } = null!;

        [Required]
        public string PasswordHash { get; set; } = null!;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? LastLogin { get; set;}

        //Propiedades de navegación
        public ICollection<File> Files { get; set; } = new List<File>();

    }
}
