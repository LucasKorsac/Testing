using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testing.Base;
using static Testing.Base.BaseMongo;

namespace Testing
{
    /// <summary>
    /// Адаптивный алгоритм перераспределения вероятностей (MAB-подобный)
    /// </summary>
    public class Adaptation
    {
        private readonly IMongoRepo<Variants> _variantRepo;
        private readonly IMongoRepo<AbResults> _resultRepo;
        private readonly IMongoRepo<Values> _valueRepo;
        private readonly IMongoRepo<AbEvent> _events;

        public Adaptation(IMongoRepo<Variants> variantRepo, IMongoRepo<AbResults> resultRepo, IMongoRepo<Values> valueRepo)
        {
            _variantRepo = variantRepo;
            _resultRepo = resultRepo;
            _valueRepo = valueRepo;
        }

        /// <summary>
        /// Возврат пула вариантов с учетом весов
        /// </summary>
        public async Task<List<Variants>> BuildPool(ObjectId testId, int minCount = 5, double k = 1.0)
        {   // Получение вариантов теста
            var variants = await _variantRepo.Where(v => v.AbTestId == testId);
            if (variants.Count == 0) return new List<Variants>();

            var stats = new List<VariantStat>();

            // Сбор данных
            foreach (var variant in variants)
            {   var results = await _resultRepo.Where(r => r.VariantId == variant.Id);
                var instanceIds = results.Select(r => r.InstanceId).ToList();
                var values = await _valueRepo.Where(v => instanceIds.Contains(v.InstanceId));

                if (values.Count == 0) continue;

                var avg = values.Average(v => v.MetricValue);

                stats.Add(new VariantStat{Variant = variant, Count = values.Count, Average = avg});
            }

            // Фильтрация 
            stats = stats
                .Where(s => s.Count >= minCount)
                .ToList();

            if (stats.Count == 0) return new List<Variants>();

            // Ранжирование по возрастанию
            stats = stats
                .OrderBy(s => s.Average)
                .ToList();

            int n = stats.Count;

            // Расчет весов
            for (int i = 0; i < stats.Count; i++)
            {
                int index = i + 1; // чтобы не было 0
                var s = stats[i];
                double weight = (Math.Pow(index, 2) * Math.Sqrt(s.Count)) / (k * n);

                // минимум 1, чтобы вариант не исчез
                s.Weight = Math.Max(1, (int)Math.Round(weight));
            }

            // Формирование пула
            var pool = new List<Variants>();

            foreach (var s in stats)
            {for (int i = 0; i < s.Weight; i++)
                { pool.Add(s.Variant); }}
            return pool;
        }

        /// <summary>
        /// Внутренняя модель статистики
        /// </summary>
        private class VariantStat
        {
            public Variants Variant { get; set; }
            public int Count { get; set; }
            public double Average { get; set; }
            public int Weight { get; set; }
        }
    }
}

