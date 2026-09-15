# TỔ CHỨC TÀI CHÍNH VI MÔ CEP
## PHÒNG CÔNG NGHỆ THÔNG TIN
### BÀI KIỂM TRA NĂNG LỰC THỰC HÀNH - VỊ TRÍ CHUYÊN VIÊN PHÁT TRIỂN PHẦN MỀM (.NET/BLAZOR)

---

# PHẦN A: PHÂN TÍCH VÀ LẬP KẾ HOẠCH DỰ ÁN
**Dự án:** Hệ thống Quản lý Khách hàng cơ bản (Mini Customer Management System)  
**Thời gian thực hiện:** Từ ngày 15/9/2026 đến 17h00 ngày 18/9/2026 (03 ngày)  
**Kế hoạch cam kết hoàn thành:** 24h00 ngày 17/9/2026 (Hoàn tất sớm hơn 01 ngày)  
**Ứng viên:** Đàm Văn Phương - Vị trí: .NET/Blazor Developer  

---

## 1. PHÂN TÍCH CHỨC NĂNG & GIAO DIỆN (UI)

### 1.1. Bối cảnh Nghiệp vụ Tài chính Vi mô CEP
Tổ chức Tài chính Vi mô CEP hoạt động với sứ mệnh cung cấp tín dụng nhỏ, dịch vụ tiết kiệm cho công nhân, người lao động nghèo và người có thu nhập thấp. Vì vậy, hệ thống CRM quản lý khách hàng cần đảm bảo:
- **Độ chính xác dữ liệu:** Số điện thoại là kênh liên lạc trọng yếu để định danh, đối chiếu giải ngân và quản lý nợ.
- **Tuổi giao dịch hợp pháp:** Khách hàng phải từ đủ 18 tuổi trở lên (đủ năng lực hành vi dân sự theo quy định pháp luật).
- **Phân loại đối tượng:** Phân định rõ nhóm đối tượng lao động (Công nhân khu công nghiệp, Tiểu thương buôn bán nhỏ, Lao động tự do) để phục vụ chính sách sản phẩm vay phù hợp.
- **Bảo mật và Phân quyền:** Ngăn chặn truy cập trái phép bằng cơ chế JWT Authentication.

---

### 1.2. Danh Sách & Mô Tả Chi Tiết Các Chức Năng (Tổng cộng: 08 Chức năng)

