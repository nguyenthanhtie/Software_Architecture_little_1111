using Final_VS1.Data;

namespace Final_VS1.Repositories
{
    public interface IDonHangRepository
    {
        Task<List<DonHang>> GetAllAsync();
        Task<DonHang?> GetByIdAsync(int id);
        Task UpdateStatusAsync(int id, string status);
        Task<DonHang> CreateOrderAsync(DonHang donHang, List<ChiTietDonHang> chiTietDonHangs);
        Task<List<DonHang>> GetByUserAsync(int userId, string? status);
      Task<int> CountByUserAsync(int userId, string? status = null);
      Task<DonHang?> GetByIdAndUserAsync(int orderId, int userId);
      Task<bool> CancelOrderAsync(int orderId, int userId);
}
}
