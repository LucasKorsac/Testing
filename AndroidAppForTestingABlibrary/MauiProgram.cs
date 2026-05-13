
using MyAbMobileApp.Services;
namespace MyAbMobileApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>();

        var httpClient = new HttpClient
        {
            BaseAddress = new Uri("http://10.0.2.2:5000/")
        };

        var transport = new HttpABTransport(httpClient);

        var storage = new MobileFileStorage();

        var options = new ABOptions
        {
            AutoFlush = true
        };

        var client = new ABClient(
            transport,
            storage,
            options);

        var manager = new ABManager(client);

        builder.Services.AddSingleton(manager);

        builder.Services.AddSingleton<MainPage>();

        return builder.Build();
    }
}