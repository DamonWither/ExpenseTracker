using ExpenseTracker.Api.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpenseTracker.Api.Migrations
{
    /// <summary>
    /// Миграция InitialCreate:
    /// Создаёт таблицу "expenses" для хранения расходов с полями
    /// - id (GUID) — первичный ключ
    /// - description (string, max 200) — описание расхода
    /// - amount (decimal numeric(12,2)) — сумма (>0)
    /// - date (date) — дата расхода
    /// - category (string, max 32) — категория (хранится как строка)
    /// - created_at (timestamp with time zone) — время создания
    /// - updated_at (timestamp with time zone, nullable) — время обновления
    /// Также добавляется ограничение CHECK на amount > 0 и индексы по category и date
    /// </summary>
    [DbContext(typeof(AppDbContext))]
    [Migration("202606300001_InitialCreate")]
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "expenses",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),

                    description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),

                    amount = table.Column<decimal>(type: "numeric(12,2)", nullable: false),

                    date = table.Column<DateTime>(type: "date", nullable: false),

                    category = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),

                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),

                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_expenses", x => x.id);
                    
                    table.CheckConstraint("CK_expenses_amount_positive", "amount > 0");
                });

            migrationBuilder.CreateIndex(name: "IX_expenses_category", table: "expenses", column: "category");
            
            migrationBuilder.CreateIndex(name: "IX_expenses_date", table: "expenses", column: "date");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "expenses");
        }
    }
}
