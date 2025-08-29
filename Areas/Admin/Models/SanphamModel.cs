namespace Final_VS1.Areas.Admin.Models
{
    public class CreateProductModel
    {
        public required string TenSanPham { get; set; }
        public string? MoTa { get; set; }
        public decimal GiaBan { get; set; }
        public bool TrangThai { get; set; }
        public int IdDanhMuc { get; set; }
        public string? ThanhPhanChinh { get; set; }
        public string? CachSuDung { get; set; }
    }

    public class UpdateProductModel
    {
        public int IdSanPham { get; set; }
        public required string TenSanPham { get; set; }
        public string? MoTa { get; set; }
        public decimal GiaBan { get; set; }
        public bool TrangThai { get; set; }
        public int IdDanhMuc { get; set; }
        public string? ThanhPhanChinh { get; set; }
        public string? CachSuDung { get; set; }
    }
    
}
