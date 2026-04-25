using MongoDB.Driver;
using Testing.Base;

namespace Testing.Pattern
{
    /// <summary>
    /// Фабрика репозиториев базы
    /// </summary>
    public interface IMongoFactory
    {
        /// <summary>
        /// Создаёт репозиторий для указанной сущности
        /// </summary>
        IMongoRepo<T> Create<T>() where T : class;
    }

    /// <summary>
    /// Реализация фабрики MongoDB репозиториев. Слой абстракции над созданием репозиториев
    /// </summary>
    public class MongoFactory : IMongoFactory
    {
        /// <summary>
        /// Подключение к базе MongoDB
        /// </summary>
        private readonly IMongoDatabase _database;

        /// <summary>
        /// Конструктор фабрики
        /// </summary>
        public MongoFactory(IMongoDatabase database)
        {
            _database = database;
        }

        /// <summary>
        /// Создание репозитория с подключением декоратора логирования
        /// </summary>
        public IMongoRepo<T> Create<T>() where T : class
        {
            // Создание базового репозитория
            var repo = new MongoRepo<T>(_database);

            // Оборот в декоратор
            var loggedRepo = new LogMongoRepo<T>(repo);

            // Возврат репозитория
            return loggedRepo;
        }
    }
}