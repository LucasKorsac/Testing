using MongoDB.Driver;
using Testing;
using Testing.Base;
using Testing.Pattern;
using WebAppTest.Control;
using static Testing.Base.BaseMongo;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddControllers();
builder.Services.AddApplicationInsightsTelemetry();


/// MongoDB
builder.Services.AddSingleton<IMongoClient>(_ => new MongoClient("mongodb://localhost:27017"));

builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase("ABTesting");
});

/// HTTP
builder.Services.AddHttpClient<ApiClient>(client =>
{
    client.BaseAddress = new Uri("https://localhost:5001/");
});

/// Репозиторий

builder.Services.AddScoped(typeof(IMongoRepo<>), typeof(MongoRepo<>));


/// Facade

builder.Services.AddScoped<Facade>(sp =>
{
    return new Facade(
        sp.GetRequiredService<IMongoRepo<ABTests>>(),
        sp.GetRequiredService<IMongoRepo<Variants>>(),
        sp.GetRequiredService<IMongoRepo<AbResults>>(),
        sp.GetRequiredService<IMongoRepo<Instances>>(),
        sp.GetRequiredService<IMongoRepo<Applications>>(),
        sp.GetRequiredService<IMongoRepo<DevelopRoleApplic>>(),
        sp.GetRequiredService<IMongoRepo<Metrics>>(),
        sp.GetRequiredService<IMongoRepo<MetricTypes>>()
    );
});

/// Бизнес-слой
builder.Services.AddScoped<ServiceControl>();
builder.Services.AddScoped<Adaptation>();

builder.Services.AddScoped<IStrategy<Variants>, AdaptiveStrategy>();
builder.Services.AddScoped<IStatsBuilder, StatsBuilder>();
builder.Services.AddScoped<IWeightStrategy, WeightStrategy>();

builder.Services.AddScoped<IUiService, UiService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

app.Run();