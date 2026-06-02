using Testing.DTO.Charts;
using Testing.DTO;

namespace WebAppTest.Control
{
    /// <summary> Формирование графиков для UI </summary>
    public class ChartService
    {
        private readonly IUiService _ui;

        public ChartService(IUiService ui)
        {
            _ui = ui;
        }

        /// <summary> График активности тестов </summary>
        public async Task<ChartDto> GetTestsActivityChartAsync()
        {
            var tests = await _ui.GetTestsAsync();

            return new ChartDto
            {
                Id = "testsChart",
                Type = "line",
                Title = "Динамика A/B тестов",

                Labels = tests.Select(t => t.Name).ToList(),

                Values = tests
                    .Select(t => t.Enabled ? 1.0 : 0.0)
                    .ToList()
            };
        }

        /// <summary> График распределения вариантов </summary>
        public async Task<ChartDto> GetVariantsChartAsync(string testId)
        {
            var variants = await _ui.GetAllVariantsAsync();
            var results = await _ui.GetResultsAsync(testId);

            var filtered = variants.Where(v => v.AbTestId == testId).ToList();

            return new ChartDto
            {
                Id = "variantChart",
                Type = "bar",
                Title = "Распределение пользователей",

                Labels = filtered.Select(v => v.Name).ToList(),

                Values = filtered
                    .Select(v => results.Count(r => r.VariantId == v.Id))
                    .Select(x => (double)x)
                    .ToList()
            };
        }
    }
}