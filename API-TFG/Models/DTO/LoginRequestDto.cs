using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace API_TFG.Models.DTO
{
    public class LoginRequestDto
    {
        [Required]
        [EmailAddress]
        public string Username { get; set; }

        [Required]
        [PasswordPropertyText]
        public string Password { get; set; }
    }
}
