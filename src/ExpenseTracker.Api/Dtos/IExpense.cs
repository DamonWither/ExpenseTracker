using ExpenseTracker.Api.Enums;

namespace ExpenseTracker.Api.Dtos;

/// <summary>
/// ќбщие свойства расхода, которыми должны обладать DTO и сущность.
/// Ќе содержит атрибутов валидации Ч атрибуты оставл€ютс€ в DTO.
/// </summary>
public interface IExpense
{
    string Description { get; set; }
    decimal Amount { get; set; }
    DateOnly Date { get; set; }
    ExpenseCategory Category { get; set; }
}