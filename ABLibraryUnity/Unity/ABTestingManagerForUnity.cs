using ABLibrary.Core;
using ABLibrary.Interfaces;
using ABLibrary.Transport;
using System;
using System.Threading;
using System.Threading.Tasks;
//using static System.Net.Mime.MediaTypeNames;

namespace ABLibraryUnity.Unity
{
    public sealed class ABTestingManagerForUnity : IABManager, IDisposable
    {
        private readonly ABManager _manager;
        private readonly HttpABTransport _transport;

        public bool IsInitialized =>
            _manager.IsInitialized;

        public string ApplicationId =>
            _manager.ApplicationId;

        public string UserId =>
            _manager.UserId;

        public string InstanceId =>
            _manager.InstanceId;

        public ABTestingManagerForUnity(
            string apiUrl,
            ABOptions options = null)
        {
            if (string.IsNullOrWhiteSpace(apiUrl))
            {
                throw new ArgumentException(
                    "API URL cannot be empty.",
                    nameof(apiUrl));
            }

            _transport = new HttpABTransport(
                apiUrl,
                options != null
                    ? TimeSpan.FromSeconds(
                        options.RequestTimeoutSeconds)
                    : TimeSpan.FromSeconds(10));

            var storage =
                new UnityFileStorage(
                    Application.persistentDataPath);

            var client =
                new ABClient(
                    _transport,
                    storage,
                    options);

            _manager =
                new ABManager(client);
        }

        public Task InitializeAsync(
            string applicationId,
            string userId,
            string instanceId = "",
            CancellationToken cancellationToken = default)
        {
            return _manager.InitializeAsync(
                applicationId,
                userId,
                instanceId,
                cancellationToken);
        }

        public string GetVariant(
            string testName)
        {
            return _manager.GetVariant(
                testName);
        }

        //public Task TrackAsync(
        //    string testName,
        //    string eventType = "conversion",
        //    double? value = null,
        //    CancellationToken cancellationToken = default)
        //{
        //    return _manager.TrackAsync(
        //        testName,
        //        eventType,
        //        value,
        //        cancellationToken);
        //}

        public Task TrackAsync(string testName, string eventType = "conversion", double value = 0,
        CancellationToken cancellationToken = default)
        {
            return _manager.TrackAsync(
                testName,
                eventType,
                value,
                cancellationToken);
        }

        public Task FlushAsync(
            CancellationToken cancellationToken = default)
        {
            return _manager.FlushAsync(
                cancellationToken);
        }

        public void Dispose()
        {
            _transport.Dispose();
        }
    }
}