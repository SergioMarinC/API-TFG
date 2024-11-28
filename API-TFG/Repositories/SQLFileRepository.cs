
using API_TFG.Data;
using Microsoft.EntityFrameworkCore;

namespace API_TFG.Repositories
{
    public class SQLFileRepository : IFileRepository
    {
        private readonly AppDbContext dbContext;
        public SQLFileRepository(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<List<Models.Domain.File>> GetAllAsync()
        {
            return await dbContext.Files.ToListAsync();
        }

        public async Task<Models.Domain.File?> GetByIdAsync(Guid id)
        {
            return await dbContext.Files.FindAsync(id);
        }

        public async Task<List<Models.Domain.File>?> GetAllByUserIdAsync(Guid id)
        {
            if (!await dbContext.Files.AnyAsync(f => f.OwnerID == id))
            {
                return null;
            }

            return await dbContext.Files.Where(f => f.OwnerID == id && !f.IsDeleted).ToListAsync();
        }

        public Task<Models.Domain.File> ShareAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<Models.Domain.File?> UpdateAsync(Guid id, Models.Domain.File file)
        {
            var existingFile = await dbContext.Files.FindAsync(id);

            if (existingFile == null)
            {
                return null;
            }
            
            existingFile.FileName = file.FileName;
            existingFile.FilePath = file.FilePath;
            existingFile.FileSize = file.FileSize;

            await dbContext.SaveChangesAsync();
            return existingFile;
        }

        public Task<Models.Domain.File> UploadAsync(Models.Domain.File file)
        {
            throw new NotImplementedException();
        }

        public async Task<Models.Domain.File?> RemoveAsync(Guid id)
        {
            var existingFile = await dbContext.Files.FindAsync(id);

            if (existingFile == null)
            {
                return null;
            }

            existingFile.IsDeleted = true;

            await dbContext.SaveChangesAsync();
            return existingFile;
        }

        public async Task<Models.Domain.File?> DeleteAsync(Guid id)
        {
            var existingFile = await dbContext.Files.FindAsync(id);

            if(existingFile == null)
            {
                return null;
            }

            dbContext.Files.Remove(existingFile);
            await dbContext.SaveChangesAsync();
            return existingFile;
        }
    }
}
