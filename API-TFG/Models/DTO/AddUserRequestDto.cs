using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace API_TFG.Models.DTO
{
    public class AddUserRequestDto
    {
        [Required]
        [MaxLength(50, ErrorMessage = "Username has to be a maximum of 50 characters")]
        public required string Username { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [PasswordPropertyText]
        public required string Password { get; set; } 
    }
}
