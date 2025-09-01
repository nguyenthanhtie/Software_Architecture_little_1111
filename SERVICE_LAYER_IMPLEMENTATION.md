# Service Layer Pattern Implementation

## Tổng quan

Service Layer Pattern đã được triển khai hoàn chỉnh cho dự án Little Fish Beauty. Pattern này cung cấp một lớp trung gian giữa Controllers và Data Access Layer (Repositories), giúp tách biệt business logic khỏi presentation layer.

## Cấu trúc

### 1. Repositories (Data Access Layer)

- `ITaiKhoanRepository` & `TaiKhoanRepository`
- `IDanhMucRepository` & `DanhMucRepository`
- `ISanPhamRepository` & `SanPhamRepository`
- `IDonHangRepository` & `DonHangRepository`

### 2. Services (Business Logic Layer)

- `ITaiKhoanService` & `TaiKhoanService`
- `IDanhMucService` & `DanhMucService`
- `ISanPhamService` & `SanPhamService`
- `IDonHangService` & `DonHangService`
- `IEmailService` & `EmailService`

## Các tính năng chính của Service Layer

### TaiKhoanService

- ✅ Quản lý tài khoản người dùng
- ✅ Xác thực mật khẩu với BCrypt
- ✅ Kích hoạt tài khoản
- ✅ Đổi mật khẩu và reset mật khẩu
- ✅ Validation business rules
- ✅ Xử lý lỗi và exception handling

### DanhMucService

- ✅ Quản lý danh mục sản phẩm
- ✅ Validation tên danh mục và độ dài
- ✅ Kiểm tra trùng lặp
- ✅ Quản lý thứ tự hiển thị
- ✅ Kiểm tra ràng buộc trước khi xóa

### SanPhamService

- ✅ Quản lý sản phẩm
- ✅ Validation SKU và business rules
- ✅ Quản lý tồn kho
- ✅ Tìm kiếm và lọc sản phẩm
- ✅ Sản phẩm gợi ý và bán chạy
- ✅ Kiểm tra tính khả dụng

### DonHangService

- ✅ Quản lý đơn hàng
- ✅ Tạo đơn hàng với validation
- ✅ Cập nhật trạng thái đơn hàng
- ✅ Hủy đơn hàng với điều kiện
- ✅ Thống kê doanh thu và báo cáo
- ✅ Lọc đơn hàng theo nhiều tiêu chí

### EmailService

- ✅ Gửi email xác nhận đăng ký
- ✅ Gửi email reset mật khẩu
- ✅ Gửi email xác nhận đơn hàng
- ✅ Gửi email chào mừng
- ✅ Gửi email cập nhật trạng thái đơn hàng
- ✅ Template HTML đẹp và responsive

## Lợi ích của Service Layer Pattern

1. **Tách biệt Business Logic**: Logic nghiệp vụ được tập trung trong Services
2. **Tái sử dụng**: Services có thể được sử dụng bởi nhiều Controllers
3. **Dễ kiểm thử**: Services có thể được unit test độc lập
4. **Bảo trì dễ dàng**: Thay đổi logic không ảnh hưởng đến Controllers
5. **Consistency**: Đảm bảo business rules được áp dụng thống nhất

## Cách sử dụng

### Dependency Injection

Services đã được đăng ký trong `Program.cs`:

```csharp
builder.Services.AddScoped<ITaiKhoanRepository, TaiKhoanRepository>();
builder.Services.AddScoped<ITaiKhoanService, TaiKhoanService>();
// ... các services khác
```

### Sử dụng trong Controllers

Ví dụ trong `DangKiServiceExampleController`:

```csharp
public class DangKiServiceExampleController : Controller
{
    private readonly ITaiKhoanService _taiKhoanService;
    private readonly IEmailService _emailService;

    public DangKiServiceExampleController(ITaiKhoanService taiKhoanService, IEmailService emailService)
    {
        _taiKhoanService = taiKhoanService;
        _emailService = emailService;
    }

    // Sử dụng services thay vì trực tiếp truy cập database
}
```

## Migration từ Repository Pattern sang Service Pattern

### Trước (Direct Repository Usage):

```csharp
public class HomeController : Controller
{
    private readonly ISanPhamRepository _sanPhamRepo;

    public async Task<IActionResult> Index()
    {
        var products = await _sanPhamRepo.GetAllAsync();
        // Business logic trực tiếp trong controller
        var availableProducts = products.Where(p => p.TrangThai == true && p.SoLuongTonKho > 0).ToList();
        return View(availableProducts);
    }
}
```

### Sau (Service Layer Usage):

```csharp
public class HomeController : Controller
{
    private readonly ISanPhamService _sanPhamService;

    public async Task<IActionResult> Index()
    {
        // Business logic được xử lý trong service
        var availableProducts = await _sanPhamService.GetAvailableProductsAsync();
        return View(availableProducts);
    }
}
```

## Exception Handling

Services implement comprehensive exception handling:

- `ArgumentNullException`: Cho các tham số null
- `ArgumentException`: Cho các giá trị không hợp lệ
- `InvalidOperationException`: Cho các operation không được phép
- Generic `Exception`: Để catch-all cho các lỗi không mong đợi

## Validation Rules

### TaiKhoanService

- Email phải unique
- Mật khẩu được hash với BCrypt
- Tài khoản mặc định inactive cho đến khi verify email

### DanhMucService

- Tên danh mục không được vượt quá 100 ký tự
- Tên danh mục phải unique
- Không thể xóa danh mục có sản phẩm

### SanPhamService

- SKU phải unique nếu được cung cấp
- Giá và số lượng không được âm
- Sản phẩm phải có tên

### DonHangService

- Đơn hàng phải có ít nhất 1 sản phẩm
- Thông tin người nhận là bắt buộc
- Chỉ cho phép hủy đơn hàng ở trạng thái "Chờ xác nhận" hoặc "Đã xác nhận"

## Testing

Services được thiết kế để dễ dàng unit testing:

```csharp
[Test]
public async Task CreateAsync_WithValidAccount_ShouldReturnCreatedAccount()
{
    // Arrange
    var mockRepo = new Mock<ITaiKhoanRepository>();
    var service = new TaiKhoanService(mockRepo.Object);
    var taiKhoan = new TaiKhoan { Email = "test@test.com", MatKhau = "password" };

    // Act
    var result = await service.CreateAsync(taiKhoan);

    // Assert
    Assert.NotNull(result);
    mockRepo.Verify(r => r.AddAsync(It.IsAny<TaiKhoan>()), Times.Once);
}
```

## Tương lai

Service Layer có thể được mở rộng để hỗ trợ:

- Caching
- Logging
- Transaction management
- Event-driven architecture
- Microservices
- API versioning

## Kết luận

Service Layer Pattern đã được triển khai hoàn chỉnh, cung cấp một kiến trúc vững chắc, dễ bảo trì và mở rộng cho ứng dụng Little Fish Beauty.
