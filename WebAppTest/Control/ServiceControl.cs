using MongoDB.Bson;
using Testing;
using Testing.Base;
using Testing.Pattern;
using static Testing.Base.BaseMongo;

namespace WebAppTest.Control
{
    /// <summary>
    /// Сервис бизнес-логики
    /// </summary>
    public class ServiceControl
    {
        /// <summary>
        /// Фасад для получения данных
        /// </summary>
        private readonly Facade _facade;

        /// <summary>
        /// Стратегия выбора варианта
        /// </summary>
        private readonly IStrategy<Variants> _strategy;

        private readonly IMongoRepo<AbEvent> _events;

        /// <summary>
        /// Репозиторий вариантов
        /// </summary>
        //private readonly IMongoRepo<Variants> _variantRepo;

        /// <summary>
        /// Конструктор с внедрением зависимостей
        /// </summary>
        public ServiceControl(Facade facade, IStrategy<Variants> strategy, IMongoRepo<Variants> variantRepo, IMongoRepo<AbEvent> events)
        {
            _facade = facade;
            _strategy = strategy;
            _events = events;
        }

        /// <summary>
        /// Основной метод запуска A/B тестирования. Возврат выбранных вариантов для каждого теста
        /// </summary>
        public async Task<Dictionary<string, string>> Run(ObjectId applicationId)
        {
            // Результат: TestName -> VariantName
            var result = new Dictionary<string, string>();

            // Получение тестов и их вариантов
            var data = await _facade.GetTestsWithVariants();

            // Проход по каждому тесту
            foreach (var pair in data)
            {
                var test = pair.Key;           // A/B тест
                var variants = pair.Value;     // список вариантов

                // Если вариантов нет, то пропуск теста
                if (variants == null || variants.Count == 0)
                    continue;

                // Выбор варианта через стратегию
                //var selected = _strategy.Choose(variants, variants[0]);

                var fallback = variants.FirstOrDefault();
                if (fallback == null) continue;

                var selected = _strategy.Choose(variants, fallback);

                // Сохраняем результат
                result[test.Name] = selected.Name;
            }

            // Возвращает результата для API
            return result;
        }

        //
        public async Task Convert(string test, string variant, string userId)
        {
            await _events.Create(new AbEvent { TestName = test, VariantName = variant, EventType = "conversion", Time = DateTime.UtcNow, UserId = userId });
        }

        public async Task<List<AbEvent>> GetEvents(string testName)
        {
            return await _events.Where(x => x.TestName == testName);
        }
    }
}
