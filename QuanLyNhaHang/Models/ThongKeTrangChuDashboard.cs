
using System.Data;

namespace QuanLyNhaHang.Models
{
    public class ThongKeTrangChuDashboard
    {
        // Tổng doanh thu đã thanh toán trong ngày.
        public decimal TongDoanhThu { get; set; }

        // Tổng số hóa đơn được lập trong ngày.
        public int TongHoaDon { get; set; }

        // Số bàn hiện đang trống.
        public int BanTrong { get; set; }

        // Số bàn hiện đang phục vụ.
        public int DangPhucVu { get; set; }

        // Số hóa đơn chưa thanh toán.
        public int HoaDonCanXuLy { get; set; }

        // Số phiếu gọi món đang chờ xử lý.
        public int PhieuGoiMonCanXuLy { get; set; }

        // Số phiếu đặt bàn đang chờ xác nhận.
        public int DatBanCanXuLy { get; set; }

        // Tổng số công việc cần xử lý.
        public int TongViecCanXuLy { get; set; }

        // Danh sách hóa đơn gần đây.
        public DataTable HoaDonGanDay { get; set; }

        // Doanh thu theo từng giờ trong ngày.
        public DataTable DoanhThuTheoGio { get; set; }

        // Số lượng bàn theo từng trạng thái.
        public DataTable TinhTrangBan { get; set; }
    }
}
