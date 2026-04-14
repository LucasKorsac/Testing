using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testing.Pattern
{
    /// <summary>
    /// Паттерн стратегия. Выбор варианта A/B теста
    /// </summary>
    public interface IStrategy
    {
        /// <summary>
        /// Выбор варианта из списка
        /// </summary>
        int GetVariant(List<int> variants, int defaultValue);
    }

    /// <summary>
    /// Случайный выбор варианта
    /// </summary>
    public class RandomStrategy : IStrategy
    {
        private static readonly Random _rnd = new();

        public int GetVariant(List<int> variants, int defaultValue)
        {
            if (variants == null || variants.Count == 0) return defaultValue;

            return variants[_rnd.Next(variants.Count)];
        }
    }

}
