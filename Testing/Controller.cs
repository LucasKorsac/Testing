using System;
using System.Collections.Generic;

namespace Testing
{
    // Контроллер для хранения текущих A/B тестов
    internal class Controller
    {
        public static readonly Controller I = new();

        // Словарь текущих тестов: ключ — имя теста, значение
        public Dictionary<string, int> CurrentTests { get; private set; } = new();

        // Инициализация контроллера данными A/B тестов
        public void Init(Dictionary<string, int> ab)
        {
            CurrentTests = ab;
        }

        // Получение значения теста по имени если тест не найден — возвращается 0
        public int Get(string name)
        {
            return CurrentTests.TryGetValue(name, out var val) ? val : 0;
        }
    }
}