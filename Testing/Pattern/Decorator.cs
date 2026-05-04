using System.Linq.Expressions;
using MongoDB.Bson;
using MongoDB.Driver;
using Testing.Base;

namespace Testing.Pattern
{
    /// <summary>
    /// Декоратор репозитория, добавляющий логирование операций, не меняет поведение основного репозитория, только оборачивает вызовы
    /// </summary>
    public class LogMongoRepo<T> : IMongoRepo<T> where T : class
    {
        // Внутренний репозиторий, к которому проксируются все вызовы
        private readonly IMongoRepo<T> _inner;

        public LogMongoRepo(IMongoRepo<T> inner)
        {
            _inner = inner;
        }

        // Доступ к запросам коллекции
        public IQueryable<T> Query => _inner.Query;

        // Получение записи по ObjectId с логированием
        public async Task<T?> GetById(ObjectId id, CancellationToken ct = default)
        {
            Console.WriteLine($"[LOG] GetById: {id}");
            return await _inner.GetById(id, ct);
        }

        // Получение записи по строковому id
        public async Task<T?> GetById(string id, CancellationToken ct = default)
        {
            Console.WriteLine($"[LOG] GetById(string): {id}");
            return await _inner.GetById(id, ct);
        }

        /// <summary> Получение всех записей </summary>
        public async Task<List<T>> GetAll(CancellationToken ct = default)
        {
            Console.WriteLine("[LOG] GetAll");
            return await _inner.GetAll(ct);
        }

        /// <summary> Запрос по условию </summary>
        public async Task<List<T>> Where(Expression<Func<T, bool>> filter, CancellationToken ct = default)
        {
            Console.WriteLine("[LOG] Where query executed");
            return await _inner.Where(filter, ct);
        }

        /// <summary> Получение первого элемента по условию </summary>
        public async Task<T?> FirstOrDefault(Expression<Func<T, bool>> filter, CancellationToken ct = default)
        {
            Console.WriteLine("[LOG] FirstOrDefault query executed");
            return await _inner.FirstOrDefault(filter, ct);
        }

        /// <summary> Создание одной записи </summary>
        public async Task Create(T entity, CancellationToken ct = default)
        {
            Console.WriteLine("[LOG] Create");
            await _inner.Create(entity, ct);
        }

        /// <summary> Создание нескольких записей </summary>
        public async Task CreateMany(IEnumerable<T> entities, CancellationToken ct = default)
        {
            Console.WriteLine("[LOG] CreateMany");
            await _inner.CreateMany(entities, ct);
        }

        /// <summary> Полное обновление по id </summary>
        public async Task Update(ObjectId id, T entity, CancellationToken ct = default)
        {
            Console.WriteLine($"[LOG] Update: {id}");
            await _inner.Update(id, entity, ct);
        }

        /// <summary> Обновление по строковому id </summary>
        public async Task Update(string id, T entity, CancellationToken ct = default)
        {
            Console.WriteLine($"[LOG] Update(string): {id}");
            await _inner.Update(id, entity, ct);
        }

        /// <summary> Частичное обновление </summary>
        public async Task<bool> Update(ObjectId id, UpdateDefinition<T> update, CancellationToken ct = default)
        {
            Console.WriteLine($"[LOG] Partial Update: {id}");
            return await _inner.Update(id, update, ct);
        }

        /// <summary> Удаление по id </summary>
        public async Task Delete(ObjectId id, CancellationToken ct = default)
        {
            Console.WriteLine($"[LOG] Delete: {id}");
            await _inner.Delete(id, ct);
        }

        /// <summary> Удаление по строковому id </summary>
        public async Task Delete(string id, CancellationToken ct = default)
        {
            Console.WriteLine($"[LOG] Delete(string): {id}");
            await _inner.Delete(id, ct);
        }

        /// <summary> Массовое удаление по условию </summary>
        public async Task<long> DeleteMany(Expression<Func<T, bool>> filter, CancellationToken ct = default)
        {
            Console.WriteLine("[LOG] DeleteMany");
            return await _inner.DeleteMany(filter, ct);
        }

        /// <summary> Удаление всех документов коллекции </summary>
        public async Task DeleteAll(CancellationToken ct = default)
        {
            Console.WriteLine("[LOG] DeleteAll");
            await _inner.DeleteAll(ct);
        }

        /// <summary> Проверка существования записи </summary>
        public async Task<bool> Exists(Expression<Func<T, bool>> filter, CancellationToken ct = default)
        {
            Console.WriteLine("[LOG] Exists check");
            return await _inner.Exists(filter, ct);
        }

        /// <summary> Подсчёт записей </summary>
        public async Task<long> Count(Expression<Func<T, bool>>? filter = null, CancellationToken ct = default)
        {
            Console.WriteLine("[LOG] Count");
            return await _inner.Count(filter, ct);
        }

        /// <summary> Полная замена документа </summary>
        public async Task<bool> Replace(ObjectId id, T entity, CancellationToken ct = default)
        {
            Console.WriteLine($"[LOG] Replace: {id}");
            return await _inner.Replace(id, entity, ct);
        }
    }
}