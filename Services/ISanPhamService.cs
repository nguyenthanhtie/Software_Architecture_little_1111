using Final_VS1.Data;

namespace Final_VS1.Services
{
    public interface ISanPhamService
    {
        Task<List<SanPham>> GetAllProductsAsync();
        Task<SanPham?> GetProductByIdAsync(int id);
        Task<SanPham> CreateProductAsync(SanPham sanPham);
        Task<bool> UpdateProductAsync(SanPham sanPham);
        Task<bool> DeleteProductAsync(int id);
        Task<bool> IsSkuExistsAsync(string sku, int? excludeId = null);
        Task<bool> UpdateProductStockAsync(int id, int newStock);
        Task<List<SanPham>> GetNewestProductsAsync(int count);
        Task<List<SanPham>> GetBestSellingProductsAsync(int count);
        Task<List<SanPham>> GetSuggestedProductsAsync(int categoryId, int excludeId, int count);
        Task<List<SanPham>> GetProductsByCategoryAsync(int categoryId);
        Task<List<SanPham>> SearchProductsAsync(string searchTerm);
        Task<List<SanPham>> GetProductsByPriceRangeAsync(decimal? minPrice, decimal? maxPrice);
        Task<bool> IsProductAvailableAsync(int productId, int quantity);
        Task<List<SanPham>> GetFeaturedProductsAsync(int count);
    }
}
