
using System;
using QuanLyNhaHang.Models;
using QuanLyNhaHang.Repositories;

namespace QuanLyNhaHang.Services
{
    public class DashboardTrangChuService
    {
        private readonly DashboardTrangChuRepository repository =
            new DashboardTrangChuRepository();

        // Lấy dữ liệu Dashboard trang chủ theo ngày.
        public ThongKeTrangChuDashboard LayDuLieuDashboard(DateTime ngay)
        {
            return repository.LayDuLieuDashboard(ngay);
        }
    }
}
