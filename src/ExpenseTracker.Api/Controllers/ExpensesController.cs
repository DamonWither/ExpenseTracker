using ExpenseTracker.Api.Dtos;
using ExpenseTracker.Api.Enums;
using ExpenseTracker.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Api.Controllers;

/// <summary>
/// Контроллер работы с расходами.
/// Поддерживает получение списка с фильтрацией/пагинацией, CRUD и сводку по периодам.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public sealed class ExpensesController(ExpenseService service) : ControllerBase
{
    /// <summary>
    /// Получить список расходов с фильтрацией и пагинацией.
    /// Параметры запроса: dateFrom, dateTo (DateOnly), category (ExpenseCategory), search (по описанию), page, pageSize.
    /// Возвращает PagedResult<ExpenseResponse> (items, totalCount, page, pageSize).
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<PagedResult<ExpenseResponse>>> Get(
        [FromQuery] DateOnly? dateFrom,
        [FromQuery] DateOnly? dateTo,
        [FromQuery] ExpenseCategory? category,
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken ct = default)
    {
        return Ok(await service.GetAsync(new ExpenseQuery(dateFrom, dateTo, category, search, page, pageSize), ct));
    }

    /// <summary>
    /// Получить расход по идентификатору.
    /// Параметр: id (GUID).
    /// Возвращает ExpenseResponse или 404, если не найден.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ExpenseResponse>> GetById(Guid id, CancellationToken ct)
    {
        var expense = await service.GetByIdAsync(id, ct);
        return expense is null ? NotFound(new { error = "Expense not found." }) : Ok(expense);
    }

    /// <summary>
    /// Создать новый расход.
    /// Тело запроса: ExpenseCreateRequest (Description, Amount, Date, Category).
    /// Возвращает Created (201) с ExpenseResponse.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ExpenseResponse>> Create([FromBody] ExpenseCreateRequest request, CancellationToken ct)
    {
        var created = await service.CreateAsync(request, ct);
        return Created($"/api/expenses/{created.Id}", created);
    }

    /// <summary>
    /// Обновить существующий расход по id.
    /// Параметр: id (GUID). Тело: ExpenseUpdateRequest (Description, Amount, Date, Category).
    /// Возвращает обновлённый ExpenseResponse или 404, если не найден.
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ExpenseResponse>> Update(Guid id, [FromBody] ExpenseUpdateRequest request, CancellationToken ct)
    {
        var updated = await service.UpdateAsync(id, request, ct);
        return updated is null ? NotFound(new { error = "Expense not found." }) : Ok(updated);
    }

    /// <summary>
    /// Удалить расход по id.
    /// Параметр: id (GUID).
    /// Возвращает 204 NoContent при успешном удалении или 404, если не найден.
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var deleted = await service.DeleteAsync(id, ct);
        return deleted ? NoContent() : NotFound(new { error = "Expense not found." });
    }

    /// <summary>
    /// Получить сводку по расходам за период.
    /// Параметры запроса: dateFrom, dateTo (DateOnly).
    /// Возвращает ExpenseSummaryResponse: общая сумма, суммарно по категориям и по дням.
    /// </summary>
    [HttpGet("summary")]
    public async Task<ActionResult<ExpenseSummaryResponse>> Summary(
        [FromQuery] DateOnly? dateFrom,
        [FromQuery] DateOnly? dateTo,
        CancellationToken ct)
    {
        return Ok(await service.GetSummaryAsync(dateFrom, dateTo, ct));
    }
}
