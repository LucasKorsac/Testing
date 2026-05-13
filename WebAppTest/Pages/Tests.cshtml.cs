using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebAppTest.Control;

namespace WebAppTest.Pages
{
    public class TestsModel : PageModel
    {
        private readonly IUiService _ui;

        public TestsModel(IUiService ui)
        {
            _ui = ui;
        }

        // Информация

        public string TestName { get; set; } = "";

        public string Description { get; set; } = "";

        public bool IsActive { get; set; }

        // Таблица

        public Dictionary<string, string> TableData
        { get; set; } = new();

        // График

        public List<string> VariantLabels
        { get; set; } = new();

        public List<int> VariantValues
        { get; set; } = new();

        // GET

        public async Task<IActionResult>
            OnGetAsync(string id)
        {
            var test =
                await _ui.GetTestByIdAsync(id);

            if (test == null)
            {
                return RedirectToPage("/TestControl");
            }

            // Информация

            TestName = test.Name ?? "Без названия";

            Description = test.Description ?? "";

            IsActive = test.Enabled;

            // Данные

            var allVariants =
                await _ui.GetVariantsAsync();

            var results =
                await _ui.GetResultsAsync(id);

            var variants = allVariants
                .Where(v => v.AbTestId == test.Id)
                .ToList();

            // Таблица

            TableData = new Dictionary<string, string>
            {
                ["ID теста"] = test.Id.ToString(),

                ["Название"] = TestName,

                ["Статус"] =
                    IsActive
                        ? "Активен"
                        : "Остановлен",

                ["Количество вариантов"] =
                    variants.Count.ToString(),

                ["Всего пользователей"] =
                    results.Count.ToString()
            };

            // График

            VariantLabels.Clear();

            VariantValues.Clear();

            foreach (var variant in variants)
            {
                VariantLabels.Add(variant.Name);

                int usersCount = results.Count(r =>
                    r.VariantId == variant.Id);

                VariantValues.Add(usersCount);
            }

            return Page();
        }

        // POST

        public async Task<IActionResult>
            OnPostAsync(string id, string mode)
        {
            switch (mode)
            {
                case "stop":

                    await _ui.StopTestAsync(id);

                    break;

                case "resume":

                    await _ui.ResumeTestAsync(id);

                    break;

                case "delete":

                    await _ui.DeleteTestAsync(id);

                    return RedirectToPage("/TestControl");
            }

            return RedirectToPage(new { id });
        }
    }
}