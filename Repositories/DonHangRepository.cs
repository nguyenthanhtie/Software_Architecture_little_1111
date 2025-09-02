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
                .Include(d => d.IdDiaChiNavigation)
                .Include(d => d.ChiTietDonHangs)
                    .ThenInclude(ct => ct.IdSanPhamNavigation)
                .OrderByDescending(d => d.NgayDat)
                .ToListAsync();
        }

        public async Task<DonHang?> GetByIdAsync(int id)
        {
            return await _context.DonHangs
                .Include(d => d.IdTaiKhoanNavigation)
                .Include(d => d.IdDiaChiNavigation)
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

        public async Task<DonHang> CreateOrderAsync(DonHang donHang, List<ChiTietDonHang> chiTietDonHangs)
        {
            _context.DonHangs.Add(donHang);
            await _context.SaveChangesAsync();

            foreach (var chiTiet in chiTietDonHangs)
            {
                chiTiet.IdDonHang = donHang.IdDonHang;
                _context.ChiTietDonHangs.Add(chiTiet);
            }
            await _context.SaveChangesAsync();
            return donHang;
        }

        public async Task<List<DonHang>> GetByUserAsync(int userId, string? status)
        {
            var query = _context.DonHangs
                .Include(d => d.IdDiaChiNavigation)
                .Include(d => d.ChiTietDonHangs)
                    .ThenInclude(ct => ct.IdSanPhamNavigation!)
                        .ThenInclude(sp => sp.AnhSanPhams)
                .Include(d => d.IdTaiKhoanNavigation)
                .Where(d => d.IdTaiKhoan == userId);

            if (!string.IsNullOrEmpty(status))
                query = query.Where(d => d.TrangThai == status);

            return await query.OrderByDescending(d => d.NgayDat).ToListAsync();
        }

        public async Task<int> CountByUserAsync(int userId, string? status = null)
        {
            var query = _context.DonHangs.Where(d => d.IdTaiKhoan == userId);
            if (!string.IsNullOrEmpty(status))
                query = query.Where(d => d.TrangThai == status);
            return await query.CountAsync();
        }

        public async Task<DonHang?> GetByIdAndUserAsync(int orderId, int userId)
        {
            return await _context.DonHangs.FirstOrDefaultAsync(d => d.IdDonHang == orderId && d.IdTaiKhoan == userId);
        }

        public async Task<bool> CancelOrderAsync(int orderId, int userId)
        {
            var donHang = await GetByIdAndUserAsync(orderId, userId);
            if (donHang == null || donHang.TrangThai != "Chờ xác nhận")
                return false;
            donHang.TrangThai = "Đã hủy";
            await _context.SaveChangesAsync();
            return true;
        }
    }
}