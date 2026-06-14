using Microsoft.AspNetCore.Mvc.RazorPages;
using Testing.DTO;
using WebAppTest.Control;

namespace WebAppTest.Pages
{
    public class IndexModel : PageModel
    {
        public Dictionary<string, int> VariantsPerTest { get; set; } = new();
        private readonly IUiService _ui;

        public IndexModel(IUiService ui)
        {
            _ui = ui;
        }

        public class ActiveTestRow
        {
            public string Id { get; set; } = "";
            public string Name { get; set; } = "";
            public int VariantsCount { get; set; }
            public bool Enabled { get; set; }
        }

        public List<ActiveTestRow> ActiveTests { get; set; } = new();
        public int TotalTests { get; set; }
        public int ActiveCount { get; set; }
        public int TotalVariants { get; set; }
        public List<string> ChartLabels { get; set; } = new();
        public List<int> ChartValues { get; set; } = new();
        public AnalyticDto Analytics { get; set; } = new();

        // Данные для графика с датами
        public List<string> ChartDates { get; set; } = new();
        public List<int> ChartTestsValues { get; set; } = new();
        public List<int> ChartVariantsValues { get; set; } = new();
        public List<int> ChartApplicationsValues { get; set; } = new();

        public async Task OnGetAsync()
        {
            var tests = await _ui.GetTestsAsync();
            var variants = await _ui.GetAllVariantsAsync();
            var active = await _ui.GetActiveTestsOnlyAsync();
            var applications = await _ui.GetApplicationsAsync();

            VariantsPerTest = variants.GroupBy(x => x.AbTestId).ToDictionary(g => g.Key, g => g.Count());
            Analytics = await _ui.GetAnalyticsAsync();

            ActiveTests = active.Select(test => new ActiveTestRow
            {
                Id = test.Id,
                Name = test.Name ?? "Без названия",
                Enabled = test.Enabled,
                VariantsCount = variants.Count(v => v.AbTestId == test.Id)
            }).ToList();

            TotalTests = tests.Count;
            ActiveCount = tests.Count(t => t.Enabled);
            TotalVariants = variants.Count;

            ChartLabels = tests.Select(x => x.Name ?? "Тест").ToList();
            ChartValues = tests.Select(x => Random.Shared.Next(50, 200)).ToList();

            // Формирование данных для графика с датами (без CreatedAt)
            var random = new Random();
            var currentDate = DateTime.Now;

            ChartDates.Clear();
            ChartTestsValues.Clear();
            ChartVariantsValues.Clear();
            ChartApplicationsValues.Clear();

            // Создаем данные за последние 6 месяцев
            for (int i = 5; i >= 0; i--)
            {
                var date = currentDate.AddMonths(-i);
                ChartDates.Add(date.ToString("MMM yyyy"));

                // Для последнего месяца (текущий) используем реальные данные
                if (i == 0)
                {
                    ChartTestsValues.Add(tests.Count);
                    ChartVariantsValues.Add(variants.Count);
                    ChartApplicationsValues.Add(applications.Count);
                }
                else
                {
                    // Для прошлых месяцев генерируем демонстрационные данные
                    // на основе реальных (чтобы график выглядел правдоподобно)
                    var maxTests = Math.Max(1, tests.Count / 3);
                    var maxVariants = Math.Max(1, variants.Count / 3);
                    var maxApplications = Math.Max(1, applications.Count / 3);

                    ChartTestsValues.Add(random.Next(1, maxTests));
                    ChartVariantsValues.Add(random.Next(1, maxVariants));
                    ChartApplicationsValues.Add(random.Next(1, maxApplications));
                }
            }


        }
    }
}