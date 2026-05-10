using Testing.Base;
using static Testing.Base.BaseMongo;
using Testing.Pattern;

namespace WebAppTest.Control
{
    public interface IUiService
    {
        Task<Dictionary<string, string>> GetActiveTestsAsync(string appId);

        Task<List<ABTests>> GetTestsAsync();

        Task<List<Variants>> GetVariantsAsync();

        Task<List<TestWithVariants>> GetTestsWithVariantsAsync();
        Task<ABTests?> GetTestByIdAsync(string id);
        Task StopTestAsync(string id);
        Task DeleteTestAsync(string id);
        Task UpdateTestAsync(string id, string name, string description);
        Task<int> GetTotalVariants();
        Task<List<AbResults>> GetResultsAsync(string testId);
        Task ResumeTestAsync(string id);
    }
}