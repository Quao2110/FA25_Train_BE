using Application.Interfaces.Repository;
using Application.Interfaces.UnitOfwork;
using Infrastructure.Models;
using Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.UnitOfWorks
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly UserOnlyContext _context;

        public UnitOfWork(UserOnlyContext context)
        {
            _context = context;
        }

        private IUserRepository _userRepository;
        public IUserRepository UserRepository => _userRepository ?? new UserRepository(_context);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
