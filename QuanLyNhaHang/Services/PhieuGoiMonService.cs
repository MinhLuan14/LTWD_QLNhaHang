using System;
using System.Collections.Generic;
using QuanLyNhaHang.Repositories;

namespace QuanLyNhaHang.Services
{
    public class PhieuGoiMonService
    {
        private readonly PhieuGoiMonRepository repository =
            new PhieuGoiMonRepository();

        // 1. Lưu gọi món
        public int LuuGoiMon(
            int maBan,
            int? maNhanVien,
            Dictionary<int, int> gioHang,
            int? maDatBan = null)
        {
            if (maBan <= 0)
                throw new Exception("Vui lòng chọn bàn hợp lệ.");

            if (maNhanVien.HasValue && maNhanVien.Value <= 0)
                throw new Exception("Mã nhân viên không hợp lệ.");

            if (gioHang == null || gioHang.Count == 0)
                throw new Exception("Giỏ hàng chưa có món ăn.");

            foreach (var item in gioHang)
            {
                if (item.Key <= 0 || item.Value <= 0)
                    throw new Exception("Mã món ăn hoặc số lượng không hợp lệ.");
            }

            return repository.LuuGoiMon(
                maBan,
                maNhanVien,
                gioHang,
                maDatBan);
        }

        // 2. Lấy mã phiếu đang mở
        public int GetMaPhieuDangMo(
            int maBan,
            int? maDatBan = null)
        {
            if (maBan <= 0)
                return 0;

            return repository.GetMaPhieuDangMo(
                maBan,
                maDatBan);
        }

        // 3. Lấy chi tiết phiếu gọi món
        public Dictionary<int, int> GetChiTietPhieuGoiMon(
            int maPhieuGoiMon)
        {
            if (maPhieuGoiMon <= 0)
                return new Dictionary<int, int>();

            return repository.GetChiTietPhieuGoiMon(
                maPhieuGoiMon);
        }

        // 4. Cập nhật số lượng món
        public void CapNhatSoLuongMonTrongPhieu(
            int maBan,
            int maMonAn,
            int soLuongMoi,
            int? maDatBan = null)
        {
            if (maBan <= 0 || maMonAn <= 0 || soLuongMoi <= 0)
                throw new Exception("Thông tin cập nhật không hợp lệ.");

            repository.CapNhatSoLuongMonTrongPhieu(
                maBan,
                maMonAn,
                soLuongMoi,
                maDatBan);
        }

        // 5. Xóa món khỏi phiếu
        public bool XoaMonKhoiPhieu(
            int maBan,
            int maMonAn,
            int? maDatBan = null)
        {
            if (maBan <= 0 || maMonAn <= 0)
                throw new Exception("Thông tin bàn hoặc món ăn không hợp lệ.");

            return repository.XoaMonKhoiPhieu(
                maBan,
                maMonAn,
                maDatBan);
        }
    }
}
