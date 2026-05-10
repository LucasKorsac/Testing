using MongoDB.Driver;
using System;
using System.Threading.Tasks;
using Testing.Pattern;
using Microsoft.Extensions.DependencyInjection;

namespace Testing
{
    internal class Program
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

            // ✔ ВАЖНО: регистрируем КЛАСС, а не интерфейс
            services.AddSingleton<RepositoryLogger>();

            // (если хочешь — можешь оставить и интерфейс тоже)
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