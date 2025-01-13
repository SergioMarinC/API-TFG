using API_TFG.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace API_TFG.Models.DTO
{
    public class ShareFileDto
    {
        [Required]
        public Guid FileID { get; set; }
        [Required]
        public Guid UserID { get; set; }
        [Required]
        public PermissionType PermissionType { get; set; }
    }
}
