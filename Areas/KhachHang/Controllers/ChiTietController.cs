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

            // Debug: Log product information
            Console.WriteLine($"Product ID: {sanPham.IdSanPham}");
            Console.WriteLine($"Product Name: {sanPham.TenSanPham}");
            Console.WriteLine($"Description: {sanPham.MoTa ?? "NULL"}");
            Console.WriteLine($"Usage: {sanPham.CachSuDung ?? "NULL"}");

            var sanPhamGoiY = await _sanPhamRepository.GetSuggestedProductsAsync(sanPham.IdDanhMuc ?? 0, id, 4);

            ViewBag.SanPhamGoiY = sanPhamGoiY;
            ViewBag.ProductId = id;

            return View(sanPham);
        }

        // Debug action to check product data
        [HttpGet]
        public async Task<IActionResult> DebugProduct(int id)
        {
            var sanPham = await _sanPhamRepository.GetByIdAsync(id);
            if (sanPham == null)
            {
                return Json(new { error = "Product not found" });
            }

            return Json(new {
                id = sanPham.IdSanPham,
                name = sanPham.TenSanPham,
                description = sanPham.MoTa,
                usage = sanPham.CachSuDung,
                price = sanPham.GiaBan,
                stock = sanPham.SoLuongTonKho,
                category = sanPham.IdDanhMucNavigation?.TenDanhMuc
            });
        }

        // Action to update sample product descriptions for testing
        [HttpGet]
        public async Task<IActionResult> UpdateSampleDescriptions()
        {
            try
            {
                // Lấy một vài sản phẩm đầu tiên để cập nhật dữ liệu mẫu
                var sanPhams = await _sanPhamRepository.GetAllAsync();
                var sampleProducts = sanPhams.Take(5).ToList();

                var sampleDescriptions = new List<(string MoTa, string CachSuDung)>
                {
                    (
                        @"<p><strong>Kem dưỡng ẩm cao cấp</strong> được chiết xuất từ thiên nhiên, giúp cung cấp độ ẩm sâu cho làn da.</p>
                        <ul>
                            <li>Chiết xuất từ <strong>lô hội</strong> và <strong>vitamin E</strong></li>
                            <li>An toàn cho mọi loại da, kể cả da nhạy cảm</li>
                            <li>Không chứa paraben, sulfate có hại</li>
                            <li>Kết cấu mềm mại, thấm hút nhanh</li>
                            <li>Có thể sử dụng cho cả mặt và cơ thể</li>
                        </ul>
                        <p><em>Sản phẩm đã được kiểm nghiệm dermatologically và an toàn tuyệt đối cho da.</em></p>",
                        
                        @"<h6><strong>Hướng dẫn sử dụng chi tiết:</strong></h6>
                        <ol>
                            <li><strong>Làm sạch da:</strong> Rửa mặt và cơ thể sạch sẽ bằng nước ấm</li>
                            <li><strong>Lau khô:</strong> Dùng khăn mềm thấm nhẹ, để da còn hơi ẩm</li>
                            <li><strong>Thoa kem:</strong> Lấy một lượng kem vừa đủ, thoa đều lên da</li>
                            <li><strong>Massage:</strong> Massage nhẹ nhàng theo chuyển động tròn trong 30-60 giây</li>
                            <li><strong>Để thấm:</strong> Chờ 2-3 phút để kem thấm hoàn toàn</li>
                        </ol>
                        <p><strong>Ghi chú:</strong> Sử dụng 2 lần/ngày (sáng và tối) để đạt hiệu quả tốt nhất.</p>"
                    ),
                    (
                        @"<p><strong>Serum trắng da đột phá</strong> với công nghệ Nano hiện đại, giúp làm sáng da tức thì.</p>
                        <ul>
                            <li>Công thức <strong>Vitamin C</strong> + <strong>Niacinamide</strong> + <strong>Hyaluronic Acid</strong></li>
                            <li>Giảm thâm nám, tàn nhang hiệu quả</li>
                            <li>Kích thích tái tạo collagen tự nhiên</li>
                            <li>Cung cấp độ ẩm sâu cho da</li>
                            <li>Phù hợp cho da dầu, da hỗn hợp</li>
                        </ul>
                        <p><em>Hiệu quả có thể thấy rõ sau 7-14 ngày sử dụng đều đặn.</em></p>",
                        
                        @"<h6><strong>Cách sử dụng Serum:</strong></h6>
                        <ol>
                            <li><strong>Tẩy trang:</strong> Làm sạch da hoàn toàn</li>
                            <li><strong>Toner:</strong> Dùng nước hoa hồng cân bằng độ pH</li>
                            <li><strong>Thoa serum:</strong> Nhỏ 2-3 giọt serum lên mặt</li>
                            <li><strong>Patting:</strong> Vỗ nhẹ để serum thấm đều</li>
                            <li><strong>Chờ thấm:</strong> Đợi 5-10 phút trước khi thoa kem dưỡng</li>
                            <li><strong>Chống nắng:</strong> Bôi kem chống nắng khi ra ngoài (ban ngày)</li>
                        </ol>
                        <p><strong>Lưu ý:</strong> Chỉ sử dụng vào buổi tối và luôn dùng kem chống nắng khi ra ngoài.</p>"
                    ),
                    (
                        @"<p><strong>Mặt nạ collagen cao cấp</strong> từ Hàn Quốc, giúp phục hồi và tái tạo da chuyên sâu.</p>
                        <ul>
                            <li>Chiết xuất <strong>collagen thủy phân</strong> + <strong>peptide</strong></li>
                            <li>Giảm nếp nhăn và dấu hiệu lão hóa</li>
                            <li>Tăng độ đàn hồi và săn chắc da</li>
                            <li>Cung cấp dưỡng chất sâu cho da</li>
                            <li>Phù hợp mọi loại da, đặc biệt da lão hóa</li>
                        </ul>
                        <p><em>Công nghệ hydrogel giúp dưỡng chất thấm sâu và lâu hơn.</em></p>",
                        
                        @"<h6><strong>Hướng dẫn sử dụng mặt nạ:</strong></h6>
                        <ol>
                            <li><strong>Làm sạch:</strong> Rửa mặt sạch và lau khô</li>
                            <li><strong>Mở gói:</strong> Lấy mặt nạ ra khỏi bao bì</li>
                            <li><strong>Đắp mặt nạ:</strong> Đắp lên mặt, ấn nhẹ cho khít</li>
                            <li><strong>Thư giãn:</strong> Để mặt nạ trong 15-20 phút</li>
                            <li><strong>Tháo mặt nạ:</strong> Gỡ nhẹ nhàng từ dưới lên trên</li>
                            <li><strong>Massage:</strong> Massage nhẹ tinh chất còn lại trên da</li>
                        </ol>
                        <p><strong>Tần suất:</strong> Sử dụng 2-3 lần/tuần để đạt hiệu quả tối ưu.</p>"
                    ),
                    (
                        @"<p><strong>Sữa rửa mặt tạo bọt</strong> với thành phần tự nhiên, làm sạch sâu mà không khô da.</p>
                        <ul>
                            <li>Chiết xuất <strong>trà xanh</strong> + <strong>mật ong</strong></li>
                            <li>Loại bỏ bụi bẩn, dầu thừa hiệu quả</li>
                            <li>Cân bằng độ ẩm tự nhiên của da</li>
                            <li>Kháng khuẩn, giảm viêm mụn</li>
                            <li>pH 5.5 phù hợp với da tự nhiên</li>
                        </ul>
                        <p><em>Công thức đặc biệt cho da nhạy cảm và da mụn.</em></p>",
                        
                        @"<h6><strong>Cách sử dụng sữa rửa mặt:</strong></h6>
                        <ol>
                            <li><strong>Làm ướt:</strong> Làm ướt mặt bằng nước ấm</li>
                            <li><strong>Lấy sữa rửa mặt:</strong> Squeeze một lượng bằng hạt đậu</li>
                            <li><strong>Tạo bọt:</strong> Đánh bọt trong lòng bàn tay</li>
                            <li><strong>Massage:</strong> Massage nhẹ nhàng khắp mặt trong 30 giây</li>
                            <li><strong>Tránh vùng mắt:</strong> Không để sữa rửa mặt vào mắt</li>
                            <li><strong>Rửa sạch:</strong> Rửa sạch bằng nước mát</li>
                        </ol>
                        <p><strong>Sử dụng:</strong> 2 lần/ngày (sáng và tối) để da luôn sạch sẽ.</p>"
                    ),
                    (
                        @"<p><strong>Kem chống nắng SPF 50+</strong> bảo vệ toàn diện khỏi tia UV có hại.</p>
                        <ul>
                            <li>Công thức <strong>kép UVA/UVB</strong> protection</li>
                            <li>Chống thấm nước, mồ hôi lâu</li>
                            <li>Không gây bết dính, thẩm thấu nhanh</li>
                            <li>Bổ sung vitamin E chống oxy hóa</li>
                            <li>Phù hợp cho da nhạy cảm</li>
                        </ul>
                        <p><em>Được bác sĩ da liễu khuyên dùng hàng đầu.</em></p>",
                        
                        @"<h6><strong>Hướng dẫn sử dụng kem chống nắng:</strong></h6>
                        <ol>
                            <li><strong>Làm sạch da:</strong> Hoàn tất skincare routine</li>
                            <li><strong>Lượng kem:</strong> Lấy lượng kem bằng đồng xu 500đ</li>
                            <li><strong>Thoa đều:</strong> Thoa đều khắp mặt và cổ</li>
                            <li><strong>Chờ thấm:</strong> Đợi 15-20 phút trước khi ra ngoài</li>
                            <li><strong>Thoa lại:</strong> Thoa lại sau mỗi 2 giờ hoặc khi tiếp xúc nước</li>
                            <li><strong>Tẩy trang:</strong> Dùng tẩy trang để làm sạch cuối ngày</li>
                        </ol>
                        <p><strong>Quan trọng:</strong> Sử dụng hàng ngày, kể cả trong nhà để bảo vệ da hiệu quả.</p>"
                    )
                };

                int updated = 0;
                for (int i = 0; i < Math.Min(sampleProducts.Count, sampleDescriptions.Count); i++)
                {
                    var product = sampleProducts[i];
                    var (mota, cachSuDung) = sampleDescriptions[i];
                    
                    // Cập nhật dữ liệu mẫu cho các sản phẩm
                    product.MoTa = mota;
                    product.CachSuDung = cachSuDung;
                    await _sanPhamRepository.UpdateAsync(product);
                    updated++;
                }

                return Json(new { 
                    success = true,
                    message = $"Đã cập nhật mô tả và cách sử dụng cho {updated} sản phẩm",
                    updatedCount = updated,
                    updatedProducts = sampleProducts.Take(updated).Select(p => new {
                        id = p.IdSanPham,
                        name = p.TenSanPham,
                        hasDescription = !string.IsNullOrEmpty(p.MoTa),
                        hasUsage = !string.IsNullOrEmpty(p.CachSuDung)
                    }).ToList()
                });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
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