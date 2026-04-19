using MongoDB.Bson;
using Testing.Base;
using static Testing.Base.BaseMongo;

namespace Testing.Pattern
{
    //Паттерн фасад
    public class Facade
    {
        // Репозиторий A/B тестов
        private readonly IMongoRepo<ABTests> _abTests;

        // Репозиторий вариантов тестов
        private readonly IMongoRepo<Variants> _variants;

        public Facade(IMongoRepo<ABTests> abTests, IMongoRepo<Variants> variants)
        {
            _abTests = abTests;
            _variants = variants;
        }

        /// <summary>
        /// Получение всех A/B тестов
        /// </summary>
        public async Task<List<ABTests>> GetAllTests() => await _abTests.GetAll();

        /// <summary>
        /// Получение вариантов для конкретного теста
        /// </summary>
        public async Task<List<Variants>> GetVariants(ObjectId testId) => await _variants.Where(x => x.AbTestId == testId);

        /*
        /// <summary>
        /// Получение тестов по приложению
        /// </summary>
        public async Task<List<ABTests>> GetByApplication(ObjectId appId)
            => await _abTests.Where(x => x.ApplicationId == appId);
        */

        /*
        /// <summary>
        /// Система событий (Наблюдатель)
        /// используется для логирования или аналитики
        /// </summary>
        private readonly Subject<AbTestEvent> _events = new();
        */

        /*
        /// <summary>
        /// Обработка события выбора варианта
        /// </summary>
        public void OnVariantSelected(string testName, string variantName)
        {
            _events.Notify(new AbTestEvent
            {
                Action = "VariantSelected",
                TestName = testName,
                Variant = variantName
            });
        }
        */
    }
}