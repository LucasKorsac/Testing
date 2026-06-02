using MongoDB.Bson;
using Testing.Base;
using Testing.DTO;
using static Testing.Base.BaseMongo;

namespace Testing.Pattern
{
    /// <summary> DTO статистики варианта </summary>
    public class VariantStat
    {
        /// <summary> Вариант теста </summary>
        public VariantDto Variant { get; set; } = new();

        /// <summary> Количество значений </summary>
        public int Count { get; set; }

        /// <summary> Среднее значение метрики </summary>
        public double Average { get; set; }

        /// <summary> Вес варианта </summary>
        public int Weight { get; set; }
    }

    /// <summary> Интерфейс построения статистики </summary>
    public interface IStatsBuilder
    {
        Task<List<VariantStat>> Build(ObjectId testId);
    }

    /// <summary> Построитель статистики A/B тестов </summary>
    public class StatsBuilder : IStatsBuilder
    {
        /// <summary> Репозиторий вариантов </summary>
        private readonly IMongoRepo<Variants> _variantRepo;

        /// <summary> Репозиторий результатов </summary>
        private readonly IMongoRepo<AbResults> _resultRepo;

        /// <summary> Репозиторий метрик </summary>
        private readonly IMongoRepo<Values> _valueRepo;

        public StatsBuilder(
            IMongoRepo<Variants> variantRepo,
            IMongoRepo<AbResults> resultRepo,
            IMongoRepo<Values> valueRepo)
        {
            _variantRepo = variantRepo;
            _resultRepo = resultRepo;
            _valueRepo = valueRepo;
        }

        /// <summary> Построение статистики по тесту </summary>
        public async Task<List<VariantStat>> Build(ObjectId testId)
        {
            // варианты теста
            var variants = await _variantRepo
                .Where(v => v.AbTestId == testId);

            var stats = new List<VariantStat>();

            foreach (var variant in variants)
            {
                // результаты по варианту
                var results = await _resultRepo
                    .Where(r => r.VariantId == variant.Id);

                var instanceIds = results
                    .Select(r => r.InstanceId)
                    .ToList();

                // если результатов нет
                if (instanceIds.Count == 0)
                    continue;

                // значения метрик
                var values = await _valueRepo
                    .Where(v => instanceIds.Contains(v.InstanceId));

                // если метрик нет
                if (values.Count == 0)
                    continue;

                var average = values.Average(v => v.MetricValue);

                // статистика
                stats.Add(new VariantStat
                {
                    Variant = new VariantDto
                    {
                        Id = variant.Id.ToString(),
                        AbTestId = variant.AbTestId.ToString(),
                        Name = variant.Name,
                        Description = variant.Description,
                        Mean = variant.Mean,
                        Audience = variant.Audience
                    },

                    Count = values.Count,

                    Average = average,

                    // пока вес = среднему значению
                    Weight = (int)Math.Round(average)
                });
            }

            return stats;
        }
    }
}