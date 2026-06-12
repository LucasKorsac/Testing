using System;

namespace Testing.Pattern
{
    /// <summary> Случайная стратегия выбора </summary>
    public class RandomStrategy<T> : IStrategy<T>
    {
        /// <summary> Генератор случайных чисел </summary>
        private static readonly Random _rnd = new();

        /// <summary> Случайный выбор элемента из списка </summary>
        //public T Choose(List<T> items, T defaultValue)
        //{
        //    // если список пуст
        //    if (items == null || items.Count == 0)
        //        return defaultValue;

        //    // случайный индекс
        //    var index = _rnd.Next(items.Count);

        //    return items[index];
        //}

        public T Choose(List<T> items, T defaultValue, string? instanceId = null)
        {
            // если список пуст
            if (items == null || items.Count == 0)
                return defaultValue;

            // случайный индекс
            var index = _rnd.Next(items.Count);

            return items[index];
        }
    }
}