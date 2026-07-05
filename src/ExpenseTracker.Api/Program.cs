using System.Text.Json.Serialization;
using ExpenseTracker.Api.Data;
using ExpenseTracker.Api.Middleware;
using ExpenseTracker.Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Добавление DbContext: подключение к PostgreSQL через строку подключения "DefaultConnection"
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Регистрация сервисов приложения
builder.Services.AddScoped<ExpenseService>();

// Контроллеры и настройки JSON
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS: разрешаем фронтенду обращаться к API
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();

// Глобальная обработка исключений
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Применение CORS-политики к конвейеру
app.UseCors("Frontend");

// Swagger доступен только в режиме разработки
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Поддержка статических файлов (frontend SPA) — корневая папка wwwroot
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();

// Автоматическое применение миграций при старте
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

app.Run();
