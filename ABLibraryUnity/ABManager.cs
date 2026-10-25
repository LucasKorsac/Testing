using ABLibrary.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace ABLibrary.Core
{
    public class ABManager : IABManager
    {
        private readonly ABClient _client;

        public ABManager(ABClient client)
        {
            _client = client;
        }

        public bool IsInitialized =>
            _client.IsInitialized;

        public string ApplicationId =>
            _client.ApplicationId;

        public string UserId =>
            _client.UserId;

        public string InstanceId =>
            _client.InstanceId;

        public async Task InitializeAsync(
            string applicationId,
            string userId,
            string instanceId = "",
            CancellationToken cancellationToken = default)
        {
            await _client.InitializeAsync(
                applicationId,
                userId,
                instanceId,
                cancellationToken);
        }

        public string GetVariant(string testName)
        {
            return _client.GetVariant(testName);
        }

        public async Task TrackAsync(
            string testName,
            string eventType = "conversion",
            double value = 1.0,
            CancellationToken cancellationToken = default)
        {
            await _client.TrackAsync(
                testName,
                eventType,
                value,
                cancellationToken);
        }

        public async Task FlushAsync(
            CancellationToken cancellationToken = default)
        {
            await _client.FlushAsync(
                cancellationToken);
        }
    }
}