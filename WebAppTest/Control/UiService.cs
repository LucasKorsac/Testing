using MongoDB.Bson;
using Testing.Base;
using Testing.Pattern;
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
            var tests = await _facade.GetAllTests();
            return tests.FirstOrDefault(t => t.Id.ToString() == id);
        }
        public async Task<int> GetTotalVariants()
        {
            var tests = await _facade.GetTests();
            return tests.Sum(t => t.Variants?.Count ?? 0);
        }
        public async Task StopTestAsync(string id)
        {
            var test = await _facade.GetById(ObjectId.Parse(id));
            if (test == null) return;

            test.Enabled = false;
            await _facade.UpdateTest(test);
        }

        public async Task DeleteTestAsync(string id)
        {
            await _facade.DeleteTest(ObjectId.Parse(id));
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
            var test = await _facade.GetById(ObjectId.Parse(id));

            if (test == null)
                return;

            test.Enabled = true;

            await _facade.UpdateTest(test);
        }
    }
}