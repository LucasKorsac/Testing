using static Testing.Base.BaseMongo;

namespace Testing.Pattern
{
    /// <summary> Адаптивная стратегия выбора варианта </summary>
    public class AdaptiveStrategy : IStrategy<Variants>
    {
        /// <summary> Сервис адаптации распределения </summary>
        private readonly Adaptation _adaptation;

        /// <summary> Генератор случайных чисел </summary>
        private static readonly Random _rnd = new();

        /// <summary> Внедрение зависимости Adaptation </summary>
        public AdaptiveStrategy(Adaptation adaptation)
        {
            _adaptation = adaptation;
        }

        /// <summary> Выбор варианта </summary>
        public Variants Choose(List<Variants> items, Variants defaultValue)
        {
            // если список пуст
            if (items == null || items.Count == 0)
                return defaultValue;

            try
            {
                // все варианты относятся к одному тесту
                var testId = items[0].AbTestId;

                // построение адаптивного пула
                var pool = _adaptation
                    .BuildPool(testId)
                    .GetAwaiter()
                    .GetResult();

                // fallback → случайный выбор
                if (pool == null || pool.Count == 0)
                    return GetRandom(items);

                // выбор из адаптивного пула
                return pool[_rnd.Next(pool.Count)];
            }
            catch (Exception ex)
            {
                // обработка ошибки
                ErrorCheck.Handle(ex, "AdaptiveStrategy");

                // fallback
                return GetRandom(items);
            }
        }

        /// <summary> Случайный выбор варианта </summary>
        private Variants GetRandom(List<Variants> items)
        {
            return items[_rnd.Next(items.Count)];
        }
    }
}