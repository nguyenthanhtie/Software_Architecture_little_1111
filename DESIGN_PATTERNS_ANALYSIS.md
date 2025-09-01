# Phân Tích Design Patterns - Little Fish Beauty Project

## Tổng Quan

Dự án Little Fish Beauty được xây dựng theo kiến trúc ASP.NET Core MVC với nhiều design patterns được áp dụng để tạo ra một hệ thống có tính mở rộng cao, dễ bảo trì và tuân thủ các nguyên tắc SOLID.

---

## 1. Repository Pattern ⭐⭐⭐⭐⭐

**Mức độ thể hiện**: Rất rõ ràng và hoàn chỉnh

### Mô tả

Repository Pattern được triển khai hoàn chỉnh với interface và implementation riêng biệt cho từng entity.

### Cấu trúc Implementation

```
Repositories/
├── Interfaces/
│   ├── ITaiKhoanRepository.cs
│   ├── IDanhMucRepository.cs
│   ├── ISanPhamRepository.cs
│   └── IDonHangRepository.cs
└── Implementations/
    ├── TaiKhoanRepository.cs
    ├── DanhMucRepository.cs
    ├── SanPhamRepository.cs
    └── DonHangRepository.cs
```

### Code Example

```csharp
// Interface
public interface ITaiKhoanRepository
{
    Task<List<TaiKhoan>> GetAllAsync();
    Task<TaiKhoan?> GetByIdAsync(int id);
    Task<TaiKhoan?> GetByEmailAsync(string email);
    Task AddAsync(TaiKhoan taiKhoan);
    Task UpdateAsync(TaiKhoan taiKhoan);
    Task DeleteAsync(int id);
}

// Implementation
public class TaiKhoanRepository : ITaiKhoanRepository
{
    private readonly LittleFishBeautyContext _context;

    public TaiKhoanRepository(LittleFishBeautyContext context)
    {
        _context = context;
    }

    public async Task<TaiKhoan?> GetByIdAsync(int id)
    {
        return await _context.TaiKhoans
            .Include(t => t.DonHangs)
            .FirstOrDefaultAsync(t => t.IdTaiKhoan == id);
    }
}
```

### Lợi ích

- Tách biệt data access logic khỏi business logic
- Dễ dàng unit testing với mock objects
- Tuân thủ Dependency Inversion Principle
- Nhất quán trong cách truy cập dữ liệu

---

## 2. Service Layer Pattern ⭐⭐⭐⭐⭐

**Mức độ thể hiện**: Rất rõ ràng và hoàn chỉnh

### Mô tả

Service Layer Pattern được sử dụng để chứa business logic và tách biệt với presentation layer.

### Cấu trúc Implementation

```
Services/
├── Interfaces/
│   ├── ITaiKhoanService.cs
│   ├── IDanhMucService.cs
│   ├── ISanPhamService.cs
│   ├── IDonHangService.cs
│   └── IEmailService.cs
└── Implementations/
    ├── TaiKhoanService.cs
    ├── DanhMucService.cs
    ├── SanPhamService.cs
    ├── DonHangService.cs
    └── EmailService.cs
```

### Code Example

```csharp
public class TaiKhoanService : ITaiKhoanService
{
    private readonly ITaiKhoanRepository _taiKhoanRepository;

    public TaiKhoanService(ITaiKhoanRepository taiKhoanRepository)
    {
        _taiKhoanRepository = taiKhoanRepository;
    }

    public async Task<bool> ValidateAndCreateAccountAsync(TaiKhoan taiKhoan)
    {
        // Business logic validation
        if (await _taiKhoanRepository.IsEmailExistsAsync(taiKhoan.Email))
            return false;

        // Hash password
        taiKhoan.MatKhau = HashPassword(taiKhoan.MatKhau);

        await _taiKhoanRepository.AddAsync(taiKhoan);
        return true;
    }
}
```

### Lợi ích

- Tập trung business logic ở một nơi
- Controller chỉ tập trung vào điều hướng
- Dễ dàng tái sử dụng logic giữa các controllers
- Hỗ trợ transaction management

---

## 3. Model-View-Controller (MVC) Pattern ⭐⭐⭐⭐⭐

**Mức độ thể hiện**: Framework core pattern

### Mô tả

ASP.NET Core MVC framework được sử dụng làm backbone của ứng dụng với Areas architecture.

### Cấu trúc Implementation

```
Areas/
├── Admin/
│   ├── Controllers/
│   ├── Models/
│   └── Views/
├── KhachHang/
│   ├── Controllers/
│   ├── Models/
│   ├── ViewModels/
│   └── Views/
Controllers/          // Root controllers
Models/              // Shared models
Views/              // Shared views
```

### Code Example

```csharp
[Area("KhachHang")]
public class SanPhamController : Controller
{
    private readonly ISanPhamService _sanPhamService;

    public SanPhamController(ISanPhamService sanPhamService)
    {
        _sanPhamService = sanPhamService;
    }

    public async Task<IActionResult> Index(string search, string category)
    {
        var products = await _sanPhamService.SearchProductsAsync(search, category);
        return View(products);
    }
}
```

