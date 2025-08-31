using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Final_VS1.Migrations
{
    /// <inheritdoc />
    public partial class InitialSync : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            

          
            
            migrationBuilder.CreateIndex(
                name: "IX_AnhSanPham_ID_SanPham",
                table: "AnhSanPham",
                column: "ID_SanPham");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDonHang_ID_DonHang",
                table: "ChiTietDonHang",
                column: "ID_DonHang");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDonHang_ID_SanPham",
                table: "ChiTietDonHang",
                column: "ID_SanPham");

            migrationBuilder.CreateIndex(
                name: "IX_DanhMuc_ID_DanhMucCha",
                table: "DanhMuc",
                column: "ID_DanhMucCha");

            migrationBuilder.CreateIndex(
                name: "IX_DonHang_ID_TaiKhoan",
                table: "DonHang",
                column: "ID_TaiKhoan");

            migrationBuilder.CreateIndex(
                name: "IX_SanPham_ID_DanhMuc",
                table: "SanPham",
                column: "ID_DanhMuc");

            migrationBuilder.CreateIndex(
                name: "UQ__SanPham__CA1ECF0D06E88054",
                table: "SanPham",
                column: "SKU",
                unique: true,
                filter: "[SKU] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UQ__TaiKhoan__A9D10534BAF084CD",
                table: "TaiKhoan",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnhSanPham");

            migrationBuilder.DropTable(
                name: "ChiTietDonHang");

            migrationBuilder.DropTable(
                name: "DonHang");

            migrationBuilder.DropTable(
                name: "SanPham");

            migrationBuilder.DropTable(
                name: "TaiKhoan");

            migrationBuilder.DropTable(
                name: "DanhMuc");
        }
    }
}
