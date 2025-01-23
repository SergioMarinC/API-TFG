using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace API_TFG.Models.DTO.UserDtos
{
    public class LoginRequestDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [PasswordPropertyText]
        public string Password { get; set; }
    }
}
