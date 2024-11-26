using API_TFG.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace API_TFG.Models.Domain
{
    public class UserFile
    {
        //Primary Key
        public int UserFileID { get; set; }

        // Foreign Keys
        [Required]
        public Guid FileID { get; set; }

        public File File { get; set; } = null!;

        public Guid? UserID { get; set; }

        public User? User { get; set; }

        [Required]
        [MaxLength(10)]
        public PermissionType PermissionType { get; set; } = PermissionType.Read;

        public DateTime SharedDate { get; set; } = DateTime.UtcNow;
    }
}