| STT | Mã Chức Năng | Tên Chức Năng | Phân Loại | Mô Tả Nghiệp Vụ & Quy Tắc |
| :---: | :--- | :--- | :---: | :--- |
| **1** | **AUTH-01** | Đăng nhập hệ thống | Điểm cộng (Bonus) | Cho phép nhân viên/quản trị viên đăng nhập bằng tài khoản được cấp (`admin`). Hệ thống xác thực và cấp mã JWT Bearer Token (thời hạn 8 giờ) để bảo vệ toàn bộ API và phân quyền thao tác. |
| **2** | **CUST-01** | Xem danh sách khách hàng | Cốt lõi (Bắt buộc) | Hiển thị bảng khách hàng dạng lưới (`MudDataGrid`) với đầy đủ các cột: Mã KH, Họ và tên, Email, Số điện thoại, Ngày sinh, Phân loại, Trạng thái, Ngày tạo. Hỗ trợ phân trang động (Pagination: 10, 25, 50 dòng/trang) và sắp xếp đa cột (Sorting). |
| **3** | **CUST-02** | Tìm kiếm & Lọc khách hàng | Cốt lõi (Bắt buộc) | - **Tìm kiếm:** Ô tìm kiếm tức thời theo từ khóa không dấu hoặc có dấu trên trường `Họ và tên` hoặc `Số điện thoại`.<br>- **Bộ lọc:** Lọc theo `Trạng thái hoạt động` (Tất cả, Đang hoạt động, Tạm ngưng, Đã khóa) và theo `Phân loại khách hàng`. |
| **4** | **CUST-03** | Thêm mới khách hàng | Cốt lõi (Bắt buộc) | Mở giao diện Dialog tạo mới khách hàng với các ràng buộc nghiêm ngặt:<br>• *Mã KH:* Tự động sinh chuẩn quy cách `KH-YYYY-XXXX` hoặc cho phép nhập thủ công, kiểm tra không trùng lặp (Unique).<br>• *Họ tên:* Bắt buộc, độ dài 2-100 ký tự.<br>• *Số điện thoại:* Bắt buộc, định dạng 10 chữ số di động Việt Nam.<br>• *Email:* Kiểm tra định dạng RFC tiêu chuẩn.<br>• *Ngày sinh:* Bắt buộc, tuổi từ 18 trở lên.<br>• *Trạng thái:* Mặc định "Đang hoạt động". |
| **5** | **CUST-04** | Cập nhật thông tin khách hàng | Cốt lõi (Bắt buộc) | Cho phép sửa đổi thông tin cá nhân (Họ tên, Email, Số điện thoại, Ngày sinh, Phân loại, Trạng thái). Không cho phép sửa đổi Mã KH (để bảo toàn tính định danh). Hệ thống tự động ghi nhận thời gian chỉnh sửa (`UpdatedAt`). |
| **6** | **CUST-05** | Xóa khách hàng (Xóa mềm) | Cốt lõi (Bắt buộc) | Áp dụng cơ chế **Soft Delete** (`IsDeleted = true`). Khi người dùng nhấn nút Xóa, hệ thống bật Dialog cảnh báo xác nhận. Bản ghi không bị xóa vĩnh viễn khỏi CSDL vật lý nhằm bảo toàn tính toàn vẹn dữ liệu kế toán/tín dụng của ngân hàng. |
| **7** | **DASH-01** | Thống kê Dashboard KPI | Giá trị gia tăng | Hiển thị các thẻ chỉ số trực quan ở đầu trang: Tổng số khách hàng, Số khách hàng đang hoạt động, Số khách hàng tạm ngưng, Khách hàng mới phát triển trong tháng. |
| **8** | **EXCL-01** | Xuất báo cáo Excel | Giá trị gia tăng | Cho phép xuất toàn bộ danh sách khách hàng đang hiển thị hoặc theo bộ lọc ra file Microsoft Excel (`.xlsx`) định dạng chuẩn để in ấn hoặc nộp báo cáo. |

---

### 1.3. Danh Sách & Bố Cục Các Màn Hình Giao Diện (UI) (Tổng cộng: 03 Màn hình chính)

1. **Màn hình Đăng nhập (Login Screen - `/login`):**
   - Thiết kế hiện đại, responsive theo tông màu xanh chủ đạo của thương hiệu CEP.
   - Form nhập: Tên đăng nhập (`Username`), Mật khẩu (`Password`).
   - Hiển thị thông báo lỗi khi thông tin sai lệch; tự động lưu JWT vào `LocalStorage` và chuyển hướng đến trang Dashboard sau khi đăng nhập thành công.

2. **Màn hình Tổng quan Dashboard & Quản lý Khách hàng (Customer Dashboard - `/customers`):**
   - **Thanh Header & Navigation:** Logo CEP, thông tin tài khoản đăng nhập (`Admin User`), nút Chuyển đổi giao diện Sáng/Tối (Light/Dark Theme) và nút Đăng xuất.
   - **Khu vực KPI Cards:** 04 thẻ số liệu thống kê nhanh với hiệu ứng trực quan.
   - **Thanh công cụ (Action Bar):**
     - Ô tìm kiếm tức thì theo Tên/SĐT (`MudTextField` với icon tìm kiếm).
     - Bộ lọc chọn trạng thái (`MudSelect`).
     - Nút "Thêm khách hàng" (`MudButton` màu xanh thương hiệu CEP, icon dấu cộng).
     - Nút "Xuất Excel" (`MudButton` icon tải về).
   - **Khu vực Bảng Dữ liệu (`MudDataGrid`):**
     - Danh sách khách hàng với badge màu hiển thị trạng thái (Xanh: Hoạt động, Vàng: Tạm ngưng, Đỏ: Đã khóa).
     - Cột Thao tác (Actions): Nút Sửa (Icon bút chì) và Nút Xóa (Icon thùng rác).
     - Phân trang dưới chân bảng: Lựa chọn số dòng hiển thị và chuyển trang linh hoạt.

