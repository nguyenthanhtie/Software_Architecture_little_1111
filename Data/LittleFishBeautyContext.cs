using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Final_VS1.Data;

public partial class LittleFishBeautyContext : DbContext
{
    public LittleFishBeautyContext()
    {
    }

    public LittleFishBeautyContext(DbContextOptions<LittleFishBeautyContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AnhSanPham> AnhSanPhams { get; set; }

    public virtual DbSet<ChiTietDonHang> ChiTietDonHangs { get; set; }

    public virtual DbSet<DanhMuc> DanhMucs { get; set; }

    public virtual DbSet<DiaChi> DiaChis { get; set; }

    public virtual DbSet<DonHang> DonHangs { get; set; }

    public virtual DbSet<SanPham> SanPhams { get; set; }

    public virtual DbSet<TaiKhoan> TaiKhoans { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=BIG-LULU\\SQLEXPRESS02;Initial Catalog=LittleFishBeauty;Integrated Security=True;Encrypt=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AnhSanPham>(entity =>
        {
            entity.HasKey(e => e.IdAnh).HasName("PK__AnhSanPh__2A42605D69CF0C15");

            entity.ToTable("AnhSanPham");

            entity.Property(e => e.IdAnh).HasColumnName("ID_Anh");
            entity.Property(e => e.DuongDan).HasMaxLength(255);
            entity.Property(e => e.IdSanPham).HasColumnName("ID_SanPham");
            entity.Property(e => e.LoaiAnh).HasMaxLength(20);

            entity.HasOne(d => d.IdSanPhamNavigation).WithMany(p => p.AnhSanPhams)
                .HasForeignKey(d => d.IdSanPham)
                .HasConstraintName("FK__AnhSanPha__ID_Sa__47DBAE45");
        });

        modelBuilder.Entity<ChiTietDonHang>(entity =>
        {
            entity.HasKey(e => e.IdChiTiet).HasName("PK__ChiTietD__1EF2F705CC1A5092");

            entity.ToTable("ChiTietDonHang");

            entity.Property(e => e.IdChiTiet).HasColumnName("ID_ChiTiet");
            entity.Property(e => e.GiaLucDat).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.IdDonHang).HasColumnName("ID_DonHang");
            entity.Property(e => e.IdSanPham).HasColumnName("ID_SanPham");

            entity.HasOne(d => d.IdDonHangNavigation).WithMany(p => p.ChiTietDonHangs)
                .HasForeignKey(d => d.IdDonHang)
                .HasConstraintName("FK__ChiTietDo__ID_Do__5535A963");

            entity.HasOne(d => d.IdSanPhamNavigation).WithMany(p => p.ChiTietDonHangs)
                .HasForeignKey(d => d.IdSanPham)
                .HasConstraintName("FK__ChiTietDo__ID_Sa__5629CD9C");
        });

        modelBuilder.Entity<DanhMuc>(entity =>
        {
            entity.HasKey(e => e.IdDanhMuc).HasName("PK__DanhMuc__662ACB01C518EE95");

            entity.ToTable("DanhMuc");

            entity.Property(e => e.IdDanhMuc).HasColumnName("ID_DanhMuc");
            entity.Property(e => e.AnhDaiDien).HasMaxLength(255);
            entity.Property(e => e.DuongDanSeo)
                .HasMaxLength(255)
                .HasColumnName("DuongDanSEO");
            entity.Property(e => e.IdDanhMucCha).HasColumnName("ID_DanhMucCha");
            entity.Property(e => e.MoTa).HasMaxLength(255);
            entity.Property(e => e.TenDanhMuc).HasMaxLength(100);
            entity.Property(e => e.ThuTuHienThi).HasDefaultValue(0);

            entity.HasOne(d => d.IdDanhMucChaNavigation).WithMany(p => p.InverseIdDanhMucChaNavigation)
                .HasForeignKey(d => d.IdDanhMucCha)
                .HasConstraintName("FK__DanhMuc__ID_Danh__3E52440B");
        });

        modelBuilder.Entity<DiaChi>(entity =>
        {
            entity.HasKey(e => e.IdDiaChi).HasName("PK__DiaChi__C793F252A34FCAEB");

            entity.ToTable("DiaChi");

            entity.Property(e => e.IdDiaChi).HasColumnName("ID_DiaChi");
            entity.Property(e => e.DiaChi1)
                .HasMaxLength(255)
                .HasColumnName("DiaChi");
            entity.Property(e => e.HoTenNguoiNhan).HasMaxLength(100);
            entity.Property(e => e.IdTaiKhoan).HasColumnName("ID_TaiKhoan");
            entity.Property(e => e.MacDinh).HasDefaultValue(false);
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.SoDienThoai).HasMaxLength(20);

            entity.HasOne(d => d.IdTaiKhoanNavigation).WithMany(p => p.DiaChis)
                .HasForeignKey(d => d.IdTaiKhoan)
                .HasConstraintName("FK__DiaChi__ID_TaiKh__4BAC3F29");
        });

