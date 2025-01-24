using API_TFG.Data;
using API_TFG.Models.Domain;
using API_TFG.Models.DTO;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using File = API_TFG.Models.Domain.File;

namespace API_TFG.Repositories.FileRepositories
{
    public class FileResolver : IValueResolver<ShareFileDto, UserFile, File>
    {
        private readonly AppDbContext _appDbContext;
        public FileResolver(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public File Resolve(ShareFileDto source, UserFile destination, File destMember, ResolutionContext context)
        {
            var file = _appDbContext.Files.Include(f => f.Owner).FirstOrDefault(f => f.FileID == source.FileID);

            if (file == null)
            {
                throw new ArgumentException($"El archivo con ID {source.FileID} no existe.");
            }

            return file;
        }
    }
}
