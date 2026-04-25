using MongoDB.Bson;
using System;
using System.Threading.Tasks;
using Testing.Base;
using Testing.Data;
using Testing.Pattern;
using static System.Net.Mime.MediaTypeNames;
using static Testing.Base.BaseMongo;
using static Testing.Pattern.RandStrategy;

namespace Testing
{
    internal class App
    {
        private readonly IMongoFactory _factory;

        //Прохождение через DI
        public App(IMongoFactory factory)
        {
            _factory = factory;
        }

        public async Task Init()
        {
            Console.WriteLine("Starting app...");

            await SinteticData.Init(_factory.Create<Companies>(),_factory.Create<Roles>(),_factory.Create<Developers>(),
            _factory.Create<Applications>(), _factory.Create<MetricTypes>(), _factory.Create<Metrics>(), _factory.Create<Instances>(),
            _factory.Create<Attributes>(), _factory.Create<Values>(), _factory.Create<ABDescriptions>(), _factory.Create<ABTests>(),
            _factory.Create<Variants>(), _factory.Create<AbResults>());

            var abTestRepo = _factory.Create<ABTests>();
            var variantRepo = _factory.Create<Variants>();
            var resultRepo = _factory.Create<AbResults>();
            var valuesRepo = _factory.Create<Values>();

            var facade = new Facade(abTestRepo, variantRepo);
            var adaptation = new Adaptation(variantRepo, resultRepo, valuesRepo);

            // Получение теста
            var tests = await facade.GetAllTests();
            var test = tests.FirstOrDefault();

            if (test == null)
            {
                Console.WriteLine("No tests found");
                return;
            }

            // Выбор стратегии
            IStrategy<Variants> strategy;

            if (test.Name.Contains("adaptive"))
            {
                strategy = new AdaptiveStrategy(adaptation);
            }
            else
            {
                strategy = new RandomStrategy<Variants>();
            }

            var example = new Example(facade, strategy, variantRepo);

            await example.Init();

            foreach (var item in example.AB)
                Console.WriteLine($"{item.Key} -> {item.Value}");

            Console.WriteLine("A/B test finished");
        }
    }
}