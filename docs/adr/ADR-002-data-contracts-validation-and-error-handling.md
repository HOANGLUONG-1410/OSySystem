# ADR-002: Chuẩn hoá Data Annotations, Custom Exceptions & ErrorResponse

- **Trạng thái:** Accepted (Đã duyệt)
- **Ngày:** 2026-11-10
- **Tác giả:** Nhóm phát triển (2 thành viên)
- **Dự án:** Quản lý Chuỗi Bún Bò (Bún Bò OSỹ)

---

## 1. Bối cảnh (Context)
Trước đây, các Service ném các Exception không đồng nhất (`UnauthorizedAccessException`, `KeyNotFoundException`, `InvalidOperationException`). Khi xảy ra lỗi, phản hồi trả về client đôi khi là thông báo kỹ thuật mặc định của .NET (tiếng Anh) hoặc lỗi 500 không mong muốn. Đồng thời, giáo viên yêu cầu sử dụng **Data Annotations** rõ ràng trên các Model và DTO.

---

## 2. Quyết định (Decisions)

### 2.1. Chuẩn hoá Data Annotations trên DTOs
Mọi DTO (Create/Update Request) đều phải khai báo Data Annotations:
- `[Required(ErrorMessage = "Trường này là bắt buộc.")]`
- `[StringLength(100, MinimumLength = 3, ErrorMessage = "Độ dài từ 3 đến 100 ký tự.")]`
- `[Range(0, 1000000000, ErrorMessage = "Giá trị không hợp lệ.")]`
- `[RegularExpression(...)]` cho mật khẩu và số điện thoại.

### 2.2. Bộ tứ Custom Exceptions chuẩn trong `Services/Exceptions/`
Nghiêm cấm ném `System.Exception` thông thường. Toàn bộ Service chỉ ném 4 loại Exception sau:
1. `BusinessException` (HTTP 400 Bad Request): Vi phạm logic kinh doanh (ví dụ: kho không đủ hàng, chuyển trạng thái đơn không hợp lệ).
2. `AuthenticationException` (HTTP 401 Unauthorized): Sai tên đăng nhập/mật khẩu, token không hợp lệ hoặc đã hết hạn.
3. `ForbiddenException` (HTTP 403 Forbidden): Không có quyền can thiệp vào chi nhánh khác hoặc hành vi vượt thẩm quyền Role.
4. `NotFoundException` (HTTP 404 Not Found): Không tìm thấy bản ghi theo Id chỉ định.

### 2.3. Cấu trúc Response Lỗi Chuẩn (`ErrorResponse`)
`Web_API/Middlewares/ExceptionMiddleware.cs` sẽ bắt mọi Exception và serialize thành JSON chuẩn:
```json
{
  "statusCode": 400,
  "message": "Số lượng nguyên liệu trong kho không đủ để xuất đơn hàng.",
  "details": null
}
```
**Quy tắc:** Tất cả `message` phải viết bằng **Tiếng Việt có dấu**, rõ ràng, dễ hiểu cho người dùng cuối.

---

## 3. Hệ quả (Consequences)
- Phía Client (`Web_Client`) dễ dàng bắt lỗi và hiển thị popup/toast notification tiếng Việt trực tiếp từ `response.message`.
- Kiểm soát hoàn toàn HTTP Status Codes theo chuẩn RESTful API.
