using ExpenseTracker.Api.Dtos;
using ExpenseTracker.Api.Enums;
using ExpenseTracker.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ExpensesController(ExpenseService service) : ControllerBase
{
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

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ExpenseResponse>> GetById(Guid id, CancellationToken ct)
    {
        var expense = await service.GetByIdAsync(id, ct);
        return expense is null ? NotFound(new { error = "Expense not found." }) : Ok(expense);
    }

    [HttpPost]
    public async Task<ActionResult<ExpenseResponse>> Create([FromBody] ExpenseCreateRequest request, CancellationToken ct)
    {
        var created = await service.CreateAsync(request, ct);
        return Created($"/api/expenses/{created.Id}", created);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ExpenseResponse>> Update(Guid id, [FromBody] ExpenseUpdateRequest request, CancellationToken ct)
    {
        var updated = await service.UpdateAsync(id, request, ct);
        return updated is null ? NotFound(new { error = "Expense not found." }) : Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var deleted = await service.DeleteAsync(id, ct);
        return deleted ? NoContent() : NotFound(new { error = "Expense not found." });
    }

    [HttpGet("summary")]
    public async Task<ActionResult<ExpenseSummaryResponse>> Summary(
        [FromQuery] DateOnly? dateFrom,
        [FromQuery] DateOnly? dateTo,
        CancellationToken ct)
    {
        return Ok(await service.GetSummaryAsync(dateFrom, dateTo, ct));
    }
}