### Lợi ích

- Tách biệt rõ ràng giữa presentation, logic và data
- Areas giúp phân chia chức năng theo vai trò
- Dễ dàng bảo trì và mở rộng
- SEO friendly với routing system

---

## 4. Dependency Injection Pattern ⭐⭐⭐⭐⭐

**Mức độ thể hiện**: Được sử dụng toàn bộ hệ thống

### Mô tả

Dependency Injection được sử dụng để quản lý dependencies và tuân thủ Inversion of Control.

### Code Example

```csharp
// Program.cs - DI Container Configuration
builder.Services.AddDbContext<LittleFishBeautyContext>(options =>
    options.UseSqlServer(connectionString));

// Repository registration
builder.Services.AddScoped<ITaiKhoanRepository, TaiKhoanRepository>();
builder.Services.AddScoped<ISanPhamRepository, SanPhamRepository>();
builder.Services.AddScoped<IDanhMucRepository, DanhMucRepository>();
builder.Services.AddScoped<IDonHangRepository, DonHangRepository>();

// Service registration
builder.Services.AddScoped<ITaiKhoanService, TaiKhoanService>();
builder.Services.AddScoped<ISanPhamService, SanPhamService>();
builder.Services.AddScoped<IEmailService, EmailService>();

// Email service
builder.Services.AddTransient<IEmailSender, MailKitEmailSender>();
```

### Constructor Injection Example

```csharp
public class SanPhamController : Controller
{
    private readonly ISanPhamService _sanPhamService;
    private readonly IDanhMucService _danhMucService;

    public SanPhamController(ISanPhamService sanPhamService, IDanhMucService danhMucService)
    {
        _sanPhamService = sanPhamService;
        _danhMucService = danhMucService;
    }
}
```

### Lợi ích

- Loose coupling between components
- Easy unit testing with mock objects
- Centralized dependency management
- Automatic lifetime management

---

## 5. Data Transfer Object (DTO) / ViewModel Pattern ⭐⭐⭐⭐

**Mức độ thể hiện**: Rõ ràng trong ViewModels và Request/Response models

### Mô tả

ViewModel pattern được sử dụng để truyền dữ liệu giữa Controller và View, tách biệt với Domain models.

### Code Example

```csharp
// ViewModels
public class DangKiViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [MinLength(6)]
    public string MatKhau { get; set; }

    [Required]
    public string HoTen { get; set; }
}

public class DonHangViewModel
{
    public List<DonHang> DonHangs { get; set; } = new List<DonHang>();
    public string? CurrentFilter { get; set; }
    public int TotalOrders { get; set; }
    public int PendingOrders { get; set; }
    public int ProcessingOrders { get; set; }
}

// Request DTOs
public class ProcessOrderRequest
{
    public string? PaymentMethod { get; set; }
    public decimal TotalAmount { get; set; }
    public string? HoTen { get; set; }
    public List<OrderItemRequest> OrderItems { get; set; }
}
```

### Lợi ích

- Tách biệt presentation model với domain model
- Validation attributes tập trung
- Tối ưu data transfer
- Bảo mật thông tin nhạy cảm

---

## 6. Active Record Pattern (Entity Framework) ⭐⭐⭐⭐

**Mức độ thể hiện**: Thông qua Entity Framework Core

### Mô tả

Entity Framework Core sử dụng Active Record pattern với DbContext làm Unit of Work.

### Code Example

```csharp
public partial class LittleFishBeautyContext : DbContext
{
    public virtual DbSet<TaiKhoan> TaiKhoans { get; set; }
    public virtual DbSet<SanPham> SanPhams { get; set; }
    public virtual DbSet<DonHang> DonHangs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SanPham>(entity =>
        {
            entity.HasKey(e => e.IdSanPham);
            entity.Property(e => e.TenSanPham).HasMaxLength(100);
            entity.HasOne(d => d.IdDanhMucNavigation)
                  .WithMany(p => p.SanPhams)
                  .HasForeignKey(d => d.IdDanhMuc);
        });
    }
}
```

### Domain Models with Navigation Properties

```csharp
public partial class SanPham
{
    public int IdSanPham { get; set; }
    public string? TenSanPham { get; set; }
    public decimal? GiaBan { get; set; }

    // Navigation properties
    public virtual DanhMuc? IdDanhMucNavigation { get; set; }
    public virtual ICollection<AnhSanPham> AnhSanPhams { get; set; } = new List<AnhSanPham>();
    public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();
}
```

### Lợi ích

- ORM tự động mapping
- Lazy loading relationships
- Change tracking
- LINQ query support

---

## 7. Factory Pattern (Email Service) ⭐⭐⭐

**Mức độ thể hiện**: Trong Email service implementation

### Mô tả

Factory pattern được sử dụng để tạo email messages với các template khác nhau.

