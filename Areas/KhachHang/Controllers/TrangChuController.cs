using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Final_VS1.Data;
using System.Threading.Tasks;
using System.Linq;

namespace Final_VS1.Areas.KhachHang.Controllers
{
    [Area("KhachHang")]
    public class TrangChuController : Controller
    {
        private readonly LittleFishBeautyContext _context;

        public TrangChuController(LittleFishBeautyContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {

            // Lấy danh mục cha
            var danhMucs = await _context.DanhMucs
                .Where(d => d.IdDanhMucCha == null)
                .OrderBy(d => d.ThuTuHienThi)
                .Take(6)
                .ToListAsync();

            // Tính số sản phẩm cho từng danh mục
            var danhMucVoiSoLuong = new System.Collections.Generic.List<dynamic>();
            foreach (var dm in danhMucs)
            {
                var soLuongSanPham = await _context.SanPhams
                    .Where(s => s.IdDanhMuc == dm.IdDanhMuc && s.TrangThai == true)
                    .CountAsync();

                danhMucVoiSoLuong.Add(new {
                    IdDanhMuc = dm.IdDanhMuc,
                    TenDanhMuc = dm.TenDanhMuc,
                    SoLuongSanPham = soLuongSanPham,
                    Slug = dm.IdDanhMuc // Đổi thành lấy IdDanhMuc thay vì DuongDanSeo
                });
            }

            // Lấy sản phẩm mới
            var sanPhamMoi = await _context.SanPhams
                .Include(s => s.AnhSanPhams) // Include ảnh sản phẩm
                .Where(s => s.TrangThai == true)
                .OrderByDescending(s => s.NgayTao)
                .Take(4)
                .ToListAsync();

            ViewBag.SanPhamMoi = sanPhamMoi;

            // Lấy sản phẩm bán chạy dựa trên số lượng đã bán trong đơn hàng
            var sanPhamBanChay = await _context.ChiTietDonHangs
                .Include(ct => ct.IdSanPhamNavigation!)
                    .ThenInclude(sp => sp.AnhSanPhams)
                .Include(ct => ct.IdDonHangNavigation)
                .Where(ct => ct.IdSanPhamNavigation != null && 
                           ct.IdDonHangNavigation != null &&
                           ct.IdSanPhamNavigation.TrangThai == true && 
                           ct.IdDonHangNavigation.TrangThai != "Đã hủy") // Chỉ tính đơn hàng không bị hủy
                .GroupBy(ct => ct.IdSanPham)
                .Select(g => new {
                    IdSanPham = g.Key,
                    TongSoLuongBan = g.Sum(ct => ct.SoLuong ?? 0)
                })
                .OrderByDescending(x => x.TongSoLuongBan)
                .Take(8) // Lấy 8 sản phẩm bán chạy nhất
                .Join(_context.SanPhams.Include(sp => sp.AnhSanPhams),
                      bc => bc.IdSanPham,
                      sp => sp.IdSanPham,
                      (bc, sp) => new {
                          SanPham = sp,
                          TongSoLuongBan = bc.TongSoLuongBan,
                          ReviewCount = 0, // Tạm thời để 0 vì chưa có bảng đánh giá
                          AverageRating = 0.0 // Tạm thời để 0 vì chưa có bảng đánh giá
                      })
                .ToListAsync();

            ViewBag.SanPhamBanChay = sanPhamBanChay;

            ViewBag.DanhMucs = danhMucVoiSoLuong;

            return View();
        }

        [HttpPost]
        public IActionResult TimKiem(string searchQuery)
        {
            return RedirectToAction("Index", "SanPham", new { search = searchQuery });
        }

        public IActionResult DanhMuc(string category)
        {
            return RedirectToAction("Index", "SanPham", new { category = category });
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            try
            {
                var categories = await _context.DanhMucs
                    .Where(d => d.IdDanhMucCha == null) // Only parent categories
                    .OrderBy(d => d.ThuTuHienThi ?? 0)
                    .ThenBy(d => d.TenDanhMuc)
                    .ToListAsync();

                return PartialView("_CategoryDropdown", categories);
            }
            catch (Exception)
            {
                // Log error if needed
                return PartialView("_CategoryDropdown", new List<DanhMuc>());
            }
        }
    }
}
         