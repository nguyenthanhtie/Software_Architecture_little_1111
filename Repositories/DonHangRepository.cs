using Final_VS1.Data;
using Microsoft.EntityFrameworkCore;

namespace Final_VS1.Repositories
{
    public class DonHangRepository : IDonHangRepository
    {
        private readonly LittleFishBeautyContext _context;
        public DonHangRepository(LittleFishBeautyContext context)
        {
            _context = context;
        }

        public async Task<List<DonHang>> GetAllAsync()
        {
            return await _context.DonHangs
                .Include(d => d.IdTaiKhoanNavigation)
                .Include(d => d.ChiTietDonHangs)
                    .ThenInclude(ct => ct.IdSanPhamNavigation)
                .OrderByDescending(d => d.NgayDat)
                .ToListAsync();
        }

        public async Task<DonHang?> GetByIdAsync(int id)
        {
            return await _context.DonHangs
                .Include(d => d.IdTaiKhoanNavigation)
                .Include(d => d.ChiTietDonHangs)
                    .ThenInclude(ct => ct.IdSanPhamNavigation)
                .FirstOrDefaultAsync(d => d.IdDonHang == id);
        }

        public async Task UpdateStatusAsync(int id, string status)
        {
            var order = await _context.DonHangs.FindAsync(id);
            if (order != null)
            {
                order.TrangThai = status;
                await _context.SaveChangesAsync();
            }
        }
    }
}