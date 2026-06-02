using ABLibrary.Interfaces;
using ABLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ABLibrary.Core
{

    public class ABClient
    {
        private readonly IABTransport _transport;

        private readonly ILocalStorage _storage;

        private readonly ABOptions _options;

        private readonly List<TestEvent> _buffer;

        public ServerConfig Config { get; private set; } = new ServerConfig();

        public void SetConfig(ServerConfig config)
        {
            Config = config;
        }

        public ABClient(IABTransport transport, ILocalStorage storage, ABOptions options = null)
        {
            _transport = transport;
            _storage = storage;
            _options = options ?? new ABOptions();

            _buffer = _storage.Load<List<TestEvent>>(_options.StorageKey) ?? new List<TestEvent>();
        }

        public async Task InitializeAsync(string appId)
        {
            try
            {
                Config = await _transport.GetConfigAsync(appId);
            }
            catch
            {
                // offline mode
            }
        }

        public string GetVariant(string testName)
        {
            return Config.Tests.TryGetValue(testName, out var variant) ? variant : "default";
        }

        public async Task TrackAsync(string testName, string userId, string eventType = "conversion")
        {
            var evt = new TestEvent
            {
                TestName = testName,
                Variant = GetVariant(testName),
                UserId = userId,
                EventType = eventType,
                Timestamp = DateTime.UtcNow
            };

            _buffer.Add(evt);

            SaveBuffer();

            if (_options.AutoFlush)
            {
                await FlushAsync();
            }
        }

        public async Task FlushAsync()
        {
            foreach (var evt in _buffer.ToList())
            {
                try
                {
                    await _transport.SendEventAsync(evt);

                    _buffer.Remove(evt);
                }
                catch
                {
                    // keep in queue
                }
            }

            SaveBuffer();
        }

        private void SaveBuffer()
        {
            _storage.Save(_options.StorageKey, _buffer);
        }
    }
}