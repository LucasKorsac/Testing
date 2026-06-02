using MongoDB.Bson;
using Testing.DTO;
using Testing.Pattern;

namespace Testing
{
    /// <summary>
    /// Сервис выполнения A/B тестов
    /// </summary>
    public class TestService
    {
        private readonly Facade _facade;

        private readonly IStrategy<VariantDto> _strategy;

        public TestService(
            Facade facade,
            IStrategy<VariantDto> strategy)
        {
            _facade = facade;
            _strategy = strategy;
        }

        /// <summary>
        /// Получение результатов A/B тестов
        /// </summary>
        public async Task<Dictionary<string, string>>
            GetAB(string instanceId)
        {
            if (!ObjectId.TryParse(instanceId, out _))
                throw new ArgumentException("Invalid instance id");

            var result = new Dictionary<string, string>();

            var tests = await _facade.GetTests();

            foreach (var item in tests)
            {
                var variants =
                    item.Variants ?? new List<VariantDto>();

                if (!item.Test.Enabled || variants.Count == 0)
                    continue;

                var fallback = variants[0];

                var selected =
                    _strategy.Choose(variants, fallback);

                result[item.Test.Name] = selected.Name;
            }

            return result;
        }
    }
}