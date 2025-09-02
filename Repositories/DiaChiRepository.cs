using Final_VS1.Data;
using Microsoft.EntityFrameworkCore;

namespace Final_VS1.Repositories
{
    public class DiaChiRepository : IDiaChiRepository
    {
        private readonly LittleFishBeautyContext _context;

        public DiaChiRepository(LittleFishBeautyContext context)
        {
            _context = context;
        }

        public async Task<List<DiaChi>> GetAllAsync()
        {
            return await _context.DiaChis
                .Include(d => d.IdTaiKhoanNavigation)
                .ToListAsync();
        }

        public async Task<DiaChi?> GetByIdAsync(int id)
        {
            return await _context.DiaChis
                .Include(d => d.IdTaiKhoanNavigation)
                .FirstOrDefaultAsync(d => d.IdDiaChi == id);
        }

        public async Task<List<DiaChi>> GetByUserAsync(int userId)
        {
            return await _context.DiaChis
                .Where(d => d.IdTaiKhoan == userId)
                .OrderByDescending(d => d.MacDinh)
                .ThenByDescending(d => d.NgayTao)
                .ToListAsync();
        }

        public async Task<DiaChi?> GetDefaultByUserAsync(int userId)
        {
            return await _context.DiaChis
                .FirstOrDefaultAsync(d => d.IdTaiKhoan == userId && d.MacDinh == true);
        }

        public async Task<DiaChi> CreateAsync(DiaChi diaChi)
        {
            diaChi.NgayTao = DateTime.Now;
            
            // If this is the first address for user, make it default
            var existingAddresses = await GetByUserAsync(diaChi.IdTaiKhoan ?? 0);
            if (!existingAddresses.Any())
            {
                diaChi.MacDinh = true;
            }
            
            _context.DiaChis.Add(diaChi);
            await _context.SaveChangesAsync();
            return diaChi;
        }

        public async Task<DiaChi> UpdateAsync(DiaChi diaChi)
        {
            _context.DiaChis.Update(diaChi);
            await _context.SaveChangesAsync();
            return diaChi;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var diaChi = await GetByIdAsync(id);
            if (diaChi == null) return false;

            _context.DiaChis.Remove(diaChi);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SetDefaultAsync(int id, int userId)
        {
            // Remove default from all user addresses
            var userAddresses = await GetByUserAsync(userId);
            foreach (var addr in userAddresses)
            {
                addr.MacDinh = false;
            }

            // Set new default
            var newDefault = userAddresses.FirstOrDefault(a => a.IdDiaChi == id);
            if (newDefault == null) return false;

            newDefault.MacDinh = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task ClearDefaultAsync(int userId)
        {
            var userAddresses = await GetByUserAsync(userId);
            foreach (var addr in userAddresses)
            {
                addr.MacDinh = false;
            }
            await _context.SaveChangesAsync();
        }
    }
}
