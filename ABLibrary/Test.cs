using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Testing.Pattern;

namespace Testing
{
    public class ClientTest
    {
        private const string TestsFile = "tests.json";
        private const string ResultsFile = "results.json";

        //public int GetValue(string testName, IStrategy<int> strategy);

        public Tests Tests { get; private set; } = new();
        public TestResults TestResults { get; private set; } = new();

        public Action<Tests>? OnLoadCompleted;

        public ClientTest()
        {
            // при старте пробуем загрузить локально
            Tests = LoadFromFile<Tests>(TestsFile) ?? new Tests();
            TestResults = LoadFromFile<TestResults>(ResultsFile) ?? new TestResults();
            // загружаем из файла Tests 
        }

        /// <summary>
        /// Загрузка тестов (имитация сервера)
        /// </summary>
        public void Load()
        {
            // TODO: заменить на HTTP запрос
            var serverTests = FakeServer.GetTests();

            Tests = serverTests;

            SaveToFile(TestsFile, Tests);

            OnLoadCompleted?.Invoke(Tests);
        }

        /// <summary>
        /// Сохранение результатов
        /// </summary>
        public void Save()
        {
            SaveToFile(ResultsFile, TestResults);

            // TODO: отправка на сервер
            FakeServer.SendResults(TestResults);
        }

        public int GetValue(string testName, IStrategy<int> strategy)
        {
            var test = Tests.Items.FirstOrDefault(t => t.Name == testName && t.IsActive);

            if (test == null)
                return 0;

            var value = test.GetValue(strategy);

            var existing = TestResults.Items.FirstOrDefault(x => x.Name == testName);

            if (existing == null)
            {
                TestResults.Items.Add(new Result {Name = testName, Value = value});
            }
            else
            {
                existing.Value = value;
            }

            return value;
        }

        private static void SaveToFile<T>(string file, T data)
        {
            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true});

            File.WriteAllText(file, json);
        }

        private static T? LoadFromFile<T>(string file)
        {
            if (!File.Exists(file))
                return default;

            var json = File.ReadAllText(file);
            return JsonSerializer.Deserialize<T>(json);
        }
    }

    public class TestResults {
        public List<Result> Items { get; set; } = new();
    }
    public class Tests {
        public List<Test> Items { get; set; } = new();
    }
    public class Result {
        public string Name { get; set; } = "";
        public int Value { get; set; }
    }
    public class Test
    {
        public string Name { get; set; } = "";
        public int Default { get; set; }
        public List<int> Values { get; set; } = new();
        public bool IsActive { get; set; }

        // Использование интерфейса стратегии
        public int GetValue(IStrategy<int> strategy)
        {
            return strategy.Choose(Values, Default);
        }

        public override string ToString()
        {
            return $"{Name}: {string.Join(",", Values)}";
        }
    }

    public static class FakeServer
    {
        public static Tests GetTests()
        {
            return new Tests
            {
                Items = new List<Test>
                {
                    new Test
                    {
                        Name = "ButtonColor",
                        Values = new List<int> { 1, 2 },
                        Default = 1,
                        IsActive = true
                    },
                    new Test
                    {
                        Name = "PriceVariant",
                        Values = new List<int> { 10, 20, 30 },
                        Default = 10,
                        IsActive = true
                    }
                }
            };
        }

        public static void SendResults(TestResults results)
        {
            Console.WriteLine("Отправка результатов на сервер:");
            foreach (var r in results.Items)
            {
                Console.WriteLine($"{r.Name} = {r.Value}");
            }
        }
    }
}
