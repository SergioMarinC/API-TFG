
using API_TFG.Data;
using API_TFG.Models.Domain;
using API_TFG.Models.DTO;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace API_TFG.Repositories
{
    public class SQLFileRepository : IFileRepository
    {
        private readonly AppDbContext dbContext;
        public SQLFileRepository(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<List<Models.Domain.File>> GetAllAsync([FromQuery] string? filterOn = null, string? filterQuery = null, string? sortBy = null, bool isAscending = true, int pageNumber = 1, int pageSize = 1000)
        {
            var files = dbContext.Files.AsQueryable();

            //Filtering
            if (string.IsNullOrWhiteSpace(filterOn) == false && string.IsNullOrWhiteSpace(filterQuery) == false)
            {
                if (filterOn.Equals("filename", StringComparison.OrdinalIgnoreCase))
                {
                    files = files.Where(x => x.FileName.Contains(filterQuery));
                }
            }

            //Sorting
            if (string.IsNullOrWhiteSpace(sortBy) == false)
            {
                if (sortBy.Equals("filename", StringComparison.OrdinalIgnoreCase))
                {
                    files = isAscending ? files.OrderBy(x  => x.FileName): files.OrderByDescending(x => x.FileName);
                }
                else if (sortBy.Equals("FileSize", StringComparison.OrdinalIgnoreCase))
                {
                    files = isAscending ? files.OrderBy(x => x.FileSize) : files.OrderByDescending(x => x.FileSize);
                }
                else if (sortBy.Equals("CreatedDate", StringComparison.OrdinalIgnoreCase))
                {
                    files = isAscending ? files.OrderBy(x => x.CreatedDate) : files.OrderByDescending(x => x.CreatedDate);
                }
            }

            //Pagination
            var skipResults = (pageNumber - 1) * pageSize;

            return await files.Skip(skipResults).Take(pageSize).Include(f => f.Owner).ToListAsync();
        }

        public async Task<Models.Domain.File?> GetByIdAsync(Guid id)
        {
            return await dbContext.Files.Include(f => f.Owner).FirstOrDefaultAsync(f => f.FileID == id);
        }

        public async Task<List<Models.Domain.File>?> GetAllByUserIdAsync(Guid id, string? filterOn = null, string? filterQuery = null, string? sortBy = null, bool isAscending = true, int pageNumber = 1, int pageSize = 1000)
        {
            if(!await dbContext.Files.AnyAsync(x => x.Owner.UserID == id))
            {
                return null;
            }

            var files = dbContext.Files.AsQueryable();

            //Filtering
            if (string.IsNullOrWhiteSpace(filterOn) == false && string.IsNullOrWhiteSpace(filterQuery) == false)
            {
                if (filterOn.Equals("filename", StringComparison.OrdinalIgnoreCase))
                {
                    files = files.Where(x => x.FileName.Contains(filterQuery));
                }
            }

            //Sorting
            if (string.IsNullOrWhiteSpace(sortBy) == false)
            {
                if (sortBy.Equals("filename", StringComparison.OrdinalIgnoreCase))
                {
                    files = isAscending ? files.OrderBy(x => x.FileName) : files.OrderByDescending(x => x.FileName);
                }
                else if (sortBy.Equals("FileSize", StringComparison.OrdinalIgnoreCase))
                {
                    files = isAscending ? files.OrderBy(x => x.FileSize) : files.OrderByDescending(x => x.FileSize);
                }
                else if (sortBy.Equals("CreatedDate", StringComparison.OrdinalIgnoreCase))
                {
                    files = isAscending ? files.OrderBy(x => x.CreatedDate) : files.OrderByDescending(x => x.CreatedDate);
                }
            }

            //Pagination
            var skipResults = (pageNumber - 1) * pageSize;

            return await files.Where(f => f.Owner.UserID == id && !f.IsDeleted).Skip(skipResults).Take(pageSize).Include(f => f.Owner).ToListAsync();
        }

        public Task<Models.Domain.File> ShareAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<Models.Domain.File?> UpdateAsync(Guid id, Models.Domain.File file)
        {
            // Buscar el archivo existente en la base de datos
            var existingFile = await dbContext.Files.Include(f => f.Owner).FirstOrDefaultAsync(f => f.FileID == id);
            if (existingFile == null)
            {
                return null; // Archivo no encontrado
            }

            // Mantener la extensión original del archivo
            var originalExtension = Path.GetExtension(existingFile.FileName);
            var newFileNameWithoutExtension = Path.GetFileNameWithoutExtension(file.FileName);
            var newFileName = $"{newFileNameWithoutExtension}{originalExtension}";

            // Obtener la ruta base para "uploads" (dentro del directorio del proyecto)
            var baseUploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");

            // Ruta antigua: dentro de la carpeta del usuario
            var userRootFolder = Path.Combine(baseUploadsPath, existingFile.Owner.UserID.ToString());
            var oldFilePath = Path.Combine(userRootFolder, existingFile.FilePath ?? string.Empty, existingFile.FileName);

            // Nueva ruta: carpeta del usuario + nueva subcarpeta (si se proporciona) + nuevo nombre de archivo
            var newFilePath = Path.Combine(userRootFolder, file.FilePath ?? string.Empty, newFileName);

            // Si la ruta ha cambiado, mover el archivo
            if (!string.Equals(oldFilePath, newFilePath, StringComparison.OrdinalIgnoreCase))
            {
                // Asegurar que la nueva carpeta existe
                var newFolderPath = Path.GetDirectoryName(newFilePath);
                if (!Directory.Exists(newFolderPath))
                {
                    Directory.CreateDirectory(newFolderPath);
                }

                // Mover el archivo si existe en la ubicación anterior
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Move(oldFilePath, newFilePath);
                }
                else
                {
                    throw new FileNotFoundException($"El archivo original no se encontró en la ruta: {oldFilePath}");
                }
            }

            // Actualizar los datos del archivo en la base de datos
            existingFile.FileName = newFileName;
            existingFile.FilePath = file.FilePath ?? string.Empty;

            // Guardar cambios
            dbContext.Files.Update(existingFile);
            await dbContext.SaveChangesAsync();

            return existingFile;
        }



        public async Task<Models.Domain.File?> SoftDelete(Guid id)
        {
            var existingFile = await dbContext.Files.Include(f => f.Owner).FirstOrDefaultAsync(f => f.FileID == id);

            if (existingFile == null)
            {
                return null;
            }

            existingFile.IsDeleted = true;

            await dbContext.SaveChangesAsync();
            return existingFile;
        }

        public async Task<Models.Domain.File?> HardDelete(Guid id)
        {
            var existingFile = await dbContext.Files.Include(f => f.Owner).FirstOrDefaultAsync(f => f.FileID == id);

            if(existingFile == null)
            {
                return null;
            }
            
            var baseUploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
            var userRootFolder = Path.Combine(baseUploadsPath, existingFile.Owner.UserID.ToString());
            var filePath = Path.Combine(userRootFolder, existingFile.FilePath ?? string.Empty, existingFile.FileName);

            // Verificar si el archivo existe físicamente
            if (System.IO.File.Exists(filePath))
            {
                try
                {
                    // Eliminar el archivo físico
                    System.IO.File.Delete(filePath);
                }
                catch (Exception ex)
                {
                    throw new IOException($"Error al eliminar el archivo físico: {filePath}", ex);
                }
            }

            // Eliminar el registro de la base de datos
            dbContext.Files.Remove(existingFile);
            await dbContext.SaveChangesAsync();

            return existingFile;
        }

        public async Task<Models.Domain.File> UploadAsync(Models.Domain.File file, IFormFile formFile)
        {
            var userRootFolder = Path.Combine("uploads", file.Owner.UserID.ToString());

            var targetFolder = string.IsNullOrWhiteSpace(file.FilePath) ? userRootFolder : Path.Combine(userRootFolder, file.FilePath);

            if (!Directory.Exists(targetFolder))
            {
                Directory.CreateDirectory(targetFolder);
            }

            var fullPath = Path.Combine(targetFolder, file.FileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await formFile.CopyToAsync(stream);
            }

            await dbContext.Files.AddAsync(file);
            await dbContext.SaveChangesAsync();

            return file;
        }

        public async Task<(Models.Domain.File? file, byte[]? fileContent)> DownloadAsync(Guid id)
        {
            // Buscar el archivo en la base de datos
            var file = await dbContext.Files.Include(f => f.Owner).FirstOrDefaultAsync(f => f.FileID == id);

            if (file == null)
            {
                return (null, null); // Archivo no encontrado
            }

            var userRootFolder = Path.Combine("uploads", file.Owner.UserID.ToString());
            var fullFilePath = Path.Combine(userRootFolder,file.FilePath,file.FileName);

            // Verificar si el archivo existe físicamente
            if (!System.IO.File.Exists(fullFilePath))
            {
                return (file, null); // Archivo no existe físicamente
            }

            // Leer el contenido del archivo
            var fileContent = await System.IO.File.ReadAllBytesAsync(fullFilePath);

            return (file, fileContent);
        }

        public string GetContentType(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLowerInvariant();
            return extension switch
            {
                ".txt" => "text/plain",
                ".pdf" => "application/pdf",
                ".png" => "image/png",
                ".jpg" or ".jpeg" => "image/jpeg",
                _ => "application/octet-stream",
            };
        }

        public async Task<Models.Domain.File?> Restore(Guid id)
        {
            var existingFile = await dbContext.Files.Include(f => f.Owner).FirstOrDefaultAsync(f => f.FileID == id);

            if (existingFile == null)
            {
                return null;
            }

            existingFile.IsDeleted = false;

            await dbContext.SaveChangesAsync();

            return existingFile;
        }
    }
}
