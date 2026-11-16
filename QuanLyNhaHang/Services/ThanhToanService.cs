using System.Collections.Generic;
using QuanLyNhaHang.Models;
using QuanLyNhaHang.Repositories;

namespace QuanLyNhaHang.Services
{
    public class ThanhToanService
    {
        private readonly ThanhToanRepository repository =
            new ThanhToanRepository();

        // 1. LẤY DANH SÁCH THANH TOÁN
        public List<ThanhToan> GetDanhSachThanhToan(
            string tuKhoa = "",
            string phuongThuc = "",
            string trangThai = "")
        {
            return repository.GetDanhSachThanhToan(
                tuKhoa,
                phuongThuc,
                trangThai
            );
        }

        // 2. LẤY CHI TIẾT THANH TOÁN
        public ThanhToan GetChiTietThanhToan(int maThanhToan)
        {
            return repository.GetChiTietThanhToan(maThanhToan);
        }

        // 3. LẤY THỐNG KÊ THANH TOÁN
        public ThongKeThanhToan GetThongKeThanhToan()
        {
            return repository.GetThongKeThanhToan();
        }

        // 4. XÁC NHẬN THANH TOÁN CHUYỂN KHOẢN
        public bool XacNhanThanhToan(int maThanhToan)
        {
            return repository.XacNhanThanhToan(maThanhToan);
        }

        // 5. TỪ CHỐI THANH TOÁN CHUYỂN KHOẢN
        public bool TuChoiThanhToan(int maThanhToan)
        {
            return repository.TuChoiThanhToan(maThanhToan);
        }
    }
}