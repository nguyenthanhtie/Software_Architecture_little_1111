using Microsoft.AspNetCore.Mvc;
using Final_VS1.Repositories;
using Final_VS1.Helpers;
using Final_VS1.Areas.KhachHang.Models;
using Final_VS1.Data;
using System.Threading.Tasks;
using System.Linq;

namespace Final_VS1.Areas.KhachHang.Controllers
{
    [Area("KhachHang")]
    public class SanPhamController : Controller
    {
        private readonly ISanPhamRepository _sanPhamRepository;
        private readonly IDanhMucRepository _danhMucRepository;

        public SanPhamController(ISanPhamRepository sanPhamRepository, IDanhMucRepository danhMucRepository)
        {
            _sanPhamRepository = sanPhamRepository;
            _danhMucRepository = danhMucRepository;
        }

        // Helper method để lấy ảnh sản phẩm theo thứ tự ưu tiên
        private List<string> GetProductImages(ICollection<AnhSanPham>? anhSanPhams)
        {
            var result = new List<string>();

            if (anhSanPhams != null && anhSanPhams.Any())
            {
                var anhChinh = anhSanPhams.FirstOrDefault(a =>
                    !string.IsNullOrEmpty(a.LoaiAnh) && !string.IsNullOrEmpty(a.DuongDan) &&
                    (a.LoaiAnh.Trim().ToLower() == "chinh" || a.LoaiAnh.Trim().ToLower() == "chính"));

                if (anhChinh != null)
                {
                    result.Add(anhChinh.DuongDan!);
                }
                else
                {
                    var anhPhu = anhSanPhams.FirstOrDefault(a =>
                        !string.IsNullOrEmpty(a.LoaiAnh) && !string.IsNullOrEmpty(a.DuongDan) &&
                        (a.LoaiAnh.Trim().ToLower() == "phu" || a.LoaiAnh.Trim().ToLower() == "phụ"));

                    if (anhPhu != null)
                    {
                        result.Add(anhPhu.DuongDan!);
                    }
                }
            }

            return result;
        }

        public async Task<IActionResult> Index(string search, string category, decimal? minPrice, decimal? maxPrice, string sortBy, int page = 1)
        {
            const int pageSize = 12;

            var sanPhams = await _sanPhamRepository.GetAllAsync();
            var query = sanPhams.Where(s => s.TrangThai == true).AsQueryable();

            // Tìm kiếm theo từ khóa
         if (!string.IsNullOrEmpty(search))
{
    string searchLower = search.ToLower();
    query = query.Where(s =>
        (s.TenSanPham ?? "").ToLower().Contains(searchLower) ||
        (s.MoTa ?? "").ToLower().Contains(searchLower) ||
        ((s.IdDanhMucNavigation != null && s.IdDanhMucNavigation.TenDanhMuc != null)
            ? s.IdDanhMucNavigation.TenDanhMuc.ToLower()
            : "").Contains(searchLower)
    );
    ViewBag.SearchQuery = search;
}

            // Lọc theo danh mục
            DanhMuc? selectedCategory = null;
            if (!string.IsNullOrEmpty(category))
            {
                if (int.TryParse(category, out int categoryId))
                {
                    selectedCategory = await _danhMucRepository.GetByIdAsync(categoryId);
                    if (selectedCategory != null)
                    {
                        query = query.Where(s => s.IdDanhMuc == selectedCategory.IdDanhMuc);
                        ViewBag.CurrentCategory = selectedCategory;
                    }
                }
            }

            // Lọc theo giá
            if (minPrice.HasValue)
            {
                query = query.Where(s => s.GiaBan >= minPrice.Value);
                ViewBag.MinPrice = minPrice;
            }
            if (maxPrice.HasValue)
            {
                query = query.Where(s => s.GiaBan <= maxPrice.Value);
                ViewBag.MaxPrice = maxPrice;
            }

            // Sắp xếp
            switch (sortBy?.ToLower())
            {
                case "newest":
                    query = query.OrderByDescending(s => s.NgayTao);
                    break;
                case "price-asc":
                    query = query.OrderBy(s => s.GiaBan);
                    break;
                case "price-desc":
                    query = query.OrderByDescending(s => s.GiaBan);
                    break;
                default:
                    query = query.OrderByDescending(s => s.NgayTao);
                    break;
            }

            int totalProducts = query.Count();
            int totalPages = (int)Math.Ceiling((double)totalProducts / pageSize);
            int skip = (page - 1) * pageSize;
            var pagedSanPhams = query.Skip(skip).Take(pageSize).ToList();

            var sanPhamViewModels = pagedSanPhams.Select(sp => new SanPhamViewModel
            {
                IdSanPham = sp.IdSanPham,
                TenSanPham = sp.TenSanPham,
                MoTa = sp.MoTa,
                GiaBan = sp.GiaBan,
                TrangThai = sp.TrangThai,
                IdDanhMuc = sp.IdDanhMuc,
                CachSuDung = sp.CachSuDung,
                NgayTao = sp.NgayTao,
                TenDanhMuc = sp.IdDanhMucNavigation?.TenDanhMuc,
                AnhChinhs = GetProductImages(sp.AnhSanPhams)
            }).ToList();

            // Lấy danh sách danh mục cho sidebar
            var danhMucs = await _danhMucRepository.GetParentCategoriesAsync();

            ViewBag.DanhMucs = danhMucs;
            ViewBag.SortBy = sortBy;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalProducts = totalProducts;
            ViewBag.PageSize = pageSize;
            ViewBag.CurrentSearch = search;
            ViewBag.CurrentCategoryString = category;
            ViewBag.CurrentMinPrice = minPrice;
            ViewBag.CurrentMaxPrice = maxPrice;
            ViewBag.CurrentSortBy = sortBy;

            return View(sanPhamViewModels);
        }

