using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using MongoDB.Driver;
using Testing;
using Testing.Base;
using Testing.DTO;
using Testing.Pattern;
using WebAppTest.Control;
using static Testing.Base.BaseMongo;
using OfficeOpenXml;


var builder = WebApplication.CreateBuilder(args);

// mvc и razor pages
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AllowAnonymousToPage("/Login");
    options.Conventions.AllowAnonymousToPage("/Registration");
    options.Conventions.AllowAnonymousToPage("/AccessDenied");
});

builder.Services.AddControllers();
builder.Services.AddApplicationInsightsTelemetry();

// аутентификация
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login";
        options.LogoutPath = "/Logout";
        options.AccessDeniedPath = "/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Lax;
    })
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"] ?? "GOOGLE_CLIENT_ID";
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"] ?? "GOOGLE_SECRET";
    });

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

// http клиент для api
builder.Services.AddHttpClient<ApiClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"] ?? "https://localhost:5001/");
    client.Timeout = TimeSpan.FromSeconds(30);
});

// ========================================
// регистрация mongodb (необходимо для testing)
// ========================================

// регистрация клиента mongodb
builder.Services.AddSingleton<IMongoClient>(_ =>
    new MongoClient(builder.Configuration.GetConnectionString("MongoDB") ?? "mongodb://localhost:27017"));

// регистрация базы данных
builder.Services.AddScoped(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    var databaseName = builder.Configuration["DatabaseName"] ?? "ABTesting";
    return client.GetDatabase(databaseName);
});

// регистрация репозиториев
builder.Services.AddScoped(typeof(IMongoRepo<>), typeof(MongoRepo<>));

// ========================================
// регистрация facade (главный сервис testing)
// ========================================

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
        sp.GetRequiredService<IMongoRepo<MetricTypes>>(),
        sp.GetRequiredService<IMongoRepo<Roles>>(),
        sp.GetRequiredService<IMongoRepo<Developers>>(),
        sp.GetRequiredService<IMongoRepo<EquipParam>>(),
        sp.GetRequiredService<IMongoRepo<Values>>()
    );
});

// регистрация построителя статистики
builder.Services.AddScoped<IStatsBuilder, StatsBuilder>(sp =>
{
    return new StatsBuilder(
        sp.GetRequiredService<IMongoRepo<Variants>>(),
        sp.GetRequiredService<IMongoRepo<AbResults>>(),
        sp.GetRequiredService<IMongoRepo<Values>>()
    );
});

// регистрация стратегий и сервисов


// стратегии
builder.Services.AddScoped<IStrategy<VariantDto>, AdaptStrategy>();
//Адаптивная
builder.Services.AddScoped<IWeightStrategy, WeightStrategy>();
// MAB
builder.Services.AddScoped<ThompsonSamplingStrategy>();
builder.Services.AddScoped<UCBStrategy>();
builder.Services.AddScoped<EpsilonGreedyStrategy>();
builder.Services.AddScoped<MABManager>();

// сервисы testing
//builder.Services.AddScoped<Adaptation>();

builder.Services.AddScoped<Adaptation>(sp =>
{
    return new Adaptation(sp.GetRequiredService<IStatsBuilder>(), sp.GetRequiredService<IWeightStrategy>(),
        sp.GetRequiredService<IMongoRepo<Values>>(), sp.GetRequiredService<IMongoRepo<EquipParam>>());
});

builder.Services.AddScoped<ServiceControl>();

// ui сервисы
builder.Services.AddScoped<IUiService, UiService>();
builder.Services.AddScoped<ChartService>();

// построение приложения

var app = builder.Build();

// настройка pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<RedirectMiddleware>();
app.MapRazorPages();
app.MapControllers();

app.Run();