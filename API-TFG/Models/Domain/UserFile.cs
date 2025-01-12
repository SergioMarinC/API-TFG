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
        public required File File { get; set; }

        [Required]
        public required User User { get; set; }

        [Required]
        [MaxLength(10)]
        public PermissionType PermissionType { get; set; } = PermissionType.Read;

        public DateTime SharedDate { get; set; } = DateTime.UtcNow;
    }
}
