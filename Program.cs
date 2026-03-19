using Microsoft.EntityFrameworkCore;
// Здесь будут твои неймспейсы (Data, Services)

var builder = WebApplication.CreateBuilder(args);

// 1. Добавляем поддержку контроллеров (для API игры и лидерборда)
builder.Services.AddControllers();

// 2. Настраиваем БД (например, SQLite для простоты прототипа)
// builder.Services.AddDbContext<ApplicationDbContext>(options => 
//    options.UseSqlite("Data Source=game.db"));

// 3. Регистрируем твои сервисы (бизнес-логику и Qwen)
// builder.Services.AddScoped<IGameEngine, GameEngine>();
// builder.Services.AddSingleton<QwenService>();

var app = builder.Build();

// 4. Включаем обслуживание статических файлов (index.html, js, css)
app.UseDefaultFiles(); // Ищет index.html в wwwroot по умолчанию
app.UseStaticFiles();

// 5. Маппим эндпоинты контроллеров
app.MapControllers();

// Тестовый эндпоинт для проверки статуса сервера
app.MapGet("/api/status", () => new { status = "Server is running", time = DateTime.Now });

app.Run();
