using MongoDB.Bson;
using Testing.Base;
using static Testing.Base.BaseMongo;

namespace Testing.Pattern
{
    /// <summary> Фасад A/B тестов </summary>
    public class Facade
    {
        private readonly IMongoRepo<ABTests> _abTests;
        private readonly IMongoRepo<Variants> _variants;
        private readonly IMongoRepo<AbResults> _results;
        private readonly IMongoRepo<Instances> _instances;

        public Facade(IMongoRepo<ABTests> abTests, IMongoRepo<Variants> variants, IMongoRepo<AbResults> results, IMongoRepo<Instances> instances)
        {
            _abTests = abTests;
            _variants = variants;
            _results = results;
            _instances = instances;
        }

        /// <summary> Все тесты </summary>
        public Task<List<ABTests>> GetAllTests()
        {
            return _abTests.GetAll();
        }

        /// <summary> Тесты + варианты </summary>
        public async Task<List<TestWithVariants>> GetTests()
        {
            var tests = await _abTests.GetAll();
            var variants = await _variants.GetAll();

            var grouped = variants
                .GroupBy(v => v.AbTestId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var result = new List<TestWithVariants>();

            foreach (var t in tests)
            {
                grouped.TryGetValue(t.Id, out var list);

                result.Add(new TestWithVariants
                {
                    Test = t,
                    Variants = list ?? new List<Variants>()
                });
            }

            return result;
        }

        /// <summary> Результаты по тесту </summary>
        public async Task<List<AbResults>> GetResults(ObjectId testId)
        {
            var variants = await _variants.Where(v => v.AbTestId == testId);
            var ids = variants.Select(v => v.Id).ToList();

            return await _results.Where(r => ids.Contains(r.VariantId));
        }

        public async Task<ABTests?> GetById(ObjectId id)
        {
            var tests = await _abTests.GetAll();
            return tests.FirstOrDefault(t => t.Id == id);
        }
        public async Task UpdateTest(ABTests test)
        {
            await _abTests.Update(test.Id, test);
        }

        public async Task DeleteTest(ObjectId id)
        {
            await _abTests.Delete(id);
        }
    }
}