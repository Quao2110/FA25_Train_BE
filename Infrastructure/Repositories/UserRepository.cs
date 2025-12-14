using Application.Interfaces.Repository;
using Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class UserRepository : GenericRepository<Account>, IUserRepository
    {
        public UserRepository(UserOnlyContext _context) : base(_context)
        {
        }
    }
}
