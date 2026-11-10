# projectmem - Project_PRN232_Backup

_Last updated: 2026-11-10_

## Project purpose
Hệ thống Quản lý Chuỗi Bún Bò (Bếp Tổng và Chi nhánh Bán lẻ) phục vụ môn học PRN232 (C# & ASP.NET Core Web API 8.0, EF Core Code First, JWT Authentication, MVC Client calling API via AJAX).

## Recent issues
- No issues logged yet.

## Decisions
- PRN232 Teacher Requirements: Web API with 5 CRUD endpoints, OData, AutoMapper, Content Negotiation/Media Formatter, EF Core Code First, Data Annotations, AJAX Client, and JWT Auth. Unit tests are excluded per request.
- Architecture Layering: Strict flow Web_API Controller -> Service -> Repository -> DAO -> BunBoDbContext. Web_Client is Razor Views + pure AJAX calling Web_API with localStorage JWT.
- Error Handling: Global ExceptionMiddleware converts custom exceptions (BusinessException -> 400, AuthenticationException -> 401, ForbiddenException -> 403, NotFoundException -> 404) into unified ErrorResponse with Vietnamese messages. Never throw generic Exception.

## Notes
- gotcha: All read queries in DAOs must use AsNoTracking() to prevent tracking collision and memory overhead. All foreign entity mappings must be explicitly Included. [DataAccessObjects]

## Key files
- No key files logged yet.

## Open questions
- None logged yet.
