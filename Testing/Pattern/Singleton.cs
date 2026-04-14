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
    //Одиночка
    //Единственный клиент Mongo
    public class Singleton
    {
        private static MongoClient _client;
        private static IMongoDatabase _database;

        static MongoContext()
        {
            _client = new MongoClient("mongodb://localhost:27017");
            _database = _client.GetDatabase("test2");
        }

        public static IMongoCollection<T> GetCollection<T>(string name)
        {
            return _database.GetCollection<T>(name);
        }
    }
}
