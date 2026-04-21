using MongoDB.Bson;
using System;
using System.Threading.Tasks;
using Testing.Base;
using Testing.Data;
using Testing.Pattern;
using static Testing.Base.BaseMongo;

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

            var facade = new Facade(abTestRepo, variantRepo);

            //IStrategy<Variants> strategy = new RandomStrategy<Variants>();

            //var example = new Example(facade, strategy, variantRepo);

            //await example.Init();

            //foreach (var item in example.AB)
            //    Console.WriteLine($"{item.Key} -> {item.Value}");

            //Console.WriteLine("A/B test finished");
        }
    }
}