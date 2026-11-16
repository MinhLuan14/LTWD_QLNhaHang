using System;

namespace QuanLyNhaHang.Models
{
    public class KhachHang
    {
        public int MaKhachHang { get; set; }

        public string HoVaTenLot { get; set; }

        public string TenKhachHang { get; set; }

        public string SoDienThoai { get; set; }

        public string DiaChi { get; set; }

        public int? MaTaiKhoan { get; set; }

        public string HoTen
        {
            get
            {
                return $"{HoVaTenLot} {TenKhachHang}".Trim();
            }
        }
    }
}