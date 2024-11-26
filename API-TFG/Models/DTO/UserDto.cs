using System.ComponentModel.DataAnnotations;

namespace API_TFG.Models.DTO
{
    public class UserDto
    {
        public Guid UserID { get; set; }

        public string Username { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        public DateTime? LastLogin { get; set; }
    }
}
