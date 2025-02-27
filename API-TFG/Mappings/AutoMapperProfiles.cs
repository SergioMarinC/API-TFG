﻿using API_TFG.Models.Domain;
using API_TFG.Models.DTO.FileDtos;
using API_TFG.Models.DTO.UserDtos;
using API_TFG.Models.DTO.UserFileDtos;
using API_TFG.Repositories.FileRepositories;
using AutoMapper;
using File = API_TFG.Models.Domain.File;

namespace API_TFG.Mappings
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            // Map User
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.UserID, opt => opt.MapFrom(src => src.Id))
                .ReverseMap();
            CreateMap<RegisterRequestDto, User>()
                .ForMember(dest => dest.Files, opt => opt.Ignore())
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
                .ReverseMap();

            CreateMap<LoginRequestDto, User>()
                .ForMember(dest => dest.Files, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<UpdateUserRequestDto, User>()
                .ForMember(dest => dest.UserName, opt => opt.Condition(src => !string.IsNullOrEmpty(src.Username)))
                .ForMember(dest => dest.Email, opt => opt.Condition(src => !string.IsNullOrEmpty(src.Email)))
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.Files, opt => opt.Ignore())
                .ReverseMap();


            // Map File
            CreateMap<File, FileDto>()
                .ForMember(dest => dest.OwnerID, opt => opt.MapFrom(src => src.Owner.Id));
            CreateMap<FileUploadDto, File>()
                .ForMember(dest => dest.FileID, opt => opt.MapFrom(src => Guid.NewGuid())) // Generar ID único
                .ForMember(dest => dest.FileName, opt => opt.MapFrom(src => src.UploadedFile.FileName)) // Nombre del archivo
                .ForMember(dest => dest.FilePath, opt => opt.MapFrom(src => src.FolderPath ?? string.Empty)) // Se asignará manualmente
                .ForMember(dest => dest.FileSize, opt => opt.MapFrom(src => src.UploadedFile.Length)) // Tamaño del archivo
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.UtcNow)) // Fecha actual
                .ForMember(dest => dest.Owner, opt => opt.Ignore())
                .ForMember(dest => dest.SharedWithUsers, opt => opt.Ignore()) // Propiedad de navegación
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => false)) // Por defecto, no eliminado
                .ReverseMap();
            CreateMap<UpdateFileRequestDto, File>()
                .ForMember(dest => dest.FilePath, opt => opt.MapFrom(src => src.FolderPath ?? string.Empty))
                .ReverseMap();

            //Map UserFile
            CreateMap<ShareFileDto, UserFile>()
            .ForMember(dest => dest.File, opt => opt.MapFrom<FileResolver>())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.PermissionType, opt => opt.MapFrom(src => src.PermissionType))
            .ForMember(dest => dest.SharedDate, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ReverseMap();
            CreateMap<UserFile, UserFileDto>()
                .ForMember(dest => dest.UserFileID, opt => opt.MapFrom(src => src.UserFileID))
                .ForMember(dest => dest.FileID, opt => opt.MapFrom(src => src.File.FileID))
                .ForMember(dest => dest.FileName, opt => opt.MapFrom(src => src.File.FileName)) // Incluye FileName
                .ForMember(dest => dest.OwnerName, opt => opt.MapFrom(src => src.File.Owner.UserName)) // Accede a UserName de Owner
                .ForMember(dest => dest.PermissionType, opt => opt.MapFrom(src => src.PermissionType))
                .ForMember(dest => dest.SharedDate, opt => opt.MapFrom(src => src.SharedDate))
                .ReverseMap();


            //Map AuditLog
            CreateMap<File, AuditLog>()
                .ForMember(dest => dest.Action, opt => opt.Ignore())
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Owner.UserName))
                .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(_ => DateTime.UtcNow));
            CreateMap<UserFile, AuditLog>()
                .ForMember(dest => dest.UserID, opt => opt.MapFrom(src => src.User.Id))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.FileID, opt => opt.MapFrom(src => src.File.FileID))
                .ForMember(dest => dest.FileName, opt => opt.MapFrom(src => src.File.FileName))
                .ForMember(dest => dest.FilePath, opt => opt.MapFrom(src => src.File.FilePath))
                .ForMember(dest => dest.FileSize, opt => opt.MapFrom(src => src.File.FileSize))
                .ForMember(dest => dest.Action, opt => opt.Ignore()) // La acción se establecerá manualmente
                .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(_ => DateTime.UtcNow));


        }
    }
}
