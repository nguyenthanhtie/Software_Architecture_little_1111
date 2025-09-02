using Final_VS1.Data;

namespace Final_VS1.Services
{
    public interface IDiaChiService
    {
        Task<List<DiaChi>> GetAllAddressesAsync();
        Task<DiaChi?> GetAddressByIdAsync(int id);
        Task<List<DiaChi>> GetAddressesByUserAsync(int userId);
        Task<DiaChi?> GetDefaultAddressByUserAsync(int userId);
        Task<DiaChi> CreateAddressAsync(DiaChi diaChi);
        Task<DiaChi> UpdateAddressAsync(DiaChi diaChi);
        Task<bool> DeleteAddressAsync(int id);
        Task<bool> SetDefaultAddressAsync(int id, int userId);
        Task<DiaChi> CreateAddressFromOrderDataAsync(int userId, string hoTen, string soDienThoai, string diaChi);
        
        // Additional methods for controller compatibility
        Task<List<DiaChi>> GetByTaiKhoanIdAsync(int taiKhoanId);
        Task<DiaChi?> GetByIdAsync(int id);
        Task<DiaChi> CreateAsync(DiaChi diaChi);
        Task<DiaChi> UpdateAsync(DiaChi diaChi);
        Task<bool> DeleteAsync(int id);
        Task ClearDefaultAddressAsync(int taiKhoanId);
    }
}
