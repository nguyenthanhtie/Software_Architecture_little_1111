using Final_VS1.Data;

namespace Final_VS1.Areas.KhachHang.ViewModels
{
    public class DonHangViewModel
    {
        public List<DonHang> DonHangs { get; set; } = new List<DonHang>();
        public string? CurrentFilter { get; set; }
        public int TotalOrders { get; set; }
        public int PendingOrders { get; set; } // "Chờ xác nhận" - có thể hủy
        public int ProcessingOrders { get; set; } // "Đang xử lý" 
        public int ConfirmedOrders { get; set; } // "Đã xác nhận"
        public int ShippingOrders { get; set; } // "Đang giao"
        public int DeliveredOrders { get; set; } // "Hoàn thành"
        public int CancelledOrders { get; set; } // "Đã hủy"
    }
}