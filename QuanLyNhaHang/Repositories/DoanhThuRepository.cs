
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace QuanLyNhaHang.Repositories
{
    public class DoanhThuRepository
    {
        private readonly string connectionString =
            ConfigurationManager.ConnectionStrings[
                "QuanLyNhaHangConnection"].ConnectionString;

        // Lấy doanh thu theo từng giờ trong ngày
        public Dictionary<int, decimal> LayDoanhThuTheoGio(
            DateTime ngay)
        {
            Dictionary<int, decimal> ketQua =
                new Dictionary<int, decimal>();

            string query = @"
                SELECT
                    DATEPART(HOUR, gioThanhToan) AS Gio,
                    SUM(soTienThanhToan) AS DoanhThu
                FROM ThanhToan
                WHERE ngayThanhToan = @Ngay
                  AND trangThai = N'Đã thanh toán'
                GROUP BY DATEPART(HOUR, gioThanhToan)
                ORDER BY Gio";

            using (SqlConnection conn =
                new SqlConnection(connectionString))
            using (SqlCommand cmd =
                new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue(
                    "@Ngay",
                    ngay.Date);

                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int gio = Convert.ToInt32(
                            reader["Gio"]);

                        decimal doanhThu = Convert.ToDecimal(
                            reader["DoanhThu"]);

                        ketQua[gio] = doanhThu;
                    }
                }
            }

            return ketQua;
        }
    }
}
