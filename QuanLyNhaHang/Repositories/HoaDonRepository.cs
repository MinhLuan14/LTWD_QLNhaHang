
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using QuanLyNhaHang.Data;
using QuanLyNhaHang.Models;

namespace QuanLyNhaHang.Repositories
{
    public class HoaDonRepository
    {
        private readonly DbConnection db = new DbConnection();

        // Kiểm tra cột maPhieuGoiMon có tồn tại trong HoaDon không.
        // Không cache kết quả để tránh dữ liệu kiểm tra bị cũ.
        private bool HasColMaPhieu(SqlConnection conn, SqlTransaction tran = null)
        {
            const string sql = @"
                SELECT COUNT(*)
                FROM INFORMATION_SCHEMA.COLUMNS
                WHERE TABLE_SCHEMA = 'dbo'
                  AND TABLE_NAME = 'HoaDon'
                  AND COLUMN_NAME = 'maPhieuGoiMon'";

            using (SqlCommand cmd = new SqlCommand(sql, conn, tran))
            {
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        // 1. LẤY DANH SÁCH HÓA ĐƠN
        public List<HoaDon> GetDanhSachHoaDon(
            DateTime tuNgay,
            DateTime denNgay,
            string trangThai = null,
            string keyword = null)
        {
            if (tuNgay.Date > denNgay.Date)
                throw new ArgumentException("Ngày bắt đầu không được lớn hơn ngày kết thúc.");

            List<HoaDon> danhSach = new List<HoaDon>();

            using (SqlConnection conn = db.GetConnection())
            {
                conn.Open();

                bool hasMaPhieu = HasColMaPhieu(conn);

                string sql;

                if (hasMaPhieu)
                {
                    sql = @"
                        SELECT
                            h.maHoaDon,
                            h.ngayLap,
                            h.gioLap,
                            h.tongTien,
                            ISNULL(h.giamGia, 0) AS giamGia,
                            ISNULL(h.tienThue, 0) AS tienThue,
                            h.trangThai,
                            h.maNhanVien,
                            ISNULL(
                                NULLIF(
                                    LTRIM(RTRIM(
                                        ISNULL(nv.hoVaTenLot, N'') + N' ' +
                                        ISNULL(nv.tenNhanVien, N'')
                                    )),
                                    N''
                                ),
                                N'Nhân viên'
                            ) AS tenNhanVien,
                            h.maPhieuGoiMon,
                            p.maBan,
                            b.soBan,
                            ISNULL(tt.phuongThucThanhToan, N'Tiền mặt')
                                AS phuongThucThanhToan
                        FROM HoaDon h
                        LEFT JOIN NhanVien nv
                            ON h.maNhanVien = nv.maNhanVien
                        LEFT JOIN PhieuGoiMon p
                            ON h.maPhieuGoiMon = p.maPhieuGoiMon
                        LEFT JOIN Ban b
                            ON p.maBan = b.maBan
                        OUTER APPLY
                        (
                            SELECT TOP 1
                                t.phuongThucThanhToan
                            FROM ThanhToan t
                            WHERE t.maHoaDon = h.maHoaDon
                            ORDER BY t.ngayThanhToan DESC, t.gioThanhToan DESC
                        ) tt
                        WHERE h.ngayLap >= @tuNgay
                          AND h.ngayLap < DATEADD(DAY, 1, @denNgay)";
                }
                else
                {
                    sql = @"
                        SELECT
                            h.maHoaDon,
                            h.ngayLap,
                            h.gioLap,
                            h.tongTien,
ISNULL(h.giamGia, 0) AS giamGia,
ISNULL(h.tienThue, 0) AS tienThue,
h.trangThai,
                            h.maNhanVien,
                            ISNULL(
                                NULLIF(
                                    LTRIM(RTRIM(
                                        ISNULL(nv.hoVaTenLot, N'') + N' ' +
                                        ISNULL(nv.tenNhanVien, N'')
                                    )),
                                    N''
                                ),
                                N'Nhân viên'
                            ) AS tenNhanVien,
                            ISNULL(tt.phuongThucThanhToan, N'Tiền mặt')
                                AS phuongThucThanhToan
                        FROM HoaDon h
                        LEFT JOIN NhanVien nv
                            ON h.maNhanVien = nv.maNhanVien
                        OUTER APPLY
                        (
                            SELECT TOP 1
                                t.phuongThucThanhToan
                            FROM ThanhToan t
                            WHERE t.maHoaDon = h.maHoaDon
                            ORDER BY t.ngayThanhToan DESC, t.gioThanhToan DESC
                        ) tt
                        WHERE h.ngayLap >= @tuNgay
                          AND h.ngayLap < DATEADD(DAY, 1, @denNgay)";
                }

                if (!string.IsNullOrWhiteSpace(trangThai) &&
                    trangThai != "Tất cả")
                {
                    sql += " AND h.trangThai = @trangThai";
                }

                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    if (hasMaPhieu)
                    {
                        sql += @"
                            AND (
                                CAST(h.maHoaDon AS VARCHAR(20)) LIKE @keyword
                                OR CAST(b.soBan AS VARCHAR(20)) LIKE @keyword
                                OR nv.tenNhanVien LIKE @keyword
                                OR nv.hoVaTenLot LIKE @keyword
                            )";
                    }
                    else
                    {
                        sql += @"
                            AND (
                                CAST(h.maHoaDon AS VARCHAR(20)) LIKE @keyword
                                OR nv.tenNhanVien LIKE @keyword
                                OR nv.hoVaTenLot LIKE @keyword
                            )";
                    }
                }

                sql += " ORDER BY h.ngayLap DESC, h.gioLap DESC, h.maHoaDon DESC";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add("@tuNgay", SqlDbType.Date).Value = tuNgay.Date;
                    cmd.Parameters.Add("@denNgay", SqlDbType.Date).Value = denNgay.Date;

                    if (!string.IsNullOrWhiteSpace(trangThai) &&
                        trangThai != "Tất cả")
                    {
                        cmd.Parameters.Add("@trangThai", SqlDbType.NVarChar, 30)
                            .Value = trangThai;
                    }

                    if (!string.IsNullOrWhiteSpace(keyword))
                    {
                        cmd.Parameters.Add("@keyword", SqlDbType.NVarChar, 100)
                            .Value = "%" + keyword.Trim() + "%";
                    }

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            HoaDon hd = new HoaDon
                            {
                                MaHoaDon = Convert.ToInt32(reader["maHoaDon"]),
                                NgayLap = Convert.ToDateTime(reader["ngayLap"]),
                                GioLap = (TimeSpan)reader["gioLap"],
                                TongTien = Convert.ToDecimal(reader["tongTien"]),
                                GiamGia = Convert.ToDecimal(reader["giamGia"]),
                                TienThue = Convert.ToDecimal(reader["tienThue"]),
                                TrangThai = reader["trangThai"].ToString(),

                                MaNhanVien = reader["maNhanVien"] != DBNull.Value
                                    ? Convert.ToInt32(reader["maNhanVien"])
                                    : 0,

                                TenNhanVien = reader["tenNhanVien"].ToString(),

                                PhuongThucThanhToan =
                                    reader["phuongThucThanhToan"] != DBNull.Value
                                        ? reader["phuongThucThanhToan"].ToString()
                                        : "Tiền mặt"
                            };

                            if (hasMaPhieu)
                            {
                                hd.MaPhieuGoiMon =
                                    reader["maPhieuGoiMon"] != DBNull.Value
                                        ? (int?)Convert.ToInt32(reader["maPhieuGoiMon"])
                                        : null;

                                hd.MaBan =
                                    reader["maBan"] != DBNull.Value
                                        ? (int?)Convert.ToInt32(reader["maBan"])
                                        : null;

                                hd.SoBan =
                                    reader["soBan"] != DBNull.Value
                                        ? (int?)Convert.ToInt32(reader["soBan"])
                                        : null;
                            }

                            danhSach.Add(hd);
                        }
                    }
                }
            }

