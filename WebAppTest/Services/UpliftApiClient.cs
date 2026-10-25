using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WebAppTest.Services;

public sealed class UpliftApiClient
{
    private readonly HttpClient _httpClient;

    public UpliftApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<JsonElement> AnalyzeAsync(
        object request,
        CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.PostAsJsonAsync(
            "api/uplift/analyze",
            request,
            cancellationToken);

        var body = await response.Content.ReadFromJsonAsync<JsonElement>(
            cancellationToken: cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var error = body.ValueKind == JsonValueKind.Object
                && body.TryGetProperty("error", out var errorValue)
                    ? errorValue.GetString()
                    : "Python API вернул ошибку";

            throw new HttpRequestException(
                error ?? "Не удалось выполнить uplift-анализ");
        }

        if (body.ValueKind != JsonValueKind.Object
            || !body.TryGetProperty("success", out var success)
            || success.ValueKind != JsonValueKind.True)
        {
            throw new InvalidOperationException(
                "Python API не подтвердил успешный расчёт");
        }

        return body;
    }
}