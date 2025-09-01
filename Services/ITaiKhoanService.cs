using Final_VS1.Data;

namespace Final_VS1.Services
{
    public interface ITaiKhoanService
    {
        Task<List<TaiKhoan>> GetAllAsync();
        Task<TaiKhoan?> GetByIdAsync(int id);
        Task<TaiKhoan?> GetByEmailAsync(string email);
        Task<TaiKhoan?> GetByUsernameOrEmailAsync(string usernameOrEmail);
        Task<TaiKhoan> CreateAsync(TaiKhoan taiKhoan);
        Task UpdateAsync(TaiKhoan taiKhoan);
        Task DeleteAsync(int id);
        Task<bool> IsEmailExistsAsync(string email, int? excludeId = null);
        Task<bool> ValidatePasswordAsync(TaiKhoan taiKhoan, string password);
        Task<bool> ActivateAccountAsync(string email);
        Task<bool> ChangePasswordAsync(int userId, string newPassword);
        Task<bool> ResetPasswordAsync(string email, string newPassword);
        Task<List<TaiKhoan>> GetUsersByRoleAsync(string role);
        Task<int> GetTotalUsersCountAsync();
        Task<int> GetActiveUsersCountAsync();
    }
}
