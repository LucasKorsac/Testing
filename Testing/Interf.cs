using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace Testing.Base
{
   
    public interface IMongoRepo<T> where T : class
    {
        // Чтение данных

        /// <summary>
        /// LINQ-доступ к коллекции MongoDB.Позволяет выполнять сложные запросы через LINQ
        /// </summary>
        IQueryable<T> Query { get; }

        /// <summary>
        /// Получение документа по ObjectId
        /// </summary>
        Task<T?> GetById(ObjectId id, CancellationToken ct = default);

        /// <summary>
        /// Получение документа по строковому ID
        /// </summary>
        Task<T?> GetById(string id, CancellationToken ct = default);

        /// <summary>
        /// Получение первого документа, подходящего под условие
        /// </summary>
        Task<T?> FirstOrDefault(Expression<Func<T, bool>> filter, CancellationToken ct = default);

        /// <summary>
        /// Получение списка документов по условию
        /// </summary>
        Task<List<T>> Where(Expression<Func<T, bool>> filter, CancellationToken ct = default);

        /// <summary>
        /// Получение всех документов коллекции без фильтрации
        /// </summary>
        Task<List<T>> GetAll(CancellationToken ct = default);

        // Создание данных

        /// <summary>
        /// Создание одного документа в коллекции
        /// </summary>
        Task Create(T entity, CancellationToken ct = default);

        /// <summary>
        /// Массовое создание документов
        /// </summary>
        Task CreateMany(IEnumerable<T> entities, CancellationToken ct = default);

        // Обновление данных

        /// <summary>
        /// Полная замена документа по ObjectId. Старый документ полностью перезаписывается новым
        /// </summary>
        Task Update(ObjectId id, T entity, CancellationToken ct = default);

        /// <summary>
        /// Полная замена документа по строковому ID
        /// </summary>
        Task Update(string id, T entity, CancellationToken ct = default);

        /// <summary>
        /// Частичное обновление документа. Позволяет обновлять только нужные поля без перезаписи всего объекта
        /// </summary>
        Task<bool> Update(ObjectId id, UpdateDefinition<T> update, CancellationToken ct = default);

        /// <summary>
        /// Полная замена документа
        /// </summary>
        Task<bool> Replace(ObjectId id, T entity, CancellationToken ct = default);

        // Удаление

        /// <summary>
        /// Удаление документа по ObjectId.
        /// </summary>
        Task Delete(ObjectId id, CancellationToken ct = default);

        /// <summary>
        /// Удаление документа по строковому ID
        /// </summary>
        Task Delete(string id, CancellationToken ct = default);

        /// <summary>
        /// Массовое удаление документов по условию.Возвращает количество удалённых записей
        /// </summary>
        Task<long> DeleteMany(Expression<Func<T, bool>> filter, CancellationToken ct = default);

        /// <summary>
        /// Полная очистка коллекции/удаление всех документов
        /// </summary>
        Task DeleteAll(CancellationToken ct = default);
         
        // Дополнительно

        /// <summary>
        /// Проверка существования документа по условию
        /// </summary>
        Task<bool> Exists(Expression<Func<T, bool>> filter, CancellationToken ct = default);

        /// <summary>
        /// Подсчёт количества документов в коллекции
        /// </summary>
        Task<long> Count(Expression<Func<T, bool>>? filter = null, CancellationToken ct = default);
    }
}