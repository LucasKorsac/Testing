using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace WebAppTest.Pages
{
    public class Registration : PageModel
    {
        // Данные формы

        [BindProperty]
        [Required(ErrorMessage = "Введите логин")]
        public string Login { get; set; } = "";

        [BindProperty]
        [Required(ErrorMessage = "Введите пароль")]
        [MinLength(6, ErrorMessage = "Минимум 6 символов")]
        public string Password { get; set; } = "";

        [BindProperty]
        [Required(ErrorMessage = "Повторите пароль")]
        public string ConfirmPassword { get; set; } = "";

        // UI

        public string Message { get; set; } = "";

        // GET

        public void OnGet()
        {
        }

        // POST

        public IActionResult OnPost()
        {
            // Проверка модели
            if (!ModelState.IsValid)
                return Page();

            // Проверка совпадения паролей
            if (Password != ConfirmPassword)
            {
                ModelState.AddModelError("", "Пароли не совпадают");

                return Page();
            }

            // TODO: сохранить в MongoDB

            Message = "Аккаунт успешно создан";

            // Очистка формы
            Login = "";
            Password = "";
            ConfirmPassword = "";

            return Page();
        }
    }
}