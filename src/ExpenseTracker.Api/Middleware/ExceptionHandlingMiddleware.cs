using System.Net;
using System.Text.Json;

namespace ExpenseTracker.Api.Middleware;

/// <summary>
/// Middleware для глобальной обработки исключений в API.
/// Перехватывает исключения из последующих middleware/контроллеров и возвращает клиенту
/// JSON-ответ с полем "error" и соответствующим HTTP-кодом.
/// </summary>
/// <param name="next">Следующий middleware в конвейере.</param>
/// <param name="logger">Логгер для записи неожиданных ошибок.</param>
public sealed class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    /// <summary>
    /// Обработчик запроса middleware.
    /// Принимает HttpContext и выполняет следующий обработчик в конвейере.
    /// В случае ArgumentException возвращает 400 Bad Request с { "error": "message" }.
    /// Для остальных исключений логирует ошибку и возвращает 500 Internal Server Error с общим сообщением.
    /// </summary>
    /// <param name="context">Контекст HTTP-запроса.</param>
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
    /// Записывает JSON-ответ об ошибке в HttpResponse.
    /// Формат ответа: { "error": "&lt;message&gt;" }.
    /// </summary>
    /// <param name="context">Текущий HttpContext.</param>
    /// <param name="statusCode">HTTP статус ответа (400, 500 и т.д.).</param>
    /// <param name="message">Текст сообщения об ошибке, который будет помещён в поле "error".</param>
    private static async Task WriteErrorAsync(HttpContext context, HttpStatusCode statusCode, string message)
    {
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(JsonSerializer.Serialize(new { error = message }));
    }
}
