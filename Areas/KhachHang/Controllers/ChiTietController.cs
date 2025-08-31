using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Final_VS1.Repositories;
using Final_VS1.Data;

namespace Final_VS1.Areas.KhachHang.Controllers
{
    [Area("KhachHang")]
    public class ChiTietController : Controller
    {
        private readonly ISanPhamRepository _sanPhamRepository;

        public ChiTietController(ISanPhamRepository sanPhamRepository)
        {
            _sanPhamRepository = sanPhamRepository;
        }

        // Helper method để lấy ảnh chính của sản phẩm theo thứ tự ưu tiên
        private string GetProductMainImage(ICollection<AnhSanPham>? anhSanPhams)
        {
            if (anhSanPhams != null && anhSanPhams.Any())
            {
                var anhChinh = anhSanPhams.FirstOrDefault(a =>
                    !string.IsNullOrEmpty(a.LoaiAnh) && !string.IsNullOrEmpty(a.DuongDan) &&
                    (a.LoaiAnh.Trim().ToLower() == "chinh" || a.LoaiAnh.Trim().ToLower() == "chính"));

                if (anhChinh != null)
                {
                    return anhChinh.DuongDan!;
                }
                else
                {
                    var anhPhu = anhSanPhams.FirstOrDefault(a =>
                        !string.IsNullOrEmpty(a.LoaiAnh) && !string.IsNullOrEmpty(a.DuongDan) &&
                        (a.LoaiAnh.Trim().ToLower() == "phu" || a.LoaiAnh.Trim().ToLower() == "phụ"));

                    if (anhPhu != null)
                    {
                        return anhPhu.DuongDan!;
                    }
                }
            }

            return "/images/noimage.jpg";
        }

        public async Task<IActionResult> Index(int id)
        {
            var sanPham = await _sanPhamRepository.GetByIdAsync(id);

            if (sanPham == null)
            {
                return NotFound();
            }

var sanPhamGoiY = await _sanPhamRepository.GetSuggestedProductsAsync(sanPham.IdDanhMuc ?? 0, id, 4);

            ViewBag.SanPhamGoiY = sanPhamGoiY;
            ViewBag.ProductId = id;

            return View(sanPham);
        }

        [HttpGet]
        public async Task<IActionResult> MuaNgay(int productId, int quantity)
        {
            try
            {
                var sanPham = await _sanPhamRepository.GetByIdAsync(productId);

                if (sanPham == null)
                {
                    TempData["Error"] = "Sản phẩm không tồn tại";
                    return RedirectToAction("Index", new { id = productId });
                }

                if ((sanPham.SoLuongTonKho ?? 0) < quantity)
                {
                    TempData["Error"] = "Số lượng vượt quá tồn kho";
                    return RedirectToAction("Index", new { id = productId });
                }

                var buyNowItem = new
                {
                    productId = productId,
                    name = sanPham.TenSanPham,
                    price = sanPham.GiaBan ?? 0,
                    quantity = quantity,
                    total = (sanPham.GiaBan ?? 0) * quantity,
                    linkAnh = GetProductMainImage(sanPham.AnhSanPhams)
                };

                var jsonString = JsonSerializer.Serialize(buyNowItem);
                TempData["BuyNowItem"] = jsonString;

                HttpContext.Session.SetString("BuyNowItem", jsonString);

                return RedirectToAction("Index", "Pay", new { area = "KhachHang" });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Có lỗi xảy ra: " + ex.Message;
                return RedirectToAction("Index", new { id = productId });
            }
        }
    }
}