using System;
using System.Collections.Generic;

namespace Final_VS1.Data;

public partial class SanPham
{
    public int IdSanPham { get; set; }

    public string? TenSanPham { get; set; }

    public string? MoTa { get; set; }

    public decimal? GiaBan { get; set; }

    public int? SoLuongTonKho { get; set; }

    public string? Sku { get; set; }

    public bool? TrangThai { get; set; }

    public int? IdDanhMuc { get; set; }

    public string? CachSuDung { get; set; }

    public DateTime? NgayTao { get; set; }

    public virtual ICollection<AnhSanPham> AnhSanPhams { get; set; } = new List<AnhSanPham>();

    public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();

    public virtual DanhMuc? IdDanhMucNavigation { get; set; }
}
