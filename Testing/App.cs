using System.Linq;
using Testing.Base;
using Testing.Data;
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

            // 🔥 Инициализация базы (НЕ ТРОГАЕМ)
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

            // Репозитории
            var abTestRepo = _factory.Create<ABTests>();
            var variantRepo = _factory.Create<Variants>();
            var resultRepo = _factory.Create<AbResults>();
            var valuesRepo = _factory.Create<Values>();
            var instanceRepo = _factory.Create<Instances>(); // ← добавили

            // 🔥 Facade (ИСПРАВЛЕНО: теперь 4 параметра)
            var facade = new Facade(
                abTestRepo,
                variantRepo,
                resultRepo,
                instanceRepo
            );

            // Статистика
            var statsBuilder = new StatsBuilder(
                variantRepo,
                resultRepo,
                valuesRepo
            );

            var weightStrategy = new WeightStrategy();
            var adaptation = new Adaptation(statsBuilder, weightStrategy);

            var tests = await facade.GetAllTests();
            var test = tests.FirstOrDefault();

            if (test == null)
            {
                Console.WriteLine("No tests found");
                return;
            }

            var name = test.Name ?? string.Empty;

            IStrategy<Variants> strategy =
                name.Contains("adaptive", StringComparison.OrdinalIgnoreCase)
                    ? new AdaptiveStrategy(adaptation)
                    : new RandomStrategy<Variants>();

            var example = new Example(facade, strategy);
            await example.Init();

            Console.WriteLine("A/B test finished");
        }
    }
}