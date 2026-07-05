using ExpenseTracker.Api.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpenseTracker.Api.Migrations
{
    /// <summary>
    /// ћиграци€ InitialCreate:
    /// —оздаЄт таблицу "expenses" дл€ хранени€ расходов с пол€ми:
    /// - id (GUID) Ч первичный ключ
    /// - description (string, max 200) Ч описание расхода
    /// - amount (decimal numeric(12,2)) Ч сумма (>0)
    /// - date (date) Ч дата расхода
    /// - category (string, max 32) Ч категори€ (хранитс€ как строка)
    /// - created_at (timestamp with time zone) Ч врем€ создани€
    /// - updated_at (timestamp with time zone, nullable) Ч врем€ обновлени€
    /// “акже добавл€етс€ ограничение CHECK на amount > 0 и индексы по category и date.
    /// </summary>
    [DbContext(typeof(AppDbContext))]
    [Migration("202606300001_InitialCreate")]
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // —оздание таблицы "expenses" с указанными колонками и типами
            migrationBuilder.CreateTable(
                name: "expenses",
                columns: table => new
                {
                    // »дентификатор записи (GUID)
                    id = table.Column<Guid>(type: "uuid", nullable: false),

                    // ќписание расхода, об€зательно, maxLength 200
                    description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),

                    // —умма расхода, numeric(12,2) Ч в миграции добавлен CHECK amount > 0
                    amount = table.Column<decimal>(type: "numeric(12,2)", nullable: false),

                    // ƒата расхода, хранитс€ как date
                    date = table.Column<DateTime>(type: "date", nullable: false),

                    //  атегори€ как строка, maxLength 32
                    category = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),

                    // ¬рем€ создани€ (TIMESTAMP WITH TIME ZONE)
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),

                    // ¬рем€ обновлени€ (nullable)
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_expenses", x => x.id);
                    // ќграничение: сумма должна быть положительной
                    table.CheckConstraint("CK_expenses_amount_positive", "amount > 0");
                });

            // »ндекс по категории дл€ ускорени€ фильтрации
            migrationBuilder.CreateIndex(name: "IX_expenses_category", table: "expenses", column: "category");
            // »ндекс по дате дл€ ускорени€ диапазонных запросов
            migrationBuilder.CreateIndex(name: "IX_expenses_date", table: "expenses", column: "date");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // ќткат: удал€ем таблицу expenses
            migrationBuilder.DropTable(name: "expenses");
        }
    }
}
