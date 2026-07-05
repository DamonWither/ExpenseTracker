using System.ComponentModel.DataAnnotations;
using ExpenseTracker.Api.Entities;
using ExpenseTracker.Api.Enums;

namespace ExpenseTracker.Api.Dtos;

/// <summary>
/// Общие свойства DTO расхода. Используется как базовый класс для Create/Update/Response.
/// Содержит атрибуты валидации для входящих моделей.
/// </summary>
public abstract class ExpenseDtoBase : IExpense
{
    /// <summary>Краткое описание расхода </summary>
    [Required]
    [MinLength(1)]
    [MaxLength(200)]
    public string Description { get; set; } = string.Empty;

    /// <summary>Сумма расхода </summary>
    [Range(0.01, 999999999.99)]
    public decimal Amount { get; set; }

    /// <summary>Дата расхода </summary>
    public DateOnly Date { get; set; }

    /// <summary>Категория расхода </summary>
    public ExpenseCategory Category { get; set; }
}