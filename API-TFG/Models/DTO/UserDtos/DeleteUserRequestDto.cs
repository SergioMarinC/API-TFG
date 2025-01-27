using System.ComponentModel.DataAnnotations;

namespace API_TFG.Models.DTO.UserDtos
{
    public class DeleteUserRequestDto
    {
        [Required]
        public required string Password { get; set; }
    }
}
