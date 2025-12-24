using Domain.Entities;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Application.Interfaces.Repository
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(Guid id);
        Task CreateAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        void DeleteById(Guid id);
        Task<T?> FindAsync(Expression<Func<T, bool>> predicate);
        Task<T?> GetByEmailAsync(string email);
    }
}
