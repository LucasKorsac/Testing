using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Testing.Interf;

namespace Testing.Pattern
{
    /// <summary>
    /// Паттерн фабрика. Создание репозиториев
    /// </summary>
    public class Factory
    {
        /// <summary>
        /// Создание репозитория с явным именем коллекции
        /// </summary>
        public IMongoRepo<T> Create<T>(string collectionName) where T : class
        {
            return new MongoRepo<T>(collectionName);
        }

        /// <summary>
        /// Создание и логирование (с декоратором)
        /// </summary>
        public IMongoRepo<T> CreateWithLogging<T>(string collectionName) where T : class
        {
            var repo = new MongoRepo<T>(collectionName);
            return new LoggingRepo<T>(repo);
        }
    }
}
