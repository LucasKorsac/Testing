using MongoDB.Bson;
using Testing.Base;
using static Testing.Base.BaseMongo;

namespace Testing.Pattern
{
    /// <summary>
    /// Фасад для работы с A/B тестами, приложениями и аналитикой
    /// </summary>
    public class Facade
    {
        private readonly IMongoRepo<ABTests> _abTests;

        private readonly IMongoRepo<Variants> _variants;

        private readonly IMongoRepo<AbResults> _results;

        private readonly IMongoRepo<Instances> _instances;

        private readonly IMongoRepo<Applications> _applications;

        private readonly IMongoRepo<DevelopRoleApplic> _devRoles;

        private readonly IMongoRepo<Metrics> _metrics;

        private readonly IMongoRepo<MetricTypes> _metricTypes;

        public Facade(
            IMongoRepo<ABTests> abTests,
            IMongoRepo<Variants> variants,
            IMongoRepo<AbResults> results,
            IMongoRepo<Instances> instances,
            IMongoRepo<Applications> applications,
            IMongoRepo<DevelopRoleApplic> devRoles,
            IMongoRepo<Metrics> metrics,
            IMongoRepo<MetricTypes> metricTypes)
        {
            _abTests = abTests;

            _variants = variants;

            _results = results;

            _instances = instances;

            _applications = applications;

            _devRoles = devRoles;

            _metrics = metrics;

            _metricTypes = metricTypes;
        }

       // Тесты

        /// <summary>
        /// Получение всех тестов
        /// </summary>
        public Task<List<ABTests>> GetAllTests()
        {
            return _abTests.GetAll();
        }

        /// <summary>
        /// Получение теста по id
        /// </summary>
        public async Task<ABTests?> GetById(ObjectId id)
        {
            return await _abTests.GetById(id);
        }

