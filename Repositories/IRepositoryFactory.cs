using Final_VS1.Repositories;

namespace Final_VS1.Repositories
{
    public interface IRepositoryFactory
    {
        ITaiKhoanRepository CreateTaiKhoanRepository();
        ISanPhamRepository CreateSanPhamRepository();
        IDanhMucRepository CreateDanhMucRepository();
        IDonHangRepository CreateDonHangRepository();
    }
}
