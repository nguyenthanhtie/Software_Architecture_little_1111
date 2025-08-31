using Final_VS1.Data;

namespace Final_VS1.Repositories
{
    public interface IDanhMucRepository
    {
        Task<List<DanhMuc>> GetAllAsync();
        Task<DanhMuc?> GetByIdAsync(int id);
        Task AddAsync(DanhMuc danhMuc);
        Task UpdateAsync(DanhMuc danhMuc);
        Task DeleteAsync(int id);
        Task<bool> IsNameExistsAsync(string tenDanhMuc, int? excludeId = null);
        Task<int> GetNextDisplayOrderAsync();
        Task<List<DanhMuc>> GetParentCategoriesAsync();
        Task<int> GetProductCountByCategoryAsync(int categoryId);
    }
}