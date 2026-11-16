
using System;
using System.Data;
using System.Data.SqlClient;
using QuanLyNhaHang.Data;
using QuanLyNhaHang.Models;

namespace QuanLyNhaHang.Repositories
{
    public class KhachHangRepository
    {
        private readonly DbConnection dbConnection = new DbConnection();

        // Tìm khách hàng theo số điện thoại
        public KhachHang TimTheoSoDienThoai(
            string soDienThoai,
            SqlConnection conn,
            SqlTransaction transaction)
        {
            string query = @"
                SELECT
                    maKhachHang,
                    hoVaTenLot,
                    tenKhachHang,
                    soDienThoai,
                    diaChi,
                    maTaiKhoan
                FROM KhachHang
                WHERE soDienThoai = @SoDienThoai";

            using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
            {
                cmd.Parameters.Add("@SoDienThoai", SqlDbType.Char, 10)
                    .Value = soDienThoai;

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new KhachHang
                        {
                            MaKhachHang = Convert.ToInt32(reader["maKhachHang"]),
                            HoVaTenLot = reader["hoVaTenLot"].ToString(),
                            TenKhachHang = reader["tenKhachHang"].ToString(),
                            SoDienThoai = reader["soDienThoai"].ToString().Trim(),
                            DiaChi = reader["diaChi"] == DBNull.Value
                                ? null
                                : reader["diaChi"].ToString(),
                            MaTaiKhoan = reader["maTaiKhoan"] == DBNull.Value
                                ? (int?)null
                                : Convert.ToInt32(reader["maTaiKhoan"])
                        };
                    }
                }
            }

            return null;
        }

        // Thêm khách hàng mới
        public int ThemKhachHang(
            KhachHang khachHang,
            SqlConnection conn,
            SqlTransaction transaction)
        {
            string query = @"
                INSERT INTO KhachHang
                (
                    hoVaTenLot,
                    tenKhachHang,
                    soDienThoai,
                    diaChi,
                    maTaiKhoan
                )
                OUTPUT INSERTED.maKhachHang
                VALUES
                (
                    @HoVaTenLot,
                    @TenKhachHang,
                    @SoDienThoai,
                    @DiaChi,
                    @MaTaiKhoan
                )";

            using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
            {
                cmd.Parameters.Add("@HoVaTenLot", SqlDbType.NVarChar, 100)
                    .Value = khachHang.HoVaTenLot;

                cmd.Parameters.Add("@TenKhachHang", SqlDbType.NVarChar, 50)
                    .Value = khachHang.TenKhachHang;

                cmd.Parameters.Add("@SoDienThoai", SqlDbType.Char, 10)
                    .Value = khachHang.SoDienThoai;

                cmd.Parameters.Add("@DiaChi", SqlDbType.NVarChar, 255)
                    .Value = string.IsNullOrWhiteSpace(khachHang.DiaChi)
                        ? (object)DBNull.Value
                        : khachHang.DiaChi;

                cmd.Parameters.Add("@MaTaiKhoan", SqlDbType.Int)
                    .Value = khachHang.MaTaiKhoan.HasValue
                        ? (object)khachHang.MaTaiKhoan.Value
                        : DBNull.Value;

                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }
        // Tìm khách hàng theo số điện thoại và họ tên
        public KhachHang TimTheoSoDienThoaiVaHoTen(
            string soDienThoai,
            string hoVaTenLot,
            string tenKhachHang,
            SqlConnection conn,
            SqlTransaction transaction)
        {
            string query = @"
    SELECT TOP 1
        maKhachHang,
        hoVaTenLot,
        tenKhachHang,
        soDienThoai,
        diaChi,
        maTaiKhoan
    FROM KhachHang WITH (UPDLOCK, HOLDLOCK)
    WHERE LTRIM(RTRIM(soDienThoai)) = @SoDienThoai
      AND LTRIM(RTRIM(hoVaTenLot)) = @HoVaTenLot
      AND LTRIM(RTRIM(tenKhachHang)) = @TenKhachHang
    ORDER BY maKhachHang";

            using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
            {
                cmd.Parameters.Add("@SoDienThoai", SqlDbType.Char, 10)
                    .Value = soDienThoai.Trim();

                cmd.Parameters.Add("@HoVaTenLot", SqlDbType.NVarChar, 100)
                    .Value = hoVaTenLot.Trim();

                cmd.Parameters.Add("@TenKhachHang", SqlDbType.NVarChar, 50)
                    .Value = tenKhachHang.Trim();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new KhachHang
                        {
                            MaKhachHang = Convert.ToInt32(reader["maKhachHang"]),
                            HoVaTenLot = reader["hoVaTenLot"].ToString(),
                            TenKhachHang = reader["tenKhachHang"].ToString(),
                            SoDienThoai = reader["soDienThoai"].ToString().Trim(),
                            DiaChi = reader["diaChi"] == DBNull.Value
                                ? null
                                : reader["diaChi"].ToString(),
                            MaTaiKhoan = reader["maTaiKhoan"] == DBNull.Value
                                ? (int?)null
                                : Convert.ToInt32(reader["maTaiKhoan"])
                        };
                    }
                }
            }

            return null;
        }
    }
}
