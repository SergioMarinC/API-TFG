using API_TFG.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace API_TFG.Models.DTO.UserFileDtos
{
    public class UpdateUserFileRequestDto
    {
        [Required]
        [MaxLength(10)]
        public PermissionType PermissionType { get; set; }
    }
}
