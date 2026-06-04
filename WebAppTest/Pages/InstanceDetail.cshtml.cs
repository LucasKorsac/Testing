using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Testing.DTO;
using WebAppTest.Control;

namespace WebAppTest.Pages
{
    public class InstanceDetail : PageModel
    {
        private readonly IUiService _ui;

        public InstanceDetail(IUiService ui)
        {
            _ui = ui;
        }

        public InstanceDto? Instance { get; set; }

        public List<TableRow> Table { get; set; } = new();

        // данные для графика
        public List<string> ChartMetrics { get; set; } = new();
        public List<ChartDataPoint> ChartData { get; set; } = new();

        // ДОБАВЬТЕ ЭТИ СВОЙСТВА:
        public List<string> ChartLabels { get; set; } = new();
        public List<int> ChartValues { get; set; } = new();

        public class TableRow
        {
            public string InstallId { get; set; } = "";
            public int VariantsCount { get; set; }
            public string VariantName { get; set; } = "";
            public string Type { get; set; } = "";
            public string Value { get; set; } = "";
            public double NumericValue { get; set; }
        }

        public class ChartDataPoint
        {
            public string MetricName { get; set; } = "";
            public double Value { get; set; }
            public int Frequency { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return RedirectToPage("/Apps");
            }

            Instance = await _ui.GetInstanceAsync(id);

            if (Instance == null)
            {
                return RedirectToPage("/Apps");
            }

            var results = await _ui.GetResultsByInstanceAsync(id);
            var metrics = await _ui.GetMetricsWithTypesAsync(Instance.ApplicationId);
            var variants = await _ui.GetAllVariantsAsync();

            // данные для таблицы
            foreach (var result in results)
            {
                var variant = variants.FirstOrDefault(v => v.Id == result.VariantId);
                if (variant == null) continue;

                foreach (var metric in metrics)
                {
                    Table.Add(new TableRow
                    {
                        InstallId = result.InstanceId,
                        VariantsCount = variants.Count(v => v.AbTestId == variant.AbTestId),
                        VariantName = variant.Name,
                        Type = metric.TypeName ?? "Метрика",
                        Value = metric.Metric.Meaning.ToString("0.00"),
                        NumericValue = metric.Metric.Meaning
                    });
                }
            }

            // данные для графика - получаем все уникальные метрики
            var metricGroups = metrics
                .GroupBy(m => m.TypeName ?? "Метрика")
                .Select(g => g.Key)
                .ToList();

            ChartMetrics = metricGroups;

            // ДОБАВЬТЕ ЭТОТ КОД ДЛЯ ЗАПОЛНЕНИЯ ChartLabels И ChartValues
            ChartLabels = Table.Select(x => x.Type).Distinct().ToList();
            ChartValues = ChartLabels.Select(label => Table.Count(x => x.Type == label)).ToList();

            // собираем статистику по каждой метрике
            ChartData = new List<ChartDataPoint>();
            foreach (var metric in metrics)
            {
                var metricName = metric.TypeName ?? "Метрика";
                var values = Table.Where(t => t.Type == metricName).Select(t => t.NumericValue).ToList();

                var frequency = values
                    .GroupBy(v => v)
                    .Select(g => new ChartDataPoint
                    {
                        MetricName = metricName,
                        Value = g.Key,
                        Frequency = g.Count()
                    })
                    .ToList();

                ChartData.AddRange(frequency);
            }

            return Page();
        }
    }
}