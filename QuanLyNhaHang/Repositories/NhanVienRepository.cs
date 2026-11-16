
using System;
using System.Data;
using System.Data.SqlClient;

using QuanLyNhaHang.Data;
using QuanLyNhaHang.Models;

namespace QuanLyNhaHang.Repositories
{
    public class NhanVienRepository
    {
        // 1. LẤY KẾT NỐI
        private SqlConnection TaoKetNoi()
        {
            return new DbConnection().GetConnection();
        }

        // 2. KIỂM TRA CỘT TRẠNG THÁI
        public void KiemTraCotTrangThai()
        {
            using (SqlConnection conn = TaoKetNoi())
            {
                conn.Open();

                string sql = @"
                    IF COL_LENGTH('dbo.NhanVien', 'trangThai') IS NULL
                    BEGIN
                        ALTER TABLE dbo.NhanVien
                        ADD trangThai NVARCHAR(30) NOT NULL
                        CONSTRAINT DF_NhanVien_trangThai
                        DEFAULT (N'Đang làm');
                    END";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // 3. LẤY DANH SÁCH NHÂN VIÊN.
        public DataTable LayDanhSach()
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = TaoKetNoi())
            {
                conn.Open();

                string sql = @"
            SELECT
                nv.maNhanVien AS MaNV,

                LTRIM(RTRIM(
                    ISNULL(nv.hoVaTenLot, '') + ' ' +
                    ISNULL(nv.tenNhanVien, '')
                )) AS HoTen,

                nv.soDienThoai AS DienThoai,

                CASE
                    WHEN nv.maTaiKhoan IS NULL
                        THEN N'Nhân viên phục vụ'
                    ELSE ISNULL(tk.vaiTro, N'Nhân viên')
                END AS VaiTro,

                ISNULL(nv.trangThai, N'Đang làm') AS TrangThai,

                ISNULL(tk.tenDangNhap, '') AS TenDangNhap,

                nv.maTaiKhoan AS MaTaiKhoan

            FROM dbo.NhanVien nv

            LEFT JOIN dbo.TaiKhoan tk
                ON nv.maTaiKhoan = tk.maTaiKhoan

            ORDER BY nv.maNhanVien DESC";

                using (SqlDataAdapter da = new SqlDataAdapter(sql, conn))
                {
                    da.Fill(dt);
                }
            }

            return dt;
        }
        // 4. KIỂM TRA TÊN ĐĂNG NHẬP
        public bool KiemTraTenDangNhap(
            SqlConnection conn,
            SqlTransaction tran,
            string tenDangNhap,
            int maTaiKhoan)
        {
            string sql = @"
                SELECT COUNT(1)
                FROM dbo.TaiKhoan
                WHERE tenDangNhap = @TenDangNhap
                AND (@MaTK = 0 OR maTaiKhoan <> @MaTK)";

            using (SqlCommand cmd = new SqlCommand(sql, conn, tran))
            {
                cmd.Parameters.Add(
                    "@TenDangNhap", SqlDbType.VarChar, 50
                ).Value = tenDangNhap;

                cmd.Parameters.Add(
                    "@MaTK", SqlDbType.Int
                ).Value = maTaiKhoan;

                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }
        // 5. THÊM TÀI KHOẢN
        public int ThemTaiKhoan(
            SqlConnection conn,
            SqlTransaction tran,
            string tenDangNhap,
            string matKhauHash,
            string vaiTro)
        {
            string sql = @"
                INSERT INTO dbo.TaiKhoan
                (
                    tenDangNhap,
                    matKhau,
                    vaiTro
                )
                VALUES
                (
                    @TenDangNhap,
                    @MatKhau,
                    @VaiTro
                );

                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using (SqlCommand cmd = new SqlCommand(sql, conn, tran))
            {
                cmd.Parameters.Add(
                    "@TenDangNhap", SqlDbType.VarChar, 50
                ).Value = tenDangNhap;

                cmd.Parameters.Add(
                    "@MatKhau", SqlDbType.VarChar, 255
                ).Value = matKhauHash;

                cmd.Parameters.Add(
                    "@VaiTro", SqlDbType.NVarChar, 50
                ).Value = vaiTro;

                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }
        // 6. THÊM HỒ SƠ NHÂN VIÊN
        public int ThemNhanVien(
            SqlConnection conn,
            SqlTransaction tran,
            NhanVien nhanVien,
            int maTaiKhoan,
            string trangThai)
        {
            string sql = @"
                INSERT INTO dbo.NhanVien
                (
                    hoVaTenLot,
                    tenNhanVien,
                    soDienThoai,
                    diaChi,
                    maTaiKhoan,
                    trangThai
                )
                VALUES
                (
                    @HoVaTenLot,
                    @TenNhanVien,
                    @SoDienThoai,
                    @DiaChi,
                    @MaTaiKhoan,
                    @TrangThai
                );

                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using (SqlCommand cmd = new SqlCommand(sql, conn, tran))
            {
                cmd.Parameters.Add(
                    "@HoVaTenLot", SqlDbType.NVarChar, 100
                ).Value = nhanVien.HoVaTenLot;

                cmd.Parameters.Add(
                    "@TenNhanVien", SqlDbType.NVarChar, 50
                ).Value = nhanVien.TenNhanVien;

                cmd.Parameters.Add(
                    "@SoDienThoai", SqlDbType.Char, 10
                ).Value = nhanVien.SoDienThoai;

                cmd.Parameters.Add(
                    "@DiaChi", SqlDbType.NVarChar, 255
                ).Value = string.IsNullOrWhiteSpace(nhanVien.DiaChi)
                    ? (object)DBNull.Value
                    : nhanVien.DiaChi;

                cmd.Parameters.Add(
                    "@MaTaiKhoan", SqlDbType.Int
                ).Value = maTaiKhoan > 0
                    ? (object)maTaiKhoan
                    : DBNull.Value;

                cmd.Parameters.Add(
                    "@TrangThai", SqlDbType.NVarChar, 30
                ).Value = trangThai;

                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }
        // 7. THÊM NHÂN VIÊN CÓ TÀI KHOẢN
        public int ThemNhanVienKemTaiKhoan(
            NhanVien nhanVien,
            string tenDangNhap,
            string matKhauHash,
            string vaiTro,
            string trangThai)
        {
            using (SqlConnection conn = TaoKetNoi())
            {
                conn.Open();

                using (SqlTransaction tran = conn.BeginTransaction())
                {
                    try
                    {
                        int maTaiKhoan = 0;

                        if (!string.IsNullOrWhiteSpace(tenDangNhap))
                        {
                            if (KiemTraTenDangNhap(
                                conn, tran, tenDangNhap, 0))
                            {
                                throw new Exception(
                                    "Tên đăng nhập này đã tồn tại!");
                            }

                            maTaiKhoan = ThemTaiKhoan(
                                conn,
                                tran,
                                tenDangNhap,
                                matKhauHash,
                                vaiTro);
                        }

                        int maNhanVien = ThemNhanVien(
                            conn,
                            tran,
                            nhanVien,
                            maTaiKhoan,
                            trangThai);

                        tran.Commit();

                        return maNhanVien;
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
        }
        // 8. THÊM NHÂN VIÊN KHÔNG CÓ TÀI KHOẢN
        // CHUYỂN NHÂN VIÊN SANG NGHỈ LÀM
        public bool NgungLamNhanVien(int maNhanVien)
        {
            using (SqlConnection conn = TaoKetNoi())
            {
                conn.Open();

                string sql = @"
            UPDATE dbo.NhanVien
            SET trangThai = N'Nghỉ làm'
            WHERE maNhanVien = @MaNV
              AND trangThai <> N'Nghỉ làm'";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add("@MaNV", SqlDbType.Int)
                        .Value = maNhanVien;

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
        // 9. CẬP NHẬT TÀI KHOẢN
        public void CapNhatTaiKhoan(
            SqlConnection conn,
            SqlTransaction tran,
            int maTaiKhoan,
            string tenDangNhap,
            string vaiTro)
        {
            string sql = @"
                UPDATE dbo.TaiKhoan
                SET
                    tenDangNhap = @TenDangNhap,
                    vaiTro = @VaiTro
                WHERE maTaiKhoan = @MaTK";

            using (SqlCommand cmd = new SqlCommand(sql, conn, tran))
            {
                cmd.Parameters.Add(
                    "@TenDangNhap", SqlDbType.VarChar, 50
                ).Value = tenDangNhap;

                cmd.Parameters.Add(
                    "@VaiTro", SqlDbType.NVarChar, 50
                ).Value = vaiTro;

                cmd.Parameters.Add(
                    "@MaTK", SqlDbType.Int
                ).Value = maTaiKhoan;

                cmd.ExecuteNonQuery();
            }
        }
        // 10. CẬP NHẬT HỒ SƠ NHÂN VIÊN
        public void CapNhatNhanVien(
            SqlConnection conn,
            SqlTransaction tran,
            int maNhanVien,
            NhanVien nhanVien,
            int maTaiKhoan,
            string trangThai)
        {
            string sql = @"
                UPDATE dbo.NhanVien
                SET
                    hoVaTenLot = @HoVaTenLot,
                    tenNhanVien = @TenNhanVien,
                    soDienThoai = @SoDienThoai,
                    diaChi = @DiaChi,
                    maTaiKhoan = @MaTaiKhoan,
                    trangThai = @TrangThai
                WHERE maNhanVien = @MaNV";

            using (SqlCommand cmd = new SqlCommand(sql, conn, tran))
            {
                cmd.Parameters.Add(
                    "@HoVaTenLot", SqlDbType.NVarChar, 100
                ).Value = nhanVien.HoVaTenLot;

                cmd.Parameters.Add(
                    "@TenNhanVien", SqlDbType.NVarChar, 50
                ).Value = nhanVien.TenNhanVien;

                cmd.Parameters.Add(
                    "@SoDienThoai", SqlDbType.Char, 10
                ).Value = nhanVien.SoDienThoai;

                cmd.Parameters.Add(
                    "@DiaChi", SqlDbType.NVarChar, 255
                ).Value = string.IsNullOrWhiteSpace(nhanVien.DiaChi)
                    ? (object)DBNull.Value
                    : nhanVien.DiaChi;

                cmd.Parameters.Add(
                    "@MaTaiKhoan", SqlDbType.Int
                ).Value = maTaiKhoan > 0
                    ? (object)maTaiKhoan
                    : DBNull.Value;

                cmd.Parameters.Add(
                    "@TrangThai", SqlDbType.NVarChar, 30
                ).Value = trangThai;

                cmd.Parameters.Add(
                    "@MaNV", SqlDbType.Int
                ).Value = maNhanVien;

                cmd.ExecuteNonQuery();
            }
        }

        // 11. CẬP NHẬT HỒ SƠ NHÂN VIÊN VÀ GIỮ NGUYÊN TÀI KHOẢN CŨ.
        public void CapNhatNhanVienVaTaiKhoan(
            int maNhanVien,
            int maTaiKhoan,
            NhanVien nhanVien,
            string tenDangNhap,
            string vaiTro,
            string trangThai)
        {
            using (SqlConnection conn = TaoKetNoi())
            {
                conn.Open();

                using (SqlTransaction tran = conn.BeginTransaction())
                {
                    try
                    {
                        // Chỉ cập nhật hồ sơ nhân viên, không sửa bảng TaiKhoan.
                        CapNhatNhanVien(
                            conn,
                            tran,
                            maNhanVien,
                            nhanVien,
                            maTaiKhoan,
                            trangThai
                        );

                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
        }
        // 12. THÊM NHÂN VIÊN PHỤC VỤ KHÔNG CÓ TÀI KHOẢN.
        public int ThemNhanVien(NhanVien nhanVien)
        {
            using (SqlConnection conn = TaoKetNoi())
            {
                conn.Open();

                using (SqlTransaction tran = conn.BeginTransaction())
                {
                    try
                    {
                        // Luôn truyền mã tài khoản bằng 0 để không tạo liên kết tài khoản.
                        int maNhanVien = ThemNhanVien(
                            conn,
                            tran,
                            nhanVien,
                            0,
                            "Đang làm"
                        );

                        tran.Commit();

                        return maNhanVien;
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}
