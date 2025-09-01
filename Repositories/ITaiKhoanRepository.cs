using Final_VS1.Data;

namespace Final_VS1.Repositories
{
    public interface ITaiKhoanRepository
    {
        Task<List<TaiKhoan>> GetAllAsync();
        Task<TaiKhoan?> GetByIdAsync(int id);
        Task<TaiKhoan?> GetByEmailAsync(string email);
        Task<TaiKhoan?> GetByUsernameOrEmailAsync(string usernameOrEmail);
        Task AddAsync(TaiKhoan taiKhoan);
        Task UpdateAsync(TaiKhoan taiKhoan);
        Task DeleteAsync(int id);
        Task<bool> IsEmailExistsAsync(string email, int? excludeId = null);
        Task<List<TaiKhoan>> GetByRoleAsync(string role);
        Task<int> GetTotalCountAsync();
        Task<int> GetActiveCountAsync();
    }
}
