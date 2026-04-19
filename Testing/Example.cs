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
            var tests = await _facade.GetAllTests();

            if (tests == null || tests.Count == 0)
                return;

            foreach (var test in tests)
            {
                // Варианты из Mongo
                var variants = await _variantRepo.Where(
                    x => x.AbTestId == test.Id
                );

                if (variants == null || variants.Count == 0)
                    continue;

                // Выбор варианта через стратегию
                var selected = _strategy.Choose(variants, variants[0]);

                if (selected == null)
                    continue;

                // Запись результата
                var key = $"{test.Name}:{selected.Name}";

                if (!AB.ContainsKey(key))
                    AB[key] = 0;

                AB[key]++;
            }
        }
    }
}