# BÁO CÁO NHÓM: NGHIÊN CỨU VÀ ÁP DỤNG CÁC MÔ HÌNH KIẾN TRÚC PHẦN MỀM TRONG PHÁT TRIỂN WEBSITE THƯƠNG MẠI ĐIỆN TỬ MỸ PHẨM

## MỤC LỤC HOÀN CHỈNH

### TRANG BÌA

- Tên trường
- Tên khoa/ngành
- Tên đề tài
- Họ tên sinh viên và MSSV
- Tên giảng viên hướng dẫn
- Thời gian thực hiện

### LỜI XÁC NHẬN CỦA SINH VIÊN

### LỜI CẢM ƠN (Tùy chọn)

### MỤC LỤC

### DANH SÁCH HÌNH ẢNH VÀ BẢNG BIỂU

### DANH SÁCH CÁC TỪ VIẾT TẮT

- MVC: Model-View-Controller
- DI: Dependency Injection
- ORM: Object-Relational Mapping
- SOLID: Single Responsibility, Open/Closed, Liskov Substitution, Interface Segregation, Dependency Inversion
- CRUD: Create, Read, Update, Delete
- API: Application Programming Interface
- EF: Entity Framework
- DDD: Domain Driven Design

### TÓM TẮT (350 từ)

_Nội dung tóm tắt ngắn gọn về mục tiêu, phương pháp, kết quả chính của đề tài_

---

## NỘI DUNG CHÍNH

### CHƯƠNG 1: MỞ ĐẦU

#### 1.1. Lý do chọn đề tài

- Tầm quan trọng của kiến trúc phần mềm trong phát triển ứng dụng hiện đại
- Nhu cầu xây dựng website thương mại điện tử với chất lượng cao
- Sự cần thiết áp dụng các mô hình kiến trúc để giải quyết khủng hoảng phần mềm

#### 1.2. Mục tiêu nghiên cứu

- **Mục tiêu tổng quát**: Nghiên cứu và áp dụng các mô hình kiến trúc phần mềm
- **Mục tiêu cụ thể**:
  - Phân tích các mô hình kiến trúc phần mềm phổ biến
  - Thiết kế kiến trúc cho website thương mại điện tử mỹ phẩm
  - Xây dựng ứng dụng demo minh họa

#### 1.3. Bối cảnh nghiên cứu

- Xu hướng phát triển thương mại điện tử
- Tình hình ứng dụng kiến trúc phần mềm trong doanh nghiệp Việt Nam

#### 1.4. Phạm vi nghiên cứu

- Giới hạn về công nghệ: ASP.NET Core MVC
- Giới hạn về lĩnh vực: Website bán mỹ phẩm
- Giới hạn về chức năng: Quản lý sản phẩm, đơn hàng, tài khoản

#### 1.5. Phương pháp nghiên cứu

- Phương pháp nghiên cứu tài liệu
- Phương pháp phân tích, thiết kế
- Phương pháp thực nghiệm (xây dựng prototype)

#### 1.6. Cấu trúc báo cáo

_Mô tả ngắn gọn nội dung từng chương_

---

### CHƯƠNG 2: TỔNG QUAN LÝ THUYẾT

#### 2.1. Khái niệm Kiến trúc Phần mềm

##### 2.1.1. Định nghĩa Software Architecture

- Các quan điểm khác nhau về kiến trúc phần mềm
- Tầm quan trọng của kiến trúc trong việc giải quyết khủng hoảng phần mềm

##### 2.1.2. Các đặc trưng của Kiến trúc tốt

- Khả năng bảo trì (Maintainability)
- Khả năng mở rộng (Scalability)
- Khả năng tái sử dụng (Reusability)
- Hiệu suất (Performance)

#### 2.2. Các Kiểu Kiến trúc Phần mềm (Architectural Styles)

##### 2.2.1. Dataflow Architecture

- Pipe-and-Filter
- Batch Sequential

##### 2.2.2. Call-and-Return Architecture

- Main Program and Subroutines
- **Layered Architecture** (Tập trung vào kiểu này)
- Object-Oriented Architecture

##### 2.2.3. Independent Components

- Communicating Processes
- Event-Driven Systems

##### 2.2.4. Virtual Machines

