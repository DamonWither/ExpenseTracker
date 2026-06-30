using ExpenseTracker.Api.Entities;
using ExpenseTracker.Api.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ExpenseTracker.Api.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Expense> Expenses => Set<Expense>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var dateOnlyConverter = new ValueConverter<DateOnly, DateTime>(
            dateOnly => DateTime.SpecifyKind(dateOnly.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc),
            dateTime => DateOnly.FromDateTime(DateTime.SpecifyKind(dateTime, DateTimeKind.Utc)));

        modelBuilder.Entity<Expense>(entity =>
        {
            entity.ToTable("expenses");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description).HasColumnName("description").HasMaxLength(200).IsRequired();
            entity.Property(e => e.Amount).HasColumnName("amount").HasColumnType("numeric(12,2)").IsRequired();
            entity.Property(e => e.Date).HasColumnName("date").HasColumnType("date").HasConversion(dateOnlyConverter).IsRequired();
            entity.Property(e => e.Category).HasColumnName("category").HasConversion<string>().HasMaxLength(32).IsRequired();
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.HasIndex(e => e.Date);
            entity.HasIndex(e => e.Category);
        });
    }
}
