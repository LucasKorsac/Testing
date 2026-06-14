using Testing.DTO;
using Testing.Pattern;

namespace WebAppTest.Control
{
    /// <summary> Сервис запуска A/B тестов </summary>
    public class ServiceControl
    {
        private readonly Facade _facade;
        private readonly IStrategy<VariantDto> _strategy;

        public ServiceControl(Facade facade, IStrategy<VariantDto> strategy)
        {
            _facade = facade;
            _strategy = strategy;
        }

        /// <summary> Запуск тестов для приложения </summary>
        //public async Task<Dictionary<string, string>> Run(string applicationId, string? instanceId = null)
//        {
//            var result = new Dictionary<string, string>();

        //            if (string.IsNullOrWhiteSpace(applicationId))
        //                return result;

        //            var tests = await _facade.GetTests();

        //            foreach (var item in tests)
        //            {
        //                if (!item.Test.Enabled)
        //                    continue;

        //                var variants = item.Variants ?? new List<VariantDto>();

        //                if (variants.Count == 0)
        //                    continue;

        //                var fallback = variants.First();

        //                // Передаём instanceId в стратегию
        //                var selected = _strategy.Choose(variants, fallback, instanceId);

        //                result[item.Test.Name] = selected.Name;
        //            }

        //            return result;
        //        }
        //    }
        //}

public async Task<Dictionary<string, string>> Run(string applicationId, string? instanceId = null)
        {
            var result = new Dictionary<string, string>();

            if (string.IsNullOrWhiteSpace(applicationId))
                return result;

            var tests = await _facade.GetTests();

            foreach (var item in tests)
            {
                if (!item.Test.Enabled)
                    continue;

                var variants = item.Variants ?? new List<VariantDto>();

                if (variants.Count == 0)
                    continue;

                var fallback = variants.First();

                // Передаём instanceId в стратегию
                var selected = _strategy.Choose(variants, fallback, instanceId);

                result[item.Test.Name] = selected.Name;
            }

            return result;
        }
    }
}