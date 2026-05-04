using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Testing.Base;
using Testing.Pattern;
using static Testing.Base.BaseMongo;

namespace Testing
{
    /// <summary> Выполнение выбора вариантов тестов с помощью стратегии и сбор статистики распределения </summary>
    internal class Example
    {
        /// <summary> Результаты выполнения A/B тестов </summary>
        public Dictionary<string, int> AB { get; private set; } = new();

        /// <summary> Фасад для получения тестов и их вариантов </summary>
        private readonly Facade _facade;

        /// <summary> Стратегия выбора варианта (Random / Adaptive) </summary>
        private readonly IStrategy<Variants> _strategy;

        /// <summary> Репозиторий вариантов </summary>
        private readonly IMongoRepo<Variants> _variantRepo;

        /// <summary> Конструктор с внедрением зависимостей </summary>
        public Example(Facade facade, IStrategy<Variants> strategy, IMongoRepo<Variants> variantRepo)
        {
            _facade = facade;
            _strategy = strategy;
            _variantRepo = variantRepo;
        }

        /// <summary> Инициализация A/B тестирования </summary>
        public async Task Init()
        {
            // Получаем все тесты вместе с вариантами
            var data = await _facade.GetTests();

            foreach (var item in data)
            {
                var test = item.Test;               // A/B тест
                var variants = item.Variants;      // варианты теста

                // Пропуск теста без вариантов
                if (variants == null || variants.Count == 0)
                    continue;

                // Выбор варианта через стратегию
                var selected = _strategy.Choose(variants, variants[0]);

                // Если стратегия не вернула результат — пропуск
                if (selected == null)
                    continue;

                // Формирование ключа статистики
                var key = $"{test.Name}:{selected.Name}";

                // Инициализация счётчика при первом появлении
                if (!AB.ContainsKey(key))
                    AB[key] = 0;

                // Увеличение счётчика выбранного варианта
                AB[key]++;
            }
        }
    }
}