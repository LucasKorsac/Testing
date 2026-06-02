using System;
using System.Linq;
using System.Threading.Tasks;
using Testing.Base;
using Testing.DTO;
using Testing.Pattern;
using static Testing.Base.BaseMongo;

namespace ABProjectTests.BaseTest
{
    public class App
    {
        private readonly IMongoFactory _factory;

        public App(IMongoFactory factory)
        {
            _factory = factory;
        }

        public async Task Init()
        {
            Console.WriteLine("Starting app...");

            // синтетические данные
            await SinteticData.Init(
                _factory.Create<Roles>(),
                _factory.Create<Developers>(),
                _factory.Create<DevelopRoleApplic>(),
                _factory.Create<Applications>(),
                _factory.Create<MetricTypes>(),
                _factory.Create<Metrics>(),
                _factory.Create<Instances>(),
                _factory.Create<EquipParam>(),
                _factory.Create<Values>(),
                _factory.Create<ABTests>(),
                _factory.Create<Variants>(),
                _factory.Create<AbResults>()
            );

            // facade
            var facade = new Facade(
                _factory.Create<ABTests>(),
                _factory.Create<Variants>(),
                _factory.Create<AbResults>(),
                _factory.Create<Instances>(),
                _factory.Create<Applications>(),
                _factory.Create<DevelopRoleApplic>(),
                _factory.Create<Metrics>(),
                _factory.Create<MetricTypes>(),
                _factory.Create<Roles>(),
                _factory.Create<Developers>(),
                _factory.Create<EquipParam>(),
                _factory.Create<Values>()
            );

            // получаем тесты
            var tests = await facade.GetTests();

            foreach (var item in tests)
            {
                var variants =
                    item.Variants ?? new List<VariantDto>();

                if (variants.Count == 0)
                    continue;

                // random стратегия
                var strategy =
                    new RandomStrategy<VariantDto>();

                var selected =
                    strategy.Choose(
                        variants,
                        variants[0]);

                Console.WriteLine(
                    $"Test: {item.Test.Name} → Variant: {selected.Name}");
            }

            Console.WriteLine("A/B test finished");
        }
    }
}