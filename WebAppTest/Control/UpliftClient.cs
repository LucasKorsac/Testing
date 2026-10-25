using System.Net.Http.Json;
using Testing.Control;
using Testing.DTO;

namespace WebAppTest.Control
{
    public class UpliftClient : IUpliftClient
    {
        private readonly HttpClient _httpClient;

        public UpliftClient(
            HttpClient httpClient
        )
        {
            _httpClient = httpClient;
        }

        public async Task<UpliftResponse> AnalyzeAsync(
            UpliftRequest request
        )
        {
            var response =
                await _httpClient.PostAsJsonAsync(
                    "/api/uplift/analyze",
                    request
                );

            response.EnsureSuccessStatusCode();

            var result =
                await response.Content
                    .ReadFromJsonAsync<UpliftResponse>();

            if (result == null)
            {
                throw new Exception(
                    "Python вернул пустой ответ"
                );
            }

            return result;
        }
    }
}