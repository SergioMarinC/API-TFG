using API_TFG.Models.Domain;

namespace API_TFG.Repositories
{
    public interface IAuditLog
    {
        Task CreateAuditLogAsync(AuditLog auditLog);
    }
}
