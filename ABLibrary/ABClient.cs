using ABLibrary.Interfaces;
using ABLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ABLibrary
{
    public class ABClient
    {
        private readonly IABTransport _transport;
        private readonly ILocalStorage _storage;
        private readonly ABOptions _options;

        private readonly object _lock = new object();

        private List<TestEvent> _buffer;

        private ServerConfig _config;

        private string _applicationId = "";
        private string _userId = "";
        private string _instanceId = "";

        public bool IsInitialized { get; private set; }

        public string ApplicationId => _applicationId;

        public string UserId => _userId;

        public string InstanceId => _instanceId;

        public ServerConfig Config
        {
            get
            {
                lock (_lock)
                {
                    return _config;
                }
            }
        }

        public ABClient(
            IABTransport transport,
            ILocalStorage storage,
            ABOptions options = null)
        {
            _transport = transport
                ?? throw new ArgumentNullException(nameof(transport));

            _storage = storage
                ?? throw new ArgumentNullException(nameof(storage));

            _options = options ?? new ABOptions();

            _buffer =
                _storage.Load<List<TestEvent>>(
                    _options.StorageKey)
                ?? new List<TestEvent>();

            _config =
                _storage.Load<ServerConfig>(
                    _options.StorageKey + "_config")
                ?? new ServerConfig();
        }

        public async Task InitializeAsync(
            string applicationId,
            string userId,
            string instanceId = "",
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(applicationId))
                throw new ArgumentException(
                    "ApplicationId cannot be empty.",
                    nameof(applicationId));

            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException(
                    "UserId cannot be empty.",
                    nameof(userId));

            _applicationId = applicationId;
            _userId = userId;
            _instanceId = instanceId ?? "";

            try
            {
                var config =
                    await _transport.GetConfigAsync(
                        applicationId,
                        userId,
                        _instanceId,
                        cancellationToken);

                if (config != null)
                {
                    lock (_lock)
                    {
                        _config = config;
                    }

                    _storage.Save(
                        _options.StorageKey + "_config",
                        config);
                }

                IsInitialized = true;
            }
            catch
            {
                var savedConfig =
                    _storage.Load<ServerConfig>(
                        _options.StorageKey + "_config");

                if (savedConfig != null)
                {
                    lock (_lock)
                    {
                        _config = savedConfig;
                    }

                    IsInitialized = true;
                }
                else
                {
                    IsInitialized = false;
                    throw;
                }
            }
        }

        public string GetVariant(string testName)
        {
            if (string.IsNullOrWhiteSpace(testName))
                return "default";

            lock (_lock)
            {
                if (_config?.Tests == null)
                    return "default";

                var test =
                    _config.Tests.FirstOrDefault(
                        x => string.Equals(
                            x.TestName,
                            testName,
                            StringComparison.OrdinalIgnoreCase));

                if (test == null)
                    return "default";

                if (!test.Enabled)
                    return "default";

                return string.IsNullOrWhiteSpace(test.Variant)
                    ? "default"
                    : test.Variant;
            }
        }

        public async Task TrackAsync(
            string testName,
            string eventType = "conversion",
            double value = 1.0,
            CancellationToken cancellationToken = default)
        {
            if (!IsInitialized)
                return;

            var variant = GetVariant(testName);

            if (variant == "default")
                return;

            var evt = new TestEvent
            {
                EventId = Guid.NewGuid().ToString("N"),

                ApplicationId = _applicationId,

                TestName = testName,

                Variant = variant,

                UserId = _userId,

                InstanceId = _instanceId,

                EventType = string.IsNullOrWhiteSpace(eventType)
                    ? "conversion"
                    : eventType,

                Value = value,

                Timestamp = DateTime.UtcNow
            };

            bool shouldFlush;

            lock (_lock)
            {
                _buffer.Add(evt);

                if (_buffer.Count > _options.MaxBufferSize)
                {
                    _buffer =
                        _buffer
                            .Skip(
                                _buffer.Count -
                                _options.MaxBufferSize)
                            .ToList();
                }

                SaveBuffer();

                shouldFlush =
                    _options.AutoFlush ||
                    _buffer.Count >= _options.BatchSize;
            }

            if (shouldFlush)
            {
                await FlushAsync(cancellationToken);
            }
        }

        public async Task FlushAsync(
            CancellationToken cancellationToken = default)
        {
            List<TestEvent> eventsToSend;

            lock (_lock)
            {
                if (_buffer.Count == 0)
                    return;

                eventsToSend =
                    _buffer.ToList();

                _buffer.Clear();

                SaveBuffer();
            }

            try
            {
                var response =
                    await _transport.SendEventsAsync(
                        eventsToSend,
                        cancellationToken);

                if (response == null || !response.Success)
                {
                    RestoreEvents(eventsToSend);
                    return;
                }

                if (response.Failed > 0)
                {
                    RestoreEvents(eventsToSend);
                }
            }
            catch
            {
                RestoreEvents(eventsToSend);
                throw;
            }
        }

        private void RestoreEvents(
            IEnumerable<TestEvent> events)
        {
            lock (_lock)
            {
                _buffer.InsertRange(
                    0,
                    events);

                if (_buffer.Count > _options.MaxBufferSize)
                {
                    _buffer =
                        _buffer
                            .Take(_options.MaxBufferSize)
                            .ToList();
                }

                SaveBuffer();
            }
        }

        private void SaveBuffer()
        {
            _storage.Save(
                _options.StorageKey,
                _buffer);
        }
    }
}