3. **Màn hình Hộp thoại Nghiệp vụ (Modal Dialogs):**
   - **Dialog Thêm mới / Chỉnh sửa khách hàng (`CustomerDialog`):** Form popup căn giữa màn hình, hỗ trợ kiểm tra lỗi trực quan theo thời gian thực (Form Validation) trước khi gửi yêu cầu tới API.
   - **Dialog Xác nhận xóa (`DeleteConfirmDialog`):** Cảnh báo màu đỏ nổi bật, thông báo rõ tên khách hàng sắp bị xóa và nút "Xác nhận xóa" / "Hủy bỏ".

---

## 2. THIẾT KẾ CƠ SỞ DỮ LIỆU (DATABASE DESIGN)

### 2.1. Cấu Trúc Các Bảng Dữ Liệu (Schema Design)

Hệ thống được thiết kế với **02 bảng chính**: `Customers` và `Users`.

#### Bảng 1: `Customers` (Quản lý Khách hàng)
Lưu trữ thông tin hồ sơ khách hàng của tổ chức tài chính vi mô.

| Tên Trường (Column) | Kiểu Dữ Liệu | Ràng Buộc (Constraints) | Ý Nghĩa / Mục Đích Nghiệp Vụ |
| :--- | :--- | :--- | :--- |
| `Id` | `UNIQUEIDENTIFIER` | Primary Key, Default: `NEWID()` | Khóa chính định danh duy nhất bản ghi |
| `CustomerCode` | `NVARCHAR(20)` | NOT NULL, **UNIQUE INDEX** | Mã khách hàng (VD: `KH-2026-0001`), không trùng lặp |
| `FullName` | `NVARCHAR(100)` | NOT NULL, **INDEX** | Họ và tên khách hàng (được đánh Index tối ưu tìm kiếm) |
| `Email` | `VARCHAR(100)` | NULL | Hộp thư điện tử liên hệ |
| `PhoneNumber` | `VARCHAR(15)` | NOT NULL, **INDEX** | Số điện thoại di động (Index phục vụ tìm kiếm nhanh) |
| `DateOfBirth` | `DATE` | NOT NULL | Ngày tháng năm sinh (Kiểm tra tuổi >= 18) |
| `Segment` | `INT` | NOT NULL, Default: 1 | Phân loại KH (1: Công nhân, 2: Tiểu thương, 3: Tự do) |
| `Status` | `INT` | NOT NULL, Default: 1 | Trạng thái (1: Hoạt động, 2: Tạm ngưng, 3: Đã khóa) |
| `IsDeleted` | `BIT` | NOT NULL, Default: 0, **FILTERED INDEX** | Cờ xóa mềm (0: Đang tồn tại, 1: Đã xóa) |
| `CreatedAt` | `DATETIME2` | NOT NULL, Default: `SYSUTCDATETIME()` | Thời điểm tạo hồ sơ |
| `UpdatedAt` | `DATETIME2` | NULL | Thời điểm cập nhật hồ sơ gần nhất |
| `CreatedBy` | `NVARCHAR(50)` | NULL | Người thực hiện tạo |
| `UpdatedBy` | `NVARCHAR(50)` | NULL | Người thực hiện cập nhật |

#### Bảng 2: `Users` (Quản lý Tài khoản & Xác thực JWT)
Lưu trữ tài khoản nhân viên/quản trị viên đăng nhập hệ thống.

