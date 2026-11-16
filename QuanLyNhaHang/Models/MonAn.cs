
using System;

namespace QuanLyNhaHang.Models
{
    public class MonAn
    {
        public int MaMonAn { get; set; }

        public string TenMonAn { get; set; }

        public decimal DonGia { get; set; }

        public DateTime NgayThem { get; set; }

        public bool TrangThai { get; set; }

        public int MaLoaiMon { get; set; }

        public string DonVi { get; set; }

        public string HinhMon { get; set; }

        // Tên danh mục dùng để hiển thị
        public string TenLoaiMon { get; set; }
    }
}
