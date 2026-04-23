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
        public async Task Init()
        {
            Console.WriteLine("Starting app...");

            await SinteticData.Init();

            var factory = new MongoFactory();

            var abTestRepo = factory.Create<ABTests>("AbTest");
            var variantRepo = factory.Create<Variants>("Variant");
            var resultRepo = factory.Create<Results>("Results");
            var valuesRepo = factory.Create<Values>("Values");

            var facade = new Facade(abTestRepo, variantRepo);

            var adaptation = new Adaptation(variantRepo, resultRepo, valuesRepo);

            // Получение теста
            var tests = await facade.GetAllTests();
            var test = tests.Count > 0 ? tests[0] : null;

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