using ExpenseTracker.Api.Data;
using ExpenseTracker.Api.Dtos;
using ExpenseTracker.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Api.Services;

/// <summary>
/// Сервис работы с расходами: получение списка с фильтрацией/пагинацией,
/// CRUD операции и формирование сводки
/// </summary>
public sealed class ExpenseService(AppDbContext db)
{
    /// <summary>
    /// Получить список расходов по фильтру
    /// </summary>
    /// <param name="query">Параметры фильтра и пагинации</param>
    /// <param name="ct">Токен отмены</param>
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

    /// <summary>
    /// Получить расход по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор расхода.</param>
    /// <param name="ct">Токен отмены.</param>
    public async Task<ExpenseResponse?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var expense = await db.Expenses.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id, ct);
        return expense is null ? null : ToResponse(expense);
    }

    /// <summary>
    /// Создать новый расход
    /// </summary>
    /// <param name="request">Данные для создания.</param>
    /// <param name="ct">Токен отмены.</param>
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

    /// <summary>
    /// Обновить существующий расход по id
    /// </summary>
    /// <param name="id">Идентификатор обновляемого расхода.</param>
    /// <param name="request">Данные для обновления.</param>
    /// <param name="ct">Токен отмены.</param>
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

    /// <summary>
    /// Удалить расход по id
    /// </summary>
    /// <param name="id">Идентификатор удаляемого расхода.</param>
    /// <param name="ct">Токен отмены.</param>
    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct)
    {
        var deleted = await db.Expenses.Where(e => e.Id == id).ExecuteDeleteAsync(ct);
        return deleted > 0;
    }

    /// <summary>
    /// Получить сводку за период
    /// </summary>
    /// <param name="dateFrom">Начальная дата фильтра или null.</param>
    /// <param name="dateTo">Конечная дата фильтра или null.</param>
    /// <param name="ct">Токен отмены.</param>
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

    /// <summary>
    /// Применяет фильтры из ExpenseQuery к IQueryable<Expense>
    /// </summary>
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

    /// <summary>
    /// Проверяет корректность параметров запроса (диапазон дат, page, pageSize)
    /// Бросает ArgumentException при неправильных значениях
    /// </summary>
    private static void ValidateQuery(ExpenseQuery query)
    {
        if (query.DateFrom is not null && query.DateTo is not null && query.DateFrom > query.DateTo)
            throw new ArgumentException("Значение DateFrom должно быть меньше или равно значению DateTo");

        if (query.Page < 1)
            throw new ArgumentException("Страница должна быть больше 0");

        if (query.PageSize < 1 || query.PageSize > 100)
            throw new ArgumentException("Размер страницы должен быть от 1 до 100");
    }

    /// <summary>
    /// Проверяет корректность данных расхода (description, amount, date)
    /// Бросает ArgumentException при ошибке валидации
    /// </summary>
    private static void ValidateExpense(string description, decimal amount, DateOnly date)
    {
        if (string.IsNullOrWhiteSpace(description)) throw new ArgumentException("Описание не должно быть пустым");
        if (description.Trim().Length > 200) throw new ArgumentException("Длина описания должна быть не более 200 символов");
        if (amount <= 0) throw new ArgumentException("Сумма должна быть больше 0");
        if (date > DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(1))) throw new ArgumentException("Эта дата слишком далека от настоящего");
    }

    /// <summary>
    /// Преобразует сущность Expense в DTO ExpenseResponse
    /// </summary>
    private static ExpenseResponse ToResponse(Expense e) => new(e.Id, e.Description, e.Amount, e.Date, e.Category, e.CreatedAt, e.UpdatedAt);
}
