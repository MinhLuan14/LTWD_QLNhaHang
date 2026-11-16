using System;

namespace QuanLyNhaHang.Models
{
    public class HoaDon
    {
        public int MaHoaDon { get; set; }
        public DateTime NgayLap { get; set; }
        public TimeSpan GioLap { get; set; }
        public decimal TongTien { get; set; }
        public string TrangThai { get; set; }
        public int MaNhanVien { get; set; }
        public string TenNhanVien { get; set; }
        public int? MaKhachHang { get; set; }
        public int? MaPhieuGoiMon { get; set; }
        public int? MaBan { get; set; }
        public int? SoBan { get; set; }
        public decimal GiamGia { get; set; }
        public decimal TienThue { get; set; }
        public string PhuongThucThanhToan { get; set; }
    }
}
