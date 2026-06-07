using ABLibrary.Models;
using Testing.DTO;
using WebAppTest.Pages;

namespace WebAppTest.Control
{
    public interface IUiService
    {
        // Тесты

        Task<Dictionary<string, string>> GetActiveTestsAsync(string appId);

        Task<List<TestDto>> GetTestsAsync();

        Task<List<TestDto>> GetAllTestsAsync();

        Task<TestDto?> GetTestByIdAsync(string id);

        Task CreateTestAsync(
            string applicationId,
            string name,
            string description);

        Task UpdateTestAsync(
            string id,
            string name,
            string description);

        Task StopTestAsync(string id);

        Task ResumeTestAsync(string id);

        Task DeleteTestAsync(string id);

        // Варианты

        Task<List<VariantDto>> GetVariantsAsync();

        Task<List<VariantDto>> GetAllVariantsAsync();

        Task<List<TestWithVariantsDto>>
            GetTestsWithVariantsAsync();

        Task DeleteVariantAsync(string id);

        // Результаты

        Task<List<AbResultsDto>>
            GetResultsAsync(string testId);

        Task<List<AbResultsDto>>
            GetResultsByInstanceAsync(string instanceId);

        // Приложения

        Task<List<ApplicationDto>>
            GetApplicationsAsync();

        Task<ApplicationDto?>
            GetApplicationAsync(string id);

        Task<List<ApplicationWithInstancesDto>>
            GetApplicationWithInstanceAsync();

        Task DeleteApplicationAsync(string id);

        // Экземпляры

        Task<List<InstanceDto>>
            GetInstancesAsync(string appId);

        Task<InstanceDto?>
            GetInstanceAsync(string id);

        // Метрики

        Task<List<MetricWithTypeDto>>
            GetMetricsWithTypesAsync(string appId);

        // Оборудование

        Task<List<EquipParamDto>>
            GetEquipParamsAsync();

        Task<List<ValueDto>>
            GetValuesByApplicationAsync(string appId);

        // Аналитика

        Task<List<TestDto>> GetActiveTestsOnlyAsync();

        Task<AnalyticDto> GetAnalyticsAsync();

        Task<int> GetTotalVariants();

        Task SaveEventAsync(TestEvent evt);

        // Аутентификация

        /// <summary> Получение разработчика по логину </summary>
        Task<DeveloperDto?> GetDeveloperByLoginAsync(string login);

        /// <summary> Регистрация нового разработчика </summary>
        Task<bool> RegisterDeveloperAsync(string login, string password);

        /// <summary> Проверка пароля </summary>
        Task<bool> VerifyPasswordAsync(string password, string hash);

        Task CreateVariantAsync(string testId, string name, string description);

        // Получение всех результатов
        Task<List<AbResultsDto>> GetAllResultsAsync();

        // Приложения
        Task CreateApplicationAsync(string name, string description);
        Task UpdateApplicationAsync(string id, string name, string description);

        // Экземпляры
        Task CreateInstanceAsync(string applicationId, string name, int version);
        Task UpdateInstanceAsync(string id, string name, int version);
        Task DeleteInstanceAsync(string id);

        // Управление стратегией теста
        Task<string?> GetTestStrategyAsync(string testId);
        Task SetTestStrategyAsync(string testId, string strategy);
        Task<List<TestsModel.MABStatRow>> GetMABStatsAsync(string testId);

    }
}