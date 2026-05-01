using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System.Linq.Expressions;
using Testing.Pattern;
using static Testing.Base.BaseMongo;

namespace Testing.Base
{
    /// <summary>
    ///Контекст MongoDB
    /// </summary>
    public static class MongoContext
    {
        private static readonly MongoClient _client = new MongoClient("mongodb://localhost:27017");

        private static readonly IMongoDatabase _database = _client.GetDatabase("ABTesting");

        public static IMongoCollection<T> GetCollection<T>(string name)
        {
            return _database.GetCollection<T>(name);
        }
    }

    ///// <summary>
    /////Коллекции
    ///// </summary>
    public class Repos
    {
        public IMongoRepo<Companies> Company { get; }
        public IMongoRepo<Roles> Role { get; }
        public IMongoRepo<Developers> Developer { get; }
        public IMongoRepo<Applications> Application { get; }
        public IMongoRepo<MetricTypes> MetricType { get; }
        public IMongoRepo<Metrics> Metric { get; }
        public IMongoRepo<Instances> Instance { get; }
        public IMongoRepo<Attributes> Attribute { get; }
        public IMongoRepo<Values> Value { get; }
        public IMongoRepo<ABDescriptions> Description { get; }
        public IMongoRepo<ABTests> AbTest { get; }
        public IMongoRepo<Variants> Variant { get; }
        public IMongoRepo<AbResults> Result { get; }

        public Repos(IMongoRepo<Companies> company, IMongoRepo<Roles> role, IMongoRepo<Developers> developer, IMongoRepo<Applications> application,
            IMongoRepo<MetricTypes> metricType, IMongoRepo<Metrics> metric, IMongoRepo<Instances> instance, IMongoRepo<Attributes> attribute,
            IMongoRepo<Values> value, IMongoRepo<ABDescriptions> description, IMongoRepo<ABTests> abTest, IMongoRepo<Variants> variant,
            IMongoRepo<AbResults> result)
        {
            Company = company;
            Role = role;
            Developer = developer;
            Application = application;
            MetricType = metricType;
            Metric = metric;
            Instance = instance;
            Attribute = attribute;
            Value = value;
            Description = description;
            AbTest = abTest;
            Variant = variant;
            Result = result;
        }
    }

    /// <summary>
    /// MongoDB репозиторий
    /// </summary>
    public class MongoRepo<T> : IMongoRepo<T> where T : class
    {
        /// <summary>
        /// Коллекция
        /// </summary>
        private readonly IMongoCollection<T> _collection;

        /// <summary>
        /// Инициализация репозитория по имени коллекции
        /// </summary>

        public MongoRepo(IMongoDatabase db)
        {
            _collection = db.GetCollection<T>(typeof(T).Name);
        }

        //public MongoRepo(string collectionName)
        //{
        //    _collection = MongoContext.GetCollection<T>(collectionName);
        //}

        /// <summary>
        /// LINQ-доступ к коллекци
        /// </summary>
        public IQueryable<T> Query => _collection.AsQueryable();

        //Запросы

        /// <summary>
        /// Получение документа по ObjectId
        /// </summary>
        public async Task<T?> GetById(ObjectId id, CancellationToken ct = default)
        {
            var filter = Builders<T>.Filter.Eq("_id", id);
            return await _collection.Find(filter).FirstOrDefaultAsync(ct);
        }

        /// <summary>
        /// Получение документа по Id
        /// </summary>
        public async Task<T?> GetById(string id, CancellationToken ct = default)
        {
            if (!ObjectId.TryParse(id, out var objectId)) return null;

            return await GetById(objectId, ct);
        }

        /// <summary>
        /// Получение первого документа по условию
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
        /// Получение всех документов коллекции
        /// </summary>
        public async Task<List<T>> GetAll(CancellationToken ct = default)
        {
            return await _collection.Find(Builders<T>.Filter.Empty).ToListAsync(ct);
        }

        // Создание

        /// <summary>
        /// Создание одного документа
        /// </summary>
        public async Task Create(T entity, CancellationToken ct = default)
        {
            await _collection.InsertOneAsync(entity, cancellationToken: ct);
        }

        /// <summary>
        /// Массовое создание документов
        /// </summary>
        public async Task CreateMany(IEnumerable<T> entities, CancellationToken ct = default)
        {
            await _collection.InsertManyAsync(entities, cancellationToken: ct);
        }

        // Обновления

        /// <summary>
        /// Полная замена документа по ObjectId, перезапись
        /// </summary>
        public async Task Update(ObjectId id, T entity, CancellationToken ct = default)
        {
            var filter = Builders<T>.Filter.Eq("_id", id);
            await _collection.ReplaceOneAsync(filter, entity, cancellationToken: ct);
        }

        /// <summary>
        /// Перегрузка Update для string id
        /// </summary>
        public async Task Update(string id, T entity, CancellationToken ct = default)
        {
            if (!ObjectId.TryParse(id, out var objectId))
                return;

            await Update(objectId, entity, ct);
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

        // Удаление

        /// <summary>
        /// Удаление по ObjectId
        /// </summary>
        public async Task Delete(ObjectId id, CancellationToken ct = default)
        {
            await _collection.DeleteOneAsync(Builders<T>.Filter.Eq("_id", id), ct);
        }

        /// <summary>
        /// Удаление нескольких документов по условию
        /// </summary>
        public async Task<long> DeleteMany(Expression<Func<T, bool>> filter, CancellationToken ct = default)
        {
            var result = await _collection.DeleteManyAsync(filter, ct);
            return result.DeletedCount;
        }

        /// <summary>
        /// Полная очистка коллекции
        /// </summary>
        public async Task DeleteAll(CancellationToken ct = default)
        {
            await _collection.DeleteManyAsync(Builders<T>.Filter.Empty, ct);
        }

        /// <summary>
        /// Удаление по string
        /// </summary>
        public async Task Delete(string id, CancellationToken ct = default)
        {
            if (!ObjectId.TryParse(id, out var objectId))
                return;

            await Delete(objectId, ct);
        }

        /// <summary>
        /// Проверка существования документа по условию
        /// </summary>
        public async Task<bool> Exists(Expression<Func<T, bool>> filter, CancellationToken ct = default)
        {
            return await _collection.Find(filter).AnyAsync(ct);
        }

        /// <summary>
        /// Подсчет документов
        /// </summary>
        public async Task<long> Count(Expression<Func<T, bool>>? filter = null, CancellationToken ct = default)
        {
            filter ??= _ => true;

            return await _collection.CountDocumentsAsync(filter, cancellationToken: ct);
        }

        /// <summary>
        /// Полная замена документа
        /// </summary>
        public async Task<bool> Replace(ObjectId id, T entity, CancellationToken ct = default)
        {
            var result = await _collection.ReplaceOneAsync(
                Builders<T>.Filter.Eq("_id", id),
                entity,
                cancellationToken: ct);

            return result.IsAcknowledged && result.ModifiedCount > 0;
        }



        /// <summary>
        /// Удаление с возвратом результата
        /// </summary>
        //public async Task<bool> Delete(ObjectId id, CancellationToken ct = default)
        //{
        //    var result = await _collection.DeleteOneAsync(
        //        Builders<T>.Filter.Eq("_id", id),
        //        ct);

        //    return result.IsAcknowledged && result.DeletedCount > 0;
        //}

    }
}