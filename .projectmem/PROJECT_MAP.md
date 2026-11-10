# Project Map - Project_PRN232_Backup

## Project purpose
Hệ thống Quản lý Chuỗi Bún Bò (Bếp Tổng và Chi nhánh Bán lẻ) phục vụ môn học PRN232 (C# & ASP.NET Core Web API 8.0, EF Core Code First, JWT Authentication, MVC Client calling API via AJAX).

## Structure
- `BusinessObjects/` — Domain entities, EF Core DbContext, Enums, and Migrations
  - `BusinessObjects/BunBoDbContext.cs` — Central EF Core DbContext
  - `BusinessObjects/Models/` — Entity models (Kitchen, SystemAccount, Ingredient, MenuItem, Recipe, Inventory, Order, Schedule, etc.)
  - `BusinessObjects/Enums/` — Domain enums (AccountRole, OrderStatus, TransferStatus, InventoryLogType)
- `DataAccessObjects/` — Direct data access layer utilizing DbContext with AsNoTracking
  - `DataAccessObjects/DAOs/` — DAO implementations (OrderDAO, InventoryDAO, ScheduleDAO, etc.)
- `Repositories/` — Repository pattern abstraction wrapping DAOs
  - `Repositories/Interfaces/` — Repository interfaces and IUnitOfWork
  - `Repositories/Implementations/` — Repository implementations and UnitOfWork
- `Services/` — Business logic services, DTOs, and Custom Exceptions
  - `Services/Interfaces/` — Service contracts (IOrderService, IInventoryService, IScheduleService, etc.)
  - `Services/Implementations/` — Service implementations containing core business rules
  - `Services/Exceptions/` — Custom exceptions (BusinessException, AuthenticationException, ForbiddenException, NotFoundException)
  - `Services/DTOs/` — Data transfer objects for requests and responses
- `Web_API/` — ASP.NET Core Web API exposing RESTful endpoints
  - `Web_API/Controllers/` — API controllers (Orders, Inventories, Schedules, Accounts, etc.)
  - `Web_API/Middlewares/ExceptionMiddleware.cs` — Global exception handler mapping to HTTP status codes with Vietnamese messages
  - `Web_API/DatabaseSeeder/DatabaseSeeder.cs` — Default seed accounts and sample data
- `Web_Client/` — ASP.NET Core MVC Frontend
  - `Web_Client/Controllers/` — MVC view controllers
  - `Web_Client/Views/` — Razor views for Admin, KitchenManager, BranchManager, and Staff
  - `Web_Client/wwwroot/js/` — Client-side JavaScript handling AJAX calls with JWT Bearer auth
- `docs/adr/` — Architecture Decision Records
  - `docs/adr/ADR-001-prn232-core-requirements-and-tech-stack.md` — Core teacher requirements & stack
  - `docs/adr/ADR-002-data-contracts-validation-and-error-handling.md` — Data annotations & exception standards
  - `docs/adr/ADR-003-shared-brain-and-collaboration-workflow.md` — Team collaboration workflow

## Relationships
- `Web_Client/Views` executes AJAX requests to `Web_API/Controllers` with JWT token.
- `Web_API/Controllers` invokes `Services/Implementations`.
- `Services/Implementations` validates inputs, throws custom exceptions, and calls `Repositories`.
- `Repositories` delegates queries and commands to `DataAccessObjects`.
- `DataAccessObjects` queries `BusinessObjects/BunBoDbContext`.
- `Web_API/Middlewares/ExceptionMiddleware.cs` catches custom exceptions and returns formatted Vietnamese ErrorResponse.
