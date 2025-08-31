using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Final_VS1.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Final_VS1.Areas.KhachHang.Controllers
{
    [Area("KhachHang")]
    [Authorize]
    public class PayController : Controller
    {
        private readonly LittleFishBeautyContext _context;

        public PayController(LittleFishBeautyContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Kiểm tra TempData trước
            if (TempData["BuyNowItem"] != null)
            {
                try
                {
                    var buyNowItemJson = TempData["BuyNowItem"]?.ToString();
                    if (!string.IsNullOrEmpty(buyNowItemJson))
                    {
                        // Truyền JSON string trực tiếp, không cần serialize lại
                        ViewBag.BuyNowItem = buyNowItemJson;
                        ViewBag.IsBuyNow = true;
                        
                        // Debug logging
                        Console.WriteLine($"PayController - BuyNowItem from TempData: {buyNowItemJson}");
                        return View();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"PayController - Error processing TempData BuyNowItem: {ex.Message}");
                }
            }
            
            // Fallback: Kiểm tra Session
            try
            {
                var sessionBuyNowItem = HttpContext.Session.GetString("BuyNowItem");
                if (!string.IsNullOrEmpty(sessionBuyNowItem))
                {
                    ViewBag.BuyNowItem = sessionBuyNowItem;
                    ViewBag.IsBuyNow = true;
                    
                    Console.WriteLine($"PayController - BuyNowItem from Session: {sessionBuyNowItem}");
                    
                    // Clear session after use
                    HttpContext.Session.Remove("BuyNowItem");
                    return View();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"PayController - Error processing Session BuyNowItem: {ex.Message}");
            }
            
            // Không có dữ liệu Buy Now
            ViewBag.IsBuyNow = false;
            Console.WriteLine("PayController - No BuyNowItem found in TempData or Session");
            return View();
        }

[HttpPost]
public async Task<IActionResult> ProcessOrder([FromBody] ProcessOrderRequest request)
{
    try
    {
        // Log toàn bộ request để debug
        var requestJson = System.Text.Json.JsonSerializer.Serialize(request, new System.Text.Json.JsonSerializerOptions 
        { 
            WriteIndented = true 
        });
        Console.WriteLine($"=== RECEIVED REQUEST ===\n{requestJson}");
        
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            Console.WriteLine("User not logged in");
            return Json(new { success = false, message = "Chưa đăng nhập" });
        }
        Console.WriteLine($"User ID: {userId}");

        // Validate input
        if (string.IsNullOrWhiteSpace(request.HoTen))
        {
            Console.WriteLine("Missing HoTen");
            return Json(new { success = false, message = "Vui lòng nhập họ tên người nhận" });
        }

        if (string.IsNullOrWhiteSpace(request.SoDienThoai))
        {
            Console.WriteLine("Missing SoDienThoai");
            return Json(new { success = false, message = "Vui lòng nhập số điện thoại người nhận" });
        }

        if (string.IsNullOrWhiteSpace(request.DiaChi))
        {
            Console.WriteLine("Missing DiaChi");
            return Json(new { success = false, message = "Vui lòng nhập địa chỉ người nhận" });
        }

        if (request.OrderItems == null || !request.OrderItems.Any())
        {
            Console.WriteLine("No order items found");
            return Json(new { success = false, message = "Không có sản phẩm nào để đặt hàng" });
        }

        Console.WriteLine($"Processing {request.OrderItems.Count} items");
        
        using var transaction = await _context.Database.BeginTransactionAsync();
        
        // Calculate total amount and validate stock
        decimal totalAmount = 0;
        var validatedItems = new List<(int ProductId, int SoLuong, decimal DonGia, SanPham Product)>();

        foreach (var item in request.OrderItems)
        {
            Console.WriteLine($"Processing item: ProductId={item.ProductId}, SoLuong={item.SoLuong}, DonGia={item.DonGia}");
            
            var product = await _context.SanPhams
                .FirstOrDefaultAsync(sp => sp.IdSanPham == item.ProductId);

            if (product == null || product.TrangThai != true)
            {
                Console.WriteLine($"Product not found or inactive: {item.ProductId}");
                return Json(new { success = false, message = $"Sản phẩm ID {item.ProductId} không tồn tại hoặc đã ngừng bán" });
            }

            if ((product.SoLuongTonKho ?? 0) < item.SoLuong)
            {
                Console.WriteLine($"Insufficient stock for product {product.TenSanPham}: Available={product.SoLuongTonKho}, Requested={item.SoLuong}");
                return Json(new { success = false, message = $"Sản phẩm {product.TenSanPham} không đủ số lượng trong kho. Còn lại: {product.SoLuongTonKho ?? 0}" });
            }

            var donGia = product.GiaBan ?? 0;
            totalAmount += donGia * item.SoLuong;
            
            validatedItems.Add((item.ProductId, item.SoLuong, donGia, product));
            Console.WriteLine($"Validated item: {product.TenSanPham}, Price: {donGia}, Qty: {item.SoLuong}, Subtotal: {donGia * item.SoLuong}");
        }

        Console.WriteLine($"Total amount calculated: {totalAmount}");

        // Create order
       var donHang = new DonHang
        {
            IdTaiKhoan = userId,
            NgayDat = DateTime.Now,
            TrangThai = "Chờ xác nhận",
            PhuongThucThanhToan = request.PaymentMethod ?? "COD",
            TongTien = totalAmount + 30000,
            HoTenNguoiNhan = request.HoTen.Trim(),
            SoDienThoai = request.SoDienThoai.Trim(),
            DiaChi = request.DiaChi.Trim() // Đổi thành DiaChi
        };

        _context.DonHangs.Add(donHang);
        await _context.SaveChangesAsync();
        
        Console.WriteLine($"Order created with ID: {donHang.IdDonHang}");

        // Add order details and update stock
        foreach (var item in validatedItems)
        {
            var chiTiet = new ChiTietDonHang
            {
                IdDonHang = donHang.IdDonHang,
                IdSanPham = item.ProductId,
                SoLuong = item.SoLuong,
                GiaLucDat = item.DonGia
            };
            _context.ChiTietDonHangs.Add(chiTiet);

            // Update product stock
            item.Product.SoLuongTonKho = (item.Product.SoLuongTonKho ?? 0) - item.SoLuong;
            if (item.Product.SoLuongTonKho < 0) 
                item.Product.SoLuongTonKho = 0;
                
            Console.WriteLine($"Updated stock for {item.Product.TenSanPham}: New stock = {item.Product.SoLuongTonKho}");
        }

        await _context.SaveChangesAsync();
        await transaction.CommitAsync();

        Console.WriteLine("=== ORDER PROCESSED SUCCESSFULLY ===");

        return Json(new { 
            success = true, 
            message = "Đặt hàng thành công",
            orderId = donHang.IdDonHang,
            orderCode = $"DH{donHang.IdDonHang:D6}"
        });
    }
catch (Exception ex)
{
    Console.WriteLine($"=== ERROR IN PROCESS ORDER ===");
    Console.WriteLine($"Error: {ex.Message}");
    Console.WriteLine($"Inner: {ex.InnerException?.Message}");
    return Json(new { success = false, message = $"Lỗi hệ thống: {ex.Message} - {ex.InnerException?.Message}" });
}
}


        // Lấy ID của user hiện tại
        private int? GetCurrentUserId()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(userIdClaim, out int userId))
                {
                    return userId;
                }
            }
            return null;
        }

        // Request models
       public class ProcessOrderRequest
        {
            public string? PaymentMethod { get; set; }
            public decimal TotalAmount { get; set; }
            public string? HoTen { get; set; }
            public string? SoDienThoai { get; set; }
            public string? DiaChi { get; set; } // Thêm dòng này
            public List<OrderItemRequest> OrderItems { get; set; } = new List<OrderItemRequest>();
        }

        public class OrderItemRequest
        {
            public int ProductId { get; set; }
            public int SoLuong { get; set; }
            public decimal DonGia { get; set; }
        }
    }
}
         