using ExpenseTracker.Api.Dtos;
using ExpenseTracker.Api.Enums;

namespace ExpenseTracker.Api.Entities;

/// <summary>
/// Сущность расхода. Представляет запись в таблице расходов с основными полями для CRUD и отчетности.
/// </summary>
public sealed class Expense : IExpense
{
    /// <summary>Идентификатор записи (GUID).</summary>
    public Guid Id { get; set; }

    /// <summary>Описание расхода. Ожидается непустая строка (макс 200 символов по конфигурации).</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>Сумма расхода (decimal).</summary>
    public decimal Amount { get; set; }

    /// <summary>Дата совершения расхода (DateOnly).</summary>
    public DateOnly Date { get; set; }

    /// <summary>Категория расхода (enum ExpenseCategory).</summary>
    public ExpenseCategory Category { get; set; }

    /// <summary>Время создания записи (DateTimeOffset, UTC).</summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>Время последнего обновления или null, если не изменялось.</summary>
    public DateTimeOffset? UpdatedAt { get; set; }
}
