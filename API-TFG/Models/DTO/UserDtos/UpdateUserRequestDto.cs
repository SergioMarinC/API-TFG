using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace API_TFG.Models.DTO.UserDtos
{
    public class UpdateUserRequestDto
    {
        [MaxLength(50, ErrorMessage = "Username has to be a maximum of 50 characters")]
        public string? Username { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        [PasswordPropertyText]
        public string? Password { get; set; }
    }
}
