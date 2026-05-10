using Microsoft.AspNetCore.Mvc.RazorPages;
using WebAppTest.Control;

namespace WebAppTest.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IUiService _ui;

        public IndexModel(IUiService ui)
        {
            _ui = ui;
        }

        // Аналитика

        public Dictionary<string, string> ActiveTests { get; set; } = new();

        public int TotalTests { get; set; }

        public int ActiveCount { get; set; }

        public int TotalVariants { get; set; }

        // Данные графика

        public List<string> ChartLabels { get; set; } = new();

        public List<int> ChartValues { get; set; } = new();

        // GET

        public async Task OnGet()
        {
            // Активные тесты
            ActiveTests = await _ui.GetActiveTestsAsync("web-app");

            // Все тесты
            var tests = await _ui.GetTestsAsync();

            // Варианты
            var variants = await _ui.GetVariantsAsync();

            // Статистика
            TotalTests = tests.Count;

            ActiveCount = tests.Count(t => t.Enabled);

            TotalVariants = variants.Count;

            // Данные для графика

            foreach (var test in tests)
            {
                ChartLabels.Add(test.Name);

                var count = variants
                    .Count(v => v.AbTestId == test.Id);

                ChartValues.Add(count);
            }
        }
    }
}