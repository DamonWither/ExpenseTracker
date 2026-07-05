using System.ComponentModel.DataAnnotations;
using ExpenseTracker.Api.Enums;

namespace ExpenseTracker.Api.Dtos;

/// <summary>
/// Ответ API: представление расхода.
/// </summary>
/// <param name="Id">Идентификатор расхода (GUID).</param>
/// <param name="Description">Описание (строка, 1–200 символов).</param>
/// <param name="Amount">Сумма расхода (decimal).</param>
/// <param name="Date">Дата расхода (DateOnly).</param>
/// <param name="Category">Категория расхода (enum ExpenseCategory).</param>
/// <param name="CreatedAt">Время создания (UTC).</param>
/// <param name="UpdatedAt">Время последнего обновления или null.</param>
public sealed record ExpenseResponse(
    Guid Id,
    string Description,
    decimal Amount,
    DateOnly Date,
    ExpenseCategory Category,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt);

/// <summary>
/// Запрос на создание расхода.
/// Принимает описание, сумму, дату и категорию.
/// Валидация атрибутами: Description обязателен (1-200), Amount в диапазоне >0.
/// </summary>
public sealed class ExpenseCreateRequest
{
    /// <summary>Краткое описание расхода. Обязательно, 1–200 символов.</summary>
    [Required]
    [MinLength(1)]
    [MaxLength(200)]
    public string Description { get; set; } = string.Empty;

    /// <summary>Сумма расхода. Допустимо от 0.01 до 999_999_999.99.</summary>
    [Range(0.01, 999999999.99)]
    public decimal Amount { get; set; }

    /// <summary>Дата расхода (DateOnly).</summary>
    public DateOnly Date { get; set; }

    /// <summary>Категория расхода (ExpenseCategory).</summary>
    public ExpenseCategory Category { get; set; }
}

/// <summary>
/// Запрос на обновление расхода.
/// Аналогичен ExpenseCreateRequest — те же поля и ограничения.
/// </summary>
public sealed class ExpenseUpdateRequest
{
    /// <summary>Краткое описание расхода. Обязательно, 1–200 символов.</summary>
    [Required]
    [MinLength(1)]
    [MaxLength(200)]
    public string Description { get; set; } = string.Empty;

    /// <summary>Сумма расхода. Допустимо от 0.01 до 999_999_999.99.</summary>
    [Range(0.01, 999999999.99)]
    public decimal Amount { get; set; }

    /// <summary>Дата расхода (DateOnly).</summary>
    public DateOnly Date { get; set; }

    /// <summary>Категория расхода (ExpenseCategory).</summary>
    public ExpenseCategory Category { get; set; }
}

/// <summary>
/// Параметры фильтрации и пагинации при запросе списка расходов.
/// </summary>
/// <param name="DateFrom">Начальная дата фильтра (включительно) или null.</param>
/// <param name="DateTo">Конечная дата фильтра (включительно) или null.</param>
/// <param name="Category">Фильтр по категории или null.</param>
/// <param name="Search">Строка поиска по описанию или null.</param>
/// <param name="Page">Номер страницы (по умолчанию 1).</param>
/// <param name="PageSize">Размер страницы (по умолчанию 50).</param>
public sealed record ExpenseQuery(
    DateOnly? DateFrom,
    DateOnly? DateTo,
    ExpenseCategory? Category,
    string? Search,
    int Page = 1,
    int PageSize = 50);

/// <summary>
/// Результат с пагинацией: список элементов, общее количество и текущая страница.
/// </summary>
/// <typeparam name="T">Тип элемента в списке.</typeparam>
public sealed record PagedResult<T>(IReadOnlyList<T> Items, int TotalCount, int Page, int PageSize);

/// <summary>Сводка по категории: категория и общая сумма по ней.</summary>
public sealed record CategorySummary(ExpenseCategory Category, decimal TotalAmount);

/// <summary>Сводка по дню: дата и общая сумма за день.</summary>
public sealed record DailySummary(DateOnly Date, decimal TotalAmount);

/// <summary>Ответ API со сводкой расходов: общая сумма, по категориям и по дням.</summary>
/// <param name="TotalAmount">Общая сумма за период.</param>
/// <param name="ByCategory">Список сумм по категориям.</param>
/// <param name="ByDay">Список сумм по дням.</param>
public sealed record ExpenseSummaryResponse(decimal TotalAmount, IReadOnlyList<CategorySummary> ByCategory, IReadOnlyList<DailySummary> ByDay);
