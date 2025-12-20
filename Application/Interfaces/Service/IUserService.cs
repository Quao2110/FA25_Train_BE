using Application.DTOs.RequestDTOs;
using Application.DTOs.ResponseDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Service
{
    public interface IUserService
    {
        Task<IEnumerable<UserResponseDTO>> GetAllAsync();
        Task<UserResponseDTO> GetById(Guid id);
        Task CreateAsync(CreateUserDTOs requestDTO);
        Task UpdateAsync(UpdateUserDTO requestDTO);
        Task DeleteAsync(Guid id);
    }
}
