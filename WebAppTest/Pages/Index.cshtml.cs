using Microsoft.AspNetCore.Mvc.RazorPages;
using WebAppTest.Control;
using static Testing.Base.BaseMongo;

namespace WebAppTest.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IUiService _ui;

        public IndexModel(IUiService ui)
        {
            _ui = ui;
        }

        public List<ABTests> ActiveTests { get; set; } = new();

        public int TotalTests { get; set; }
        public int ActiveCount { get; set; }
        public int TotalVariants { get; set; }

        public List<string> ChartLabels { get; set; } = new();
        public List<int> ChartValues { get; set; } = new();

        public async Task OnGet()
        {
            var testsTask = _ui.GetTestsAsync();
            var variantsTask = _ui.GetAllVariantsAsync();
            var activeTask = _ui.GetActiveTestsOnlyAsync();

            await Task.WhenAll(testsTask, variantsTask, activeTask);

            var tests = await testsTask;
            var variants = await variantsTask;

            ActiveTests = await activeTask;

            TotalTests = tests.Count;
            ActiveCount = tests.Count(t => t.Enabled);
            TotalVariants = variants.Count;

            foreach (var test in tests)
            {
                ChartLabels.Add(test.Name);

                ChartValues.Add(
                    variants.Count(v => v.AbTestId == test.Id)
                );
            }
        }
    }
}