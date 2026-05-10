using System.Net.Http;
using System.Net.Http.Json;

namespace WebAppTest.Control
{
    /// <summary> Клиент для работы с A/B API </summary>
    public class ApiClient
    {
        private readonly HttpClient _http;

        public ApiClient(HttpClient http)
        {
            _http = http;
        }

        public async Task<Dictionary<string, string>?> RunAsync(string appId)
        {
            var response = await _http.PostAsJsonAsync(
                "api/ab/run",
                new { AppId = appId });

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        }

        public async Task ConvertAsync(string test, string variant, string userId)
        {
            await _http.PostAsJsonAsync(
                "api/ab/convert",
                new { TestName = test, VariantName = variant, UserId = userId });
        }

        public async Task<List<object>?> GetStats(string testName)
        {
            return await _http.GetFromJsonAsync<List<object>>(
                $"api/ab/stats?testName={testName}");
        }
    }
}