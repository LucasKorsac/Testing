using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Testing.Pattern;
using WebAppTest.Control;

namespace Testing
{
    public class ClientTest
    {
        private const string ResultsFile = "results.json";

        private readonly ApiClient _api;

        public Dictionary<string, string> ActiveTests { get; private set; } = new();
        public TestResults TestResults { get; private set; } = new();

        public ClientTest(ApiClient api)
        {
            _api = api;

            // загружаем только результаты (тесты теперь приходят с сервера)
            TestResults = LoadFromFile<TestResults>(ResultsFile) ?? new TestResults();
        }

        /// <summary>
        /// Загрузка тестов с сервера
        /// </summary>
        public async Task LoadAsync(string appId)
        {
            var serverTests = await _api.RunAsync(appId);

            if (serverTests != null)
            {
                ActiveTests = serverTests;
            }
        }

        /// <summary>
        /// Отправка результата (конверсии)
        /// </summary>
        public async Task SendResultAsync(string testName, string userId)
        {
            if (!ActiveTests.TryGetValue(testName, out var variant))
                return;

            await _api.ConvertAsync(testName, variant, userId);

            // сохраняем локально
            var existing = TestResults.Items.FirstOrDefault(x => x.Name == testName);

            if (existing == null)
            {
                TestResults.Items.Add(new Result { Name = testName, Value = 1 });
            }
            else
            {
                existing.Value++;
            }

            SaveToFile(ResultsFile, TestResults);
        }

        /// <summary>
        /// Получить вариант теста
        /// </summary>
        public string GetVariant(string testName)
        {
            return ActiveTests.TryGetValue(testName, out var variant)
                ? variant
                : "default";
        }

        // ================= FILE =================

        private static void SaveToFile<T>(string file, T data)
        {
            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions
            {
                WriteIndented = true
            });

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

    // ================= MODELS =================

    public class TestResults
    {
        public List<Result> Items { get; set; } = new();
    }

    public class Result
    {
        public string Name { get; set; } = "";
        public int Value { get; set; }
    }
}