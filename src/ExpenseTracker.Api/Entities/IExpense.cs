using ExpenseTracker.Api.Enums;

namespace ExpenseTracker.Api.Entities;

/// <summary>
/// Общие свойства расхода, которыми должны обладать DTO и сущность.
/// Размещён в Entities чтобы быть доступным как общий контракт в проекте.
/// </summary>
public interface IExpense
{
    string Description { get; set; }
    decimal Amount { get; set; }
    DateOnly Date { get; set; }
    ExpenseCategory Category { get; set; }
}