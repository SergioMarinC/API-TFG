using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace API_TFG.Models.DTO
{
    public class UserDto
    {
        public Guid UserID { get; set; }
        public required string Username { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastLogin { get; set; }
    }
}