        public IActionResult TimKiem(string searchTerm)
        {
            return RedirectToAction("Index", new { search = searchTerm });
        }

        public IActionResult DanhMuc(string category)
        {
            return RedirectToAction("Index", new { category = category });
        }

        public IActionResult LuocGia(decimal? minPrice, decimal? maxPrice)
        {
            return RedirectToAction("Index", new { minPrice = minPrice, maxPrice = maxPrice });
        }

        public IActionResult SapXep(string sortBy)
        {
            return RedirectToAction("Index", new { sortBy = sortBy });
        }

        [HttpGet]
        public async Task<IActionResult> SearchSuggestions(string term)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(term) || term.Length < 1)
                {
                    return Json(new List<object>());
                }

                var normalizedTerm = VietnameseTextHelper.RemoveDiacritics(term.Trim().ToLower());
                var sanPhams = await _sanPhamRepository.GetAllAsync();

                var queryResults = sanPhams
                    .Where(s => s.TrangThai == true)
                    .Select(s => new
                    {
                        s.IdSanPham,
                        s.TenSanPham,
                        s.GiaBan,
                        DanhMuc = s.IdDanhMucNavigation != null ? s.IdDanhMucNavigation.TenDanhMuc : "",
                        AnhChinh = s.AnhSanPhams
                            .Where(a => a.LoaiAnh == "Chinh" || a.LoaiAnh == "Primary" || a.LoaiAnh == "Main" || a.LoaiAnh == "Chính")
                            .Select(a => a.DuongDan)
                            .FirstOrDefault() ??
                            s.AnhSanPhams.Select(a => a.DuongDan).FirstOrDefault(),
                        NormalizedName = s.TenSanPham != null ? s.TenSanPham.ToLower() : ""
                    })
                    .ToList();

                var suggestions = queryResults
                    .Where(s => VietnameseTextHelper.RemoveDiacritics(s.NormalizedName).Contains(normalizedTerm))
                    .Take(8)
                    .Select(s => new
                    {
                        name = s.TenSanPham,
                        price = s.GiaBan,
                        category = s.DanhMuc,
                        image = s.AnhChinh,
                        url = Url.Action("Index", "Chitiet", new { area = "KhachHang", id = s.IdSanPham })
                    })
                    .ToList();

                return Json(suggestions);
            }
            catch
            {
                return Json(new List<object>());
            }
        }
    }
}