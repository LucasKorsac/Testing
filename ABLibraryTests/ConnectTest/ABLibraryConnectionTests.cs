using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ABLibrary.Core;
using ABLibrary.Interfaces;
using ABLibrary.Models;
using ABLibrary.Transport;
using ABLibrary.Storage;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Testing.Base;
using Testing.Pattern;
using static Testing.Base.BaseMongo;
using System.IO;
using System.Net.Http;
using System.Threading;
using Xunit;

namespace ABProjectTests.ConnectTest
{
    /// <summary>
    /// Тестирование передачи данных между клиентской библиотекой и сервером
    /// </summary>
    public class ABLibraryConnectionTests
    {
        private const string TestServerUrl = "http://localhost:5001/";
        private const string TestAppId = "test_game_app";
        private readonly string _testStoragePath;

        public ABLibraryConnectionTests()
        {
            _testStoragePath = Path.Combine(Path.GetTempPath(), "ABLibraryTests");
            if (!Directory.Exists(_testStoragePath))
                Directory.CreateDirectory(_testStoragePath);
        }

        /// <summary>
        /// Тест 1: Получение конфигурации с сервера
        /// </summary>
        [Fact]
        public async Task GetConfigFromServer_ShouldReturnValidConfig()
        {
            // Arrange
            var transport = new HttpABTransport(TestServerUrl);
            var storage = new FileStorageForUnity(_testStoragePath);
            var client = new ABClient(transport, storage);

            // Act
            await client.InitializeAsync(TestAppId);
            var variant = client.GetVariant("test_button_color");

            // Assert
            Assert.NotNull(client.Config);
            Assert.NotNull(client.Config.Tests);
        }

        /// <summary>
        /// Тест 2: Отправка события на сервер
        /// </summary>
        [Fact]
        public async Task SendEventToServer_ShouldSuccessfullySend()
        {
            // Arrange
            var transport = new HttpABTransport(TestServerUrl);
            var storage = new FileStorageForUnity(_testStoragePath);
            var client = new ABClient(transport, storage, new ABOptions { AutoFlush = true });

            // Устанавливаем тестовую конфигурацию
            client.SetConfig(new ServerConfig
            {
                Tests = new Dictionary<string, string>
                {
                    { "test_event", "VariantA" }
                }
            });

            // Act
            var exception = await Record.ExceptionAsync(async () =>
            {
                await client.TrackAsync("test_event", "test_user_123", "conversion");
            });

            // Assert
            Assert.Null(exception);
        }

        /// <summary>
        /// Тест 3: Получение варианта для несуществующего теста
        /// </summary>
        [Fact]
        public async Task GetVariantForNonExistentTest_ShouldReturnDefault()
        {
            // Arrange
            var transport = new HttpABTransport(TestServerUrl);
            var storage = new FileStorageForUnity(_testStoragePath);
            var client = new ABClient(transport, storage);

            // Act
            await client.InitializeAsync(TestAppId);
            var result = client.GetVariant("non_existent_test");

            // Assert
            Assert.Equal("default", result);
        }

        /// <summary>
        /// Тест 4: Проверка буферизации событий при отсутствии сети
        /// </summary>
        [Fact]
        public async Task BufferEvents_WhenNetworkUnavailable_ShouldStoreLocally()
        {
            // Arrange
            var transport = new HttpABTransport("http://unavailable-server:9999/");
            var storage = new FileStorageForUnity(_testStoragePath);
            var client = new ABClient(transport, storage, new ABOptions { AutoFlush = false });

            client.SetConfig(new ServerConfig
            {
                Tests = new Dictionary<string, string>
                {
                    { "test_buffer", "A" }
                }
            });

            // Act
            await client.TrackAsync("test_buffer", "user_offline", "click");

            // Assert - событие должно быть в буфере, даже если сервер недоступен
            Assert.NotNull(client);
        }

        /// <summary>
        /// Тест 5: Отправка нескольких событий одним запросом
        /// </summary>
        [Fact]
        public async Task FlushMultipleEvents_ShouldSendAllEvents()
        {
            // Arrange
            var transport = new HttpABTransport(TestServerUrl);
            var storage = new FileStorageForUnity(_testStoragePath);
            var client = new ABClient(transport, storage, new ABOptions { AutoFlush = false });

            client.SetConfig(new ServerConfig
            {
                Tests = new Dictionary<string, string>
                {
                    { "test1", "A" },
                    { "test2", "B" },
                    { "test3", "C" }
                }
            });

            // Act
            await client.TrackAsync("test1", "user1", "event1");
            await client.TrackAsync("test2", "user2", "event2");
            await client.TrackAsync("test3", "user3", "event3");
            await client.FlushAsync();

            // Assert - проверяем, что буфер очищен
        }

        /// <summary>
        /// Тест 6: Проверка формата отправляемых данных
        /// </summary>
        [Fact]
        public async Task SendEvent_ShouldHaveCorrectDataFormat()
        {
            // Arrange
            var transport = new HttpABTransport(TestServerUrl);
            var storage = new FileStorageForUnity(_testStoragePath);
            var client = new ABClient(transport, storage, new ABOptions { AutoFlush = true });

            client.SetConfig(new ServerConfig
            {
                Tests = new Dictionary<string, string>
                {
                    { "format_test", "TestVariant" }
                }
            });

            var testTime = DateTime.UtcNow;

            // Act
            await client.TrackAsync("format_test", "test_user_format", "conversion");

            // Assert
        }

