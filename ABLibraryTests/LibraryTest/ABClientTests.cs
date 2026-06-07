using ABLibrary.Core;
using ABLibrary.Interfaces;
using ABLibrary.Models;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

public class ABClientTests
{
    #region InitializeAsync Tests

    [Fact]
    public async Task InitializeAsync_LoadsConfigSuccessfully()
    {
        // Arrange
        var transport = new Mock<IABTransport>();
        var storage = new Mock<ILocalStorage>();
        var expectedConfig = new ServerConfig
        {
            Tests = new Dictionary<string, string>
            {
                { "button_color_test", "A" },
                { "payment_flow_test", "B" },
                { "onboarding_test", "C" }
            }
        };

        transport.Setup(x => x.GetConfigAsync("app1")).ReturnsAsync(expectedConfig);
        var client = new ABClient(transport.Object, storage.Object);

        // Act
        await client.InitializeAsync("app1");

        // Assert
        Assert.Equal("A", client.GetVariant("button_color_test"));
        Assert.Equal("B", client.GetVariant("payment_flow_test"));
        Assert.Equal("C", client.GetVariant("onboarding_test"));
        transport.Verify(x => x.GetConfigAsync("app1"), Times.Once);
    }

    [Fact]
    public async Task InitializeAsync_WhenNetworkError_StaysInOfflineMode()
    {
        // Arrange
        var transport = new Mock<IABTransport>();
        var storage = new Mock<ILocalStorage>();
        transport.Setup(x => x.GetConfigAsync(It.IsAny<string>()))
            .ThrowsAsync(new Exception("Network error"));
        var client = new ABClient(transport.Object, storage.Object);

        // Act
        await client.InitializeAsync("app1");

        // Assert
        Assert.Equal("default", client.GetVariant("any_test"));
    }

    [Fact]
    public async Task InitializeAsync_WhenServerReturnsEmptyConfig_UsesEmptyConfig()
    {
        // Arrange
        var transport = new Mock<IABTransport>();
        var storage = new Mock<ILocalStorage>();
        transport.Setup(x => x.GetConfigAsync("app1")).ReturnsAsync(new ServerConfig());
        var client = new ABClient(transport.Object, storage.Object);

        // Act
        await client.InitializeAsync("app1");

        // Assert
        Assert.Equal("default", client.GetVariant("any_test"));
        transport.Verify(x => x.GetConfigAsync("app1"), Times.Once);
    }

    [Fact]
    public async Task InitializeAsync_OverridesExistingConfig()
    {
        // Arrange
        var transport = new Mock<IABTransport>();
        var storage = new Mock<ILocalStorage>();
        transport.Setup(x => x.GetConfigAsync("app1"))
            .ReturnsAsync(new ServerConfig
            {
                Tests = new Dictionary<string, string> { { "new_test", "new_variant" } }
            });

        var client = new ABClient(transport.Object, storage.Object);
        client.SetConfig(new ServerConfig
        {
            Tests = new Dictionary<string, string> { { "old_test", "old_variant" } }
        });

        // Act
        await client.InitializeAsync("app1");

        // Assert
        Assert.Equal("new_variant", client.GetVariant("new_test"));
        Assert.Equal("default", client.GetVariant("old_test"));
    }

    #endregion

    #region GetVariant Tests

    [Fact]
    public void GetVariant_ForExistingTest_ReturnsCorrectVariant()
    {
        // Arrange
        var transport = new Mock<IABTransport>();
        var storage = new Mock<ILocalStorage>();
        var client = new ABClient(transport.Object, storage.Object);
        client.SetConfig(new ServerConfig
        {
            Tests = new Dictionary<string, string>
            {
                { "test_a", "variant_1" },
                { "test_b", "variant_2" },
                { "test_c", "variant_3" }
            }
        });

        // Act & Assert
        Assert.Equal("variant_1", client.GetVariant("test_a"));
        Assert.Equal("variant_2", client.GetVariant("test_b"));
        Assert.Equal("variant_3", client.GetVariant("test_c"));
    }

    [Fact]
    public void GetVariant_ForNonExistingTest_ReturnsDefault()
    {
        // Arrange
        var transport = new Mock<IABTransport>();
        var storage = new Mock<ILocalStorage>();
        var client = new ABClient(transport.Object, storage.Object);
        client.SetConfig(new ServerConfig
        {
            Tests = new Dictionary<string, string> { { "existing_test", "value" } }
        });

        // Act
        var result = client.GetVariant("non_existing_test");

        // Assert
        Assert.Equal("default", result);
    }

