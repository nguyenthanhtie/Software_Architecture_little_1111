using Final_VS1.Services;

namespace Final_VS1.Services
{
    public interface IServiceFactory
    {
        ITaiKhoanService CreateTaiKhoanService();
        ISanPhamService CreateSanPhamService();
        IDanhMucService CreateDanhMucService();
        IDonHangService CreateDonHangService();
        IEmailService CreateEmailService();
        IDiaChiService CreateDiaChiService();
    }
}
