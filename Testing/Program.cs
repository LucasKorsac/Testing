using MongoDB.Driver;
using Testing;
using Testing.Pattern;

namespace TestingApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Application started");

            try
            {
                // подключение MongoDB

                var client =
                    new MongoClient(
                        "mongodb://localhost:27017");

                var database =
                    client.GetDatabase("ABTesting");

                // логгер

                var logger =
                    new RepositoryLogger();

                // фабрика

                IMongoFactory factory =
                    new MongoFactory(
                        database,
                        logger);

                // запуск приложения

                var app =
                    new App(factory);

                await app.Init();

                Console.WriteLine("Application finished");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fatal error:");

                Console.WriteLine(ex.Message);

                Console.WriteLine(ex.StackTrace);
            }

            Console.WriteLine("Press any key...");

            Console.ReadKey();
        }
    }
}