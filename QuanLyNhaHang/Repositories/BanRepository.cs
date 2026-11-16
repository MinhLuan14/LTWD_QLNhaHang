using QuanLyNhaHang.Data;
using QuanLyNhaHang.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace QuanLyNhaHang.Repositories
{
    public class BanRepository
    {
        private readonly DbConnection db = new DbConnection();

        public List<Ban> GetAll()
        {
            List<Ban> dsBan = new List<Ban>();

            using (SqlConnection conn = db.GetConnection())
            {
                conn.Open();

                string sql =
                    "SELECT * FROM Ban ORDER BY soBan";

                SqlCommand cmd =
                    new SqlCommand(sql, conn);

                SqlDataReader reader =
                    cmd.ExecuteReader();

                while (reader.Read())
                {
                    dsBan.Add(new Ban
                    {
                        MaBan = Convert.ToInt32(reader["maBan"]),
                        SoBan = Convert.ToInt32(reader["soBan"]),
                        SoChoNgoi = Convert.ToInt32(reader["soChoNgoi"]),
                        TrangThai = reader["trangThai"].ToString()
                    });
                }
            }

            return dsBan;
        }

        public bool Insert(Ban ban)
        {
            using (SqlConnection conn = db.GetConnection())
            {
                conn.Open();

                string sql =
                @"INSERT INTO Ban
                  (soBan, soChoNgoi, trangThai)
                  VALUES
                  (@soBan,@soChoNgoi,@trangThai)";

                SqlCommand cmd =
                    new SqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@soBan", ban.SoBan);
                cmd.Parameters.AddWithValue("@soChoNgoi", ban.SoChoNgoi);
                cmd.Parameters.AddWithValue("@trangThai", ban.TrangThai);

                return cmd.ExecuteNonQuery() > 0;
            }
        }
        public bool DatBan(int maBan,string tenKhachHang,string soDienThoai,int soKhach,DateTime ngayDat, string ghiChu)
        {
            using (SqlConnection conn = db.GetConnection())
            {
                conn.Open();

                SqlTransaction tran = conn.BeginTransaction();

                try
                {
                    int maKhachHang;

                    // Tìm khách hàng
                    string sqlFindKhach =
                    @"SELECT maKhachHang
              FROM KhachHang
              WHERE soDienThoai = @soDienThoai";

                    SqlCommand cmdFind =
                        new SqlCommand(sqlFindKhach, conn, tran);

                    cmdFind.Parameters.AddWithValue("@soDienThoai", soDienThoai);

                    object result = cmdFind.ExecuteScalar();

                    if (result != null)
                    {
                        maKhachHang = Convert.ToInt32(result);
                    }
                    else
                    {
                        // Thêm khách hàng mới
                        string sqlInsertKhach =
                        @"INSERT INTO KhachHang( hoVaTenLot,tenKhachHang,soDienThoai)
                    VALUES(N'',@tenKhachHang, @soDienThoai);
                SELECT SCOPE_IDENTITY();";

                        SqlCommand cmdInsertKhach =
                            new SqlCommand(sqlInsertKhach, conn, tran);

                        cmdInsertKhach.Parameters.AddWithValue(
                            "@tenKhachHang",
                            tenKhachHang);

                        cmdInsertKhach.Parameters.AddWithValue(
                            "@soDienThoai",
                            soDienThoai);

                        maKhachHang = Convert.ToInt32(
                            cmdInsertKhach.ExecuteScalar());
                    }

                    // Insert DatBan
                    string sqlDatBan =
                    @"INSERT INTO DatBan
            (
                ngayDat,
                gioDat,
                soLuongKhach,
                trangThai,
                ghiChu,
                maBan,
                maKhachHang
            )
            VALUES
            (
                @ngayDat,
                @gioDat,
                @soLuongKhach,
                N'Chờ xác nhận',
                @ghiChu,
                @maBan,
                @maKhachHang
            )";

                    SqlCommand cmdDatBan =
                        new SqlCommand(sqlDatBan, conn, tran);

                    cmdDatBan.Parameters.AddWithValue(
                        "@ngayDat",
                        ngayDat.Date);

                    cmdDatBan.Parameters.AddWithValue(
                        "@gioDat",
                        ngayDat.TimeOfDay);

                    cmdDatBan.Parameters.AddWithValue(
                        "@soLuongKhach",
                        soKhach);

                    cmdDatBan.Parameters.AddWithValue(
                        "@ghiChu",
                        string.IsNullOrWhiteSpace(ghiChu)
                        ? DBNull.Value
                        : (object)ghiChu);

                    cmdDatBan.Parameters.AddWithValue(
                        "@maBan",
                        maBan);

                    cmdDatBan.Parameters.AddWithValue(
                        "@maKhachHang",
                        maKhachHang);

                    cmdDatBan.ExecuteNonQuery();

                    // Update trạng thái bàn
                    string sqlBan =
                    @"UPDATE Ban
              SET trangThai = N'Đã đặt'
              WHERE maBan = @maBan";

                    SqlCommand cmdBan =
                        new SqlCommand(sqlBan, conn, tran);

                    cmdBan.Parameters.AddWithValue(
                        "@maBan",
                        maBan);

                    cmdBan.ExecuteNonQuery();

                    tran.Commit();

                    return true;
                }
                catch (Exception ex)
                {
                    tran.Rollback();

                    MessageBox.Show(ex.Message);

                    return false;
                }
            }
        }
        public bool Update(Ban ban)
        {
            using (SqlConnection conn = db.GetConnection())
            {
                conn.Open();

                string sql = @"
            UPDATE Ban
            SET
                soBan = @soBan,
                soChoNgoi = @soChoNgoi,
                trangThai = @trangThai
            WHERE maBan = @maBan";

                SqlCommand cmd = new SqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@maBan", ban.MaBan);
                cmd.Parameters.AddWithValue("@soBan", ban.SoBan);
                cmd.Parameters.AddWithValue("@soChoNgoi", ban.SoChoNgoi);
                cmd.Parameters.AddWithValue("@trangThai", ban.TrangThai);

                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }
}