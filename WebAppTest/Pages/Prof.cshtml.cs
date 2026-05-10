using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebAppTest.Pages
{
    public class ProfModel : PageModel
    {
        [BindProperty]
        public string FirstName { get; set; } = "Имя";

        [BindProperty]
        public string LastName { get; set; } = "Фамилия";

        [BindProperty]
        public string Description { get; set; } = "Описание пользователя";

        public string AvatarUrl { get; set; } = "/img/avatar.png";

        public string UserId { get; set; } = "1";

        //Режим UI
        [BindProperty(SupportsGet = true)]
        public string Mode { get; set; } = "view";

        public string ProfileLink => $"user/profile/{UserId}";

        public void OnGet()
        {
        }

        public IActionResult OnPost(string mode)
        {
            if (mode == "edit")
            {
                // Имитация сохранения
                Console.WriteLine($"Saved: {FirstName} {LastName} - {Description}");
            }

            return RedirectToPage(new { mode = "view" });
        }
    }
}