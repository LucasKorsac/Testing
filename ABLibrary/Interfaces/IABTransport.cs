using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ABLibrary.Models;

namespace ABLibrary.Interfaces
{
    public interface IABTransport
    {
        Task<ServerConfig> GetConfigAsync(
            string applicationId,
            string userId,
            string instanceId,
            CancellationToken cancellationToken = default);

        Task<EventBatchResponse> SendEventsAsync(
            IReadOnlyCollection<TestEvent> events,
            CancellationToken cancellationToken = default);
    }
}