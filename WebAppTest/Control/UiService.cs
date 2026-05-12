using MongoDB.Bson;
//using Testing.Base;
using Testing.Pattern;
using WebAppTest.Controllers;
using static System.Net.Mime.MediaTypeNames;
using static Testing.Base.BaseMongo;

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

        public Task<List<ApplicationWithInstances>> GetApplicationWithInstanceAsync()
            => _facade.GetApplicationsWithInstances();

        public Task<Dictionary<string, string>> GetActiveTestsAsync(string appId)
            => _service.Run(appId);

        public Task<List<ABTests>> GetTestsAsync()
            => _facade.GetAllTests();

        public async Task<List<Variants>> GetVariantsAsync()
        {
            var tests = await _facade.GetTests();
            return tests.SelectMany(t => t.Variants).ToList();
        }

        public async Task<List<TestWithVariants>> GetTestsWithVariantsAsync()
        {
            return await _facade.GetTests();
        }
        public async Task<ABTests?> GetTestByIdAsync(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId))
                return null;

            return await _facade.GetById(objectId);
        }
        public async Task<int> GetTotalVariants()
        {
            var tests = await _facade.GetTests();
            return tests.Sum(t => t.Variants?.Count ?? 0);
        }
        public async Task StopTestAsync(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId))
                return;

            await _facade.StopTest(objectId);
        }

        public async Task DeleteTestAsync(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId))
                return;

            await _facade.DeleteTest(objectId);
        }

        public async Task UpdateTestAsync(string id, string name, string description)
        {
            var test = await _facade.GetById(ObjectId.Parse(id));
            if (test == null) return;

            test.Name = name;
            test.Description = description;

            await _facade.UpdateTest(test);
        }
        public async Task<List<AbResults>> GetResultsAsync(string testId)
        {
            var objectId = ObjectId.Parse(testId);

            return await _facade.GetResults(objectId);
        }
        public async Task ResumeTestAsync(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId))
                return;

            await _facade.ResumeTest(objectId);
        }
        public async Task<List<Applications>> GetApplicationsAsync()
        {
            return await _facade.GetApplications();
        }
        public async Task<Applications?> GetApplicationAsync(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId))
                return null;

            return await _facade.GetApplication(objectId);
        }
        public async Task<List<Instances>> GetInstancesAsync(string appId)
        {
            return await _facade.GetInstancesByApp(ObjectId.Parse(appId));
        }

        public async Task<List<AbResults>> GetResultsByInstanceAsync(string instanceId)
        {
            return await _facade.GetResultsByInstance(ObjectId.Parse(instanceId));
        }
        public async Task<Instances?> GetInstanceAsync(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId))
                return null;

            return await _facade.GetInstance(objectId);
        }

        public async Task<List<Variants>> GetAllVariantsAsync()
        {
            return await _facade.GetAllVariants();
        }

        public async Task<List<ABTests>> GetAllTestsAsync()
        {
            return await _facade.GetAllTests();
        }

        public async Task<List<Metrics>>
    GetMetricsByApplicationAsync(string appId)
        {
            if (!ObjectId.TryParse(appId, out var objectId))
                return new List<Metrics>();

            return await _facade.GetMetricsByApplication(objectId);
        }

        public async Task<List<MetricTypes>>
            GetMetricTypesAsync()
        {
            return await _facade.GetMetricTypes();
        }

        public async Task<List<(Metrics Metric, MetricTypes? Type)>>
            GetMetricsWithTypesAsync(string appId)
        {
            if (!ObjectId.TryParse(appId, out var objectId))
            {
                return new List<(Metrics, MetricTypes?)>();
            }

            return await _facade
                .GetMetricsWithTypes(objectId);
        }

        public Task<List<ApplicationWithInstances>> GetApplicationsWithInstancesAsync() => _facade.GetApplicationsWithInstances();
    }
}