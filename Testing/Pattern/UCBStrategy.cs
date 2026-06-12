using System;
using System.Collections.Generic;
using System.Linq;
using Testing.DTO;

namespace Testing.Pattern
{
    /// <summary>
    /// Upper Confidence Bound (UCB) стратегия для MAB
    /// </summary>
    public class UCBStrategy : IMABStrategy
    {
        private readonly Dictionary<string, VariantStats> _stats = new Dictionary<string, VariantStats>();
        private int _totalCount = 0;
        private readonly double _c = 1.0;

        public UCBStrategy(double c = 1.0)
        {
            _c = c;
        }

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

            var untriedVariants = items.Where(v => _stats[v.Id].Count == 0).ToList();
            if (untriedVariants.Any())
            {
                return untriedVariants.First();
            }

            var bestVariant = items
                .Select(item => new
                {
                    Variant = item,
                    UCBValue = CalculateUCB(item.Id)
                })
                .OrderByDescending(x => x.UCBValue)
                .FirstOrDefault();

            return bestVariant?.Variant ?? defaultValue;
        }

        private double CalculateUCB(string variantId)
        {
            var stats = _stats[variantId];
            var avgReward = stats.ConversionRate;
            var exploration = _c * Math.Sqrt(Math.Log(_totalCount + 1) / stats.Count);
            return avgReward + exploration;
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