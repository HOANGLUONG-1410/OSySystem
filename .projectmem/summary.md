# projectmem - OSySystem

_Last updated: 2026-11-13_

## Project purpose
Hệ thống Quản lý Chuỗi Bún Bò (Bếp Tổng và Chi nhánh Bán lẻ) phục vụ môn học PRN232 (C# & ASP.NET Core Web API 8.0, EF Core Code First, JWT Authentication, MVC Client calling API via AJAX).

## Recent issues
- No issues logged yet.

## Decisions
- PRN232 Teacher Requirements: Web API with 5 CRUD endpoints, OData, AutoMapper, Content Negotiation/Media Formatter, EF Core Code First, Data Annotations, AJAX Client, and JWT Auth. Unit tests are excluded per request.
- Architecture Layering: Strict flow Web_API Controller -> Service -> Repository -> DAO -> BunBoDbContext. Web_Client is Razor Views + pure AJAX calling Web_API with localStorage JWT.
- Error Handling: Global ExceptionMiddleware converts custom exceptions (BusinessException -> 400, AuthenticationException -> 401, ForbiddenException -> 403, NotFoundException -> 404) into unified ErrorResponse with Vietnamese messages. Never throw generic Exception.
- Bổ sung AutoMapper 12.0.1 (MappingProfile), OData 8.2.5 (EnableQuery), Content Negotiation & XmlSerializerFormatters, và cấu hình jQuery AJAX cho Web_Client.

## Notes
- gotcha: All read queries in DAOs must use AsNoTracking() to prevent tracking collision and memory overhead. All foreign entity mappings must be explicitly Included. [DataAccessObjects]
- Đã cấu hình AddControllers với RespectBrowserAcceptHeader = true, AddXmlSerializerFormatters(), AddOData(Select/Filter/OrderBy/Expand/Count), AutoMapper 12.0.1 và jQuery AJAX. Build 0 error.
- Đã cấu hình RollForward cho Web_API & Web_Client, kết nối SQL Server local bằng Windows Authentication (BunBoManagementDb), tự động migrate schema và seed đầy đủ data mẫu qua FINAL_SQL.sql.

## Key files
- `12.0.1`
- `8.2.5`
- `FINAL_SQL.sql`

## Open questions
- None logged yet.
