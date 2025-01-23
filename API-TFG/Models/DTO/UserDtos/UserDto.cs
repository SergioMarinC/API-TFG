using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace API_TFG.Models.DTO.UserDtos
{
    public class UserDto
    {
        public Guid UserID { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
    }
}
