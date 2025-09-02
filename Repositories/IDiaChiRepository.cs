using Final_VS1.Data;

namespace Final_VS1.Repositories
{
    public interface IDiaChiRepository
    {
        Task<List<DiaChi>> GetAllAsync();
        Task<DiaChi?> GetByIdAsync(int id);
        Task<List<DiaChi>> GetByUserAsync(int userId);
        Task<DiaChi?> GetDefaultByUserAsync(int userId);
        Task<DiaChi> CreateAsync(DiaChi diaChi);
        Task<DiaChi> UpdateAsync(DiaChi diaChi);
        Task<bool> DeleteAsync(int id);
        Task<bool> SetDefaultAsync(int id, int userId);
        Task ClearDefaultAsync(int userId);
    }
}
