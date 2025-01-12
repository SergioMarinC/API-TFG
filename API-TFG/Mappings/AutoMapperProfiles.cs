using API_TFG.Models.Domain;
using API_TFG.Models.DTO;
using AutoMapper;
using File = API_TFG.Models.Domain.File;

namespace API_TFG.Mappings
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            // Mapeo de usuarios
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<AddUserRequestDto, User>().ReverseMap();
            CreateMap<UpdateUserRequestDto, User>().ReverseMap();

            // Mapeo de archivos
            CreateMap<File, FileDto>().ReverseMap();
            CreateMap<FileUploadDto, File>()
                .ForMember(dest => dest.FileID, opt => opt.MapFrom(src => Guid.NewGuid())) // Generar ID único
                .ForMember(dest => dest.FileName, opt => opt.MapFrom(src => src.UploadedFile.FileName)) // Nombre del archivo
                .ForMember(dest => dest.FilePath, opt => opt.MapFrom(src => src.FolderPath ?? string.Empty)) // Se asignará manualmente
                .ForMember(dest => dest.FileSize, opt => opt.MapFrom(src => src.UploadedFile.Length)) // Tamaño del archivo
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.UtcNow)) // Fecha actual
                .ForMember(dest => dest.OwnerID, opt => opt.MapFrom(src => src.OwnerID)) // Propietario
                .ForMember(dest => dest.SharedWithUsers, opt => opt.Ignore()) // Propiedad de navegación
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => false)) // Por defecto, no eliminado
                .ReverseMap();
            CreateMap<UpdateFileRequestDto, File>()
                .ForMember(dest => dest.FilePath, opt => opt.MapFrom(src => src.FolderPath ?? string.Empty))
                .ReverseMap();
        }
    }
}
