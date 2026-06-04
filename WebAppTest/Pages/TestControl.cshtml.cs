using Microsoft.AspNetCore.Mvc;
using Testing.DTO;
using WebAppTest.Control;

namespace WebAppTest.Pages
{
    public class TestControlModel : BasePageModel
    {
        public TestControlModel(IUiService ui) : base(ui) { }

        public List<ApplicationDto> Applications { get; set; } = new();
        public List<TestCard> Tests { get; set; } = new();

        [BindProperty]
        public CreateTestInput CreateModel { get; set; } = new();

        public async Task OnGetAsync()
        {
            await LoadDataAsync();
        }

        public async Task<IActionResult> OnPostCreateAsync()
        {
            if (string.IsNullOrWhiteSpace(CreateModel.Name) ||
                string.IsNullOrWhiteSpace(CreateModel.ApplicationId))
            {
                await LoadDataAsync();
                return Page();
            }

            return await SafeExecuteAsync(
                async () =>
                {
                    await _ui.CreateTestAsync(
                        CreateModel.ApplicationId,
                        CreateModel.Name,
                        CreateModel.Description);

                    if (!string.IsNullOrWhiteSpace(CreateModel.VariantsInput))
                    {
                        var variantNames = CreateModel.VariantsInput.Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(v => v.Trim())
                            .Where(v => !string.IsNullOrEmpty(v))
                            .ToList();

                        var tests = await _ui.GetTestsAsync();
                        var newTest = tests.FirstOrDefault(t => t.Name == CreateModel.Name);

                        if (newTest != null)
                        {
                            foreach (var variantName in variantNames)
                            {
                                await _ui.CreateVariantAsync(newTest.Id, variantName, "");
                            }
                        }
                    }

                    ClearCache();
                },
                successMessage: $"Тест '{CreateModel.Name}' успешно создан",
                errorMessage: "Не удалось создать тест"
            );
        }

        public async Task<IActionResult> OnPostUpdateAsync(string id, string name, string description)
        {
            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(name))
            {
                return RedirectToPage();
            }

            return await SafeExecuteAsync(
                async () =>
                {
                    await _ui.UpdateTestAsync(id, name, description);
                    ClearCache();
                },
                successMessage: "Тест успешно обновлен",
                errorMessage: "Не удалось обновить тест"
            );
        }

        public async Task<IActionResult> OnPostDeleteAsync(string id)
        {
            return await SafeExecuteAsync(
                async () => await _ui.DeleteTestAsync(id),
                successMessage: "Тест удален",
                errorMessage: "Не удалось удалить тест"
            );
        }

        public async Task<IActionResult> OnPostToggleAsync(string id)
        {
            var test = await GetTestOrRedirectAsync(id);
            if (test == null)
                return RedirectToPage();

            return await SafeExecuteAsync(
                async () =>
                {
                    if (test.Enabled)
                        await _ui.StopTestAsync(id);
                    else
                        await _ui.ResumeTestAsync(id);
                    ClearCache();
                },
                successMessage: test.Enabled ? "Тест остановлен" : "Тест запущен",
                errorMessage: "Не удалось изменить статус теста"
            );
        }

        private async Task LoadDataAsync()
        {
            Applications = await GetApplicationsAsync();
            var tests = await GetTestsAsync();
            var variants = await GetVariantsAsync();
            var allVariants = await _ui.GetAllVariantsAsync();

            // Получаем все результаты
            var allResults = await _ui.GetAllResultsAsync();

            // Создаем словарь: VariantId -> AbTestId
            var variantToTestMap = allVariants.ToDictionary(v => v.Id, v => v.AbTestId);

            Tests = new List<TestCard>();

            foreach (var test in tests)
            {
                // Находим все Id вариантов этого теста
                var testVariantIds = allVariants
                    .Where(v => v.AbTestId == test.Id)
                    .Select(v => v.Id)
                    .ToHashSet();

                // Считаем количество результатов для вариантов этого теста
                var installCount = allResults.Count(r => testVariantIds.Contains(r.VariantId));

                Tests.Add(new TestCard
                {
                    Id = test.Id,
                    Name = test.Name ?? "Без названия",
                    Description = test.Description ?? "",
                    Enabled = test.Enabled,
                    VariantsCount = variants.Count(v => v.AbTestId == test.Id),
                    InstallCount = installCount
                });
            }
        }

        public class CreateTestInput
        {
            public string ApplicationId { get; set; } = "";
            public string Name { get; set; } = "";
            public string Description { get; set; } = "";
            public string VariantsInput { get; set; } = "";
        }

        public class TestCard
        {
            public string Id { get; set; } = "";
            public string Name { get; set; } = "";
            public string Description { get; set; } = "";
            public bool Enabled { get; set; }
            public int VariantsCount { get; set; }
            public int InstallCount { get; set; }
        }
    }
}