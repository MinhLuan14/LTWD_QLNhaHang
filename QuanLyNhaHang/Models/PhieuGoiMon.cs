using System;

namespace QuanLyNhaHang.Models
{
    public class PhieuGoiMon
    {
        public int MaPhieuGoiMon { get; set; }
        public int MaBan { get; set; }
        public int? MaNhanVien { get; set; }
        public DateTime NgayGoi { get; set; }
        public TimeSpan GioGoi { get; set; }
        public string TrangThai { get; set; }
        public string GhiChu { get; set; }
    }
}
