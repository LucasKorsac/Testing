using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Testing.Base;
using Testing.Pattern;
using static Testing.Base.BaseMongo;

namespace Testing
{
    /// <summary>
    /// Пример использования A/B системы (refactored)
    /// </summary>
    internal class Example
    {
        /// <summary>
        /// Результаты: VariantName → Count
        /// </summary>
        public Dictionary<string, int> AB { get; private set; } = new();

        private readonly Facade _facade;
        private readonly IStrategy<Variants> _strategy;
        private readonly IMongoRepo<Variants> _variantRepo;

        public Example(Facade facade, IStrategy<Variants> strategy, IMongoRepo<Variants> variantRepo)
        {
            _facade = facade;
            _strategy = strategy;
            _variantRepo = variantRepo;
        }

        /// <summary>
        /// Инициализация A/B теста
        /// </summary>
        public async Task Init()
        {
            var data = await _facade.GetTestsWithVariants();

            foreach (var pair in data)
            {
                var test = pair.Key;
                var variants = pair.Value;

                if (variants == null || variants.Count == 0)
                    continue;

                var selected = _strategy.Choose(variants, variants[0]);

                if (selected == null)
                    continue;

                var key = $"{test.Name}:{selected.Name}";

                if (!AB.ContainsKey(key))
                    AB[key] = 0;

                AB[key]++;
            }
        }
    }
}