            return danhSach;
        }

        // 2. LẤY CHI TIẾT HÓA ĐƠN
        public List<ChiTietHoaDon> GetChiTietHoaDon(int maHoaDon)
        {
            if (maHoaDon <= 0)
                throw new ArgumentException("Mã hóa đơn không hợp lệ.");

            List<ChiTietHoaDon> danhSach = new List<ChiTietHoaDon>();

            const string sql = @"
                SELECT
                    c.maChiTietHoaDon,
                    c.maHoaDon,
                    c.maMonAn,
                    ISNULL(m.tenMonAn, N'Món ăn không còn kinh doanh') AS tenMonAn,
                    c.soLuong,
                    c.donGia
                FROM ChiTietHoaDon c
                LEFT JOIN MonAn m
                    ON c.maMonAn = m.maMonAn
                WHERE c.maHoaDon = @maHoaDon
                ORDER BY c.maChiTietHoaDon";

            using (SqlConnection conn = db.GetConnection())
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add("@maHoaDon", SqlDbType.Int).Value = maHoaDon;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            danhSach.Add(new ChiTietHoaDon
                            {
                                MaChiTietHoaDon =
                                    Convert.ToInt32(reader["maChiTietHoaDon"]),

                                MaHoaDon =
                                    Convert.ToInt32(reader["maHoaDon"]),

                                MaMonAn =
                                    Convert.ToInt32(reader["maMonAn"]),

                                TenMonAn =
                                    reader["tenMonAn"].ToString(),

                                SoLuong =
                                    Convert.ToInt32(reader["soLuong"]),

                                DonGia =
                                    Convert.ToDecimal(reader["donGia"])
                            });
                        }
                    }
                }
            }

            return danhSach;
        }


        // 3. LẬP HÓA ĐƠN TỪ PHIẾU GỌI MÓN
        //    Tự động lấy maKhachHang từ phiếu đặt bàn nếu có.
        public int TaoHoaDonTuPhieuGoiMon(int maPhieuGoiMon, int maNhanVien)
        {
            if (maPhieuGoiMon <= 0)
                throw new ArgumentException("Mã phiếu gọi món không hợp lệ.");

            if (maNhanVien <= 0)
                throw new ArgumentException("Mã nhân viên không hợp lệ.");

            using (SqlConnection conn = db.GetConnection())
            {
                conn.Open();

                using (SqlTransaction tran =
                    conn.BeginTransaction(IsolationLevel.Serializable))
                {
                    try
                    {
                        if (!HasColMaPhieu(conn, tran))
                        {
                            throw new Exception(
                                "Bảng HoaDon chưa có cột maPhieuGoiMon."
                            );
                        }

                        // 1. Kiểm tra hóa đơn đã tồn tại chưa.
                        const string sqlKiemTra = @"
                    SELECT maHoaDon
                    FROM HoaDon WITH (UPDLOCK, HOLDLOCK)
                    WHERE maPhieuGoiMon = @maPhieuGoiMon";

                        using (SqlCommand cmd = new SqlCommand(
                            sqlKiemTra, conn, tran))
                        {
                            cmd.Parameters.Add("@maPhieuGoiMon", SqlDbType.Int)
                                .Value = maPhieuGoiMon;

                            object result = cmd.ExecuteScalar();

                            if (result != null && result != DBNull.Value)
                            {
                                int maHoaDonCu = Convert.ToInt32(result);
                                tran.Commit();
                                return maHoaDonCu;
                            }
                        }

                        // 2. Lấy thông tin phiếu gọi món.
                        int maBan;
                        int? maDatBan;
                        string trangThaiPhieu;

                        const string sqlPhieu = @"
                    SELECT maBan, maDatBan, trangThai
                    FROM PhieuGoiMon WITH (UPDLOCK, HOLDLOCK)
                    WHERE maPhieuGoiMon = @maPhieuGoiMon";

                        using (SqlCommand cmd = new SqlCommand(
                            sqlPhieu, conn, tran))
                        {
                            cmd.Parameters.Add("@maPhieuGoiMon", SqlDbType.Int)
                                .Value = maPhieuGoiMon;

                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (!reader.Read())
                                    throw new Exception("Không tìm thấy phiếu gọi món.");

                                maBan = Convert.ToInt32(reader["maBan"]);

                                maDatBan = reader["maDatBan"] == DBNull.Value
                                    ? (int?)null
                                    : Convert.ToInt32(reader["maDatBan"]);

                                trangThaiPhieu = reader["trangThai"].ToString();
                            }
                        }

                        if (trangThaiPhieu != "Chờ xử lý" &&
                            trangThaiPhieu != "Đang phục vụ")
                        {
                            throw new Exception(
                                "Phiếu gọi món không ở trạng thái có thể lập hóa đơn."
                            );
                        }

                        // 3. Lấy mã khách hàng từ phiếu đặt bàn.
                        int? maKhachHang = null;

                        if (maDatBan.HasValue)
                        {
                            const string sqlKhachHang = @"
                        SELECT maKhachHang
                        FROM DatBan WITH (UPDLOCK, HOLDLOCK)
                        WHERE maDatBan = @maDatBan";

                            using (SqlCommand cmd = new SqlCommand(
                                sqlKhachHang, conn, tran))
                            {
                                cmd.Parameters.Add("@maDatBan", SqlDbType.Int)
                                    .Value = maDatBan.Value;

                                object result = cmd.ExecuteScalar();

                                if (result == null)
                                {
                                    throw new Exception(
                                        "Không tìm thấy thông tin đặt bàn liên kết với phiếu gọi món."
                                    );
                                }

                                if (result != DBNull.Value)
                                    maKhachHang = Convert.ToInt32(result);
                            }
                        }

                        // 4. Tính tổng tiền từ chi tiết phiếu gọi món.
                        decimal tongTien;

                        const string sqlTongTien = @"
                    SELECT ISNULL(SUM(soLuong * donGia), 0)
                    FROM ChiTietPhieuGoiMon
                    WHERE maPhieuGoiMon = @maPhieuGoiMon";

                        using (SqlCommand cmd = new SqlCommand(
                            sqlTongTien, conn, tran))
                        {
                            cmd.Parameters.Add("@maPhieuGoiMon", SqlDbType.Int)
                                .Value = maPhieuGoiMon;

                            tongTien = Convert.ToDecimal(cmd.ExecuteScalar());
                        }

                        if (tongTien <= 0)
                            throw new Exception("Phiếu gọi món chưa có món ăn.");

                        // 5. Tạo hóa đơn và lưu mã khách hàng.
                        int maHoaDonMoi;

                        const string sqlTaoHoaDon = @"
                    INSERT INTO HoaDon
                    (
                        ngayLap,
                        gioLap,
                        tongTien,
                        giamGia,
                        tienThue,
                        trangThai,
                        maNhanVien,
                        maKhachHang,
                        maPhieuGoiMon
                    )
                    VALUES
                    (
                        CAST(GETDATE() AS DATE),
                        CAST(GETDATE() AS TIME(0)),
                        @tongTien,
                        0,
                        0,
                        N'Chưa thanh toán',
                        @maNhanVien,
                        @maKhachHang,
                        @maPhieuGoiMon
                    );

                    SELECT CAST(SCOPE_IDENTITY() AS INT);";

                        using (SqlCommand cmd = new SqlCommand(
                            sqlTaoHoaDon, conn, tran))
                        {
                            cmd.Parameters.Add("@tongTien", SqlDbType.Decimal)
                                .Value = tongTien;
                            cmd.Parameters["@tongTien"].Precision = 18;
                            cmd.Parameters["@tongTien"].Scale = 2;

                            cmd.Parameters.Add("@maNhanVien", SqlDbType.Int)
                                .Value = maNhanVien;

                            cmd.Parameters.Add("@maKhachHang", SqlDbType.Int)
                                .Value = (object)maKhachHang ?? DBNull.Value;

                            cmd.Parameters.Add("@maPhieuGoiMon", SqlDbType.Int)
                                .Value = maPhieuGoiMon;

                            maHoaDonMoi = Convert.ToInt32(cmd.ExecuteScalar());
                        }

                        // 6. Sao chép chi tiết món sang ChiTietHoaDon.
                        const string sqlTaoChiTiet = @"
                    INSERT INTO ChiTietHoaDon
                    (
                        maHoaDon,
                        maMonAn,
                        soLuong,
                        donGia
                    )
                    SELECT
                        @maHoaDon,
                        maMonAn,
                        soLuong,
                        donGia
                    FROM ChiTietPhieuGoiMon
                    WHERE maPhieuGoiMon = @maPhieuGoiMon";

                        int soDongSaoChep;

                        using (SqlCommand cmd = new SqlCommand(
                            sqlTaoChiTiet, conn, tran))
                        {
                            cmd.Parameters.Add("@maHoaDon", SqlDbType.Int)
                                .Value = maHoaDonMoi;

                            cmd.Parameters.Add("@maPhieuGoiMon", SqlDbType.Int)
                                .Value = maPhieuGoiMon;

                            soDongSaoChep = cmd.ExecuteNonQuery();
                        }

                        if (soDongSaoChep <= 0)
                        {
                            throw new Exception(
                                "Không thể sao chép chi tiết món sang hóa đơn."
                            );
                        }

                        // 7. Cập nhật trạng thái phiếu gọi món.
                        const string sqlCapNhatPhieu = @"
                    UPDATE PhieuGoiMon
                    SET trangThai = N'Đã lập hóa đơn'
                    WHERE maPhieuGoiMon = @maPhieuGoiMon";

                        using (SqlCommand cmd = new SqlCommand(
                            sqlCapNhatPhieu, conn, tran))
                        {
                            cmd.Parameters.Add("@maPhieuGoiMon", SqlDbType.Int)
                                .Value = maPhieuGoiMon;

                            cmd.ExecuteNonQuery();
                        }

                        // 8. Chưa thanh toán nên giữ nguyên trạng thái bàn.
                        tran.Commit();

                        return maHoaDonMoi;
                    }
                    catch
                    {
                        try
                        {
                            tran.Rollback();
                        }
                        catch
                        {
                            // Giữ nguyên lỗi ban đầu.
                        }

                        throw;
                    }
                }
            }
        }


        // 4. THANH TOÁN MỘT HÓA ĐƠN ĐÃ LẬP
        public bool ThanhToanHoaDon(
            int maHoaDon,
            decimal giamGia,
            string phuongThuc)
        {
            if (maHoaDon <= 0)
                throw new ArgumentException("Mã hóa đơn không hợp lệ.");

            if (giamGia < 0)
                throw new ArgumentException("Giảm giá không được nhỏ hơn 0.");

            string phuongThucTT = string.IsNullOrWhiteSpace(phuongThuc)
                ? "Tiền mặt"
                : phuongThuc.Trim();

            bool laTienMat = string.Equals(
                phuongThucTT,
                "Tiền mặt",
                StringComparison.OrdinalIgnoreCase
            );

            bool laChuyenKhoan = string.Equals(
                phuongThucTT,
                "Chuyển khoản",
                StringComparison.OrdinalIgnoreCase
            );

            if (!laTienMat && !laChuyenKhoan)
                throw new ArgumentException("Phương thức thanh toán không hợp lệ.");

            using (SqlConnection conn = db.GetConnection())
            {
                conn.Open();

                using (SqlTransaction tran =
                    conn.BeginTransaction(IsolationLevel.Serializable))
                {
                    try
                    {
                        decimal tongTien;
                        string trangThaiHoaDonCu;

                        // 1. Lấy và khóa hóa đơn.
                        const string sqlLayHoaDon = @"
                    SELECT tongTien, trangThai
                    FROM HoaDon WITH (UPDLOCK, HOLDLOCK)
                    WHERE maHoaDon = @maHoaDon";

                        using (SqlCommand cmd = new SqlCommand(
                            sqlLayHoaDon, conn, tran))
                        {
                            cmd.Parameters.Add("@maHoaDon", SqlDbType.Int)
                                .Value = maHoaDon;

                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (!reader.Read())
                                    throw new Exception("Không tìm thấy hóa đơn.");

                                tongTien = Convert.ToDecimal(reader["tongTien"]);
                                trangThaiHoaDonCu =
                                    reader["trangThai"].ToString().Trim();
                            }
                        }

                        // 2. Chỉ cho phép thanh toán hóa đơn chưa thanh toán.
                        if (!string.Equals(
                            trangThaiHoaDonCu,
                            "Chưa thanh toán",
                            StringComparison.OrdinalIgnoreCase))
                        {
                            throw new Exception(
                                "Hóa đơn không ở trạng thái chưa thanh toán. " +
                                "Vui lòng kiểm tra lại trạng thái hóa đơn."
                            );
                        }

                        if (giamGia > tongTien)
                            throw new Exception(
                                "Giảm giá không được lớn hơn tổng tiền."
                            );

                        // 3. Tính tiền sau giảm giá và thuế.
                        decimal tienSauGiam = tongTien - giamGia;

                        decimal tienThue = Math.Round(
                            tienSauGiam * 0.10m,
                            0,
                            MidpointRounding.AwayFromZero
                        );

                        decimal soTienThanhToan = tienSauGiam + tienThue;

                        string trangThaiMoi = laTienMat
                            ? "Đã thanh toán"
                            : "Đang xử lý";

                        // 4. Cập nhật hóa đơn.
                        const string sqlCapNhatHoaDon = @"
                    UPDATE HoaDon
                    SET giamGia = @giamGia,
                        tienThue = @tienThue,
                        trangThai = @trangThai
                    WHERE maHoaDon = @maHoaDon
                      AND trangThai = N'Chưa thanh toán'";

                        using (SqlCommand cmd = new SqlCommand(
                            sqlCapNhatHoaDon, conn, tran))
                        {
                            cmd.Parameters.Add("@giamGia", SqlDbType.Decimal)
                                .Value = giamGia;
                            cmd.Parameters["@giamGia"].Precision = 18;
                            cmd.Parameters["@giamGia"].Scale = 2;

                            cmd.Parameters.Add("@tienThue", SqlDbType.Decimal)
                                .Value = tienThue;
                            cmd.Parameters["@tienThue"].Precision = 18;
                            cmd.Parameters["@tienThue"].Scale = 2;

                            cmd.Parameters.Add("@trangThai", SqlDbType.NVarChar, 30)
                                .Value = trangThaiMoi;

                            cmd.Parameters.Add("@maHoaDon", SqlDbType.Int)
                                .Value = maHoaDon;

                            if (cmd.ExecuteNonQuery() <= 0)
                                throw new Exception("Không thể cập nhật hóa đơn.");
                        }

                        // 5. Kiểm tra giao dịch thanh toán đã tồn tại chưa.
                        string trangThaiGiaoDichCu = null;

                        const string sqlKiemTraThanhToan = @"
    SELECT trangThai
    FROM ThanhToan WITH (UPDLOCK, HOLDLOCK)
    WHERE maHoaDon = @maHoaDon";

                        using (SqlCommand cmd = new SqlCommand(
                            sqlKiemTraThanhToan, conn, tran))
                        {
                            cmd.Parameters.Add("@maHoaDon", SqlDbType.Int)
                                .Value = maHoaDon;

                            object result = cmd.ExecuteScalar();

                            if (result != null && result != DBNull.Value)
                                trangThaiGiaoDichCu = result.ToString().Trim();
                        }

                        // Chuẩn bị câu lệnh cập nhật hoặc thêm mới.
                        if (trangThaiGiaoDichCu != null)
                        {
                            // Chỉ cho phép thanh toán lại giao dịch thất bại.
                            if (trangThaiGiaoDichCu != "Không thành công")
                            {
                                throw new Exception(
                                    "Hóa đơn đã có giao dịch thanh toán với trạng thái: " +
                                    trangThaiGiaoDichCu +
                                    ". Không thể tạo giao dịch mới."
                                );
                            }

                            // Cập nhật lại giao dịch cũ.
                            const string sqlCapNhatThanhToan = @"
        UPDATE ThanhToan
        SET
            soTienThanhToan = @soTien,
            phuongThucThanhToan = @phuongThuc,
            ngayThanhToan = CAST(GETDATE() AS DATE),
            gioThanhToan = CAST(GETDATE() AS TIME(0)),
            trangThai = @trangThai
        WHERE maHoaDon = @maHoaDon
          AND trangThai = N'Không thành công'";

                            using (SqlCommand cmd = new SqlCommand(
                                sqlCapNhatThanhToan, conn, tran))
                            {
                                cmd.Parameters.Add("@maHoaDon", SqlDbType.Int)
                                    .Value = maHoaDon;

                                cmd.Parameters.Add("@soTien", SqlDbType.Decimal)
                                    .Value = soTienThanhToan;
                                cmd.Parameters["@soTien"].Precision = 18;
                                cmd.Parameters["@soTien"].Scale = 2;

                                cmd.Parameters.Add("@phuongThuc", SqlDbType.NVarChar, 50)
                                    .Value = phuongThucTT;

                                cmd.Parameters.Add("@trangThai", SqlDbType.NVarChar, 30)
                                    .Value = trangThaiMoi;

                                if (cmd.ExecuteNonQuery() != 1)
                                {
                                    throw new Exception(
                                        "Không thể cập nhật giao dịch thanh toán cũ."
                                    );
                                }
                            }
                        }
                        else
                        {
                            // Chưa có giao dịch thì mới thêm bản ghi.
                            const string sqlThemThanhToan = @"
        INSERT INTO ThanhToan
        (
            maHoaDon,
            soTienThanhToan,
            phuongThucThanhToan,
            ngayThanhToan,
            gioThanhToan,
            trangThai
        )
        VALUES
        (
            @maHoaDon,
            @soTien,
            @phuongThuc,
            CAST(GETDATE() AS DATE),
            CAST(GETDATE() AS TIME(0)),
            @trangThai
        )";

                            using (SqlCommand cmd = new SqlCommand(
                                sqlThemThanhToan, conn, tran))
                            {
                                cmd.Parameters.Add("@maHoaDon", SqlDbType.Int)
                                    .Value = maHoaDon;

                                cmd.Parameters.Add("@soTien", SqlDbType.Decimal)
                                    .Value = soTienThanhToan;
                                cmd.Parameters["@soTien"].Precision = 18;
                                cmd.Parameters["@soTien"].Scale = 2;

                                cmd.Parameters.Add("@phuongThuc", SqlDbType.NVarChar, 50)
                                    .Value = phuongThucTT;

                                cmd.Parameters.Add("@trangThai", SqlDbType.NVarChar, 30)
                                    .Value = trangThaiMoi;

                                cmd.ExecuteNonQuery();
                            }
                        }
                        // 6. Hoàn tất phiếu gọi món và giải phóng bàn khi thanh toán tiền mặt.
                        if (laTienMat)
                        {
                            bool hasMaPhieu = HasColMaPhieu(conn, tran);

                            if (!hasMaPhieu)
                            {
                                throw new Exception(
                                    "Bảng HoaDon chưa có cột maPhieuGoiMon."
                                );
                            }

                            // Cập nhật phiếu gọi món đã thanh toán.
                            const string sqlCapNhatPhieu = @"
        UPDATE p
        SET p.trangThai = N'Đã thanh toán'
        FROM PhieuGoiMon p
        INNER JOIN HoaDon h
            ON h.maPhieuGoiMon = p.maPhieuGoiMon
        WHERE h.maHoaDon = @maHoaDon";

                            using (SqlCommand cmd = new SqlCommand(
                                sqlCapNhatPhieu, conn, tran))
                            {
                                cmd.Parameters.Add("@maHoaDon", SqlDbType.Int)
                                    .Value = maHoaDon;

                                if (cmd.ExecuteNonQuery() <= 0)
                                {
                                    throw new Exception(
                                        "Không tìm thấy phiếu gọi món của hóa đơn."
                                    );
                                }
                            }

                            // Chỉ giải phóng bàn khi không còn phiếu hoạt động
                            // và không còn hóa đơn chưa thanh toán hoặc đang xử lý.
                            const string sqlCapNhatBan = @"
        UPDATE b
        SET b.trangThai = N'Trống'
        FROM Ban b
        INNER JOIN PhieuGoiMon p
            ON p.maBan = b.maBan
        INNER JOIN HoaDon h
            ON h.maPhieuGoiMon = p.maPhieuGoiMon
        WHERE h.maHoaDon = @maHoaDon
          AND NOT EXISTS
          (
              SELECT 1
              FROM PhieuGoiMon p2
              LEFT JOIN HoaDon h2
                  ON h2.maPhieuGoiMon = p2.maPhieuGoiMon
              WHERE p2.maBan = b.maBan
                AND
                (
                    p2.trangThai IN
                    (
                        N'Chờ xử lý',
                        N'Đang phục vụ',
                        N'Đã lập hóa đơn'
                    )
                    OR h2.trangThai IN
                    (
                        N'Chưa thanh toán',
                        N'Đang xử lý'
                    )
                )
          )";

                            using (SqlCommand cmd = new SqlCommand(
                                sqlCapNhatBan, conn, tran))
                            {
                                cmd.Parameters.Add("@maHoaDon", SqlDbType.Int)
                                    .Value = maHoaDon;

                                cmd.ExecuteNonQuery();
                            }
                        }


                        tran.Commit();
                        return true;
                    }
                    catch
                    {
                        try
                        {
                            tran.Rollback();
                        }
                        catch
                        {
                            // Giữ nguyên lỗi ban đầu.
                        }

                        throw;
                    }
                }
            }
        }


        // 5. THANH TOÁN TRỰC TIẾP THEO BÀN
        //    Giữ lại chữ ký cũ để hạn chế ảnh hưởng Form/Service.
        public bool ThanhToan(
            int maBan,
            int maNhanVien,
            decimal giamGia,
            string phuongThuc,
            out int maHoaDonMoi)
        {
            maHoaDonMoi = 0;

            if (maBan <= 0)
                throw new ArgumentException("Mã bàn không hợp lệ.");

            if (maNhanVien <= 0)
                throw new ArgumentException("Mã nhân viên không hợp lệ.");

            if (giamGia < 0)
                throw new ArgumentException("Giảm giá không được nhỏ hơn 0.");

            int maPhieu;

            // Tìm phiếu mới nhất của bàn chưa thanh toán.
            const string sqlTimPhieu = @"
                SELECT TOP 1 maPhieuGoiMon
                FROM PhieuGoiMon
                WHERE maBan = @maBan
                  AND trangThai IN
                  (
                      N'Chờ xử lý',
                      N'Đang phục vụ',
                      N'Đã lập hóa đơn'
                  )
                ORDER BY maPhieuGoiMon DESC";

            using (SqlConnection conn = db.GetConnection())
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(sqlTimPhieu, conn))
                {
                    cmd.Parameters.Add("@maBan", SqlDbType.Int).Value = maBan;

                    object result = cmd.ExecuteScalar();

                    if (result == null || result == DBNull.Value)
                        return false;

                    maPhieu = Convert.ToInt32(result);
                }
            }

            // Nếu chưa có hóa đơn thì lập hóa đơn trước.
            // Nếu đã có thì phương thức sẽ trả về mã hóa đơn cũ.
            maHoaDonMoi = TaoHoaDonTuPhieuGoiMon(maPhieu, maNhanVien);

            // Sau đó thực hiện thanh toán.
            return ThanhToanHoaDon(maHoaDonMoi, giamGia, phuongThuc);
        }
    }
}
