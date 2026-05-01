using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebAppTest.Pages
{
    public class ProfModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public ProfModel(ILogger<IndexModel> logger)
        {

        }
        public void OnGet()
        {
        }
    }
}
