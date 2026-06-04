using ABLibrary.Core;
using ABLibrary.Interfaces;
using ABLibrary.Models;
using Moq;

namespace ABLibraryTests.LibraryTest;

public class ABClientTests
{
    [Fact]
    public async Task InitializeAsync_Loads_Config_Success()
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

        transport.Setup(x => x.GetConfigAsync("app1"))
            .ReturnsAsync(expectedConfig);

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
    public async Task InitializeAsync_WhenNetworkError_UsesOfflineMode()
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
        // Проверяем, что конфиг остался пустым, но клиент не упал
    }

    [Fact]
    public async Task TrackAsync_WithAutoFlush_SendsEventImmediately()
    {
        // Arrange
        var transport = new Mock<IABTransport>();
        var storage = new Mock<ILocalStorage>();
        var savedEvents = new List<TestEvent>();

        storage.Setup(x => x.Load<List<TestEvent>>(It.IsAny<string>()))
            .Returns(new List<TestEvent>());

        storage.Setup(x => x.Save(It.IsAny<string>(), It.IsAny<List<TestEvent>>()))
            .Callback<string, List<TestEvent>>((key, events) => savedEvents = events);

        transport.Setup(x => x.SendEventAsync(It.IsAny<TestEvent>()))
            .Returns(Task.CompletedTask);

        var client = new ABClient(
            transport.Object,
            storage.Object,
            new ABOptions { AutoFlush = true });

        client.SetConfig(new ServerConfig
        {
            Tests = new Dictionary<string, string>
            {
                { "test1", "VariantA" }
            }
        });

        // Act
        await client.TrackAsync("test1", "user_123", "conversion");

        // Assert
        transport.Verify(x => x.SendEventAsync(It.IsAny<TestEvent>()), Times.Once);
        Assert.Single(savedEvents); // Буфер должен быть пуст после отправки
    }

    [Fact]
    public async Task TrackAsync_WithoutAutoFlush_BuffersEvent()
    {
        // Arrange
        var transport = new Mock<IABTransport>();
        var storage = new Mock<ILocalStorage>();
        var savedEvents = new List<TestEvent>();

        storage.Setup(x => x.Load<List<TestEvent>>(It.IsAny<string>()))
            .Returns(new List<TestEvent>());

        storage.Setup(x => x.Save(It.IsAny<string>(), It.IsAny<List<TestEvent>>()))
            .Callback<string, List<TestEvent>>((key, events) => savedEvents = events);

        transport.Setup(x => x.SendEventAsync(It.IsAny<TestEvent>()))
            .Returns(Task.CompletedTask);

        var client = new ABClient(
            transport.Object,
            storage.Object,
            new ABOptions { AutoFlush = false });

        client.SetConfig(new ServerConfig
        {
            Tests = new Dictionary<string, string> { { "test1", "A" } }
        });

        // Act
        await client.TrackAsync("test1", "user_456", "click");

        // Assert
        transport.Verify(x => x.SendEventAsync(It.IsAny<TestEvent>()), Times.Never);
        Assert.Single(savedEvents);
    }

    [Fact]
    public async Task FlushAsync_SendsAllBufferedEvents()
    {
        // Arrange
        var transport = new Mock<IABTransport>();
        var storage = new Mock<ILocalStorage>();
        var sendCount = 0;

        storage.Setup(x => x.Load<List<TestEvent>>(It.IsAny<string>()))
            .Returns(new List<TestEvent>());

        storage.Setup(x => x.Save(It.IsAny<string>(), It.IsAny<List<TestEvent>>()))
            .Callback<string, List<TestEvent>>((key, events) => { });

        transport.Setup(x => x.SendEventAsync(It.IsAny<TestEvent>()))
            .Returns(Task.CompletedTask)
            .Callback(() => sendCount++);

        var client = new ABClient(
            transport.Object,
            storage.Object,
            new ABOptions { AutoFlush = false });

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

        storage.Setup(x => x.Load<List<TestEvent>>(It.IsAny<string>()))
            .Returns(new List<TestEvent>());

        storage.Setup(x => x.Save(It.IsAny<string>(), It.IsAny<List<TestEvent>>()))
            .Callback<string, List<TestEvent>>((key, events) => savedEvents = events);

        transport.Setup(x => x.SendEventAsync(It.IsAny<TestEvent>()))
            .ThrowsAsync(new Exception("Network error"));

        var client = new ABClient(
            transport.Object,
            storage.Object,
            new ABOptions { AutoFlush = false });

        client.SetConfig(new ServerConfig
        {
            Tests = new Dictionary<string, string> { { "test1", "A" } }
        });

        // Act
        await client.TrackAsync("test1", "user1", "event1");
        var bufferBeforeFlush = savedEvents.Count;
        await client.FlushAsync();

        // Assert
        Assert.Equal(1, bufferBeforeFlush);
        Assert.Equal(1, savedEvents.Count); // Событие осталось в буфере
    }

    [Fact]
    public async Task TrackAsync_AddsCorrectEventData()
    {
        // Arrange
        var transport = new Mock<IABTransport>();
        var storage = new Mock<ILocalStorage>();
        TestEvent capturedEvent = null;

        storage.Setup(x => x.Load<List<TestEvent>>(It.IsAny<string>()))
            .Returns(new List<TestEvent>());

        storage.Setup(x => x.Save(It.IsAny<string>(), It.IsAny<List<TestEvent>>()))
            .Callback<string, List<TestEvent>>((key, events) => capturedEvent = events.First());

        var client = new ABClient(
            transport.Object,
            storage.Object,
            new ABOptions { AutoFlush = false });

        client.SetConfig(new ServerConfig
        {
            Tests = new Dictionary<string, string> { { "login_test", "VariantX" } }
        });

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

        // Config не установлен (по умолчанию пустой)

        // Act
        var result = client.GetVariant("any_test");

        // Assert
        Assert.Equal("default", result);
    }

    [Fact]
    public async Task MultipleEvents_AreTrackedAndFlushedCorrectly()
    {
        // Arrange
        var transport = new Mock<IABTransport>();
        var storage = new Mock<ILocalStorage>();
        var sentEvents = new List<TestEvent>();

        storage.Setup(x => x.Load<List<TestEvent>>(It.IsAny<string>()))
            .Returns(new List<TestEvent>());

        transport.Setup(x => x.SendEventAsync(It.IsAny<TestEvent>()))
            .Callback<TestEvent>(e => sentEvents.Add(e))
            .Returns(Task.CompletedTask);

        var client = new ABClient(
            transport.Object,
            storage.Object,
            new ABOptions { AutoFlush = false });

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
    public async Task InitializeAsync_LoadsConfigAndOverridesExisting()
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

        // Устанавливаем старую конфигурацию
        client.SetConfig(new ServerConfig
        {
            Tests = new Dictionary<string, string> { { "old_test", "old_variant" } }
        });

        // Act
        await client.InitializeAsync("app1");

        // Assert
        Assert.Equal("new_variant", client.GetVariant("new_test"));
        Assert.Equal("default", client.GetVariant("old_test")); // Старая конфигурация перезаписана
    }

    [Fact]
    public async Task SaveBuffer_IsCalledAfterTracking()
    {
        // Arrange
        var transport = new Mock<IABTransport>();
        var storage = new Mock<ILocalStorage>();
        var saveCallCount = 0;

        storage.Setup(x => x.Load<List<TestEvent>>(It.IsAny<string>()))
            .Returns(new List<TestEvent>());

        storage.Setup(x => x.Save(It.IsAny<string>(), It.IsAny<List<TestEvent>>()))
            .Callback(() => saveCallCount++);

        var client = new ABClient(
            transport.Object,
            storage.Object,
            new ABOptions { AutoFlush = false });

        client.SetConfig(new ServerConfig
        {
            Tests = new Dictionary<string, string> { { "test1", "A" } }
        });

        // Act
        await client.TrackAsync("test1", "user1", "event1");
        await client.TrackAsync("test1", "user2", "event2");

        // Assert
        Assert.Equal(2, saveCallCount); // Save вызывается при каждом Track
    }
}