using Application.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.UnitOfwork
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository UserRepository { get; }
        IPostRepository PostRepository { get; }
        IConversationRepository ConversationRepository { get; }
        IAuthRepository authRepository { get; }
        IUserRepository userRepository { get; }
        Task<int> SaveChangesAsync();
        Task CommitAsync();
    }
}
