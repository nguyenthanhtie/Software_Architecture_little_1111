using Final_VS1.Data;
using Final_VS1.Repositories;

namespace Final_VS1.Repositories
{
    public class RepositoryFactory : IRepositoryFactory
    {
        private readonly LittleFishBeautyContext _context;
        
        public RepositoryFactory(LittleFishBeautyContext context)
        {
            _context = context;
        }
        
        public ITaiKhoanRepository CreateTaiKhoanRepository()
        {
            return new TaiKhoanRepository(_context);
        }
        
        public ISanPhamRepository CreateSanPhamRepository()
        {
            return new SanPhamRepository(_context);
        }
        
        public IDanhMucRepository CreateDanhMucRepository()
        {
            return new DanhMucRepository(_context);
        }
        
        public IDonHangRepository CreateDonHangRepository()
        {
            return new DonHangRepository(_context);
        }

        public IDiaChiRepository CreateDiaChiRepository()
        {
            return new DiaChiRepository(_context);
        }
    }
}
