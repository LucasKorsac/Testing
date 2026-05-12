using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Bson;
using WebAppTest.Control;
using static Testing.Base.BaseMongo;

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

        public Applications? Application { get; set; }
        public List<Instances> Instances { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId))
                return RedirectToPage("/Apps");

            Application = await _ui.GetApplicationAsync(id);

            if (Application == null)
                return RedirectToPage("/Apps");

            Instances = await _ui.GetInstancesAsync(Application.Id.ToString());

            return Page();
        }
    }
}