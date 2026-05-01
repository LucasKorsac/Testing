using Microsoft.AspNetCore.Mvc.RazorPages;
using Testing;

namespace WebAppTest.Pages
{
    public class TestControlModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        public Test Test { get; set; }

        public TestControlModel(ILogger<IndexModel> logger)
        {

        }

        public void OnGet()
        {

        }
    }
}
