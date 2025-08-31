using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Final_VS1.Data;
using Final_VS1.Repositories;
using System.Security.Claims;

namespace Final_VS1.Areas.KhachHang.Controllers
{
    [Area("KhachHang")]
    [Authorize]
    public class PayController : Controller
    {
        private readonly ISanPhamRepository _sanPhamRepo;
        private readonly IDonHangRepository _donHangRepo;

        public PayController(ISanPhamRepository sanPhamRepo, IDonHangRepository donHangRepo)
        {
            _sanPhamRepo = sanPhamRepo;
            _donHangRepo = donHangRepo;
        }

        public IActionResult Index()
        {
            if (TempData["BuyNowItem"] != null)
            {
                try
                {
                    var buyNowItemJson = TempData["BuyNowItem"]?.ToString();
                    if (!string.IsNullOrEmpty(buyNowItemJson))
                    {
                        ViewBag.BuyNowItem = buyNowItemJson;
                        ViewBag.IsBuyNow = true;
                        Console.WriteLine($"PayController - BuyNowItem from TempData: {buyNowItemJson}");
                        return View();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"PayController - Error processing TempData BuyNowItem: {ex.Message}");
                }
            }

            try
            {
                var sessionBuyNowItem = HttpContext.Session.GetString("BuyNowItem");
                if (!string.IsNullOrEmpty(sessionBuyNowItem))
                {
                    ViewBag.BuyNowItem = sessionBuyNowItem;
                    ViewBag.IsBuyNow = true;
                    Console.WriteLine($"PayController - BuyNowItem from Session: {sessionBuyNowItem}");
                    HttpContext.Session.Remove("BuyNowItem");
                    return View();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"PayController - Error processing Session BuyNowItem: {ex.Message}");
            }

            ViewBag.IsBuyNow = false;
            Console.WriteLine("PayController - No BuyNowItem found in TempData or Session");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ProcessOrder([FromBody] ProcessOrderRequest request)
        {
            try
            {
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

                if (string.IsNullOrWhiteSpace(request.HoTen))
                    return Json(new { success = false, message = "Vui lòng nhập họ tên người nhận" });

                if (string.IsNullOrWhiteSpace(request.SoDienThoai))
                    return Json(new { success = false, message = "Vui lòng nhập số điện thoại người nhận" });

                if (string.IsNullOrWhiteSpace(request.DiaChi))
                    return Json(new { success = false, message = "Vui lòng nhập địa chỉ người nhận" });

                if (request.OrderItems == null || !request.OrderItems.Any())
                    return Json(new { success = false, message = "Không có sản phẩm nào để đặt hàng" });

                Console.WriteLine($"Processing {request.OrderItems.Count} items");

                decimal totalAmount = 0;
                var validatedItems = new List<(int ProductId, int SoLuong, decimal DonGia, SanPham Product)>();

                foreach (var item in request.OrderItems)
                {
                    Console.WriteLine($"Processing item: ProductId={item.ProductId}, SoLuong={item.SoLuong}, DonGia={item.DonGia}");

                    var product = await _sanPhamRepo.GetByIdAsync(item.ProductId);

                    if (product == null || product.TrangThai != true)
                        return Json(new { success = false, message = $"Sản phẩm ID {item.ProductId} không tồn tại hoặc đã ngừng bán" });

                    if ((product.SoLuongTonKho ?? 0) < item.SoLuong)
                        return Json(new { success = false, message = $"Sản phẩm {product.TenSanPham} không đủ số lượng trong kho. Còn lại: {product.SoLuongTonKho ?? 0}" });

                    var donGia = product.GiaBan ?? 0;
                    totalAmount += donGia * item.SoLuong;
                    validatedItems.Add((item.ProductId, item.SoLuong, donGia, product));
                    Console.WriteLine($"Validated item: {product.TenSanPham}, Price: {donGia}, Qty: {item.SoLuong}, Subtotal: {donGia * item.SoLuong}");
                }

                Console.WriteLine($"Total amount calculated: {totalAmount}");

                var donHang = new DonHang
                {
                    IdTaiKhoan = userId,
                    NgayDat = DateTime.Now,
                    TrangThai = "Chờ xác nhận",
                    PhuongThucThanhToan = request.PaymentMethod ?? "COD",
                    TongTien = totalAmount + 30000,
                    HoTenNguoiNhan = request.HoTen.Trim(),
                    SoDienThoai = request.SoDienThoai.Trim(),
                    DiaChi = request.DiaChi.Trim()
                };

                var chiTietDonHangs = new List<ChiTietDonHang>();
                foreach (var item in validatedItems)
                {
                    var chiTiet = new ChiTietDonHang
                    {
                        IdSanPham = item.ProductId,
                        SoLuong = item.SoLuong,
                        GiaLucDat = item.DonGia
                    };
                    chiTietDonHangs.Add(chiTiet);

                    // Update product stock
                    var newStock = (item.Product.SoLuongTonKho ?? 0) - item.SoLuong;
                    if (newStock < 0) newStock = 0;
                    await _sanPhamRepo.UpdateStockAsync(item.ProductId, newStock);
                    Console.WriteLine($"Updated stock for {item.Product.TenSanPham}: New stock = {newStock}");
                }

                var createdOrder = await _donHangRepo.CreateOrderAsync(donHang, chiTietDonHangs);

                Console.WriteLine("=== ORDER PROCESSED SUCCESSFULLY ===");

                return Json(new
                {
                    success = true,
                    message = "Đặt hàng thành công",
                    orderId = createdOrder.IdDonHang,
                    orderCode = $"DH{createdOrder.IdDonHang:D6}",
                    redirectUrl = Url.Action("Index", "SanPham", new { area = "KhachHang" })
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

        public class ProcessOrderRequest
        {
            public string? PaymentMethod { get; set; }
            public decimal TotalAmount { get; set; }
            public string? HoTen { get; set; }
            public string? SoDienThoai { get; set; }
            public string? DiaChi { get; set; }
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