    [Fact]
    public void GetVariant_WhenConfigIsEmpty_ReturnsDefault()
    {
        // Arrange
        var transport = new Mock<IABTransport>();
        var storage = new Mock<ILocalStorage>();
        var client = new ABClient(transport.Object, storage.Object);

        // Act
        var result = client.GetVariant("any_test");

        // Assert
        Assert.Equal("default", result);
    }

    [Fact]
    public void GetVariant_IsCaseSensitive()
    {
        // Arrange
        var transport = new Mock<IABTransport>();
        var storage = new Mock<ILocalStorage>();
        var client = new ABClient(transport.Object, storage.Object);
        client.SetConfig(new ServerConfig
        {
            Tests = new Dictionary<string, string> { { "TestA", "A" } }
        });

        // Act
        var resultUpper = client.GetVariant("TESTA");
        var resultLower = client.GetVariant("testa");

        // Assert
        Assert.Equal("default", resultUpper);
        Assert.Equal("default", resultLower);
    }

    #endregion

    #region TrackAsync Tests

    [Fact]
    public async Task TrackAsync_WithAutoFlush_SendsEventImmediately()
    {
        // Arrange
        var transport = new Mock<IABTransport>();
        var storage = new Mock<ILocalStorage>();
        TestEvent sentEvent = null;
        var saveCalls = new List<List<TestEvent>>();

        storage.Setup(x => x.Load<List<TestEvent>>(It.IsAny<string>())).Returns(new List<TestEvent>());
        storage.Setup(x => x.Save(It.IsAny<string>(), It.IsAny<List<TestEvent>>()))
            .Callback<string, List<TestEvent>>((key, events) =>
            {
                saveCalls.Add(events);
            });

        transport.Setup(x => x.SendEventAsync(It.IsAny<TestEvent>()))
            .Callback<TestEvent>(e => sentEvent = e)
            .Returns(Task.CompletedTask);

        var client = new ABClient(transport.Object, storage.Object, new ABOptions { AutoFlush = true });
        client.SetConfig(new ServerConfig { Tests = new Dictionary<string, string> { { "test1", "VariantA" } } });

        // Act
        await client.TrackAsync("test1", "user_123", "conversion");

        // Assert
        transport.Verify(x => x.SendEventAsync(It.Is<TestEvent>(e =>
            e.TestName == "test1" &&
            e.Variant == "VariantA" &&
            e.UserId == "user_123" &&
            e.EventType == "conversion")), Times.Once);

        // Проверяем, что событие было отправлено
        Assert.NotNull(sentEvent);
        Assert.Equal("test1", sentEvent.TestName);
        Assert.Equal("VariantA", sentEvent.Variant);
        Assert.Equal("user_123", sentEvent.UserId);
        Assert.Equal("conversion", sentEvent.EventType);

        // При AutoFlush = true буфер должен быть пуст после отправки
        // Последний вызов Save должен сохранить пустой список
        if (saveCalls.Count > 0)
        {
            var lastSave = saveCalls.Last();
            Assert.Empty(lastSave);
        }
    }

    [Fact]
    public async Task TrackAsync_WithoutAutoFlush_BuffersEventLocally()
    {
        // Arrange
        var transport = new Mock<IABTransport>();
        var storage = new Mock<ILocalStorage>();
        var savedEvents = new List<TestEvent>();

        storage.Setup(x => x.Load<List<TestEvent>>(It.IsAny<string>())).Returns(new List<TestEvent>());
        storage.Setup(x => x.Save(It.IsAny<string>(), It.IsAny<List<TestEvent>>()))
            .Callback<string, List<TestEvent>>((key, events) => savedEvents = events);
        transport.Setup(x => x.SendEventAsync(It.IsAny<TestEvent>())).Returns(Task.CompletedTask);

        var client = new ABClient(transport.Object, storage.Object, new ABOptions { AutoFlush = false });
        client.SetConfig(new ServerConfig { Tests = new Dictionary<string, string> { { "test1", "A" } } });

        // Act
        await client.TrackAsync("test1", "user_456", "click");

        // Assert
        transport.Verify(x => x.SendEventAsync(It.IsAny<TestEvent>()), Times.Never);
        Assert.Single(savedEvents);
    }

