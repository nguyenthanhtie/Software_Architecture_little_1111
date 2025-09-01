using Microsoft.AspNetCore.Mvc;
using Final_VS1.Models;
using Final_VS1.Data;
using Final_VS1.Services;
using System;
using System.Threading.Tasks;

namespace Final_VS1.Controllers
{
    public class DangKiServiceExampleController : Controller
    {
        private readonly IServiceFactory _serviceFactory;

        public DangKiServiceExampleController(IServiceFactory serviceFactory)
        {
            _serviceFactory = serviceFactory;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region Đăng Kí tài Khoản với Service Layer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(DangKiViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            try
            {
                // Sử dụng service factory để tạo services
                var taiKhoanService = _serviceFactory.CreateTaiKhoanService();
                var emailService = _serviceFactory.CreateEmailService();

                // Sử dụng service để kiểm tra email đã tồn tại
                if (await taiKhoanService.IsEmailExistsAsync(model.Email))
                {
                    ModelState.AddModelError("Email", "Email này đã được sử dụng");
                    return View("Index", model);
                }

                // Tạo tài khoản mới thông qua service
                var taiKhoan = new TaiKhoan
                {
                    Email = model.Email,
                    MatKhau = BCrypt.Net.BCrypt.HashPassword(model.MatKhau),
                    HoTen = model.HoTen,
                    VaiTro = "Khach"
                };

                var createdAccount = await taiKhoanService.CreateAsync(taiKhoan);

                // Tạo token xác nhận
                var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                var confirmationLink = Url.Action("XacNhanEmail", "DangKi",
                    new { email = model.Email, token = token }, Request.Scheme);

                // Lưu token vào session
                var sessionKey = $"EmailConfirmation_{model.Email}";
                HttpContext.Session.SetString(sessionKey, token);
                HttpContext.Session.SetString($"{sessionKey}_CreatedAt", DateTime.Now.ToString());

                // Gửi email xác nhận thông qua EmailService
                var emailSent = await emailService.SendConfirmationEmailAsync(model.Email, confirmationLink);
                
                if (emailSent)
                {
                    TempData["SuccessMessage"] = "Đăng ký thành công! Vui lòng kiểm tra email để kích hoạt tài khoản.";
                }
                else
                {
                    TempData["WarningMessage"] = "Đăng ký thành công nhưng không thể gửi email xác nhận. Vui lòng liên hệ admin.";
                }

                return RedirectToAction("Index");
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View("Index", model);
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View("Index", model);
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra trong quá trình đăng ký. Vui lòng thử lại.");
                return View("Index", model);
            }
        }
        #endregion

        #region Kiểm tra Email với Service
        [HttpPost]
        public async Task<IActionResult> CheckEmail(string email)
        {
            try
            {
                var taiKhoanService = _serviceFactory.CreateTaiKhoanService();
                var exists = await taiKhoanService.IsEmailExistsAsync(email);
                return Json(!exists); // Trả về true nếu email chưa tồn tại
            }
            catch
            {
                return Json(false);
            }
        }

        // Action xác nhận email với Service
        [HttpGet]
        public async Task<IActionResult> XacNhanEmail(string email, string token)
        {
            try
            {
                var sessionKey = $"EmailConfirmation_{email}";
                var storedToken = HttpContext.Session.GetString(sessionKey);
                var createdAtString = HttpContext.Session.GetString($"{sessionKey}_CreatedAt");

                // Kiểm tra token và thời gian hết hạn
                if (string.IsNullOrEmpty(storedToken) || storedToken != token)
                {
                    TempData["ErrorMessage"] = "Xác nhận thất bại. Liên kết không hợp lệ hoặc đã hết hạn.";
                    return RedirectToAction("Index");
                }

                // Kiểm tra thời gian hết hạn (24 giờ)
                if (!string.IsNullOrEmpty(createdAtString) && DateTime.TryParse(createdAtString, out var createdAt))
                {
                    if (DateTime.Now.Subtract(createdAt).TotalHours > 24)
                    {
                        HttpContext.Session.Remove(sessionKey);
                        HttpContext.Session.Remove($"{sessionKey}_CreatedAt");
                        TempData["ErrorMessage"] = "Liên kết xác nhận đã hết hạn.";
                        return RedirectToAction("Index");
                    }
                }

                // Kích hoạt tài khoản thông qua service
                var taiKhoanService = _serviceFactory.CreateTaiKhoanService();
                var emailService = _serviceFactory.CreateEmailService();
                var activated = await taiKhoanService.ActivateAccountAsync(email);

                if (activated)
                {
                    // Xóa token khỏi session
                    HttpContext.Session.Remove(sessionKey);
                    HttpContext.Session.Remove($"{sessionKey}_CreatedAt");

                    // Gửi email chào mừng
                    var user = await taiKhoanService.GetByEmailAsync(email);
                    if (user != null)
                    {
                        await emailService.SendWelcomeEmailAsync(user.Email, user.HoTen ?? "Khách hàng");
                    }

                    TempData["SuccessMessage"] = "Xác nhận email thành công! Tài khoản của bạn đã được kích hoạt.";
                    return RedirectToAction("Index", "DangNhap");
                }
                else
                {
                    TempData["ErrorMessage"] = "Không thể kích hoạt tài khoản. Vui lòng thử lại.";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra trong quá trình xác nhận. Vui lòng thử lại.";
                return RedirectToAction("Index");
            }
        }
        #endregion
    }
}
