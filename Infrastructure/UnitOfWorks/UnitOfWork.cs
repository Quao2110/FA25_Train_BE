using Application.Interfaces.Repository;
using Application.Interfaces.UnitOfwork;
using Domain.Entities;
using Infrastructure.DbContexts;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure.UnitOfWorks
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly FakebookContext _context;
        private IDbContextTransaction? _transaction;
        private bool _disposed;

        public UnitOfWork(FakebookContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // User Repository
        private IUserRepository _userRepository;
        public IUserRepository UserRepository => _userRepository ??= new UserRepository(_context);

        // Post Repository
        private IPostRepository _postRepository;
        public IPostRepository PostRepository => _postRepository ??= new PostRepository(_context);

        // Conversation Repository
        private IConversationRepository? _conversationRepository;
        public IConversationRepository ConversationRepository => _conversationRepository ??= new ConversationRepository(_context);

        public IAuthRepository authRepository => throw new NotImplementedException();

        public IUserRepository userRepository => throw new NotImplementedException();

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task CommitAsync()
        {
            try
            {
                await _context.Database.BeginTransactionAsync();
                await _context.SaveChangesAsync();
                await _context.Database.CommitTransactionAsync();
            }
            catch
            {
                await RollbackAsync();
                throw;
            }
        }

        private async Task RollbackAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _transaction?.Dispose();
                    _context.Dispose();
                }
                _disposed = true;
            }
        }
    }
}
