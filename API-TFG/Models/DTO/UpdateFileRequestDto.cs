using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace API_TFG.Models.DTO
{
    public class UpdateFileRequestDto
    {
        [Required]
        [MaxLength(255)]
        public required string FileName { get; set; }

        public string? FolderPath { get; set; }
    }
}
