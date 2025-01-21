using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace API_TFG.Models.DTO
{
    public class AddUserRequestDto
    {
        [Required]
        [EmailAddress]
        public required string Username { get; set; }

        [Required]
        [PasswordPropertyText]
        public required string Password { get; set; } 

        public string[] Roles { get; set; }
    }
}
