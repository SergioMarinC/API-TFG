using API_TFG.Models.Domain;
using File = API_TFG.Models.Domain.File;

namespace API_TFG.Repositories
{
    public interface IFileRepository
    {
        Task<List<File>> GetAllAsync();
        Task<File?> GetByIdAsync(Guid id);
        Task<File> UploadAsync(File file);
        Task<File?> UpdateAsync(Guid id, File file);
        Task<File?> RemoveAsync(Guid id);
        Task<File?> DeleteAsync(Guid id);
        Task<File> ShareAsync(Guid id);
        Task<List<File>?> GetAllByUserIdAsync(Guid id);
    }
}
