using Microsoft.AspNetCore.Mvc;
using Final_VS1.Models;
using Final_VS1.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Final_VS1.Repositories;

namespace Final_VS1.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class DanhmucController : Controller
    {
        private readonly IDanhMucRepository _danhMucRepository;

        public DanhmucController(IDanhMucRepository danhMucRepository)
        {
            _danhMucRepository = danhMucRepository;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _danhMucRepository.GetAllAsync();
            return View(categories);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCategoryRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.TenDanhMuc))
                {
                    return Json(new { success = false, message = "Tên danh mục là bắt buộc" });
                }

                if (request.TenDanhMuc.Length > 100)
                {
                    return Json(new { success = false, message = "Tên danh mục không được vượt quá 100 ký tự" });
                }

                // Kiểm tra trùng tên danh mục qua repository
                if (await _danhMucRepository.IsNameExistsAsync(request.TenDanhMuc))
                {
                    return Json(new { success = false, message = "Tên danh mục đã tồn tại. Vui lòng chọn tên khác." });
                }

                var category = new Final_VS1.Data.DanhMuc
                {
                    TenDanhMuc = request.TenDanhMuc.Trim(),
                    MoTa = string.IsNullOrEmpty(request.MoTa) ? null : request.MoTa.Trim(),
                    ThuTuHienThi = await _danhMucRepository.GetNextDisplayOrderAsync()
                };

                await _danhMucRepository.AddAsync(category);

                return Json(new { success = true, message = "Thêm danh mục thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Có lỗi xảy ra: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Update([FromBody] UpdateCategoryRequest request)
        {
            try
            {
                var category = await _danhMucRepository.GetByIdAsync(request.Id);
                if (category == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy danh mục" });
                }

                if (string.IsNullOrEmpty(request.TenDanhMuc))
                {
                    return Json(new { success = false, message = "Tên danh mục là bắt buộc" });
                }

                // Kiểm tra trùng tên danh mục (ngoại trừ chính nó) qua repository
                if (await _danhMucRepository.IsNameExistsAsync(request.TenDanhMuc, request.Id))
                {
                    return Json(new { success = false, message = "Tên danh mục đã tồn tại. Vui lòng chọn tên khác." });
                }

                category.TenDanhMuc = request.TenDanhMuc.Trim();

                if (request.MoTa != null)
                {
                    category.MoTa = string.IsNullOrEmpty(request.MoTa) ? null : request.MoTa.Trim();
                }

                await _danhMucRepository.UpdateAsync(category);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra khi cập nhật" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var category = await _danhMucRepository.GetByIdAsync(id);
                if (category == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy danh mục" });
                }

                if (category.SanPhams.Any())
                {
                    return Json(new { success = false, message = "Không thể xóa danh mục có chứa sản phẩm. Vui lòng di chuyển hoặc xóa tất cả sản phẩm trong danh mục trước." });
                }

                await _danhMucRepository.DeleteAsync(id);

                return Json(new { success = true, message = "Xóa danh mục thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra khi xóa danh mục" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCategoryProducts(int id)
        {
            var category = await _danhMucRepository.GetByIdAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            var products = category.SanPhams.Select(p => new
            {
                Id = p.IdSanPham,
                Name = p.TenSanPham,
                Price = p.GiaBan?.ToString("N0") + " ₫",
                Status = p.TrangThai == true ? "Hiển thị" : "Ẩn",
                MainImage = p.AnhSanPhams.FirstOrDefault(a => a.LoaiAnh == "Chinh")?.DuongDan ??
                           p.AnhSanPhams.FirstOrDefault()?.DuongDan ??
                           "/Images/noimage.jpg"
            }).ToList();

            return Json(products);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateOrder([FromBody] List<int> categoryIds)
        {
            try
            {
                for (int i = 0; i < categoryIds.Count; i++)
                {
                    var category = await _danhMucRepository.GetByIdAsync(categoryIds[i]);
                    if (category != null)
                    {
                        category.ThuTuHienThi = i + 1;
                        await _danhMucRepository.UpdateAsync(category);
                    }
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra khi cập nhật thứ tự" });
            }
        }

        public class CreateCategoryRequest
        {
            public string TenDanhMuc { get; set; } = null!;
            public string? MoTa { get; set; }
        }

        public class UpdateCategoryRequest
        {
            public int Id { get; set; }
            public string TenDanhMuc { get; set; } = null!;
            public string? MoTa { get; set; }
        }
    }
}