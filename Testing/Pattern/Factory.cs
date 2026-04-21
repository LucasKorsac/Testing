using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testing.Base;

namespace Testing.Pattern
{
    /// <summary>
    /// Фабрика репозиториев MongoDB. Создаёт репозитории и скрывает логику их инициализации
    /// </summary>
    interface Factory
    {
        IMongoRepo<T> Create<T>(string collectionName) where T : class;
    }
    /// <summary>
    /// Реализация фабрики репозиториев
    /// </summary>
    public class MongoFactory : Factory
    {
        /// <summary>
        /// Создание репозитория с автоматическим подключением декоратора логирования
        /// </summary>
        public IMongoRepo<T> Create<T>(string collectionName) where T : class
        {
            // Базовый репозиторий
            var repo = new MongoRepo<T>(collectionName);

            // Обертка в декоратор
            return new LogMongoRepo<T>(repo);
        }
    }
}
