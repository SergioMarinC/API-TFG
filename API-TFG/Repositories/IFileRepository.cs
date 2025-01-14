using API_TFG.Models.Domain;
using API_TFG.Models.DTO;
using File = API_TFG.Models.Domain.File;

namespace API_TFG.Repositories
{
    public interface IFileRepository
    {
        Task<List<File>> GetAllAsync();
        Task<File?> GetByIdAsync(Guid id);
        Task<File> UploadAsync(File file, IFormFile formFile);
        Task<File?> UpdateAsync(Guid id, File file);
        Task<File?> SoftDelete(Guid id);
        Task<File?> HardDelete(Guid id);
        Task<File?> Restore(Guid id);
        Task<File> ShareAsync(Guid id);
        Task<(File? file, byte[]? fileContent)> DownloadAsync(Guid id);
        Task<List<File>?> GetAllByUserIdAsync(Guid id);
        String GetContentType(String filePath);
    }
}