| Tên Trường (Column) | Kiểu Dữ Liệu | Ràng Buộc (Constraints) | Ý Nghĩa / Mục Đích Nghiệp Vụ |
| :--- | :--- | :--- | :--- |
| `Id` | `UNIQUEIDENTIFIER` | Primary Key, Default: `NEWID()` | Khóa chính người dùng |
| `Username` | `VARCHAR(50)` | NOT NULL, **UNIQUE INDEX** | Tên đăng nhập (VD: `admin`) |
| `PasswordHash` | `NVARCHAR(MAX)` | NOT NULL | Mật khẩu băm an toàn (BCrypt / PBKDF2) |
| `FullName` | `NVARCHAR(100)` | NOT NULL | Họ và tên cán bộ nhân viên |
| `Email` | `VARCHAR(100)` | NULL | Email cán bộ nhân viên |
| `Role` | `VARCHAR(20)` | NOT NULL, Default: `'Admin'` | Phân quyền vai trò hệ thống (`Admin`, `Staff`) |
| `IsActive` | `BIT` | NOT NULL, Default: 1 | Trạng thái tài khoản (1: Kích hoạt, 0: Vô hiệu) |
| `CreatedAt` | `DATETIME2` | NOT NULL, Default: `SYSUTCDATETIME()` | Thời điểm tạo tài khoản |

---

### 2.2. Phân Tích Chuyên Sâu: Có Cần Sử Dụng Stored Procedure Hay Trigger Không?

Đây là câu hỏi cốt lõi được đặt ra trong đề thi tuyển dụng. Ứng viên đóng vai trò là Kiến trúc sư hệ thống và Chuyên gia CSDL xin phân tích và đưa ra giải pháp kỹ thuật như sau:

