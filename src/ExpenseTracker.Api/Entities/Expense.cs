using ExpenseTracker.Api.Dtos;
using ExpenseTracker.Api.Enums;

namespace ExpenseTracker.Api.Entities;

/// <summary>
/// Сущность расхода. Представляет запись в таблице расходов с основными полями для CRUD и отчетности
/// </summary>
public sealed class Expense : IExpense
{
    /// <summary>Идентификатор записи </summary>
    public Guid Id { get; set; }

    /// <summary>Описание расхода </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>Сумма расхода </summary>
    public decimal Amount { get; set; }

    /// <summary>Дата совершения расхода </summary>
    public DateOnly Date { get; set; }

    /// <summary>Категория расхода </summary>
    public ExpenseCategory Category { get; set; }

    /// <summary>Время создания записи </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>Время последнего обновления </summary>
    public DateTimeOffset? UpdatedAt { get; set; }
}
