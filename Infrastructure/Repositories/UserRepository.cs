using Application.Interfaces.Repository;
using Domain.Entities;
using Infrastructure.DbContexts;


namespace Infrastructure.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(FakebookContext context) : base(context)
        {
        }
    }
}
