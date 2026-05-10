using Testing.Base;
using Testing.Pattern;
using static Testing.Base.BaseMongo;

namespace WebAppTest.Control
{
    public class ServiceControl
    {
        private readonly Facade _facade;
        private readonly IStrategy<Variants> _strategy;

        public ServiceControl(Facade facade, IStrategy<Variants> strategy)
        {
            _facade = facade;
            _strategy = strategy;
        }

        public async Task<Dictionary<string, string>> Run(string applicationId)
        {
            var result = new Dictionary<string, string>();

            if (string.IsNullOrWhiteSpace(applicationId))
                return result;

            var tests = await _facade.GetTests();

            foreach (var item in tests)
            {
                if (!item.Test.Enabled)
                    continue;

                if (item.Variants == null || item.Variants.Count == 0)
                    continue;

                var fallback = item.Variants.First();
                var selected = _strategy.Choose(item.Variants, fallback);

                var key = item.Test.Name ?? "unknown";
                result[key] = selected.Name;
            }

            return result;
        }
    }
}