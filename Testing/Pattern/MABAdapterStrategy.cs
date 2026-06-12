using System.Collections.Generic;
using Testing.DTO;

namespace Testing.Pattern
{
    /// <summary>
    /// Адаптер для использования MAB стратегий в существующей системе
    /// </summary>
    public class MABAdapterStrategy : IStrategy<VariantDto>
    {
        private readonly IMABStrategy _mabStrategy;

        public MABAdapterStrategy(string testId, MABStrategyType strategyType = MABStrategyType.ThompsonSampling)
        {
            var manager = new MABManager(strategyType);
            _mabStrategy = manager.GetOrCreateStrategy(testId, strategyType);
        }

        public MABAdapterStrategy(IMABStrategy mabStrategy)
        {
            _mabStrategy = mabStrategy;
        }

        //public VariantDto Choose(List<VariantDto> items, VariantDto defaultValue)
        //{
        //    return _mabStrategy.Choose(items, defaultValue);
        //}

        public VariantDto Choose(List<VariantDto> items, VariantDto defaultValue, string? instanceId = null)
        {
            return _mabStrategy.Choose(items, defaultValue, instanceId);
        }

        public void UpdateReward(string variantId, double reward)
        {
            _mabStrategy.UpdateStats(variantId, reward);
        }

        public void UpdateConversion(string variantId, bool isSuccess)
        {
            _mabStrategy.UpdateStats(variantId, isSuccess ? 1.0 : 0.0);
        }

        public Dictionary<string, VariantStats> GetStats()
        {
            return _mabStrategy.GetCurrentStats();
        }

        public void Reset()
        {
            _mabStrategy.ResetStats();
        }
    }
}