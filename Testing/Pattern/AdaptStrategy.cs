using MongoDB.Bson;
using Testing.DTO;

namespace Testing.Pattern
{
    /// <summary> Адаптивная стратегия выбора варианта (работает через DTO) </summary>
    public class AdaptStrategy : IStrategy<VariantDto>
    {
        /// <summary> Сервис адаптации распределения </summary>
        private readonly Adaptation _adaptation;

        /// <summary> Генератор случайных чисел </summary>
        private static readonly Random _rnd = new();

        /// <summary> Внедрение зависимости Adaptation </summary>
        public AdaptStrategy(Adaptation adaptation)
        {
            _adaptation = adaptation;
        }

        /// <summary> Выбор варианта </summary>
        /// <param name="items">Список вариантов</param>
        /// <param name="defaultValue">Значение по умолчанию</param>
        /// <param name="instanceId">ID экземпляра приложения (нужен для T-оценки)</param>
        public VariantDto Choose(List<VariantDto> items, VariantDto defaultValue, string? instanceId = null)
        {
            // защита от пустого списка
            if (items == null || items.Count == 0)
                return defaultValue;

            try
            {
                // берем id теста (DTO → string → ObjectId)
                var testIdRaw = items[0].AbTestId;

                if (!ObjectId.TryParse(testIdRaw, out var testId))
                    return GetRandom(items);

                // Проверяем, есть ли instanceId
                if (string.IsNullOrEmpty(instanceId))
                {
                    // Если нет instanceId — используем случайный выбор
                    return GetRandom(items);
                }

                // построение адаптивного пула с передачей instanceId
                var pool = _adaptation
                    .BuildPool(testId, instanceId)  // ← добавили instanceId
                    .GetAwaiter()
                    .GetResult();

                // fallback
                if (pool == null || pool.Count == 0)
                    return GetRandom(items);

                // выбор из пула
                return pool[_rnd.Next(pool.Count)];
            }
            catch (Exception ex)
            {
                ErrorCheck.Handle(ex, "AdaptStrategy");
                return GetRandom(items);
            }
        }

        /// <summary> Случайный выбор варианта </summary>
        private VariantDto GetRandom(List<VariantDto> items)
        {
            return items[_rnd.Next(items.Count)];
        }
    }
}