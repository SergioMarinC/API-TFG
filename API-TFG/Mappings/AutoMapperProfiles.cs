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
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<AddUserRequestDto, UserDto>().ReverseMap();
            CreateMap<UpdateUserRequestDto, UserDto>().ReverseMap();
            CreateMap<File, FileDto>().ReverseMap();
        }
    }
}
