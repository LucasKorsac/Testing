using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebAppTest.Control;
using Testing.Base;
using static Testing.Base.BaseMongo;

namespace WebAppTest.Pages
{
    public class TestControlModel : PageModel
    {
        private readonly IUiService _ui;

        public TestControlModel(IUiService ui)
        {
            _ui = ui;
        }

        // список всех тестов
        public List<ABTests> Tests { get; set; } = new();

        // загрузка страницы
        public async Task OnGetAsync()
        {
            Tests = await _ui.GetTestsAsync()
                    ?? new List<ABTests>();
        }

        // удаление теста
        public async Task<IActionResult> OnPostDeleteAsync(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                await _ui.DeleteTestAsync(id);
            }

            return RedirectToPage();
        }

        // остановка теста
        public async Task<IActionResult> OnPostStopAsync(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                await _ui.StopTestAsync(id);
            }

            return RedirectToPage();
        }

        // возобновление теста
        public async Task<IActionResult> OnPostResumeAsync(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                await _ui.ResumeTestAsync(id);
            }

            return RedirectToPage();
        }
    }
}