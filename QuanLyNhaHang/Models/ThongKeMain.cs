
using System;

namespace QuanLyNhaHang.Models
{
    // 1. Thống kê tổng quan
    public class ThongKeTongQuan
    {
        public decimal TongDoanhThu { get; set; }
        public int SoHoaDon { get; set; }
        public decimal TrungBinhHoaDon { get; set; }
    }

    // 2. Doanh thu theo ngày
    public class ThongKeDoanhThuNgay
    {
        public DateTime Ngay { get; set; }
        public decimal DoanhThu { get; set; }
    }

    // 3. Top món bán chạy
    public class ThongKeMonBanChay
    {
        public string TenMon { get; set; }
        public int SoLuong { get; set; }
        public decimal DoanhThu { get; set; }
    }

    // 4. Thống kê số hóa đơn theo khung giờ
    public class ThongKeKhungGio
    {
        public string KhungGio { get; set; }
        public int SoHoaDon { get; set; }
    }
}
