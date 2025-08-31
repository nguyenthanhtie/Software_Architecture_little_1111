using Final_VS1.Data;

namespace Final_VS1.Repositories
{
    public interface IDonHangRepository
    {
        Task<List<DonHang>> GetAllAsync();
        Task<DonHang?> GetByIdAsync(int id);
        Task UpdateStatusAsync(int id, string status);
    }
}