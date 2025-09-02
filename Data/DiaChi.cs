using System;
using System.Collections.Generic;

namespace Final_VS1.Data;

public partial class DiaChi
{
    public int IdDiaChi { get; set; }

    public int? IdTaiKhoan { get; set; }

    public string DiaChi1 { get; set; } = null!;

    public string SoDienThoai { get; set; } = null!;

    public string? HoTenNguoiNhan { get; set; }

    public bool? MacDinh { get; set; }

    public DateTime? NgayTao { get; set; }

    public virtual ICollection<DonHang> DonHangs { get; set; } = new List<DonHang>();

    public virtual TaiKhoan? IdTaiKhoanNavigation { get; set; }
}
