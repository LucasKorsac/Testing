using System.Net.Http;
using System.Net.Http.Json;
using ABLibrary.Models;

namespace WebAppTest.Control
{
    /// <summary>
    /// Клиент для работы с A/B API
    /// </summary>
    public class ApiClient
    {
        private readonly HttpClient _http;

        public ApiClient(HttpClient http)
        {
            _http = http;
        }

        public async Task<ServerConfig?> GetConfigAsync(string appId)
        {
            return await _http.GetFromJsonAsync<ServerConfig>(
                $"api/ab/config/{appId}");
        }

        public async Task SendEventAsync(TestEvent evt)
        {
            await _http.PostAsJsonAsync(
                "api/ab/event",
                evt);
        }
        public async Task<Dictionary<string, string>?> RunAsync(string appId)
        {
            var response = await _http.PostAsJsonAsync(
                "api/ab/run",
                new { AppId = appId });

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content
                .ReadFromJsonAsync<Dictionary<string, string>>();
        }
        public async Task ConvertAsync(
            string test,
            string variant,
            string userId)
        {
            await _http.PostAsJsonAsync(
                "api/ab/event",
                new TestEvent
                {
                    TestName = test,
                    Variant = variant,
                    UserId = userId,
                    EventType = "conversion"
                });
        }

        public async Task<List<object>?> GetStats(
            string testName)
        {
            return await _http.GetFromJsonAsync<List<object>>(
                $"api/ab/stats?testName={testName}");
        }
    }
}