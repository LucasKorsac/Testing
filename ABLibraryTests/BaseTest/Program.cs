using ABProjectTests.BaseTest;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;
using Testing.Pattern;

namespace ABProjectTests.BaseTest 
{ 
    public class Program
    {
        static async Task Main()
        {
            var services = new ServiceCollection();

            // MongoDB подключение
            services.AddSingleton<IMongoDatabase>(sp =>
            {
                var client = new MongoClient("mongodb://localhost:27017");
                return client.GetDatabase("ABTesting");
            });

            services.AddSingleton<RepositoryLogger>();
            services.AddSingleton<IRepositoryLogger, RepositoryLogger>();

            // Фабрика репозиториев
            services.AddSingleton<IMongoFactory, MongoFactory>();

            // Приложение
            services.AddSingleton<App>();

            var provider = services.BuildServiceProvider();

            var app = provider.GetRequiredService<App>();

            await app.Init();

            Console.ReadKey();
        }
    }
}