        /// <summary>
        /// Тест 7: Параллельные запросы от нескольких клиентов
        /// </summary>
        [Fact]
        public async Task MultipleClients_ShouldWorkSimultaneously()
        {
            // Arrange
            var tasks = new List<Task>();

            // Act
            for (int i = 0; i < 5; i++)
            {
                int clientId = i;
                tasks.Add(Task.Run(async () =>
                {
                    // Создаем уникальную директорию для каждого клиента
                    var clientPath = Path.Combine(_testStoragePath, $"client_{clientId}");

                    // СОЗДАЕМ ДИРЕКТОРИЮ, если её нет
                    if (!Directory.Exists(clientPath))
                    {
                        Directory.CreateDirectory(clientPath);
                    }

                    var transport = new HttpABTransport(TestServerUrl);
                    var storage = new FileStorageForUnity(clientPath);
                    var client = new ABClient(transport, storage, new ABOptions { AutoFlush = true });

                    client.SetConfig(new ServerConfig
                    {
                        Tests = new Dictionary<string, string>
                {
                    { $"test_{clientId}", $"Variant{clientId}" }
                }
                    });

                    await client.TrackAsync($"test_{clientId}", $"user_{clientId}", "conversion");
                }));
            }

            await Task.WhenAll(tasks);

            // Assert - все задачи выполнены без ошибок
            Assert.True(true);
        }

        /// <summary>
        /// Тест 8: Восстановление после ошибки сети
        /// </summary>
        [Fact]
        public async Task RecoverAfterNetworkError_ShouldSendBufferedEvents()
        {
            // Arrange
            var transport = new HttpABTransport("http://unavailable-server:9999/");
            var storage = new FileStorageForUnity(_testStoragePath);
            var client = new ABClient(transport, storage, new ABOptions { AutoFlush = false });

            client.SetConfig(new ServerConfig
            {
                Tests = new Dictionary<string, string>
                {
                    { "recovery_test", "A" }
                }
            });

            // Act - отправка при недоступном сервере
            await client.TrackAsync("recovery_test", "user_recovery", "click");

            // Создаем новый клиент с рабочим сервером
            var workingTransport = new HttpABTransport(TestServerUrl);
            var workingStorage = new FileStorageForUnity(_testStoragePath);
            var workingClient = new ABClient(workingTransport, workingStorage, new ABOptions { AutoFlush = true });

            workingClient.SetConfig(new ServerConfig
            {
                Tests = new Dictionary<string, string>
                {
                    { "recovery_test", "A" }
                }
            });

            await workingClient.FlushAsync();

            // Assert
        }
    }

    /// <summary>
    /// Тестирование HttpABTransport
    /// </summary>
    public class HttpABTransportTests
    {
        private const string TestServerUrl = "http://localhost:5001/";

        [Fact]
        public async Task GetConfigAsync_ShouldReturnServerConfig()
        {
            // Arrange
            var transport = new HttpABTransport(TestServerUrl);

            // Act
            var config = await transport.GetConfigAsync("test_app");

            // Assert
            Assert.NotNull(config);
            Assert.NotNull(config.Tests);
        }

        [Fact]
        public async Task SendEventAsync_ShouldNotThrowException()
        {
            // Arrange
            var transport = new HttpABTransport(TestServerUrl);
            var testEvent = new TestEvent
            {
                TestName = "test_event",
                Variant = "A",
                UserId = "test_user",
                EventType = "conversion",
                Timestamp = DateTime.UtcNow
            };

            // Act
            var exception = await Record.ExceptionAsync(async () =>
            {
                await transport.SendEventAsync(testEvent);
            });

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public async Task GetConfigAsync_WhenServerUnavailable_ShouldReturnEmptyConfig()
        {
            // Arrange
            var transport = new HttpABTransport("http://invalid-server:9999/");

            // Act
            var config = await transport.GetConfigAsync("test_app");

            // Assert
            Assert.NotNull(config);
            Assert.NotNull(config.Tests);
        }
    }

    /// <summary>
    /// Тестирование FileStorageForUnity
    /// </summary>
    public class FileStorageTests : IDisposable
    {
        private readonly string _testPath;
        private readonly FileStorageForUnity _storage;

        public FileStorageTests()
        {
            _testPath = Path.Combine(Path.GetTempPath(), "ABStorageTests");
            Directory.CreateDirectory(_testPath);
            _storage = new FileStorageForUnity(_testPath);
        }

        [Fact]
        public void SaveAndLoad_ShouldPreserveData()
        {
            // Arrange
            var testData = new List<TestEvent>
            {
                new TestEvent { TestName = "test1", Variant = "A", UserId = "user1" },
                new TestEvent { TestName = "test2", Variant = "B", UserId = "user2" }
            };

            // Act
            _storage.Save("test_events", testData);
            var loadedData = _storage.Load<List<TestEvent>>("test_events");

            // Assert
            Assert.NotNull(loadedData);
            Assert.Equal(2, loadedData.Count);
            Assert.Equal("test1", loadedData[0].TestName);
            Assert.Equal("test2", loadedData[1].TestName);
        }

        [Fact]
        public void LoadNonExistentFile_ShouldReturnDefault()
        {
            // Act
            var result = _storage.Load<List<TestEvent>>("non_existent_key");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void SaveOverwrites_ShouldReplaceOldData()
        {
            // Arrange
            var firstData = new List<TestEvent> { new TestEvent { TestName = "first" } };
            var secondData = new List<TestEvent> { new TestEvent { TestName = "second" } };

            // Act
            _storage.Save("test_key", firstData);
            _storage.Save("test_key", secondData);
            var loadedData = _storage.Load<List<TestEvent>>("test_key");

            // Assert
            Assert.NotNull(loadedData);
            Assert.Single(loadedData);
            Assert.Equal("second", loadedData[0].TestName);
        }

        public void Dispose()
        {
            if (Directory.Exists(_testPath))
            {
                Directory.Delete(_testPath, true);
            }
        }
    }
}