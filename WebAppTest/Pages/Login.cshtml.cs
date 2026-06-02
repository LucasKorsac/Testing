using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using WebAppTest.Control;

namespace WebAppTest.Pages
{
    public class LoginModel : PageModel
    {
        private readonly IUiService _uiService;

        public LoginModel(IUiService uiService)
        {
            _uiService = uiService;
        }

        [BindProperty]
        [Required(ErrorMessage = "Введите логин")]
        public string Login { get; set; } = "";

        [BindProperty]
        [Required(ErrorMessage = "Введите пароль")]
        public string Password { get; set; } = "";

        public string? ErrorMessage { get; set; }
        public string? ReturnUrl { get; set; }

        public async Task OnGetAsync(string? returnUrl = null)
        {
            ReturnUrl = returnUrl;

            // Если пользователь уже авторизован, перенаправляем
            if (User.Identity?.IsAuthenticated == true)
            {
                var redirectUrl = string.IsNullOrEmpty(ReturnUrl) ? "/Index" : ReturnUrl;
                RedirectToPage(redirectUrl);
            }
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            ReturnUrl = returnUrl;

            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                // Используем IUiService для проверки пользователя
                var developer = await _uiService.GetDeveloperByLoginAsync(Login);

                if (developer == null)
                {
                    ErrorMessage = "Пользователь не найден";
                    return Page();
                }

                // Проверка пароля через IUiService
                if (!await _uiService.VerifyPasswordAsync(Password, developer.PasswordHash))
                {
                    ErrorMessage = "Неверный пароль";
                    return Page();
                }

                // Создаем claims
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, developer.Login),
                    new Claim(ClaimTypes.NameIdentifier, developer.Id),
                    new Claim("DeveloperId", developer.Id)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                // Перенаправление
                var redirectUrl = string.IsNullOrEmpty(ReturnUrl) ? "/Index" : ReturnUrl;
                return RedirectToPage(redirectUrl);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка при входе: {ex.Message}";
                return Page();
            }
        }
    }
}