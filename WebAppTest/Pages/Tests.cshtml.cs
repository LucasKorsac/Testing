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

        public string TestName { get; set; } = "";
        public string Description { get; set; } = "";

        public bool IsActive { get; set; }

        public Dictionary<string, string> TableData { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(string id)
        {
            var test = await _ui.GetTestByIdAsync(id);

            if (test == null)
                return RedirectToPage("/TestControl");

            TestName = test.Name ?? "Без названия";
            Description = test.Description ?? "";

            IsActive = test.Enabled;

            var results = await _ui.GetResultsAsync(id);

            TableData = new()
            {
                ["ID"] = test.Id.ToString(),
                ["Статус"] = test.Enabled ? "Активен" : "Остановлен",
                ["Результатов"] = results.Count.ToString()
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id, string mode)
        {
            switch (mode)
            {
                case "edit":
                    return RedirectToPage(new { id, mode = "edit" });

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