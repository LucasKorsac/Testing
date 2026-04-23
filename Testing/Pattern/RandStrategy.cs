using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testing.Pattern
{
    internal class RandStrategy
    {
        /// <summary>
        /// Случайная стратегия выбора
        /// </summary>
        public class RandomStrategy<T> : IStrategy<T>
        {
            private static readonly Random _rnd = new();

            public T Choose(List<T> items, T defaultValue)
            {
                if (items == null || items.Count == 0)
                    return defaultValue;

                return items[_rnd.Next(items.Count)];
            }
        }
    }
}
