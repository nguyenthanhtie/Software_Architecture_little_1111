using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Final_VS1.Repositories;
using Final_VS1.Areas.KhachHang.ViewModels;
using System.Security.Claims;

namespace Final_VS1.Areas.KhachHang.Controllers
{
    [Area("KhachHang")]
    [Authorize]
    public class DonHangController : Controller
    {
        private readonly IDonHangRepository _donHangRepo;
        private readonly ILogger<DonHangController> _logger;

        public DonHangController(IDonHangRepository donHangRepo, ILogger<DonHangController> logger)
        {
            _donHangRepo = donHangRepo;
            _logger = logger;
        }

        public async Task<IActionResult> Index(string? status)
        {
            try
            {
                var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
                {
                    return RedirectToAction("Login", "Account");
                }

                var donHangs = await _donHangRepo.GetByUserAsync(userId, status);

                var viewModel = new DonHangViewModel
                {
                    DonHangs = donHangs,
                    CurrentFilter = status,
                    TotalOrders = await _donHangRepo.CountByUserAsync(userId),
                    PendingOrders = await _donHangRepo.CountByUserAsync(userId, "Chờ xác nhận"),
                    ProcessingOrders = await _donHangRepo.CountByUserAsync(userId, "Đang xử lý"),
                    ConfirmedOrders = await _donHangRepo.CountByUserAsync(userId, "Đã xác nhận"),
                    ShippingOrders = await _donHangRepo.CountByUserAsync(userId, "Đang giao"),
                    DeliveredOrders = await _donHangRepo.CountByUserAsync(userId, "Hoàn thành"),
                    CancelledOrders = await _donHangRepo.CountByUserAsync(userId, "Đã hủy")
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
                var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
                {
                    return Json(new { success = false, message = "Vui lòng đăng nhập để thực hiện chức năng này" });
                }

                var success = await _donHangRepo.CancelOrderAsync(request.Id, userId);
                if (!success)
                {
                    return Json(new { success = false, message = "Không thể hủy đơn hàng. Đơn hàng không tồn tại hoặc không ở trạng thái chờ xác nhận." });
                }

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