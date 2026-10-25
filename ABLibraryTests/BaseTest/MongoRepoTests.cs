```csharp
using System;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Testing.Base;
using Xunit;

namespace ABProjectTests.ServerTests
{
    /// <summary>
    /// Интеграционные тесты MongoRepo.
    /// Для каждого экземпляра тестового класса используется
    /// отдельная временная база данных.
    /// </summary>
    public sealed class MongoRepoTests : IDisposable
    {
        private readonly MongoClient _client;
        private readonly IMongoDatabase _database;
        private readonly MongoRepo<TestEntity> _repo;
        private readonly string _databaseName;

        public MongoRepoTests()
        {
            var connectionString =
                Environment.GetEnvironmentVariable(
                    "MONGODB_TEST_CONNECTION_STRING")
                ?? "mongodb://localhost:27017";

            _client = new MongoClient(connectionString);

            _databaseName =
                $"ABProjectTests_{Guid.NewGuid():N}";

            _database = _client.GetDatabase(_databaseName);
            _repo = new MongoRepo<TestEntity>(_database);
        }

        private sealed class TestEntity
        {
            public ObjectId Id { get; set; }

            public string Name { get; set; } = string.Empty;

            public int Value { get; set; }
        }

        [Fact]
        public async Task Create_ShouldInsertEntity()
        {
            var entity = new TestEntity
            {
                Name = "Тестовая запись",
                Value = 100
            };

            await _repo.Create(entity);

            var result = await _repo.GetById(entity.Id);

            Assert.NotNull(result);
            Assert.Equal(entity.Id, result.Id);
            Assert.Equal("Тестовая запись", result.Name);
            Assert.Equal(100, result.Value);
        }

        [Fact]
        public async Task GetById_ShouldReturnEntity_WhenEntityExists()
        {
            var entity = new TestEntity
            {
                Name = "Поиск по ID",
                Value = 200
            };

            await _repo.Create(entity);

            var result = await _repo.GetById(entity.Id);

            Assert.NotNull(result);
            Assert.Equal(entity.Id, result.Id);
            Assert.Equal(entity.Name, result.Name);
            Assert.Equal(entity.Value, result.Value);
        }

        [Fact]
        public async Task GetById_ShouldReturnNull_WhenEntityDoesNotExist()
        {
            var result = await _repo.GetById(ObjectId.GenerateNewId());

            Assert.Null(result);
        }

        [Fact]
        public async Task GetById_ShouldReturnNull_WhenIdIsInvalid()
        {
            var result = await _repo.GetById("invalid-id");

            Assert.Null(result);
        }

        [Fact]
        public async Task Update_ShouldReplaceEntity()
        {
            var entity = new TestEntity
            {
                Name = "До обновления",
                Value = 10
            };

            await _repo.Create(entity);

            entity.Name = "После обновления";
            entity.Value = 20;

            await _repo.Update(entity.Id, entity);

            var result = await _repo.GetById(entity.Id);

            Assert.NotNull(result);
            Assert.Equal("После обновления", result.Name);
            Assert.Equal(20, result.Value);
        }

        [Fact]
        public async Task Delete_ShouldRemoveEntity()
        {
            var entity = new TestEntity
            {
                Name = "Для удаления",
                Value = 5
            };

            await _repo.Create(entity);

            await _repo.Delete(entity.Id);

            var result = await _repo.GetById(entity.Id);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetAll_ShouldReturnAllInsertedEntities()
        {
            await _repo.Create(new TestEntity
            {
                Name = "Элемент 1",
                Value = 1
            });

            await _repo.Create(new TestEntity
            {
                Name = "Элемент 2",
                Value = 2
            });

            var results = await _repo.GetAll();

            Assert.Equal(2, results.Count);
            Assert.Contains(results, x => x.Name == "Элемент 1");
            Assert.Contains(results, x => x.Name == "Элемент 2");
        }

        [Fact]
        public async Task Exists_ShouldReturnTrue_WhenEntityExists()
        {
            await _repo.Create(new TestEntity
            {
                Name = "Существующий",
                Value = 99
            });

            var exists = await _repo.Exists(
                x => x.Name == "Существующий");

            Assert.True(exists);
        }

        [Fact]
        public async Task Exists_ShouldReturnFalse_WhenEntityDoesNotExist()
        {
            var exists = await _repo.Exists(
                x => x.Name == "Несуществующий");

            Assert.False(exists);
        }

        [Fact]
        public async Task Count_ShouldReturnNumberOfInsertedEntities()
        {
            await _repo.Create(new TestEntity
            {
                Name = "Элемент A",
                Value = 1
            });

            await _repo.Create(new TestEntity
            {
                Name = "Элемент B",
                Value = 2
            });

            await _repo.Create(new TestEntity
            {
                Name = "Элемент C",
                Value = 3
            });

            var count = await _repo.Count();

            Assert.Equal(3, count);
        }

        [Fact]
        public async Task DeleteMany_ShouldDeleteOnlyMatchingEntities()
        {
            await _repo.Create(new TestEntity
            {
                Name = "Удалить 1",
                Value = 1
            });

            await _repo.Create(new TestEntity
            {
                Name = "Удалить 2",
                Value = 2
            });

            await _repo.Create(new TestEntity
            {
                Name = "Оставить",
                Value = 3
            });

            var deletedCount = await _repo.DeleteMany(
                x => x.Name.StartsWith("Удалить"));

            var remaining = await _repo.GetAll();

            Assert.Equal(2L, deletedCount);
            var remainingEntity = Assert.Single(remaining);
            Assert.Equal("Оставить", remainingEntity.Name);
        }

        [Fact]
        public async Task DeleteAll_ShouldEmptyCollection()
        {
            await _repo.Create(new TestEntity
            {
                Name = "Элемент 1",
                Value = 1
            });

            await _repo.Create(new TestEntity
            {
                Name = "Элемент 2",
                Value = 2
            });

            await _repo.DeleteAll();

            var count = await _repo.Count();

            Assert.Equal(0, count);
        }

        public void Dispose()
        {
            // Удаляем только временную базу этого тестового экземпляра.
            _client.DropDatabase(_databaseName);
        }
    }
}
```