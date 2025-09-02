# Hướng dẫn hiển thị mô tả sản phẩm từ Database

## Tổng quan
Trang chi tiết sản phẩm đã được cập nhật để hiển thị đầy đủ thông tin mô tả từ database, bao gồm mô tả sản phẩm, hướng dẫn sử dụng và bảng thông tin chi tiết.

## Cách sử dụng Database

### 1. Cấu trúc Database

#### Bảng SanPham:
```sql
-- Các trường quan trọng cho mô tả
- TenSanPham: Tên sản phẩm
- MoTa: Mô tả chi tiết sản phẩm (TEXT/NVARCHAR(MAX))
- CachSuDung: Hướng dẫn sử dụng (TEXT/NVARCHAR(MAX))
- GiaBan: Giá bán
- SoLuongTonKho: Số lượng tồn kho
- IdDanhMuc: Liên kết đến danh mục
- NgayTao: Ngày tạo sản phẩm
```

### 2. Repository Layer

#### ISanPhamRepository.GetByIdAsync():
```csharp
public async Task<SanPham?> GetByIdAsync(int id)
{
    return await _context.SanPhams
        .Include(s => s.IdDanhMucNavigation) // Load thông tin danh mục
        .Include(s => s.AnhSanPhams)         // Load hình ảnh
        .FirstOrDefaultAsync(s => s.IdSanPham == id);
}
```

### 3. Controller Implementation

#### ChiTietController.Index():
```csharp
public async Task<IActionResult> Index(int id)
{
    // Load sản phẩm từ database với đầy đủ thông tin
    var sanPham = await _sanPhamRepository.GetByIdAsync(id);
    
    if (sanPham == null)
    {
        return NotFound();
    }

    // Debug information (có thể xóa trong production)
    Console.WriteLine($"Product Description: {sanPham.MoTa ?? "NULL"}");
    Console.WriteLine($"Usage Instructions: {sanPham.CachSuDung ?? "NULL"}");

    return View(sanPham);
}
```

### 4. View Implementation

#### Hiển thị mô tả sản phẩm:
```html
@if (!string.IsNullOrWhiteSpace(Model?.MoTa))
{
    <div class="description-content">
        <div class="description-text">
            @Html.Raw(Model.MoTa.Replace("\\n", "<br>"))
        </div>
    </div>
}
else
{
    <div class="no-description">
        <i class="fas fa-info-circle"></i>
        <p>Chưa có mô tả chi tiết cho sản phẩm này.</p>
    </div>
}
```

#### Hiển thị hướng dẫn sử dụng:
```html
@if (!string.IsNullOrWhiteSpace(Model?.CachSuDung))
{
    <h6 class="usage-title mt-4">HƯỚNG DẪN SỬ DỤNG</h6>
    <div class="usage-content">
        <div class="usage-text">
            @Html.Raw(Model.CachSuDung.Replace("\\n", "<br>"))
        </div>
    </div>
}
```

## Cách thêm dữ liệu mô tả

### 1. Qua Admin Panel (Khuyến nghị)

Truy cập Admin Panel và cập nhật sản phẩm:
```
URL: /Admin/Sanpham
- Chọn sản phẩm cần chỉnh sửa
- Cập nhật trường "Mô tả" và "Cách sử dụng"
- Lưu thay đổi
```

### 2. Qua SQL Command

```sql
-- Cập nhật mô tả sản phẩm
UPDATE SanPham 
SET MoTa = N'Mô tả chi tiết về sản phẩm...',
    CachSuDung = N'Hướng dẫn sử dụng chi tiết...'
WHERE IdSanPham = 1;

-- Ví dụ với mô tả HTML
UPDATE SanPham 
SET MoTa = N'<p>Sản phẩm chăm sóc da cao cấp</p>
             <ul>
                <li>Chiết xuất từ thiên nhiên</li>
                <li>Không chứa paraben</li>
                <li>Phù hợp cho mọi loại da</li>
             </ul>',
    CachSuDung = N'<ol>
                    <li>Làm sạch da mặt</li>
                    <li>Thoa đều sản phẩm</li>
                    <li>Massage nhẹ nhàng</li>
                    <li>Rửa sạch với nước</li>
                   </ol>'
WHERE IdSanPham = 1;
```

