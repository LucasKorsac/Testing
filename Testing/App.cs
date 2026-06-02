using System.Linq;
using Testing.Base;
using Testing.DTO;
using Testing.Pattern;
using static Testing.Base.BaseMongo;

namespace Testing
{
    internal class App
    {
        private readonly IMongoFactory _factory;

        public App(IMongoFactory factory)
        {
            _factory = factory;
        }

        public async Task Init()
        {
            Console.WriteLine("Starting app...");

             // репозитории

            var abTestRepo = _factory.Create<ABTests>();

            var variantRepo = _factory.Create<Variants>();

            var resultRepo = _factory.Create<AbResults>();

            var instanceRepo = _factory.Create<Instances>();

            var applicationRepo = _factory.Create<Applications>();

            var devRoleAppRepo = _factory.Create<DevelopRoleApplic>();

            var metricsRepo = _factory.Create<Metrics>();

            var metricTypesRepo = _factory.Create<MetricTypes>();

            var roleRepo = _factory.Create<Roles>();

            var developerRepo = _factory.Create<Developers>();

            var equipParamRepo = _factory.Create<EquipParam>();

            var valueRepo = _factory.Create<Values>();

            // facade

            var facade = new Facade(abTestRepo, variantRepo, resultRepo, instanceRepo, applicationRepo,
                devRoleAppRepo, metricsRepo, metricTypesRepo, roleRepo, developerRepo, equipParamRepo,
                valueRepo);

            // статистика

            var statsBuilder = new StatsBuilder(
                variantRepo,
                resultRepo,
                valueRepo
            );

            var weightStrategy =
                new WeightStrategy();

            var adaptation =
                new Adaptation(
                    statsBuilder,
                    weightStrategy);

            // тесты

            var tests =
                await facade.GetAllTests();

            var test =
                tests.FirstOrDefault();

            if (test == null)
            {
                Console.WriteLine("No tests found");
                return;
            }

            var name =
                test.Name ?? string.Empty;

            IStrategy<VariantDto> strategy =
                name.Contains(
                    "adaptive",
                    StringComparison.OrdinalIgnoreCase)
                    ? new AdaptStrategy(adaptation)
                    : new RandomStrategy<VariantDto>();

            var example =
                new Example(facade, strategy);

            await example.Init();

            Console.WriteLine("A/B test finished");
        }
    }
}