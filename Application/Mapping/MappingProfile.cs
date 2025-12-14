using Application.DTOs.RequestDTOs;
using Application.DTOs.ResponseDTOs;
using AutoMapper;
using Infrastructure.Models;

namespace Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Account, UserResponseDTO>().ReverseMap();
            CreateMap<Account, CreateUserDTOs>().ReverseMap();
            CreateMap<Account, UpdateUserDTO>().ReverseMap();
        }
    }
}
