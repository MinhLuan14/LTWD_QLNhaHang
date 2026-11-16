
using QuanLyNhaHang.Data;
using QuanLyNhaHang.Models;
using System;
using System.Data;
using System.Data.SqlClient;

namespace QuanLyNhaHang.Repositories
{
    public class TaiKhoanRepository
    {
        // 1. KIỂM TRA TÊN ĐĂNG NHẬP
        public bool KiemTraTenDangNhap(
            string tenDangNhap,
            SqlConnection conn,
            SqlTransaction transaction)
        {
            const string sql = @"
                SELECT COUNT(1)
                FROM dbo.TaiKhoan
                WHERE tenDangNhap = @TenDangNhap";

            using (SqlCommand cmd =
                new SqlCommand(sql, conn, transaction))
            {
                cmd.Parameters.Add(
                    "@TenDangNhap", SqlDbType.VarChar, 50
                ).Value = tenDangNhap;

                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        // 2. KIỂM TRA EMAIL
        public bool KiemTraEmail(
            string email,
            SqlConnection conn,
            SqlTransaction transaction)
        {
            const string sql = @"
                SELECT COUNT(1)
                FROM dbo.TaiKhoan
                WHERE Email = @Email";

            using (SqlCommand cmd =
                new SqlCommand(sql, conn, transaction))
            {
                cmd.Parameters.Add(
                    "@Email", SqlDbType.VarChar, 100
                ).Value = email;

                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        // 3. THÊM TÀI KHOẢN
        public int ThemTaiKhoan(
            string tenDangNhap,
            string matKhau,
            string vaiTro,
            string email,
            SqlConnection conn,
            SqlTransaction transaction)
        {
            const string sql = @"
                INSERT INTO dbo.TaiKhoan
                    (tenDangNhap, matKhau, vaiTro, Email)
                VALUES
                    (@TenDangNhap, @MatKhau, @VaiTro, @Email);

                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using (SqlCommand cmd =
                new SqlCommand(sql, conn, transaction))
            {
                cmd.Parameters.Add(
                    "@TenDangNhap", SqlDbType.VarChar, 50
                ).Value = tenDangNhap;

                cmd.Parameters.Add(
                    "@MatKhau", SqlDbType.VarChar, 255
                ).Value = matKhau;

                cmd.Parameters.Add(
                    "@VaiTro", SqlDbType.NVarChar, 50
                ).Value = vaiTro;

                cmd.Parameters.Add(
                    "@Email", SqlDbType.VarChar, 100
                ).Value = string.IsNullOrWhiteSpace(email)
                    ? (object)DBNull.Value
                    : email;

                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        // 4. THÊM HỒ SƠ NHÂN VIÊN
        public void ThemHoSoNhanVien(
            NhanVien nhanVien,
            int maTaiKhoan,
            SqlConnection conn,
            SqlTransaction transaction)
        {
            const string sql = @"
                INSERT INTO dbo.NhanVien
                    (hoVaTenLot, tenNhanVien, soDienThoai,
                     diaChi, maTaiKhoan)
                VALUES
                    (@HoVaTenLot, @TenNhanVien, @SoDienThoai,
                     @DiaChi, @MaTaiKhoan);";

            using (SqlCommand cmd =
                new SqlCommand(sql, conn, transaction))
            {
                cmd.Parameters.Add(
                    "@HoVaTenLot", SqlDbType.NVarChar, 100
                ).Value = nhanVien.HoVaTenLot.Trim();

                cmd.Parameters.Add(
                    "@TenNhanVien", SqlDbType.NVarChar, 50
                ).Value = nhanVien.TenNhanVien.Trim();

                cmd.Parameters.Add(
                    "@SoDienThoai", SqlDbType.Char, 10
                ).Value = nhanVien.SoDienThoai.Trim();

                cmd.Parameters.Add(
                    "@DiaChi", SqlDbType.NVarChar, 255
                ).Value = string.IsNullOrWhiteSpace(nhanVien.DiaChi)
                    ? (object)DBNull.Value
                    : nhanVien.DiaChi.Trim();

                cmd.Parameters.Add(
                    "@MaTaiKhoan", SqlDbType.Int
                ).Value = maTaiKhoan;

                cmd.ExecuteNonQuery();
            }
        }

        // 5. TẠO TÀI KHOẢN VÀ HỒ SƠ NHÂN VIÊN
        // Hai thao tác được thực hiện trong cùng transaction.
        public void TaoTaiKhoanNhanVien(
            NhanVien nhanVien,
            string tenDangNhap,
            string passwordHash,
            string vaiTro,
            string email)
        {
            using (SqlConnection conn =
                new DbConnection().GetConnection())
            {
                conn.Open();

                using (SqlTransaction transaction =
                    conn.BeginTransaction())
                {
                    try
                    {
                        if (KiemTraTenDangNhap(
                            tenDangNhap, conn, transaction))
                        {
                            throw new Exception(
                                "Tên đăng nhập đã tồn tại.");
                        }

                        if (!string.IsNullOrWhiteSpace(email) &&
                            KiemTraEmail(email, conn, transaction))
                        {
                            throw new Exception(
                                "Email đã được sử dụng.");
                        }

                        int maTaiKhoan = ThemTaiKhoan(
                            tenDangNhap,
                            passwordHash,
                            vaiTro,
                            email,
                            conn,
                            transaction
                        );

                        ThemHoSoNhanVien(
                            nhanVien,
                            maTaiKhoan,
                            conn,
                            transaction
                        );

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        // 6. LẤY THÔNG TIN TÀI KHOẢN ĐỂ ĐĂNG NHẬP
        public DataTable LayThongTinDangNhap(string tenDangNhap)
        {
            const string sql = @"
                SELECT
                    tk.maTaiKhoan,
                    tk.tenDangNhap,
                    tk.matKhau,
                    tk.vaiTro,
                    tk.Email,
                    nv.maNhanVien,
                    nv.hoVaTenLot,
                    nv.tenNhanVien,
                    nv.soDienThoai,
                    nv.diaChi
                FROM dbo.TaiKhoan tk
                INNER JOIN dbo.NhanVien nv
                    ON nv.maTaiKhoan = tk.maTaiKhoan
                WHERE tk.tenDangNhap = @TenDangNhap";

            DataTable table = new DataTable();

            using (SqlConnection conn =
                new DbConnection().GetConnection())
            using (SqlCommand cmd =
                new SqlCommand(sql, conn))
            using (SqlDataAdapter adapter =
                new SqlDataAdapter(cmd))
            {
                cmd.Parameters.Add(
                    "@TenDangNhap", SqlDbType.VarChar, 50
                ).Value = tenDangNhap;

                adapter.Fill(table);
            }

            return table;
        }

    }
}
