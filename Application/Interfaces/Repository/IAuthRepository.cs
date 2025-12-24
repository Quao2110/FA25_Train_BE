using Application.DTOs.RequestDTOs.AuthDTO;
using Application.DTOs.ResponseDTOs;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Repository
{
    public interface IAuthRepository
    {
        Task<User> LoginUser(LoginRequestDTO loginRequest);
        Task<ResponseDTO<object>> IsExistAccount(string email);
        Task<bool> Register(RegisterRequestDTO registerRequest);
        Task<User> LoginGoogle(string Email);
    }
}
