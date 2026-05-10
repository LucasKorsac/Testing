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

        public List<ABTests> Tests { get; set; } = new();

        public async Task OnGetAsync()
        {
            Tests = await _ui.GetTestsAsync() ?? new List<ABTests>();
        }
    }
}