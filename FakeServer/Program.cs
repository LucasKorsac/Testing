using ABLibrary.Models;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapGet("/api/ab/config/{appId}", (string appId) =>
{
    return new ServerConfig
    {
        Tests = new Dictionary<string, string>
        {
            ["button_test"] = "red_button"
        }
    };
});

app.MapPost("/api/ab/event", (TestEvent evt) =>
{
    Console.WriteLine("EVENT RECEIVED:");
    Console.WriteLine(
        $"{evt.TestName} | {evt.Variant} | {evt.UserId}");

    return Results.Ok();
});

app.Run("http://0.0.0.0:5000");