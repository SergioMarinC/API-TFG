using API_TFG.Data;
using API_TFG.Models.Domain;

namespace API_TFG.Repositories.AuditLogRepositories
{
    public class SQLAuditLogRepository : IAuditLogRepository
    {
        private readonly AppDbContext dbContext;

        public SQLAuditLogRepository(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task CreateAuditLogAsync(AuditLog auditLog)
        {
            dbContext.AuditLogs.Add(auditLog);
            await dbContext.SaveChangesAsync();
        }
    }
}
