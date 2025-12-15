using Application.DTOs.RequestDTOs;
using Application.DTOs.ResponseDTOs;
using Application.Interfaces.Service;
using Application.Interfaces.UnitOfwork;
using AutoMapper;
using Domain.Entities;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task CreateAsync(CreateUserDTOs requestDTO)
        {
            var entity = _mapper.Map<User>(requestDTO);
            await _unitOfWork.UserRepository.CreateAsync(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            _unitOfWork.UserRepository.DeleteById(id);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(UserResponseDTO requestDTO)
        {
            var entity = _mapper.Map<User>(requestDTO);
            _unitOfWork.UserRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<UserResponseDTO>> GetAllAsync()
        {
            var users = await _unitOfWork.UserRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<UserResponseDTO>>(users);
        }

        public async Task<UserResponseDTO> GetById(Guid id)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(id);
            return _mapper.Map<UserResponseDTO>(user);
        }

        public async Task UpdateAsync(UpdateUserDTO requestDTO)
        {
            var entity = _mapper.Map<User>(requestDTO);
            _unitOfWork.UserRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
