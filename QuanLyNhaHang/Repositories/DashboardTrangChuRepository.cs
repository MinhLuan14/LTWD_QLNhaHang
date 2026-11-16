using QuanLyNhaHang.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyNhaHang.Repositories
{
    public class DashboardTrangChuRepository
    {
        // Lấy chuỗi kết nối database QuanLyNhaHang.
        private readonly string connectionString =
            ConfigurationManager.ConnectionStrings[
                "QuanLyNhaHangConnection"].ConnectionString;

        // Lấy các chỉ số thống kê tổng quan của Dashboard.
        public ThongKeTrangChuDashboard LayThongKeTongQuan(DateTime ngay)
        {
            ThongKeTrangChuDashboard thongKe =
                new ThongKeTrangChuDashboard();

            string query = @"
                SELECT
                    (
                        SELECT ISNULL(SUM(soTienThanhToan), 0)
                        FROM dbo.ThanhToan
                        WHERE ngayThanhToan = @Ngay
                          AND trangThai = N'Đã thanh toán'
                    ) AS TongDoanhThu,

                    (
                        SELECT COUNT(*)
                        FROM dbo.HoaDon
                        WHERE ngayLap = @Ngay
                    ) AS TongHoaDon,

                    (
                        SELECT COUNT(*)
                        FROM dbo.Ban
                        WHERE trangThai = N'Trống'
                    ) AS BanTrong,

                    (
                        SELECT COUNT(*)
                        FROM dbo.Ban
                        WHERE trangThai = N'Đang phục vụ'
                    ) AS DangPhucVu,

                    (
                        SELECT COUNT(*)
                        FROM dbo.HoaDon
                        WHERE trangThai = N'Chưa thanh toán'
                    ) AS HoaDonCanXuLy,

                    (
                        SELECT COUNT(*)
                        FROM dbo.PhieuGoiMon
                        WHERE trangThai = N'Chờ xử lý'
                    ) AS PhieuGoiMonCanXuLy,

                    (
                        SELECT COUNT(*)
                        FROM dbo.DatBan
                        WHERE trangThai = N'Chờ xác nhận'
                    ) AS DatBanCanXuLy;
            ";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.Add("@Ngay", SqlDbType.Date).Value = ngay.Date;

                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        thongKe.TongDoanhThu =
                            Convert.ToDecimal(reader["TongDoanhThu"]);

                        thongKe.TongHoaDon =
                            Convert.ToInt32(reader["TongHoaDon"]);

                        thongKe.BanTrong =
                            Convert.ToInt32(reader["BanTrong"]);

                        thongKe.DangPhucVu =
                            Convert.ToInt32(reader["DangPhucVu"]);

                        thongKe.HoaDonCanXuLy =
                            Convert.ToInt32(reader["HoaDonCanXuLy"]);

                        thongKe.PhieuGoiMonCanXuLy =
                            Convert.ToInt32(reader["PhieuGoiMonCanXuLy"]);

                        thongKe.DatBanCanXuLy =
                            Convert.ToInt32(reader["DatBanCanXuLy"]);
                    }
                }
            }

            thongKe.TongViecCanXuLy =
                thongKe.HoaDonCanXuLy +
                thongKe.PhieuGoiMonCanXuLy +
                thongKe.DatBanCanXuLy;

            return thongKe;
        }

        // Lấy danh sách hóa đơn mới nhất kèm số bàn.
        public DataTable LayHoaDonGanDay(int soLuong)
        {
            DataTable dt = new DataTable();

            string query = @"
                SELECT TOP (@SoLuong)
                    hd.maHoaDon AS MaHoaDon,

                    CASE
                        WHEN b.soBan IS NOT NULL
                            THEN N'Bàn ' + CAST(b.soBan AS NVARCHAR(10))
                        ELSE N'Chưa xác định'
                    END AS Ban,

                    CONVERT(VARCHAR(5), hd.gioLap, 108) AS ThoiGian,

                    hd.tongTien AS TongTien,

                    hd.trangThai AS TrangThai

                FROM dbo.HoaDon hd

                LEFT JOIN dbo.PhieuGoiMon pgm
                    ON hd.maPhieuGoiMon = pgm.maPhieuGoiMon

                LEFT JOIN dbo.Ban b
                    ON pgm.maBan = b.maBan

                ORDER BY
                    hd.ngayLap DESC,
                    hd.gioLap DESC,
                    hd.maHoaDon DESC;
            ";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.Add("@SoLuong", SqlDbType.Int).Value =
                    Math.Max(1, soLuong);

                adapter.Fill(dt);
            }

            return dt;
        }

        // Lấy tổng tiền thanh toán thành công theo từng giờ trong ngày.
        public DataTable LayDoanhThuTheoGio(DateTime ngay)
        {
            DataTable dt = new DataTable();

            string query = @"
                SELECT
                    DATEPART(HOUR, gioThanhToan) AS Gio,
                    SUM(soTienThanhToan) AS DoanhThu

                FROM dbo.ThanhToan

                WHERE ngayThanhToan = @Ngay
                  AND trangThai = N'Đã thanh toán'

                GROUP BY DATEPART(HOUR, gioThanhToan)

                ORDER BY Gio;
            ";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.Add("@Ngay", SqlDbType.Date).Value =
                    ngay.Date;

                adapter.Fill(dt);
            }

            return dt;
        }

        // Đếm số bàn theo từng trạng thái hiện tại.
        public DataTable LayTinhTrangBan()
        {
            DataTable dt = new DataTable();

            string query = @"
                SELECT
                    trangThai AS TrangThai,
                    COUNT(*) AS SoLuong

                FROM dbo.Ban

                GROUP BY trangThai

                ORDER BY
                    CASE trangThai
                        WHEN N'Trống' THEN 1
                        WHEN N'Đang phục vụ' THEN 2
                        WHEN N'Đã đặt' THEN 3
                        ELSE 4
                    END;
            ";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
            {
                adapter.Fill(dt);
            }

            return dt;
        }

        // Lấy toàn bộ dữ liệu cần thiết cho Dashboard trang chủ.
        public ThongKeTrangChuDashboard LayDuLieuDashboard(DateTime ngay)
        {
            ThongKeTrangChuDashboard data = LayThongKeTongQuan(ngay);

            data.HoaDonGanDay = LayHoaDonGanDay(5);
            data.DoanhThuTheoGio = LayDoanhThuTheoGio(ngay);
            data.TinhTrangBan = LayTinhTrangBan();

            return data;
        }
    }
}
