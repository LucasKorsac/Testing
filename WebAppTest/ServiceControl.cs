using MongoDB.Bson;
using Testing.Base;
using Testing.Pattern;
using static Testing.Base.BaseMongo;

namespace WebAppTest
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

        /// <summary>
        /// Репозиторий вариантов
        /// </summary>
        private readonly IMongoRepo<Variants> _variantRepo;

        /// <summary>
        /// Конструктор с внедрением зависимостей
        /// </summary>
        public ServiceControl(Facade facade, IStrategy<Variants> strategy, IMongoRepo<Variants> variantRepo)
        {
            _facade = facade;
            _strategy = strategy;
            _variantRepo = variantRepo;
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
                var selected = _strategy.Choose(variants, variants[0]);

                // Сохраняем результат
                result[test.Name] = selected.Name;
            }

            /// Возвращат результата для API
            return result;
        }
    }
}