using System.ComponentModel.DataAnnotations;

namespace API_TFG.Models.Domain
{
    public class File
    {
        [Key]
        public Guid FileID { get; set; }

        [Required]
        [MaxLength(255)]
        public string FileName { get; set; } = null!;

        [Required]
        public string FilePath { get; set; } = null!;

        public long FileSize { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public bool IsDeleted { get; set; } = false;

        //Foreign Key
        [Required]
        public Guid OwnerID { get; set; }

        public User Owner { get; set; } = null!;

        //Propiedades de navegación
        public ICollection<UserFile> SharedWithUsers { get; set; } = new List<UserFile>();
    }
}
