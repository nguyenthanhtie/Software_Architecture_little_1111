﻿using System;
using System.Collections.Generic;

namespace Final_VS1.Data;

public partial class ChiTietDonHang
{
    public int IdChiTiet { get; set; }

    public int? IdDonHang { get; set; }

    public int? IdSanPham { get; set; }

    public int? SoLuong { get; set; }

    public decimal? GiaLucDat { get; set; }

    public virtual DonHang? IdDonHangNavigation { get; set; }

    public virtual SanPham? IdSanPhamNavigation { get; set; }
}
