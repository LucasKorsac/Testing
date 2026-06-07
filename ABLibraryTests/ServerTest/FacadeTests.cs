using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Testing.Base;
using Testing.DTO;
using Testing.Pattern;
using static Testing.Base.BaseMongo;
using Xunit;

namespace ABProjectTests.ServerTest
{
    /// <summary>
    /// Тестирование фасада системы
    /// </summary>
    public class FacadeTests : IDisposable
    {
        private readonly IMongoDatabase _database;
        private readonly Facade _facade;
        private readonly IMongoRepo<ABTests> _abTestRepo;
        private readonly IMongoRepo<Variants> _variantRepo;
        private readonly IMongoRepo<AbResults> _resultRepo;
        private readonly IMongoRepo<Applications> _applicationRepo;
        private readonly IMongoRepo<Instances> _instanceRepo;

        public FacadeTests()
        {
            // Подключение к тестовой базе
            var client = new MongoClient("mongodb://localhost:27017");
            _database = client.GetDatabase("ABTesting_Test");

            // Очистка тестовой базы
            ClearTestDatabase().Wait();

            // Создание репозиториев
            _abTestRepo = new MongoRepo<ABTests>(_database);
            _variantRepo = new MongoRepo<Variants>(_database);
            _resultRepo = new MongoRepo<AbResults>(_database);
            _applicationRepo = new MongoRepo<Applications>(_database);
            _instanceRepo = new MongoRepo<Instances>(_database);
            var metricRepo = new MongoRepo<Metrics>(_database);
            var metricTypeRepo = new MongoRepo<MetricTypes>(_database);
            var roleRepo = new MongoRepo<Roles>(_database);
            var developerRepo = new MongoRepo<Developers>(_database);
            var devRoleRepo = new MongoRepo<DevelopRoleApplic>(_database);
            var equipParamRepo = new MongoRepo<EquipParam>(_database);
            var valueRepo = new MongoRepo<Values>(_database);

            _facade = new Facade(
                _abTestRepo, _variantRepo, _resultRepo, _instanceRepo,
                _applicationRepo, devRoleRepo, metricRepo, metricTypeRepo,
                roleRepo, developerRepo, equipParamRepo, valueRepo
            );
        }

        [Fact]
        public async Task CreateTest_ShouldCreateNewTest()
        {
            // Arrange
            var appId = await CreateTestApplication();

            // Act
            await _facade.CreateTest(appId, "Тестовый тест", "Описание теста");

            // Assert
            var tests = await _facade.GetAllTests();
            Assert.Contains(tests, t => t.Name == "Тестовый тест");
        }

        [Fact]
        public async Task GetAllTests_ShouldReturnAllTests()
        {
            // Arrange
            var appId = await CreateTestApplication();
            await _facade.CreateTest(appId, "Тест 1", "Описание 1");
            await _facade.CreateTest(appId, "Тест 2", "Описание 2");

            // Act
            var tests = await _facade.GetAllTests();

            // Assert
            Assert.Equal(2, tests.Count);
        }

        [Fact]
        public async Task StopTest_ShouldDisableTest()
        {
            // Arrange
            var appId = await CreateTestApplication();
            await _facade.CreateTest(appId, "Тест для остановки", "Описание");
            var tests = await _facade.GetAllTests();
            var testId = tests.First().Id;

            // Act
            await _facade.StopTest(testId);
            var updatedTests = await _facade.GetAllTests();

            // Assert
            Assert.False(updatedTests.First().Enabled);
        }

        [Fact]
        public async Task ResumeTest_ShouldEnableTest()
        {
            // Arrange
            var appId = await CreateTestApplication();
            await _facade.CreateTest(appId, "Тест для возобновления", "Описание");
            var tests = await _facade.GetAllTests();
            var testId = tests.First().Id;
            await _facade.StopTest(testId);

            // Act
            await _facade.ResumeTest(testId);
            var updatedTests = await _facade.GetAllTests();

            // Assert
            Assert.True(updatedTests.First().Enabled);
        }

        [Fact]
        public async Task DeleteTest_ShouldRemoveTestAndRelatedData()
        {
            // Arrange
            var appId = await CreateTestApplication();
            await _facade.CreateTest(appId, "Тест для удаления", "Описание");
            var tests = await _facade.GetAllTests();
            var testId = tests.First().Id;

            // Act
            await _facade.DeleteTest(testId);
            var remainingTests = await _facade.GetAllTests();

            // Assert
            Assert.DoesNotContain(remainingTests, t => t.Id == testId);
        }

        [Fact]
        public async Task CreateApplication_ShouldCreateNewApplication()
        {
            // Arrange & Act
            var appId = await CreateTestApplication("Новое приложение", "Описание приложения");

            // Assert
            var apps = await _facade.GetApplications();
            Assert.Contains(apps, a => a.Name == "Новое приложение");
        }

        [Fact]
        public async Task DeleteApplication_ShouldRemoveApplicationAndRelatedData()
        {
            // Arrange
            var appId = await CreateTestApplication("Приложение для удаления", "Описание");

            // Act
            await _facade.DeleteApplication(appId);
            var apps = await _facade.GetApplications();

            // Assert
            Assert.DoesNotContain(apps, a => a.Id == appId);
        }

