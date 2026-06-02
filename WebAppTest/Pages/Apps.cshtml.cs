//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.RazorPages;
//using WebAppTest.Control;
//using Testing.DTO;

//namespace WebAppTest.Pages
//{
//    public class Apps : PageModel
//    {
//        private readonly IUiService _ui;

//        public Apps(IUiService ui)
//        {
//            _ui = ui;
//        }

//        public List<ApplicationWithInstancesDto>
//            AppsList
//        { get; set; } = new();

//        public async Task OnGetAsync()
//        {
//            AppsList =
//                await _ui
//                    .GetApplicationWithInstanceAsync();
//        }

//        public async Task<IActionResult>
//            OnPostDeleteAsync(string id)
//        {
//            // позже:
//            // await _ui.DeleteApplicationAsync(id);

//            return RedirectToPage();
//        }
//    }
//}

using Microsoft.AspNetCore.Mvc;
using Testing.DTO;
using WebAppTest.Control;

namespace WebAppTest.Pages
{
    public class Apps : BasePageModel
    {
        public Apps(IUiService ui) : base(ui) { }

        public List<ApplicationWithInstancesDto> AppsList { get; set; } = new();

        public async Task OnGetAsync()
        {
            AppsList = await _ui.GetApplicationWithInstanceAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync(string id)
        {
            return await SafeExecuteAsync(
                async () => await _ui.DeleteApplicationAsync(id),
                successMessage: "Приложение успешно удалено",
                errorMessage: "Не удалось удалить приложение"
            );
        }
    }
}
