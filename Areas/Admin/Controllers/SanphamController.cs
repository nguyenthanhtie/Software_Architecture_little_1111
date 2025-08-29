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

namespace Final_VS1.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class SanphamController : Controller
    {
        private readonly LittleFishBeautyContext _context;

        public SanphamController(LittleFishBeautyContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var sanPhams = await _context.SanPhams
                .Include(s => s.IdDanhMucNavigation)
                .Include(s => s.AnhSanPhams)
                .OrderByDescending(s => s.NgayTao)
                .ToListAsync();

            var danhMucs = await _context.DanhMucs
                .OrderBy(d => d.ThuTuHienThi)
                .ToListAsync();

            ViewBag.DanhMucs = danhMucs;

            return View(sanPhams);
        }

        public async Task<IActionResult> Detail(int id)
        {
            var sanPham = await _context.SanPhams
                .Include(s => s.IdDanhMucNavigation)
                .Include(s => s.AnhSanPhams)
                .FirstOrDefaultAsync(s => s.IdSanPham == id);

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
                model = await _context.SanPhams
                    .Include(s => s.AnhSanPhams)
                    .FirstOrDefaultAsync(s => s.IdSanPham == id.Value);
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
            // Remove any price validation constraints
            if (sanPham.GiaBan < 0)
            {
                sanPham.GiaBan = 0; // Set negative prices to 0 if needed, or remove this check to allow negative prices
            }

            if (ModelState.IsValid)
            {
                bool isNewProduct = id == null || id == 0;

                // Kiểm tra SKU trùng lặp
                Console.WriteLine($"DEBUG: Checking SKU '{sanPham.Sku}' for product ID {sanPham.IdSanPham}");
                if (!string.IsNullOrEmpty(sanPham.Sku))
                {
                    var existingSku = await _context.SanPhams
                        .Where(s => s.Sku == sanPham.Sku && s.IdSanPham != sanPham.IdSanPham)
                        .AnyAsync();
                    
                    Console.WriteLine($"DEBUG: Existing SKU found: {existingSku}");
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
                        _context.Add(sanPham);
                        await _context.SaveChangesAsync(); // Save first to get ID
                    }
                    else
                    {
                        var existingProduct = await _context.SanPhams.FindAsync(id);
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
                        
                        sanPham = existingProduct; // Use existing product for image processing
                        await _context.SaveChangesAsync();
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
                                    // Xóa file khỏi server nếu là ảnh upload
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

                    // Xử lý file upload
                    if (ImageFiles != null && ImageFiles.Count > 0)
                    {
                        string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "products");
                        if (!Directory.Exists(uploadsFolder))
                            Directory.CreateDirectory(uploadsFolder);

                        foreach (var file in ImageFiles)
                        {
                            if (file != null && file.Length > 0)
                            {
                                // Validate file type
                                var allowedTypes = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                                var fileExtension = Path.GetExtension(file.FileName).ToLower();
                                if (!allowedTypes.Contains(fileExtension))
                                {
                                    return Json(new { success = false, message = $"File {file.FileName} không được hỗ trợ. Chỉ cho phép: jpg, jpeg, png, gif, webp" });
                                }

                                // Validate file size (5MB max)
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

                        // Kiểm tra xem đã có ảnh chính chưa
                        var hasMainImage = await _context.AnhSanPhams
                            .AnyAsync(a => a.IdSanPham == sanPham.IdSanPham && a.LoaiAnh == mainType);

                        // Xác định ảnh chính
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
                    // Xử lý lỗi database constraint
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
            
            // Lấy chi tiết lỗi validation
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
                // Lấy danh sách giá trị LoaiAnh đã tồn tại
                var existingTypes = await _context.AnhSanPhams
                    .Where(a => a.LoaiAnh != null)
                    .Select(a => a.LoaiAnh)
                    .Distinct()
                    .ToListAsync();

                // Kiểm tra một số giá trị phổ biến
                var testValues = new List<string> { "Chinh", "Phu", "Primary", "Secondary", "Main", "Other" };
                var validValues = new List<string>();

                foreach (var value in testValues)
                {
                    try
                    {
                        var testEntity = new AnhSanPham
                        {
                            IdSanPham = 1, // Sử dụng ID sản phẩm đã tồn tại để test
                            DuongDan = "/test-path.jpg",
                            LoaiAnh = value
                        };

                        _context.AnhSanPhams.Add(testEntity);
                        await _context.SaveChangesAsync();

                        // Nếu lưu thành công, thêm vào danh sách giá trị hợp lệ
                        validValues.Add(value);

                        // Xóa dữ liệu test
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

        // Thêm helper method để chuẩn hóa giá trị LoaiAnh
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
                // Kiểm tra database xem có giá trị nào đã được sử dụng chưa
                var existingTypes = await _context.AnhSanPhams
                    .Where(a => a.LoaiAnh != null)
                    .Select(a => a.LoaiAnh)
                    .Distinct()
                    .ToListAsync();

                if (existingTypes.Any())
                {
                    // Tìm cặp giá trị đã tồn tại trong database
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

                    // Sử dụng giá trị có sẵn nếu không khớp với pattern
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

            // Mặc định trả về cặp giá trị đầu tiên
            return defaultPairs[0];
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var sanPham = await _context.SanPhams
                    .Include(s => s.AnhSanPhams)
                    .Include(s => s.ChiTietDonHangs)
                    .FirstOrDefaultAsync(s => s.IdSanPham == id);

                if (sanPham == null)
                {
                    return Json(new { success = false, message = "Sản phẩm không tồn tại" });
                }

                // Check if product is used in orders
                if (sanPham.ChiTietDonHangs.Any())
                {
                    return Json(new { success = false, message = "Sản phẩm đã có trong đơn hàng, không thể xóa" });
                }

                // Xóa các ảnh sản phẩm trước
                if (sanPham.AnhSanPhams.Any())
                {
                    // Xóa file ảnh khỏi server (chỉ xóa ảnh upload, không xóa URL)
                    foreach (var anh in sanPham.AnhSanPhams)
                    {
                        if (!string.IsNullOrEmpty(anh.DuongDan) && anh.DuongDan.StartsWith("/images/products/"))
                        {
                            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", anh.DuongDan.TrimStart('/'));
                            if (System.IO.File.Exists(filePath))
                            {
                                System.IO.File.Delete(filePath);
                            }
                        }
                    }

                    _context.AnhSanPhams.RemoveRange(sanPham.AnhSanPhams);
                }

                // Xóa sản phẩm
                _context.SanPhams.Remove(sanPham);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Xóa sản phẩm thành công!" });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra khi xóa sản phẩm" });
            }
        }

        // Helper method để render view thành string
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