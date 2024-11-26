using API_TFG.Models.Enum;

namespace API_TFG.Models.Domain
{
    public class AuditLog
    {
        // Primary Key
        public int AuditLogID { get; set; }

        public Guid UserID { get; set; }

        public string UserName { get; set; } = null!;

        public Guid FileID { get; set; }

        public string FileName { get; set; } = null!;

        public string FilePath { get; set; } = null!;

        public long FileSize { get; set; }

        public ActionType Action { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
