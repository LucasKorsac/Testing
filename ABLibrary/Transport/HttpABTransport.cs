using System.Net.Http.Json;
using ABLibrary.Interfaces;
using ABLibrary.Models;

namespace ABLibrary.Transport;

public class HttpABTransport : IABTransport
{
    private readonly HttpClient _http;

    public HttpABTransport(HttpClient http)
    {
        _http = http;
    }

    public async Task<ServerConfig> GetConfigAsync(string appId)
    {
        var result = await _http.GetFromJsonAsync<ServerConfig>($"api/ab/config/{appId}");

        return result ?? new ServerConfig();
    }

    public async Task SendEventAsync(TestEvent evt)
    {
        var response =
            await _http.PostAsJsonAsync("api/ab/event", evt);

        response.EnsureSuccessStatusCode();
    }
}