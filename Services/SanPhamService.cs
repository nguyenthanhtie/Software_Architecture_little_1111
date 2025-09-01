using Final_VS1.Data;
using Final_VS1.Repositories;
using Final_VS1.Services;

namespace Final_VS1.Services
{
    public class SanPhamService : ISanPhamService
    {
        private readonly ISanPhamRepository _sanPhamRepository;

        public SanPhamService(ISanPhamRepository sanPhamRepository)
        {
            _sanPhamRepository = sanPhamRepository;
        }

        public async Task<List<SanPham>> GetAllProductsAsync()
        {
            return await _sanPhamRepository.GetAllAsync();
        }

        public async Task<SanPham?> GetProductByIdAsync(int id)
        {
            return await _sanPhamRepository.GetByIdAsync(id);
        }

        public async Task<SanPham> CreateProductAsync(SanPham sanPham)
        {
            if (sanPham == null)
                throw new ArgumentNullException(nameof(sanPham));

            if (string.IsNullOrWhiteSpace(sanPham.TenSanPham))
                throw new ArgumentException("Tên sản phẩm là bắt buộc", nameof(sanPham.TenSanPham));

            if (sanPham.GiaBan < 0)
                sanPham.GiaBan = 0;

            if (sanPham.SoLuongTonKho < 0)
                sanPham.SoLuongTonKho = 0;

            // Check SKU uniqueness if provided
            if (!string.IsNullOrWhiteSpace(sanPham.Sku))
            {
                if (await _sanPhamRepository.IsSkuExistsAsync(sanPham.Sku))
                    throw new InvalidOperationException("SKU đã tồn tại trong hệ thống");
            }

            // Set default values
            if (sanPham.TrangThai == null)
                sanPham.TrangThai = true;

            await _sanPhamRepository.AddAsync(sanPham);
            return sanPham;
        }

        public async Task<bool> UpdateProductAsync(SanPham sanPham)
        {
            if (sanPham == null)
                throw new ArgumentNullException(nameof(sanPham));

            var existingProduct = await _sanPhamRepository.GetByIdAsync(sanPham.IdSanPham);
            if (existingProduct == null)
                throw new InvalidOperationException("Sản phẩm không tồn tại");

            if (string.IsNullOrWhiteSpace(sanPham.TenSanPham))
                throw new ArgumentException("Tên sản phẩm là bắt buộc", nameof(sanPham.TenSanPham));

            if (sanPham.GiaBan < 0)
                sanPham.GiaBan = 0;

            if (sanPham.SoLuongTonKho < 0)
                sanPham.SoLuongTonKho = 0;

            // Check SKU uniqueness if provided
            if (!string.IsNullOrWhiteSpace(sanPham.Sku))
            {
                if (await _sanPhamRepository.IsSkuExistsAsync(sanPham.Sku, sanPham.IdSanPham))
                    throw new InvalidOperationException("SKU đã được sử dụng bởi sản phẩm khác");
            }

            await _sanPhamRepository.UpdateAsync(sanPham);
            return true;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _sanPhamRepository.GetByIdAsync(id);
            if (product == null)
                return false;

            await _sanPhamRepository.DeleteAsync(id);
            return true;
        }

        public async Task<bool> IsSkuExistsAsync(string sku, int? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(sku))
                return false;

            return await _sanPhamRepository.IsSkuExistsAsync(sku, excludeId);
        }

        public async Task<bool> UpdateProductStockAsync(int id, int newStock)
        {
            if (newStock < 0)
                throw new ArgumentException("Số lượng tồn kho không được âm", nameof(newStock));

            return await _sanPhamRepository.UpdateStockAsync(id, newStock);
        }

        public async Task<List<SanPham>> GetNewestProductsAsync(int count)
        {
            if (count <= 0)
                return new List<SanPham>();

            return await _sanPhamRepository.GetNewestProductsAsync(count);
        }

        public async Task<List<SanPham>> GetBestSellingProductsAsync(int count)
        {
            if (count <= 0)
                return new List<SanPham>();

            return await _sanPhamRepository.GetBestSellingProductsAsync(count);
        }

        public async Task<List<SanPham>> GetSuggestedProductsAsync(int categoryId, int excludeId, int count)
        {
            if (count <= 0)
                return new List<SanPham>();

            return await _sanPhamRepository.GetSuggestedProductsAsync(categoryId, excludeId, count);
        }

        public async Task<List<SanPham>> GetProductsByCategoryAsync(int categoryId)
        {
            var allProducts = await _sanPhamRepository.GetAllAsync();
            return allProducts.Where(p => p.IdDanhMuc == categoryId && p.TrangThai == true).ToList();
        }

        public async Task<List<SanPham>> SearchProductsAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return new List<SanPham>();

            var allProducts = await _sanPhamRepository.GetAllAsync();
            var searchLower = searchTerm.ToLower();

            return allProducts.Where(p =>
                p.TrangThai == true &&
                (
                    (p.TenSanPham != null && p.TenSanPham.ToLower().Contains(searchLower)) ||
                    (p.MoTa != null && p.MoTa.ToLower().Contains(searchLower)) ||
                    (p.Sku != null && p.Sku.ToLower().Contains(searchLower))
                )
            ).ToList();
        }

        public async Task<List<SanPham>> GetProductsByPriceRangeAsync(decimal? minPrice, decimal? maxPrice)
        {
            var allProducts = await _sanPhamRepository.GetAllAsync();
            var query = allProducts.Where(p => p.TrangThai == true).AsQueryable();

            if (minPrice.HasValue)
                query = query.Where(p => p.GiaBan >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(p => p.GiaBan <= maxPrice.Value);

            return query.ToList();
        }

        public async Task<bool> IsProductAvailableAsync(int productId, int quantity)
        {
            var product = await _sanPhamRepository.GetByIdAsync(productId);
            if (product == null || product.TrangThai != true)
                return false;

            return (product.SoLuongTonKho ?? 0) >= quantity;
        }

        public async Task<List<SanPham>> GetFeaturedProductsAsync(int count)
        {
            if (count <= 0)
                return new List<SanPham>();

            var allProducts = await _sanPhamRepository.GetAllAsync();
            return allProducts
                .Where(p => p.TrangThai == true)
                .OrderByDescending(p => p.IdSanPham) // You might want to add a Featured flag to the model
                .Take(count)
                .ToList();
        }
    }
}
