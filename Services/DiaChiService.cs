using Final_VS1.Data;
using Final_VS1.Repositories;

namespace Final_VS1.Services
{
    public class DiaChiService : IDiaChiService
    {
        private readonly IDiaChiRepository _diaChiRepository;

        public DiaChiService(IDiaChiRepository diaChiRepository)
        {
            _diaChiRepository = diaChiRepository;
        }

        public async Task<List<DiaChi>> GetAllAddressesAsync()
        {
            return await _diaChiRepository.GetAllAsync();
        }

        public async Task<DiaChi?> GetAddressByIdAsync(int id)
        {
            return await _diaChiRepository.GetByIdAsync(id);
        }

        public async Task<List<DiaChi>> GetAddressesByUserAsync(int userId)
        {
            return await _diaChiRepository.GetByUserAsync(userId);
        }

        public async Task<DiaChi?> GetDefaultAddressByUserAsync(int userId)
        {
            return await _diaChiRepository.GetDefaultByUserAsync(userId);
        }

        public async Task<DiaChi> CreateAddressAsync(DiaChi diaChi)
        {
            if (diaChi == null)
                throw new ArgumentNullException(nameof(diaChi));

            if (string.IsNullOrWhiteSpace(diaChi.DiaChi1))
                throw new ArgumentException("Địa chỉ không được để trống", nameof(diaChi.DiaChi1));

            if (string.IsNullOrWhiteSpace(diaChi.SoDienThoai))
                throw new ArgumentException("Số điện thoại không được để trống", nameof(diaChi.SoDienThoai));

            return await _diaChiRepository.CreateAsync(diaChi);
        }

        public async Task<DiaChi> UpdateAddressAsync(DiaChi diaChi)
        {
            if (diaChi == null)
                throw new ArgumentNullException(nameof(diaChi));

            if (string.IsNullOrWhiteSpace(diaChi.DiaChi1))
                throw new ArgumentException("Địa chỉ không được để trống", nameof(diaChi.DiaChi1));

            if (string.IsNullOrWhiteSpace(diaChi.SoDienThoai))
                throw new ArgumentException("Số điện thoại không được để trống", nameof(diaChi.SoDienThoai));

            return await _diaChiRepository.UpdateAsync(diaChi);
        }

        public async Task<bool> DeleteAddressAsync(int id)
        {
            return await _diaChiRepository.DeleteAsync(id);
        }

        public async Task<bool> SetDefaultAddressAsync(int id, int userId)
        {
            return await _diaChiRepository.SetDefaultAsync(id, userId);
        }

        public async Task<DiaChi> CreateAddressFromOrderDataAsync(int userId, string hoTen, string soDienThoai, string diaChi)
        {
            if (string.IsNullOrWhiteSpace(hoTen))
                throw new ArgumentException("Họ tên không được để trống", nameof(hoTen));

            if (string.IsNullOrWhiteSpace(soDienThoai))
                throw new ArgumentException("Số điện thoại không được để trống", nameof(soDienThoai));

            if (string.IsNullOrWhiteSpace(diaChi))
                throw new ArgumentException("Địa chỉ không được để trống", nameof(diaChi));

            var newAddress = new DiaChi
            {
                IdTaiKhoan = userId,
                HoTenNguoiNhan = hoTen.Trim(),
                SoDienThoai = soDienThoai.Trim(),
                DiaChi1 = diaChi.Trim(),
                MacDinh = false,
                NgayTao = DateTime.Now
            };

            return await _diaChiRepository.CreateAsync(newAddress);
        }

        // Additional methods for controller compatibility
        public async Task<List<DiaChi>> GetByTaiKhoanIdAsync(int taiKhoanId)
        {
            return await GetAddressesByUserAsync(taiKhoanId);
        }

        public async Task<DiaChi?> GetByIdAsync(int id)
        {
            return await GetAddressByIdAsync(id);
        }

        public async Task<DiaChi> CreateAsync(DiaChi diaChi)
        {
            return await CreateAddressAsync(diaChi);
        }

        public async Task<DiaChi> UpdateAsync(DiaChi diaChi)
        {
            return await UpdateAddressAsync(diaChi);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await DeleteAddressAsync(id);
        }

        public async Task ClearDefaultAddressAsync(int taiKhoanId)
        {
            await _diaChiRepository.ClearDefaultAsync(taiKhoanId);
        }
    }
}