        [Fact]
        public async Task GetApplications_ShouldReturnAllApplications()
        {
            // Arrange
            await CreateTestApplication("Приложение 1", "Описание 1");
            await CreateTestApplication("Приложение 2", "Описание 2");

            // Act
            var apps = await _facade.GetApplications();

            // Assert
            Assert.Equal(2, apps.Count);
        }

        [Fact]
        public async Task CreateVariant_ShouldCreateNewVariant()
        {
            // Arrange
            var appId = await CreateTestApplication();
            await _facade.CreateTest(appId, "Тест для варианта", "Описание");
            var tests = await _facade.GetAllTests();
            var testId = tests.First().Id;

            // Act
            await _facade.CreateVariant(testId, "Вариант A", "Описание варианта A");
            var variants = await _facade.GetAllVariants();

            // Assert
            Assert.Contains(variants, v => v.Name == "Вариант A");
        }

        [Fact]
        public async Task GetVariantsCount_ShouldReturnCorrectCount()
        {
            // Arrange
            var appId = await CreateTestApplication();
            await _facade.CreateTest(appId, "Тест 1", "Описание");
            var tests = await _facade.GetAllTests();
            var testId = tests.First().Id;
            await _facade.CreateVariant(testId, "Вариант 1", "Описание");
            await _facade.CreateVariant(testId, "Вариант 2", "Описание");

            // Act
            var count = await _facade.GetVariantsCount();

            // Assert
            Assert.Equal(2, count);
        }

        [Fact]
        public async Task GetTests_ShouldReturnTestsWithVariants()
        {
            // Arrange
            var appId = await CreateTestApplication();
            await _facade.CreateTest(appId, "Тест с вариантами", "Описание");
            var tests = await _facade.GetAllTests();
            var testId = tests.First().Id;
            await _facade.CreateVariant(testId, "Вариант A", "Описание A");
            await _facade.CreateVariant(testId, "Вариант B", "Описание B");

            // Act
            var testsWithVariants = await _facade.GetTests();

            // Assert
            var test = testsWithVariants.FirstOrDefault(t => t.Test.Name == "Тест с вариантами");
            Assert.NotNull(test);
            Assert.Equal(2, test.Variants.Count);
        }

        [Fact]
        public async Task GetResultsByTest_ShouldReturnResultsForTest()
        {
            // Arrange
            var appId = await CreateTestApplication();
            await _facade.CreateTest(appId, "Тест для результатов", "Описание");
            var tests = await _facade.GetAllTests();
            var testId = tests.First().Id;
            await _facade.CreateVariant(testId, "Вариант A", "Описание A");

            // Создаем экземпляр и результат
            var instance = new Instances
            {
                Id = ObjectId.GenerateNewId(),
                ApplicationId = ObjectId.Parse(appId),
                Name = "Тестовый экземпляр",
                Version = 1,
                Date = DateTime.UtcNow
            };
            await _instanceRepo.Create(instance);

            var variants = await _facade.GetAllVariants();
            var variantId = variants.First().Id;

            var result = new AbResults
            {
                Id = ObjectId.GenerateNewId(),
                VariantId = ObjectId.Parse(variantId),
                InstanceId = instance.Id
            };
            await _resultRepo.Create(result);

            // Act
            var results = await _facade.GetResultsByTest(ObjectId.Parse(testId));

            // Assert
            Assert.Single(results);
        }

        [Fact]
        public async Task GetActiveTestsCount_ShouldReturnOnlyEnabledTests()
        {
            // Arrange
            var appId = await CreateTestApplication();
            await _facade.CreateTest(appId, "Активный тест", "Описание");
            await _facade.CreateTest(appId, "Неактивный тест", "Описание");
            var tests = await _facade.GetAllTests();
            var inactiveTestId = tests.Last().Id;
            await _facade.StopTest(inactiveTestId);

            // Act
            var activeCount = await _facade.GetActiveTestsCount();

            // Assert
            Assert.Equal(1, activeCount);
        }

        [Fact]
        public async Task GetAnalytics_ShouldReturnCorrectStatistics()
        {
            // Arrange
            var appId = await CreateTestApplication();
            await _facade.CreateTest(appId, "Тест 1", "Описание");
            await _facade.CreateTest(appId, "Тест 2", "Описание");

            // Act
            var analytics = new AnalyticDto
            {
                TotalTests = (await _facade.GetAllTests()).Count,
                ActiveTests = await _facade.GetActiveTestsCount(),
                TotalVariants = await _facade.GetVariantsCount()
            };

            // Assert
            Assert.Equal(2, analytics.TotalTests);
        }

        #region Helper Methods

        private async Task<string> CreateTestApplication(string name = "Тестовое приложение", string description = "Описание")
        {
            var app = new Applications
            {
                Id = ObjectId.GenerateNewId(),
                Name = name,
                Description = description
            };
            await _applicationRepo.Create(app);
            return app.Id.ToString();
        }

        private async Task ClearTestDatabase()
        {
            var collections = new[] { "ABTests", "Variants", "AbResults", "Applications", "Instances" };
            foreach (var collection in collections)
            {
                await _database.DropCollectionAsync(collection);
            }
        }

        public void Dispose()
        {
            ClearTestDatabase().Wait();
        }

        #endregion
    }
}