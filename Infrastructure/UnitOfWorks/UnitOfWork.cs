using Application.Interfaces.Repository;
using Application.Interfaces.UnitOfwork;
using Infrastructure.DbContexts;
using Infrastructure.Repositories;

namespace Infrastructure.UnitOfWorks
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly FakebookContext _context;

        public UnitOfWork(FakebookContext context)
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
