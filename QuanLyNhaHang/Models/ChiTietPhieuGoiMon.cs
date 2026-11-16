using System;

namespace QuanLyNhaHang.Models
{
    public class ChiTietPhieuGoiMon
    {
        public int MaChiTietPhieuGoiMon { get; set; }
        public int MaPhieuGoiMon { get; set; }
        public int MaMonAn { get; set; }
        public string TenMonAn { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien => SoLuong * DonGia;
    }
}
