using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq.Expressions;
using Testing.Pattern;
using static Testing.Base.BaseMongo;

namespace Testing.Base
{
    /// <summary> Контекст подключения к MongoDB </summary>
    public static class MongoContext
    {
        /// <summary> Mongo клиент </summary>
        private static readonly MongoClient _client = new MongoClient("mongodb://localhost:27017");

        /// <summary> База данных </summary>
        private static readonly IMongoDatabase _database = _client.GetDatabase("ABTesting");

        /// <summary> Получение объекта базы данных </summary>
        public static IMongoDatabase Database => _database;

        /// <summary> Получение коллекции </summary>
        public static IMongoCollection<T> GetCollection<T>(string name)
        {
            return _database.GetCollection<T>(name);
        }
    }

    // Репозитории

    /// <summary> Хранилище репозиториев </summary>
    public class Repos
    {
        public IMongoRepo<Roles> Role { get; }
        public IMongoRepo<Developers> Developer { get; }
        public IMongoRepo<Applications> Application { get; }
        public IMongoRepo<MetricTypes> MetricType { get; }
        public IMongoRepo<Metrics> Metric { get; }
        public IMongoRepo<Instances> Instance { get; }
        public IMongoRepo<EquipParam> EquipParam { get; }
        public IMongoRepo<Values> Value { get; }
        public IMongoRepo<ABTests> AbTest { get; }
        public IMongoRepo<Variants> Variant { get; }
        public IMongoRepo<AbResults> Result { get; }
        public IMongoRepo<DevelopRoleApplic> DevRolApp { get; }

        /// <summary> Конструктор репозиториев </summary>
        public Repos(IMongoDatabase db)
        {
            Role = new MongoRepo<Roles>(db);

            Developer = new MongoRepo<Developers>(db);

            Application = new MongoRepo<Applications>(db);

            MetricType = new MongoRepo<MetricTypes>(db);

            Metric = new MongoRepo<Metrics>(db);

            Instance = new MongoRepo<Instances>(db);

            EquipParam = new MongoRepo<EquipParam>(db);

            Value = new MongoRepo<Values>(db);

            AbTest = new MongoRepo<ABTests>(db);

            Variant = new MongoRepo<Variants>(db);

            Result = new MongoRepo<AbResults>(db);

            DevRolApp = new MongoRepo<DevelopRoleApplic>(db);
        }
    }

