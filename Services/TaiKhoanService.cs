using Final_VS1.Data;
using Final_VS1.Repositories;
using Final_VS1.Services;

namespace Final_VS1.Services
{
    public class TaiKhoanService : ITaiKhoanService
    {
        private readonly ITaiKhoanRepository _taiKhoanRepository;

        public TaiKhoanService(ITaiKhoanRepository taiKhoanRepository)
        {
            _taiKhoanRepository = taiKhoanRepository;
        }

        public async Task<List<TaiKhoan>> GetAllAsync()
        {
            return await _taiKhoanRepository.GetAllAsync();
        }

        public async Task<TaiKhoan?> GetByIdAsync(int id)
        {
            return await _taiKhoanRepository.GetByIdAsync(id);
        }

        public async Task<TaiKhoan?> GetByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            return await _taiKhoanRepository.GetByEmailAsync(email);
        }

        public async Task<TaiKhoan?> GetByUsernameOrEmailAsync(string usernameOrEmail)
        {
            if (string.IsNullOrWhiteSpace(usernameOrEmail))
                return null;

            return await _taiKhoanRepository.GetByUsernameOrEmailAsync(usernameOrEmail);
        }

        public async Task<TaiKhoan> CreateAsync(TaiKhoan taiKhoan)
        {
            if (taiKhoan == null)
                throw new ArgumentNullException(nameof(taiKhoan));

            if (string.IsNullOrWhiteSpace(taiKhoan.Email))
                throw new ArgumentException("Email là bắt buộc", nameof(taiKhoan.Email));

            if (await _taiKhoanRepository.IsEmailExistsAsync(taiKhoan.Email))
                throw new InvalidOperationException("Email đã tồn tại trong hệ thống");

            // Set default values
            taiKhoan.NgayTao = DateTime.Now;
            taiKhoan.TrangThai = false; // Default to inactive, needs email confirmation
            
            if (string.IsNullOrWhiteSpace(taiKhoan.VaiTro))
                taiKhoan.VaiTro = "Khach";

            await _taiKhoanRepository.AddAsync(taiKhoan);
            return taiKhoan;
        }

        public async Task UpdateAsync(TaiKhoan taiKhoan)
        {
            if (taiKhoan == null)
                throw new ArgumentNullException(nameof(taiKhoan));

            var existingAccount = await _taiKhoanRepository.GetByIdAsync(taiKhoan.IdTaiKhoan);
            if (existingAccount == null)
                throw new InvalidOperationException("Tài khoản không tồn tại");

            if (await _taiKhoanRepository.IsEmailExistsAsync(taiKhoan.Email, taiKhoan.IdTaiKhoan))
                throw new InvalidOperationException("Email đã được sử dụng bởi tài khoản khác");

            await _taiKhoanRepository.UpdateAsync(taiKhoan);
        }

        public async Task DeleteAsync(int id)
        {
            var taiKhoan = await _taiKhoanRepository.GetByIdAsync(id);
            if (taiKhoan == null)
                throw new InvalidOperationException("Tài khoản không tồn tại");

            // Check if account has any orders before deletion
            if (taiKhoan.DonHangs != null && taiKhoan.DonHangs.Any())
                throw new InvalidOperationException("Không thể xóa tài khoản có đơn hàng");

            await _taiKhoanRepository.DeleteAsync(id);
        }

        public async Task<bool> IsEmailExistsAsync(string email, int? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            return await _taiKhoanRepository.IsEmailExistsAsync(email, excludeId);
        }

        public Task<bool> ValidatePasswordAsync(TaiKhoan taiKhoan, string password)
        {
            if (taiKhoan == null || string.IsNullOrWhiteSpace(password))
                return Task.FromResult(false);

            return Task.FromResult(BCrypt.Net.BCrypt.Verify(password, taiKhoan.MatKhau));
        }

        public async Task<bool> ActivateAccountAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            var taiKhoan = await _taiKhoanRepository.GetByEmailAsync(email);
            if (taiKhoan == null)
                return false;

            taiKhoan.TrangThai = true;
            await _taiKhoanRepository.UpdateAsync(taiKhoan);
            return true;
        }

        public async Task<bool> ChangePasswordAsync(int userId, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(newPassword))
                return false;

            var taiKhoan = await _taiKhoanRepository.GetByIdAsync(userId);
            if (taiKhoan == null)
                return false;

            taiKhoan.MatKhau = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _taiKhoanRepository.UpdateAsync(taiKhoan);
            return true;
        }

        public async Task<bool> ResetPasswordAsync(string email, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(newPassword))
                return false;

            var taiKhoan = await _taiKhoanRepository.GetByEmailAsync(email);
            if (taiKhoan == null)
                return false;

            taiKhoan.MatKhau = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _taiKhoanRepository.UpdateAsync(taiKhoan);
            return true;
        }

        public async Task<List<TaiKhoan>> GetUsersByRoleAsync(string role)
        {
            if (string.IsNullOrWhiteSpace(role))
                return new List<TaiKhoan>();

            return await _taiKhoanRepository.GetByRoleAsync(role);
        }

        public async Task<int> GetTotalUsersCountAsync()
        {
            return await _taiKhoanRepository.GetTotalCountAsync();
        }

        public async Task<int> GetActiveUsersCountAsync()
        {
            return await _taiKhoanRepository.GetActiveCountAsync();
        }
    }
}
