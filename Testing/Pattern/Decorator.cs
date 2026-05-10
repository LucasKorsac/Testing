using System.Linq.Expressions;
using MongoDB.Bson;
using MongoDB.Driver;
using Testing.Base;

namespace Testing.Pattern
{
    /// <summary>
    /// Декоратор Mongo-репозитория.
    /// Добавляет логирование всех операций, не изменяя поведение базового репозитория.
    /// </summary>
    public class Decorator<T> : IMongoRepo<T> where T : class
    {
        /// <summary>
        /// Базовый репозиторий, который выполняет реальные операции с MongoDB
        /// </summary>
        private readonly IMongoRepo<T> _inner;

        /// <summary>
        /// Логгер (вынесен в абстракцию, чтобы не зависеть от Console)
        /// </summary>
        private readonly RepositoryLogger _logger;

        public Decorator(IMongoRepo<T> inner, RepositoryLogger logger)
        {
            _inner = inner;
            _logger = logger;
        }

        /// <summary>
        /// Прямой доступ к IQueryable коллекции (без логирования, так как это не операция)
        /// </summary>
        public IQueryable<T> Query => _inner.Query;

        public async Task<T?> GetById(ObjectId id, CancellationToken ct = default)
        {
            _logger.Log($"GetById(ObjectId): {id}");
            return await _inner.GetById(id, ct);
        }

        public async Task<T?> GetById(string id, CancellationToken ct = default)
        {
            _logger.Log($"GetById(string): {id}");
            return await _inner.GetById(id, ct);
        }

        public async Task<List<T>> GetAll(CancellationToken ct = default)
        {
            _logger.Log("GetAll");
            return await _inner.GetAll(ct);
        }

        public async Task<List<T>> Where(Expression<Func<T, bool>> filter, CancellationToken ct = default)
        {
            _logger.Log("Where executed");
            return await _inner.Where(filter, ct);
        }

        public async Task<T?> FirstOrDefault(Expression<Func<T, bool>> filter, CancellationToken ct = default)
        {
            _logger.Log("FirstOrDefault executed");
            return await _inner.FirstOrDefault(filter, ct);
        }

        public async Task Create(T entity, CancellationToken ct = default)
        {
            _logger.Log("Create");
            await _inner.Create(entity, ct);
        }

        public async Task CreateMany(IEnumerable<T> entities, CancellationToken ct = default)
        {
            _logger.Log("CreateMany");
            await _inner.CreateMany(entities, ct);
        }

        public async Task Update(ObjectId id, T entity, CancellationToken ct = default)
        {
            _logger.Log($"Update(ObjectId): {id}");
            await _inner.Update(id, entity, ct);
        }

        public async Task Update(string id, T entity, CancellationToken ct = default)
        {
            _logger.Log($"Update(string): {id}");
            await _inner.Update(id, entity, ct);
        }

        public async Task<bool> Update(ObjectId id, UpdateDefinition<T> update, CancellationToken ct = default)
        {
            _logger.Log($"Partial Update: {id}");
            return await _inner.Update(id, update, ct);
        }

        public async Task Delete(ObjectId id, CancellationToken ct = default)
        {
            _logger.Log($"Delete(ObjectId): {id}");
            await _inner.Delete(id, ct);
        }

        public async Task Delete(string id, CancellationToken ct = default)
        {
            _logger.Log($"Delete(string): {id}");
            await _inner.Delete(id, ct);
        }

        public async Task<long> DeleteMany(Expression<Func<T, bool>> filter, CancellationToken ct = default)
        {
            _logger.Log("DeleteMany");
            return await _inner.DeleteMany(filter, ct);
        }

        public async Task DeleteAll(CancellationToken ct = default)
        {
            _logger.Log("DeleteAll");
            await _inner.DeleteAll(ct);
        }

        public async Task<bool> Exists(Expression<Func<T, bool>> filter, CancellationToken ct = default)
        {
            _logger.Log("Exists");
            return await _inner.Exists(filter, ct);
        }

        public async Task<long> Count(Expression<Func<T, bool>>? filter = null, CancellationToken ct = default)
        {
            _logger.Log("Count");
            return await _inner.Count(filter, ct);
        }

        public async Task<bool> Replace(ObjectId id, T entity, CancellationToken ct = default)
        {
            _logger.Log($"Replace: {id}");
            return await _inner.Replace(id, entity, ct);
        }
    }

}