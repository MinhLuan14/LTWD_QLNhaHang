using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using QuanLyNhaHang.Models;

namespace QuanLyNhaHang.Repositories
{
    public class DatBanRepository
    {
        // 1. Thêm phiếu đặt bàn
        public int ThemDatBan(
            DatBan datBan,
            SqlConnection conn,
            SqlTransaction transaction)
        {
            string query = @"
                INSERT INTO DatBan
                (
                    ngayDat,
                    gioDat,
                    soLuongKhach,
                    trangThai,
                    ghiChu,
                    maKhachHang
                )
                OUTPUT INSERTED.maDatBan
                VALUES
                (
                    @NgayDat,
                    @GioDat,
                    @SoLuongKhach,
                    @TrangThai,
                    @GhiChu,
                    @MaKhachHang
                )";

            using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
            {
                cmd.Parameters.Add("@NgayDat", SqlDbType.Date)
                    .Value = datBan.NgayDat.Date;

                cmd.Parameters.Add("@GioDat", SqlDbType.Time)
                    .Value = datBan.GioDat;

                cmd.Parameters.Add("@SoLuongKhach", SqlDbType.Int)
                    .Value = datBan.SoLuongKhach;

                cmd.Parameters.Add("@TrangThai", SqlDbType.NVarChar, 50)
                    .Value = datBan.TrangThai;

                cmd.Parameters.Add("@GhiChu", SqlDbType.NVarChar, 255)
                    .Value = string.IsNullOrWhiteSpace(datBan.GhiChu)
                        ? (object)DBNull.Value
                        : datBan.GhiChu.Trim();

                cmd.Parameters.Add("@MaKhachHang", SqlDbType.Int)
                    .Value = datBan.MaKhachHang;

                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        // 2. Lấy phiếu đặt bàn theo mã
        public DatBan LayDatBanTheoMa(
    int maDatBan,
    SqlConnection conn,
    SqlTransaction transaction = null)
        {
            string query = @"
        SELECT
            db.maDatBan,
            db.ngayDat,
            db.gioDat,
            db.soLuongKhach,
            db.trangThai,
            db.ghiChu,
            db.maKhachHang,
            kh.hoVaTenLot,
            kh.tenKhachHang,
            kh.soDienThoai,
            kh.diaChi
        FROM dbo.DatBan db
        INNER JOIN dbo.KhachHang kh
            ON db.maKhachHang = kh.maKhachHang
        WHERE db.maDatBan = @MaDatBan;";

            using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
            {
                cmd.Parameters.Add("@MaDatBan", SqlDbType.Int)
                    .Value = maDatBan;

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new DatBan
                        {
                            MaDatBan = Convert.ToInt32(reader["maDatBan"]),
                            NgayDat = Convert.ToDateTime(reader["ngayDat"]),
                            GioDat = (TimeSpan)reader["gioDat"],
                            SoLuongKhach = Convert.ToInt32(reader["soLuongKhach"]),
                            TrangThai = reader["trangThai"].ToString(),

                            GhiChu = reader["ghiChu"] == DBNull.Value
                                ? null
                                : reader["ghiChu"].ToString(),

                            MaKhachHang = Convert.ToInt32(reader["maKhachHang"]),

                            TenKhachHang =
                                reader["hoVaTenLot"].ToString() + " " +
                                reader["tenKhachHang"].ToString(),

                            SoDienThoai = reader["soDienThoai"].ToString(),

                            // Lấy địa chỉ khách hàng
                            DiaChi = reader["diaChi"] == DBNull.Value
                                ? null
                                : reader["diaChi"].ToString()
                        };
                    }
                }
            }

            return null;
        }

        // 3. Lấy tất cả phiếu đặt bàn
        public List<DatBan> LayTatCaDatBan(
            SqlConnection conn,
            SqlTransaction transaction = null)
        {
            List<DatBan> danhSach = new List<DatBan>();

            string query = @"
                SELECT
                    db.maDatBan,
                    db.ngayDat,
                    db.gioDat,
                    db.soLuongKhach,
                    db.trangThai,
                    db.ghiChu,
                    db.maKhachHang,
                    kh.hoVaTenLot,
                    kh.tenKhachHang,
                    kh.soDienThoai
                FROM DatBan db
                INNER JOIN KhachHang kh
                    ON db.maKhachHang = kh.maKhachHang
                ORDER BY db.ngayDat DESC, db.gioDat DESC";

            using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    DatBan datBan = new DatBan
                    {
                        MaDatBan = Convert.ToInt32(reader["maDatBan"]),
                        NgayDat = Convert.ToDateTime(reader["ngayDat"]),
                        GioDat = (TimeSpan)reader["gioDat"],
                        SoLuongKhach = Convert.ToInt32(reader["soLuongKhach"]),
                        TrangThai = reader["trangThai"].ToString(),
                        GhiChu = reader["ghiChu"] == DBNull.Value
                            ? null
                            : reader["ghiChu"].ToString(),
                        MaKhachHang = Convert.ToInt32(reader["maKhachHang"]),
                        TenKhachHang =
                            reader["hoVaTenLot"].ToString() + " " +
                            reader["tenKhachHang"].ToString(),
                        SoDienThoai = reader["soDienThoai"].ToString()
                    };

                    danhSach.Add(datBan);
                }
            }

            return danhSach;
        }

        // 4. Cập nhật trạng thái phiếu đặt bàn
        public void CapNhatTrangThai(
            int maDatBan,
            string trangThai,
            SqlConnection conn,
            SqlTransaction transaction)
        {
            string query = @"
                UPDATE DatBan
                SET trangThai = @TrangThai
                WHERE maDatBan = @MaDatBan";

            using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
            {
                cmd.Parameters.Add("@TrangThai", SqlDbType.NVarChar, 50)
              .Value = trangThai;

                cmd.Parameters.Add("@MaDatBan", SqlDbType.Int)
                    .Value = maDatBan;

                if (cmd.ExecuteNonQuery() == 0)
                    throw new Exception("Không tìm thấy phiếu đặt bàn cần cập nhật.");
            }
        }
    }
}