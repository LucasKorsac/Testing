using MongoDB.Bson;
using Testing.Base;
using static Testing.Base.BaseMongo;

namespace Testing.Pattern
{
    //Паттерн фасад
    public class Facade
    {
        private readonly IMongoRepo<ABTests> _abTests;
        private readonly IMongoRepo<Variants> _variants;

        public Facade(IMongoRepo<ABTests> abTests, IMongoRepo<Variants> variants)
        {
            _abTests = abTests;
            _variants = variants;
        }

        /// <summary>
        /// Получение всех A/B тестов
        /// </summary>
        public async Task<List<ABTests>> GetAllTests() => await _abTests.GetAll();

        /// <summary>
        /// Получение вариантов для конкретного теста
        /// </summary>
        public async Task<List<Variants>> GetVariants(ObjectId testId) => await _variants.Where(x => x.AbTestId == testId);

        /// <summary>
        /// Получение теста и варианты сразу
        /// </summary>
        public async Task<Dictionary<ABTests, List<Variants>>> GetTestsWithVariants()
        {
            var result = new Dictionary<ABTests, List<Variants>>();

            var tests = await _abTests.GetAll();

            foreach (var test in tests)
            {
                var variants = await _variants.Where(x => x.AbTestId == test.Id);
                result[test] = variants;
            }

            return result;
        }
    }
}