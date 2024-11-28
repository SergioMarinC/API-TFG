namespace API_TFG.Models.DTO
{
    public class UpdateUserRequestDto
    {
        public required string Username { get; set; } = null!;
        public required string Email { get; set; } = null!;
        public required string Password { get; set; } = null!;
    }
}
