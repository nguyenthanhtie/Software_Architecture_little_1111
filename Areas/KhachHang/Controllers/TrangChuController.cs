using Microsoft.AspNetCore.Mvc;
using Final_VS1.Repositories;
using Final_VS1.Data;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Final_VS1.Areas.KhachHang.Controllers
{
    [Area("KhachHang")]
    public class TrangChuController : Controller
    {
        private readonly IDanhMucRepository _danhMucRepository;
        private readonly ISanPhamRepository _sanPhamRepository;

        public TrangChuController(IDanhMucRepository danhMucRepository, ISanPhamRepository sanPhamRepository)
        {
            _danhMucRepository = danhMucRepository;
            _sanPhamRepository = sanPhamRepository;
        }

        public async Task<IActionResult> Index()
        {
            // Lấy danh mục cha
            var danhMucs = await _danhMucRepository.GetParentCategoriesAsync();

            // Tính số sản phẩm cho từng danh mục
            var danhMucVoiSoLuong = new List<dynamic>();
            foreach (var dm in danhMucs)
            {
                var soLuongSanPham = await _danhMucRepository.GetProductCountByCategoryAsync(dm.IdDanhMuc);
                danhMucVoiSoLuong.Add(new {
                    IdDanhMuc = dm.IdDanhMuc,
                    TenDanhMuc = dm.TenDanhMuc,
                    SoLuongSanPham = soLuongSanPham,
                    Slug = dm.IdDanhMuc
                });
            }

            // Lấy sản phẩm mới
            var sanPhamMoi = await _sanPhamRepository.GetNewestProductsAsync(4);

            // Lấy sản phẩm bán chạy
            var sanPhamBanChay = await _sanPhamRepository.GetBestSellingProductsAsync(8);

            ViewBag.DanhMucs = danhMucVoiSoLuong;
            ViewBag.SanPhamMoi = sanPhamMoi;
            ViewBag.SanPhamBanChay = sanPhamBanChay;

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
                var categories = await _danhMucRepository.GetParentCategoriesAsync();
                return PartialView("_CategoryDropdown", categories);
            }
            catch
            {
                return PartialView("_CategoryDropdown", new List<DanhMuc>());
            }
        }
    }
}