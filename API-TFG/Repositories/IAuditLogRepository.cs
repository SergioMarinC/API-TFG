using API_TFG.Models.Domain;

namespace API_TFG.Repositories
{
    public interface IAuditLogRepository
    {
        Task CreateAuditLogAsync(AuditLog auditLog);
    }
}