    /// <summary> MongoDB репозиторий </summary>
    public class MongoRepo<T> : IMongoRepo<T>
        where T : class
    {
        /// <summary> Mongo коллекция </summary>
        private readonly IMongoCollection<T> _collection;

        /// <summary> Конструктор </summary>
        public MongoRepo(IMongoDatabase db)
        {
            _collection = db.GetCollection<T>(typeof(T).Name);
        }

        /// <summary> LINQ запросы </summary>
        public IQueryable<T> Query =>_collection.AsQueryable();

        // GET

        /// <summary> Получение документа по ObjectId </summary>
        public async Task<T?> GetById(ObjectId id, CancellationToken ct = default)
        {
            var filter = Builders<T>.Filter.Eq("_id", id);

            return await _collection
                .Find(filter)
                .FirstOrDefaultAsync(ct);
        }

        /// <summary> Получение документа по string id </summary>
        public async Task<T?> GetById(string id, CancellationToken ct = default)
        {
            if (!ObjectId.TryParse(id, out var objectId))
                return null;

            return await GetById(objectId, ct);
        }

        /// <summary> Получение первого документа по условию </summary>
        public async Task<T?> FirstOrDefault(Expression<Func<T, bool>> filter, CancellationToken ct = default)
        {
            return await _collection
                .Find(filter)
                .FirstOrDefaultAsync(ct);
        }

        /// <summary> Получение списка документов </summary>
        public async Task<List<T>> Where(Expression<Func<T, bool>> filter, CancellationToken ct = default)
        {
            return await _collection
                .Find(filter)
                .ToListAsync(ct);
        }

        /// <summary> Получение всех документов </summary>
        public async Task<List<T>> GetAll(CancellationToken ct = default)
        {
            return await _collection
                .Find(Builders<T>.Filter.Empty)
                .ToListAsync(ct);
        }

        // CREATE

        /// <summary> Добавление документа </summary>
        public async Task Create(T entity, CancellationToken ct = default)
        {
            await _collection.InsertOneAsync(entity, cancellationToken: ct);
        }

        /// <summary> Массовое добавление </summary>
        public async Task CreateMany(IEnumerable<T> entities, CancellationToken ct = default)
        {
            await _collection.InsertManyAsync(entities, cancellationToken: ct);
        }

        // UPDATE

        /// <summary> Полная замена документа </summary>
        public async Task<bool> Replace(ObjectId id, T entity, CancellationToken ct = default)
        {
            var filter = Builders<T>.Filter.Eq("_id", id);

            var result = await _collection.ReplaceOneAsync(filter, entity, cancellationToken: ct);

            return result.IsAcknowledged &&
                   result.ModifiedCount > 0;
        }

        /// <summary> Полное обновление документа </summary>
        public async Task Update(ObjectId id, T entity, CancellationToken ct = default)
        {
            await Replace(id, entity, ct);
        }

        /// <summary> Полное обновление по string id </summary>
        public async Task Update(string id, T entity, CancellationToken ct = default)
        {
            if (!ObjectId.TryParse(id, out var objectId))
                return;

            await Update(objectId, entity, ct);
        }

        /// <summary> Частичное обновление документа </summary>
        public async Task<bool> Update(ObjectId id, UpdateDefinition<T> update, CancellationToken ct = default)
        {
            var filter = Builders<T>.Filter.Eq("_id", id);

            var result = await _collection.UpdateOneAsync(filter, update, cancellationToken: ct);

            return result.IsAcknowledged &&
                   result.ModifiedCount > 0;
        }

        // DELETE

        /// <summary> Удаление документа </summary>
        public async Task Delete(ObjectId id, CancellationToken ct = default)
        {
            var filter = Builders<T>.Filter.Eq("_id", id);

            await _collection.DeleteOneAsync(filter, ct);
        }

        /// <summary> Удаление документа по string id </summary>
        public async Task Delete(string id, CancellationToken ct = default)
        {
            if (!ObjectId.TryParse(id, out var objectId))
                return;

            await Delete(objectId, ct);
        }

        /// <summary> Удаление по условию </summary>
        public async Task<long> DeleteMany(Expression<Func<T, bool>> filter, CancellationToken ct = default)
        {
            var result = await _collection.DeleteManyAsync(filter, ct);

            return result.DeletedCount;
        }

        /// <summary> Полная очистка коллекции </summary>
        public async Task DeleteAll(CancellationToken ct = default)
        {
            await _collection.DeleteManyAsync(Builders<T>.Filter.Empty, ct);
        }

        // HELPERS

        /// <summary> Проверка существования </summary>
        public async Task<bool> Exists(Expression<Func<T, bool>> filter, CancellationToken ct = default)
        {
            return await _collection
                .Find(filter)
                .AnyAsync(ct);
        }

        /// <summary> Подсчёт документов </summary>
        public async Task<long> Count(Expression<Func<T, bool>>? filter = null, CancellationToken ct = default)
        {
            filter ??= _ => true;

            return await _collection.CountDocumentsAsync(filter, cancellationToken: ct);
        }


        // Доп


        /// <summary> Получение отсортированного списка </summary>
        public async Task<List<T>> GetSorted(SortDefinition<T> sort, CancellationToken ct = default)
        {
            return await _collection
                .Find(Builders<T>.Filter.Empty)
                .Sort(sort)
                .ToListAsync(ct);
        }

        /// <summary> Получение части документов </summary>
        public async Task<List<T>> GetPaged(int page, int pageSize, CancellationToken ct = default)
        {
            if (page < 1)
                page = 1;

            return await _collection
                .Find(Builders<T>.Filter.Empty)
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync(ct);
        }

        /// <summary> Получение первого или создание нового </summary>
        public async Task<T?> FirstOrCreate(Expression<Func<T, bool>> filter, T entity, CancellationToken ct = default)
        {
            var existing = await FirstOrDefault(filter, ct);

            if (existing != null)
                return existing;

            await Create(entity, ct);

            return entity;
        }

        /// <summary> Получение случайного документа </summary>
        public async Task<T?> Random(CancellationToken ct = default)
        {
            var count = await Count(ct: ct);

            if (count == 0)
                return null;

            var rnd = new Random();

            var index = rnd.Next(0, (int)count);

            return await _collection
                .Find(Builders<T>.Filter.Empty)
                .Skip(index)
                .Limit(1)
                .FirstOrDefaultAsync(ct);
        }
    }
}