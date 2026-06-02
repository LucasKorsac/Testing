using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using WebAppTest.Control;

namespace WebAppTest.Pages
{
    public class RegistrationModel : PageModel
    {
        private readonly IUiService _uiService;

        public RegistrationModel(IUiService uiService)
        {
            _uiService = uiService;
        }

        [BindProperty]
        [Required(ErrorMessage = "Введите логин")]
        [MinLength(3, ErrorMessage = "Логин должен содержать минимум 3 символа")]
        public string Login { get; set; } = "";

        [BindProperty]
        [Required(ErrorMessage = "Введите пароль")]
        [MinLength(6, ErrorMessage = "Пароль должен содержать минимум 6 символов")]
        public string Password { get; set; } = "";

        [BindProperty]
        [Required(ErrorMessage = "Подтвердите пароль")]
        [Compare(nameof(Password), ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; } = "";

        public string? ErrorMessage { get; set; }

        public void OnGet()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                RedirectToPage("/Index");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                // Используем IUiService для регистрации
                var success = await _uiService.RegisterDeveloperAsync(Login, Password);

                if (!success)
                {
                    ErrorMessage = "Пользователь с таким логином уже существует";
                    return Page();
                }

                TempData["SuccessMessage"] = "Регистрация прошла успешно! Теперь вы можете войти.";
                return RedirectToPage("/Login");
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка при регистрации: {ex.Message}";
                return Page();
            }
        }
    }
}