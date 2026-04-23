using MongoDB.Bson;
using static Testing.Base.BaseMongo;

namespace Testing.Pattern
{
    /// <summary>
    /// Адаптивная стратегия выбора варианта.
    /// </summary>
    public class AdaptiveStrategy : IStrategy<Variants>
    {
        /// <summary>
        /// Адаптация
        /// </summary>
        private readonly Adaptation _adaptation;

        /// <summary>
        /// Случайные числа для выбора варианта
        /// </summary>
        private static readonly Random _rnd = new();

        /// <summary>
        /// Конструктор с внедрением зависимости Adaptation
        /// </summary>
        public AdaptiveStrategy(Adaptation adaptation)
        {
            _adaptation = adaptation;
        }

        /// <summary>
        /// Выбор варианта
        /// <returns>Выбранный вариант</returns>
        public Variants Choose(List<Variants> items, Variants defaultValue)
        {
            // null и пустое значение
            if (items == null || items.Count == 0) return defaultValue;

            try
            {
                // Все варианты принадлежат одному тесту
                var testId = items[0].AbTestId;

                // Синхронное ожидание метода
                var pool = _adaptation
                    .BuildPool(testId)
                    .GetAwaiter()
                    .GetResult();

                // Если адаптация не дала результата, то выбор случаен
                if (pool == null || pool.Count == 0) return items[_rnd.Next(items.Count)];

                // Выбор из взвешенного пула
                return pool[_rnd.Next(pool.Count)];
            }
            catch (Exception ex)
            {
                // Логирование ошибки
                ErrorCheck.Handle(ex, "AdaptiveStrategy");

                // Случайный выбор
                return items[_rnd.Next(items.Count)];
            }
        }
    }

    /// <summary>
    /// Интерфейс стратегии выбора
    /// </summary>
    public interface IStrategy<T>
    {
        /// <summary>
        /// Выбор элемента из списка
        /// </summary>
        T Choose(List<T> items, T defaultValue);
    }
}