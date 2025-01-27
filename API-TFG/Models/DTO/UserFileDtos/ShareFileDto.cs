using API_TFG.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace API_TFG.Models.DTO.UserFileDtos
{
    public class ShareFileDto
    {
        [Required]
        public Guid FileID { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public PermissionType? PermissionType { get; set; }
    }
}
