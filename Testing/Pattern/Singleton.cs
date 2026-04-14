using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Testing.Pattern
{
    /// <summary>
    /// Паттерн одиночка. Один клиент на всё приложение
    /// </summary>
    public class Singleton
    {
        private static readonly Lazy<Singleton> _instance =
            new(() => new Singleton());

        private readonly MongoClient _client;
        private readonly IMongoDatabase _database;

        /// <summary>
        /// Приватный конструктор, во избежание создания извне
        /// </summary>
        private Singleton()
        {
            _client = new MongoClient("mongodb://localhost:27017");
            _database = _client.GetDatabase("Test2");
        }

        /// <summary>
        /// Точка доступа
        /// </summary>
        public static Singleton Instance => _instance.Value;

        /// <summary>
        /// Получение коллекции коллекцию
        /// </summary>
        public IMongoCollection<T> GetCollection<T>(string name)
        {
            return _database.GetCollection<T>(name);
        }
    }
}
