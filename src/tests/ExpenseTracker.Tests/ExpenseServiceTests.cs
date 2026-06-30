using ExpenseTracker.Api.Data;
using ExpenseTracker.Api.Dtos;
using ExpenseTracker.Api.Enums;
using ExpenseTracker.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Tests;

public sealed class ExpenseServiceTests
{
    [Fact]
    public async Task CreateAsync_Rejects_NonPositiveAmount()
    {
        var service = CreateService();
        var request = new ExpenseCreateRequest("Coffee", 0, DateOnly.FromDateTime(DateTime.UtcNow), ExpenseCategory.Food);
        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAsync(request, CancellationToken.None));
    }

    [Fact]
    public async Task GetSummaryAsync_Groups_By_Category()
    {
        var service = CreateService();
        var date = new DateOnly(2026, 1, 10);

        await service.CreateAsync(new ExpenseCreateRequest("Coffee", 100, date, ExpenseCategory.Food), CancellationToken.None);
        await service.CreateAsync(new ExpenseCreateRequest("Taxi", 250, date, ExpenseCategory.Transport), CancellationToken.None);
        await service.CreateAsync(new ExpenseCreateRequest("Lunch", 300, date, ExpenseCategory.Food), CancellationToken.None);

        var summary = await service.GetSummaryAsync(new DateOnly(2026, 1, 1), new DateOnly(2026, 1, 31), CancellationToken.None);

        Assert.Equal(650, summary.TotalAmount);
        Assert.Contains(summary.ByCategory, x => x.Category == ExpenseCategory.Food && x.TotalAmount == 400);
        Assert.Contains(summary.ByCategory, x => x.Category == ExpenseCategory.Transport && x.TotalAmount == 250);
    }

    private static ExpenseService CreateService()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ExpenseService(new AppDbContext(options));
    }
}
