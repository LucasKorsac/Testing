//using Microsoft.AspNetCore.Mvc;
//using WebAppTest.Control;

//namespace WebAppTest.Pages
//{
//    public class TestsModel : BasePageModel
//    {
//        public TestsModel(IUiService ui) : base(ui) { }

//        // Добавлено свойство Id
//        public string Id { get; set; } = "";

//        public string TestName { get; set; } = "";
//        public string Description { get; set; } = "";
//        public bool IsActive { get; set; }
//        public Dictionary<string, string> TableData { get; set; } = new();
//        public List<string> VariantLabels { get; set; } = new();
//        public List<int> ChartInstalls { get; set; } = new();
//        public List<double> ChartMetrics { get; set; } = new();
//        public List<string> ChartLabels { get; set; } = new();
//        public List<ResultRow> Results { get; set; } = new();

//        public async Task<IActionResult> OnGetAsync(string id)
//        {
//            // Сохраняем Id для экспорта
//            Id = id;

//            var test = await GetTestOrRedirectAsync(id);
//            if (test == null)
//                return RedirectToPage("/TestControl");

//            TestName = test.Name ?? "";
//            Description = test.Description ?? "";
//            IsActive = test.Enabled;

//            await LoadTestDataAsync(id, test.ApplicationId);

//            return Page();
//        }

//        private async Task LoadTestDataAsync(string testId, string applicationId)
//        {
//            var variants = (await GetVariantsAsync())
//                .Where(v => v.AbTestId == testId)
//                .ToList();

//            var results = await _ui.GetResultsAsync(testId);
//            var metrics = await _ui.GetMetricsWithTypesAsync(applicationId);

//            VariantLabels = variants.Select(v => v.Name).Distinct().ToList();
//            ChartLabels = VariantLabels;

//            foreach (var variant in variants)
//            {
//                ChartInstalls.Add(results.Count(r => r.VariantId == variant.Id));

//                var metricAverage = metrics.Any()
//                    ? metrics.Average(m => m.Metric.Meaning)
//                    : 0;
//                ChartMetrics.Add(Math.Round(metricAverage, 2));
//            }

//            foreach (var result in results)
//            {
//                var variant = variants.FirstOrDefault(v => v.Id == result.VariantId);
//                if (variant == null) continue;

//                foreach (var metric in metrics)
//                {
//                    Results.Add(new ResultRow
//                    {
//                        InstallId = result.InstanceId,
//                        VariantName = variant.Name,
//                        MetricType = metric.TypeName ?? "Метрика",
//                        MetricName = metric.TypeName ?? "Значение",
//                        Value = Math.Round(metric.Metric.Meaning, 2)
//                    });
//                }
//            }

//            TableData = new Dictionary<string, string>
//            {
//                ["Статус"] = IsActive ? "Активен" : "Остановлен",
//                ["Вариантов"] = variants.Count.ToString(),
//                ["Установок"] = results.Count.ToString()
//            };
//        }

//        public class ResultRow
//        {
//            public string InstallId { get; set; } = "";
//            public string VariantName { get; set; } = "";
//            public string MetricType { get; set; } = "";
//            public string MetricName { get; set; } = "";
//            public double Value { get; set; }
//        }
//    }
//}

using Microsoft.AspNetCore.Mvc;
using WebAppTest.Control;

namespace WebAppTest.Pages
{
    public class TestsModel : BasePageModel
    {
        public TestsModel(IUiService ui) : base(ui) { }

        public string Id { get; set; } = "";
        public string TestName { get; set; } = "";
        public string Description { get; set; } = "";
        public bool IsActive { get; set; }
        public string CurrentStrategy { get; set; } = "Random";
        public Dictionary<string, string> TableData { get; set; } = new();
        public List<string> VariantLabels { get; set; } = new();
        public List<int> ChartInstalls { get; set; } = new();
        public List<double> ChartMetrics { get; set; } = new();
        public List<string> ChartLabels { get; set; } = new();
        public List<ResultRow> Results { get; set; } = new();
        public List<MABStatRow> MABStats { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(string id)
        {
            Id = id;
            var test = await GetTestOrRedirectAsync(id);
            if (test == null)
                return RedirectToPage("/TestControl");

            TestName = test.Name ?? "";
            Description = test.Description ?? "";
            IsActive = test.Enabled;

            // Загрузка сохраненной стратегии для теста
            CurrentStrategy = await _ui.GetTestStrategyAsync(id) ?? "Random";

            await LoadTestDataAsync(id, test.ApplicationId);
            await LoadMABStatsAsync(id);

            return Page();
        }

        private async Task LoadTestDataAsync(string testId, string applicationId)
        {
            var variants = (await GetVariantsAsync())
                .Where(v => v.AbTestId == testId)
                .ToList();

            var results = await _ui.GetResultsAsync(testId);
            var metrics = await _ui.GetMetricsWithTypesAsync(applicationId);

            VariantLabels = variants.Select(v => v.Name).Distinct().ToList();
            ChartLabels = VariantLabels;

            foreach (var variant in variants)
            {
                ChartInstalls.Add(results.Count(r => r.VariantId == variant.Id));

                var metricAverage = metrics.Any()
                    ? metrics.Average(m => m.Metric.Meaning)
                    : 0;
                ChartMetrics.Add(Math.Round(metricAverage, 2));
            }

            foreach (var result in results)
            {
                var variant = variants.FirstOrDefault(v => v.Id == result.VariantId);
                if (variant == null) continue;

                foreach (var metric in metrics)
                {
                    Results.Add(new ResultRow
                    {
                        InstallId = result.InstanceId,
                        VariantName = variant.Name,
                        MetricType = metric.TypeName ?? "Метрика",
                        MetricName = metric.TypeName ?? "Значение",
                        Value = Math.Round(metric.Metric.Meaning, 2)
                    });
                }
            }

            TableData = new Dictionary<string, string>
            {
                ["Статус"] = IsActive ? "Активен" : "Остановлен",
                ["Вариантов"] = variants.Count.ToString(),
                ["Установок"] = results.Count.ToString(),
                ["Стратегия"] = CurrentStrategy
            };
        }

        private async Task LoadMABStatsAsync(string testId)
        {
            var mabStats = await _ui.GetMABStatsAsync(testId);
            MABStats = mabStats;
        }

        public class ResultRow
        {
            public string InstallId { get; set; } = "";
            public string VariantName { get; set; } = "";
            public string MetricType { get; set; } = "";
            public string MetricName { get; set; } = "";
            public double Value { get; set; }
        }

        public class MABStatRow
        {
            public string VariantId { get; set; } = "";
            public string VariantName { get; set; } = "";
            public int Count { get; set; }
            public int Successes { get; set; }
            public double ConversionRate { get; set; }
            public string ConfidenceInterval { get; set; } = "";
        }
    }
}