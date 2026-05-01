using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebAppTest.Pages
{
    public class ManageModel : PageModel
    {

        public ManageModel(ILogger<ProfModel> logger)
        {
            //_logger = logger;
            //Test = new Test() { Default = 7, Name = "qwrqwr", Values = [1, 3, 5] };
        }
        public void OnGet()
        {
        }
    }
}
