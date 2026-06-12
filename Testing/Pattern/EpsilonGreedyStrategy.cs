using System;
using System.Collections.Generic;
using System.Linq;
using Testing.DTO;

namespace Testing.Pattern
{
    /// <summary>
    /// Epsilon-Greedy стратегия для MAB
    /// </summary>
    public class EpsilonGreedyStrategy : IMABStrategy
    {
        private readonly Random _random = new Random();
        private readonly Dictionary<string, VariantStats> _stats = new Dictionary<string, VariantStats>();
        private readonly double _epsilon;

        public EpsilonGreedyStrategy(double epsilon = 0.1)
        {
            _epsilon = epsilon;
        }

        //public VariantDto Choose(List<VariantDto> items, VariantDto defaultValue)
        //{
        //    if (items == null || items.Count == 0)
        //        return defaultValue;

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

        //    if (_random.NextDouble() < _epsilon)
        //    {
        //        return items[_random.Next(items.Count)];
        //    }

        //    var bestVariant = items
        //        .OrderByDescending(v => _stats[v.Id].ConversionRate)
        //        .FirstOrDefault();

        //    return bestVariant ?? defaultValue;
        //}

        public VariantDto Choose(List<VariantDto> items, VariantDto defaultValue, string? instanceId = null)
        {
            if (items == null || items.Count == 0)
                return defaultValue;

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

            if (_random.NextDouble() < _epsilon)
            {
                return items[_random.Next(items.Count)];
            }

            var bestVariant = items
                .OrderByDescending(v => _stats[v.Id].ConversionRate)
                .FirstOrDefault();

            return bestVariant ?? defaultValue;
        }

        public void UpdateStats(string variantId, double reward)
        {
            if (!_stats.ContainsKey(variantId))
                return;

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
        }

        public Dictionary<string, VariantStats> GetCurrentStats()
        {
            return new Dictionary<string, VariantStats>(_stats);
        }
    }
}