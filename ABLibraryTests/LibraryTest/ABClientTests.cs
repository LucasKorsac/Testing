using ABLibrary.Core;
using ABLibrary.Interfaces;
using ABLibrary.Models;
using Moq;

namespace ABLibraryTests.LibraryTest;

public class ABClientTests
{
    [Fact]
    public async Task InitializeAsync_Loads_Config()
    {
        // Arrange
        var transport = new Mock<IABTransport>();
        var storage = new Mock<ILocalStorage>();

        transport.Setup(x => x.GetConfigAsync("app1"))
            .ReturnsAsync(new ServerConfig
            {
                Tests = new Dictionary<string, string>
                {
                    { "button_test", "A" }
                }
            });

        var client = new ABClient(
            transport.Object,
            storage.Object);

        // Act
        await client.InitializeAsync("app1");

        // Assert
        Assert.Equal(
            "A",
            client.GetVariant("button_test"));
    }

    [Fact]
    public async Task TrackAsync_Sends_Event()
    {
        // Arrange
        var transport = new Mock<IABTransport>();
        var storage = new Mock<ILocalStorage>();

        storage.Setup(x =>
            x.Load<List<TestEvent>>(It.IsAny<string>()))
            .Returns(new List<TestEvent>());

        transport.Setup(x =>
            x.SendEventAsync(It.IsAny<TestEvent>()))
            .Returns(Task.CompletedTask);

        var client = new ABClient(
            transport.Object,
            storage.Object,
            new ABOptions
            {
                AutoFlush = true
            });

        client.SetConfig(new ServerConfig
        {
            Tests = new Dictionary<string, string>
            {
                { "paywall_test", "B" }
            }
        });

        // Act
        await client.TrackAsync(
            "paywall_test",
            "Samsung_S23",
            "purchase");

        // Assert
        transport.Verify(x =>
            x.SendEventAsync(It.Is<TestEvent>(e =>
                e.TestName == "paywall_test" &&
                e.Variant == "B" &&
                e.UserId == "Samsung_S23" &&
                e.EventType == "purchase")),
            Times.Once);
    }

    [Fact]
    public void GetVariant_Returns_Default()
    {
        // Arrange
        var transport = new Mock<IABTransport>();
        var storage = new Mock<ILocalStorage>();

        var client = new ABClient(
            transport.Object,
            storage.Object);

        // Act
        var result = client.GetVariant("unknown_test");

        // Assert
        Assert.Equal("default", result);
    }
}