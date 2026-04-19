using MongoDB.Bson;
using static Testing.Base.BaseMongo;

namespace Testing.Pattern
{
    /// <summary>
    /// Паттерн стратегия. Выбор варианта A/B теста
    /// </summary>
    public interface IStrategy<T>
    {
        T Choose(List<T> items, T defaultValue);
    }

    /// <summary>
    /// Случайный выбор варианта
    /// </summary>
    public class RandomStrategy<T> : IStrategy<T>
    {
        private static readonly Random _rnd = new();

        /// <summary>
        /// Выбор случайного элемента из списка, если список пустой или null — возвращает значение по умолчанию
        /// </summary>
        public T Choose(List<T> items, T defaultValue)
        {
            if (items == null || items.Count == 0)
                return defaultValue;

            return items[_rnd.Next(items.Count)];
        }
    }
}