### 3. Qua Entity Framework (Code)

```csharp
// Trong Service hoặc Repository
public async Task UpdateProductDescriptionAsync(int productId, string description, string usage)
{
    var product = await _context.SanPhams.FindAsync(productId);
    if (product != null)
    {
        product.MoTa = description;
        product.CachSuDung = usage;
        await _context.SaveChangesAsync();
    }
}
```

## Debug và Testing

### 1. Debug Action
Sử dụng endpoint debug để kiểm tra dữ liệu:
```
URL: /KhachHang/ChiTiet/DebugProduct?id=1
```

Response sẽ trả về JSON với thông tin sản phẩm:
```json
{
    "id": 1,
    "name": "Tên sản phẩm",
    "description": "Mô tả sản phẩm...",
    "usage": "Hướng dẫn sử dụng...",
    "price": 100000,
    "stock": 50,
    "category": "Danh mục"
}
```

### 2. Console Logging
Controller sẽ log thông tin vào console:
```
Product ID: 1
Product Name: Tên sản phẩm
Description: Mô tả chi tiết...
Usage: Hướng dẫn sử dụng...
```

### 3. Kiểm tra trong Browser
- Mở Developer Tools (F12)
- Vào tab Console để xem log
- Kiểm tra Network tab để xem request/response

## Tính năng đã implement

### ✅ Hiển thị từ Database
- ✅ Mô tả sản phẩm (Model.MoTa)
- ✅ Hướng dẫn sử dụng (Model.CachSuDung)
- ✅ Thông tin cơ bản (tên, giá, tồn kho)
- ✅ Thông tin danh mục

### ✅ Giao diện cải tiến
- ✅ Layout responsive
- ✅ Icon và styling đẹp mắt
- ✅ Bảng thông tin chi tiết
- ✅ Hiển thị placeholder khi không có dữ liệu

### ✅ HTML Support
- ✅ Hỗ trợ HTML trong mô tả
- ✅ Tự động convert \\n thành <br>
- ✅ @Html.Raw() để render HTML

### ✅ Error Handling
- ✅ Hiển thị thông báo khi không có mô tả
- ✅ Fallback cho dữ liệu null
- ✅ Graceful handling cho missing data

## Best Practices

### 1. Bảo mật
```csharp
// Sử dụng Html.Raw() cẩn thận, validate input trước khi lưu
var sanitizedDescription = HtmlSanitizer.Sanitize(description);
```

### 2. Performance
```csharp
// Include navigation properties khi cần thiết
.Include(s => s.IdDanhMucNavigation)
.Include(s => s.AnhSanPhams)
```

### 3. SEO
```html
<!-- Structured data cho SEO -->
<script type="application/ld+json">
{
    "@context": "https://schema.org/",
    "@type": "Product",
    "name": "@Model.TenSanPham",
    "description": "@Model.MoTa"
}
</script>
```

## Troubleshooting

### Vấn đề thường gặp:

1. **Mô tả không hiển thị:**
   - Kiểm tra dữ liệu trong database
   - Sử dụng DebugProduct action
   - Kiểm tra Console log

2. **HTML không render:**
   - Đảm bảo sử dụng @Html.Raw()
   - Kiểm tra HTML markup hợp lệ

3. **Encoding issues:**
   - Đảm bảo database sử dụng NVARCHAR
   - Set charset UTF-8

4. **Performance slow:**
   - Optimize database queries
   - Consider caching for frequently accessed products

## Kết luận

Hệ thống hiện tại đã hỗ trợ đầy đủ việc hiển thị mô tả sản phẩm từ database với:
- Repository Pattern để truy xuất dữ liệu
- View được optimize cho UX/UI
- Debug tools để troubleshooting
- Flexible content với HTML support
- Responsive design cho mobile

Để sử dụng, chỉ cần cập nhật dữ liệu trong database qua Admin Panel hoặc SQL, và hệ thống sẽ tự động hiển thị trên trang chi tiết sản phẩm.
