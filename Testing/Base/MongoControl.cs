using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System.Linq.Expressions;
using static Testing.Base.BaseMongo;

namespace Testing.Base
{
    //Контекст MongoDB
    public static class MongoContext
    {
        private static readonly MongoClient _client = new MongoClient("mongodb://localhost:27017");

        private static readonly IMongoDatabase _database = _client.GetDatabase("Test2");

        public static IMongoCollection<T> GetCollection<T>(string name)
        {
            return _database.GetCollection<T>(name);
        }
    }

    // Коллекции
    public static class Repos
    {
        public static MongoRepo<Company> Company = new("Company");
        public static MongoRepo<Role> Role = new("Role");
        public static MongoRepo<Developer> Developer = new("Developer");
        public static MongoRepo<Application> Application = new("Application");
        public static MongoRepo<MetricType> MetricType = new("MetricType");
        public static MongoRepo<Metric> Metric = new("Metric");
        public static MongoRepo<Instance> Instance = new("Instance");
        public static MongoRepo<MAttribute> Attribute = new("Attribute");
        public static MongoRepo<Value> Value = new("Value");
        public static MongoRepo<ABDescription> Description = new("Description");
        public static MongoRepo<AbTest> AbTest = new("AbTest");
        public static MongoRepo<Variant> Variant = new("Variant");
        public static MongoRepo<Result> Result = new("Result");
    }

    // Репозиторий
    public class MongoRepo<T> where T : class
    {
        private readonly IMongoCollection<T> _collection;

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
        /// Получение записей по ObjectId
        /// </summary>
        public async Task<T?> GetById(ObjectId id, CancellationToken ct = default)
        {
            var filter = Builders<T>.Filter.Eq("_id", id);
            return await _collection.Find(filter).FirstOrDefaultAsync(ct);
        }

        /// <summary>
        /// Получение записи по строковому id
        /// </summary>
        public async Task<T?> GetById(string id, CancellationToken ct = default)
        {
            if (!ObjectId.TryParse(id, out var objectId)) return null;

            return await GetById(objectId, ct);
        }

        /// <summary>
        /// Получение значения по условию
        /// </summary>
        public async Task<T?> FirstOrDefault(Expression<Func<T, bool>> filter, CancellationToken ct = default)
        {
            return await _collection.Find(filter).FirstOrDefaultAsync(ct);
        }

        /// <summary>
        /// Получение списка по условию
        /// </summary>
        public async Task<List<T>> Where(Expression<Func<T, bool>> filter, CancellationToken ct = default)
        {
            return await _collection.Find(filter).ToListAsync(ct);
        }

        /// <summary>
        /// Получение всех записей
        /// </summary>
        public async Task<List<T>> GetAll(CancellationToken ct = default)
        {
            return await _collection.Find(Builders<T>.Filter.Empty).ToListAsync(ct);
        }

        // Создание

        /// <summary>
        /// Создание одного объекта
        /// </summary>
        public async Task Create(T entity, CancellationToken ct = default)
        {
            await _collection.InsertOneAsync(entity, cancellationToken: ct);
        }

        /// <summary>
        /// Создание нескольких объектов
        /// </summary>
        public async Task CreateMany(IEnumerable<T> entities, CancellationToken ct = default)
        {
            await _collection.InsertManyAsync(entities, cancellationToken: ct);
        }

        // Обновление

        /// <summary>
        /// Полное обновление документа, перезапись всего документа
        /// </summary>
        public async Task<bool> Replace(ObjectId id, T entity, CancellationToken ct = default)
        {
            var filter = Builders<T>.Filter.Eq("_id", id);

            var result = await _collection.ReplaceOneAsync(filter, entity, cancellationToken: ct);

            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        /// <summary>
        /// Частичное обновление
        /// </summary>
        public async Task<bool> Update(ObjectId id, UpdateDefinition<T> update, CancellationToken ct = default)
        {
            var filter = Builders<T>.Filter.Eq("_id", id);

            var result = await _collection.UpdateOneAsync(filter, update, cancellationToken: ct);

            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        //Удаление

        /// <summary>
        /// Удаление по ObjectId
        /// </summary>
        public async Task<bool> Delete(ObjectId id, CancellationToken ct = default)
        {
            var filter = Builders<T>.Filter.Eq("_id", id);

            var result = await _collection.DeleteOneAsync(filter, ct);

            return result.IsAcknowledged && result.DeletedCount > 0;
        }

        /// <summary>
        /// Удаление по строковому id
        /// </summary>
        public async Task<bool> Delete(string id, CancellationToken ct = default)
        {
            if (!ObjectId.TryParse(id, out var objectId)) return false;

            return await Delete(objectId, ct);
        }

        /// <summary>
        /// Удаление по условию
        /// </summary>
        public async Task<long> DeleteMany(Expression<Func<T, bool>> filter, CancellationToken ct = default)
        {
            var result = await _collection.DeleteManyAsync(filter, ct);
            return result.DeletedCount;
        }

        /// <summary>
        /// Очистка коллекции
        /// </summary>
        public async Task Clear(CancellationToken ct = default)
        {
            await _collection.DeleteManyAsync(Builders<T>.Filter.Empty, ct);
        }

        /// <summary>
        /// Проверка существования
        /// </summary>
        public async Task<bool> Exists(Expression<Func<T, bool>> filter, CancellationToken ct = default)
        {
            return await _collection.Find(filter).AnyAsync(ct);
        }

        /// <summary>
        /// Количество документов
        /// </summary>
        public async Task<long> Count(Expression<Func<T, bool>>? filter = null, CancellationToken ct = default)
        {
            filter ??= _ => true;

            return await _collection.CountDocumentsAsync(filter, cancellationToken: ct);
        }
    }
}