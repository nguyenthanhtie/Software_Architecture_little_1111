using Final_VS1.Data;
using System.ComponentModel.DataAnnotations;

namespace Final_VS1.Areas.KhachHang.ViewModels
{
    public class DiaChiManagementViewModel
    {
        public List<DiaChi> DiaChiList { get; set; } = new List<DiaChi>();
        public TaiKhoan TaiKhoan { get; set; }
    }

    public class DiaChiCreateViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [Display(Name = "Họ tên")]
        public string HoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Display(Name = "Số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string SoDienThoai { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ")]
        [Display(Name = "Địa chỉ")]
        public string DiaChi1 { get; set; } = string.Empty;

        [Display(Name = "Đặt làm địa chỉ mặc định")]
        public bool LaMacDinh { get; set; } = false;
    }

    public class DiaChiEditViewModel
    {
        public int IdDiaChi { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [Display(Name = "Họ tên")]
        public string HoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Display(Name = "Số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string SoDienThoai { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ")]
        [Display(Name = "Địa chỉ")]
        public string DiaChi1 { get; set; } = string.Empty;

        [Display(Name = "Đặt làm địa chỉ mặc định")]
        public bool LaMacDinh { get; set; } = false;
    }
}
