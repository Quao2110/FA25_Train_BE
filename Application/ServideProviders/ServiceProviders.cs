using Application.Interfaces.Service;
using Application.Interfaces.ServiceProviders;
using Application.Interfaces.UnitOfwork;
using Application.Services;
using AutoMapper;

namespace Application.ServideProviders
{
    public class ServiceProviders : IServiceProviders
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ServiceProviders(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        private IUserService _userService;
        public IUserService UserService => _userService ??  new UserService(_unitOfWork, _mapper);
    }
}
