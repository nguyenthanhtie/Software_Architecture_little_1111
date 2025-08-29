using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Final_VS1.Data;
using Final_VS1.Areas.KhachHang.Models;

namespace Final_VS1.Areas.KhachHang.Controllers
{
    [Area("KhachHang")]
    public class ChiTietController : Controller
    {
        private readonly LittleFishBeautyContext _context;

        public ChiTietController(LittleFishBeautyContext context)
        {
            _context = context;
        }

        // Helper method để lấy ảnh chính của sản phẩm theo thứ tự ưu tiên
        private string GetProductMainImage(ICollection<AnhSanPham>? anhSanPhams)
        {
            if (anhSanPhams != null && anhSanPhams.Any())
            {
                // Bước 1: Tìm ảnh chính
                var anhChinh = anhSanPhams.FirstOrDefault(a => 
                    !string.IsNullOrEmpty(a.LoaiAnh) && !string.IsNullOrEmpty(a.DuongDan) &&
                    (a.LoaiAnh.Trim().ToLower() == "chinh" || a.LoaiAnh.Trim().ToLower() == "chính"));
                    
                if (anhChinh != null)
                {
                    return anhChinh.DuongDan!;
                }
                else
                {
                    // Bước 2: Nếu không có ảnh chính, tìm ảnh phụ
                    var anhPhu = anhSanPhams.FirstOrDefault(a => 
                        !string.IsNullOrEmpty(a.LoaiAnh) && !string.IsNullOrEmpty(a.DuongDan) &&
                        (a.LoaiAnh.Trim().ToLower() == "phu" || a.LoaiAnh.Trim().ToLower() == "phụ"));
                        
                    if (anhPhu != null)
                    {
                        return anhPhu.DuongDan!;
                    }
                }
            }
            
            // Nếu không có ảnh nào, trả về ảnh mặc định
            return "/images/noimage.jpg";
        }

        public async Task<IActionResult> Index(int id)
        {
            var sanPham = await _context.SanPhams
                .Include(s => s.AnhSanPhams)
                .Include(s => s.IdDanhMucNavigation)
                .FirstOrDefaultAsync(s => s.IdSanPham == id);

            if (sanPham == null)
            {
                return NotFound();
            }

            // Get suggested products
            var sanPhamGoiY = await _context.SanPhams
                .Include(s => s.AnhSanPhams)
                .Where(s => s.IdDanhMuc == sanPham.IdDanhMuc &&
                           s.IdSanPham != id &&
                           s.TrangThai == true)
                .Take(4)
                .ToListAsync();

            ViewBag.SanPhamGoiY = sanPhamGoiY;
            ViewBag.ProductId = id;

            return View(sanPham);
        }

        [HttpGet]
        public async Task<IActionResult> MuaNgay(int productId, int quantity)
        {
            try
            {
                var sanPham = await _context.SanPhams
                    .Include(s => s.AnhSanPhams)
                    .FirstOrDefaultAsync(s => s.IdSanPham == productId);

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
                
                // Debug logging
                Console.WriteLine($"ChiTietController - MuaNgay: {jsonString}");
                
                // Cũng lưu vào session để backup
                HttpContext.Session.SetString("BuyNowItem", jsonString);
                
                return RedirectToAction("Index", "Pay", new { area = "KhachHang" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ChiTietController - MuaNgay Error: {ex.Message}");
                TempData["Error"] = "Có lỗi xảy ra: " + ex.Message;
                return RedirectToAction("Index", new { id = productId });
            }
        }
    }
}