- Interpreter
- Rule-Based Systems

##### 2.2.5. Data-Centered Architecture

- Repository
- Blackboard

#### 2.3. Design Patterns

##### 2.3.1. Khái niệm và phân loại

- Định nghĩa Design Patterns
- Lợi ích của việc sử dụng patterns

##### 2.3.2. Creational Patterns

- **Factory Pattern** ⭐ (Áp dụng trong dự án)
- Builder Pattern
- Singleton Pattern
- Prototype Pattern

##### 2.3.3. Structural Patterns

- **Adapter Pattern**
- Composite Pattern
- Decorator Pattern
- **Proxy Pattern**

##### 2.3.4. Behavioral Patterns

- **Observer Pattern**
- **Strategy Pattern**
- Command Pattern
- State Pattern

#### 2.4. Layered Architecture Pattern

##### 2.4.1. Khái niệm và đặc điểm

- Định nghĩa kiến trúc phân lớp
- Các nguyên tắc của Layered Architecture

##### 2.4.2. Các lớp trong kiến trúc MVC

- **Presentation Layer** (Controllers, Views)
- **Business Logic Layer** (Services)
- **Data Access Layer** (Repositories)
- **Data Layer** (Database, Models)

##### 2.4.3. Ưu và nhược điểm

- Ưu điểm: Tách biệt rõ ràng, dễ bảo trì, test
- Nhược điểm: Performance overhead, độ phức tạp

#### 2.5. Repository Pattern

##### 2.5.1. Khái niệm và mục đích

- Tách biệt business logic khỏi data access logic
- Tạo interface thống nhất cho truy cập dữ liệu

##### 2.5.2. Cấu trúc và Implementation

- Repository Interface
- Concrete Repository Implementation
- Unit of Work Pattern (nếu có)

#### 2.6. Service Layer Pattern

##### 2.6.1. Vai trò của Service Layer

- Đóng gói business logic
- Tạo API cho Presentation Layer

##### 2.6.2. So sánh với Repository Pattern

- Phân biệt trách nhiệm giữa Service và Repository

#### 2.7. Dependency Injection

##### 2.7.1. Khái niệm Inversion of Control

- Dependency Inversion Principle
- Các cách thức DI: Constructor, Property, Method injection

##### 2.7.2. DI Container trong ASP.NET Core

- Built-in DI Container
- Service Lifetime: Transient, Scoped, Singleton

---

### CHƯƠNG 3: PHƯƠNG PHÁP NGHIÊN CỨU

#### 3.1. Lựa chọn phương pháp nghiên cứu

##### 3.1.1. Nghiên cứu tài liệu

- Tài liệu học thuật về Software Architecture
- Best practices từ các dự án thực tế
- Documentation của ASP.NET Core

##### 3.1.2. Phương pháp phân tích và thiết kế

- Phân tích yêu cầu hệ thống
- Thiết kế kiến trúc theo từng lớp
- Áp dụng Design Patterns phù hợp

##### 3.1.3. Phương pháp thực nghiệm

- Xây dựng prototype
- Đánh giá hiệu quả của kiến trúc

#### 3.2. Công cụ và công nghệ sử dụng

##### 3.2.1. Môi trường phát triển

- Visual Studio 2022
- .NET 8.0
- SQL Server Express

##### 3.2.2. Framework và Libraries

- ASP.NET Core MVC 8.0
- Entity Framework Core
- Bootstrap 5
- jQuery

##### 3.2.3. Công cụ quản lý mã nguồn

- Git & GitHub
- Visual Studio Code (hỗ trợ)

#### 3.3. Quy trình thực hiện

##### 3.3.1. Giai đoạn 1: Nghiên cứu lý thuyết (2 tuần)

- Tìm hiểu các mô hình kiến trúc
- Phân tích ưu nhược điểm từng mô hình

##### 3.3.2. Giai đoạn 2: Phân tích và thiết kế (3 tuần)

- Phân tích yêu cầu website thương mại điện tử
- Thiết kế kiến trúc hệ thống
- Lựa chọn Design Patterns phù hợp

##### 3.3.3. Giai đoạn 3: Implement và Demo (4 tuần)

- Xây dựng ứng dụng theo thiết kế
- Testing và debugging
- Tạo tài liệu hướng dẫn