#### A. Đối với Stored Procedure (SP):
* **Đánh giá thực tế với bài toán Mini CRM:**
  - Trong mô hình phát triển hiện đại với **Entity Framework Core (EF Core) Code-First**, toàn bộ các truy vấn CRUD, phân trang và tìm kiếm đều được biên dịch thành mã SQL tham số hóa (`Parameterized SQL`) tối ưu thông qua LINQ, hoàn toàn miễn nhiễm với tấn công SQL Injection và tận dụng tốt cơ chế Execution Plan Caching của CSDL.
  - Việc duy trì nghiệp vụ CRUD trong Stored Procedure sẽ làm **phân mảnh logic** (logic bị xé lẻ giữa C# Application Layer và Database Layer), gây khó khăn cho việc quản lý mã nguồn qua Git (Version Control), khó viết Unit Test tự động, và giảm tính linh động khi cần chuyển đổi giữa SQL Server và PostgreSQL.
* **Kết luận & Đề xuất kiến trúc:**
  - Với phạm vi bài toán Mini CRM, **KHÔNG CẦN THIẾT** phải dùng Stored Procedure cho các tác vụ CRUD cơ bản. Việc sử dụng **EF Core Code-First kết hợp Repository Pattern và Compiled Query/LINQ** đem lại tốc độ phát triển nhanh, code sạch sẽ, dễ bảo trì và dễ test hơn rất nhiều.
  - Tuy nhiên, trong tương lai khi hệ thống CEP phát triển đến quy mô hàng triệu bản ghi và cần thực hiện các báo cáo tổng hợp tài chính đa chiều cực lớn (Financial Reconciliation / Year-end Closing), ta có thể tích hợp Stored Procedure chuyên biệt thông qua phương thức `FromSqlRaw()` của EF Core.

#### B. Đối với Trigger:
* **Đánh giá thực tế với bài toán Mini CRM:**
  - Trigger thực thi ngầm phía máy chủ CSDL mà không thông qua tầng ứng dụng, dẫn đến hiện tượng "Side-effect ngầm" (khó debug, khó bắt lỗi và làm chậm hiệu năng của các lệnh `INSERT`/`UPDATE` hàng loạt).
  - Tác vụ cập nhật cờ `IsDeleted` (Soft Delete) và ghi nhận thời gian `UpdatedAt` hoàn toàn được kiểm soát một cách tường minh, tự động và chuẩn mực thông qua việc ghi đè phương thức `SaveChangesAsync()` trong lớp `ApplicationDbContext` của EF Core (Interceptors / Entity Lifecycle Hooks).
* **Kết luận & Đề xuất kiến trúc:**
  - Với bài toán hiện tại, **KHÔNG CẦN SỬ DỤNG TRIGGER**. Thay vào đó, toàn bộ nghiệp vụ Soft Delete và Audit Trail được xử lý tập trung, an toàn tại tầng Infrastructure của .NET.
  - *Lưu ý nâng cao:* Trigger chỉ nên được xem xét trong các kịch bản bắt buộc ghi Audit Log độc lập ở mức Database (ví dụ bảng `CustomerAuditLogs`) khi có yêu cầu thanh tra nghiêm ngặt từ Ngân hàng Nhà nước mà ứng dụng không được phép can thiệp.

---

## 3. BẢNG ƯỚC LƯỢNG THỜI GIAN CHI TIẾT (ESTIMATION WBS)

Bảng phân rã công việc chi tiết được xây dựng theo tiến trình **4 tiếng/buổi tối**, đảm bảo hoàn tất toàn bộ sản phẩm vào **24h00 ngày 17/9/2026** (trước hạn nộp chính thức 01 ngày):

| Mã WBS | Hạng Mục Công Việc Chi Tiết | Phân Vai Thực Hiện | Ước Lượng (Giờ) | Khung Thời Gian Dự Kiến | Kết Quả Bàn Giao (Deliverables) |
| :---: | :--- | :---: | :---: | :---: | :--- |
| **WBS-01** | **PHÂN TÍCH & LẬP KẾ HOẠCH (PHẦN A)** | | **4.0h** | **Tối 15/9 (20:30 - 00:30)** | |
| 01.1 | Phân tích nghiệp vụ tín dụng CEP, định danh các quy tắc Validation | Business Analyst | 1.0h | 20:30 - 21:30 | Danh mục 8 tính năng, 3 màn hình UI |
| 01.2 | Thiết kế Schema CSDL, tối ưu Index, phân tích Stored Procedure & Trigger | Database Architect | 1.0h | 21:30 - 22:30 | ERD thiết kế bảng `Customers`, `Users` |
| 01.3 | Lập bảng Estimation chi tiết theo giờ, thiết lập timeline 3 ngày | Project Manager | 1.0h | 22:30 - 23:30 | Bảng WBS phân rã công việc |
| 01.4 | Định dạng tài liệu, xuất file PDF chính thức Phần A | Documentation Lead | 1.0h | 23:30 - 00:30 | File `TaiLieuPhanTich_KeHoach_PhanA.pdf` |
| **WBS-02** | **THIẾT LẬP KIẾN TRÚC & CSDL (BACKEND CORE)** | | **4.0h** | **Tối 16/9 (20:00 - 24:00 - Phần 1)**| |
| 02.1 | Khởi tạo Solution .NET 8 chuẩn Clean Architecture 4 tầng | System Architect | 1.0h | 20:00 - 21:00 | Solution `CrmCep.sln` chuẩn cấu trúc |
| 02.2 | Xây dựng Domain Entities, Enums, BaseEntity, Auditable Rules | Senior .NET Dev | 1.0h | 21:00 - 22:00 | Lớp thực thể C# 12 chuẩn OOP |
| 02.3 | Cấu hình EF Core DbContext, Fluent API mapping, Index | Database Architect | 1.0h | 22:00 - 23:00 | Cấu hình SQL Server + PostgreSQL |
| 02.4 | Tạo Migrations ban đầu (`InitialCreate`) và nạp Seed Data mẫu | Senior .NET Dev | 1.0h | 23:00 - 24:00 | CSDL tạo thành công trên SQL Server |
| **WBS-03** | **TRIỂN KHAI REST API & XÁC THỰC JWT (BACKEND)** | | **4.0h** | **Tối 16/9 (Bổ trợ) & 17/9 (Sớm)**| |
| 03.1 | Xây dựng Authentication Service & JWT Token Generator (Bonus) | Security Architect | 1.0h | 20:00 - 21:00 | Auth Controller, Endpoint `/api/auth/login` |
| 03.2 | Xây dựng Customer CRUD API (List, GetById, Create, Update, Delete) | Senior .NET Dev | 1.5h | 21:00 - 22:30 | Customers Controller hoàn thiện |
| 03.3 | Xây dựng bộ tìm kiếm theo Tên/SĐT, lọc Trạng thái và Phân trang | Senior .NET Dev | 0.5h | 22:30 - 23:00 | Tìm kiếm tức thì, phân trang linh hoạt |
| 03.4 | Cấu hình FluentValidation, Global Exception Middleware & Postman JSON | Code Review & QA | 1.0h | 23:00 - 24:00 | File `CrmCep_Postman_Collection.json` |
| **WBS-04** | **TRIỂN KHAI GIAO DIỆN BLAZOR & MUDBLAZOR (FRONTEND)**| | **4.0h** | **Tối 17/9 (20:00 - 24:00)** | |
| 04.1 | Cấu hình Blazor Web App, tích hợp MudBlazor và tùy biến Theme CEP | UI/UX Developer | 0.5h | 20:00 - 20:30 | Giao diện Dashboard & Navbar chuyên nghiệp |
| 04.2 | Xây dựng Màn hình Đăng nhập JWT, Custom AuthenticationStateProvider | Senior .NET Dev | 1.0h | 20:30 - 21:30 | Luồng Login mượt mà, lưu trữ Token |
| 04.3 | Xây dựng Bảng dữ liệu Khách hàng với MudDataGrid (Search, Filter, Paging) | Senior .NET Dev | 1.0h | 21:30 - 22:30 | DataGrid hiện đại, chuẩn UI/UX |
| 04.4 | Xây dựng Dialog Thêm/Sửa Khách hàng (Validation trực quan) | Senior .NET Dev | 0.75h| 22:30 - 23:15 | Dialog thêm/sửa thân thiện người dùng |
| 04.5 | Xây dựng Dialog Xác nhận Xóa (Soft Delete) & Toast thông báo | Senior .NET Dev | 0.75h| 23:15 - 24:00 | Xóa an toàn kèm Snackbar Toast |
| **WBS-05** | **ĐÓNG GÓI, TÀI LIỆU README & BÀN GIAO** | | **2.0h** | **Đêm 17/9 (23:00 - 01:00)** | |
| 05.1 | Viết file `README.md` toàn diện theo tiêu chí đề bài tuyển dụng | Technical Writer | 1.0h | 23:00 - 24:00 | File `README.md` chuẩn mực |
| 05.2 | Xuất tài liệu Hướng dẫn Cài đặt & HDSD dạng PDF | Documentation Lead | 0.5h | 00:00 - 00:30 | File `HuongDanCaiDat_SuDung.pdf` |
| 05.3 | Review code toàn bộ (English comments), Push source code lên GitHub | Code Review & QA | 0.5h | 00:30 - 01:00 | Repo GitHub cập nhật đầy đủ nhánh `dev` |

---

### KẾT LUẬN PHẦN A:
Phương án trên đảm bảo bám sát 100% yêu cầu đề thi của Tổ chức Tài chính Vi mô CEP, tận dụng tối đa thế mạnh của nền tảng **.NET 8**, kiến trúc **Clean Architecture** và thư viện **MudBlazor** nhằm đạt điểm số tối đa trong kỳ tuyển dụng.