        /// <summary>
        /// Тесты вместе с вариантами
        /// </summary>
        public async Task<List<TestWithVariants>> GetTests()
        {
            var tests = await _abTests.GetAll();

            var variants = await _variants.GetAll();

            var grouped = variants
                .GroupBy(v => v.AbTestId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var result = new List<TestWithVariants>();

            foreach (var test in tests)
            {
                grouped.TryGetValue(test.Id, out var list);

                result.Add(new TestWithVariants
                {
                    Test = test,
                    Variants = list ?? new List<Variants>()
                });
            }

            return result;
        }

        /// <summary>
        /// Создание теста
        /// </summary>
        public async Task CreateTest(ABTests test)
        {
            await _abTests.Create(test);
        }

        /// <summary>
        /// Обновление теста
        /// </summary>
        public async Task UpdateTest(ABTests test)
        {
            await _abTests.Update(test.Id, test);
        }

        /// <summary>
        /// Удаление теста
        /// </summary>
        public async Task DeleteTest(ObjectId id)
        {
            var variants = await _variants.Where(v => v.AbTestId == id);

            var variantIds = variants
                .Select(v => v.Id)
                .ToList();

            await _results.DeleteMany(r => variantIds.Contains(r.VariantId));

            await _variants.DeleteMany(v => v.AbTestId == id);

            await _abTests.Delete(id);
        }

        /// <summary>
        /// Остановка теста
        /// </summary>
        public async Task StopTest(ObjectId id)
        {
            var test = await _abTests.GetById(id);

            if (test == null)
                return;

            test.Enabled = false;

            await _abTests.Update(id, test);
        }

        /// <summary>
        /// Возобновление теста
        /// </summary>
        public async Task ResumeTest(ObjectId id)
        {
            var test = await _abTests.GetById(id);

            if (test == null)
                return;

            test.Enabled = true;

            await _abTests.Update(id, test);
        }

        // Варианты

        /// <summary>
        /// Получение всех вариантов
        /// </summary>
        public Task<List<Variants>> GetAllVariants()
        {
            return _variants.GetAll();
        }

        /// <summary>
        /// Варианты конкретного теста
        /// </summary>
        public async Task<List<Variants>> GetVariantsByTest(ObjectId testId)
        {
            return await _variants.Where(v => v.AbTestId == testId);
        }

        /// <summary>
        /// Создание варианта
        /// </summary>
        public async Task CreateVariant(Variants variant)
        {
            await _variants.Create(variant);
        }

        /// <summary>
        /// Обновление варианта
        /// </summary>
        public async Task UpdateVariant(Variants variant)
        {
            await _variants.Update(variant.Id, variant);
        }

        /// <summary>
        /// Удаление варианта
        /// </summary>
        public async Task DeleteVariant(ObjectId variantId)
        {
            await _results.DeleteMany(r => r.VariantId == variantId);

            await _variants.Delete(variantId);
        }

        // Результаты

        /// <summary>
        /// Результаты теста
        /// </summary>
        public async Task<List<AbResults>> GetResults(ObjectId testId)
        {
            var variants = await _variants.Where(v => v.AbTestId == testId);

            var ids = variants
                .Select(v => v.Id)
                .ToList();

            return await _results.Where(r => ids.Contains(r.VariantId));
        }

        /// <summary>
        /// Добавление результата
        /// </summary>
        public async Task AddResult(AbResults result)
        {
            await _results.Create(result);
        }

        /// <summary>
        /// Получение результатов по экземпляру
        /// </summary>
        public async Task<List<AbResults>> GetResultsByInstance(ObjectId instanceId)
        {
            return await _results.Where(r => r.InstanceId == instanceId);
        }

        // Приложения

        /// <summary>
        /// Все приложения
        /// </summary>
        public Task<List<Applications>> GetApplications()
        {
            return _applications.GetAll();
        }

        /// <summary>
        /// Приложение по id
        /// </summary>
        public async Task<Applications?> GetApplication(ObjectId id)
        {
            return await _applications.GetById(id);
        }

        /// <summary>
        /// Создание приложения
        /// </summary>
        public async Task CreateApplication(Applications app)
        {
            await _applications.Create(app);
        }

        /// <summary>
        /// Обновление приложения
        /// </summary>
        public async Task UpdateApplication(Applications app)
        {
            await _applications.Update(app.Id, app);
        }

        /// <summary>
        /// Удаление приложения
        /// </summary>
        public async Task DeleteApplication(ObjectId id)
        {
            await _devRoles.DeleteMany(x => x.Application == id);

            var instances = await _instances.Where(x => x.ApplicationId == id);

            var instanceIds = instances
                .Select(x => x.Id)
                .ToList();

            await _results.DeleteMany(x => instanceIds.Contains(x.InstanceId));

            await _instances.DeleteMany(x => x.ApplicationId == id);

            await _applications.Delete(id);
        }

        // Экземпляры

        /// <summary>
        /// Экземпляры приложения
        /// </summary>
        public async Task<List<Instances>> GetInstances(ObjectId appId)
        {
            return await _instances.Where(x => x.ApplicationId == appId);
        }

        /// <summary>
        /// Экземпляр по id
        /// </summary>
        public async Task<Instances?> GetInstance(ObjectId id)
        {
            return await _instances.GetById(id);
        }

        /// <summary>
        /// Создание экземпляра
        /// </summary>
        public async Task CreateInstance(Instances instance)
        {
            await _instances.Create(instance);
        }

        /// <summary>
        /// Экземпляры по приложению
        /// </summary>
        public async Task<List<Instances>> GetInstancesByApp(ObjectId appId)
        {
            return await _instances.Where(i => i.ApplicationId == appId);
        }

        /// <summary>
        /// Получить приложения вместе с экземплярами
        /// </summary>
        public async Task<List<ApplicationWithInstances>> GetApplicationsWithInstances()
        {
            var apps = await _applications.GetAll();

            var instances = await _instances.GetAll();

            var grouped = instances
                .GroupBy(i => i.ApplicationId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var result = new List<ApplicationWithInstances>();

            foreach (var app in apps)
            {
                grouped.TryGetValue(app.Id, out var list);

                result.Add(new ApplicationWithInstances
                {
                    Application = app,
                    Instances = list ?? new List<Instances>()
                });
            }

            return result;
        }

        // Метрики

        /// <summary>
        /// Метрики приложения
        /// </summary>
        public async Task<List<Metrics>> GetMetricsByApplication(ObjectId appId)
        {
            return await _metrics.Where(x => x.ApplicationId == appId);
        }

        /// <summary>
        /// Все типы метрик
        /// </summary>
        public async Task<List<MetricTypes>> GetMetricTypes()
        {
            return await _metricTypes.GetAll();
        }

        /// <summary>
        /// Метрики вместе с названиями типов
        /// </summary>
        public async Task<List<(Metrics Metric, MetricTypes? Type)>> GetMetricsWithTypes(ObjectId appId)
        {
            var metrics = await _metrics.Where(x => x.ApplicationId == appId);

            var metricTypes = await _metricTypes.GetAll();

            var result = new List<(Metrics, MetricTypes?)>();

            foreach (var metric in metrics)
            {
                var type = metricTypes
                    .FirstOrDefault(t => t.Id == metric.MetricTypeId);

                result.Add((metric, type));
            }

            return result;
        }

        // Аналитика

        /// <summary>
        /// Количество активных тестов
        /// </summary>
        public async Task<int> GetActiveTestsCount()
        {
            var tests = await _abTests.Where(x => x.Enabled);

            return tests.Count;
        }

        /// <summary>
        /// Общее количество тестов
        /// </summary>
        public async Task<int> GetTestsCount()
        {
            var tests = await _abTests.GetAll();

            return tests.Count;
        }

        /// <summary>
        /// Количество вариантов
        /// </summary>
        public async Task<int> GetVariantsCount()
        {
            var variants = await _variants.GetAll();

            return variants.Count;
        }
    }
}