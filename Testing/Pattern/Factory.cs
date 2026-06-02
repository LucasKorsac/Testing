using MongoDB.Driver;
using Testing.Base;

namespace Testing.Pattern
{
    /// <summary> Фабрика репозиториев базы </summary>
    public interface IMongoFactory
    {
        IMongoRepo<T> Create<T>() where T : class;
    }

    /// <summary> Реализация фабрики MongoDB репозиториев </summary>
    public class MongoFactory : IMongoFactory
    {
        /// <summary> подключение к MongoDB </summary>
        private readonly IMongoDatabase _database;

        /// <summary> логгер для декоратора </summary>
        private readonly RepositoryLogger _logger;

        public MongoFactory(IMongoDatabase database, RepositoryLogger logger)
        {
            _database = database;
            _logger = logger;
        }

        /// <summary> Создание репозитория с подключением декоратора логирования </summary>
        public IMongoRepo<T> Create<T>() where T : class
        {
            // базовый репозиторий
            var repo = new MongoRepo<T>(_database);

            // декоратор логирования
            var loggedRepo = new Decorator<T>(repo, _logger);

            return loggedRepo;
        }
    }
}