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

        //private readonly IMongoRepo<AbEvent> _events;

        /// <summary>
        /// Репозиторий вариантов
        /// </summary>
        //private readonly IMongoRepo<Variants> _variantRepo;

        /// <summary>
        /// Конструктор с внедрением зависимостей
        /// </summary>
        public ServiceControl(Facade facade, IStrategy<Variants> strategy, IMongoRepo<Variants> variantRepo)
        {
            _facade = facade;
            _strategy = strategy;
        }

        /// <summary>
        /// Основной метод запуска A/B тестирования. Возврат выбранных вариантов для каждого теста
        /// </summary>
        public async Task<Dictionary<string, string>> Run(ObjectId applicationId)
        {
            // Результат: TestName -> VariantName
            var result = new Dictionary<string, string>();

            // Получение тестов и их вариантов
            var tests = await _facade.GetTests();

            foreach (var item in tests)
            {
                var test = item.Test;

                var variants = item.Variants;

                if (variants == null || variants.Count == 0)
                    continue;

                var fallback = variants.First();
                var selected = _strategy.Choose(variants, fallback);

                result[test.Name] = selected.Name;
            }

            // Возвращает результата для API
            return result;
        }
    }
}
