using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Final_VS1.Data;
using Final_VS1.Areas.KhachHang.ViewModels;
using System.Security.Claims;

namespace Final_VS1.Areas.KhachHang.Controllers
{
    [Area("KhachHang")]
    [Authorize]
    public class DonHangController : Controller
    {
        private readonly LittleFishBeautyContext _context;
        private readonly ILogger<DonHangController> _logger;

        public DonHangController(LittleFishBeautyContext context, ILogger<DonHangController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index(string? status)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return RedirectToAction("Login", "Account");
                }

                var query = _context.DonHangs
                    .Include(d => d.ChiTietDonHangs)
                        .ThenInclude(ct => ct.IdSanPhamNavigation)
                            .ThenInclude(sp => sp.AnhSanPhams)
                    .Include(d => d.IdTaiKhoanNavigation)
                    .Where(d => d.IdTaiKhoan == int.Parse(userId));

                if (!string.IsNullOrEmpty(status))
                {
                    query = query.Where(d => d.TrangThai == status);
                }

                var donHangs = await query.OrderByDescending(d => d.NgayDat).ToListAsync();

                var viewModel = new DonHangViewModel
                {
                    DonHangs = donHangs,
                    CurrentFilter = status,
                    TotalOrders = await _context.DonHangs.CountAsync(d => d.IdTaiKhoan == int.Parse(userId)),
                    PendingOrders = await _context.DonHangs.CountAsync(d => d.IdTaiKhoan == int.Parse(userId) && d.TrangThai == "Chờ xác nhận"),
                    ProcessingOrders = await _context.DonHangs.CountAsync(d => d.IdTaiKhoan == int.Parse(userId) && d.TrangThai == "Đang xử lý"),
                    ConfirmedOrders = await _context.DonHangs.CountAsync(d => d.IdTaiKhoan == int.Parse(userId) && d.TrangThai == "Đã xác nhận"),
                    ShippingOrders = await _context.DonHangs.CountAsync(d => d.IdTaiKhoan == int.Parse(userId) && d.TrangThai == "Đang giao"),
                    DeliveredOrders = await _context.DonHangs.CountAsync(d => d.IdTaiKhoan == int.Parse(userId) && d.TrangThai == "Hoàn thành"),
                    CancelledOrders = await _context.DonHangs.CountAsync(d => d.IdTaiKhoan == int.Parse(userId) && d.TrangThai == "Đã hủy")
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Index action");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải danh sách đơn hàng";
                return View(new DonHangViewModel());
            }
        }

        [HttpPost]
        public async Task<IActionResult> CancelOrder([FromBody] CancelOrderRequest request)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Json(new { success = false, message = "Vui lòng đăng nhập để thực hiện chức năng này" });
                }

                var donHang = await _context.DonHangs
                    .FirstOrDefaultAsync(d => d.IdDonHang == request.Id && d.IdTaiKhoan == int.Parse(userId));

                if (donHang == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy đơn hàng" });
                }

                if (donHang.TrangThai != "Chờ xác nhận")
                {
                    return Json(new { success = false, message = "Chỉ có thể hủy đơn hàng đang chờ xác nhận" });
                }

                donHang.TrangThai = "Đã hủy";
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Order {request.Id} cancelled by user {userId}");
                return Json(new { success = true, message = "Hủy đơn hàng thành công" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error cancelling order {request.Id}");
                return Json(new { success = false, message = "Có lỗi xảy ra khi hủy đơn hàng" });
            }
        }

        public class CancelOrderRequest
        {
            public int Id { get; set; }
        }
    }
}