# ADR-003: Cơ chế Phối hợp Nhóm 2 Thành viên & Quản lý Living Context

- **Trạng thái:** Accepted (Đã duyệt)
- **Ngày:** 2026-11-10
- **Tác giả:** Nhóm phát triển (2 thành viên)
- **Dự án:** Quản lý Chuỗi Bún Bò (Bún Bò OSỹ)

---

## 1. Bối cảnh (Context)
Khi 2 lập trình viên cùng sử dụng các AI Coding Assistant (Cursor, Windsurf, Antigravity, VS Code Copilot), nguy cơ lớn nhất là **Context Drift** (mất đồng bộ ngữ cảnh):
- Thành viên A đổi logic một Service hoặc thêm Entity mới, nhưng AI của thành viên B không biết, dẫn đến sinh code lỗi thời hoặc xung đột.
- Các "bẫy" kỹ thuật (Gotchas như EF Core Tracking, quan hệ Circular Dependency) nếu một người đã gặp và sửa, người kia lại lặp lại.

---

## 2. Quyết định (Decisions)

### 2.1. Phân chia vai trò 2 công cụ cốt lõi: `projectmem` và `serena`
1. **`serena` (Codebase AST & Semantic Navigation):**
   - Đảm nhiệm tầng **Cấu trúc mã nguồn**: Phân tích AST, sơ đồ quan hệ class, interface, method signature.
   - Trước khi viết class mới hay implement endpoint mới, dev yêu cầu Agent dùng `serena` để quét xem interface/repository tương ứng đã có chưa.
2. **`projectmem` (Shared Mental State & Gotchas):**
   - Đảm nhiệm tầng **Bộ nhớ lâu dài & Trạng thái dự án**:
     - Lưu trữ quyết định: Sử dụng `add_decision` hoặc CLI `pjm decision`.
     - Lưu trữ kinh nghiệm/bẫy kỹ thuật: Sử dụng `add_note` hoặc CLI `pjm note "gotcha: ..."`.
     - Lưu trữ bug/lỗi đang xử lý: Sử dụng `log_issue` -> `record_attempt` -> `record_fix`.
   - Cả 2 thành viên đều sync thư mục `.projectmem/` qua Git để kế thừa toàn bộ bộ nhớ của nhau.

### 2.2. Living Context qua Repo-level Files
- File `.cursorrules` và `.windsurfrules` được commit ở root Git. Mọi Agent của cả 2 thành viên đều tự động nạp cùng 1 bộ luật khi mở workspace.
- Các quyết định lớn được ghi lại trong thư mục `docs/adr/`.

### 2.3. Quy trình làm việc hàng ngày của 2 Dev (Workflow)
1. **Bắt đầu phiên làm việc:**
   - Kéo code mới nhất từ Git (`git pull`).
   - Agent tự động nạp `.cursorrules` và gọi `pjm show` hoặc `projectmem.get_summary()` để cập nhật các quyết định và gotchas mới nhất của đồng đội.
2. **Trong quá trình làm việc:**
   - Khi phát hiện một lỗi đặc thù (ví dụ: EF Core tracking collision): Ghi chú ngay vào `pjm note "gotcha: ..."`.
   - Nếu thay đổi cách xử lý kiến trúc: Ghi nhận qua `pjm decision "..."`.
3. **Trước khi commit & push:**
   - Đảm bảo solution build thành công (`dotnet build` ra 0 error).
   - Commit kèm theo các cập nhật trong `.projectmem/events.jsonl` để đồng bộ trí não sang cho bạn đồng hành.

---

## 3. Hệ quả (Consequences)
- Triệt tiêu hoàn toàn tình trạng "AI của tôi sinh code khác AI của bạn".
- Rút ngắn thời gian họp bàn và giải quyết xung đột khi merge code.
