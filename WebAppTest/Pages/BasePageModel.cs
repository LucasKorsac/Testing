using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebAppTest.Control;
using Testing.DTO;

namespace WebAppTest.Pages
{
    /// <summary> Базовый класс для всех страниц приложения </summary>
    public abstract class BasePageModel : PageModel
    {
        protected readonly IUiService _ui;

        // Кэшированные данные
        private List<ApplicationDto>? _cachedApplications;
        private List<TestDto>? _cachedTests;
        private List<VariantDto>? _cachedVariants;

        protected BasePageModel(IUiService ui)
        {
            _ui = ui;
        }

        /// <summary> Получение всех приложений с кэшированием на время запроса </summary>
        protected async Task<List<ApplicationDto>> GetApplicationsAsync(bool useCache = true)
        {
            if (useCache && _cachedApplications != null)
                return _cachedApplications;

            _cachedApplications = await _ui.GetApplicationsAsync();
            return _cachedApplications;
        }

        /// <summary> Получение всех тестов с кэшированием </summary>
        protected async Task<List<TestDto>> GetTestsAsync(bool useCache = true)
        {
            if (useCache && _cachedTests != null)
                return _cachedTests;

            _cachedTests = await _ui.GetTestsAsync();
            return _cachedTests;
        }

        /// <summary> Получение всех вариантов с кэшированием </summary>
        protected async Task<List<VariantDto>> GetVariantsAsync(bool useCache = true)
        {
            if (useCache && _cachedVariants != null)
                return _cachedVariants;

            _cachedVariants = await _ui.GetAllVariantsAsync();
            return _cachedVariants;
        }

        /// <summary> Проверка существования приложения </summary>
        protected async Task<bool> ApplicationExistsAsync(string applicationId)
        {
            if (string.IsNullOrWhiteSpace(applicationId))
                return false;

            var apps = await GetApplicationsAsync();
            return apps.Any(a => a.Id == applicationId);
        }

        /// <summary> Получение теста по ID с проверкой существования </summary>
        protected async Task<TestDto?> GetTestOrRedirectAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return null;

            var test = await _ui.GetTestByIdAsync(id);
            if (test == null)
            {
                // Установка сообщения для TempData
                TempData["ErrorMessage"] = $"Тест с ID '{id}' не найден";
                return null;
            }

            return test;
        }

        /// <summary> Получение приложения по ID с проверкой </summary>
        protected async Task<ApplicationDto?> GetApplicationOrRedirectAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return null;

            var app = await _ui.GetApplicationAsync(id);
            if (app == null)
            {
                TempData["ErrorMessage"] = $"Приложение с ID '{id}' не найдено";
                return null;
            }

            return app;
        }

        /// <summary> Добавление успешного сообщения </summary>
        protected void AddSuccessMessage(string message)
        {
            TempData["SuccessMessage"] = message;
        }

        /// <summary> Добавление сообщения об ошибке </summary>
        protected void AddErrorMessage(string message)
        {
            TempData["ErrorMessage"] = message;
        }

        /// <summary> Безопасное выполнение операции с обработкой ошибок </summary>
        protected async Task<IActionResult> SafeExecuteAsync(
            Func<Task> action,
            string successMessage = "",
            string errorMessage = "Произошла ошибка при выполнении операции",
            string redirectUrl = "")
        {
            try
            {
                await action();

                if (!string.IsNullOrWhiteSpace(successMessage))
                    AddSuccessMessage(successMessage);

                return string.IsNullOrWhiteSpace(redirectUrl)
                    ? RedirectToPage()
                    : Redirect(redirectUrl);
            }
            catch (Exception ex)
            {
                AddErrorMessage($"{errorMessage}: {ex.Message}");
                return Page();
            }
        }

        /// <summary> Сброс кэша </summary>
        protected void ClearCache()
        {
            _cachedApplications = null;
            _cachedTests = null;
            _cachedVariants = null;
        }
    }
}