        modelBuilder.Entity<DonHang>(entity =>
        {
            entity.HasKey(e => e.IdDonHang).HasName("PK__DonHang__99B72639CEDAF2F1");

            entity.ToTable("DonHang");

            entity.Property(e => e.IdDonHang).HasColumnName("ID_DonHang");
            entity.Property(e => e.IdDiaChi).HasColumnName("ID_DiaChi");
            entity.Property(e => e.IdTaiKhoan).HasColumnName("ID_TaiKhoan");
            entity.Property(e => e.NgayDat)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PhuongThucThanhToan).HasMaxLength(50);
            entity.Property(e => e.TongTien).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.TrangThai).HasMaxLength(50);

            entity.HasOne(d => d.IdDiaChiNavigation).WithMany(p => p.DonHangs)
                .HasForeignKey(d => d.IdDiaChi)
                .HasConstraintName("FK__DonHang__ID_DiaC__5165187F");

            entity.HasOne(d => d.IdTaiKhoanNavigation).WithMany(p => p.DonHangs)
                .HasForeignKey(d => d.IdTaiKhoan)
                .HasConstraintName("FK__DonHang__ID_TaiK__5070F446");
        });

        modelBuilder.Entity<SanPham>(entity =>
        {
            entity.HasKey(e => e.IdSanPham).HasName("PK__SanPham__617EA392DFDFCCD3");

            entity.ToTable("SanPham");

            entity.HasIndex(e => e.Sku, "UQ__SanPham__CA1ECF0D89F1D858").IsUnique();

            entity.Property(e => e.IdSanPham).HasColumnName("ID_SanPham");
            entity.Property(e => e.GiaBan).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.IdDanhMuc).HasColumnName("ID_DanhMuc");
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Sku)
                .HasMaxLength(50)
                .HasColumnName("SKU");
            entity.Property(e => e.SoLuongTonKho).HasDefaultValue(0);
            entity.Property(e => e.TenSanPham).HasMaxLength(100);
            entity.Property(e => e.TrangThai).HasDefaultValue(true);

            entity.HasOne(d => d.IdDanhMucNavigation).WithMany(p => p.SanPhams)
                .HasForeignKey(d => d.IdDanhMuc)
                .HasConstraintName("FK__SanPham__ID_Danh__440B1D61");
        });

        modelBuilder.Entity<TaiKhoan>(entity =>
        {
            entity.HasKey(e => e.IdTaiKhoan).HasName("PK__TaiKhoan__0E3EC2102AD18E5C");

            entity.ToTable("TaiKhoan");

            entity.HasIndex(e => e.Email, "UQ__TaiKhoan__A9D105347602237C").IsUnique();

            entity.Property(e => e.IdTaiKhoan).HasColumnName("ID_TaiKhoan");
            entity.Property(e => e.AnhDaiDien).HasMaxLength(255);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.GioiTinh).HasMaxLength(10);
            entity.Property(e => e.HoTen).HasMaxLength(100);
            entity.Property(e => e.MatKhau).HasMaxLength(255);
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TrangThai).HasDefaultValue(true);
            entity.Property(e => e.VaiTro).HasMaxLength(20);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
