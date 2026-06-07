using ABLibrary.Core;
using ABLibrary.Interfaces;
using ABLibrary.Models;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

public class ABManagerTests
{
    [Fact]
    public async Task InitAsync_CallsClientInitialize()
    {
        // Arrange
        var transport = new Mock<IABTransport>();
        var storage = new Mock<ILocalStorage>();
        var client = new ABClient(transport.Object, storage.Object);
        var manager = new ABManager(client);

        transport.Setup(x => x.GetConfigAsync(It.IsAny<string>()))
            .ReturnsAsync(new ServerConfig());

        // Act
        await manager.InitAsync("test_app");

        // Assert
        transport.Verify(x => x.GetConfigAsync("test_app"), Times.Once);
    }

    [Fact]
    public void GetVariant_CallsClientGetVariant()
    {
        // Arrange
        var transport = new Mock<IABTransport>();
        var storage = new Mock<ILocalStorage>();
        var client = new ABClient(transport.Object, storage.Object);
        var manager = new ABManager(client);

        var expectedConfig = new ServerConfig
        {
            Tests = new Dictionary<string, string> { { "test_name", "test_variant" } }
        };
        client.SetConfig(expectedConfig);

        // Act
        var result = manager.GetVariant("test_name");

        // Assert
        Assert.Equal("test_variant", result);
    }

    [Fact]
    public async Task TrackAsync_CallsClientTrack()
    {
        // Arrange
        var transport = new Mock<IABTransport>();
        var storage = new Mock<ILocalStorage>();
        var client = new ABClient(transport.Object, storage.Object, new ABOptions { AutoFlush = false });
        var manager = new ABManager(client);

        client.SetConfig(new ServerConfig
        {
            Tests = new Dictionary<string, string> { { "test_name", "variant" } }
        });

        // Act
        await manager.TrackAsync("test_name", "user_id", "conversion");

        // Assert
        transport.Verify(x => x.SendEventAsync(It.Is<TestEvent>(e =>
            e.TestName == "test_name" &&
            e.UserId == "user_id" &&
            e.EventType == "conversion")), Times.Never); // AutoFlush = false, поэтому не отправляется
    }

    [Fact]
    public async Task FlushAsync_CallsClientFlush()
    {
        // Arrange
        var transport = new Mock<IABTransport>();
        var storage = new Mock<ILocalStorage>();
        var client = new ABClient(transport.Object, storage.Object, new ABOptions { AutoFlush = false });
        var manager = new ABManager(client);

        client.SetConfig(new ServerConfig
        {
            Tests = new Dictionary<string, string> { { "test_name", "variant" } }
        });

        await manager.TrackAsync("test_name", "user_id", "conversion");

        transport.Setup(x => x.SendEventAsync(It.IsAny<TestEvent>()))
            .Returns(Task.CompletedTask);

        // Act
        await manager.FlushAsync();

        // Assert
        transport.Verify(x => x.SendEventAsync(It.IsAny<TestEvent>()), Times.Once);
    }
}