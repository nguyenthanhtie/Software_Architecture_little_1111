using Final_VS1.Data;
using Final_VS1.Repositories;
using Final_VS1.Services;

namespace Final_VS1.Services
{
    public class DanhMucService : IDanhMucService
    {
        private readonly IDanhMucRepository _danhMucRepository;

        public DanhMucService(IDanhMucRepository danhMucRepository)
        {
            _danhMucRepository = danhMucRepository;
        }

        public async Task<List<DanhMuc>> GetAllCategoriesAsync()
        {
            return await _danhMucRepository.GetAllAsync();
        }

        public async Task<DanhMuc?> GetCategoryByIdAsync(int id)
        {
            return await _danhMucRepository.GetByIdAsync(id);
        }

        public async Task<DanhMuc> CreateCategoryAsync(DanhMuc danhMuc)
        {
            if (danhMuc == null)
                throw new ArgumentNullException(nameof(danhMuc));

            if (string.IsNullOrWhiteSpace(danhMuc.TenDanhMuc))
                throw new ArgumentException("Tên danh mục là bắt buộc", nameof(danhMuc.TenDanhMuc));

            if (danhMuc.TenDanhMuc.Length > 100)
                throw new ArgumentException("Tên danh mục không được vượt quá 100 ký tự", nameof(danhMuc.TenDanhMuc));

            if (await _danhMucRepository.IsNameExistsAsync(danhMuc.TenDanhMuc))
                throw new InvalidOperationException("Tên danh mục đã tồn tại");

            // Set default values
            if (danhMuc.ThuTuHienThi == null || danhMuc.ThuTuHienThi == 0)
            {
                danhMuc.ThuTuHienThi = await _danhMucRepository.GetNextDisplayOrderAsync();
            }

            await _danhMucRepository.AddAsync(danhMuc);
            return danhMuc;
        }

        public async Task<bool> UpdateCategoryAsync(DanhMuc danhMuc)
        {
            if (danhMuc == null)
                throw new ArgumentNullException(nameof(danhMuc));

            var existingCategory = await _danhMucRepository.GetByIdAsync(danhMuc.IdDanhMuc);
            if (existingCategory == null)
                throw new InvalidOperationException("Danh mục không tồn tại");

            if (string.IsNullOrWhiteSpace(danhMuc.TenDanhMuc))
                throw new ArgumentException("Tên danh mục là bắt buộc", nameof(danhMuc.TenDanhMuc));

            if (danhMuc.TenDanhMuc.Length > 100)
                throw new ArgumentException("Tên danh mục không được vượt quá 100 ký tự", nameof(danhMuc.TenDanhMuc));

            if (await _danhMucRepository.IsNameExistsAsync(danhMuc.TenDanhMuc, danhMuc.IdDanhMuc))
                throw new InvalidOperationException("Tên danh mục đã tồn tại");

            await _danhMucRepository.UpdateAsync(danhMuc);
            return true;
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var category = await _danhMucRepository.GetByIdAsync(id);
            if (category == null)
                return false;

            if (!await CanDeleteCategoryAsync(id))
                throw new InvalidOperationException("Không thể xóa danh mục đã có sản phẩm");

            await _danhMucRepository.DeleteAsync(id);
            return true;
        }

        public async Task<bool> IsCategoryNameExistsAsync(string tenDanhMuc, int? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(tenDanhMuc))
                return false;

            return await _danhMucRepository.IsNameExistsAsync(tenDanhMuc, excludeId);
        }

        public async Task<List<DanhMuc>> GetParentCategoriesAsync()
        {
            return await _danhMucRepository.GetParentCategoriesAsync();
        }

        public async Task<int> GetProductCountByCategoryAsync(int categoryId)
        {
            return await _danhMucRepository.GetProductCountByCategoryAsync(categoryId);
        }

        public async Task<bool> CanDeleteCategoryAsync(int id)
        {
            var productCount = await _danhMucRepository.GetProductCountByCategoryAsync(id);
            return productCount == 0;
        }

        public Task UpdateCategoriesOrderAsync(List<int> categoryIds)
        {
            if (categoryIds == null || categoryIds.Count == 0)
                return Task.CompletedTask;

            // This would need to be implemented in the repository
            // For now, we'll throw a not implemented exception
            throw new NotImplementedException("UpdateCategoriesOrderAsync needs to be implemented in repository");
        }

        public async Task<int> GetNextDisplayOrderAsync()
        {
            return await _danhMucRepository.GetNextDisplayOrderAsync();
        }
    }
}
