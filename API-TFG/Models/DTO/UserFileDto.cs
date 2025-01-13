using API_TFG.Models.Domain;
using API_TFG.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace API_TFG.Models.DTO
{
    public class UserFileDto
    {
        public int UserFileID { get; set; }
        public Guid FileID { get; set; }
        public required string FileName { get; set; }
        public required string OwnerName { get; set; }
        public PermissionType PermissionType { get; set; }
        public DateTime SharedDate { get; set; }
    }
}
