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

        // Xử lý đặt hàng
        [HttpPost]
        public async Task<IActionResult> ProcessOrder([FromBody] ProcessOrderRequest request)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Json(new { success = false, message = "Chưa đăng nhập" });
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            
            try
            {
                // Validate order items
                if (!request.OrderItems.Any())
                {
                    return Json(new { success = false, message = "Không có sản phẩm nào để đặt hàng" });
                }

                // Calculate total amount and validate stock
                decimal totalAmount = 0;
                var validatedItems = new List<OrderItemRequest>();

                foreach (var item in request.OrderItems)
                {
                    var product = await _context.SanPhams
                        .FirstOrDefaultAsync(sp => sp.IdSanPham == item.ProductId);

                    if (product == null || product.TrangThai != true)
                    {
                        return Json(new { success = false, message = "Sản phẩm không tồn tại hoặc đã ngừng bán" });
                    }

                    if ((product.SoLuongTonKho ?? 0) < item.SoLuong)
                    {
                        return Json(new { success = false, message = $"Sản phẩm {product.TenSanPham} không đủ số lượng trong kho" });
                    }

                    totalAmount += (product.GiaBan ?? 0) * item.SoLuong;
                    validatedItems.Add(new OrderItemRequest
                    {
                        ProductId = item.ProductId,
                        SoLuong = item.SoLuong,
                        DonGia = product.GiaBan ?? 0
                    });
                }

                // Create order with default delivery information
                var user = await _context.TaiKhoans.FindAsync(userId);
                var donHang = new DonHang
                {
                    IdTaiKhoan = userId,
                    NgayDat = DateTime.Now,
                    TrangThai = "Chờ xác nhận",
                    PhuongThucThanhToan = request.PaymentMethod ?? "COD",
                    TongTien = totalAmount + 30000, // Add shipping fee
                    HoTenNguoiNhan = request.HoTen ?? user?.HoTen ?? "Khách hàng",
                    SoDienThoai = request.SoDienThoai ?? "COD"
                };

                _context.DonHangs.Add(donHang);
                await _context.SaveChangesAsync();

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

                    var product = await _context.SanPhams.FindAsync(item.ProductId);
                    if (product != null)
                    {
                        product.SoLuongTonKho -= item.SoLuong;
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Json(new { 
                    success = true, 
                    message = "Đặt hàng thành công",
                    orderId = donHang.IdDonHang,
                    orderCode = $"DH{donHang.IdDonHang:D6}"
                });
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return Json(new { success = false, message = "Có lỗi xảy ra khi đặt hàng. Vui lòng thử lại." });
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
         