using ExpenseTracker.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

#nullable disable

namespace ExpenseTracker.Api.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        /// <summary>
        /// Снимок модели EF Core для миграций.
        /// Описывает сущность Expense и её маппинг на таблицу "expenses".
        /// Поля сущности и соответствие колонкам:
        /// - Id (GUID) -> id
        /// - Description (string, max 200) -> description
        /// - Amount (decimal numeric(12,2)) -> amount
        /// - Date (DateTime / date) -> date
        /// - Category (string, max 32) -> category
        /// - CreatedAt (DateTimeOffset) -> created_at
        /// - UpdatedAt (DateTimeOffset?) -> updated_at
        /// Также определены индексы по полям Category и Date.
        /// </summary>
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity("ExpenseTracker.Api.Entities.Expense", b =>
            {
                // Идентификатор записи
                b.Property<Guid>("Id").HasColumnName("id");

                // Сумма с точностью numeric(12,2)
                b.Property<decimal>("Amount").HasColumnType("numeric(12,2)").HasColumnName("amount");

                // Категория как строка (ExpenseCategory -> string)
                b.Property<string>("Category").IsRequired().HasMaxLength(32).HasColumnName("category");

                // Время создания записи
                b.Property<DateTimeOffset>("CreatedAt").HasColumnName("created_at");

                // Дата расхода (хранится как date)
                b.Property<DateTime>("Date").HasColumnType("date").HasColumnName("date");

                // Описание расхода, обязательно, max 200
                b.Property<string>("Description").IsRequired().HasMaxLength(200).HasColumnName("description");

                // Время последнего обновления (nullable)
                b.Property<DateTimeOffset?>("UpdatedAt").HasColumnName("updated_at");

                b.HasKey("Id");
                b.HasIndex("Category");
                b.HasIndex("Date");
                b.ToTable("expenses");
            });
        }
    }
}
