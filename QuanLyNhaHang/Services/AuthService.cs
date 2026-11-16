
using System;
using System.Data;
using System.Security.Cryptography;
using QuanLyNhaHang.Models;
using QuanLyNhaHang.Repositories;

namespace QuanLyNhaHang.Services
{
    public class AuthService
    {
        private readonly TaiKhoanRepository authRepository =
            new TaiKhoanRepository();

        private const int SoVongLapHash = 100000;
        private const int DoDaiSalt = 16;
        private const int DoDaiHash = 32;

        // 1. BĂM MẬT KHẨU
        public string HashPassword(string password)
        {
            byte[] salt = new byte[DoDaiSalt];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            using (var pbkdf2 = new Rfc2898DeriveBytes(
                password, salt, SoVongLapHash))
            {
                byte[] hash = pbkdf2.GetBytes(DoDaiHash);

                return Convert.ToBase64String(salt) + "." +
                       Convert.ToBase64String(hash);
            }
        }

        // 2. XÁC MINH MẬT KHẨU
        private bool VerifyPassword(
            string password,
            string storedHash)
        {
            if (string.IsNullOrEmpty(password) ||
                string.IsNullOrEmpty(storedHash))
                return false;

            string[] parts = storedHash.Split('.');

            if (parts.Length != 2)
                return false;

            try
            {
                byte[] salt =
                    Convert.FromBase64String(parts[0]);

                byte[] expectedHash =
                    Convert.FromBase64String(parts[1]);

                using (var pbkdf2 = new Rfc2898DeriveBytes(
                    password, salt, SoVongLapHash))
                {
                    byte[] actualHash =
                        pbkdf2.GetBytes(DoDaiHash);

                    int diff =
                        actualHash.Length ^ expectedHash.Length;

                    int length = Math.Min(
                        actualHash.Length, expectedHash.Length);

                    for (int i = 0; i < length; i++)
                        diff |= actualHash[i] ^ expectedHash[i];

                    return diff == 0;
                }
            }
            catch (FormatException)
            {
                return false;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        // 3. ĐĂNG KÝ NHÂN VIÊN MẶC ĐỊNH
        public void DangKyNhanVien(
            NhanVien nhanVien,
            string tenDangNhap,
            string email,
            string matKhau)
        {
            DangKyNhanVien(
                nhanVien,
                tenDangNhap,
                email,
                matKhau,
                "Nhân viên"
            );
        }

        // 4. ĐĂNG KÝ NHÂN VIÊN THEO VAI TRÒ
        public void DangKyNhanVien(
            NhanVien nhanVien,
            string tenDangNhap,
            string email,
            string matKhau,
            string vaiTro)
        {
            if (nhanVien == null)
                throw new ArgumentNullException(nameof(nhanVien));

            tenDangNhap = (tenDangNhap ?? "").Trim();
            email = (email ?? "").Trim();
            vaiTro = (vaiTro ?? "").Trim();

            // Kiểm tra vai trò
            if (vaiTro != "Quản trị viên" &&
                vaiTro != "Quản lý" &&
                vaiTro != "Nhân viên")
            {
                throw new Exception("Vai trò tài khoản không hợp lệ.");
            }

            // Kiểm tra thông tin nhân viên
            if (string.IsNullOrWhiteSpace(nhanVien.HoVaTenLot) ||
                string.IsNullOrWhiteSpace(nhanVien.TenNhanVien) ||
                string.IsNullOrWhiteSpace(nhanVien.SoDienThoai))
            {
                throw new Exception(
                    "Vui lòng nhập đầy đủ họ tên và số điện thoại.");
            }

            if (nhanVien.SoDienThoai.Trim().Length != 10)
                throw new Exception(
                    "Số điện thoại phải có đúng 10 ký tự.");

            // Kiểm tra tài khoản
            if (tenDangNhap.Length == 0 ||
                string.IsNullOrEmpty(matKhau))
            {
                throw new Exception(
                    "Tên đăng nhập và mật khẩu không được để trống.");
            }

            if (tenDangNhap.Length > 50)
                throw new Exception(
                    "Tên đăng nhập không được vượt quá 50 ký tự.");

            if (matKhau.Length < 8)
                throw new Exception(
                    "Mật khẩu phải có ít nhất 8 ký tự.");

            // Kiểm tra email
            if (email.Length > 100)
                throw new Exception("Email không được vượt quá 100 ký tự.");

            if (email.Length > 0 &&
                (!email.Contains("@") ||
                 email.StartsWith("@") ||
                 email.EndsWith("@")))
            {
                throw new Exception("Email không hợp lệ.");
            }

            // Kiểm tra độ dài thông tin
            if (nhanVien.HoVaTenLot.Trim().Length > 100 ||
                nhanVien.TenNhanVien.Trim().Length > 50 ||
                (nhanVien.DiaChi ?? "").Trim().Length > 255)
            {
                throw new Exception(
                    "Một hoặc nhiều trường dữ liệu quá dài.");
            }

            // Mã hóa mật khẩu
            string passwordHash = HashPassword(matKhau);

            // Gọi Repository để lưu dữ liệu
            authRepository.TaoTaiKhoanNhanVien(
                nhanVien,
                tenDangNhap,
                passwordHash,
                vaiTro,
                email
            );
        }

        // 5. ĐĂNG NHẬP
        public DataRow DangNhap(
            string tenDangNhap,
            string matKhau)
        {
            tenDangNhap = (tenDangNhap ?? "").Trim();

            if (tenDangNhap.Length == 0 ||
                string.IsNullOrEmpty(matKhau))
                return null;

            // Gọi Repository lấy dữ liệu
            DataTable table =
                authRepository.LayThongTinDangNhap(tenDangNhap);

            if (table.Rows.Count == 0)
                return null;

            DataRow row = table.Rows[0];

            string storedHash =
                Convert.ToString(row["matKhau"]);

            // Xác minh mật khẩu
            if (!VerifyPassword(matKhau, storedHash))
                return null;

            // Không trả mật khẩu băm ra ngoài
            row["matKhau"] = DBNull.Value;

            return row;
        }
    }
}
