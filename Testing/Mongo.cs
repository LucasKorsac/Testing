using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Testing.Base;
using static Testing.Interf;

namespace Testing
{
    /// <summary>
    /// Реализация универсального MongoDB репозитория через интерфейс IMongoRepo
    /// </summary>
    internal class MongoRepo<T> : IMongoRepo<T> where T : class
    {
        /// <summary>
        /// Коллекция MongoDB
        /// </summary>
        private readonly IMongoCollection<T> _collection;

        /// <summary>
        /// Конструктор
        /// Получение коллекции по имени
        /// </summary>
        public MongoRepo(string collectionName)
        {
            _collection = MongoContext.GetCollection<T>(collectionName);
        }

        /// <summary>
        /// IQueryable для LINQ-запросов
        /// </summary>
        public IQueryable<T> Query => _collection.AsQueryable();

        // Запросы

        /// <summary>
        /// Получение документа по Id
        /// </summary>
        public async Task<T?> Get(string id)
        {
            // Пробуем распарсить ObjectId
            if (!ObjectId.TryParse(id, out var objectId)) return null;

            var filter = Builders<T>.Filter.Eq("_id", objectId);

            return await _collection
                .Find(filter)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Получение списка по условию
        /// </summary>
        public async Task<List<T>> Get(Expression<Func<T, bool>> filter)
        {
            return await _collection
                .Find(filter)
                .ToListAsync();
        }

        // Создание

        /// <summary>
        /// Создание одного документа
        /// </summary>
        public async Task Create(T entity)
        {
            await _collection.InsertOneAsync(entity);
        }

        /// <summary>
        /// Создание нескольких документов
        /// </summary>
        public async Task CreateMany(IEnumerable<T> entities)
        {
            await _collection.InsertManyAsync(entities);
        }

        // Обновление

        /// <summary>
        /// Полное обновление документа, перезапись всего объекта
        /// </summary>
        public async Task Update(string id, T entity)
        {
            // Если поле не передано.то оно будет удалено
            if (!ObjectId.TryParse(id, out var objectId)) return;

            var filter = Builders<T>.Filter.Eq("_id", objectId);

            await _collection.ReplaceOneAsync(filter, entity);
        }

        // Удаление

        /// <summary>
        /// Удаление документа по Id
        /// </summary>
        public async Task Delete(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId)) return;

            var filter = Builders<T>.Filter.Eq("_id", objectId);

            await _collection.DeleteOneAsync(filter);
        }

        /// <summary>
        /// Проверка существования документа
        /// </summary>
        public async Task<bool> Exists(Expression<Func<T, bool>> filter)
        {
            return await _collection
                .Find(filter)
                .AnyAsync();
        }

        /// <summary>
        /// Удаление всех документов из коллекции
        /// </summary>
        public async Task DeleteAll()
        {
            await _collection.DeleteManyAsync(Builders<T>.Filter.Empty);
        }
    }
}