---

### CHƯƠNG 4: PHÂN TÍCH VÀ THIẾT KẾ

#### 4.1. Phân tích yêu cầu hệ thống

##### 4.1.1. Yêu cầu chức năng

- **Quản lý tài khoản**: Đăng ký, đăng nhập, quên mật khẩu
- **Quản lý sản phẩm**: CRUD sản phẩm, danh mục
- **Quản lý đơn hàng**: Tạo đơn, thanh toán, theo dõi
- **Quản lý giỏ hàng**: Thêm, sửa, xóa sản phẩm

##### 4.1.2. Yêu cầu phi chức năng

- Hiệu suất: Thời gian phản hồi < 3s
- Bảo mật: Mã hóa mật khẩu, xác thực session
- Khả năng mở rộng: Hỗ trợ thêm module mới

##### 4.1.3. Actors và Use Cases

- **Admin**: Quản lý sản phẩm, đơn hàng, danh mục
- **Khách hàng**: Xem sản phẩm, đặt hàng, quản lý tài khoản

#### 4.2. Lựa chọn Kiến trúc Phần mềm

##### 4.2.1. Tiêu chí lựa chọn

- Độ phức tạp của hệ thống
- Yêu cầu về khả năng bảo trì
- Khả năng mở rộng trong tương lai

##### 4.2.2. So sánh các lựa chọn

- **Monolithic vs Microservices**: Chọn Monolithic cho project vừa
- **Layered vs Clean Architecture**: Chọn Layered vì đơn giản hơn

##### 4.2.3. Kiến trúc được chọn: Layered Architecture với MVC Pattern

- **Lý do lựa chọn**:
  - Phù hợp với ASP.NET Core MVC
  - Tách biệt rõ ràng các layer
  - Dễ hiểu và implement

#### 4.3. Thiết kế chi tiết từng Layer

##### 4.3.1. Data Layer

```
Data Models:
- TaiKhoan (Id, Email, MatKhau, HoTen, SoDienThoai, DiaChi, NgayTao, TrangThai)
- SanPham (Id, TenSP, Gia, MoTa, Hinh, DanhMucId, NgayTao, TrangThai)
- DanhMuc (Id, TenDanhMuc, MoTa, ThuTuHienThi)
- DonHang (Id, TaiKhoanId, NgayDat, TongTien, TrangThai, DiaChi)
- ChiTietDonHang (Id, DonHangId, SanPhamId, SoLuong, Gia)
- AnhSanPham (Id, SanPhamId, DuongDan, LaChinh)
```

##### 4.3.2. Data Access Layer (Repository Pattern)

```
Repository Interfaces:
- ITaiKhoanRepository
- ISanPhamRepository
- IDanhMucRepository
- IDonHangRepository

Repository Implementations:
- TaiKhoanRepository
- SanPhamRepository
- DanhMucRepository
- DonHangRepository
```

##### 4.3.3. Business Logic Layer (Service Pattern)

```
Service Interfaces:
- ITaiKhoanService
- ISanPhamService
- IDanhMucService
- IDonHangService
- IEmailService

Service Implementations:
- TaiKhoanService
- SanPhamService
- DanhMucService
- DonHangService
- EmailService
```

##### 4.3.4. Presentation Layer

```
Areas Structure:
- Admin Area: Quản lý hệ thống
  + Controllers: DanhmucController, SanphamController, DonhangController
  + Views: CRUD interfaces for admin

- KhachHang Area: Giao diện khách hàng
  + Controllers: TrangChuController, SanPhamController, DonHangController
  + Views: Customer-facing pages

- Root Controllers: DangNhapController, DangKiController, HomeController
```

#### 4.4. Áp dụng Design Patterns

##### 4.4.1. Factory Pattern

```
Implementations:
- IServiceFactory & ServiceFactory: Tạo service instances
- IRepositoryFactory & RepositoryFactory: Tạo repository instances

Lợi ích:
- Tách biệt việc tạo object khỏi business logic
- Dễ dàng thay đổi implementation
- Hỗ trợ unit testing
```

##### 4.4.2. Repository Pattern

