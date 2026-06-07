using ABLibrary.Models;
using MongoDB.Bson;
using Testing.DTO;
using Testing.Pattern;
using WebAppTest.Pages;

namespace WebAppTest.Control
{
    public class UiService : IUiService
    {
        private readonly ServiceControl _service;

        private readonly Facade _facade;

        public UiService(ServiceControl service, Facade facade)
        {
            _service = service;
            _facade = facade;
        }

        // Тесты

        public Task<Dictionary<string, string>>
            GetActiveTestsAsync(string appId)
        {
            return _service.Run(appId);
        }

        public Task<List<TestDto>>
            GetTestsAsync()
        {
            return _facade.GetAllTests();
        }

        public Task<List<TestDto>>
            GetAllTestsAsync()
        {
            return _facade.GetAllTests();
        }

        public Task<TestDto?>
            GetTestByIdAsync(string id)
        {
            return _facade.GetById(
                ObjectId.Parse(id));
        }

        public async Task CreateTestAsync(
            string applicationId,
            string name,
            string description)
        {
            await _facade.CreateTest(
                applicationId,
                name,
                description);
        }

        public async Task UpdateTestAsync(
            string id,
            string name,
            string description)
        {
            var test =
                await _facade.GetById(
                    ObjectId.Parse(id));

            if (test == null)
            {
                return;
            }

            test.Name = name;
            test.Description = description;

            await _facade.UpdateTest(test);
        }

        public Task StopTestAsync(string id)
        {
            return _facade.StopTest(id);
        }

        public Task ResumeTestAsync(string id)
        {
            return _facade.ResumeTest(id);
        }

        public Task DeleteTestAsync(string id)
        {
            return _facade.DeleteTest(id);
        }

        // Варианты

        public Task<List<VariantDto>>
            GetVariantsAsync()
        {
            return _facade.GetAllVariants();
        }

        public Task<List<VariantDto>>
            GetAllVariantsAsync()
        {
            return _facade.GetAllVariants();
        }

        public Task<List<TestWithVariantsDto>>
            GetTestsWithVariantsAsync()
        {
            return _facade.GetTests();
        }

        public Task DeleteVariantAsync(string id)
        {
            return _facade.DeleteVariant(id);
        }

        // Результаты

        public Task<List<AbResultsDto>>
            GetResultsAsync(string testId)
        {
            return _facade.GetResultsByTest(
                ObjectId.Parse(testId));
        }

        public Task<List<AbResultsDto>>
            GetResultsByInstanceAsync(string instanceId)
        {
            return _facade.GetResultsByInstance(
                ObjectId.Parse(instanceId));
        }

        // Приложения

        public Task<List<ApplicationDto>>
            GetApplicationsAsync()
        {
            return _facade.GetApplications();
        }

        public Task<ApplicationDto?>
            GetApplicationAsync(string id)
        {
            return _facade.GetApplication(
                ObjectId.Parse(id));
        }

        public Task<List<ApplicationWithInstancesDto>>
            GetApplicationWithInstanceAsync()
        {
            return _facade.GetApplicationsWithInstances();
        }

        public Task DeleteApplicationAsync(string id)
        {
            return _facade.DeleteApplication(id);
        }

        // Экземпляры

        public Task<List<InstanceDto>>
            GetInstancesAsync(string appId)
        {
            return _facade.GetInstancesByApp(
                ObjectId.Parse(appId));
        }

        public Task<InstanceDto?>
            GetInstanceAsync(string id)
        {
            return _facade.GetInstance(
                ObjectId.Parse(id));
        }

        // Метрики

        public Task<List<MetricWithTypeDto>>
            GetMetricsWithTypesAsync(string appId)
        {
            return _facade.GetMetricsWithTypes(
                ObjectId.Parse(appId));
        }

        // Оборудование

        public Task<List<EquipParamDto>>
            GetEquipParamsAsync()
        {
            return _facade.GetEquipParam();
        }

        public Task<List<ValueDto>>
            GetValuesByApplicationAsync(string appId)
        {
            return _facade.GetDeviceValues(
                ObjectId.Parse(appId));
        }

        // Аналитика

        public async Task<List<TestDto>>
            GetActiveTestsOnlyAsync()
        {
            var tests =
                await _facade.GetAllTests();

            return tests
                .Where(x => x.Enabled)
                .ToList();
        }

