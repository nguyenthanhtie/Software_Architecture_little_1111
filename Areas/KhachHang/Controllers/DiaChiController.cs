using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Final_VS1.Data;
using Final_VS1.Areas.KhachHang.ViewModels;
using Final_VS1.Services;
using System.Security.Claims;

namespace Final_VS1.Areas.KhachHang.Controllers
{
    [Area("KhachHang")]
    [Authorize]
    public class DiaChiController : Controller
    {
        private readonly IDiaChiService _diaChiService;
        private readonly ITaiKhoanService _taiKhoanService;

        public DiaChiController(IDiaChiService diaChiService, ITaiKhoanService taiKhoanService)
        {
            _diaChiService = diaChiService;
            _taiKhoanService = taiKhoanService;
        }

        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out int userId))
            {
                return userId;
            }
            return null;
        }

        // GET: /KhachHang/DiaChi
        public async Task<IActionResult> Index()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return RedirectToAction("Index", "DangNhap", new { area = "" });
                }

                // Lấy thông tin tài khoản
                var taiKhoan = await _taiKhoanService.GetByIdAsync(userId.Value);
                if (taiKhoan == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy thông tin tài khoản.";
                    return RedirectToAction("Index", "DangNhap", new { area = "" });
                }

                // Lấy danh sách địa chỉ của user
                var diaChiList = await _diaChiService.GetAddressesByUserAsync(userId.Value);

                var viewModel = new DiaChiManagementViewModel
                {
                    DiaChiList = diaChiList,
                    TaiKhoan = taiKhoan
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải danh sách địa chỉ: " + ex.Message;
                return View(new DiaChiManagementViewModel { DiaChiList = new List<DiaChi>() });
            }
        }

        // GET: /KhachHang/DiaChi/Create
        public IActionResult Create()
        {
            return View(new DiaChiCreateViewModel());
        }

        // POST: /KhachHang/DiaChi/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DiaChiCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return RedirectToAction("Index", "DangNhap", new { area = "" });
                }

                var diaChi = new DiaChi
                {
                    IdTaiKhoan = userId.Value,
                    HoTenNguoiNhan = model.HoTen.Trim(),
                    SoDienThoai = model.SoDienThoai.Trim(),
                    DiaChi1 = model.DiaChi1.Trim(),
                    MacDinh = model.LaMacDinh,
                    NgayTao = DateTime.Now
                };

                // Nếu đặt làm mặc định, clear mặc định cũ
                if (model.LaMacDinh)
                {
                    await _diaChiService.ClearDefaultAddressAsync(userId.Value);
                }

                await _diaChiService.CreateAddressAsync(diaChi);
                TempData["SuccessMessage"] = "Thêm địa chỉ thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra: " + ex.Message);
                return View(model);
            }
        }

        // POST: /KhachHang/DiaChi/CreateAjax
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAjax(DiaChiCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value != null && x.Value.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                    );
                return Json(new { success = false, errors = errors });
            }

            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Json(new { success = false, message = "Phiên đăng nhập đã hết hạn." });
                }

                var diaChi = new DiaChi
                {
                    IdTaiKhoan = userId.Value,
                    HoTenNguoiNhan = model.HoTen.Trim(),
                    SoDienThoai = model.SoDienThoai.Trim(),
                    DiaChi1 = model.DiaChi1.Trim(),
                    MacDinh = model.LaMacDinh,
                    NgayTao = DateTime.Now
                };

                // Nếu đặt làm mặc định, clear mặc định cũ
                if (model.LaMacDinh)
                {
                    await _diaChiService.ClearDefaultAddressAsync(userId.Value);
                }

                await _diaChiService.CreateAddressAsync(diaChi);
                return Json(new { success = true, message = "Thêm địa chỉ thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        // GET: /KhachHang/DiaChi/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return RedirectToAction("Index", "DangNhap", new { area = "" });
                }

                var diaChi = await _diaChiService.GetAddressByIdAsync(id);
                if (diaChi == null || diaChi.IdTaiKhoan != userId.Value)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy địa chỉ hoặc bạn không có quyền chỉnh sửa.";
                    return RedirectToAction(nameof(Index));
                }

                var viewModel = new DiaChiEditViewModel
                {
                    IdDiaChi = diaChi.IdDiaChi,
                    HoTen = diaChi.HoTenNguoiNhan ?? "",
                    SoDienThoai = diaChi.SoDienThoai ?? "",
                    DiaChi1 = diaChi.DiaChi1 ?? "",
                    LaMacDinh = diaChi.MacDinh ?? false
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: /KhachHang/DiaChi/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DiaChiEditViewModel model)
        {
            if (id != model.IdDiaChi)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return RedirectToAction("Index", "DangNhap", new { area = "" });
                }

                var existingDiaChi = await _diaChiService.GetAddressByIdAsync(id);
                if (existingDiaChi == null || existingDiaChi.IdTaiKhoan != userId.Value)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy địa chỉ hoặc bạn không có quyền chỉnh sửa.";
                    return RedirectToAction(nameof(Index));
                }

                // Update properties
                existingDiaChi.HoTenNguoiNhan = model.HoTen.Trim();
                existingDiaChi.SoDienThoai = model.SoDienThoai.Trim();
                existingDiaChi.DiaChi1 = model.DiaChi1.Trim();

                // Handle default setting
                if (model.LaMacDinh && !(existingDiaChi.MacDinh ?? false))
                {
                    await _diaChiService.ClearDefaultAddressAsync(userId.Value);
                    existingDiaChi.MacDinh = true;
                }
                else if (!model.LaMacDinh && (existingDiaChi.MacDinh ?? false))
                {
                    existingDiaChi.MacDinh = false;
                }

                await _diaChiService.UpdateAddressAsync(existingDiaChi);
                TempData["SuccessMessage"] = "Cập nhật địa chỉ thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra: " + ex.Message);
                return View(model);
            }
        }

        // POST: /KhachHang/DiaChi/EditAjax
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAjax(DiaChiEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value != null && x.Value.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                    );
                return Json(new { success = false, errors = errors });
            }

            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Json(new { success = false, message = "Phiên đăng nhập đã hết hạn." });
                }

                var existingDiaChi = await _diaChiService.GetAddressByIdAsync(model.IdDiaChi);
                if (existingDiaChi == null || existingDiaChi.IdTaiKhoan != userId.Value)
                {
                    return Json(new { success = false, message = "Không tìm thấy địa chỉ hoặc bạn không có quyền chỉnh sửa." });
                }

                // Update properties
                existingDiaChi.HoTenNguoiNhan = model.HoTen.Trim();
                existingDiaChi.SoDienThoai = model.SoDienThoai.Trim();
                existingDiaChi.DiaChi1 = model.DiaChi1.Trim();

                // Handle default setting
                if (model.LaMacDinh && !(existingDiaChi.MacDinh ?? false))
                {
                    await _diaChiService.ClearDefaultAddressAsync(userId.Value);
                    existingDiaChi.MacDinh = true;
                }
                else if (!model.LaMacDinh && (existingDiaChi.MacDinh ?? false))
                {
                    existingDiaChi.MacDinh = false;
                }

                await _diaChiService.UpdateAddressAsync(existingDiaChi);
                return Json(new { success = true, message = "Cập nhật địa chỉ thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        // POST: /KhachHang/DiaChi/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Json(new { success = false, message = "Phiên đăng nhập đã hết hạn." });
                }

                var diaChi = await _diaChiService.GetAddressByIdAsync(id);
                if (diaChi == null || diaChi.IdTaiKhoan != userId.Value)
                {
                    return Json(new { success = false, message = "Không tìm thấy địa chỉ hoặc bạn không có quyền xóa." });
                }

                // Kiểm tra xem có phải địa chỉ mặc định không
                if (diaChi.MacDinh ?? false)
                {
                    // Kiểm tra xem có địa chỉ khác không
                    var userAddresses = await _diaChiService.GetAddressesByUserAsync(userId.Value);
                    if (userAddresses.Count > 1)
                    {
                        return Json(new { success = false, message = "Không thể xóa địa chỉ mặc định. Hãy đặt địa chỉ khác làm mặc định trước." });
                    }
                }

                var success = await _diaChiService.DeleteAddressAsync(id);
                if (success)
                {
                    return Json(new { success = true, message = "Xóa địa chỉ thành công!" });
                }
                else
                {
                    return Json(new { success = false, message = "Không thể xóa địa chỉ." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        // POST: /KhachHang/DiaChi/SetDefault
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetDefault([FromForm] int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Json(new { success = false, message = "Phiên đăng nhập đã hết hạn." });
                }

                var diaChi = await _diaChiService.GetAddressByIdAsync(id);
                if (diaChi == null || diaChi.IdTaiKhoan != userId.Value)
                {
                    return Json(new { success = false, message = "Không tìm thấy địa chỉ hoặc bạn không có quyền thay đổi." });
                }

                var success = await _diaChiService.SetDefaultAddressAsync(id, userId.Value);
                if (success)
                {
                    return Json(new { success = true, message = "Đã đặt làm địa chỉ mặc định!" });
                }
                else
                {
                    return Json(new { success = false, message = "Không thể đặt làm địa chỉ mặc định." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        // GET: /KhachHang/DiaChi/GetAddressData/5 - AJAX endpoint
        [HttpGet]
        public async Task<IActionResult> GetAddressData(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Json(new { success = false, message = "Phiên đăng nhập đã hết hạn." });
                }

                var diaChi = await _diaChiService.GetAddressByIdAsync(id);
                if (diaChi == null || diaChi.IdTaiKhoan != userId.Value)
                {
                    return Json(new { success = false, message = "Không tìm thấy địa chỉ hoặc bạn không có quyền truy cập." });
                }

                return Json(new { 
                    success = true, 
                    data = new {
                        IdDiaChi = diaChi.IdDiaChi,
                        HoTen = diaChi.HoTenNguoiNhan ?? "",
                        SoDienThoai = diaChi.SoDienThoai ?? "",
                        DiaChi1 = diaChi.DiaChi1 ?? "",
                        LaMacDinh = diaChi.MacDinh ?? false
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        // GET: /KhachHang/DiaChi/Test - Endpoint for testing
        [AllowAnonymous]
        public IActionResult Test()
        {
            return View();
        }
    }
}
