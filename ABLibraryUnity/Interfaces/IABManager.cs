using System.Threading;
using System.Threading.Tasks;

namespace ABLibrary.Interfaces
{
    public interface IABManager
    {
        bool IsInitialized { get; }

        string ApplicationId { get; }

        string UserId { get; }

        string InstanceId { get; }

        Task InitializeAsync(
            string applicationId,
            string userId,
            string instanceId = "",
            CancellationToken cancellationToken = default);

        string GetVariant(string testName);

        Task TrackAsync(
            string testName,
            string eventType = "conversion",
            double value = 1.0,
            CancellationToken cancellationToken = default);

        Task FlushAsync(
            CancellationToken cancellationToken = default);
    }
}