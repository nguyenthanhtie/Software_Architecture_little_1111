using Final_VS1.Data;
using Final_VS1.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Final_VS1.Repositories
{
    public class TaiKhoanRepository : ITaiKhoanRepository
    {
        private readonly LittleFishBeautyContext _context;

        public TaiKhoanRepository(LittleFishBeautyContext context)
        {
            _context = context;
        }

        public async Task<List<TaiKhoan>> GetAllAsync()
        {
            return await _context.TaiKhoans
                .OrderByDescending(t => t.NgayTao)
                .ToListAsync();
        }

        public async Task<TaiKhoan?> GetByIdAsync(int id)
        {
            return await _context.TaiKhoans
                .Include(t => t.DonHangs)
                .FirstOrDefaultAsync(t => t.IdTaiKhoan == id);
        }

        public async Task<TaiKhoan?> GetByEmailAsync(string email)
        {
            return await _context.TaiKhoans
                .FirstOrDefaultAsync(t => t.Email == email);
        }

        public async Task<TaiKhoan?> GetByUsernameOrEmailAsync(string usernameOrEmail)
        {
            return await _context.TaiKhoans
                .FirstOrDefaultAsync(t => t.Email == usernameOrEmail || t.HoTen == usernameOrEmail);
        }

        public async Task AddAsync(TaiKhoan taiKhoan)
        {
            _context.TaiKhoans.Add(taiKhoan);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(TaiKhoan taiKhoan)
        {
            _context.TaiKhoans.Update(taiKhoan);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var taiKhoan = await GetByIdAsync(id);
            if (taiKhoan != null)
            {
                _context.TaiKhoans.Remove(taiKhoan);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> IsEmailExistsAsync(string email, int? excludeId = null)
        {
            var query = _context.TaiKhoans.Where(t => t.Email == email);
            
            if (excludeId.HasValue)
            {
                query = query.Where(t => t.IdTaiKhoan != excludeId.Value);
            }
            
            return await query.AnyAsync();
        }

        public async Task<List<TaiKhoan>> GetByRoleAsync(string role)
        {
            return await _context.TaiKhoans
                .Where(t => t.VaiTro == role)
                .OrderByDescending(t => t.NgayTao)
                .ToListAsync();
        }

        public async Task<int> GetTotalCountAsync()
        {
            return await _context.TaiKhoans.CountAsync();
        }

        public async Task<int> GetActiveCountAsync()
        {
            return await _context.TaiKhoans
                .Where(t => t.TrangThai == true)
                .CountAsync();
        }
    }
}
