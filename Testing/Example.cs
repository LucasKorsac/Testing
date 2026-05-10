using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Testing.Base;
using Testing.Pattern;
using static Testing.Base.BaseMongo;

namespace Testing
{
    /// <summary> Запуск A/B тестов </summary>
    internal class Example
    {
        private readonly Facade _facade;
        private readonly IStrategy<Variants> _strategy;

        public Example(Facade facade, IStrategy<Variants> strategy)
        {
            _facade = facade;
            _strategy = strategy;
        }

        /// <summary> Инициализация тестирования </summary>
        public async Task Init()
        {
            var data = await _facade.GetTests();

            foreach (var item in data)
            {
                // защита от пустых данных
                if (item.Variants == null || item.Variants.Count == 0)
                    continue;

                // выбор варианта через стратегию
                var selected = _strategy.Choose(item.Variants, item.Variants[0]);

                // вывод результата
                Console.WriteLine($"Test: {item.Test.Name} → Variant: {selected.Name}");
            }
        }
    }
}