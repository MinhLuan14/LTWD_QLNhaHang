
using System;
using System.Collections.Generic;
using QuanLyNhaHang.Repositories;
using QuanLyNhaHang.Models;

namespace QuanLyNhaHang.Services
{
    public class HoaDonService
    {
        private readonly HoaDonRepository hoaDonRepository =
            new HoaDonRepository();

        // 1. LẬP HÓA ĐƠN TỪ PHIẾU GỌI MÓN
        public int LapHoaDon(int maPhieuGoiMon, int maNhanVien)
        {
            if (maPhieuGoiMon <= 0)
                throw new Exception("Mã phiếu gọi món không hợp lệ.");

            if (maNhanVien <= 0)
                throw new Exception("Không xác định được nhân viên lập hóa đơn.");

            int maHoaDon = hoaDonRepository.TaoHoaDonTuPhieuGoiMon(
                maPhieuGoiMon,
                maNhanVien
            );

            if (maHoaDon <= 0)
                throw new Exception("Không thể lập hóa đơn.");

            return maHoaDon;
        }

        // 2. LẤY DANH SÁCH HÓA ĐƠN
        public List<HoaDon> GetDanhSachHoaDon(
            DateTime tuNgay,
            DateTime denNgay,
            string trangThai = null,
            string keyword = null)
        {
            if (tuNgay.Date > denNgay.Date)
                throw new Exception("Ngày bắt đầu không được lớn hơn ngày kết thúc.");

            return hoaDonRepository.GetDanhSachHoaDon(
                tuNgay,
                denNgay,
                trangThai,
                keyword
            );
        }

        // 3. LẤY CHI TIẾT HÓA ĐƠN
        public List<ChiTietHoaDon> GetChiTietHoaDon(int maHoaDon)
        {
            if (maHoaDon <= 0)
                throw new Exception("Mã hóa đơn không hợp lệ.");

            return hoaDonRepository.GetChiTietHoaDon(maHoaDon);
        }

        // 4. THANH TOÁN HÓA ĐƠN ĐÃ LẬP
        public bool ThanhToanHoaDon(
            int maHoaDon,
            decimal giamGia,
            string phuongThuc)
        {
            if (maHoaDon <= 0)
                throw new Exception("Mã hóa đơn không hợp lệ.");

            if (giamGia < 0)
                throw new Exception("Giảm giá không được nhỏ hơn 0.");

            if (string.IsNullOrWhiteSpace(phuongThuc))
                phuongThuc = "Tiền mặt";

            return hoaDonRepository.ThanhToanHoaDon(
                maHoaDon,
                giamGia,
                phuongThuc
            );
        }

        // 5. THANH TOÁN TRỰC TIẾP THEO BÀN
        public bool ThanhToan(
            int maBan,
            int maNhanVien,
            decimal giamGia,
            string phuongThuc,
            out int maHoaDonMoi)
        {
            if (maBan <= 0)
                throw new Exception("Mã bàn không hợp lệ.");

            if (maNhanVien <= 0)
                throw new Exception("Không xác định được nhân viên thanh toán.");

            if (giamGia < 0)
                throw new Exception("Giảm giá không được nhỏ hơn 0.");

            if (string.IsNullOrWhiteSpace(phuongThuc))
                phuongThuc = "Tiền mặt";

            return hoaDonRepository.ThanhToan(
                maBan,
                maNhanVien,
                giamGia,
                phuongThuc,
                out maHoaDonMoi
            );
        }
    }
}
