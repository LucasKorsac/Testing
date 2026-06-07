using System;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Testing.Base;
using static Testing.Base.BaseMongo;
using Xunit;

namespace ABProjectTests.ServerTests
{
    /// <summary>
    /// Тестирование репозиториев MongoDB
    /// </summary>
    public class MongoRepoTests : IDisposable
    {
        private readonly IMongoDatabase _database;
        private readonly MongoRepo<TestEntity> _repo;

        private class TestEntity
        {
            public ObjectId Id { get; set; }
            public string Name { get; set; }
            public int Value { get; set; }
        }

        public MongoRepoTests()
        {
            var client = new MongoClient("mongodb://localhost:27017");
            _database = client.GetDatabase("ABTesting_Test");
            _repo = new MongoRepo<TestEntity>(_database);
            ClearCollection().Wait();
        }

        [Fact]
        public async Task Create_ShouldAddEntity()
        {
            // Arrange
            var entity = new TestEntity { Name = "Тест", Value = 100 };

            // Act
            await _repo.Create(entity);

            // Assert
            var result = await _repo.GetById(entity.Id);
            Assert.NotNull(result);
            Assert.Equal("Тест", result.Name);
        }

        [Fact]
        public async Task GetById_ShouldReturnCorrectEntity()
        {
            // Arrange
            var entity = new TestEntity { Name = "Поиск по ID", Value = 200 };
            await _repo.Create(entity);

            // Act
            var result = await _repo.GetById(entity.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(entity.Id, result.Id);
            Assert.Equal("Поиск по ID", result.Name);
        }

        [Fact]
        public async Task Update_ShouldModifyEntity()
        {
            // Arrange
            var entity = new TestEntity { Name = "До обновления", Value = 10 };
            await _repo.Create(entity);

            // Act
            entity.Name = "После обновления";
            entity.Value = 20;
            await _repo.Update(entity.Id, entity);
            var result = await _repo.GetById(entity.Id);

            // Assert
            Assert.Equal("После обновления", result.Name);
            Assert.Equal(20, result.Value);
        }

        [Fact]
        public async Task Delete_ShouldRemoveEntity()
        {
            // Arrange
            var entity = new TestEntity { Name = "Для удаления", Value = 5 };
            await _repo.Create(entity);

            // Act
            await _repo.Delete(entity.Id);
            var result = await _repo.GetById(entity.Id);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAll_ShouldReturnAllEntities()
        {
            // Arrange
            await _repo.Create(new TestEntity { Name = "Элемент 1", Value = 1 });
            await _repo.Create(new TestEntity { Name = "Элемент 2", Value = 2 });

            // Act
            var results = await _repo.GetAll();

            // Assert
            Assert.Equal(2, results.Count);
        }

        [Fact]
        public async Task Exists_ShouldReturnTrueForExistingEntity()
        {
            // Arrange
            var entity = new TestEntity { Name = "Существующий", Value = 99 };
            await _repo.Create(entity);

            // Act
            var exists = await _repo.Exists(x => x.Name == "Существующий");

            // Assert
            Assert.True(exists);
        }

        [Fact]
        public async Task Count_ShouldReturnCorrectNumberOfEntities()
        {
            // Arrange
            await _repo.Create(new TestEntity { Name = "Элемент A", Value = 1 });
            await _repo.Create(new TestEntity { Name = "Элемент B", Value = 2 });
            await _repo.Create(new TestEntity { Name = "Элемент C", Value = 3 });

            // Act
            var count = await _repo.Count();

            // Assert
            Assert.Equal(3, count);
        }

        [Fact]
        public async Task DeleteMany_ShouldRemoveMatchingEntities()
        {
            // Arrange
            await _repo.Create(new TestEntity { Name = "Удалить 1", Value = 1 });
            await _repo.Create(new TestEntity { Name = "Удалить 2", Value = 2 });
            await _repo.Create(new TestEntity { Name = "Оставить", Value = 3 });

            // Act
            var deletedCount = await _repo.DeleteMany(x => x.Name.StartsWith("Удалить"));

            // Assert
            Assert.Equal(2, deletedCount);
        }

        [Fact]
        public async Task DeleteAll_ShouldRemoveAllEntities()
        {
            // Arrange
            await _repo.Create(new TestEntity { Name = "Элемент 1", Value = 1 });
            await _repo.Create(new TestEntity { Name = "Элемент 2", Value = 2 });

            // Act
            await _repo.DeleteAll();
            var count = await _repo.Count();

            // Assert
            Assert.Equal(0, count);
        }

        private async Task ClearCollection()
        {
            await _repo.DeleteAll();
        }

        public void Dispose()
        {
            ClearCollection().Wait();
            _database.Client.DropDatabase("ABTesting_Test");
        }
    }
}