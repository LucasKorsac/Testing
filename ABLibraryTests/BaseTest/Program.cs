using ABProjectTests.BaseTest;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Testing.Pattern;

namespace ABProjectTests.BaseTest
{
    public class Program
    {
        static async Task Main()
        {
            Console.WriteLine("Генератор синтетических данных");
            Console.WriteLine();

            var stopwatch = Stopwatch.StartNew();

            var services = new ServiceCollection();

            // MongoDB подключение
            services.AddSingleton<IMongoClient>(_ => new MongoClient("mongodb://localhost:27017"));
            services.AddSingleton<IMongoDatabase>(sp =>
            {
                var client = sp.GetRequiredService<IMongoClient>();
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

            stopwatch.Stop();

            Console.WriteLine();
            Console.WriteLine($"Время выполнения: {stopwatch.Elapsed.TotalSeconds:F2} секунд ===");
            Console.WriteLine();
            Console.WriteLine("Нажмите любую клавишу для выхода...");
            Console.ReadKey();
        }
    }
}