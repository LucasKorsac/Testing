using Testing.Pattern;
using static Testing.Base.BaseMongo;

namespace WebAppTest.Control
{
    public interface IUiService
    {
        Task<Dictionary<string, string>> GetActiveTestsAsync(string appId);

        // Тесты
        Task<List<ABTests>> GetTestsAsync();
        Task<List<ABTests>> GetAllTestsAsync();
        Task<ABTests?> GetTestByIdAsync(string id);

        Task StopTestAsync(string id);
        Task ResumeTestAsync(string id);
        Task DeleteTestAsync(string id);

        Task UpdateTestAsync(
            string id,
            string name,
            string description);

        // Варианты
        Task<List<Variants>> GetVariantsAsync();
        Task<List<Variants>> GetAllVariantsAsync();

        Task<List<TestWithVariants>> GetTestsWithVariantsAsync();

        // Результаты
        Task<List<AbResults>> GetResultsAsync(string testId);

        Task<List<AbResults>> GetResultsByInstanceAsync(string instanceId);

        // Приложения
        Task<List<Applications>> GetApplicationsAsync();

        Task<Applications?> GetApplicationAsync(string id);

        Task<List<ApplicationWithInstances>>
            GetApplicationWithInstanceAsync();

        // Экземпляры
        Task<List<Instances>> GetInstancesAsync(string appId);

        Task<Instances?> GetInstanceAsync(string id);

        // Метрики
        Task<List<Metrics>> GetMetricsByApplicationAsync(string appId);

        Task<List<MetricTypes>> GetMetricTypesAsync();

        Task<List<(Metrics Metric, MetricTypes? Type)>> GetMetricsWithTypesAsync(string appId);
        Task<List<ABTests>> GetActiveTestsOnlyAsync();
        // Аналитика
        Task<int> GetTotalVariants();
    }
}