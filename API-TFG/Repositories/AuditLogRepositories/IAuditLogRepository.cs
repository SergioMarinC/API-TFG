using API_TFG.Models.Domain;

namespace API_TFG.Repositories.AuditLogRepositories
{
    public interface IAuditLogRepository
    {
        Task CreateAuditLogAsync(AuditLog auditLog);
    }
}
