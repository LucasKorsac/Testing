using MongoDB.Bson;
using Testing.Pattern;
using static Testing.Base.BaseMongo;

namespace Testing
{
    /// <summary> Адаптивный сервис построения пула вариантов </summary>
    public class Adaptation
    {
        private readonly IStatsBuilder _statsBuilder;
        private readonly IWeightStrategy _weightStrategy;

        public Adaptation(IStatsBuilder statsBuilder, IWeightStrategy weightStrategy)
        {
            _statsBuilder = statsBuilder;
            _weightStrategy = weightStrategy;
        }

        /// <summary> Построение вероятностного пула вариантов </summary>
        public async Task<List<Variants>> BuildPool(ObjectId testId, int minCount = 5)
        {
            var stats = await _statsBuilder.Build(testId);

            if (stats.Count == 0)
                return new List<Variants>();

            // фильтр по минимальной выборке
            stats = stats
                .Where(s => s.Count >= minCount)
                .OrderByDescending(s => s.Average)
                .ToList();

            if (stats.Count == 0)
                return new List<Variants>();

            int total = stats.Count;

            var pool = new List<Variants>();

            for (int i = 0; i < stats.Count; i++)
            {
                var s = stats[i];

                int weight = _weightStrategy.CalculateWeight(index: i + 1, count: s.Count, total: total, average: s.Average);

                for (int j = 0; j < weight; j++)
                    pool.Add(s.Variant);
            }

            return pool;
        }
    }
}