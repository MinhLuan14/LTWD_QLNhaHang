
using System;
using System.Collections.Generic;
using QuanLyNhaHang.Models;
using QuanLyNhaHang.Repositories;

namespace QuanLyNhaHang.Services
{
    public class MonAnService
    {
        private readonly MonAnRepository repository =
            new MonAnRepository();

        // 1. Lấy danh sách món ăn
        public List<MonAn> GetAll()
        {
            return repository.GetAll();
        }

        // 2. Lấy món ăn theo mã
        public MonAn GetById(int maMonAn)
        {
            if (maMonAn <= 0)
                throw new Exception("Mã món ăn không hợp lệ!");

            return repository.GetById(maMonAn);
        }

        // 3. Thêm món ăn
        public bool Them(MonAn monAn)
        {
            if (monAn == null)
                throw new Exception("Thông tin món ăn không hợp lệ!");

            KiemTraDuLieu(monAn);

            monAn.TenMonAn = monAn.TenMonAn.Trim();

            if (!string.IsNullOrWhiteSpace(monAn.DonVi))
                monAn.DonVi = monAn.DonVi.Trim();

            if (!string.IsNullOrWhiteSpace(monAn.HinhMon))
                monAn.HinhMon = monAn.HinhMon.Trim();

            return repository.Them(monAn);
        }

        // 4. Cập nhật món ăn
        public bool Sua(MonAn monAn)
        {
            if (monAn == null || monAn.MaMonAn <= 0)
                throw new Exception("Vui lòng chọn món ăn cần sửa!");

            KiemTraDuLieu(monAn);

            monAn.TenMonAn = monAn.TenMonAn.Trim();

            if (!string.IsNullOrWhiteSpace(monAn.DonVi))
                monAn.DonVi = monAn.DonVi.Trim();

            if (!string.IsNullOrWhiteSpace(monAn.HinhMon))
                monAn.HinhMon = monAn.HinhMon.Trim();

            return repository.Sua(monAn);
        }

        // 5. Ngừng bán / Mở bán
        public bool CapNhatTrangThai(int maMonAn, bool trangThai)
        {
            if (maMonAn <= 0)
                throw new Exception("Vui lòng chọn món ăn!");

            return repository.CapNhatTrangThai(maMonAn, trangThai);
        }

        // 6. Tìm kiếm món ăn
        public List<MonAn> TimKiem(string tuKhoa)
        {
            return repository.TimKiem(
                tuKhoa == null ? "" : tuKhoa.Trim());
        }

        // 7. Lọc theo danh mục
        public List<MonAn> LocTheoDanhMuc(int maLoaiMon)
        {
            if (maLoaiMon <= 0)
                return GetAll();

            return repository.LocTheoDanhMuc(maLoaiMon);
        }

        // 8. Kiểm tra dữ liệu món ăn
        private void KiemTraDuLieu(MonAn monAn)
        {
            if (string.IsNullOrWhiteSpace(monAn.TenMonAn))
                throw new Exception("Vui lòng nhập tên món ăn!");

            if (monAn.TenMonAn.Trim().Length > 255)
                throw new Exception("Tên món ăn không được vượt quá 255 ký tự!");

            if (monAn.DonGia < 0)
                throw new Exception("Đơn giá không được nhỏ hơn 0!");

            if (monAn.MaLoaiMon <= 0)
                throw new Exception("Vui lòng chọn danh mục món ăn!");

            if (monAn.DonVi != null && monAn.DonVi.Length > 50)
                throw new Exception("Đơn vị không được vượt quá 50 ký tự!");

            if (monAn.HinhMon != null && monAn.HinhMon.Length > 500)
                throw new Exception("Đường dẫn hình món không được vượt quá 500 ký tự!");
        }
        // 9. Xóa món ăn
        public bool Xoa(int maMonAn)
        {
            if (maMonAn <= 0)
                throw new Exception("Mã món ăn không hợp lệ!");

            return repository.Xoa(maMonAn);
        }
    }
}
