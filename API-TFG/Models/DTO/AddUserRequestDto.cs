using System.ComponentModel.DataAnnotations;

namespace API_TFG.Models.DTO
{
    public class AddUserRequestDto
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
