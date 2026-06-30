using ExpenseTracker.Api.Data;
using ExpenseTracker.Api.Dtos;
using ExpenseTracker.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Api.Services;

public sealed class ExpenseService(AppDbContext db)
{
    public async Task<PagedResult<ExpenseResponse>> GetAsync(ExpenseQuery query, CancellationToken ct)
    {
        ValidateQuery(query);
        var expenses = ApplyFilters(db.Expenses.AsNoTracking(), query);
        var total = await expenses.CountAsync(ct);

        var page = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var items = await expenses
            .OrderByDescending(e => e.Date)
            .ThenByDescending(e => e.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(e => ToResponse(e))
            .ToListAsync(ct);

        return new PagedResult<ExpenseResponse>(items, total, page, pageSize);
    }

    public async Task<ExpenseResponse?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var expense = await db.Expenses.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id, ct);
        return expense is null ? null : ToResponse(expense);
    }

    public async Task<ExpenseResponse> CreateAsync(ExpenseCreateRequest request, CancellationToken ct)
    {
        ValidateExpense(request.Description, request.Amount, request.Date);

        var now = DateTimeOffset.UtcNow;
        var expense = new Expense
        {
            Id = Guid.NewGuid(),
            Description = request.Description.Trim(),
            Amount = request.Amount,
            Date = request.Date,
            Category = request.Category,
            CreatedAt = now
        };

        db.Expenses.Add(expense);
        await db.SaveChangesAsync(ct);
        return ToResponse(expense);
    }

    public async Task<ExpenseResponse?> UpdateAsync(Guid id, ExpenseUpdateRequest request, CancellationToken ct)
    {
        ValidateExpense(request.Description, request.Amount, request.Date);

        var expense = await db.Expenses.FirstOrDefaultAsync(e => e.Id == id, ct);
        if (expense is null) return null;

        expense.Description = request.Description.Trim();
        expense.Amount = request.Amount;
        expense.Date = request.Date;
        expense.Category = request.Category;
        expense.UpdatedAt = DateTimeOffset.UtcNow;

        await db.SaveChangesAsync(ct);
        return ToResponse(expense);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct)
    {
        var deleted = await db.Expenses.Where(e => e.Id == id).ExecuteDeleteAsync(ct);
        return deleted > 0;
    }

    public async Task<ExpenseSummaryResponse> GetSummaryAsync(DateOnly? dateFrom, DateOnly? dateTo, CancellationToken ct)
    {
        var query = new ExpenseQuery(dateFrom, dateTo, null, null, 1, 100);
        ValidateQuery(query);

        var expenses = ApplyFilters(db.Expenses.AsNoTracking(), query);
        var total = await expenses.SumAsync(e => (decimal?)e.Amount, ct) ?? 0m;

        var byCategoryRows = await expenses
            .GroupBy(e => e.Category)
            .Select(g => new { Category = g.Key, TotalAmount = g.Sum(e => e.Amount) })
            .OrderByDescending(x => x.TotalAmount)
            .ToListAsync(ct);

        var byDayRows = await expenses
            .GroupBy(e => e.Date)
            .Select(g => new { Date = g.Key, TotalAmount = g.Sum(e => e.Amount) })
            .OrderBy(x => x.Date)
            .ToListAsync(ct);

        var byCategory = byCategoryRows
            .Select(x => new CategorySummary(x.Category, x.TotalAmount))
            .ToList();

        var byDay = byDayRows
            .Select(x => new DailySummary(x.Date, x.TotalAmount))
            .ToList();

        return new ExpenseSummaryResponse(total, byCategory, byDay);
    }

    private static IQueryable<Expense> ApplyFilters(IQueryable<Expense> query, ExpenseQuery filter)
    {
        if (filter.DateFrom is not null) query = query.Where(e => e.Date >= filter.DateFrom.Value);
        if (filter.DateTo is not null) query = query.Where(e => e.Date <= filter.DateTo.Value);
        if (filter.Category is not null) query = query.Where(e => e.Category == filter.Category.Value);
        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var search = filter.Search.Trim().ToLower();
            query = query.Where(e => e.Description.ToLower().Contains(search));
        }
        return query;
    }

    private static void ValidateQuery(ExpenseQuery query)
    {
        if (query.DateFrom is not null && query.DateTo is not null && query.DateFrom > query.DateTo)
            throw new ArgumentException("dateFrom must be less than or equal to dateTo.");

        if (query.Page < 1)
            throw new ArgumentException("Page must be greater than 0.");

        if (query.PageSize < 1 || query.PageSize > 100)
            throw new ArgumentException("PageSize must be between 1 and 100.");
    }

    private static void ValidateExpense(string description, decimal amount, DateOnly date)
    {
        if (string.IsNullOrWhiteSpace(description)) throw new ArgumentException("Description must not be empty.");
        if (description.Trim().Length > 200) throw new ArgumentException("Description length must be 200 characters or less.");
        if (amount <= 0) throw new ArgumentException("Amount must be greater than 0.");
        if (date > DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(1))) throw new ArgumentException("Дата слишком отдалённая от настоящего");
    }

    private static ExpenseResponse ToResponse(Expense e) => new(e.Id, e.Description, e.Amount, e.Date, e.Category, e.CreatedAt, e.UpdatedAt);
}
