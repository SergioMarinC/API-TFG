using System.ComponentModel.DataAnnotations;

namespace API_TFG.Models.DTO
{
    public class FileDto
    {
        public Guid FileID { get; set; }

        public required string FileName { get; set; }

        public required string FilePath { get; set; }

        public long FileSize { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool IsDeleted { get; set; }

        public Guid OwnerID { get; set; }
    }
}
