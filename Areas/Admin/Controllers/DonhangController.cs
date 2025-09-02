using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Final_VS1.Repositories;

namespace Final_VS1.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class DonhangController : Controller
    {
        private readonly IDonHangRepository _donHangRepository;

        public DonhangController(IDonHangRepository donHangRepository)
        {
            _donHangRepository = donHangRepository;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await _donHangRepository.GetAllAsync();
            return View(orders);
        }

        [HttpGet]
        public async Task<IActionResult> GetOrderDetail(int id)
        {
            var order = await _donHangRepository.GetByIdAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            var orderDetail = new
            {
                Id = order.IdDonHang,
                CustomerName = order.IdTaiKhoanNavigation?.HoTen ?? "Khách vãng lai",
                CustomerEmail = order.IdTaiKhoanNavigation?.Email ?? "N/A",
                ContactName = order.IdDiaChiNavigation?.HoTenNguoiNhan ?? "N/A",
                Phone = order.IdDiaChiNavigation?.SoDienThoai ?? "N/A",
                Address = order.IdDiaChiNavigation?.DiaChi1 ?? "N/A",
                OrderDate = order.NgayDat?.ToString("dd/MM/yyyy HH:mm") ?? "N/A",
                Status = order.TrangThai ?? "Không xác định",
                PaymentMethod = order.PhuongThucThanhToan ?? "Chưa xác định",
                TotalAmount = order.TongTien?.ToString("N0") + " ₫",
                Items = order.ChiTietDonHangs.Select(ct => new
                {
                    ProductName = ct.IdSanPhamNavigation?.TenSanPham ?? "Sản phẩm không xác định",
                    Quantity = ct.SoLuong ?? 0,
                    Price = ct.GiaLucDat?.ToString("N0") + " ₫",
                    SubTotal = ((ct.SoLuong ?? 0) * (ct.GiaLucDat ?? 0)).ToString("N0") + " ₫"
                }).ToList()
            };

            return Json(orderDetail);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            try
            {
                // Lấy đơn hàng hiện tại để kiểm tra trạng thái
                var currentOrder = await _donHangRepository.GetByIdAsync(id);
                if (currentOrder == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy đơn hàng" });
                }

                // Kiểm tra nếu đơn hàng đã hủy hoặc hoàn thành thì không cho phép thay đổi
                if (currentOrder.TrangThai == "Đã hủy")
                {
                    return Json(new { success = false, message = "Đơn hàng đã hủy không thể thay đổi trạng thái" });
                }

                if (currentOrder.TrangThai == "Hoàn thành")
                {
                    return Json(new { success = false, message = "Đơn hàng đã hoàn thành không thể thay đổi trạng thái" });
                }

                // Kiểm tra luồng trạng thái hợp lệ
                var validTransitions = new Dictionary<string, List<string>>
                {
                    ["Đang xử lý"] = new List<string> { "Đã xác nhận", "Đã hủy" },
                    ["Đã xác nhận"] = new List<string> { "Đang giao", "Đã hủy" },
                    ["Đang giao"] = new List<string> { "Hoàn thành", "Đã hủy" }
                };

                var currentStatus = currentOrder.TrangThai ?? "";
                if (validTransitions.ContainsKey(currentStatus) && 
                    !validTransitions[currentStatus].Contains(status))
                {
                    return Json(new { success = false, message = $"Không thể chuyển từ '{currentStatus}' sang '{status}'" });
                }

                await _donHangRepository.UpdateStatusAsync(id, status);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }
    }
}