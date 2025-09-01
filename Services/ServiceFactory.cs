using Final_VS1.Services;

namespace Final_VS1.Services
{
    public class ServiceFactory : IServiceFactory
    {
        private readonly IServiceProvider _serviceProvider;
        
        public ServiceFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        
        public ITaiKhoanService CreateTaiKhoanService()
        {
            return _serviceProvider.GetRequiredService<ITaiKhoanService>();
        }
        
        public ISanPhamService CreateSanPhamService()
        {
            return _serviceProvider.GetRequiredService<ISanPhamService>();
        }
        
        public IDanhMucService CreateDanhMucService()
        {
            return _serviceProvider.GetRequiredService<IDanhMucService>();
        }
        
        public IDonHangService CreateDonHangService()
        {
            return _serviceProvider.GetRequiredService<IDonHangService>();
        }
        
        public IEmailService CreateEmailService()
        {
            return _serviceProvider.GetRequiredService<IEmailService>();
        }
    }
}
