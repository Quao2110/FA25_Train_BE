using Application.DTOs.RequestDTOs.AuthDTO;
using Application.DTOs.ResponseDTOs;
using Application.Interfaces.Repository;
using Domain.Entities;
using Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly FakebookContext _context;

        public AuthRepository(FakebookContext context)
        {
            _context = context;
        }

        public async Task<ResponseDTO<object>> IsExistAccount(string email)
        {
            var user = await _context.Users.Where(u => u.Email == email).Select(u => u.Email).FirstOrDefaultAsync();
            if (user != null)
            {
                return new ResponseDTO<object>("Email của bạn đã được sử dụng", false);
            }

            return new ResponseDTO<object>("Tài khoản hợp lệ để đăng ký", true);
        }

        public async Task<User> LoginGoogle(string Email)
        {
            var user = await _context.Users.Where(c => c.Email == Email)
                .Select(u => new User
                {
                    UserId = u.UserId,
                    Username = u.Username,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                }).FirstOrDefaultAsync();
            if (user == null)
                return null;
            else
                return user;
        }

        public async Task<User> LoginUser(LoginRequestDTO loginRequest)
        {
            var user = await _context.Users.FirstOrDefaultAsync(c => c.Email == loginRequest.Email);
            return user;
        }

        public async Task<bool> Register(RegisterRequestDTO registerRequest)
        {
            User registerUser = new User()
            {
                Email = registerRequest.Email,
            };

            var hasher = new PasswordHasher<User>();
            registerUser.Password = hasher.HashPassword(registerUser, registerRequest.Password);

            if (registerUser == null) return false;
            else
            {
                await _context.Users.AddAsync(registerUser);
                return true;
            }
        }
    }
}
