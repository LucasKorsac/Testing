using Microsoft.AspNetCore.Mvc;
using Testing.DTO;
using WebAppTest.Control;

namespace WebAppTest.Pages
{
    public class Apps : BasePageModel
    {
        public Apps(IUiService ui) : base(ui) { }

        public List<ApplicationWithInstancesDto> AppsList { get; set; } = new();

        [BindProperty]
        public CreateAppInput CreateModel { get; set; } = new();

        [BindProperty]
        public CreateInstanceInput InstanceModel { get; set; } = new();

        [BindProperty]
        public UpdateInstanceInput UpdateInstanceModel { get; set; } = new();

        public class CreateAppInput
        {
            public string Name { get; set; } = "";
            public string Description { get; set; } = "";
        }

        public class CreateInstanceInput
        {
            public string ApplicationId { get; set; } = "";
            public string Name { get; set; } = "";
            public int Version { get; set; } = 1;
        }

        public class UpdateInstanceInput
        {
            public string Id { get; set; } = "";
            public string ApplicationId { get; set; } = "";
            public string Name { get; set; } = "";
            public int Version { get; set; } = 1;
        }

        public async Task OnGetAsync()
        {
            AppsList = await _ui.GetApplicationWithInstanceAsync();
        }

        // Создание приложения
        public async Task<IActionResult> OnPostCreateAsync()
        {
            if (string.IsNullOrWhiteSpace(CreateModel.Name))
            {
                await LoadDataAsync();
                return Page();
            }

            return await SafeExecuteAsync(
                async () =>
                {
                    await _ui.CreateApplicationAsync(CreateModel.Name, CreateModel.Description);
                    ClearCache();
                },
                successMessage: $"Приложение '{CreateModel.Name}' успешно создано",
                errorMessage: "Не удалось создать приложение"
            );
        }

        // Редактирование приложения
        public async Task<IActionResult> OnPostUpdateAppAsync(string id, string name, string description)
        {
            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(name))
            {
                return RedirectToPage();
            }

            return await SafeExecuteAsync(
                async () =>
                {
                    await _ui.UpdateApplicationAsync(id, name, description);
                    ClearCache();
                },
                successMessage: "Приложение успешно обновлено",
                errorMessage: "Не удалось обновить приложение"
            );
        }

        // Удаление приложения
        public async Task<IActionResult> OnPostDeleteAsync(string id)
        {
            return await SafeExecuteAsync(
                async () => await _ui.DeleteApplicationAsync(id),
                successMessage: "Приложение успешно удалено",
                errorMessage: "Не удалось удалить приложение"
            );
        }

        // Создание экземпляра
        public async Task<IActionResult> OnPostCreateInstanceAsync()
        {
            if (string.IsNullOrWhiteSpace(InstanceModel.Name) ||
                string.IsNullOrWhiteSpace(InstanceModel.ApplicationId))
            {
                await LoadDataAsync();
                return Page();
            }

            return await SafeExecuteAsync(
                async () =>
                {
                    await _ui.CreateInstanceAsync(
                        InstanceModel.ApplicationId,
                        InstanceModel.Name,
                        InstanceModel.Version);
                    ClearCache();
                },
                successMessage: $"Экземпляр '{InstanceModel.Name}' успешно создан",
                errorMessage: "Не удалось создать экземпляр"
            );
        }

        // Редактирование экземпляра
        public async Task<IActionResult> OnPostUpdateInstanceAsync(string id, string applicationId, string name, int version)
        {
            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(name))
            {
                return RedirectToPage();
            }

            return await SafeExecuteAsync(
                async () =>
                {
                    await _ui.UpdateInstanceAsync(id, name, version);
                    ClearCache();
                },
                successMessage: "Экземпляр успешно обновлен",
                errorMessage: "Не удалось обновить экземпляр"
            );
        }

        // Удаление экземпляра
        public async Task<IActionResult> OnPostDeleteInstanceAsync(string id)
        {
            return await SafeExecuteAsync(
                async () => await _ui.DeleteInstanceAsync(id),
                successMessage: "Экземпляр успешно удален",
                errorMessage: "Не удалось удалить экземпляр"
            );
        }

        private async Task LoadDataAsync()
        {
            AppsList = await _ui.GetApplicationWithInstanceAsync();
        }
    }
}