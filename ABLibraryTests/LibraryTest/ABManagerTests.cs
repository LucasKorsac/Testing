//using ABLibrary.Interfaces;
//using ABLibrary.Models;
//using ABLibrary;
//using Moq;

//public class ABManagerTests
//{
//    private readonly Mock<IABTransport> _transportMock;
//    private readonly Mock<ILocalStorage> _storageMock;
//    private readonly ABClient _client;
//    private readonly ABManager _manager;

//    public ABManagerTests()
//    {
//        _transportMock = new Mock<IABTransport>();
//        _storageMock = new Mock<ILocalStorage>();

//        _client = new ABClient(
//            _transportMock.Object,
//            _storageMock.Object,
//            new ABOptions { AutoFlush = false });

//        _manager = new ABManager(_client);
//    }

//    [Fact]
//    public async Task InitAsync_ShouldRequestConfigForApplication()
//    {
//        // Arrange
//        const string appId = "test_app";

//        _transportMock
//            .Setup(x => x.GetConfigAsync(
//                appId,
//                It.IsAny<CancellationToken>()))
//            .ReturnsAsync(new ServerConfig
//            {
//                Tests = new Dictionary<string, string>
//                {
//                    { "test_name", "variant_a" }
//                }
//            });

//        // Act
//        await _manager.InitAsync(appId);

//        // Assert
//        _transportMock.Verify(
//            x => x.GetConfigAsync(
//                appId,
//                It.IsAny<CancellationToken>()),
//            Times.Once);

//        Assert.Equal("variant_a", _manager.GetVariant("test_name"));
//    }

//    [Fact]
//    public void GetVariant_ShouldReturnConfiguredVariant()
//    {
//        // Arrange
//        _client.SetConfig(new ServerConfig
//        {
//            Tests = new Dictionary<string, string>
//            {
//                { "test_name", "variant_b" }
//            }
//        });

//        // Act
//        var result = _manager.GetVariant("test_name");

//        // Assert
//        Assert.Equal("variant_b", result);
//    }

//    [Fact]
//    public void GetVariant_ShouldReturnNullForUnknownTest()
//    {
//        // Arrange
//        _client.SetConfig(new ServerConfig
//        {
//            Tests = new Dictionary<string, string>()
//        });

//        // Act
//        var result = _manager.GetVariant("unknown_test");

//        // Assert
//        Assert.Null(result);
//    }

//    [Fact]
//    public async Task TrackAsync_WithAutoFlushDisabled_ShouldNotSendEventImmediately()
//    {
//        // Arrange
//        const string testName = "checkout_test";
//        const string userId = "user_123";
//        const string eventType = "conversion";

//        _client.SetConfig(new ServerConfig
//        {
//            Tests = new Dictionary<string, string>
//            {
//                { testName, "variant_a" }
//            }
//        });

//        // Act
//        await _manager.TrackAsync(testName, userId, eventType);

//        // Assert
//        _transportMock.Verify(
//            x => x.SendEventAsync(
//                It.IsAny<TestEvent>(),
//                It.IsAny<CancellationToken>()),
//            Times.Never);
//    }

//    [Fact]
//    public async Task FlushAsync_AfterTrackingEvent_ShouldSendBufferedEvent()
//    {
//        // Arrange
//        const string testName = "checkout_test";
//        const string userId = "user_123";
//        const string eventType = "conversion";

//        _client.SetConfig(new ServerConfig
//        {
//            Tests = new Dictionary<string, string>
//            {
//                { testName, "variant_a" }
//            }
//        });

//        TestEvent sentEvent = null;

//        _transportMock
//            .Setup(x => x.SendEventsAsync(
//                It.IsAny<TestEvent>(),
//                It.IsAny<CancellationToken>()))
//            .Callback<TestEvent, CancellationToken>(
//                (evt, _) => sentEvent = evt)
//            .Returns(Task.CompletedTask);

//        await _manager.TrackAsync(testName, userId, eventType);

//        // Act
//        await _manager.FlushAsync();

//        // Assert
//        _transportMock.Verify(
//            x => x.SendEventsAsync(
//                It.IsAny<TestEvent>(),
//                It.IsAny<CancellationToken>()),
//            Times.Once);

//        Assert.NotNull(sentEvent);
//        Assert.Equal(userId, sentEvent.UserId);
//        Assert.Equal(eventType, sentEvent.EventType);
//    }

//    [Fact]
//    public async Task FlushAsync_WithoutTrackedEvents_ShouldNotSendEvent()
//    {
//        // Arrange
//        _transportMock
//            .Setup(x => x.SendEventsAsync(
//                It.IsAny<TestEvent>(),
//                It.IsAny<CancellationToken>()))
//            .Returns(Task.CompletedTask);

//        // Act
//        await _manager.FlushAsync();

//        // Assert
//        _transportMock.Verify(
//            x => x.SendEventsAsync(
//                It.IsAny<TestEvent>(),
//                It.IsAny<CancellationToken>()),
//            Times.Never);
//    }
//}