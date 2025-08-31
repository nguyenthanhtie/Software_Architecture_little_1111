using Final_VS1.Data;
using Final_VS1.Models;

namespace Final_VS1.Repositories
{
    public interface ISanPhamRepository
    {
        Task<List<SanPham>> GetAllAsync();
        Task<SanPham?> GetByIdAsync(int id);
        Task AddAsync(SanPham sanPham);
        Task UpdateAsync(SanPham sanPham);
        Task DeleteAsync(int id);
        Task<bool> IsSkuExistsAsync(string sku, int? excludeId = null);
    }
}