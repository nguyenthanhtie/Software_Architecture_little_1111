using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Final_VS1.Data
{
    public partial class ChiTietDonHang
    {
        [Key]
        public int IdChiTiet { get; set; }
        
        public int? IdDonHang { get; set; }
        
        public int? IdSanPham { get; set; }
        
        public int? SoLuong { get; set; }
        
        [Column(TypeName = "decimal(18,0)")]
        public decimal? GiaLucDat { get; set; }
        
        [ForeignKey("IdDonHang")]
        [InverseProperty("ChiTietDonHangs")]
        public virtual DonHang? IdDonHangNavigation { get; set; }
        
        [ForeignKey("IdSanPham")]
        [InverseProperty("ChiTietDonHangs")]
        public virtual SanPham? IdSanPhamNavigation { get; set; }
    }
}