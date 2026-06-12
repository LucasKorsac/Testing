using System;
using System.Collections.Generic;
using System.Linq;
using Testing.DTO;

namespace Testing.Pattern
{
    /// <summary>
    /// Thompson Sampling стратегия для бинарных метрик (конверсия)
    /// </summary>
    public class ThompsonSamplingStrategy : IMABStrategy
    {
        private readonly Random _random = new Random();
        private readonly Dictionary<string, VariantStats> _stats = new Dictionary<string, VariantStats>();
        private readonly double _epsilon = 0.05;
        private int _totalCount = 0;

        public ThompsonSamplingStrategy(double epsilon = 0.05)
        {
            _epsilon = epsilon;
        }

        //public VariantDto Choose(List<VariantDto> items, VariantDto defaultValue)
        //{
        //    if (items == null || items.Count == 0)
        //        return defaultValue;

        //    // Инициализация статистики для новых вариантов
        //    foreach (var item in items)
        //    {
        //        if (!_stats.ContainsKey(item.Id))
        //        {
        //            _stats[item.Id] = new VariantStats
        //            {
        //                VariantId = item.Id,
        //                VariantName = item.Name
        //            };
        //        }
        //    }

        //    // Эпсилон-жадное исследование
        //    if (_random.NextDouble() < _epsilon && _totalCount > 100)
        //    {
        //        return items[_random.Next(items.Count)];
        //    }

        //    // Thompson Sampling: выбор варианта с максимальным сэмплом
        //    var bestVariant = items
        //        .Select(item => new
        //        {
        //            Variant = item,
        //            Sample = SampleBeta(
        //                _stats[item.Id].Successes + 1,
        //                _stats[item.Id].Failures + 1
        //            )
        //        })
        //        .OrderByDescending(x => x.Sample)
        //        .FirstOrDefault();

        //    return bestVariant?.Variant ?? defaultValue;
        //}

        public VariantDto Choose(List<VariantDto> items, VariantDto defaultValue, string? instanceId = null)
        {
            if (items == null || items.Count == 0)
                return defaultValue;

            // Инициализация статистики для новых вариантов
            foreach (var item in items)
            {
                if (!_stats.ContainsKey(item.Id))
                {
                    _stats[item.Id] = new VariantStats
                    {
                        VariantId = item.Id,
                        VariantName = item.Name
                    };
                }
            }

            // Эпсилон-жадное исследование
            if (_random.NextDouble() < _epsilon && _totalCount > 100)
            {
                return items[_random.Next(items.Count)];
            }

            // Thompson Sampling: выбор варианта с максимальным сэмплом
            var bestVariant = items
                .Select(item => new
                {
                    Variant = item,
                    Sample = SampleBeta(
                        _stats[item.Id].Successes + 1,
                        _stats[item.Id].Failures + 1
                    )
                })
                .OrderByDescending(x => x.Sample)
                .FirstOrDefault();

            return bestVariant?.Variant ?? defaultValue;
        }

        private double SampleBeta(double alpha, double beta)
        {
            var x = SampleGamma(alpha, 1);
            var y = SampleGamma(beta, 1);
            return x / (x + y);
        }

        private double SampleGamma(double shape, double scale)
        {
            if (shape < 1)
            {
                var u = _random.NextDouble();
                return SampleGamma(1 + shape, scale) * Math.Pow(u, 1 / shape);
            }

            var d = shape - 1 / 3.0;
            var c = 1 / Math.Sqrt(9 * d);

            while (true)
            {
                var x = 0.0;
                var v = 0.0;

                do
                {
                    x = SampleNormal(0, 1);
                    v = 1 + c * x;
                }
                while (v <= 0);

                v = v * v * v;
                var u = _random.NextDouble();

                if (u < 1 - 0.0331 * Math.Pow(x, 4))
                    return d * v * scale;

                if (Math.Log(u) < 0.5 * Math.Pow(x, 2) + d * (1 - v + Math.Log(v)))
                    return d * v * scale;
            }
        }

        private double SampleNormal(double mean, double stdDev)
        {
            var u1 = 1.0 - _random.NextDouble();
            var u2 = 1.0 - _random.NextDouble();
            var randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
            return mean + stdDev * randStdNormal;
        }

        public void UpdateStats(string variantId, double reward)
        {
            if (!_stats.ContainsKey(variantId))
                return;

            _totalCount++;
            _stats[variantId].Count++;
            _stats[variantId].TotalReward += reward;

            if (reward >= 0.5)
            {
                _stats[variantId].Successes++;
            }
            else
            {
                _stats[variantId].Failures++;
            }
        }

        public void ResetStats()
        {
            _stats.Clear();
            _totalCount = 0;
        }

        public Dictionary<string, VariantStats> GetCurrentStats()
        {
            return new Dictionary<string, VariantStats>(_stats);
        }
    }
}