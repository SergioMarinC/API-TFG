using System.ComponentModel.DataAnnotations;

namespace API_TFG.Models.DTO
{
    public class FileDto
    {
        public Guid FileID { get; set; }

        public string FileName { get; set; } = null!;

        public string FilePath { get; set; } = null!;

        public long FileSize { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool IsDeleted { get; set; }

        public Guid OwnerID { get; set; }
    }
}
