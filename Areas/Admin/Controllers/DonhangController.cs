using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Final_VS1.Models;
using Final_VS1.Data;
using Microsoft.AspNetCore.Authorization;


namespace Final_VS1.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class DonhangController : Controller
    {
        private readonly LittleFishBeautyContext _context;

        public DonhangController(LittleFishBeautyContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await _context.DonHangs
                .Include(d => d.IdTaiKhoanNavigation)
                .Include(d => d.ChiTietDonHangs)
                    .ThenInclude(ct => ct.IdSanPhamNavigation)
                .OrderByDescending(d => d.NgayDat)
                .ToListAsync();

            return View(orders);
        }

        [HttpGet]
        public async Task<IActionResult> GetOrderDetail(int id)
        {
            var order = await _context.DonHangs
                .Include(d => d.IdTaiKhoanNavigation)
                .Include(d => d.ChiTietDonHangs)
                    .ThenInclude(ct => ct.IdSanPhamNavigation)
                .FirstOrDefaultAsync(d => d.IdDonHang == id);

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
            var order = await _context.DonHangs.FindAsync(id);
            if (order == null)
            {
                return Json(new { success = false });
            }

            order.TrangThai = status;
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }
    }
}