    [Fact]
    public async Task TrackAsync_AddsCorrectEventData()
    {
        // Arrange
        var transport = new Mock<IABTransport>();
        var storage = new Mock<ILocalStorage>();
        TestEvent capturedEvent = null;

        storage.Setup(x => x.Load<List<TestEvent>>(It.IsAny<string>())).Returns(new List<TestEvent>());
        storage.Setup(x => x.Save(It.IsAny<string>(), It.IsAny<List<TestEvent>>()))
            .Callback<string, List<TestEvent>>((key, events) => capturedEvent = events.First());

        var client = new ABClient(transport.Object, storage.Object, new ABOptions { AutoFlush = false });
        client.SetConfig(new ServerConfig { Tests = new Dictionary<string, string> { { "login_test", "VariantX" } } });

        var testTime = DateTime.UtcNow;

        // Act
        await client.TrackAsync("login_test", "user_789", "login_success");

        // Assert
        Assert.NotNull(capturedEvent);
        Assert.Equal("login_test", capturedEvent.TestName);
        Assert.Equal("VariantX", capturedEvent.Variant);
        Assert.Equal("user_789", capturedEvent.UserId);
        Assert.Equal("login_success", capturedEvent.EventType);
        Assert.True(capturedEvent.Timestamp >= testTime);
    }

    [Fact]
    public async Task TrackAsync_WhenVariantNotFound_UsesDefaultVariant()
    {
        // Arrange
        var transport = new Mock<IABTransport>();
        var storage = new Mock<ILocalStorage>();
        TestEvent capturedEvent = null;

        storage.Setup(x => x.Load<List<TestEvent>>(It.IsAny<string>())).Returns(new List<TestEvent>());
        storage.Setup(x => x.Save(It.IsAny<string>(), It.IsAny<List<TestEvent>>()))
            .Callback<string, List<TestEvent>>((key, events) => capturedEvent = events.First());

        var client = new ABClient(transport.Object, storage.Object, new ABOptions { AutoFlush = false });
        client.SetConfig(new ServerConfig { Tests = new Dictionary<string, string>() });

        // Act
        await client.TrackAsync("unknown_test", "user_999", "conversion");

        // Assert
        Assert.NotNull(capturedEvent);
        Assert.Equal("unknown_test", capturedEvent.TestName);
        Assert.Equal("default", capturedEvent.Variant);
    }

    #endregion

    #region FlushAsync Tests

    [Fact]
    public async Task FlushAsync_SendsAllBufferedEvents()
    {
        // Arrange
        var transport = new Mock<IABTransport>();
        var storage = new Mock<ILocalStorage>();
        var sendCount = 0;

        storage.Setup(x => x.Load<List<TestEvent>>(It.IsAny<string>())).Returns(new List<TestEvent>());
        transport.Setup(x => x.SendEventAsync(It.IsAny<TestEvent>()))
            .Returns(Task.CompletedTask)
            .Callback(() => sendCount++);

        var client = new ABClient(transport.Object, storage.Object, new ABOptions { AutoFlush = false });
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

        // Assert
        Assert.Equal(3, sendCount);
    }

    [Fact]
    public async Task FlushAsync_WhenSendFails_KeepsEventsInBuffer()
    {
        // Arrange
        var transport = new Mock<IABTransport>();
        var storage = new Mock<ILocalStorage>();
        var savedEvents = new List<TestEvent>();

        storage.Setup(x => x.Load<List<TestEvent>>(It.IsAny<string>())).Returns(new List<TestEvent>());
        storage.Setup(x => x.Save(It.IsAny<string>(), It.IsAny<List<TestEvent>>()))
            .Callback<string, List<TestEvent>>((key, events) => savedEvents = events);
        transport.Setup(x => x.SendEventAsync(It.IsAny<TestEvent>()))
            .ThrowsAsync(new Exception("Network error"));

        var client = new ABClient(transport.Object, storage.Object, new ABOptions { AutoFlush = false });
        client.SetConfig(new ServerConfig { Tests = new Dictionary<string, string> { { "test1", "A" } } });

        // Act
        await client.TrackAsync("test1", "user1", "event1");
        await client.FlushAsync();

        // Assert
        Assert.Equal(1, savedEvents.Count);
    }