        public async Task<AnalyticDto>
            GetAnalyticsAsync()
        {
            var tests =
                await _facade.GetAllTests();

            var variants =
                await _facade.GetAllVariants();

            var results =
                await _facade.GetAllResults();

            var totalUsers =
                results.Count;

            return new AnalyticDto
            {
                TotalTests =
                    tests.Count,

                ActiveTests =
                    tests.Count(t => t.Enabled),

                TotalVariants =
                    variants.Count,

                TotalUsers =
                    totalUsers,

                AvgUsersPerTest =
                    tests.Count == 0
                        ? 0
                        : (double)totalUsers
                            / tests.Count
            };
        }

        public Task<int>
            GetTotalVariants()
        {
            return _facade.GetVariantsCount();
        }

        public async Task SaveEventAsync(TestEvent evt)
        {
            await _facade.SaveEvent(evt);
        }

        // Аутентификация

        public async Task<DeveloperDto?> GetDeveloperByLoginAsync(string login)
        {
            return await _facade.GetDeveloperByLogin(login);
        }

        public async Task<bool> RegisterDeveloperAsync(string login, string password)
        {
            return await _facade.RegisterDeveloper(login, password);
        }

        public async Task<bool> VerifyPasswordAsync(string password, string hash)
        {
            return await Task.Run(() => _facade.VerifyPassword(password, hash));
        }

        public async Task CreateVariantAsync(string testId, string name, string description)
        {
            await _facade.CreateVariant(testId, name, description);
        }

        public async Task<List<AbResultsDto>> GetAllResultsAsync()
        {
            return await _facade.GetAllResults();
        }

        // Приложения
        public async Task CreateApplicationAsync(string name, string description)
        {
            await _facade.CreateApplication(name, description);
        }

        public async Task UpdateApplicationAsync(string id, string name, string description)
        {
            await _facade.UpdateApplication(id, name, description);
        }

        // Экземпляры
        public async Task CreateInstanceAsync(string applicationId, string name, int version)
        {
            await _facade.CreateInstance(applicationId, name, version);
        }

        public async Task UpdateInstanceAsync(string id, string name, int version)
        {
            await _facade.UpdateInstance(id, name, version);
        }

        public async Task DeleteInstanceAsync(string id)
        {
            await _facade.DeleteInstance(id);
        }

        private static readonly Dictionary<string, string> _testStrategies = new();

        public Task<string?> GetTestStrategyAsync(string testId)
        {
            _testStrategies.TryGetValue(testId, out var strategy);
            return Task.FromResult<string?>(strategy);
        }

        public Task SetTestStrategyAsync(string testId, string strategy)
        {
            _testStrategies[testId] = strategy;
            return Task.CompletedTask;
        }

        public async Task<List<TestsModel.MABStatRow>> GetMABStatsAsync(string testId)
        {
            var results = await _facade.GetResultsByTest(ObjectId.Parse(testId));
            var variants = await _facade.GetAllVariants();
            var testVariants = variants.Where(v => v.AbTestId == testId).ToList();

            var stats = new List<TestsModel.MABStatRow>();

            foreach (var variant in testVariants)
            {
                var variantResults = results.Count(r => r.VariantId == variant.Id);
                var variantSuccesses = variantResults; // Для бинарных метрик

                stats.Add(new TestsModel.MABStatRow
                {
                    VariantId = variant.Id,
                    VariantName = variant.Name,
                    Count = variantResults,
                    Successes = variantSuccesses,
                    ConversionRate = variantResults > 0 ? (double)variantSuccesses / variantResults : 0,
                    ConfidenceInterval = CalculateConfidenceInterval(variantSuccesses, variantResults)
                });
            }

            return stats;
        }

        private string CalculateConfidenceInterval(int successes, int total)
        {
            if (total == 0) return "Нет данных";

            double p = (double)successes / total;
            double z = 1.96; // 95% доверительный интервал
            double se = Math.Sqrt(p * (1 - p) / total);
            double ci = z * se;

            double lower = Math.Max(0, p - ci);
            double upper = Math.Min(1, p + ci);

            return $"{p:P1} (±{ci:P1})";
        }

    }
}