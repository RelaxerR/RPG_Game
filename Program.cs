using System.Text.Json;
using System.Text.Json.Serialization; // ← Добавь этот using
using RPG_Game.Services;

var builder = WebApplication.CreateBuilder(args);

// === 1. НАСТРОЙКА ЛОГГЕРА ===
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Information);

// 2. Добавляем поддержку контроллеров
builder.Services.AddControllers()
    .AddJsonOptions(options => {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// 3. Регистрируем сервисы
builder.Services.AddSingleton<GameSessionService>();

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
