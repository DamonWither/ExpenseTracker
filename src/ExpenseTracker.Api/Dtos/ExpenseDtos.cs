using System.ComponentModel.DataAnnotations;
using ExpenseTracker.Api.Enums;

namespace ExpenseTracker.Api.Dtos;

/// <summary>
/// Ответ API: представление расхода
/// </summary>
public sealed class ExpenseResponse : ExpenseDtoBase
{
    public Guid Id { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }

    public ExpenseResponse(Guid id, string description, decimal amount, DateOnly date, ExpenseCategory category, DateTimeOffset createdAt, DateTimeOffset? updatedAt)
    {
        Id = id;
        Description = description;
        Amount = amount;
        Date = date;
        Category = category;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }
}

/// <summary>
/// Запрос на создание расхода
/// </summary>
public sealed class ExpenseCreateRequest : ExpenseDtoBase
{
}

/// <summary>
/// Запрос на обновление расхода
/// </summary>
public sealed class ExpenseUpdateRequest : ExpenseDtoBase
{
}

/// <summary>
/// Параметры фильтрации и пагинации при запросе списка расходов
/// </summary>
/// <param name="DateFrom">Начальная дата фильтра (включительно) </param>
/// <param name="DateTo">Конечная дата фильтра (включительно) </param>
/// <param name="Category">Фильтр по категории </param>
/// <param name="Search">Строка поиска по описанию </param>
/// <param name="Page">Номер страницы </param>
/// <param name="PageSize">Размер страницы </param>
public sealed record ExpenseQuery(
    DateOnly? DateFrom,
    DateOnly? DateTo,
    ExpenseCategory? Category,
    string? Search,
    int Page = 1,
    int PageSize = 50);

/// <summary>
/// Результат с пагинацией: список элементов, общее количество и текущая страница
/// </summary>
/// <typeparam name="T">Тип элемента в списке.</typeparam>
public sealed record PagedResult<T>(IReadOnlyList<T> Items, int TotalCount, int Page, int PageSize);

/// <summary>Сводка по категории: категория и общая сумма по ней </summary>
public sealed record CategorySummary(ExpenseCategory Category, decimal TotalAmount);

/// <summary>Сводка по дню: дата и общая сумма за день </summary>
public sealed record DailySummary(DateOnly Date, decimal TotalAmount);

/// <summary>Ответ API со сводкой расходов: общая сумма, по категориям и по дням ы</summary>
/// <param name="TotalAmount">Общая сумма за период.</param>
/// <param name="ByCategory">Список сумм по категориям.</param>
/// <param name="ByDay">Список сумм по дням.</param>
public sealed record ExpenseSummaryResponse(decimal TotalAmount, IReadOnlyList<CategorySummary> ByCategory, IReadOnlyList<DailySummary> ByDay);
