using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using QuanLyNhaHang.Data;
using QuanLyNhaHang.Models;

namespace QuanLyNhaHang.Repositories
{
    public class ThanhToanRepository
    {
        private readonly DbConnection db = new DbConnection();

        // 1. LẤY DANH SÁCH THANH TOÁN
        public List<ThanhToan> GetDanhSachThanhToan(
            string tuKhoa = "",
            string phuongThuc = "",
            string trangThai = "")
        {
            List<ThanhToan> danhSach = new List<ThanhToan>();

            using (SqlConnection conn = db.GetConnection())
            {
                string query = @"
            SELECT
                tt.maThanhToan,
                tt.maHoaDon,
                hd.maPhieuGoiMon,
                pgm.maBan,
                b.soBan,
                tt.soTienThanhToan,
                tt.phuongThucThanhToan,
                tt.ngayThanhToan,
                tt.gioThanhToan,
                tt.trangThai
            FROM dbo.ThanhToan tt
            INNER JOIN dbo.HoaDon hd
                ON tt.maHoaDon = hd.maHoaDon
            LEFT JOIN dbo.PhieuGoiMon pgm
                ON hd.maPhieuGoiMon = pgm.maPhieuGoiMon
            LEFT JOIN dbo.Ban b
                ON pgm.maBan = b.maBan
            WHERE
                (
                    @TuKhoa = N''
                    OR CONVERT(NVARCHAR(20), tt.maThanhToan)
                        LIKE N'%' + @TuKhoa + N'%'
                    OR CONVERT(NVARCHAR(20), tt.maHoaDon)
                        LIKE N'%' + @TuKhoa + N'%'
                )
                AND
                (
                    @PhuongThuc = N''
                    OR tt.phuongThucThanhToan = @PhuongThuc
                )
                AND
                (
                    @TrangThai = N''
                    OR tt.trangThai = @TrangThai
                )
            ORDER BY
                tt.ngayThanhToan DESC,
                tt.gioThanhToan DESC,
                tt.maThanhToan DESC;";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add("@TuKhoa", SqlDbType.NVarChar, 50)
                        .Value = (tuKhoa ?? "").Trim();

                    cmd.Parameters.Add("@PhuongThuc", SqlDbType.NVarChar, 30)
                        .Value = (phuongThuc ?? "").Trim();

                    cmd.Parameters.Add("@TrangThai", SqlDbType.NVarChar, 30)
                        .Value = (trangThai ?? "").Trim();

                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ThanhToan tt = new ThanhToan();

                            tt.MaThanhToan =
                                Convert.ToInt32(reader["maThanhToan"]);

                            tt.MaHoaDon =
                                Convert.ToInt32(reader["maHoaDon"]);

                            tt.MaPhieuGoiMon =
                                reader["maPhieuGoiMon"] == DBNull.Value
                                ? (int?)null
                                : Convert.ToInt32(reader["maPhieuGoiMon"]);

                            tt.MaBan =
                                reader["maBan"] == DBNull.Value
                                ? (int?)null
                                : Convert.ToInt32(reader["maBan"]);

                            tt.TenBan =
                                reader["soBan"] == DBNull.Value
                                ? "Không có thông tin"
                                : "Bàn " + reader["soBan"].ToString();

                            tt.SoTienThanhToan =
                                Convert.ToDecimal(reader["soTienThanhToan"]);

                            tt.PhuongThucThanhToan =
                                reader["phuongThucThanhToan"].ToString();

                            tt.NgayThanhToan =
                                Convert.ToDateTime(reader["ngayThanhToan"]);

                            TimeSpan gio =
                                (TimeSpan)reader["gioThanhToan"];

                            tt.GioThanhToan = DateTime.Today.Add(gio);

                            tt.NgayGioThanhToan =
                                tt.NgayThanhToan.Date.Add(gio);

                            tt.TrangThai =
                                reader["trangThai"].ToString();

                            danhSach.Add(tt);
                        }
                    }
                }
            }

            return danhSach;
        }

        // 2. LẤY CHI TIẾT MỘT GIAO DỊCH
        public ThanhToan GetChiTietThanhToan(int maThanhToan)
        {
            using (SqlConnection conn = db.GetConnection())
            {
                string query = @"
                    SELECT
                        tt.maThanhToan,
                        tt.maHoaDon,
                        hd.maPhieuGoiMon,
                        pgm.maBan,
                        b.soBan,
                        tt.soTienThanhToan,
                        tt.phuongThucThanhToan,
                        tt.ngayThanhToan,
                        tt.gioThanhToan,
                        tt.trangThai
                    FROM dbo.ThanhToan tt
                    INNER JOIN dbo.HoaDon hd
                        ON tt.maHoaDon = hd.maHoaDon
                    LEFT JOIN dbo.PhieuGoiMon pgm
                        ON hd.maPhieuGoiMon = pgm.maPhieuGoiMon
                    LEFT JOIN dbo.Ban b
                        ON pgm.maBan = b.maBan
                    WHERE tt.maThanhToan = @MaThanhToan;";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add("@MaThanhToan", SqlDbType.Int)
                        .Value = maThanhToan;

                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            ThanhToan tt = new ThanhToan();

                            tt.MaThanhToan = Convert.ToInt32(reader["maThanhToan"]);
                            tt.MaHoaDon = Convert.ToInt32(reader["maHoaDon"]);

                            tt.MaPhieuGoiMon = reader["maPhieuGoiMon"] == DBNull.Value
                                ? (int?)null
                                : Convert.ToInt32(reader["maPhieuGoiMon"]);

                            tt.MaBan = reader["maBan"] == DBNull.Value
                                ? (int?)null
                                : Convert.ToInt32(reader["maBan"]);

                            tt.TenBan = reader["soBan"] == DBNull.Value
                                ? "Không có thông tin"
                                : "Bàn " + reader["soBan"].ToString();

                            tt.SoTienThanhToan =
                                Convert.ToDecimal(reader["soTienThanhToan"]);

                            tt.PhuongThucThanhToan =
                                reader["phuongThucThanhToan"].ToString();

                            tt.NgayThanhToan =
                                Convert.ToDateTime(reader["ngayThanhToan"]);

                            TimeSpan gio = (TimeSpan)reader["gioThanhToan"];

                            tt.GioThanhToan = DateTime.Today.Add(gio);
                            tt.NgayGioThanhToan =
                                tt.NgayThanhToan.Date.Add(gio);

                            tt.TrangThai = reader["trangThai"].ToString();

                            return tt;
                        }
                    }
                }
            }

            return null;
        }
        // 3. THỐNG KÊ SỐ LƯỢNG GIAO DỊCH THEO TRẠNG THÁI
        public ThongKeThanhToan GetThongKeThanhToan()
        {
            ThongKeThanhToan thongKe = new ThongKeThanhToan();

            using (SqlConnection conn = db.GetConnection())
            {
                string query = @"
            SELECT
                COUNT(CASE WHEN trangThai = N'Đang xử lý'
                    THEN 1 END) AS ChoXacNhan,

                COUNT(CASE WHEN trangThai = N'Đã thanh toán'
                    THEN 1 END) AS DaThanhToan,

                COUNT(CASE WHEN trangThai = N'Không thành công'
                    THEN 1 END) AS KhongThanhCong
            FROM dbo.ThanhToan;";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            thongKe.ChoXacNhan =
                                Convert.ToInt32(reader["ChoXacNhan"]);

                            thongKe.DaThanhToan =
                                Convert.ToInt32(reader["DaThanhToan"]);

                            thongKe.KhongThanhCong =
                                Convert.ToInt32(reader["KhongThanhCong"]);
                        }
                    }
                }
            }

            return thongKe;
        }

        // 4. XÁC NHẬN GIAO DỊCH CHUYỂN KHOẢN

        // 4. XÁC NHẬN GIAO DỊCH CHUYỂN KHOẢN
        public bool XacNhanThanhToan(int maThanhToan)
        {
            using (SqlConnection conn = db.GetConnection())
            {
                conn.Open();

                using (SqlTransaction tran =
                    conn.BeginTransaction(IsolationLevel.Serializable))
                {
                    try
                    {
                        int maHoaDon;
                        int? maPhieuGoiMon;
                        int? maBan;

                        string phuongThuc;
                        string trangThaiThanhToan;
                        string trangThaiHoaDon;
                        string trangThaiPhieu = "";

                        // 1. Khóa và kiểm tra giao dịch.
                        string queryKiemTra = @"
                    SELECT
                        tt.maHoaDon,
                        tt.phuongThucThanhToan,
                        tt.trangThai AS TrangThaiThanhToan,
                        hd.trangThai AS TrangThaiHoaDon,
                        hd.maPhieuGoiMon,
                        pgm.maBan,
                        pgm.trangThai AS TrangThaiPhieu
                    FROM dbo.ThanhToan tt WITH (UPDLOCK, HOLDLOCK)
                    INNER JOIN dbo.HoaDon hd WITH (UPDLOCK, HOLDLOCK)
                        ON tt.maHoaDon = hd.maHoaDon
                    LEFT JOIN dbo.PhieuGoiMon pgm WITH (UPDLOCK, HOLDLOCK)
                        ON hd.maPhieuGoiMon = pgm.maPhieuGoiMon
                    WHERE tt.maThanhToan = @MaThanhToan;";

                        using (SqlCommand cmd = new SqlCommand(
                            queryKiemTra, conn, tran))
                        {
                            cmd.Parameters.Add("@MaThanhToan", SqlDbType.Int)
                                .Value = maThanhToan;

                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (!reader.Read())
                                    throw new Exception(
                                        "Không tìm thấy giao dịch thanh toán.");

                                maHoaDon = Convert.ToInt32(reader["maHoaDon"]);

                                maPhieuGoiMon =
                                    reader["maPhieuGoiMon"] == DBNull.Value
                                    ? (int?)null
                                    : Convert.ToInt32(reader["maPhieuGoiMon"]);

                                maBan =
                                    reader["maBan"] == DBNull.Value
                                    ? (int?)null
                                    : Convert.ToInt32(reader["maBan"]);

                                phuongThuc =
                                    Convert.ToString(reader["phuongThucThanhToan"]);

                                trangThaiThanhToan =
                                    Convert.ToString(reader["TrangThaiThanhToan"]);

                                trangThaiHoaDon =
                                    Convert.ToString(reader["TrangThaiHoaDon"]);

                                trangThaiPhieu =
                                    reader["TrangThaiPhieu"] == DBNull.Value
                                    ? ""
                                    : Convert.ToString(reader["TrangThaiPhieu"]);
                            }
                        }

                        // 2. Chỉ xác nhận giao dịch chuyển khoản.
                        if (phuongThuc != "Chuyển khoản")
                            throw new Exception(
                                "Chỉ có thể xác nhận giao dịch chuyển khoản.");

                        // 3. Cho phép xác nhận giao dịch đang xử lý
                        // hoặc giao dịch trước đó đã bị từ chối.
                        if (trangThaiThanhToan != "Đang xử lý" &&
                            trangThaiThanhToan != "Không thành công")
                        {
                            throw new Exception(
                                "Giao dịch không ở trạng thái có thể xác nhận.");
                        }

                        // 4. Hóa đơn có thể đang xử lý hoặc chưa thanh toán.
                        if (trangThaiHoaDon != "Đang xử lý" &&
                            trangThaiHoaDon != "Chưa thanh toán")
                        {
                            throw new Exception(
                                "Hóa đơn không ở trạng thái có thể thanh toán.");
                        }

                        // 5. Kiểm tra phiếu gọi món.
                        if (maPhieuGoiMon.HasValue &&
                            trangThaiPhieu != "Đã lập hóa đơn")
                        {
                            throw new Exception(
                                "Phiếu gọi món không ở trạng thái đã lập hóa đơn.");
                        }

                        // 6. Cập nhật giao dịch thành công.
                        string updateThanhToan = @"
                    UPDATE dbo.ThanhToan
                    SET trangThai = N'Đã thanh toán'
                    WHERE maThanhToan = @MaThanhToan
                      AND trangThai IN
                      (
                          N'Đang xử lý',
                          N'Không thành công'
                      );";

                        using (SqlCommand cmd = new SqlCommand(
                            updateThanhToan, conn, tran))
                        {
                            cmd.Parameters.Add("@MaThanhToan", SqlDbType.Int)
                                .Value = maThanhToan;

                            if (cmd.ExecuteNonQuery() != 1)
                                throw new Exception(
                                    "Không thể cập nhật giao dịch thanh toán.");
                        }

                        // 7. Cập nhật hóa đơn thành đã thanh toán.
                        string updateHoaDon = @"
                    UPDATE dbo.HoaDon
                    SET trangThai = N'Đã thanh toán'
                    WHERE maHoaDon = @MaHoaDon
                      AND trangThai IN
                      (
                          N'Đang xử lý',
                          N'Chưa thanh toán'
                      );";

                        using (SqlCommand cmd = new SqlCommand(
                            updateHoaDon, conn, tran))
                        {
                            cmd.Parameters.Add("@MaHoaDon", SqlDbType.Int)
                                .Value = maHoaDon;

                            if (cmd.ExecuteNonQuery() != 1)
                                throw new Exception(
                                    "Không thể cập nhật trạng thái hóa đơn.");
                        }

                        // 8. Cập nhật phiếu gọi món.
                        if (maPhieuGoiMon.HasValue)
                        {
                            string updatePhieu = @"
                        UPDATE dbo.PhieuGoiMon
                        SET trangThai = N'Đã thanh toán'
                        WHERE maPhieuGoiMon = @MaPhieuGoiMon
                          AND trangThai = N'Đã lập hóa đơn';";

                            using (SqlCommand cmd = new SqlCommand(
                                updatePhieu, conn, tran))
                            {
                                cmd.Parameters.Add(
                                    "@MaPhieuGoiMon", SqlDbType.Int)
                                    .Value = maPhieuGoiMon.Value;

                                if (cmd.ExecuteNonQuery() != 1)
                                    throw new Exception(
                                        "Không thể cập nhật trạng thái phiếu gọi món.");
                            }
                        }

                        // 9. Chỉ giải phóng bàn khi không còn đơn chưa hoàn tất.
                        if (maBan.HasValue)
                        {
                            string updateBan = @"
                        UPDATE dbo.Ban
                        SET trangThai = N'Trống'
                        WHERE maBan = @MaBan
                          AND trangThai = N'Đang phục vụ'

                          AND NOT EXISTS
                          (
                              SELECT 1
                              FROM dbo.PhieuGoiMon pgm
                              WHERE pgm.maBan = @MaBan
                                AND pgm.trangThai IN
                                (
                                    N'Chờ xử lý',
                                    N'Đang phục vụ',
                                    N'Đã lập hóa đơn'
                                )
                          )

                          AND NOT EXISTS
                          (
                              SELECT 1
                              FROM dbo.PhieuGoiMon pgm
                              INNER JOIN dbo.HoaDon hd
                                  ON pgm.maPhieuGoiMon = hd.maPhieuGoiMon
                              WHERE pgm.maBan = @MaBan
                                AND hd.trangThai IN
                                (
                                    N'Chưa thanh toán',
                                    N'Đang xử lý'
                                )
                          );";

                            using (SqlCommand cmd = new SqlCommand(
                                updateBan, conn, tran))
                            {
                                cmd.Parameters.Add("@MaBan", SqlDbType.Int)
                                    .Value = maBan.Value;

                                cmd.ExecuteNonQuery();
                            }
                        }

                        tran.Commit();
                        return true;
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
        }


        // 5. TỪ CHỐI GIAO DỊCH CHUYỂN KHOẢN
        public bool TuChoiThanhToan(int maThanhToan)
        {
            using (SqlConnection conn = db.GetConnection())
            {
                conn.Open();

                using (SqlTransaction tran =
                    conn.BeginTransaction(IsolationLevel.Serializable))
                {
                    try
                    {
                        int maHoaDon;
                        string phuongThuc;
                        string trangThaiThanhToan;
                        string trangThaiHoaDon;

                        // 1. Khóa và kiểm tra giao dịch.
                        string queryKiemTra = @"
                    SELECT
                        tt.maHoaDon,
                        tt.phuongThucThanhToan,
                        tt.trangThai AS TrangThaiThanhToan,
                        hd.trangThai AS TrangThaiHoaDon
                    FROM dbo.ThanhToan tt WITH (UPDLOCK, HOLDLOCK)
                    INNER JOIN dbo.HoaDon hd WITH (UPDLOCK, HOLDLOCK)
                        ON tt.maHoaDon = hd.maHoaDon
                    WHERE tt.maThanhToan = @MaThanhToan;";

                        using (SqlCommand cmd = new SqlCommand(
                            queryKiemTra, conn, tran))
                        {
                            cmd.Parameters.Add("@MaThanhToan", SqlDbType.Int)
                                .Value = maThanhToan;

                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (!reader.Read())
                                    throw new Exception(
                                        "Không tìm thấy giao dịch thanh toán.");

                                maHoaDon = Convert.ToInt32(reader["maHoaDon"]);

                                phuongThuc =
                                    Convert.ToString(reader["phuongThucThanhToan"]);

                                trangThaiThanhToan =
                                    Convert.ToString(reader["TrangThaiThanhToan"]);

                                trangThaiHoaDon =
                                    Convert.ToString(reader["TrangThaiHoaDon"]);
                            }
                        }

                        // 2. Kiểm tra phương thức.
                        if (phuongThuc != "Chuyển khoản")
                            throw new Exception(
                                "Chỉ có thể từ chối giao dịch chuyển khoản.");

                        // 3. Kiểm tra trạng thái.
                        if (trangThaiThanhToan != "Đang xử lý")
                            throw new Exception(
                                "Giao dịch không còn ở trạng thái đang xử lý.");

                        if (trangThaiHoaDon != "Đang xử lý")
                            throw new Exception(
                                "Hóa đơn không còn ở trạng thái đang xử lý.");

                        // 4. Đánh dấu giao dịch không thành công.
                        string updateThanhToan = @"
                    UPDATE dbo.ThanhToan
                    SET trangThai = N'Không thành công'
                    WHERE maThanhToan = @MaThanhToan
                      AND trangThai = N'Đang xử lý';";

                        using (SqlCommand cmd = new SqlCommand(
                            updateThanhToan, conn, tran))
                        {
                            cmd.Parameters.Add("@MaThanhToan", SqlDbType.Int)
                                .Value = maThanhToan;

                            if (cmd.ExecuteNonQuery() != 1)
                                throw new Exception(
                                    "Không thể cập nhật trạng thái giao dịch.");
                        }

                        // 5. Đưa hóa đơn về chưa thanh toán.
                        string updateHoaDon = @"
                    UPDATE dbo.HoaDon
                    SET trangThai = N'Chưa thanh toán'
                    WHERE maHoaDon = @MaHoaDon
                      AND trangThai = N'Đang xử lý';";

                        using (SqlCommand cmd = new SqlCommand(
                            updateHoaDon, conn, tran))
                        {
                            cmd.Parameters.Add("@MaHoaDon", SqlDbType.Int)
                                .Value = maHoaDon;

                            if (cmd.ExecuteNonQuery() != 1)
                                throw new Exception(
                                    "Không thể cập nhật trạng thái hóa đơn.");
                        }

                        tran.Commit();
                        return true;
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}