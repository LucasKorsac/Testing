using MongoDB.Bson;
using Testing.Base;
using static Testing.Base.BaseMongo;

namespace Testing.Pattern
{
    /// <summary> DTO: тест и его варианты </summary>
    public class TestWithVariants
    {
        public ABTests Test { get; set; }
        public List<Variants> Variants { get; set; } = new();
    }

    /// <summary> Фасад для работы с A/B тестами </summary>
    public class Facade
    {
        private readonly IMongoRepo<ABTests> _abTests;
        private readonly IMongoRepo<Variants> _variants;

        public Facade(IMongoRepo<ABTests> abTests, IMongoRepo<Variants> variants)
        {
            _abTests = abTests;
            _variants = variants;
        }

        /// <summary> Получение всех тестов </summary>
        public Task<List<ABTests>> GetAllTests()
        {
            return _abTests.GetAll();
        }

        /// <summary> Получить тесты вместе с вариантами </summary>
        public async Task<List<TestWithVariants>> GetTests()
        {
            var tests = await _abTests.GetAll();
            var variants = await _variants.GetAll();

            var variantsByTestId = variants
                .GroupBy(v => v.AbTestId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var result = new List<TestWithVariants>(tests.Count);

            foreach (var test in tests)
            {
                variantsByTestId.TryGetValue(test.Id, out var testVariants);

                result.Add(new TestWithVariants
                {
                    Test = test,
                    Variants = testVariants ?? new List<Variants>()
                });
            }

            return result;
        }
    }
}