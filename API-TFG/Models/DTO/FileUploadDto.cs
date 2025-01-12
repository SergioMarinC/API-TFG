using System.ComponentModel.DataAnnotations;

namespace API_TFG.Models.DTO
{
    public class FileUploadDto
    {
        [Required]
        public required IFormFile UploadedFile { get; set; }

        public string? FolderPath { get; set; }

        [Required]
        public required Guid OwnerID { get; set; }
    }
}
