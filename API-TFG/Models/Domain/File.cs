using System.ComponentModel.DataAnnotations;

namespace API_TFG.Models.Domain
{
    public class File
    {
        [Key]
        public Guid FileID { get; set; }

        [Required]
        [MaxLength(255)]
        public required string FileName { get; set; }

        [Required]
        public required string FilePath { get; set; }

        public long FileSize { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public bool IsDeleted { get; set; } = false;

        //Relacion con User

        public required User Owner { get; set; }

        //Propiedades de navegación
        public ICollection<UserFile> SharedWithUsers { get; set; } = new List<UserFile>();
    }
}
