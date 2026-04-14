using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// IMongoRepo.cs
using System.Linq.Expressions;

namespace Testing
{
    public class Interf
    {
        public interface IMongoRepo<T> where T : class
        {
            IQueryable<T> Query { get; }

            Task<T?> Get(string id);
            Task<List<T>> Get(Expression<Func<T, bool>> filter);

            Task Create(T entity);
            Task CreateMany(IEnumerable<T> entities);

            Task Update(string id, T entity);
            Task Delete(string id);

            Task<bool> Exists(Expression<Func<T, bool>> filter);
            Task DeleteAll();
        }
    }
}
