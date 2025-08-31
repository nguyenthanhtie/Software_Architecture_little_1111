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
                ContactName = order.HoTenNguoiNhan ?? "N/A",
                Phone = order.SoDienThoai ?? "N/A",
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
            await _donHangRepository.UpdateStatusAsync(id, status);
            return Json(new { success = true });
        }
    }
}