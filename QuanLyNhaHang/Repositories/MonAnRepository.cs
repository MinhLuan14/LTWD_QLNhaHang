
using QuanLyNhaHang.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace QuanLyNhaHang.Repositories
{
    public class MonAnRepository
    {
        private readonly string connectionString =
            ConfigurationManager.ConnectionStrings[
                "QuanLyNhaHangConnection"].ConnectionString;

        // 1. Lấy toàn bộ danh sách món ăn


        public List<MonAn> GetAll()
        {
            List<MonAn> danhSach = new List<MonAn>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = @"
            SELECT
                m.maMonAn,
                m.tenMonAn,
                m.donGia,
                m.ngayThem,
                m.trangThai,
                m.maLoaiMon,
                m.donVi,
                m.hinhMon,
                l.tenLoaiMon
            FROM dbo.MonAn m
            LEFT JOIN dbo.LoaiMon l
                ON m.maLoaiMon = l.maLoaiMon
            ORDER BY m.maMonAn DESC";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            danhSach.Add(new MonAn
                            {
                                MaMonAn = Convert.ToInt32(reader["maMonAn"]),
                                TenMonAn = reader["tenMonAn"].ToString(),
                                DonGia = Convert.ToDecimal(reader["donGia"]),
                                NgayThem = Convert.ToDateTime(reader["ngayThem"]),
                                TrangThai = Convert.ToBoolean(reader["trangThai"]),
                                MaLoaiMon = Convert.ToInt32(reader["maLoaiMon"]),
                                DonVi = reader["donVi"] == DBNull.Value
                                    ? null : reader["donVi"].ToString(),
                                HinhMon = reader["hinhMon"] == DBNull.Value
                                    ? null : reader["hinhMon"].ToString(),
                                TenLoaiMon = reader["tenLoaiMon"] == DBNull.Value
                                    ? "Chưa phân loại"
                                    : reader["tenLoaiMon"].ToString()
                            });
                        }
                    }
                }
            }

            return danhSach;
        }



        // 2. Lấy món ăn theo mã
        public MonAn GetById(int maMonAn)
        {
            using (SqlConnection conn =
                new SqlConnection(connectionString))
            {
                string sql = @"
                    SELECT 
                        m.maMonAn,
                        m.tenMonAn,
                        m.donGia,
                        m.ngayThem,
                        m.trangThai,
                        m.maLoaiMon,
                        m.donVi,
                        m.hinhMon,
                        l.tenLoaiMon
                    FROM MonAn m
                    INNER JOIN LoaiMon l
                        ON m.maLoaiMon = l.maLoaiMon
                    WHERE m.maMonAn = @maMonAn";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add("@maMonAn",
                        SqlDbType.Int).Value = maMonAn;

                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new MonAn
                            {
                                MaMonAn = Convert.ToInt32(reader["maMonAn"]),
                                TenMonAn = reader["tenMonAn"].ToString(),
                                DonGia = Convert.ToDecimal(reader["donGia"]),
                                NgayThem = Convert.ToDateTime(reader["ngayThem"]),
                                TrangThai = Convert.ToBoolean(reader["trangThai"]),
                                MaLoaiMon = Convert.ToInt32(reader["maLoaiMon"]),
                                DonVi = reader["donVi"] == DBNull.Value
                                    ? null : reader["donVi"].ToString(),
                                HinhMon = reader["hinhMon"] == DBNull.Value
                                    ? null : reader["hinhMon"].ToString(),
                                TenLoaiMon = reader["tenLoaiMon"].ToString()
                            };
                        }
                    }
                }
            }

            return null;
        }

        // 3. Thêm món ăn
        public bool Them(MonAn monAn)
        {
            using (SqlConnection conn =
                new SqlConnection(connectionString))
            {
                string sql = @"
                    INSERT INTO MonAn
                    (
                        tenMonAn,
                        donGia,
                        trangThai,
                        maLoaiMon,
                        donVi,
                        hinhMon
                    )
                    VALUES
                    (
                        @tenMonAn,
                        @donGia,
                        @trangThai,
                        @maLoaiMon,
                        @donVi,
                        @hinhMon
                    )";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add("@tenMonAn",
                        SqlDbType.NVarChar, 255).Value =
                        monAn.TenMonAn;

                    cmd.Parameters.Add("@donGia",
                        SqlDbType.Decimal).Value = monAn.DonGia;

                    cmd.Parameters["@donGia"].Precision = 18;
                    cmd.Parameters["@donGia"].Scale = 2;

                    cmd.Parameters.Add("@trangThai",
                        SqlDbType.Bit).Value = monAn.TrangThai;

                    cmd.Parameters.Add("@maLoaiMon",
                        SqlDbType.Int).Value = monAn.MaLoaiMon;

                    cmd.Parameters.Add("@donVi",
                        SqlDbType.NVarChar, 50).Value =
                        string.IsNullOrWhiteSpace(monAn.DonVi)
                            ? (object)DBNull.Value : monAn.DonVi;

                    cmd.Parameters.Add("@hinhMon",
                        SqlDbType.NVarChar, 500).Value =
                        string.IsNullOrWhiteSpace(monAn.HinhMon)
                            ? (object)DBNull.Value : monAn.HinhMon;

                    conn.Open();

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // 4. Cập nhật món ăn
        public bool Sua(MonAn monAn)
        {
            using (SqlConnection conn =
                new SqlConnection(connectionString))
            {
                string sql = @"
                    UPDATE MonAn
                    SET
                        tenMonAn = @tenMonAn,
                        donGia = @donGia,
                        trangThai = @trangThai,
                        maLoaiMon = @maLoaiMon,
                        donVi = @donVi,
                        hinhMon = @hinhMon
                    WHERE maMonAn = @maMonAn";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add("@maMonAn",
                        SqlDbType.Int).Value = monAn.MaMonAn;

                    cmd.Parameters.Add("@tenMonAn",
                        SqlDbType.NVarChar, 255).Value =
                        monAn.TenMonAn;

                    cmd.Parameters.Add("@donGia",
                        SqlDbType.Decimal).Value = monAn.DonGia;

                    cmd.Parameters["@donGia"].Precision = 18;
                    cmd.Parameters["@donGia"].Scale = 2;

                    cmd.Parameters.Add("@trangThai",
                        SqlDbType.Bit).Value = monAn.TrangThai;

                    cmd.Parameters.Add("@maLoaiMon",
                        SqlDbType.Int).Value = monAn.MaLoaiMon;

                    cmd.Parameters.Add("@donVi",
                        SqlDbType.NVarChar, 50).Value =
                        string.IsNullOrWhiteSpace(monAn.DonVi)
                            ? (object)DBNull.Value : monAn.DonVi;

                    cmd.Parameters.Add("@hinhMon",
                        SqlDbType.NVarChar, 500).Value =
                        string.IsNullOrWhiteSpace(monAn.HinhMon)
                            ? (object)DBNull.Value : monAn.HinhMon;

                    conn.Open();

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // 5. Cập nhật trạng thái món ăn
        public bool CapNhatTrangThai(int maMonAn, bool trangThai)
        {
            using (SqlConnection conn =
                new SqlConnection(connectionString))
            {
                string sql = @"
                    UPDATE MonAn
                    SET trangThai = @trangThai
                    WHERE maMonAn = @maMonAn";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add("@maMonAn",
                        SqlDbType.Int).Value = maMonAn;

                    cmd.Parameters.Add("@trangThai",
                        SqlDbType.Bit).Value = trangThai;

                    conn.Open();

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // 6. Tìm kiếm món ăn theo tên
        public List<MonAn> TimKiem(string tuKhoa)
        {
            List<MonAn> danhSach = new List<MonAn>();

            using (SqlConnection conn =
                new SqlConnection(connectionString))
            {
                string sql = @"
                    SELECT
                        m.maMonAn,
                        m.tenMonAn,
                        m.donGia,
                        m.ngayThem,
                        m.trangThai,
                        m.maLoaiMon,
                        m.donVi,
                        m.hinhMon,
                        l.tenLoaiMon
                    FROM MonAn m
                    INNER JOIN LoaiMon l
                        ON m.maLoaiMon = l.maLoaiMon
                    WHERE m.tenMonAn LIKE @tuKhoa
                    ORDER BY m.maMonAn DESC";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add("@tuKhoa",
                        SqlDbType.NVarChar, 255).Value =
                        "%" + tuKhoa + "%";

                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            danhSach.Add(new MonAn
                            {
                                MaMonAn = Convert.ToInt32(reader["maMonAn"]),
                                TenMonAn = reader["tenMonAn"].ToString(),
                                DonGia = Convert.ToDecimal(reader["donGia"]),
                                NgayThem = Convert.ToDateTime(reader["ngayThem"]),
                                TrangThai = Convert.ToBoolean(reader["trangThai"]),
                                MaLoaiMon = Convert.ToInt32(reader["maLoaiMon"]),
                                DonVi = reader["donVi"] == DBNull.Value
                                    ? null : reader["donVi"].ToString(),
                                HinhMon = reader["hinhMon"] == DBNull.Value
                                    ? null : reader["hinhMon"].ToString(),
                                TenLoaiMon = reader["tenLoaiMon"].ToString()
                            });
                        }
                    }
                }
            }

            return danhSach;
        }

        // 7. Lọc món ăn theo danh mục
        public List<MonAn> LocTheoDanhMuc(int maLoaiMon)
        {
            List<MonAn> danhSach = new List<MonAn>();

            using (SqlConnection conn =
                new SqlConnection(connectionString))
            {
                string sql = @"
                    SELECT
                        m.maMonAn,
                        m.tenMonAn,
                        m.donGia,
                        m.ngayThem,
                        m.trangThai,
                        m.maLoaiMon,
                        m.donVi,
                        m.hinhMon,
                        l.tenLoaiMon
                    FROM MonAn m
                    INNER JOIN LoaiMon l
                        ON m.maLoaiMon = l.maLoaiMon
                    WHERE m.maLoaiMon = @maLoaiMon
                    ORDER BY m.maMonAn DESC";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add("@maLoaiMon",
                        SqlDbType.Int).Value = maLoaiMon;

                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            danhSach.Add(new MonAn
                            {
                                MaMonAn = Convert.ToInt32(reader["maMonAn"]),
                                TenMonAn = reader["tenMonAn"].ToString(),
                                DonGia = Convert.ToDecimal(reader["donGia"]),
                                NgayThem = Convert.ToDateTime(reader["ngayThem"]),
                                TrangThai = Convert.ToBoolean(reader["trangThai"]),
                                MaLoaiMon = Convert.ToInt32(reader["maLoaiMon"]),
                                DonVi = reader["donVi"] == DBNull.Value
                                    ? null : reader["donVi"].ToString(),
                                HinhMon = reader["hinhMon"] == DBNull.Value
                                    ? null : reader["hinhMon"].ToString(),
                                TenLoaiMon = reader["tenLoaiMon"].ToString()
                            });
                        }
                    }
                }
            }

            return danhSach;
        }
        // 8. Xóa món ăn
        public bool Xoa(int maMonAn)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = @"
            DELETE FROM MonAn
            WHERE maMonAn = @maMonAn";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add("@maMonAn", SqlDbType.Int).Value = maMonAn;

                    conn.Open();

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}
