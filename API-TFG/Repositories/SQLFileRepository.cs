
using API_TFG.Data;
using API_TFG.Models.Domain;
using API_TFG.Models.DTO;
using AutoMapper;
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
            // Buscar el archivo existente en la base de datos
            var existingFile = await dbContext.Files.FindAsync(id);
            if (existingFile == null)
            {
                return null; // Archivo no encontrado
            }

            // Obtener la ruta base para "uploads" (dentro del directorio del proyecto)
            var baseUploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");

            // Ruta antigua: dentro de la carpeta del usuario
            var userRootFolder = Path.Combine(baseUploadsPath, existingFile.OwnerID.ToString());
            var oldFilePath = Path.Combine(userRootFolder, existingFile.FilePath ?? string.Empty, existingFile.FileName);

            // Nueva ruta: carpeta del usuario + nueva subcarpeta (si se proporciona) + nuevo nombre de archivo
            var newFilePath = Path.Combine(userRootFolder, file.FilePath ?? string.Empty, file.FileName);

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
            existingFile.FileName = file.FileName;
            existingFile.FilePath = file.FilePath ?? string.Empty;

            // Guardar cambios
            dbContext.Files.Update(existingFile);
            await dbContext.SaveChangesAsync();

            return existingFile;
        }


        public async Task<Models.Domain.File?> SoftDelete(Guid id)
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

        public async Task<Models.Domain.File?> HardDelete(Guid id)
        {
            var existingFile = await dbContext.Files.FindAsync(id);

            if(existingFile == null)
            {
                return null;
            }
            
            var baseUploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
            var userRootFolder = Path.Combine(baseUploadsPath, existingFile.OwnerID.ToString());
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
            var userRootFolder = Path.Combine("uploads", file.OwnerID.ToString());

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
            var file = await dbContext.Files.FindAsync(id);

            if (file == null)
            {
                return (null, null); // Archivo no encontrado
            }

            var userRootFolder = Path.Combine("uploads", file.OwnerID.ToString());
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
    }
}
