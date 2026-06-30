using System.ComponentModel.DataAnnotations;
using ExpenseTracker.Api.Enums;

namespace ExpenseTracker.Api.Dtos;

public sealed record ExpenseResponse(
    Guid Id,
    string Description,
    decimal Amount,
    DateOnly Date,
    ExpenseCategory Category,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt);

public sealed class ExpenseCreateRequest
{
    [Required]
    [MinLength(1)]
    [MaxLength(200)]
    public string Description { get; set; } = string.Empty;

    [Range(0.01, 999999999.99)]
    public decimal Amount { get; set; }

    public DateOnly Date { get; set; }

    public ExpenseCategory Category { get; set; }
}

public sealed class ExpenseUpdateRequest
{
    [Required]
    [MinLength(1)]
    [MaxLength(200)]
    public string Description { get; set; } = string.Empty;

    [Range(0.01, 999999999.99)]
    public decimal Amount { get; set; }

    public DateOnly Date { get; set; }

    public ExpenseCategory Category { get; set; }
}

public sealed record ExpenseQuery(
    DateOnly? DateFrom,
    DateOnly? DateTo,
    ExpenseCategory? Category,
    string? Search,
    int Page = 1,
    int PageSize = 50);

public sealed record PagedResult<T>(IReadOnlyList<T> Items, int TotalCount, int Page, int PageSize);

public sealed record CategorySummary(ExpenseCategory Category, decimal TotalAmount);

public sealed record DailySummary(DateOnly Date, decimal TotalAmount);

public sealed record ExpenseSummaryResponse(decimal TotalAmount, IReadOnlyList<CategorySummary> ByCategory, IReadOnlyList<DailySummary> ByDay);
