# Web Client OSỹ - Hướng dẫn chạy

## 1. Chạy Web API

Chọn profile HTTPS của `Web_API` hoặc chạy:

```powershell
dotnet run --project Web_API --launch-profile https
```

API mặc định: `https://localhost:7055`.

## 2. Chạy Web Client

```powershell
dotnet run --project Web_Client --launch-profile https
```

Web Client mặc định: `https://localhost:7165`.

## 3. Đăng nhập

Tài khoản seed mặc định:

- Username: `admin`
- Password: `Admin@123`

## 4. Nếu cổng API thay đổi

Sửa `Web_Client/appsettings.json`:

```json
"ApiSettings": {
  "BaseUrl": "https://localhost:7055"
}
```

## Các màn hình

- Login: đăng nhập JWT.
- Dashboard: tổng hợp Order, Inventory và Transfer.
- Inventory: xem kho, nhập kho, xem InventoryLog.
- Orders: tạo Order và test Confirm/Complete/Cancel.
- Transfers: tạo Transfer và test Send/Receive/Cancel.
- Catalog: xem Kitchen, Ingredient và MenuItem.
- API Console: gọi thủ công mọi endpoint còn lại.

JWT được lưu trong `localStorage` để phục vụ kiểm thử trực quan. Đây là cách đơn giản cho môi trường học tập/demo; hệ thống production nên dùng cookie bảo mật hoặc cơ chế token phù hợp hơn.
