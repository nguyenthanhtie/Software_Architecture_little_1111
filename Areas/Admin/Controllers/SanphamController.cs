using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Final_VS1.Models;
using Final_VS1.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.IO;
using System.Threading.Tasks;
using Final_VS1.Data;
using Microsoft.AspNetCore.Authorization;
using Final_VS1.Repositories; // Thêm namespace repository


namespace Final_VS1.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class SanphamController : Controller
    {
        private readonly ISanPhamRepository _sanPhamRepository;
        private readonly LittleFishBeautyContext _context;

        public SanphamController(ISanPhamRepository sanPhamRepository, LittleFishBeautyContext context)
        {
            _sanPhamRepository = sanPhamRepository;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var sanPhams = await _sanPhamRepository.GetAllAsync();
            var danhMucs = await _context.DanhMucs
                .OrderBy(d => d.ThuTuHienThi)
                .ToListAsync();

            ViewBag.DanhMucs = danhMucs;

            return View(sanPhams);
        }

        public async Task<IActionResult> Detail(int id)
        {
            var sanPham = await _sanPhamRepository.GetByIdAsync(id);

            if (sanPham == null)
                return NotFound();

            return PartialView("_ChiTietSanPhamPartial", sanPham);
        }

        [HttpGet]
        public async Task<IActionResult> AddOrEdit(int? id)
        {
            SanPham model;
            if (id == null)
            {
                model = new SanPham();
            }
            else
            {
                model = await _sanPhamRepository.GetByIdAsync(id.Value);
                if (model == null)
                    return NotFound();
            }
            ViewBag.DanhMucs = await _context.DanhMucs.OrderBy(d => d.ThuTuHienThi).ToListAsync();
            return PartialView("_ThemSuaSanPhamPartial", model);
        }

        // Thêm helper method để lấy các giá trị hợp lệ cho LoaiAnh
        private string[] GetMainImageTypes()
        {
            return new[] { "Chinh", "Primary", "Main", "Chính" };
        }

        private string[] GetOtherImageTypes()
        {
            return new[] { "Phu", "Secondary", "Other", "Phụ" };
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(int? id, [Bind("IdSanPham,TenSanPham,MoTa,GiaBan,SoLuongTonKho,Sku,TrangThai,IdDanhMuc,CachSuDung")] SanPham sanPham,
            List<IFormFile> ImageFiles, string? mainImage, List<string> RemovedImageIds)
        {
            if (sanPham.GiaBan < 0)
            {
                sanPham.GiaBan = 0;
            }

            if (ModelState.IsValid)
            {
                bool isNewProduct = id == null || id == 0;

                // Kiểm tra SKU trùng lặp qua repository
                if (!string.IsNullOrEmpty(sanPham.Sku))
                {
                    var existingSku = await _sanPhamRepository.IsSkuExistsAsync(sanPham.Sku, sanPham.IdSanPham);
                    if (existingSku)
                    {
                        return Json(new { success = false, message = $"SKU '{sanPham.Sku}' đã tồn tại. Vui lòng sử dụng SKU khác." });
                    }
                }

                try
                {
                    if (isNewProduct)
                    {
                        sanPham.IdSanPham = 0;
                        sanPham.NgayTao = DateTime.Now;
                        await _sanPhamRepository.AddAsync(sanPham);
                    }
                    else
                    {
                        var existingProduct = await _sanPhamRepository.GetByIdAsync(id.Value);
                        if (existingProduct == null)
                        {
                            return Json(new { success = false, message = "Sản phẩm không tồn tại" });
                        }

                        existingProduct.TenSanPham = sanPham.TenSanPham;
                        existingProduct.MoTa = sanPham.MoTa;
                        existingProduct.GiaBan = sanPham.GiaBan;
                        existingProduct.SoLuongTonKho = sanPham.SoLuongTonKho;
                        existingProduct.Sku = sanPham.Sku;
                        existingProduct.TrangThai = sanPham.TrangThai;
                        existingProduct.IdDanhMuc = sanPham.IdDanhMuc;
                        existingProduct.CachSuDung = sanPham.CachSuDung;

                        sanPham = existingProduct;
                        await _sanPhamRepository.UpdateAsync(sanPham);
                    }

                    // Xử lý xóa ảnh cũ nếu có
                    if (RemovedImageIds != null && RemovedImageIds.Any())
                    {
                        foreach (var removedIdStr in RemovedImageIds)
                        {
                            if (int.TryParse(removedIdStr, out int removedId))
                            {
                                var imageToRemove = await _context.AnhSanPhams.FindAsync(removedId);
                                if (imageToRemove != null && imageToRemove.IdSanPham == sanPham.IdSanPham)
                                {
                                    if (!string.IsNullOrEmpty(imageToRemove.DuongDan) && imageToRemove.DuongDan.StartsWith("/images/products/"))
                                    {
                                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", imageToRemove.DuongDan.TrimStart('/'));
                                        if (System.IO.File.Exists(filePath))
                                        {
                                            System.IO.File.Delete(filePath);
                                        }
                                    }
                                    _context.AnhSanPhams.Remove(imageToRemove);
                                }
                            }
                        }
                        await _context.SaveChangesAsync();
                    }

                    // Xử lý lưu ảnh mới
                    List<string> newImageUrls = new List<string>();

                    if (ImageFiles != null && ImageFiles.Count > 0)
                    {
                        string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "products");
                        if (!Directory.Exists(uploadsFolder))
                            Directory.CreateDirectory(uploadsFolder);

                        foreach (var file in ImageFiles)
                        {
                            if (file != null && file.Length > 0)
                            {
                                var allowedTypes = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                                var fileExtension = Path.GetExtension(file.FileName).ToLower();
                                if (!allowedTypes.Contains(fileExtension))
                                {
                                    return Json(new { success = false, message = $"File {file.FileName} không được hỗ trợ. Chỉ cho phép: jpg, jpeg, png, gif, webp" });
                                }

                                if (file.Length > 5 * 1024 * 1024)
                                {
                                    return Json(new { success = false, message = $"File {file.FileName} quá lớn. Kích thước tối đa: 5MB" });
                                }

                                string uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
                                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                                using (var fileStream = new FileStream(filePath, FileMode.Create))
                                {
                                    await file.CopyToAsync(fileStream);
                                }

                                newImageUrls.Add($"/images/products/{uniqueFileName}");
                            }
                        }
                    }

                    // Lưu ảnh mới vào database
                    if (newImageUrls.Any())
                    {
                        var (mainType, otherType) = await GetValidImageTypes();

                        var hasMainImage = await _context.AnhSanPhams
                            .AnyAsync(a => a.IdSanPham == sanPham.IdSanPham && a.LoaiAnh == mainType);

                        string? primaryImageUrl = !string.IsNullOrEmpty(mainImage) && newImageUrls.Contains(mainImage)
                            ? mainImage
                            : (!hasMainImage ? newImageUrls.First() : null);

                        foreach (var url in newImageUrls)
                        {
                            var anhSanPham = new AnhSanPham
                            {
                                IdSanPham = sanPham.IdSanPham,
                                DuongDan = url,
                                LoaiAnh = (url == primaryImageUrl) ? mainType : otherType
                            };

                            _context.AnhSanPhams.Add(anhSanPham);
                        }

                        await _context.SaveChangesAsync();
                    }

                    // Cập nhật ảnh chính nếu có thay đổi
                    if (!string.IsNullOrEmpty(mainImage))
                    {
                        var allImages = await _context.AnhSanPhams
                            .Where(a => a.IdSanPham == sanPham.IdSanPham)
                            .ToListAsync();

                        var (mainType, otherType) = await GetValidImageTypes();

                        foreach (var img in allImages)
                        {
                            img.LoaiAnh = img.DuongDan == mainImage ? mainType : otherType;
                        }

                        await _context.SaveChangesAsync();
                    }

                    return Json(new { success = true, message = isNewProduct ? "Thêm sản phẩm thành công!" : "Cập nhật sản phẩm thành công!" });
                }
                catch (DbUpdateException ex)
                {
                    if (ex.InnerException?.Message?.Contains("UNIQUE KEY constraint") == true)
                    {
                        if (ex.InnerException.Message.Contains("UQ__SanPham__CA1ECF0D"))
                        {
                            return Json(new { success = false, message = "SKU đã tồn tại. Vui lòng sử dụng SKU khác hoặc để trống." });
                        }
                        return Json(new { success = false, message = "Dữ liệu bị trùng lặp. Vui lòng kiểm tra lại thông tin." });
                    }
                    return Json(new { success = false, message = "Lỗi database: " + ex.InnerException?.Message ?? ex.Message });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
                }
            }

            ViewBag.DanhMucs = await _context.DanhMucs.OrderBy(d => d.ThuTuHienThi).ToListAsync();

            var validationErrors = ModelState
                .Where(x => x.Value?.Errors?.Count > 0)
                .Select(x => new { Field = x.Key, Errors = x.Value?.Errors?.Select(e => e.ErrorMessage) ?? new List<string>() })
                .ToList();

            var errorMessage = "Dữ liệu không hợp lệ: " + string.Join("; ", validationErrors.SelectMany(x => x.Errors));

            var html = await RenderViewToStringAsync("_ThemSuaSanPhamPartial", sanPham, true);
            return Json(new { success = false, html, message = errorMessage, validationErrors });
        }

        [HttpGet]
        public async Task<IActionResult> CheckLoaiAnhConstraint()
        {
            try
            {
                var existingTypes = await _context.AnhSanPhams
                    .Where(a => a.LoaiAnh != null)
                    .Select(a => a.LoaiAnh)
                    .Distinct()
                    .ToListAsync();

                var testValues = new List<string> { "Chinh", "Phu", "Primary", "Secondary", "Main", "Other" };
                var validValues = new List<string>();

                foreach (var value in testValues)
                {
                    try
                    {
                        var testEntity = new AnhSanPham
                        {
                            IdSanPham = 1,
                            DuongDan = "/test-path.jpg",
                            LoaiAnh = value
                        };

                        _context.AnhSanPhams.Add(testEntity);
                        await _context.SaveChangesAsync();

                        validValues.Add(value);

                        _context.AnhSanPhams.Remove(testEntity);
                        await _context.SaveChangesAsync();
                    }
                    catch
                    {
                        // Giá trị không hợp lệ, bỏ qua
                    }
                }

                return Json(new
                {
                    success = true,
                    existingTypes = existingTypes,
                    validTestValues = validValues
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        private async Task<(string MainType, string OtherType)> GetValidImageTypes()
        {
            var defaultPairs = new List<(string Main, string Other)>
            {
                ("Chinh", "Phu"),
                ("Primary", "Secondary"),
                ("Main", "Other")
            };

            try
            {
                var existingTypes = await _context.AnhSanPhams
                    .Where(a => a.LoaiAnh != null)
                    .Select(a => a.LoaiAnh)
                    .Distinct()
                    .ToListAsync();

                if (existingTypes.Any())
                {
                    foreach (var pair in defaultPairs)
                    {
                        if (existingTypes.Contains(pair.Main))
                        {
                            string otherType = existingTypes.Contains(pair.Other)
                                ? pair.Other
                                : (existingTypes.Count > 1 ? existingTypes.First(t => t != pair.Main) ?? pair.Other : pair.Other);

                            return (pair.Main, otherType);
                        }
                    }

                    if (existingTypes.Count >= 2)
                    {
                        var first = existingTypes[0] ?? defaultPairs[0].Main;
                        var second = existingTypes[1] ?? defaultPairs[0].Other;
                        return (first, second);
                    }

                    if (existingTypes.Count > 0)
                    {
                        var firstType = existingTypes[0] ?? defaultPairs[0].Main;
                        return (firstType, firstType);
                    }
                }
            }
            catch
            {
                // Bỏ qua lỗi và sử dụng giá trị mặc định
            }

            return defaultPairs[0];
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _sanPhamRepository.DeleteAsync(id);
                return Json(new { success = true, message = "Xóa sản phẩm thành công!" });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra khi xóa sản phẩm" });
            }
        }

        private async Task<string> RenderViewToStringAsync<TModel>(string viewName, TModel model, bool partial = false)
        {
            if (string.IsNullOrEmpty(viewName))
            {
                viewName = ControllerContext.ActionDescriptor.ActionName;
            }

            ViewData.Model = model;

            using (var writer = new StringWriter())
            {
                var viewEngine = HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine;
                var viewResult = viewEngine!.FindView(ControllerContext, viewName, !partial);

                if (!viewResult.Success)
                {
                    throw new InvalidOperationException($"Không tìm thấy view {viewName}");
                }

                var viewContext = new ViewContext(
                    ControllerContext,
                    viewResult.View,
                    ViewData,
                    TempData,
                    writer,
                    new HtmlHelperOptions()
                );

                await viewResult.View.RenderAsync(viewContext);
                return writer.GetStringBuilder().ToString();
            }
        }
    }
}