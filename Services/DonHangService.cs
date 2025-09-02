using Final_VS1.Data;
using Final_VS1.Repositories;
using Final_VS1.Services;

namespace Final_VS1.Services
{
    public class DonHangService : IDonHangService
    {
        private readonly IDonHangRepository _donHangRepository;
        private readonly IDiaChiService _diaChiService;

        public DonHangService(IDonHangRepository donHangRepository, IDiaChiService diaChiService)
        {
            _donHangRepository = donHangRepository;
            _diaChiService = diaChiService;
        }

        public async Task<List<DonHang>> GetAllOrdersAsync()
        {
            return await _donHangRepository.GetAllAsync();
        }

        public async Task<DonHang?> GetOrderByIdAsync(int id)
        {
            return await _donHangRepository.GetByIdAsync(id);
        }

        public async Task<DonHang?> GetOrderByIdAndUserAsync(int orderId, int userId)
        {
            return await _donHangRepository.GetByIdAndUserAsync(orderId, userId);
        }

        public async Task<DonHang> CreateOrderAsync(DonHang donHang, List<ChiTietDonHang> chiTietDonHangs)
        {
            if (donHang == null)
                throw new ArgumentNullException(nameof(donHang));

            if (chiTietDonHangs == null || chiTietDonHangs.Count == 0)
                throw new ArgumentException("Đơn hàng phải có ít nhất một sản phẩm", nameof(chiTietDonHangs));

            // Validate required fields
            if (donHang.IdDiaChi == null || donHang.IdDiaChi <= 0)
                throw new ArgumentException("Địa chỉ giao hàng là bắt buộc", nameof(donHang.IdDiaChi));

            // Set default values
            donHang.NgayDat = DateTime.Now;
            if (string.IsNullOrWhiteSpace(donHang.TrangThai))
                donHang.TrangThai = "Chờ xác nhận";

            // Calculate total amount
            donHang.TongTien = chiTietDonHangs.Sum(ct => (ct.SoLuong ?? 0) * (ct.GiaLucDat ?? 0));

            return await _donHangRepository.CreateOrderAsync(donHang, chiTietDonHangs);
        }

        public async Task<DonHang> CreateOrderWithAddressAsync(int userId, string hoTen, string soDienThoai, string diaChi, string? paymentMethod, List<ChiTietDonHang> chiTietDonHangs)
        {
            if (chiTietDonHangs == null || chiTietDonHangs.Count == 0)
                throw new ArgumentException("Đơn hàng phải có ít nhất một sản phẩm", nameof(chiTietDonHangs));

            // Create address first
            var newAddress = await _diaChiService.CreateAddressFromOrderDataAsync(userId, hoTen, soDienThoai, diaChi);

            // Create order
            var donHang = new DonHang
            {
                IdTaiKhoan = userId,
                IdDiaChi = newAddress.IdDiaChi,
                NgayDat = DateTime.Now,
                TrangThai = "Chờ xác nhận",
                PhuongThucThanhToan = paymentMethod ?? "COD",
                TongTien = chiTietDonHangs.Sum(ct => (ct.SoLuong ?? 0) * (ct.GiaLucDat ?? 0)) + 30000 // Add shipping fee
            };

            return await _donHangRepository.CreateOrderAsync(donHang, chiTietDonHangs);
        }

        public async Task<bool> UpdateOrderStatusAsync(int id, string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                throw new ArgumentException("Trạng thái không được để trống", nameof(status));

            var validStatuses = new[] { "Chờ xác nhận", "Đã xác nhận", "Đang xử lý", "Đang giao", "Hoàn thành", "Đã hủy" };
            if (!validStatuses.Contains(status))
                throw new ArgumentException("Trạng thái không hợp lệ", nameof(status));

            await _donHangRepository.UpdateStatusAsync(id, status);
            return true;
        }

        public async Task<List<DonHang>> GetOrdersByUserAsync(int userId, string? status = null)
        {
            return await _donHangRepository.GetByUserAsync(userId, status);
        }

        public async Task<int> CountOrdersByUserAsync(int userId, string? status = null)
        {
            return await _donHangRepository.CountByUserAsync(userId, status);
        }

        public async Task<bool> CancelOrderAsync(int orderId, int userId)
        {
            if (!await CanCancelOrderAsync(orderId, userId))
                return false;

            return await _donHangRepository.CancelOrderAsync(orderId, userId);
        }

        public async Task<bool> CanCancelOrderAsync(int orderId, int userId)
        {
            var order = await _donHangRepository.GetByIdAndUserAsync(orderId, userId);
            if (order == null)
                return false;

            // Only allow cancellation for orders that are "Chờ xác nhận" or "Đã xác nhận"
            return order.TrangThai == "Chờ xác nhận" || order.TrangThai == "Đã xác nhận";
        }

        public async Task<List<DonHang>> GetOrdersByStatusAsync(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                return new List<DonHang>();

            var allOrders = await _donHangRepository.GetAllAsync();
            return allOrders.Where(o => o.TrangThai == status).ToList();
        }

        public async Task<List<DonHang>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ArgumentException("Ngày bắt đầu không thể lớn hơn ngày kết thúc");

            var allOrders = await _donHangRepository.GetAllAsync();
            return allOrders.Where(o =>
                o.NgayDat.HasValue &&
                o.NgayDat.Value.Date >= startDate.Date &&
                o.NgayDat.Value.Date <= endDate.Date
            ).ToList();
        }

        public async Task<decimal> GetTotalRevenueAsync()
        {
            var completedOrders = await GetOrdersByStatusAsync("Hoàn thành");
            return completedOrders.Sum(o => o.TongTien ?? 0);
        }

        public async Task<decimal> GetRevenueByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var ordersInRange = await GetOrdersByDateRangeAsync(startDate, endDate);
            return ordersInRange.Where(o => o.TrangThai == "Hoàn thành").Sum(o => o.TongTien ?? 0);
        }

        public async Task<int> GetTotalOrdersCountAsync()
        {
            var allOrders = await _donHangRepository.GetAllAsync();
            return allOrders.Count;
        }

        public async Task<List<DonHang>> GetRecentOrdersAsync(int count)
        {
            if (count <= 0)
                return new List<DonHang>();

            var allOrders = await _donHangRepository.GetAllAsync();
            return allOrders
                .OrderByDescending(o => o.NgayDat)
                .Take(count)
                .ToList();
        }
    }
}
