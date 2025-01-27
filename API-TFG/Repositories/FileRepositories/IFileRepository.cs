using API_TFG.Models.Domain;
using API_TFG.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using File = API_TFG.Models.Domain.File;

namespace API_TFG.Repositories.FileRepositories
{
    public interface IFileRepository
    {
        Task<File?> GetByIdAsync(Guid id);
        Task<File> UploadAsync(File file, IFormFile formFile);
        Task<File?> UpdateAsync(Guid id, File file);
        Task<File?> SoftDelete(Guid id);
        Task<File?> HardDelete(Guid id);
        Task<File?> Restore(Guid id);
        Task<List<File>?> GetFilesSharedWithUserAsync(Guid id);
        Task<(File? file, byte[]? fileContent)> DownloadAsync(Guid id);
        Task<List<File>?> GetAllByUserIdAsync(Guid id, string? filterOn = null, string? filterQuery = null, string? sortBy = null, bool isAscending = true, int pageNumber = 1, int pageSize = 1000);
        string GetContentType(string filePath);
    }
}
