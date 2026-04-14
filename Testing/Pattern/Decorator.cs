using System.Linq.Expressions;
using Testing.Base;
using static Testing.Interf;

namespace Testing.Pattern
{
    /// <summary>
    /// паттерн декоратор. Добавление логирования к репозиторию
    /// </summary>
    public class LoggingRepo<T> : IMongoRepo<T> where T : class
    {
        private readonly IMongoRepo<T> _inner;

        public LoggingRepo(IMongoRepo<T> inner)
        {
            _inner = inner;
        }

        public IQueryable<T> Query => _inner.Query;

        public async Task<T?> Get(string id)
        {
            Console.WriteLine($"[LOG] Get {typeof(T).Name} id={id}");

            var result = await _inner.Get(id);

            Console.WriteLine(result == null
                ? "[LOG] Not found"
                : "[LOG] Found");

            return result;
        }

        public async Task<List<T>> Get(Expression<Func<T, bool>> filter)
        {
            Console.WriteLine($"[LOG] Query {typeof(T).Name}");

            var result = await _inner.Get(filter);

            Console.WriteLine($"[LOG] Found {result.Count} items");

            return result;
        }

        public async Task Create(T entity)
        {
            Console.WriteLine($"[LOG] Create {typeof(T).Name}");
            await _inner.Create(entity);
        }

        public async Task CreateMany(IEnumerable<T> entities)
        {
            Console.WriteLine($"[LOG] CreateMany {typeof(T).Name}");
            await _inner.CreateMany(entities);
        }

        public async Task Update(string id, T entity)
        {
            Console.WriteLine($"[LOG] Update {typeof(T).Name} id={id}");
            await _inner.Update(id, entity);
        }

        public async Task Delete(string id)
        {
            Console.WriteLine($"[LOG] Delete {typeof(T).Name} id={id}");
            await _inner.Delete(id);
        }

        public async Task<bool> Exists(Expression<Func<T, bool>> filter)
        {
            return await _inner.Exists(filter);
        }

        public async Task DeleteAll()
        {
            Console.WriteLine($"[LOG] DeleteAll {typeof(T).Name}");
            await _inner.DeleteAll();
        }
    }
}
