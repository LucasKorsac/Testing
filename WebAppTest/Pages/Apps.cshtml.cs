using Microsoft.AspNetCore.Mvc.RazorPages;
using Testing.Pattern;
using WebAppTest.Control;

namespace WebAppTest.Pages
{
    public class Apps : PageModel
    {
        private readonly IUiService _ui;

        public Apps(IUiService ui)
        {
            _ui = ui;
        }

        public List<ApplicationWithInstances> AppsList { get; set; } = new();

        public async Task OnGetAsync()
        {
            AppsList = await _ui.GetApplicationWithInstanceAsync();
        }
    }
}