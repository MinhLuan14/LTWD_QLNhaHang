
using System;
using System.Data.SqlClient;
using QuanLyNhaHang.Data;
using QuanLyNhaHang.Models;

namespace QuanLyNhaHang.Repositories
{
    public class ThongKeRepository
    {
        private readonly DbConnection dbConnection = new DbConnection();

        public ThongKeBan GetThongKeBan()
        {
            ThongKeBan thongKe = new ThongKeBan();

            string query = @"
                SELECT
                    COUNT(*) AS TongSoBan,

                    ISNULL(SUM(CASE
                        WHEN LTRIM(RTRIM(trangThai)) = N'Trống'
                        THEN 1 ELSE 0
                    END), 0) AS SoBanTrong,

                    ISNULL(SUM(CASE
                        WHEN LTRIM(RTRIM(trangThai)) = N'Đang phục vụ'
                        THEN 1 ELSE 0
                    END), 0) AS SoBanDangPhucVu,

                    ISNULL(SUM(CASE
                        WHEN LTRIM(RTRIM(trangThai)) = N'Đã đặt'
                        THEN 1 ELSE 0
                    END), 0) AS SoBanDaDat

                FROM Ban;";

            using (SqlConnection conn = dbConnection.GetConnection())
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        thongKe.TongSoBan =
                            Convert.ToInt32(reader["TongSoBan"]);

                        thongKe.SoBanTrong =
                            Convert.ToInt32(reader["SoBanTrong"]);

                        thongKe.SoBanDangPhucVu =
                            Convert.ToInt32(reader["SoBanDangPhucVu"]);

                        thongKe.SoBanDaDat =
                            Convert.ToInt32(reader["SoBanDaDat"]);
                    }
                }
            }

            return thongKe;
        }
    }
}
