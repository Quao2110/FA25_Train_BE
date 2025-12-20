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

        // User Repository
        private IUserRepository _userRepository;
        public IUserRepository UserRepository => _userRepository ?? new UserRepository(_context);

        // Post Repository
        private IPostRepository _postRepository;
        public IPostRepository PostRepository => _postRepository ?? new PostRepository(_context);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