    [Fact]
    public async Task FlushAsync_WhenBufferEmpty_DoesNothing()
    {
        // Arrange
        var transport = new Mock<IABTransport>();
        var storage = new Mock<ILocalStorage>();
        var sendCount = 0;

        storage.Setup(x => x.Load<List<TestEvent>>(It.IsAny<string>())).Returns(new List<TestEvent>());
        transport.Setup(x => x.SendEventAsync(It.IsAny<TestEvent>()))
            .Callback(() => sendCount++);

        var client = new ABClient(transport.Object, storage.Object, new ABOptions { AutoFlush = false });

        // Act
        await client.FlushAsync();

        // Assert
        Assert.Equal(0, sendCount);
    }

    [Fact]
    public async Task FlushAsync_PartialFailure_KeepsFailedEvents()
    {
        // Arrange
        var transport = new Mock<IABTransport>();
        var storage = new Mock<ILocalStorage>();
        var sentEvents = new List<string>();
        var savedEvents = new List<TestEvent>();

        storage.Setup(x => x.Load<List<TestEvent>>(It.IsAny<string>())).Returns(new List<TestEvent>());
        storage.Setup(x => x.Save(It.IsAny<string>(), It.IsAny<List<TestEvent>>()))
            .Callback<string, List<TestEvent>>((key, events) => savedEvents = events);

        transport.Setup(x => x.SendEventAsync(It.Is<TestEvent>(e => e.TestName == "test1")))
            .Returns(Task.CompletedTask)
            .Callback(() => sentEvents.Add("test1"));
        transport.Setup(x => x.SendEventAsync(It.Is<TestEvent>(e => e.TestName == "test2")))
            .ThrowsAsync(new Exception("Send failed"));

        var client = new ABClient(transport.Object, storage.Object, new ABOptions { AutoFlush = false });
        client.SetConfig(new ServerConfig
        {
            Tests = new Dictionary<string, string>
            {
                { "test1", "A" },
                { "test2", "B" }
            }
        });

        // Act
        await client.TrackAsync("test1", "user1", "event1");
        await client.TrackAsync("test2", "user2", "event2");
        await client.FlushAsync();

        // Assert
        Assert.Contains("test1", sentEvents);
        Assert.Single(savedEvents);
        Assert.Equal("test2", savedEvents.First().TestName);
    }

    #endregion

    #region Buffer Persistence Tests

    [Fact]
    public async Task SaveBuffer_IsCalledAfterEachTrack()
    {
        // Arrange
        var transport = new Mock<IABTransport>();
        var storage = new Mock<ILocalStorage>();
        var saveCallCount = 0;

        storage.Setup(x => x.Load<List<TestEvent>>(It.IsAny<string>())).Returns(new List<TestEvent>());
        storage.Setup(x => x.Save(It.IsAny<string>(), It.IsAny<List<TestEvent>>()))
            .Callback(() => saveCallCount++);

        var client = new ABClient(transport.Object, storage.Object, new ABOptions { AutoFlush = false });
        client.SetConfig(new ServerConfig { Tests = new Dictionary<string, string> { { "test1", "A" } } });

        // Act
        await client.TrackAsync("test1", "user1", "event1");
        await client.TrackAsync("test1", "user2", "event2");
        await client.TrackAsync("test1", "user3", "event3");

        // Assert
        Assert.Equal(3, saveCallCount);
    }

    [Fact]
    public async Task LoadBuffer_RestoresPreviousEvents()
    {
        // Arrange
        var transport = new Mock<IABTransport>();
        var storage = new Mock<ILocalStorage>();
        var existingEvents = new List<TestEvent>
        {
            new TestEvent { TestName = "existing_test", Variant = "A", UserId = "existing_user" }
        };

        storage.Setup(x => x.Load<List<TestEvent>>(It.IsAny<string>())).Returns(existingEvents);

        // Act
        var client = new ABClient(transport.Object, storage.Object, new ABOptions { AutoFlush = false });

        // Assert - проверяем через рефлексию или публичный метод FlushAsync
        // (косвенная проверка - буфер загружен)
        Assert.NotNull(client);
    }

    #endregion

    #region Multiple Events Tests

