using Application.DTOs.RequestDTOs;
using Application.DTOs.RequestDTOs.PostDTO;
using Application.DTOs.ResponseDTOs;
using AutoMapper;
using Domain.Entities;

namespace Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserResponseDTO>().ReverseMap();
            CreateMap<User, CreateUserDTOs>().ReverseMap();
            CreateMap<User, UpdateUserDTO>().ReverseMap();
            CreateMap<Post, PostResponseDTO>().ReverseMap();
            CreateMap<Post, CreatePostRequestDTO>().ReverseMap();
            CreateMap<Post, UpdatePostRequestDTO>().ReverseMap();
        }
    }
}
