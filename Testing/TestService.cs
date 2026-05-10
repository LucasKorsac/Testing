using MongoDB.Bson;
using Testing.Pattern;
using static Testing.Base.BaseMongo;

namespace Testing
{
    /// <summary> Сервис выполнения A/B тестов </summary>
    public class TestService
    {
        private readonly Facade _facade;
        private readonly IStrategy<Variants> _strategy;

        public TestService(Facade facade, IStrategy<Variants> strategy)
        {
            _facade = facade;
            _strategy = strategy;
        }

        /// <summary> Получение результатов A/B тестов для экземпляра приложения </summary>
        public async Task<Dictionary<string, string>> GetAB(string instanceId)
        {
            if (!ObjectId.TryParse(instanceId, out _))
                throw new ArgumentException("Invalid instance id");

            var result = new Dictionary<string, string>();

            var tests = await _facade.GetTests();

            foreach (var item in tests)
            {
                var variants = item.Variants ?? new List<Variants>();

                if (!item.Test.Enabled || variants.Count == 0)
                    continue;

                var fallback = variants[0];

                var selected = _strategy.Choose(variants, fallback);

                result[item.Test.Name] = selected.Name;
            }

            return result;
        }
    }
}