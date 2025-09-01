using System.Diagnostics;
using Final_VS1.Models;
using Microsoft.AspNetCore.Mvc;
using Final_VS1.Services;

namespace Final_VS1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IServiceFactory _serviceFactory;

        public HomeController(ILogger<HomeController> logger, IServiceFactory serviceFactory)
        {
            _logger = logger;
            _serviceFactory = serviceFactory;
        }

        public IActionResult Index()
        {
            // Sử dụng factory để tạo services khi cần
            var sanPhamService = _serviceFactory.CreateSanPhamService();
            var danhMucService = _serviceFactory.CreateDanhMucService();
            
            // Logic hiển thị trang chủ có thể được thêm vào đây
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
