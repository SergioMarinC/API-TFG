using API_TFG.Data;
using API_TFG.Models.Domain;
using API_TFG.Models.Enum;
using Microsoft.EntityFrameworkCore;

namespace API_TFG.Repositories
{
    public class SQLUserFileRepository : IUserFileRepository
    {
        private readonly AppDbContext dbContext;
        public SQLUserFileRepository(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<UserFile> CreateAsync(UserFile userFile)
        {
            await dbContext.UserFiles.AddAsync(userFile);
            await dbContext.SaveChangesAsync();

            return userFile;
        }

        public async Task<List<UserFile>> GetFilesSharedWithUserAsync(Guid userId)
        {
            return await dbContext.UserFiles.Include(uf => uf.File)
                .Where(uf => uf.User.UserID == userId)
                .ToListAsync();
        }

        public async Task<UserFile?> GetUserFileAccessAsync(Guid userId, Guid fileId)
        {
            return await dbContext.UserFiles
                .Include(uf => uf.File)
                .Include(uf => uf.User)
                .FirstOrDefaultAsync(uf => uf.User.UserID == userId && uf.File.FileID == fileId && !uf.File.IsDeleted);
        }

        public async Task<List<UserFile>> GetUserWithAccesToFileAsync(Guid fileId)
        {
            return await dbContext.UserFiles.Include(uf => uf.User)
                .Where(uf => uf.File.FileID == fileId && !uf.File.IsDeleted)
                .ToListAsync();
        }

        public async Task<UserFile?> RemoveUserAccessAsync(int userFileId)
        {
            var userFile = await dbContext.UserFiles.FindAsync(userFileId);

            if (userFile == null)
            {
                return null;
            }

            dbContext.UserFiles.Remove(userFile);
            await dbContext.SaveChangesAsync();

            return userFile;
        }

        public async Task<UserFile?> UpdatePermissionsAsync(int userFileId, PermissionType permissionType)
        {
            var existingUserFile = await dbContext.UserFiles.FindAsync(userFileId);

            if (existingUserFile == null)
            {
                return null;
            }

            existingUserFile.PermissionType = permissionType;
            dbContext.UserFiles.Update(existingUserFile);
            await dbContext.SaveChangesAsync();

            return existingUserFile;

        }
    }
}
