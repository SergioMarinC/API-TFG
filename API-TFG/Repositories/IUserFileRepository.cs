﻿using API_TFG.Models.Domain;
using API_TFG.Models.Enum;

namespace API_TFG.Repositories
{
    public interface IUserFileRepository
    {
        Task<UserFile> CreateAsync(UserFile userFile);
        Task<List<UserFile>> GetFilesSharedWithUserAsync(Guid userId);
        Task<List<UserFile>> GetUserWithAccesToFileAsync(Guid fileId);
        Task<UserFile?> UpdatePermissionsAsync(int userFileId, PermissionType permissionType);
        Task<UserFile?> RemoveUserAccessAsync(int  userFileId);
        Task<UserFile?> GetUserFileAccessAsync(Guid userId, Guid fileId);
    }
}
