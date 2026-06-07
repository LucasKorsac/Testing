using System.Collections.Generic;

namespace Testing.Pattern
{
    /// <summary>
    /// Типы MAB стратегий
    /// </summary>
    public enum MABStrategyType
    {
        ThompsonSampling,
        UCB,
        EpsilonGreedy
    }

    /// <summary>
    /// Менеджер для работы с MAB стратегиями
    /// </summary>
    public class MABManager
    {
        private readonly Dictionary<string, IMABStrategy> _strategies = new Dictionary<string, IMABStrategy>();
        private readonly MABStrategyType _defaultStrategyType;

        public MABManager(MABStrategyType defaultStrategyType = MABStrategyType.ThompsonSampling)
        {
            _defaultStrategyType = defaultStrategyType;
        }

        public IMABStrategy GetOrCreateStrategy(string testId, MABStrategyType? strategyType = null)
        {
            var type = strategyType ?? _defaultStrategyType;
            var key = $"{testId}_{type}";

            if (!_strategies.ContainsKey(key))
            {
                _strategies[key] = CreateStrategy(type);
            }

            return _strategies[key];
        }

        private IMABStrategy CreateStrategy(MABStrategyType type)
        {
            return type switch
            {
                MABStrategyType.ThompsonSampling => new ThompsonSamplingStrategy(),
                MABStrategyType.UCB => new UCBStrategy(),
                MABStrategyType.EpsilonGreedy => new EpsilonGreedyStrategy(),
                _ => new ThompsonSamplingStrategy()
            };
        }

        public void UpdateStats(string testId, string variantId, double reward, MABStrategyType? strategyType = null)
        {
            var strategy = GetOrCreateStrategy(testId, strategyType);
            strategy.UpdateStats(variantId, reward);
        }

        public void ResetStats(string testId, MABStrategyType? strategyType = null)
        {
            var type = strategyType ?? _defaultStrategyType;
            var key = $"{testId}_{type}";

            if (_strategies.ContainsKey(key))
            {
                _strategies[key].ResetStats();
                _strategies.Remove(key);
            }
        }

        public Dictionary<string, VariantStats> GetStats(string testId, MABStrategyType? strategyType = null)
        {
            var strategy = GetOrCreateStrategy(testId, strategyType);
            return strategy.GetCurrentStats();
        }
    }
}