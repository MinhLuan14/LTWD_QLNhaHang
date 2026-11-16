
using QuanLyNhaHang.Models;
using QuanLyNhaHang.Repositories;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace QuanLyNhaHang.Services
{
    public class NhanVienService
    {
        private readonly NhanVienRepository nhanVienRepository =
            new NhanVienRepository();

        private readonly AuthService authService =
            new AuthService();

        // 1. KIỂM TRA CỘT TRẠNG THÁI
        public void KiemTraCotTrangThai()
        {
            nhanVienRepository.KiemTraCotTrangThai();
        }

        // 2. LẤY DANH SÁCH NHÂN VIÊN
        public DataTable LayDanhSach()
        {
            return nhanVienRepository.LayDanhSach();
        }

        // 3. KIỂM TRA THÔNG TIN NHÂN VIÊN
        private void KiemTraNhanVien(
            NhanVien nhanVien,
            string trangThai)
        {
            if (nhanVien == null)
                throw new Exception("Thông tin nhân viên không hợp lệ!");

            if (string.IsNullOrWhiteSpace(nhanVien.HoVaTenLot) ||
                string.IsNullOrWhiteSpace(nhanVien.TenNhanVien))
            {
                throw new Exception("Vui lòng nhập đầy đủ họ và tên!");
            }

            if (nhanVien.HoVaTenLot.Length > 100 ||
                nhanVien.TenNhanVien.Length > 50)
            {
                throw new Exception("Họ tên nhân viên vượt quá độ dài cho phép!");
            }

            string dienThoai = nhanVien.SoDienThoai?.Trim() ?? "";

            if (dienThoai.Length != 10 ||
                !dienThoai.All(char.IsDigit))
            {
                throw new Exception(
                    "Số điện thoại phải gồm đúng 10 chữ số!");
            }

            if (!string.IsNullOrWhiteSpace(nhanVien.DiaChi) &&
                nhanVien.DiaChi.Length > 255)
            {
                throw new Exception("Địa chỉ không được vượt quá 255 ký tự!");
            }

            if (trangThai != "Đang làm" &&
                trangThai != "Nghỉ làm")
            {
                throw new Exception("Trạng thái nhân viên không hợp lệ!");
            }
        }

        // 4. KIỂM TRA TÊN ĐĂNG NHẬP
        private void KiemTraTaiKhoan(
            string tenDangNhap,
            string vaiTro,
            string matKhau)
        {
            if (string.IsNullOrWhiteSpace(tenDangNhap))
                throw new Exception("Vui lòng nhập tên đăng nhập!");

            tenDangNhap = tenDangNhap.Trim();

            if (tenDangNhap.Length < 3 ||
                tenDangNhap.Length > 50 ||
                tenDangNhap.Any(char.IsWhiteSpace))
            {
                throw new Exception(
                    "Tên đăng nhập phải từ 3 đến 50 ký tự " +
                    "và không chứa khoảng trắng!");
            }

            if (vaiTro != "Quản trị viên" &&
                vaiTro != "Quản lý" &&
                vaiTro != "Nhân viên")
            {
                throw new Exception("Vai trò tài khoản không hợp lệ!");
            }

            if (string.IsNullOrWhiteSpace(matKhau) ||
                matKhau.Length < 8)
            {
                throw new Exception(
                    "Mật khẩu phải có ít nhất 8 ký tự!");
            }
        }

        // 5. TÁCH HỌ TÊN
        public NhanVien TaoNhanVien(
            string hoTen,
            string dienThoai,
            string diaChi)
        {
            if (string.IsNullOrWhiteSpace(hoTen))
                throw new Exception("Vui lòng nhập họ và tên!");

            string[] parts = hoTen.Trim().Split(
                new char[] { ' ' },
                StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length < 2)
                throw new Exception("Vui lòng nhập đầy đủ họ và tên!");

            return new NhanVien
            {
                HoVaTenLot = string.Join(
                    " ", parts.Take(parts.Length - 1)),

                TenNhanVien = parts[parts.Length - 1],

                SoDienThoai = dienThoai?.Trim(),

                DiaChi = string.IsNullOrWhiteSpace(diaChi)
                    ? null
                    : diaChi.Trim()
            };
        }

        // 6. THÊM NHÂN VIÊN KHÔNG CÓ TÀI KHOẢN
        // THÊM NHÂN VIÊN
        public int ThemNhanVien(NhanVien nhanVien)
        {
            if (nhanVien == null)
                throw new Exception("Thông tin nhân viên không hợp lệ!");

            KiemTraNhanVien(nhanVien, "Đang làm");

            nhanVien.SoDienThoai =
                (nhanVien.SoDienThoai ?? "").Trim();

            return nhanVienRepository.ThemNhanVien(nhanVien);
        }
        // 7. THÊM NHÂN VIÊN KÈM TÀI KHOẢN
        public int ThemNhanVienKemTaiKhoan(
            NhanVien nhanVien,
            string tenDangNhap,
            string matKhau,
            string vaiTro,
            string trangThai)
        {
            KiemTraNhanVien(nhanVien, trangThai);

            KiemTraTaiKhoan(
                tenDangNhap,
                vaiTro,
                matKhau);

            nhanVien.SoDienThoai =
                nhanVien.SoDienThoai.Trim();

            // Mã hóa mật khẩu bằng AuthService hiện có
            string matKhauHash =
                authService.HashPassword(matKhau);

            return nhanVienRepository.ThemNhanVienKemTaiKhoan(
                nhanVien,
                tenDangNhap.Trim(),
                matKhauHash,
                vaiTro,
                trangThai);
        }

        // 8. CẬP NHẬT THÔNG TIN NHÂN VIÊN.
        public void CapNhatNhanVien(
            int maNhanVien,
            int maTaiKhoan,
            NhanVien nhanVien,
            string tenDangNhap,
            string vaiTro,
            string trangThai)
        {
            if (maNhanVien <= 0)
                throw new Exception("Vui lòng chọn nhân viên cần cập nhật!");

            // Kiểm tra thông tin nhân viên.
            KiemTraNhanVien(nhanVien, trangThai);

            // Chuẩn hóa số điện thoại.
            nhanVien.SoDienThoai = nhanVien.SoDienThoai.Trim();

            // Cập nhật hồ sơ, không thay đổi tài khoản đăng nhập.
            nhanVienRepository.CapNhatNhanVienVaTaiKhoan(
                maNhanVien,
                maTaiKhoan,
                nhanVien,
                null,
                null,
                trangThai
            );
        }
        // 9. NGỪNG LÀM NHÂN VIÊN
        public bool NgungLamNhanVien(int maNhanVien)
        {
            if (maNhanVien <= 0)
                throw new Exception("Vui lòng chọn nhân viên cần xóa!");

            bool ketQua =
                nhanVienRepository.NgungLamNhanVien(maNhanVien);

            if (!ketQua)
            {
                throw new Exception(
                    "Không tìm thấy nhân viên hoặc nhân viên đã nghỉ làm.");
            }

            return true;
        }

    }
}
