using ExpenseTracker.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

#nullable disable

namespace ExpenseTracker.Api.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity("ExpenseTracker.Api.Entities.Expense", b =>
            {
                b.Property<Guid>("Id").HasColumnName("id");
                b.Property<decimal>("Amount").HasColumnType("numeric(12,2)").HasColumnName("amount");
                b.Property<string>("Category").IsRequired().HasMaxLength(32).HasColumnName("category");
                b.Property<DateTimeOffset>("CreatedAt").HasColumnName("created_at");
                b.Property<DateTime>("Date").HasColumnType("date").HasColumnName("date");
                b.Property<string>("Description").IsRequired().HasMaxLength(200).HasColumnName("description");
                b.Property<DateTimeOffset?>("UpdatedAt").HasColumnName("updated_at");
                b.HasKey("Id");
                b.HasIndex("Category");
                b.HasIndex("Date");
                b.ToTable("expenses");
            });
        }
    }
}
