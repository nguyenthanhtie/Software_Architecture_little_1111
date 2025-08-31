using Final_VS1.Data;
using Microsoft.EntityFrameworkCore;

namespace Final_VS1.Repositories
{
    public class DanhMucRepository : IDanhMucRepository
    {
        private readonly LittleFishBeautyContext _context;
        public DanhMucRepository(LittleFishBeautyContext context)
        {
            _context = context;
        }

        public async Task<List<DanhMuc>> GetAllAsync()
        {
            return await _context.DanhMucs
                .Include(d => d.SanPhams)
                .Include(d => d.IdDanhMucChaNavigation)
                .OrderBy(d => d.ThuTuHienThi)
                .ToListAsync();
        }

        public async Task<DanhMuc?> GetByIdAsync(int id)
        {
            return await _context.DanhMucs
                .Include(d => d.SanPhams)
                .Include(d => d.IdDanhMucChaNavigation)
                .FirstOrDefaultAsync(d => d.IdDanhMuc == id);
        }

        public async Task AddAsync(DanhMuc danhMuc)
        {
            _context.DanhMucs.Add(danhMuc);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(DanhMuc danhMuc)
        {
            _context.DanhMucs.Update(danhMuc);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var danhMuc = await _context.DanhMucs.Include(d => d.SanPhams).FirstOrDefaultAsync(d => d.IdDanhMuc == id);
            if (danhMuc == null) return;
            if (danhMuc.SanPhams.Any()) return;
            _context.DanhMucs.Remove(danhMuc);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsNameExistsAsync(string tenDanhMuc, int? excludeId = null)
        {
            var nameLower = tenDanhMuc.Trim().ToLower();
            return await _context.DanhMucs
                .AnyAsync(d => d.TenDanhMuc != null &&
                               d.TenDanhMuc.ToLower() == nameLower &&
                               (!excludeId.HasValue || d.IdDanhMuc != excludeId.Value));
        }

        public async Task<int> GetNextDisplayOrderAsync()
        {
            var maxOrder = await _context.DanhMucs.MaxAsync(d => (int?)d.ThuTuHienThi) ?? 0;
            return maxOrder + 1;
        }

   

public async Task<List<DanhMuc>> GetParentCategoriesAsync()
{
    return await _context.DanhMucs
        .Where(d => d.IdDanhMucCha == null)
        .OrderBy(d => d.ThuTuHienThi)
        .Take(6)
        .ToListAsync();
}

public async Task<int> GetProductCountByCategoryAsync(int categoryId)
{
    return await _context.SanPhams
        .Where(s => s.IdDanhMuc == categoryId && s.TrangThai == true)
        .CountAsync();
}

    }
}