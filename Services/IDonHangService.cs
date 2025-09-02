using Final_VS1.Data;

namespace Final_VS1.Services
{
    public interface IDonHangService
    {
        Task<List<DonHang>> GetAllOrdersAsync();
        Task<DonHang?> GetOrderByIdAsync(int id);
        Task<DonHang?> GetOrderByIdAndUserAsync(int orderId, int userId);
        Task<DonHang> CreateOrderAsync(DonHang donHang, List<ChiTietDonHang> chiTietDonHangs);
        Task<DonHang> CreateOrderWithAddressAsync(int userId, string hoTen, string soDienThoai, string diaChi, string? paymentMethod, List<ChiTietDonHang> chiTietDonHangs);
        Task<bool> UpdateOrderStatusAsync(int id, string status);
        Task<List<DonHang>> GetOrdersByUserAsync(int userId, string? status = null);
        Task<int> CountOrdersByUserAsync(int userId, string? status = null);
        Task<bool> CancelOrderAsync(int orderId, int userId);
        Task<bool> CanCancelOrderAsync(int orderId, int userId);
        Task<List<DonHang>> GetOrdersByStatusAsync(string status);
        Task<List<DonHang>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<decimal> GetTotalRevenueAsync();
        Task<decimal> GetRevenueByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<int> GetTotalOrdersCountAsync();
        Task<List<DonHang>> GetRecentOrdersAsync(int count);
    }
}
