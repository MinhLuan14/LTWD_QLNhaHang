using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using QuanLyNhaHang.Models;

namespace QuanLyNhaHang.Repositories
{
    public class ChiTietDatBanRepository
    {
        // 1. Thêm một bàn vào phiếu đặt
        public void ThemChiTietDatBan(
            ChiTietDatBan chiTiet,
            SqlConnection conn,
            SqlTransaction transaction)
        {
            string query = @"
                INSERT INTO ChiTietDatBan
                (
                    maDatBan,
                    maBan
                )
                VALUES
                (
                    @MaDatBan,
                    @MaBan
                )";

            using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
            {
                cmd.Parameters.Add("@MaDatBan", SqlDbType.Int)
                    .Value = chiTiet.MaDatBan;

                cmd.Parameters.Add("@MaBan", SqlDbType.Int)
                    .Value = chiTiet.maBan;

                cmd.ExecuteNonQuery();
            }
        }

        // 2. Thêm nhiều bàn vào cùng một phiếu đặt
        public void ThemDanhSachBan(
            List<ChiTietDatBan> danhSachBan,
            int maDatBan,
            SqlConnection conn,
            SqlTransaction transaction)
        {
            foreach (ChiTietDatBan chiTiet in danhSachBan)
            {
                chiTiet.MaDatBan = maDatBan;

                ThemChiTietDatBan(
                    chiTiet,
                    conn,
                    transaction
                );
            }
        }

        // 3. Lấy danh sách bàn theo mã phiếu đặt
        public List<ChiTietDatBan> LayDanhSachBanTheoMaDatBan(
            int maDatBan,
            SqlConnection conn,
            SqlTransaction transaction = null)
        {
            List<ChiTietDatBan> danhSach = new List<ChiTietDatBan>();

            string query = @"
        SELECT
            ct.maChiTietDatBan,
            ct.maDatBan,
            ct.maBan,
            b.soBan,
            b.soChoNgoi,
            b.trangThai
        FROM ChiTietDatBan ct
        INNER JOIN Ban b
            ON ct.maBan = b.maBan
        WHERE ct.maDatBan = @MaDatBan
        ORDER BY b.soBan";

            using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
            {
                cmd.Parameters.Add("@MaDatBan", SqlDbType.Int)
                    .Value = maDatBan;

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ChiTietDatBan chiTiet = new ChiTietDatBan
                        {
                            MaChiTietDatBan =
                                Convert.ToInt32(reader["maChiTietDatBan"]),

                            MaDatBan =
                                Convert.ToInt32(reader["maDatBan"]),

                            maBan =
                                Convert.ToInt32(reader["maBan"]),

                            soBan =
                                Convert.ToInt32(reader["soBan"]),

                            soChoNgoi =
                                Convert.ToInt32(reader["soChoNgoi"]),

                            trangThai =
                                reader["trangThai"] == DBNull.Value
                                ? ""
                                : reader["trangThai"].ToString()
                        };

                        danhSach.Add(chiTiet);
                    }
                }
            }

            return danhSach;
        }

        // 4. Xóa các bàn thuộc phiếu đặt
        public void XoaChiTietTheoMaDatBan(
            int maDatBan,
            SqlConnection conn,
            SqlTransaction transaction)
        {
            string query = @"
                DELETE FROM ChiTietDatBan
                WHERE maDatBan = @MaDatBan";

            using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
            {
                cmd.Parameters.Add("@MaDatBan", SqlDbType.Int)
                    .Value = maDatBan;

                cmd.ExecuteNonQuery();
            }
        }
        // 5. Lấy mã đặt bàn và mã khách hàng theo mã bàn
        public Tuple<int, int> LayThongTinDatBanTheoMaBan(
            int maBan,
            SqlConnection conn,
            SqlTransaction transaction = null)
        {
            string sql = @"
        SELECT TOP 1
            db.maDatBan,
            db.maKhachHang
        FROM dbo.DatBan db
        INNER JOIN dbo.ChiTietDatBan ct
            ON db.maDatBan = ct.maDatBan
        WHERE ct.maBan = @maBan
          AND db.trangThai IN
          (
              N'Đã đặt',
              N'Đang phục vụ'
          )
        ORDER BY db.maDatBan DESC";

            using (SqlCommand cmd = new SqlCommand(sql, conn, transaction))
            {
                cmd.Parameters.Add("@maBan", SqlDbType.Int).Value = maBan;

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int maDatBan =
                            Convert.ToInt32(reader["maDatBan"]);

                        int maKhachHang =
                            Convert.ToInt32(reader["maKhachHang"]);

                        return Tuple.Create(maDatBan, maKhachHang);
                    }
                }
            }

            return null;
        }

        // 6. Lấy mã đặt bàn theo mã bàn
        public int? LayMaDatBanTheoMaBan(
            int maBan,
            SqlConnection conn,
            SqlTransaction transaction = null)
        {
            var thongTin = LayThongTinDatBanTheoMaBan(
                maBan,
                conn,
                transaction
            );

            return thongTin == null ? (int?)null : thongTin.Item1;
        }

    }
}