using ABLibrary.Models;

namespace ABLibrary.Interfaces;

public interface IABTransport
{
    Task<ServerConfig> GetConfigAsync(string appId);

    Task SendEventAsync(TestEvent evt);
}