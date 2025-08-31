using Final_VS1.Data;
using Final_VS1.Models;
using Microsoft.EntityFrameworkCore;

namespace Final_VS1.Repositories
{
    public class SanPhamRepository : ISanPhamRepository
    {
        private readonly LittleFishBeautyContext _context;
        public SanPhamRepository(LittleFishBeautyContext context)
        {
            _context = context;
        }

        public async Task<List<SanPham>> GetAllAsync()
        {
            return await _context.SanPhams
                .Include(s => s.IdDanhMucNavigation)
                .Include(s => s.AnhSanPhams)
                .OrderByDescending(s => s.NgayTao)
                .ToListAsync();
        }

        public async Task<SanPham?> GetByIdAsync(int id)
        {
            return await _context.SanPhams
                .Include(s => s.IdDanhMucNavigation)
                .Include(s => s.AnhSanPhams)
                .FirstOrDefaultAsync(s => s.IdSanPham == id);
        }

        public async Task AddAsync(SanPham sanPham)
        {
            _context.SanPhams.Add(sanPham);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(SanPham sanPham)
        {
            _context.SanPhams.Update(sanPham);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var sanPham = await _context.SanPhams
                .Include(s => s.AnhSanPhams)
                .Include(s => s.ChiTietDonHangs)
                .FirstOrDefaultAsync(s => s.IdSanPham == id);

            if (sanPham == null) return;

            if (sanPham.ChiTietDonHangs.Any()) return;

            if (sanPham.AnhSanPhams.Any())
            {
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

            _context.SanPhams.Remove(sanPham);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsSkuExistsAsync(string sku, int? excludeId = null)
        {
            return await _context.SanPhams
                .AnyAsync(s => s.Sku == sku && (!excludeId.HasValue || s.IdSanPham != excludeId.Value));
        }
    }
}