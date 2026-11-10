# ADR-001: Yêu cầu Môn học PRN232 & Kiến trúc Hệ thống

- **Trạng thái:** Accepted (Đã duyệt)
- **Ngày:** 2026-11-10
- **Tác giả:** Nhóm phát triển (2 thành viên)
- **Dự án:** Quản lý Chuỗi Bún Bò (Bún Bò OSỹ)

---

## 1. Bối cảnh (Context)
Dự án được xây dựng phục vụ môn học **PRN232 (C# & ASP.NET Core Web API)**. Giáo viên yêu cầu sản phẩm phải thỏa mãn các tiêu chuẩn kỹ thuật cụ thể để chấm điểm đồ án, đồng thời hệ thống phải vận hành thực tế mô hình Bếp tổng và Chuỗi chi nhánh bán lẻ.

## 2. Tiêu chuẩn Bắt buộc từ Giáo viên (Teacher Checklist)
1. **Chủ đề:** Quản lý chuỗi nhà hàng Bún Bò (hợp lệ, thực tế).
2. **Tối thiểu 5 CRUD APIs:** Phải có đầy đủ `GET all items`, `GET item by id`, `POST`, `PUT`, `DELETE`.
3. **Thư viện OData:** Tích hợp `Microsoft.AspNetCore.OData` để hỗ trợ truy vấn nâng cao (`$filter`, `$select`, `$orderby`, `$top`, `$skip`).
4. **AutoMapper:** Sử dụng `AutoMapper` để chuyển đổi tự động giữa Entity và DTO.
5. **Media Formatter & Content Negotiation:** API phải hỗ trợ Content Negotiation (tối thiểu JSON và XML).
6. **Data Annotations:** Khai báo trên các Model/DTO để kiểm tra tính toàn vẹn dữ liệu.
7. **Gọi API bằng AJAX:** Giao diện `Web_Client` (MVC Razor) giao tiếp 100% với `Web_API` thông qua JavaScript AJAX/Fetch.
8. **JWT Authentication & Authorization:** Xác thực đăng nhập và phân quyền 4 Roles: `SystemAdmin`, `KitchenManager`, `BranchManager`, `Staff`.
9. **EF Core Code First:** Quản lý schema và migration trên SQL Server.
10. **Lưu ý:** Không yêu cầu viết Unit Tests.

---

## 3. Quyết định Kiến trúc (Decisions)

### 3.1. Mô hình Phân tầng (6 Projects)
Dự án được tổ chức theo kiến trúc phân tầng một chiều nghiêm ngặt:
- `BusinessObjects`: DbContext (`BunBoDbContext`), EF Core Migrations, Entities (`Kitchen`, `Ingredient`, `MenuItem`, `Recipe`, `Inventory`, `Order`, `Schedule`, ...), Enums.
- `DataAccessObjects`: Chứa các DAO thực thi LINQ/EF Core trực tiếp.
- `Repositories`: Interface và Implementation Repository bọc DAO + `UnitOfWork`.
- `Services`: Business logic, DTOs (Request/Response), Custom Exceptions, Mapping Profiles.
- `Web_API`: ASP.NET Core Web API, Swagger, JWT Auth, Middlewares, Controllers.
- `Web_Client`: ASP.NET Core MVC (chỉ chứa Razor Views và JS Client gọi AJAX).

### 3.2. Kế hoạch Tích hợp các Thư viện Bổ sung (OData, AutoMapper, Content Negotiation)
- **AutoMapper:** Cài đặt package `AutoMapper.Extensions.Microsoft.DependencyInjection` vào `Services` và `Web_API`. Tạo profile ánh xạ chung (`MappingProfile.cs`).
- **OData:** Cài đặt `Microsoft.AspNetCore.OData` vào `Web_API`. Đăng ký OData batching/query options trong `Program.cs` và thêm `[EnableQuery]` vào các controller danh mục (Ingredients, MenuItems, Orders, Inventories).
- **Content Negotiation:** Đăng ký `.AddXmlSerializerFormatters()` tại `builder.Services.AddControllers()` trong `Web_API/Program.cs`.

---

## 4. Hệ quả (Consequences)
- Đảm bảo 100% tiêu chí chấm điểm của giáo viên.
- Hai thành viên có kim chỉ nam rõ ràng khi chia việc (một người làm API / backend, một người làm UI / AJAX hoặc chia theo module chức năng).