### Code Example

```csharp
public interface IEmailSender
{
    Task SenderEmailAsync(string toEmail, string subject, string body);
}

public class MailKitEmailSender : IEmailSender
{
    private readonly IConfiguration _configuration;

    public MailKitEmailSender(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SenderEmailAsync(string toEmail, string subject, string body)
    {
        var email = new MimeMessage();
        var emailSettings = _configuration.GetSection("EmailSettings");

        email.From.Add(new MailboxAddress(
            emailSettings["SenderName"],
            emailSettings["SenderEmail"]));
        email.To.Add(MailboxAddress.Parse(toEmail));
        email.Subject = subject;

        var builder = new BodyBuilder { HtmlBody = body };
        email.Body = builder.ToMessageBody();

        // Send email logic
    }
}
```

### Lợi ích

- Tách biệt email creation logic
- Dễ dàng thay đổi email provider
- Consistent email formatting

---

## 8. Observer Pattern (Sessions & Authentication) ⭐⭐⭐

**Mức độ thể hiện**: Trong authentication và session management

### Mô tả

Observer pattern được sử dụng thông qua ASP.NET Core middleware pipeline và events.

### Code Example

```csharp
// Program.cs - Middleware configuration
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/DangNhap/Index";
        options.LogoutPath = "/DangNhap/DangXuat";
        options.AccessDeniedPath = "/DangNhap/Index";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;
    });

// Session configuration
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
```

### Lợi ích

- Automatic state management
- Event-driven authentication
- Centralized security concerns

---

## 9. Strategy Pattern (Business Rules) ⭐⭐⭐

**Mức độ thể hiện**: Trong business logic services

### Mô tả

Strategy pattern được áp dụng trong các business rules khác nhau cho từng entity.

### Code Example

```csharp
public class DonHangService : IDonHangService
{
    public async Task<bool> CanCancelOrderAsync(int orderId, int userId)
    {
        var order = await _donHangRepository.GetByIdAndUserAsync(orderId, userId);

        if (order == null) return false;

        // Different cancellation strategies based on status
        return order.TrangThai switch
        {
            "Chờ xác nhận" => true,
            "Đang xử lý" => (DateTime.Now - order.NgayDat)?.TotalHours < 2,
            "Đã xác nhận" => false,
            "Đang giao" => false,
            "Hoàn thành" => false,
            "Đã hủy" => false,
            _ => false
        };
    }
}
```

### Lợi ích

- Flexible business rules
- Easy to extend and modify
- Clear separation of concerns

---

## 10. Adapter Pattern (ViewModels) ⭐⭐⭐

**Mức độ thể hiện**: Conversion giữa Domain models và ViewModels

### Mô tả

Adapter pattern được sử dụng để chuyển đổi giữa domain models và presentation models.

### Code Example

```csharp
// Adapter methods in controllers
public async Task<IActionResult> Index(string search, string category)
{
    var sanPhams = await _sanPhamRepository.GetFilteredProductsAsync(search, category);

    // Adapt domain models to view models
    var sanPhamViewModels = sanPhams.Select(sp => new SanPhamViewModel
    {
        IdSanPham = sp.IdSanPham,
        TenSanPham = sp.TenSanPham,
        GiaBan = sp.GiaBan,
        TenDanhMuc = sp.IdDanhMucNavigation?.TenDanhMuc,
        AnhChinhs = sp.AnhSanPhams
            .Where(a => a.LoaiAnh == "Chinh")
            .Select(a => a.DuongDan ?? "/Images/noimage.jpg")
            .ToList()
    }).ToList();

    return View(sanPhamViewModels);
}
```

### Lợi ích

- Clean separation between layers
- Flexible data presentation
- Protection of domain model structure

---

## Tổng Kết

### Design Patterns Chính (5⭐)

1. **Repository Pattern** - Hoàn chỉnh và professional
2. **Service Layer Pattern** - Business logic tập trung
3. **MVC Pattern** - Framework architecture
4. **Dependency Injection** - Toàn bộ hệ thống

### Design Patterns Phụ (3-4⭐)

5. **DTO/ViewModel Pattern** - Data transfer
6. **Active Record Pattern** - Entity Framework
7. **Factory Pattern** - Email services
8. **Strategy Pattern** - Business rules
9. **Observer Pattern** - Authentication & Events
10. **Adapter Pattern** - Model conversion

### Đánh Giá Tổng Thể

- **Architecture Quality**: Excellent ⭐⭐⭐⭐⭐
- **SOLID Principles**: Được tuân thủ tốt
- **Maintainability**: Cao
- **Testability**: Tốt với DI và Repository pattern
- **Scalability**: Có khả năng mở rộng tốt

Dự án thể hiện sự hiểu biết sâu sắc về design patterns và áp dụng chúng một cách phù hợp trong context của web application. Các patterns được sử dụng hỗ trợ lẫn nhau tạo ra một kiến trúc vững chắc và dễ bảo trì.
