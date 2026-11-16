
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using QuanLyNhaHang.Models;

namespace QuanLyNhaHang.Repositories
{
    public class LoaiMonRepository
    {
        private readonly string connectionString =
            ConfigurationManager.ConnectionStrings[
                "QuanLyNhaHangConnection"].ConnectionString;

        // Lấy danh sách danh mục
        public List<LoaiMon> GetAll()
        {
            List<LoaiMon> danhSach = new List<LoaiMon>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = @"
                    SELECT maLoaiMon, tenLoaiMon, ngayThem
                    FROM LoaiMon
                    ORDER BY maLoaiMon DESC";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            LoaiMon loaiMon = new LoaiMon
                            {
                                MaLoaiMon = Convert.ToInt32(reader["maLoaiMon"]),
                                TenLoaiMon = reader["tenLoaiMon"].ToString(),
                                NgayThem = Convert.ToDateTime(reader["ngayThem"])
                            };

                            danhSach.Add(loaiMon);
                        }
                    }
                }
            }

            return danhSach;
        }

        // Thêm danh mục
        public bool Them(LoaiMon loaiMon)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = @"
                    INSERT INTO LoaiMon (tenLoaiMon)
                    VALUES (@tenLoaiMon)";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add("@tenLoaiMon",
                        SqlDbType.NVarChar, 100).Value =
                        loaiMon.TenLoaiMon;

                    conn.Open();

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // Cập nhật danh mục
        public bool Sua(LoaiMon loaiMon)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = @"
                    UPDATE LoaiMon
                    SET tenLoaiMon = @tenLoaiMon
                    WHERE maLoaiMon = @maLoaiMon";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add("@maLoaiMon",
                        SqlDbType.Int).Value = loaiMon.MaLoaiMon;

                    cmd.Parameters.Add("@tenLoaiMon",
                        SqlDbType.NVarChar, 100).Value =
                        loaiMon.TenLoaiMon;

                    conn.Open();

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // Xóa danh mục
        public bool Xoa(int maLoaiMon)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = @"
                    DELETE FROM LoaiMon
                    WHERE maLoaiMon = @maLoaiMon";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add("@maLoaiMon",
                        SqlDbType.Int).Value = maLoaiMon;

                    conn.Open();

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}
