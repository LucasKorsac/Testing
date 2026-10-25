using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebAppTest.Pages
{
    using global::WebAppTest.Services;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using WebAppTest.Services;

    namespace WebAppTest.Pages;

    public class UpliftModel : PageModel
    {
        private readonly UpliftApiClient _upliftApiClient;

        public UpliftModel(UpliftApiClient upliftApiClient)
        {
            _upliftApiClient = upliftApiClient;
        }

        [BindProperty]
        public string TestId { get; set; } = string.Empty;

        [BindProperty]
        public string ModelName { get; set; } = "T";

        [BindProperty]
        public bool UseCuped { get; set; }

        public List<UpliftTestOption> Tests { get; set; } = new();

        public UpliftAnalysisResult? Result { get; set; }

        public string? ErrorMessage { get; set; }

        public async Task OnGetAsync()
        {
            // Здесь загрузим реальные A/B-тесты из MongoDB.
            // Пока источник данных ещё не подключён.
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(TestId))
            {
                ErrorMessage = "Выбери A/B-тест.";
                await OnGetAsync();
                return Page();
            }

            // Здесь нужно получить данные выбранного теста,
            // отправить их в Python API и разобрать ответ.
            // Это зависит от структуры твоих MongoDB-документов.

            ErrorMessage =
                "Источник данных теста ещё не подключён.";

            await OnGetAsync();
            return Page();
        }
    }

    public class UpliftTestOption
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }

    public class UpliftAnalysisResult
    {
        public UpliftMetrics Metrics { get; set; } = new();
    }

    public class UpliftMetrics
    {
        public int Users { get; set; }
        public double ControlMean { get; set; }
        public double TreatmentMean { get; set; }
        public double ObservedUplift { get; set; }
        public double PredictedUpliftMean { get; set; }
        public int PositiveUpliftUsers { get; set; }
    }
}
