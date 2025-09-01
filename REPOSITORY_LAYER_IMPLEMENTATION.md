# Repository Layer Pattern Implementation

## Tổng quan

Repository Layer Pattern đã được triển khai hoàn chỉnh cho dự án Little Fish Beauty. Pattern này cung cấp một lớp trừu tượng giữa Business Logic (Service Layer) và Data Storage, giúp tách biệt data access logic khỏi business logic và cung cấp một interface nhất quán để truy cập dữ liệu.

## Cấu trúc Repository Layer

### 1. Repository Interfaces (Contracts)

```
Repositories/
├── ITaiKhoanRepository.cs    - Interface cho tài khoản
├── IDanhMucRepository.cs     - Interface cho danh mục
├── ISanPhamRepository.cs     - Interface cho sản phẩm
└── IDonHangRepository.cs     - Interface cho đơn hàng
```

### 2. Repository Implementations (Concrete Classes)

```
Repositories/
├── TaiKhoanRepository.cs     - Implementation cho tài khoản
├── DanhMucRepository.cs      - Implementation cho danh mục
├── SanPhamRepository.cs      - Implementation cho sản phẩm
└── DonHangRepository.cs      - Implementation cho đơn hàng
```

## Các tính năng chính của Repository Layer

### ITaiKhoanRepository & TaiKhoanRepository

#### Core Operations

```csharp
Task<List<TaiKhoan>> GetAllAsync();
Task<TaiKhoan?> GetByIdAsync(int id);
Task<TaiKhoan?> GetByEmailAsync(string email);
Task<TaiKhoan?> GetByUsernameOrEmailAsync(string usernameOrEmail);
Task AddAsync(TaiKhoan taiKhoan);
Task UpdateAsync(TaiKhoan taiKhoan);
Task DeleteAsync(int id);
```

#### Advanced Features

- ✅ **Email Validation**: Kiểm tra email tồn tại với `IsEmailExistsAsync()`
- ✅ **Role-based Filtering**: Lọc tài khoản theo role với `GetByRoleAsync()`
- ✅ **Statistics**: Đếm tổng số và số active với `GetTotalCountAsync()`, `GetActiveCountAsync()`
- ✅ **Related Data**: Include đơn hàng khi lấy thông tin tài khoản
- ✅ **Flexible Search**: Tìm kiếm theo email hoặc username

### IDanhMucRepository & DanhMucRepository

#### Core Operations

```csharp
Task<List<DanhMuc>> GetAllAsync();
Task<DanhMuc?> GetByIdAsync(int id);
Task AddAsync(DanhMuc danhMuc);
Task UpdateAsync(DanhMuc danhMuc);
Task DeleteAsync(int id);
```

#### Advanced Features

- ✅ **Name Uniqueness**: Kiểm tra tên danh mục trùng lặp
- ✅ **Display Order**: Quản lý thứ tự hiển thị danh mục
- ✅ **Product Relations**: Include sản phẩm liên quan
- ✅ **Active Status**: Lọc theo trạng thái hoạt động

### ISanPhamRepository & SanPhamRepository

#### Core Operations

```csharp
Task<List<SanPham>> GetAllAsync();
Task<SanPham?> GetByIdAsync(int id);
Task AddAsync(SanPham sanPham);
Task UpdateAsync(SanPham sanPham);
Task DeleteAsync(int id);
```

#### Business-Specific Features

- ✅ **SKU Validation**: Kiểm tra SKU trùng lặp với `IsSkuExistsAsync()`
- ✅ **Stock Management**: Cập nhật tồn kho với `UpdateStockAsync()`
- ✅ **Product Discovery**:
  - Sản phẩm mới nhất: `GetNewestProductsAsync()`
  - Sản phẩm bán chạy: `GetBestSellingProductsAsync()`
  - Sản phẩm gợi ý: `GetSuggestedProductsAsync()`
- ✅ **Rich Data**: Include danh mục và hình ảnh sản phẩm
- ✅ **Ordering**: Sắp xếp theo ngày tạo mặc định

### IDonHangRepository & DonHangRepository

#### Core Operations

```csharp
Task<List<DonHang>> GetAllAsync();
Task<DonHang?> GetByIdAsync(int id);
Task AddAsync(DonHang donHang);
Task UpdateAsync(DonHang donHang);
Task DeleteAsync(int id);
```

#### Advanced Features

- ✅ **Order Details**: Include chi tiết đơn hàng và thông tin sản phẩm
- ✅ **Customer Orders**: Lấy đơn hàng theo khách hàng
- ✅ **Status Filtering**: Lọc theo trạng thái đơn hàng
- ✅ **Date Range Queries**: Truy vấn theo khoảng thời gian
- ✅ **Order Analytics**: Thống kê và báo cáo đơn hàng

## Thiết kế Architecture

### 1. Separation of Concerns

```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   Controllers   │ -> │    Services     │ -> │  Repositories   │
│  (Presentation) │    │ (Business Logic)│    │ (Data Access)   │
└─────────────────┘    └─────────────────┘    └─────────────────┘
                                                        |
                                               ┌─────────────────┐
                                               │   Database      │
                                               │ (Entity Framework)│
                                               └─────────────────┘
```

### 2. Dependency Injection Pattern