```
Đặc điểm:
- Interface-based design
- Async/await pattern cho performance
- Generic methods cho CRUD operations

Lợi ích:
- Tách biệt business logic khỏi data access
- Dễ dàng mock cho unit testing
- Có thể thay đổi data source mà không ảnh hưởng business logic
```

##### 4.4.3. Service Layer Pattern

```
Chức năng:
- Đóng gói business logic phức tạp
- Validation dữ liệu đầu vào
- Error handling và logging
- Transaction management

Ví dụ TaiKhoanService:
- ValidateUser(): Kiểm tra thông tin đăng nhập
- CreateUser(): Tạo tài khoản mới với mã hóa password
- SendActivationEmail(): Gửi email kích hoạt
```

#### 4.5. Dependency Injection Configuration

```csharp
// Repository registrations
builder.Services.AddScoped<ISanPhamRepository, SanPhamRepository>();
builder.Services.AddScoped<IDanhMucRepository, DanhMucRepository>();
builder.Services.AddScoped<IDonHangRepository, DonHangRepository>();
builder.Services.AddScoped<ITaiKhoanRepository, TaiKhoanRepository>();

// Service registrations
builder.Services.AddScoped<ISanPhamService, SanPhamService>();
builder.Services.AddScoped<IDanhMucService, DanhMucService>();
builder.Services.AddScoped<IDonHangService, DonHangService>();
builder.Services.AddScoped<ITaiKhoanService, TaiKhoanService>();
builder.Services.AddScoped<IEmailService, EmailService>();

// Factory registrations
builder.Services.AddScoped<IRepositoryFactory, RepositoryFactory>();
builder.Services.AddScoped<IServiceFactory, ServiceFactory>();
```

---

### CHƯƠNG 5: XÂY DỰNG ỨNG DỤNG DEMO

#### 5.1. Cài đặt môi trường phát triển

##### 5.1.1. Yêu cầu hệ thống

- Windows 10/11
- .NET 8.0 SDK
- Visual Studio 2022
- SQL Server Express

##### 5.1.2. Tạo project và cấu hình

```bash
dotnet new mvc -n Final_VS1
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package BCrypt.Net-Next
dotnet add package MailKit
```

#### 5.2. Implement Data Layer

##### 5.2.1. Entity Models

_Code examples từ các file trong Data folder_

##### 5.2.2. DbContext Configuration

```csharp
public class LittleFishBeautyContext : DbContext
{
    // DbSet properties
    // OnModelCreating configuration
    // Relationships setup
}
```

##### 5.2.3. Database Migration

```bash
dotnet ef migrations add InitialSync
dotnet ef database update
```

#### 5.3. Implement Repository Layer

##### 5.3.1. Repository Interfaces

_Detailed code examples from actual implementation_

##### 5.3.2. Repository Implementations

_Show async/await patterns, error handling_

##### 5.3.3. Repository Factory

```csharp
public interface IRepositoryFactory
{
    ITaiKhoanRepository CreateTaiKhoanRepository();
    ISanPhamRepository CreateSanPhamRepository();
    IDanhMucRepository CreateDanhMucRepository();
    IDonHangRepository CreateDonHangRepository();
}
```

#### 5.4. Implement Service Layer

##### 5.4.1. Service Interfaces

_Business contract definitions_

##### 5.4.2. Service Implementations

_Business logic with validation, error handling_

##### 5.4.3. Service Factory Implementation

_Show how Factory pattern simplifies object creation_

#### 5.5. Implement Presentation Layer

##### 5.5.1. Area-based Organization

- Admin area cho quản lý
- KhachHang area cho customer interface

##### 5.5.2. Controllers Implementation

_Examples showing how controllers use services via DI_

##### 5.5.3. Views và UI/UX Design

- Bootstrap-based responsive design
- AJAX for dynamic interactions

#### 5.6. Authentication và Authorization

##### 5.6.1. Cookie Authentication Setup

```csharp
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => {
        options.LoginPath = "/DangNhap/Index";
        options.LogoutPath = "/DangNhap/DangXuat";
        // Other configurations
    });
```

##### 5.6.2. Password Hashing với BCrypt

```csharp
public bool VerifyPassword(string password, string hash)
{
    return BCrypt.Net.BCrypt.Verify(password, hash);
}
```

#### 5.7. Email Service Implementation

##### 5.7.1. MailKit Configuration

