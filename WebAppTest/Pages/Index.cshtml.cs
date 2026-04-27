using Microsoft.AspNetCore.Mvc.RazorPages;
using Testing;

namespace WebAppTest.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        public Test Test { get; set; }

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
            Test = new Test() { Default = 7, Name = "qwrqwr", Values = [1, 3, 5] };
        }

        public void OnGet()
        {

        }
    }
}
