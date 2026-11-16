using System;

namespace QuanLyNhaHang.Models
{
    public class ThanhToan
    {
        public int MaThanhToan { get; set; }
        public int MaHoaDon { get; set; }
        public int? MaPhieuGoiMon { get; set; }

        public int? MaBan { get; set; }
        public string TenBan { get; set; }

        public decimal SoTienThanhToan { get; set; }
        public string PhuongThucThanhToan { get; set; }

        public DateTime NgayThanhToan { get; set; }
        public DateTime GioThanhToan { get; set; }
        public DateTime NgayGioThanhToan { get; set; }

        public string TrangThai { get; set; }
    }
}