- SMTP settings
- Email templates

##### 5.7.2. Email sending for account activation

_Show integration with registration process_

---

### CHƯƠNG 6: ĐÁNH GIÁ VÀ THẢO LUẬN

#### 6.1. Đánh giá kiến trúc đã triển khai

##### 6.1.1. Tính hiệu quả của Layered Architecture

- **Ưu điểm đạt được**:

  - Tách biệt rõ ràng giữa các layer
  - Dễ dàng bảo trì và mở rộng
  - Code có tính tái sử dụng cao
  - Dễ dàng unit testing

- **Challenges gặp phải**:
  - Performance overhead do nhiều layer
  - Complexity tăng lên so với simple architecture

##### 6.1.2. Hiệu quả của Design Patterns

- **Repository Pattern**:

  - ✅ Tách biệt thành công data access
  - ✅ Dễ dàng thay đổi ORM framework
  - ✅ Hỗ trợ unit testing tốt

- **Service Layer Pattern**:

  - ✅ Encapsulation business logic tốt
  - ✅ Reusable across different controllers
  - ✅ Clear separation of concerns

- **Factory Pattern**:
  - ✅ Simplify object creation
  - ✅ Loose coupling between components
  - ✅ Easy to extend new services

#### 6.2. So sánh với các approaches khác

##### 6.2.1. Layered vs Clean Architecture

| Tiêu chí             | Layered  | Clean Architecture |
| -------------------- | -------- | ------------------ |
| Complexity           | Thấp     | Cao                |
| Learning Curve       | Dễ       | Khó                |
| Testability          | Tốt      | Rất tốt            |
| Dependency Direction | Top-down | Inside-out         |

##### 6.2.2. Repository Pattern vs Direct DbContext

_So sánh code trước và sau khi áp dụng Repository Pattern_

#### 6.3. Performance Analysis

##### 6.3.1. Benchmark results

- Page load times
- Database query performance
- Memory usage

##### 6.3.2. Optimization opportunities

- Caching strategies
- Query optimization
- Lazy loading considerations

#### 6.4. Scalability Assessment

##### 6.4.1. Horizontal scaling possibilities

- Stateless services design
- Database scaling options

##### 6.4.2. Vertical scaling considerations

- Memory usage patterns
- CPU utilization

#### 6.5. Maintainability Evaluation

##### 6.5.1. Code metrics

- Cyclomatic complexity
- Code coverage
- Technical debt assessment

##### 6.5.2. Developer experience

- Time to understand codebase
- Ease of adding new features

---

### CHƯƠNG 7: KẾT LUẬN

#### 7.1. Tóm tắt kết quả đạt được

##### 7.1.1. Mục tiêu hoàn thành

- ✅ **Task 1**: Nghiên cứu thành công các mô hình kiến trúc phần mềm
- ✅ **Task 2**: Thiết kế và triển khai kiến trúc Layered Architecture
- ✅ **Task 3**: Xây dựng demo website thương mại điện tử hoàn chỉnh

##### 7.1.2. Patterns và Principles được áp dụng

- Repository Pattern với 100% coverage
- Service Layer Pattern hoàn chỉnh
- Factory Pattern cho object creation
- Dependency Injection throughout the application
- SOLID principles compliance

##### 7.1.3. Chức năng demo đã implement

- Hệ thống quản lý tài khoản với authentication
- CRUD đầy đủ cho sản phẩm và danh mục
- Giỏ hàng và đặt hàng
- Email service cho account activation
- Admin panel và customer interface

#### 7.2. Đóng góp của nghiên cứu

##### 7.2.1. Về mặt học thuật

- Minh họa cách áp dụng multiple design patterns trong một project
- So sánh hiệu quả các approaches khác nhau
- Cung cấp template cho các dự án tương tự

##### 7.2.2. Về mặt thực tiễn

- Demo application có thể làm cơ sở cho dự án thực tế
- Best practices có thể áp dụng trong development team
- Architecture foundation cho scaling up

#### 7.3. Hạn chế của nghiên cứu

##### 7.3.1. Về phạm vi

- Chỉ tập trung vào monolithic architecture
- Chưa implement microservices patterns
- Demo scope còn hạn chế

##### 7.3.2. Về technical aspects

