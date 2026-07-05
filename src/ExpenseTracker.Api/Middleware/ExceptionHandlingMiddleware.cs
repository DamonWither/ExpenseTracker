using System.Net;
using System.Text.Json;

namespace ExpenseTracker.Api.Middleware;

/// <summary>
/// Middleware для глобальной обработки исключений в API
/// Перехватывает исключения из последующих middleware/контроллеров и возвращает клиенту
/// </summary>
/// <param name="next">Следующий middleware в конвейере </param>
/// <param name="logger">Логгер для записи неожиданных ошибок </param>
public sealed class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    /// <summary>
    /// Обработчик запроса middleware
    /// Принимает HttpContext и выполняет следующий обработчик в конвейере
    /// </summary>
    /// <param name="context">Контекст HTTP-запроса </param>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ArgumentException ex)
        {
            await WriteErrorAsync(context, HttpStatusCode.BadRequest, ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled API error");
            await WriteErrorAsync(context, HttpStatusCode.InternalServerError, "Unexpected server error.");
        }
    }

    /// <summary>
    /// Записывает JSON-ответ об ошибке в HttpResponse
    /// </summary>
    /// <param name="context">Текущий HttpContext </param>
    /// <param name="statusCode">HTTP статус ответа </param>
    /// <param name="message">Текст сообщения об ошибке, который будет помещён в поле "error" </param>
    private static async Task WriteErrorAsync(HttpContext context, HttpStatusCode statusCode, string message)
    {
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(JsonSerializer.Serialize(new { error = message }));
    }
}
