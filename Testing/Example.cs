using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Testing.DTO;
using Testing.Pattern;

namespace Testing
{
    /// <summary>
    /// Запуск A/B тестов
    /// </summary>
    internal class Example
    {
        private readonly Facade _facade;

        // стратегия теперь работает с DTO
        private readonly IStrategy<VariantDto> _strategy;

        public Example(
            Facade facade,
            IStrategy<VariantDto> strategy)
        {
            _facade = facade;
            _strategy = strategy;
        }

        /// <summary>
        /// Инициализация тестирования
        /// </summary>
        public async Task Init()
        {
            var data = await _facade.GetTests();

            foreach (var item in data)
            {
                // защита от null
                var variants = item.Variants ?? new List<VariantDto>();

                if (variants.Count == 0)
                    continue;

                // fallback
                var fallback = variants[0];

                // выбор варианта
                var selected =  _strategy.Choose(variants, fallback);

                Console.WriteLine($"Test: {item.Test.Name} - Variant: {selected.Name}");
            }
        }
    }
}