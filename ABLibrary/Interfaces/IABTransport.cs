using ABLibrary.Models;
using System.Threading.Tasks;

namespace ABLibrary.Interfaces
{

    public interface IABTransport
    {
        //Task<ServerConfig> GetConfigAsync(string appId);
        Task<ServerConfig> GetConfigAsync(string appId, string instanceId = "");
        Task SendEventAsync(TestEvent evt);
    }
}