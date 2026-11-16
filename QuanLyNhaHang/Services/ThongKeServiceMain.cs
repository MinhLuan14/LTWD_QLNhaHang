
using System;
using System.Collections.Generic;

using QuanLyNhaHang.Models;
using QuanLyNhaHang.Repositories;

namespace QuanLyNhaHang.Services
{
    public class ThongKeServiceMain
    {
        private readonly ThongKeRepositoryMain thongKeRepository =
            new ThongKeRepositoryMain();

        // 1. LẤY THỐNG KÊ TỔNG QUAN
        // 1. LẤY THỐNG KÊ TỔNG QUAN
        public ThongKeTongQuan GetThongKeTongQuan(
            DateTime tuNgay,
            DateTime denNgay)
        {
            KiemTraKhoangNgay(tuNgay, denNgay);

            return thongKeRepository.GetThongKeTongQuan(
                tuNgay,
                denNgay);
        }

        // 2. LẤY DOANH THU THEO NGÀY
        public List<ThongKeDoanhThuNgay> GetDoanhThuTheoNgay(
            DateTime tuNgay,
            DateTime denNgay)
        {
            KiemTraKhoangNgay(tuNgay, denNgay);

            return thongKeRepository.GetDoanhThuTheoNgay(
                tuNgay,
                denNgay);
        }

        // 3. LẤY TOP MÓN BÁN CHẠY
        public List<ThongKeMonBanChay> GetTopMonBanChay(
            DateTime tuNgay,
            DateTime denNgay,
            int topN = 5)
        {
            KiemTraKhoangNgay(tuNgay, denNgay);

            if (topN <= 0)
            {
                throw new ArgumentException(
                    "Số lượng món cần thống kê phải lớn hơn 0.",
                    nameof(topN));
            }

            return thongKeRepository.GetTopMonBanChay(
                tuNgay,
                denNgay,
                topN);
        }

        // 4. LẤY THỐNG KÊ KHUNG GIỜ ĐÔNG KHÁCH
        public List<ThongKeKhungGio> GetThongKeKhungGio(
            DateTime tuNgay,
            DateTime denNgay)
        {
            KiemTraKhoangNgay(tuNgay, denNgay);

            return thongKeRepository.GetThongKeKhungGio(
                tuNgay,
                denNgay);
        }

        // 5. KIỂM TRA KHOẢNG NGÀY
        private void KiemTraKhoangNgay(
            DateTime tuNgay,
            DateTime denNgay)
        {
            if (tuNgay.Date > denNgay.Date)
            {
                throw new ArgumentException(
                    "Ngày bắt đầu không được lớn hơn ngày kết thúc.");
            }
        }
    }
}
