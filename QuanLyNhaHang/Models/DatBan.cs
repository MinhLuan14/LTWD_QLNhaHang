using System;
using System.Collections.Generic;

namespace QuanLyNhaHang.Models
{
    public class DatBan
    {
        // Thông tin phiếu đặt bàn
        public int MaDatBan { get; set; }

        public DateTime NgayDat { get; set; }

        public TimeSpan GioDat { get; set; }

        public int SoLuongKhach { get; set; }

        public string TrangThai { get; set; }

        public string GhiChu { get; set; }

        public int MaKhachHang { get; set; }

        // Thông tin khách hàng phục vụ hiển thị
        public string TenKhachHang { get; set; }

        public string SoDienThoai { get; set; }

        // Danh sách bàn thuộc phiếu đặt
        public List<ChiTietDatBan> DanhSachBan { get; set; }
            = new List<ChiTietDatBan>();

        // Tổng sức chứa của các bàn được chọn
        public int TongSoChoNgoi
        {
            get
            {
                if (DanhSachBan == null)
                    return 0;

                int tong = 0;

                foreach (ChiTietDatBan ban in DanhSachBan)
                {
                    if (ban != null)
                        tong += ban.soChoNgoi;
                }

                return tong;
            }
        }

        // Kiểm tra sức chứa có đáp ứng số lượng khách không
        public bool DuSucChua
        {
            get
            {
                return TongSoChoNgoi >= SoLuongKhach;
            }
        }
        public string DiaChi { get; set; }
    }
}