namespace ABLibrary.Core;

public class ABManager
{
    private readonly ABClient _client;

    public ABManager(ABClient client)
    {
        _client = client;
    }

    public async Task InitAsync(string appId)
    {
        await _client.InitializeAsync(appId);
    }

    public string GetVariant(string testName)
    {
        return _client.GetVariant(testName);
    }

    public async Task TrackAsync(string testName, string userId, string eventType = "conversion")
    {
        await _client.TrackAsync(testName, userId, eventType);
    }

    public async Task FlushAsync()
    {
        await _client.FlushAsync();
    }
}