- Chưa có comprehensive performance testing
- Security measures cơ bản
- Chưa implement advanced caching

#### 7.4. Hướng phát triển tương lai

##### 7.4.1. Technical improvements

- Implement CQRS pattern
- Add comprehensive logging
- Advanced caching strategies
- API development cho mobile apps

##### 7.4.2. Architectural evolution

- Migration plan to microservices
- Event-driven architecture integration
- Cloud-native considerations

##### 7.4.3. Business features expansion

- Payment integration
- Inventory management
- Analytics và reporting
- Multi-tenant support

#### 7.5. Bài học kinh nghiệm

##### 7.5.1. Về Software Architecture

- Importance of planning architecture upfront
- Balance between simplicity and flexibility
- Value of consistent patterns throughout codebase

##### 7.5.2. Về Development Process

- Value of incremental development
- Importance of documentation
- Benefits of code review process

---

## TÀI LIỆU THAM KHẢO

### Sách và tài liệu học thuật

1. Fowler, M. (2002). _Patterns of Enterprise Application Architecture_. Addison-Wesley.
2. Evans, E. (2003). _Domain-Driven Design: Tackling Complexity in the Heart of Software_. Addison-Wesley.
3. Martin, R. C. (2017). _Clean Architecture: A Craftsman's Guide to Software Structure and Design_. Prentice Hall.
4. Freeman, E., Robson, E., Bates, B., & Sierra, K. (2020). _Head First Design Patterns_. O'Reilly Media.

### Tài liệu trực tuyến

1. Microsoft Docs. (2024). _ASP.NET Core MVC Overview_. https://docs.microsoft.com/en-us/aspnet/core/mvc/overview
2. Microsoft Docs. (2024). _Entity Framework Core Documentation_. https://docs.microsoft.com/en-us/ef/core/
3. Khorikov, V. (2019). _Repository and Unit of Work Pattern_. Enterprise Craftsmanship Blog.

### Design Patterns Resources

1. Gang of Four. (1994). _Design Patterns: Elements of Reusable Object-Oriented Software_. Addison-Wesley.
2. Refactoring.Guru. (2024). _Design Patterns Catalog_. https://refactoring.guru/design-patterns

---

## PHỤ LỤC

### PHỤ LỤC A: Cấu trúc thư mục dự án

```
Software_Architecture_little_1111/
├── Areas/
│   ├── Admin/
│   │   ├── Controllers/
│   │   ├── Models/
│   │   └── Views/
│   └── KhachHang/
│       ├── Controllers/
│       ├── Models/
│       ├── ViewModels/
│       └── Views/
├── Controllers/
├── Data/
├── Helper/
├── Migrations/
├── Models/
├── Repositories/
├── Services/
├── Views/
└── wwwroot/
```

### PHỤ LỤC B: Database Schema

_ERD diagram và table structures_

### PHỤ LỤC C: Code Examples

_Key code snippets demonstrating patterns implementation_

### PHỤ LỤC D: Screenshots của ứng dụng Demo

_UI screenshots showing các chức năng chính_

### PHỤ LỤC E: Performance Test Results

_Benchmark data và analysis_

### PHỤ LỤC F: Git Commit History

_Development timeline và major milestones_

---

## HƯỚNG DẪN SỬ DỤNG BÁO CÁO NÀY

### Cách đọc báo cáo

1. **Đọc tổng quan**: Bắt đầu từ Tóm tắt và Chương 1
2. **Hiểu lý thuyết**: Chương 2 cung cấp foundation knowledge
3. **Theo dõi implementation**: Chương 4 và 5 show cách áp dụng
4. **Đánh giá kết quả**: Chương 6 và 7 cho insights và lessons learned

### Cách sử dụng code examples

1. Clone repository từ GitHub
2. Follow setup instructions trong Chương 5.1
3. Run migrations để setup database
4. Study code structure theo architectural layers

### Cách extend project

1. Follow patterns đã established
2. Use Factory patterns cho new services
3. Maintain separation of concerns
4. Add comprehensive tests

---

_Báo cáo này là kết quả nghiên cứu và thực hành về Software Architecture trong phát triển ứng dụng web thương mại điện tử, với focus vào việc áp dụng các Design Patterns và Best Practices._
