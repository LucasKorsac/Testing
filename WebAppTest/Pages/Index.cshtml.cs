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

        public async Task OnGetAsync()
        {
            var tests = await _ui.GetTestsAsync();
            var variants = await _ui.GetAllVariantsAsync();
            var active = await _ui.GetActiveTestsOnlyAsync();

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
        }
    }
}