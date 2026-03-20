using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using RPG_Game.Services;

var builder = WebApplication.CreateBuilder(args);

// === 1. НАСТРОЙКА ЛОГГЕРА ===
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Уровни логирования (можно вынести в appsettings.json)
builder.Logging.SetMinimumLevel(LogLevel.Information);

// 2. Добавляем поддержку контроллеров
builder.Services.AddControllers()
    .AddJsonOptions(options => {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });

// 3. Регистрируем сервисы
builder.Services.AddSingleton<GameSessionService>();
// builder.Services.AddSingleton<QwenService>(); // Когда реализуешь

var app = builder.Build();

// 4. Статические файлы
app.UseDefaultFiles();
app.UseStaticFiles();

// 5. Маппим эндпоинты
app.MapControllers();

// Тестовый эндпоинт
app.MapGet("/api/status", () => new { status = "Server is running", time = DateTime.Now });

// === ЛОГ ЗАПУСКА ===
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("🎮 Echoes of the Tavern запущен на порту {Port}", app.Urls.FirstOrDefault() ?? "default");

app.Run();
