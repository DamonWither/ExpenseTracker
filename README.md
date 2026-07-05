# Expense Tracker — веб-приложение «Трекер расходов»

Небольшое веб-приложение для учета личных расходов. Пользователь может добавлять расходы, смотреть список, фильтровать записи, удалять, редактировать их и видеть статистику по тратам.

Проект сделан под тестовое задание: акцент на понятную структуру, валидацию, обработку ошибок, REST API, PostgreSQL, EF Core, frontend на одной странице и Docker Compose.

## Стек

- Backend: ASP.NET Core 8 Web API
- Язык: C#
- ORM: Entity Framework Core
- БД: PostgreSQL
- Frontend: HTML/CSS/JavaScript + Chart.js
- Документация API: Swagger/OpenAPI
- Запуск: Docker Compose

## Возможности

- Добавление расхода: описание, сумма, дата, категория
- Просмотр списка расходов с сортировкой по дате от новых к старым
- Удаление расхода
- Редактирование расхода
- Фильтры:
  - дата с / дата по
  - категория
  - поиск по описанию
- Статистика:
  - общая сумма за выбранный период
  - сумма по категориям
  - график расходов по дням
- Пагинация списка расходов
- Swagger

## Категории

В приложении используются заранее заданные категории:

- `Food` — еда
- `Transport` — транспорт
- `Housing` — жилье
- `Entertainment` — развлечения
- `Health` — здоровье
- `Other` — другое

## Структура проекта

```text
ExpenseTracker/
├── src/
│   └── ExpenseTracker.Api/
│       ├── Controllers/        # REST API контроллеры
│       ├── Data/               # DbContext
│       ├── Dtos/               # DTO для запросов и ответов
│       ├── Entities/           # EF Core сущности
│       ├── Enums/              # Категории расходов
│       ├── Middleware/         # Глобальная обработка ошибок
│       ├── Migrations/         # EF Core миграция
│       ├── Services/           # Бизнес-логика
│       ├── wwwroot/            # Одностраничный frontend
│       ├── Dockerfile
│       └── Program.cs
├── docker-compose.yml
├── ExpenseTracker.sln
└── README.md
```

## Запуск через Docker

Требуется установленный Docker.

```bash
docker compose up --build
```

После запуска:

- Frontend: http://localhost:8080
- Swagger: http://localhost:8080/swagger
- PostgreSQL: `localhost:5432`

Данные для подключения к БД:

```text
Host: localhost
Port: 5432
Database: expense_tracker
Username: postgres
Password: postgres
```

Миграции применяются автоматически при старте API.

## Локальный запуск без Docker

Требуется:

- .NET 8 SDK
- PostgreSQL

1. Создать базу данных:

```sql
CREATE DATABASE expense_tracker;
```

2. Проверить строку подключения в `src/ExpenseTracker.Api/appsettings.json`:

```json
"DefaultConnection": "Host=localhost;Port=5432;Database=expense_tracker;Username=postgres;Password=postgres"
```

3. Запустить API:

```bash
dotnet restore
dotnet run --project src/ExpenseTracker.Api/ExpenseTracker.Api.csproj
```

4. Открыть приложение:

```text
http://localhost:5000
```

Если используется другой порт, его покажет `dotnet run`.

## REST API

### Получить список расходов

```http
GET /api/expenses
```

Параметры:

| Параметр | Описание |
|---|---|
| `dateFrom` | дата начала периода, например `2026-01-01` |
| `dateTo` | дата конца периода, например `2026-01-31` |
| `category` | категория, например `Food` |
| `search` | поиск по описанию |
| `page` | номер страницы |
| `pageSize` | размер страницы, максимум 100 |

Пример:

```http
GET /api/expenses?dateFrom=2026-01-01&dateTo=2026-01-31&category=Food&search=coffee
```

### Получить один расход

```http
GET /api/expenses/{id}
```

### Создать расход

```http
POST /api/expenses
Content-Type: application/json

{
  "description": "Кофе",
  "amount": 250,
  "date": "2026-01-10",
  "category": "Food"
}
```

### Обновить расход

```http
PUT /api/expenses/{id}
Content-Type: application/json

{
  "description": "Обед",
  "amount": 550,
  "date": "2026-01-10",
  "category": "Food"
}
```

### Удалить расход

```http
DELETE /api/expenses/{id}
```

### Получить статистику

```http
GET /api/expenses/summary?dateFrom=2026-01-01&dateTo=2026-01-31
```

Ответ содержит:

- `totalAmount` — общая сумма расходов
- `byCategory` — суммы по категориям
- `byDay` — суммы по дням для графика

## Валидация и ошибки

Реализована проверка:

- описание не должно быть пустым
- описание не длиннее 200 символов
- сумма должна быть больше 0
- `dateFrom` не может быть больше `dateTo`
- `page` должен быть больше 0
- `pageSize` должен быть от 1 до 100
- при удалении/обновлении несуществующей записи возвращается `404 Not Found`
- внутренние исключения не возвращаются пользователю напрямую

Покрыты сценарии:

- нельзя создать расход с нулевой суммой
- статистика корректно группирует расходы по категориям

## Принятые решения

1. **ASP.NET Core Web API** выбран как основной backend, потому что он соответствует стеку тестового задания и хорошо подходит для REST API.
2. **EF Core + PostgreSQL** используются для хранения расходов в реляционной БД и удобной работы с миграциями.
3. **Категория хранится enum-значением в сущности Expense**, чтобы на backend была строгая типизация, а в БД значение сохранялось понятной строкой.
4. **Фильтры реализованы на backend через параметры REST API**, чтобы список и статистика считались по одним правилам.
5. **Статистика считается отдельным сервисным методом**: сумма за период, группировка по категориям и группировка по дням.
6. **Frontend сделан без тяжелого фреймворка**, так как для задания достаточно одной аккуратной страницы.
7. **Chart.js** используется только для простого графика расходов по дням.
8. **Docker Compose** добавлен для быстрого запуска API и PostgreSQL одной командой.

## Troubleshooting

Если после обновления проекта при добавлении расхода появляется ошибка, пересоберите контейнеры и удалите старый volume базы данных:

```bash
docker compose down -v
docker compose up --build
```

Для проверки API откройте Swagger: `http://localhost:8080/swagger`.
