using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebAppTest.Control;
using Testing.DTO;

namespace WebAppTest.Pages
{
    /// <summary>
    /// Детали приложения + список версий
    /// </summary>
    public class AppDetail : PageModel
    {
        private readonly IUiService _ui;

        public AppDetail(IUiService ui)
        {
            _ui = ui;
        }

        /// <summary> Приложение </summary>
        public ApplicationDto? Application { get; set; }

        /// <summary> Список инстансов приложения </summary>
        public List<InstanceDto> Instances { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return RedirectToPage("/Apps");

            Application = await _ui.GetApplicationAsync(id);

            if (Application == null)
                return RedirectToPage("/Apps");

            Instances = await _ui.GetInstancesAsync(id);

            return Page();
        }
    }
}