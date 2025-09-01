using Final_VS1.Data;

namespace Final_VS1.Services
{
    public interface IDanhMucService
    {
        Task<List<DanhMuc>> GetAllCategoriesAsync();
        Task<DanhMuc?> GetCategoryByIdAsync(int id);
        Task<DanhMuc> CreateCategoryAsync(DanhMuc danhMuc);
        Task<bool> UpdateCategoryAsync(DanhMuc danhMuc);
        Task<bool> DeleteCategoryAsync(int id);
        Task<bool> IsCategoryNameExistsAsync(string tenDanhMuc, int? excludeId = null);
        Task<List<DanhMuc>> GetParentCategoriesAsync();
        Task<int> GetProductCountByCategoryAsync(int categoryId);
        Task<bool> CanDeleteCategoryAsync(int id);
        Task UpdateCategoriesOrderAsync(List<int> categoryIds);
        Task<int> GetNextDisplayOrderAsync();
    }
}
