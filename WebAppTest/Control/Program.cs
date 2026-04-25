using MongoDB.Driver;
using Testing;
using Testing.Base;
using Testing.Pattern;
using WebAppTest.Control;
using static Testing.Base.BaseMongo;

var builder = WebApplication.CreateBuilder(args);

// Конфигурация сервисов

// Подключение Razor Pages
builder.Services.AddRazorPages();

// Подключение API контроллеров
builder.Services.AddControllers();

// Мониторинг, телеметрия
builder.Services.AddApplicationInsightsTelemetry();

//Регистрация базы 
builder.Services.AddSingleton<IMongoClient>(_ => { return new MongoClient("mongodb://localhost:27017"); });

builder.Services.AddSingleton<IMongoDatabase>(sp => { var client = sp.GetRequiredService<IMongoClient>(); return client.GetDatabase("ABTesting"); });

// Бизнес-слой


// Сервис управления A/B тестами
builder.Services.AddScoped(typeof(IMongoRepo<>), typeof(MongoRepo<>));

// Фасад над MongoDB репозиториями
builder.Services.AddScoped<Facade>();

// Стратегия выбора вариантов


// Основная стратегия: адаптивный выбор
builder.Services.AddScoped<IStrategy<Variants>, AdaptiveStrategy>();

// Слой адаптации

// Класс, который строит пул вариантов на основе результатов
builder.Services.AddScoped<Adaptation>();


var app = builder.Build();

// HTTP обработка запросов

// Обработка ошибок
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");

    // HTTP Strict Transport Security (HSTS)
    app.UseHsts();
}

// Перенаправление HTTP к HTTPS
app.UseHttpsRedirection();

// Раздача статических файлов (wwwroot: HTML, JS, CSS)
app.UseStaticFiles();

app.UseRouting();

// Авторизация
app.UseAuthorization();

// Маршрутизация

// Razor Pages endpoints
app.MapRazorPages();

// API контроллеры
app.MapControllers();

// Запуск 
app.Run();