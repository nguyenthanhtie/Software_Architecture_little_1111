using Microsoft.AspNetCore.Mvc;
using Final_VS1.Data;
using Final_VS1.Models;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

namespace Final_VS1.Controllers
{
    public class DangNhapController : Controller
    {
        private readonly LittleFishBeautyContext _context;

        public DangNhapController(LittleFishBeautyContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index(string returnUrl = null)
        {
            // Nếu đã đăng nhập, chuyển hướng đến trang phù hợp
            if (User.Identity.IsAuthenticated)
            {
                var role = User.FindFirst(ClaimTypes.Role)?.Value?.ToLower();
                if (role == "admin")
                {
                    return RedirectToAction("Index", "DonHang", new { area = "Admin" });
                }
                else
                {
                    return RedirectToAction("Index", "TrangChu", new { area = "KhachHang" });
                }
            }

            // Lưu returnUrl để chuyển hướng sau khi đăng nhập
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(DangNhapViewModel model, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }

            var user = _context.TaiKhoans
                .FirstOrDefault(u => u.Email == model.Username || u.HoTen == model.Username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.MatKhau))
            {
                ViewBag.Error = "Sai tên đăng nhập hoặc mật khẩu.";
                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }

            if (user.TrangThai != true)
            {
                ViewBag.Error = "Tài khoản chưa được kích hoạt. Vui lòng kiểm tra email để kích hoạt tài khoản.";
                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }

            // Chuẩn hóa vai trò - đảm bảo vai trò admin được xử lý đúng
            string normalizedRole = "Customer"; // Mặc định
            if (!string.IsNullOrEmpty(user.VaiTro))
            {
                var roleValue = user.VaiTro.Trim().ToLower();
                if (roleValue == "admin" || roleValue == "administrator")
                {
                    normalizedRole = "admin";
                }
                else if (roleValue == "khach" || roleValue == "customer")
                {
                    normalizedRole = "Customer";
                }
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.IdTaiKhoan.ToString()),
                new Claim(ClaimTypes.Name, user.HoTen ?? user.Email),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, normalizedRole)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties
            );

            // Debug log để kiểm tra vai trò
            Console.WriteLine($"[DEBUG] User {user.Email} logged in with role: {normalizedRole} (Original: {user.VaiTro})");

            // Chuyển hướng theo vai trò - ưu tiên admin
            if (normalizedRole == "admin")
            {
                Console.WriteLine($"[DEBUG] Redirecting admin user to Admin/Sanpham");
                return RedirectToAction("Index", "Sanpham", new { area = "Admin" });
            }
            else if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "TrangChu", new { area = "KhachHang" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DangXuat()
        {
            try
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                
                // Clear session
                HttpContext.Session.Clear();
                
                // Clear any remember me cookies
                Response.Cookies.Delete("RememberMe");
                
                // Clear authentication cookie explicitly
                Response.Cookies.Delete(".AspNetCore.Cookies");
                
                return RedirectToAction("Index", "TrangChu", new { area = "KhachHang" });
            }
            catch (Exception ex)
            {
                // Log error if needed
                TempData["Error"] = "Có lỗi xảy ra khi đăng xuất. Vui lòng thử lại.";
                return RedirectToAction("Index", "ThongTin", new { area = "KhachHang" });
            }
        }
    }
}
