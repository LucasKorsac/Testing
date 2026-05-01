using MongoDB.Driver;
using System;
using System.Threading.Tasks;
using Testing.Pattern;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.IO;

namespace Testing
{
    internal class Program
    {
        static async Task Main()
        {
            //File.WriteAllText()

            // DI контейнер
            var services = new ServiceCollection();
            // Mongo
            services.AddSingleton<IMongoDatabase>(sp => {var client = new MongoClient("mongodb://localhost:27017"); 
                return client.GetDatabase("ABTesting");});

            services.AddScoped<IMongoFactory, MongoFactory>();
            services.AddScoped<App>();

            var provider = services.BuildServiceProvider();

            // Создание App через DI
            var app = provider.GetRequiredService<App>();

            await app.Init();

            foreach (var kvp in Controller.I.CurrentTests)
            {
                Console.WriteLine($"{kvp.Key} = {kvp.Value}");
            }

            Console.ReadKey();
        }
    }
}