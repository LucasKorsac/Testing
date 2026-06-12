using System.Collections.Generic;

namespace Testing.Pattern
{
    /// <summary>
    /// Методы расширения для IStrategy<T>
    /// </summary>
    public static class StrategyExtensions
    {
        /// <summary>
        /// Выбор варианта без указания
        /// </summary>
        public static T Choose<T>(this IStrategy<T> strategy, List<T> items, T defaultValue)
        {
            return strategy.Choose(items, defaultValue, null);
        }
    }
}