```csharp
// Program.cs
services.AddScoped<ITaiKhoanRepository, TaiKhoanRepository>();
services.AddScoped<IDanhMucRepository, DanhMucRepository>();
services.AddScoped<ISanPhamRepository, SanPhamRepository>();
services.AddScoped<IDonHangRepository, DonHangRepository>();
```

### 3. Interface Segregation

Mỗi repository interface chỉ định nghĩa các methods liên quan đến entity cụ thể, tuân theo Interface Segregation Principle.

## Database Integration

### Entity Framework Core Features

#### 1. DbContext Integration

```csharp
public class TaiKhoanRepository : ITaiKhoanRepository
{
    private readonly LittleFishBeautyContext _context;

    public TaiKhoanRepository(LittleFishBeautyContext context)
    {
        _context = context;
    }
}
```

#### 2. Async/Await Pattern

Tất cả database operations sử dụng async pattern để tránh blocking:

```csharp
public async Task<TaiKhoan?> GetByIdAsync(int id)
{
    return await _context.TaiKhoans
        .Include(t => t.DonHangs)
        .FirstOrDefaultAsync(t => t.IdTaiKhoan == id);
}
```

#### 3. Include Strategy

Sử dụng Include để load related data khi cần thiết:

```csharp
// Sản phẩm với danh mục và hình ảnh
return await _context.SanPhams
    .Include(s => s.IdDanhMucNavigation)
    .Include(s => s.AnhSanPhams)
    .ToListAsync();
```

## Query Optimization Techniques

### 1. Selective Loading

```csharp
// Chỉ load dữ liệu cần thiết
public async Task<List<TaiKhoan>> GetByRoleAsync(string role)
{
    return await _context.TaiKhoans
        .Where(t => t.Role == role)
        .OrderByDescending(t => t.NgayTao)
        .ToListAsync();
}
```

### 2. Efficient Filtering

```csharp
// Sử dụng Where clause hiệu quả
public async Task<bool> IsEmailExistsAsync(string email, int? excludeId = null)
{
    var query = _context.TaiKhoans.Where(t => t.Email == email);
    if (excludeId.HasValue)
        query = query.Where(t => t.IdTaiKhoan != excludeId.Value);

    return await query.AnyAsync();
}
```

### 3. Pagination Ready

Structure sẵn sàng cho pagination:

```csharp
public async Task<List<SanPham>> GetNewestProductsAsync(int count)
{
    return await _context.SanPhams
        .OrderByDescending(s => s.NgayTao)
        .Take(count)
        .ToListAsync();
}
```

## Error Handling Strategy

### 1. Repository Level Error Handling

```csharp
public async Task<bool> UpdateStockAsync(int id, int newStock)
{
    try
    {
        var product = await _context.SanPhams.FindAsync(id);
        if (product == null) return false;

        product.SoLuongTonKho = newStock;
        await _context.SaveChangesAsync();
        return true;
    }
    catch (Exception)
    {
        return false;
    }
}
```

### 2. Null Safety

```csharp
public async Task<TaiKhoan?> GetByIdAsync(int id)
{
    return await _context.TaiKhoans
        .FirstOrDefaultAsync(t => t.IdTaiKhoan == id);
}
```

## Testing Advantages

### 1. Interface-Based Design

```csharp
// Easy mocking trong unit tests
public class TaiKhoanServiceTests
{
    private readonly Mock<ITaiKhoanRepository> _mockRepo;
    private readonly TaiKhoanService _service;

    public TaiKhoanServiceTests()
    {
        _mockRepo = new Mock<ITaiKhoanRepository>();
        _service = new TaiKhoanService(_mockRepo.Object);
    }
}
```

### 2. Isolated Testing

Mỗi repository có thể được test độc lập mà không cần database thật.

## Performance Benefits

### 1. Connection Pooling

Entity Framework tự động quản lý connection pooling.

### 2. Async Operations

Non-blocking database calls cải thiện throughput.

### 3. Query Optimization

Selective loading và efficient filtering giảm database load.

### 4. Caching Ready

Structure sẵn sàng để implement caching layer ở repository level.

## Security Considerations

### 1. SQL Injection Prevention

Entity Framework sử dụng parameterized queries tự động.

### 2. Input Validation

```csharp
public async Task<bool> IsEmailExistsAsync(string email, int? excludeId = null)
{
    if (string.IsNullOrEmpty(email))
        return false;

    // Safe query execution
    return await _context.TaiKhoans
        .AnyAsync(t => t.Email == email && (excludeId == null || t.IdTaiKhoan != excludeId));
}
```

## Maintainability Features

### 1. Single Responsibility

Mỗi repository chịu trách nhiệm cho một entity.

### 2. Open/Closed Principle

Dễ dàng extend functionality mà không modify existing code.

### 3. Dependency Inversion

High-level modules (Services) không phụ thuộc vào low-level modules (Repositories).

### 4. Consistent Naming

Tất cả repositories follow consistent naming convention.

## Future Extensions

Repository Layer đã sẵn sàng cho:

- **Caching Layer**: Redis hoặc In-Memory caching
- **Audit Trail**: Tracking changes và user activities
- **Multi-Database Support**: Dễ dàng switch database providers
- **Query Optimization**: Advanced query techniques
- **Bulk Operations**: Batch insert/update operations