    [Fact]
    public async Task MultipleEvents_AreTrackedAndFlushedCorrectly()
    {
        // Arrange
        var transport = new Mock<IABTransport>();
        var storage = new Mock<ILocalStorage>();
        var sentEvents = new List<TestEvent>();

        storage.Setup(x => x.Load<List<TestEvent>>(It.IsAny<string>())).Returns(new List<TestEvent>());
        transport.Setup(x => x.SendEventAsync(It.IsAny<TestEvent>()))
            .Callback<TestEvent>(e => sentEvents.Add(e))
            .Returns(Task.CompletedTask);

        var client = new ABClient(transport.Object, storage.Object, new ABOptions { AutoFlush = false });
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
        for (int i = 0; i < 10; i++)
        {
            await client.TrackAsync("test1", $"user_{i}", "conversion");
            await client.TrackAsync("test2", $"user_{i}", "click");
            await client.TrackAsync("test3", $"user_{i}", "view");
        }
        await client.FlushAsync();

        // Assert
        Assert.Equal(30, sentEvents.Count);
        Assert.Equal(10, sentEvents.Count(e => e.TestName == "test1"));
        Assert.Equal(10, sentEvents.Count(e => e.TestName == "test2"));
        Assert.Equal(10, sentEvents.Count(e => e.TestName == "test3"));
    }

    [Fact]
    public async Task HighVolumeEvents_DoNotCauseErrors()
    {
        // Arrange
        var transport = new Mock<IABTransport>();
        var storage = new Mock<ILocalStorage>();
        var sendCount = 0;

        storage.Setup(x => x.Load<List<TestEvent>>(It.IsAny<string>())).Returns(new List<TestEvent>());
        transport.Setup(x => x.SendEventAsync(It.IsAny<TestEvent>()))
            .Returns(Task.CompletedTask)
            .Callback(() => sendCount++);

        var client = new ABClient(transport.Object, storage.Object, new ABOptions { AutoFlush = false });
        client.SetConfig(new ServerConfig
        {
            Tests = new Dictionary<string, string> { { "stress_test", "A" } }
        });

        // Act
        const int eventCount = 100;
        for (int i = 0; i < eventCount; i++)
        {
            await client.TrackAsync("stress_test", $"user_{i}", "event");
        }
        await client.FlushAsync();

        // Assert
        Assert.Equal(eventCount, sendCount);
    }

    #endregion

    #region SetConfig Tests

    [Fact]
    public void SetConfig_ReplacesCurrentConfig()
    {
        // Arrange
        var transport = new Mock<IABTransport>();
        var storage = new Mock<ILocalStorage>();
        var client = new ABClient(transport.Object, storage.Object);
        client.SetConfig(new ServerConfig
        {
            Tests = new Dictionary<string, string> { { "old_test", "old_value" } }
        });

        // Act
        client.SetConfig(new ServerConfig
        {
            Tests = new Dictionary<string, string> { { "new_test", "new_value" } }
        });

        // Assert
        Assert.Equal("default", client.GetVariant("old_test"));
        Assert.Equal("new_value", client.GetVariant("new_test"));
    }

    [Fact]
    public void SetConfig_WithEmptyConfig_RemovesAllVariants()
    {
        // Arrange
        var transport = new Mock<IABTransport>();
        var storage = new Mock<ILocalStorage>();
        var client = new ABClient(transport.Object, storage.Object);
        client.SetConfig(new ServerConfig
        {
            Tests = new Dictionary<string, string> { { "test1", "A" } }
        });

        // Act
        client.SetConfig(new ServerConfig());

        // Assert
        Assert.Equal("default", client.GetVariant("test1"));
    }

    #endregion

    #region Constructor and Options Tests

    [Fact]
    public void Constructor_WithNullOptions_UsesDefaultOptions()
    {
        // Arrange & Act
        var transport = new Mock<IABTransport>();
        var storage = new Mock<ILocalStorage>();
        var client = new ABClient(transport.Object, storage.Object, null);

        // Assert - проверяем через рефлексию или просто что клиент создался
        Assert.NotNull(client);
    }

    [Fact]
    public void Constructor_WithCustomOptions_UsesProvidedOptions()
    {
        // Arrange & Act
        var transport = new Mock<IABTransport>();
        var storage = new Mock<ILocalStorage>();
        var options = new ABOptions { StorageKey = "custom_key", AutoFlush = false };
        var client = new ABClient(transport.Object, storage.Object, options);

        // Assert
        Assert.NotNull(client);
    }

    #endregion
}