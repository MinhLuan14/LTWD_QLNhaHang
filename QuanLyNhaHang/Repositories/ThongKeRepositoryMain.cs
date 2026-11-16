
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

using QuanLyNhaHang.Data;
using QuanLyNhaHang.Models;

namespace QuanLyNhaHang.Repositories
{
    public class ThongKeRepositoryMain
    {
        private readonly DbConnection db = new DbConnection();

        // 1. LẤY THỐNG KÊ TỔNG QUAN
        public ThongKeTongQuan GetThongKeTongQuan(
            DateTime tuNgay,
            DateTime denNgay)
        {
            ThongKeTongQuan thongKe = new ThongKeTongQuan();

            string sql = @"
                SELECT
                    ISNULL(SUM(tt.soTienThanhToan), 0) AS TongDoanhThu,
                    COUNT(hd.maHoaDon) AS SoHoaDon,
                    CASE
                        WHEN COUNT(hd.maHoaDon) = 0 THEN 0
                        ELSE
                            SUM(tt.soTienThanhToan) * 1.0
                            / COUNT(hd.maHoaDon)
                    END AS TrungBinhHoaDon
                FROM HoaDon hd
                INNER JOIN ThanhToan tt
                    ON hd.maHoaDon = tt.maHoaDon
                WHERE hd.trangThai = N'Đã thanh toán'
                    AND tt.trangThai = N'Đã thanh toán'
                    AND hd.ngayLap >= @TuNgay
                    AND hd.ngayLap < DATEADD(DAY, 1, @DenNgay);";

            using (SqlConnection conn = db.GetConnection())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add("@TuNgay", SqlDbType.Date).Value =
                    tuNgay.Date;

                cmd.Parameters.Add("@DenNgay", SqlDbType.Date).Value =
                    denNgay.Date;

                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        thongKe.TongDoanhThu =
                            Convert.ToDecimal(reader["TongDoanhThu"]);

                        thongKe.SoHoaDon =
                            Convert.ToInt32(reader["SoHoaDon"]);

                        thongKe.TrungBinhHoaDon =
                            Convert.ToDecimal(reader["TrungBinhHoaDon"]);
                    }
                }
            }

            return thongKe;
        }

        // 2. LẤY DOANH THU THEO NGÀY
        public List<ThongKeDoanhThuNgay> GetDoanhThuTheoNgay(
            DateTime tuNgay,
            DateTime denNgay)
        {
            List<ThongKeDoanhThuNgay> danhSach =
                new List<ThongKeDoanhThuNgay>();

            string sql = @"
                SELECT
                    CAST(hd.ngayLap AS DATE) AS Ngay,
                    SUM(tt.soTienThanhToan) AS DoanhThu
                FROM HoaDon hd
                INNER JOIN ThanhToan tt
                    ON hd.maHoaDon = tt.maHoaDon
                WHERE hd.trangThai = N'Đã thanh toán'
                    AND tt.trangThai = N'Đã thanh toán'
                    AND hd.ngayLap >= @TuNgay
                    AND hd.ngayLap < DATEADD(DAY, 1, @DenNgay)
                GROUP BY CAST(hd.ngayLap AS DATE)
                ORDER BY CAST(hd.ngayLap AS DATE);";

            using (SqlConnection conn = db.GetConnection())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add("@TuNgay", SqlDbType.Date).Value =
                    tuNgay.Date;

                cmd.Parameters.Add("@DenNgay", SqlDbType.Date).Value =
                    denNgay.Date;

                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ThongKeDoanhThuNgay item =
                            new ThongKeDoanhThuNgay();

                        item.Ngay =
                            Convert.ToDateTime(reader["Ngay"]);

                        item.DoanhThu =
                            Convert.ToDecimal(reader["DoanhThu"]);

                        danhSach.Add(item);
                    }
                }
            }

            return danhSach;
        }

        // 3. LẤY TOP MÓN BÁN CHẠY
        public List<ThongKeMonBanChay> GetTopMonBanChay(
            DateTime tuNgay,
            DateTime denNgay,
            int topN = 5)
        {
            List<ThongKeMonBanChay> danhSach =
                new List<ThongKeMonBanChay>();

            string sql = @"
                SELECT TOP (@TopN)
                    ma.tenMonAn AS TenMon,
                    SUM(cthd.soLuong) AS SoLuong,
                    SUM(cthd.thanhTien) AS DoanhThu
                FROM HoaDon hd
                INNER JOIN ThanhToan tt
                    ON hd.maHoaDon = tt.maHoaDon
                INNER JOIN ChiTietHoaDon cthd
                    ON hd.maHoaDon = cthd.maHoaDon
                INNER JOIN MonAn ma
                    ON cthd.maMonAn = ma.maMonAn
                WHERE hd.trangThai = N'Đã thanh toán'
                    AND tt.trangThai = N'Đã thanh toán'
                    AND hd.ngayLap >= @TuNgay
                    AND hd.ngayLap < DATEADD(DAY, 1, @DenNgay)
                GROUP BY
                    ma.maMonAn,
                    ma.tenMonAn
                ORDER BY
                    SUM(cthd.soLuong) DESC,
                    SUM(cthd.thanhTien) DESC;";

            using (SqlConnection conn = db.GetConnection())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add("@TopN", SqlDbType.Int).Value =
                    topN;

                cmd.Parameters.Add("@TuNgay", SqlDbType.Date).Value =
                    tuNgay.Date;

                cmd.Parameters.Add("@DenNgay", SqlDbType.Date).Value =
                    denNgay.Date;

                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ThongKeMonBanChay item =
                            new ThongKeMonBanChay();

                        item.TenMon =
                            reader["TenMon"].ToString();

                        item.SoLuong =
                            Convert.ToInt32(reader["SoLuong"]);

                        item.DoanhThu =
                            Convert.ToDecimal(reader["DoanhThu"]);

                        danhSach.Add(item);
                    }
                }
            }

            return danhSach;
        }

        // 4. THỐNG KÊ HÓA ĐƠN THEO 8 KHUNG GIỜ
        public List<ThongKeKhungGio> GetThongKeKhungGio(
            DateTime tuNgay,
            DateTime denNgay)
        {
            List<ThongKeKhungGio> danhSach =
                new List<ThongKeKhungGio>();

            string sql = @"
                WITH KhungGio AS
                (
                    SELECT *
                    FROM
                    (
                        VALUES
                            (1, N'07-09'),
                            (2, N'09-11'),
                            (3, N'11-13'),
                            (4, N'13-15'),
                            (5, N'15-17'),
                            (6, N'17-19'),
                            (7, N'19-21'),
                            (8, N'21-23')
                    ) AS K(ThuTu, TenKhungGio)
                ),
                DuLieuHoaDon AS
                (
                    SELECT
                        CASE
                            WHEN DATEPART(HOUR, hd.gioLap) >= 7
                                AND DATEPART(HOUR, hd.gioLap) < 9 THEN 1

                            WHEN DATEPART(HOUR, hd.gioLap) >= 9
                                AND DATEPART(HOUR, hd.gioLap) < 11 THEN 2

                            WHEN DATEPART(HOUR, hd.gioLap) >= 11
                                AND DATEPART(HOUR, hd.gioLap) < 13 THEN 3

                            WHEN DATEPART(HOUR, hd.gioLap) >= 13
                                AND DATEPART(HOUR, hd.gioLap) < 15 THEN 4

                            WHEN DATEPART(HOUR, hd.gioLap) >= 15
                                AND DATEPART(HOUR, hd.gioLap) < 17 THEN 5

                            WHEN DATEPART(HOUR, hd.gioLap) >= 17
                                AND DATEPART(HOUR, hd.gioLap) < 19 THEN 6

                            WHEN DATEPART(HOUR, hd.gioLap) >= 19
                                AND DATEPART(HOUR, hd.gioLap) < 21 THEN 7

                            WHEN DATEPART(HOUR, hd.gioLap) >= 21
                                AND DATEPART(HOUR, hd.gioLap) < 23 THEN 8

                            ELSE NULL
                        END AS ThuTu,
                        hd.maHoaDon
                    FROM HoaDon hd
                    INNER JOIN ThanhToan tt
                        ON hd.maHoaDon = tt.maHoaDon
                    WHERE hd.trangThai = N'Đã thanh toán'
                        AND tt.trangThai = N'Đã thanh toán'
                        AND hd.ngayLap >= @TuNgay
                        AND hd.ngayLap < DATEADD(DAY, 1, @DenNgay)
                ),
                ThongKe AS
                (
                    SELECT
                        ThuTu,
                        COUNT(maHoaDon) AS SoHoaDon
                    FROM DuLieuHoaDon
                    WHERE ThuTu IS NOT NULL
                    GROUP BY ThuTu
                )
                SELECT
                    kg.TenKhungGio,
                    ISNULL(tk.SoHoaDon, 0) AS SoHoaDon
                FROM KhungGio kg
                LEFT JOIN ThongKe tk
                    ON kg.ThuTu = tk.ThuTu
                ORDER BY kg.ThuTu;";

            using (SqlConnection conn = db.GetConnection())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add("@TuNgay", SqlDbType.Date).Value =
                    tuNgay.Date;

                cmd.Parameters.Add("@DenNgay", SqlDbType.Date).Value =
                    denNgay.Date;

                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ThongKeKhungGio item =
                            new ThongKeKhungGio();

                        item.KhungGio =
                            reader["TenKhungGio"].ToString();

                        item.SoHoaDon =
                            Convert.ToInt32(reader["SoHoaDon"]);

                        danhSach.Add(item);
                    }
                }
            }

            return danhSach;
        }
    }
}
