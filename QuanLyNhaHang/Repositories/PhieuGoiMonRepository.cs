
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace QuanLyNhaHang.Repositories
{
    public class PhieuGoiMonRepository
    {
        private readonly string connectionString =
            ConfigurationManager.ConnectionStrings[
                "QuanLyNhaHangConnection"].ConnectionString;
        // 1. LƯU GỌI MÓN
        // - Chưa có phiếu: tạo phiếu mới.
        // - Đã có phiếu: thêm món vào phiếu hiện tại.
        // - Nếu gọi món từ đặt bàn: lưu đúng maDatBan.
        // - Đồng bộ trạng thái bàn và phiếu gọi món.
        public int LuuGoiMon(
            int maBan,
            int? maNhanVien,
            Dictionary<int, int> gioHang,
            int? maDatBan = null)
        {
            if (maBan <= 0)
                throw new Exception("Vui lòng chọn bàn hợp lệ.");

            if (gioHang == null || gioHang.Count == 0)
                throw new Exception("Giỏ hàng chưa có món ăn.");

            foreach (var item in gioHang)
            {
                if (item.Key <= 0 || item.Value <= 0)
                {
                    throw new Exception(
                        "Mã món ăn hoặc số lượng không hợp lệ.");
                }
            }

            using (SqlConnection conn =
                new SqlConnection(connectionString))
            {
                conn.Open();

                using (SqlTransaction transaction =
                    conn.BeginTransaction(IsolationLevel.Serializable))
                {
                    try
                    {
                        // 1. Kiểm tra bàn tồn tại
                        string sqlKiemTraBan = @"
                            SELECT trangThai
                            FROM dbo.Ban WITH (UPDLOCK, HOLDLOCK)
                            WHERE maBan = @maBan";

                        using (SqlCommand cmd = new SqlCommand(
                            sqlKiemTraBan, conn, transaction))
                        {
                            cmd.Parameters.Add(
                                "@maBan", SqlDbType.Int).Value = maBan;

                            object ketQua = cmd.ExecuteScalar();

                            if (ketQua == null ||
                                ketQua == DBNull.Value)
                            {
                                throw new Exception(
                                    "Không tìm thấy bàn.");
                            }
                        }

                        // 2. Nếu gọi món từ đặt bàn:
                        // Kiểm tra đặt bàn và bàn có thuộc đặt bàn đó không
                        if (maDatBan.HasValue)
                        {
                            string sqlKiemTraDatBan = @"
                                SELECT COUNT(*)
                                FROM dbo.DatBan db WITH (UPDLOCK, HOLDLOCK)
                                INNER JOIN dbo.ChiTietDatBan ct
                                    ON db.maDatBan = ct.maDatBan
                                WHERE db.maDatBan = @maDatBan
                                  AND ct.maBan = @maBan
                                  AND db.trangThai = N'Đã đặt'";

                            using (SqlCommand cmd = new SqlCommand(
                                sqlKiemTraDatBan, conn, transaction))
                            {
                                cmd.Parameters.Add(
                                    "@maDatBan",
                                    SqlDbType.Int).Value = maDatBan.Value;

                                cmd.Parameters.Add(
                                    "@maBan",
                                    SqlDbType.Int).Value = maBan;

                                int hopLe = Convert.ToInt32(
                                    cmd.ExecuteScalar());

                                if (hopLe == 0)
                                {
                                    throw new Exception(
                                        "Bàn này không thuộc đặt bàn đã chọn " +
                                        "hoặc đặt bàn không còn ở trạng thái Đã đặt.");
                                }
                            }
                        }

                        // 3. Tìm phiếu gọi món đang hoạt động
                        int maPhieuGoiMon = 0;
                        int? maDatBanCuaPhieu = null;

                        string sqlTimPhieu = @"
                            SELECT TOP 1
                                maPhieuGoiMon,
                                maDatBan
                            FROM dbo.PhieuGoiMon WITH (UPDLOCK, HOLDLOCK)
                            WHERE maBan = @maBan
                              AND trangThai IN
                              (
                                  N'Chờ xử lý',
                                  N'Đang phục vụ'
                              )
                            ORDER BY maPhieuGoiMon DESC";

                        using (SqlCommand cmd = new SqlCommand(
                            sqlTimPhieu, conn, transaction))
                        {
                            cmd.Parameters.Add(
                                "@maBan", SqlDbType.Int).Value = maBan;

                            using (SqlDataReader reader =
                                cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    maPhieuGoiMon =
                                        Convert.ToInt32(
                                            reader["maPhieuGoiMon"]);

                                    if (reader["maDatBan"] != DBNull.Value)
                                    {
                                        maDatBanCuaPhieu =
                                            Convert.ToInt32(
                                                reader["maDatBan"]);
                                    }
                                }
                            }
                        }

                        // 4. Nếu đã có phiếu:
                        // Kiểm tra phiếu có đúng lượt đặt bàn không
                        if (maPhieuGoiMon > 0)
                        {
                            if (maDatBanCuaPhieu != maDatBan)
                            {
                                throw new Exception(
                                    "Bàn này đã có phiếu gọi món đang mở " +
                                    "thuộc một lượt phục vụ khác. " +
                                    "Không thể thêm món vào phiếu này.");
                            }
                        }

                        // 5. Tạo phiếu mới nếu chưa có
                        if (maPhieuGoiMon == 0)
                        {
                            string sqlTaoPhieu = @"
                                INSERT INTO dbo.PhieuGoiMon
                                (
                                    maBan,
                                    maNhanVien,
                                    maDatBan,
                                    ngayGoi,
                                    gioGoi,
                                    trangThai
                                )
                                VALUES
                                (
                                    @maBan,
                                    @maNhanVien,
                                    @maDatBan,
                                    CONVERT(DATE, GETDATE()),
                                    CONVERT(TIME(0), GETDATE()),
                                    @trangThai
                                );

                                SELECT CAST(SCOPE_IDENTITY() AS INT);";

                            using (SqlCommand cmd = new SqlCommand(
                                sqlTaoPhieu, conn, transaction))
                            {
                                cmd.Parameters.Add(
                                    "@maBan",
                                    SqlDbType.Int).Value = maBan;

                                cmd.Parameters.Add(
                                    "@maNhanVien",
                                    SqlDbType.Int).Value =
                                    maNhanVien.HasValue
                                        ? (object)maNhanVien.Value
                                        : DBNull.Value;

                                cmd.Parameters.Add(
                                    "@maDatBan",
                                    SqlDbType.Int).Value =
                                    maDatBan.HasValue
                                        ? (object)maDatBan.Value
                                        : DBNull.Value;

                                cmd.Parameters.Add(
                                    "@trangThai",
                                    SqlDbType.NVarChar,
                                    50).Value = "Chờ xử lý";

                                maPhieuGoiMon = Convert.ToInt32(
                                    cmd.ExecuteScalar());
                            }
                        }

                        // 6. Lưu từng món vào chi tiết phiếu
                        foreach (var item in gioHang)
                        {
                            int maMonAn = item.Key;
                            int soLuongThem = item.Value;
                            decimal donGia;

                            // 6.1. Kiểm tra món và lấy giá hiện tại
                            string sqlLayMon = @"
                                SELECT donGia, trangThai
                                FROM dbo.MonAn WITH (UPDLOCK, HOLDLOCK)
                                WHERE maMonAn = @maMonAn";

                            using (SqlCommand cmd = new SqlCommand(
                                sqlLayMon, conn, transaction))
                            {
                                cmd.Parameters.Add(
                                    "@maMonAn",
                                    SqlDbType.Int).Value = maMonAn;

                                using (SqlDataReader reader =
                                    cmd.ExecuteReader())
                                {
                                    if (!reader.Read())
                                    {
                                        throw new Exception(
                                            "Không tìm thấy món ăn mã: " +
                                            maMonAn);
                                    }

                                    donGia = Convert.ToDecimal(
                                        reader["donGia"]);

                                    bool dangKinhDoanh =
                                        Convert.ToBoolean(
                                            reader["trangThai"]);

                                    if (!dangKinhDoanh)
                                    {
                                        throw new Exception(
                                            "Món ăn mã " + maMonAn +
                                            " hiện không còn kinh doanh.");
                                    }
                                }
                            }

                            // 6.2. Kiểm tra món đã tồn tại trong phiếu chưa
                            int maChiTiet = 0;

                            string sqlTimChiTiet = @"
                                SELECT TOP 1 maChiTietPhieuGoiMon
                                FROM dbo.ChiTietPhieuGoiMon
                                    WITH (UPDLOCK, HOLDLOCK)
                                WHERE maPhieuGoiMon = @maPhieuGoiMon
                                  AND maMonAn = @maMonAn";

                            using (SqlCommand cmd = new SqlCommand(
                                sqlTimChiTiet, conn, transaction))
                            {
                                cmd.Parameters.Add(
                                    "@maPhieuGoiMon",
                                    SqlDbType.Int).Value = maPhieuGoiMon;

                                cmd.Parameters.Add(
                                    "@maMonAn",
                                    SqlDbType.Int).Value = maMonAn;

                                object ketQua = cmd.ExecuteScalar();

                                if (ketQua != null &&
                                    ketQua != DBNull.Value)
                                {
                                    maChiTiet =
                                        Convert.ToInt32(ketQua);
                                }
                            }

                            // 6.3. Nếu món đã có thì cộng số lượng
                            if (maChiTiet > 0)
                            {
                                string sqlCapNhat = @"
                                    UPDATE dbo.ChiTietPhieuGoiMon
                                    SET soLuong = soLuong + @soLuong
                                    WHERE maChiTietPhieuGoiMon = @maChiTiet";

                                using (SqlCommand cmd = new SqlCommand(
                                    sqlCapNhat, conn, transaction))
                                {
                                    cmd.Parameters.Add(
                                        "@soLuong",
                                        SqlDbType.Int).Value = soLuongThem;

                                    cmd.Parameters.Add(
                                        "@maChiTiet",
                                        SqlDbType.Int).Value = maChiTiet;

                                    cmd.ExecuteNonQuery();
                                }
                            }
                            else
                            {
                                // 6.4. Nếu chưa có thì thêm chi tiết mới
                                string sqlThemChiTiet = @"
                                    INSERT INTO dbo.ChiTietPhieuGoiMon
                                    (
                                        maPhieuGoiMon,
                                        maMonAn,
                                        soLuong,
                                        donGia
                                    )
                                    VALUES
                                    (
                                        @maPhieuGoiMon,
                                        @maMonAn,
                                        @soLuong,
                                        @donGia
                                    )";

                                using (SqlCommand cmd = new SqlCommand(
                                    sqlThemChiTiet, conn, transaction))
                                {
                                    cmd.Parameters.Add(
                                        "@maPhieuGoiMon",
                                        SqlDbType.Int).Value = maPhieuGoiMon;

                                    cmd.Parameters.Add(
                                        "@maMonAn",
                                        SqlDbType.Int).Value = maMonAn;

                                    cmd.Parameters.Add(
                                        "@soLuong",
                                        SqlDbType.Int).Value = soLuongThem;

                                    SqlParameter p =
                                        cmd.Parameters.Add(
                                            "@donGia",
                                            SqlDbType.Decimal);

                                    p.Precision = 18;
                                    p.Scale = 2;
                                    p.Value = donGia;

                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }

                        // 7. Cập nhật trạng thái phiếu gọi món
                        string sqlCapNhatPhieu = @"
                            UPDATE dbo.PhieuGoiMon
                            SET trangThai = N'Đang phục vụ'
                            WHERE maPhieuGoiMon = @maPhieuGoiMon";

                        using (SqlCommand cmd = new SqlCommand(
                            sqlCapNhatPhieu, conn, transaction))
                        {
                            cmd.Parameters.Add(
                                "@maPhieuGoiMon",
                                SqlDbType.Int).Value = maPhieuGoiMon;

                            cmd.ExecuteNonQuery();
                        }

                        // 8. Cập nhật trạng thái bàn
                        string sqlCapNhatBan = @"
                            UPDATE dbo.Ban
                            SET trangThai = N'Đang phục vụ'
                            WHERE maBan = @maBan";

                        using (SqlCommand cmd = new SqlCommand(
                            sqlCapNhatBan, conn, transaction))
                        {
                            cmd.Parameters.Add(
                                "@maBan",
                                SqlDbType.Int).Value = maBan;

                            cmd.ExecuteNonQuery();
                        }

                        // 9. Hoàn tất giao dịch
                        transaction.Commit();

                        return maPhieuGoiMon;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
        // 2. LẤY MÃ PHIẾU GỌI MÓN ĐANG MỞ THEO BÀN
        public int GetMaPhieuDangMo(
            int maBan,
            int? maDatBan = null)
        {
            if (maBan <= 0)
                return 0;

            using (SqlConnection conn =
                new SqlConnection(connectionString))
            {
                conn.Open();

                string sql = @"
                    SELECT TOP 1 maPhieuGoiMon
                    FROM dbo.PhieuGoiMon
                    WHERE maBan = @maBan
                      AND trangThai IN
                      (
                          N'Chờ xử lý',
                          N'Đang phục vụ'
                      )
                      AND
                      (
                          (@maDatBan IS NULL AND maDatBan IS NULL)
                          OR maDatBan = @maDatBan
                      )
                    ORDER BY maPhieuGoiMon DESC";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add(
                        "@maBan",
                        SqlDbType.Int).Value = maBan;

                    cmd.Parameters.Add(
                        "@maDatBan",
                        SqlDbType.Int).Value =
                        maDatBan.HasValue
                            ? (object)maDatBan.Value
                            : DBNull.Value;

                    object result = cmd.ExecuteScalar();

                    if (result == null || result == DBNull.Value)
                        return 0;

                    return Convert.ToInt32(result);
                }
            }
        }
        // 3. LẤY DANH SÁCH MÓN ĐÃ GỌI TRONG PHIẾU
        // Key = mã món ăn
        // Value = số lượng
        public Dictionary<int, int> GetChiTietPhieuGoiMon(
            int maPhieuGoiMon)
        {
            Dictionary<int, int> danhSachMon =
                new Dictionary<int, int>();

            if (maPhieuGoiMon <= 0)
                return danhSachMon;

            using (SqlConnection conn =
                new SqlConnection(connectionString))
            {
                conn.Open();

                string sql = @"
                    SELECT maMonAn, SUM(soLuong) AS soLuong
                    FROM dbo.ChiTietPhieuGoiMon
                    WHERE maPhieuGoiMon = @maPhieuGoiMon
                    GROUP BY maMonAn";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add(
                        "@maPhieuGoiMon",
                        SqlDbType.Int).Value = maPhieuGoiMon;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int maMonAn =
                                Convert.ToInt32(reader["maMonAn"]);

                            int soLuong =
                                Convert.ToInt32(reader["soLuong"]);

                            danhSachMon[maMonAn] = soLuong;
                        }
                    }
                }
            }

            return danhSachMon;
        }

        // 4. XÓA MÓN KHỎI PHIẾU GỌI MÓN
        // - Không cho xóa nếu phiếu đã lập hóa đơn.
        // - Nếu còn món: bàn tiếp tục Đang phục vụ.
        // - Nếu hết món: xóa phiếu rỗng và khôi phục trạng thái bàn.
        public bool XoaMonKhoiPhieu(
            int maBan,
            int maMonAn,
            int? maDatBan = null)
        {
            if (maBan <= 0 || maMonAn <= 0)
            {
                throw new Exception(
                    "Thông tin bàn hoặc món ăn không hợp lệ.");
            }

            using (SqlConnection conn =
                new SqlConnection(connectionString))
            {
                conn.Open();

                using (SqlTransaction transaction =
                    conn.BeginTransaction(IsolationLevel.Serializable))
                {
                    try
                    {
                        int maPhieuGoiMon = 0;

                        // 1. Tìm phiếu đang mở theo bàn và mã đặt bàn
                        string sqlTimPhieu = @"
                            SELECT TOP 1 maPhieuGoiMon
                            FROM dbo.PhieuGoiMon WITH (UPDLOCK, HOLDLOCK)
                            WHERE maBan = @maBan
                              AND trangThai IN
                              (
                                  N'Chờ xử lý',
                                  N'Đang phục vụ'
                              )
                              AND
                              (
                                  (@maDatBan IS NULL AND maDatBan IS NULL)
                                  OR maDatBan = @maDatBan
                              )
                            ORDER BY maPhieuGoiMon DESC";

                        using (SqlCommand cmd = new SqlCommand(
                            sqlTimPhieu, conn, transaction))
                        {
                            cmd.Parameters.Add(
                                "@maBan",
                                SqlDbType.Int).Value = maBan;

                            cmd.Parameters.Add(
                                "@maDatBan",
                                SqlDbType.Int).Value =
                                maDatBan.HasValue
                                    ? (object)maDatBan.Value
                                    : DBNull.Value;

                            object result = cmd.ExecuteScalar();

                            if (result == null ||
                                result == DBNull.Value)
                            {
                                throw new Exception(
                                    "Không tìm thấy phiếu gọi món đang mở.");
                            }

                            maPhieuGoiMon = Convert.ToInt32(result);
                        }

                        // 2. Kiểm tra phiếu đã lập hóa đơn chưa
                        string sqlKiemTraHoaDon = @"
                            SELECT COUNT(*)
                            FROM dbo.HoaDon
                            WHERE maPhieuGoiMon = @maPhieuGoiMon";

                        using (SqlCommand cmd = new SqlCommand(
                            sqlKiemTraHoaDon, conn, transaction))
                        {
                            cmd.Parameters.Add(
                                "@maPhieuGoiMon",
                                SqlDbType.Int).Value = maPhieuGoiMon;

                            if (Convert.ToInt32(cmd.ExecuteScalar()) > 0)
                            {
                                throw new Exception(
                                    "Phiếu đã lập hóa đơn, không thể xóa món.");
                            }
                        }

                        // 3. Xóa món khỏi phiếu
                        string sqlXoa = @"
                            DELETE FROM dbo.ChiTietPhieuGoiMon
                            WHERE maPhieuGoiMon = @maPhieuGoiMon
                              AND maMonAn = @maMonAn";

                        using (SqlCommand cmd = new SqlCommand(
                            sqlXoa, conn, transaction))
                        {
                            cmd.Parameters.Add(
                                "@maPhieuGoiMon",
                                SqlDbType.Int).Value = maPhieuGoiMon;

                            cmd.Parameters.Add(
                                "@maMonAn",
                                SqlDbType.Int).Value = maMonAn;

                            int soDongXoa = cmd.ExecuteNonQuery();

                            if (soDongXoa == 0)
                            {
                                throw new Exception(
                                    "Không tìm thấy món trong phiếu.");
                            }
                        }

                        // 4. Kiểm tra phiếu còn món hay không
                        string sqlDemMon = @"
                            SELECT COUNT(*)
                            FROM dbo.ChiTietPhieuGoiMon
                            WHERE maPhieuGoiMon = @maPhieuGoiMon";

                        int soMonConLai;

                        using (SqlCommand cmd = new SqlCommand(
                            sqlDemMon, conn, transaction))
                        {
                            cmd.Parameters.Add(
                                "@maPhieuGoiMon",
                                SqlDbType.Int).Value = maPhieuGoiMon;

                            soMonConLai = Convert.ToInt32(
                                cmd.ExecuteScalar());
                        }

                        // 5. Nếu phiếu vẫn còn món
                        if (soMonConLai > 0)
                        {
                            string sqlCapNhatBan = @"
                                UPDATE dbo.Ban
                                SET trangThai = N'Đang phục vụ'
                                WHERE maBan = @maBan";

                            using (SqlCommand cmd = new SqlCommand(
                                sqlCapNhatBan, conn, transaction))
                            {
                                cmd.Parameters.Add(
                                    "@maBan",
                                    SqlDbType.Int).Value = maBan;

                                cmd.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            // 6. Phiếu không còn món:
                            // Xóa phiếu rỗng chưa lập hóa đơn
                            string sqlXoaPhieuRong = @"
                                DELETE FROM dbo.PhieuGoiMon
                                WHERE maPhieuGoiMon = @maPhieuGoiMon
                                  AND NOT EXISTS
                                  (
                                      SELECT 1
                                      FROM dbo.ChiTietPhieuGoiMon ct
                                      WHERE ct.maPhieuGoiMon =
                                            dbo.PhieuGoiMon.maPhieuGoiMon
                                  )";

                            using (SqlCommand cmd = new SqlCommand(
                                sqlXoaPhieuRong, conn, transaction))
                            {
                                cmd.Parameters.Add(
                                    "@maPhieuGoiMon",
                                    SqlDbType.Int).Value = maPhieuGoiMon;

                                int soDongXoa = cmd.ExecuteNonQuery();

                                if (soDongXoa == 0)
                                {
                                    throw new Exception(
                                        "Không thể xóa phiếu gọi món rỗng.");
                                }
                            }

                            // 7. Kiểm tra đặt bàn còn hiệu lực cho bàn này
                            bool coDatBan = false;

                            string sqlKiemTraDatBan;

                            if (maDatBan.HasValue)
                            {
                                sqlKiemTraDatBan = @"
                                    SELECT COUNT(*)
                                    FROM dbo.DatBan db
                                    INNER JOIN dbo.ChiTietDatBan ct
                                        ON db.maDatBan = ct.maDatBan
                                    WHERE db.maDatBan = @maDatBan
                                      AND ct.maBan = @maBan
                                      AND db.trangThai = N'Đã đặt'";
                            }
                            else
                            {
                                sqlKiemTraDatBan = @"
                                    SELECT COUNT(*)
                                    FROM dbo.DatBan db
                                    INNER JOIN dbo.ChiTietDatBan ct
                                        ON db.maDatBan = ct.maDatBan
                                    WHERE ct.maBan = @maBan
                                      AND db.trangThai = N'Đã đặt'";
                            }

                            using (SqlCommand cmd = new SqlCommand(
                                sqlKiemTraDatBan, conn, transaction))
                            {
                                cmd.Parameters.Add(
                                    "@maBan",
                                    SqlDbType.Int).Value = maBan;

                                if (maDatBan.HasValue)
                                {
                                    cmd.Parameters.Add(
                                        "@maDatBan",
                                        SqlDbType.Int).Value =
                                        maDatBan.Value;
                                }

                                coDatBan = Convert.ToInt32(
                                    cmd.ExecuteScalar()) > 0;
                            }

                            // 8. Khôi phục trạng thái bàn
                            string trangThaiBanMoi =
                                coDatBan ? "Đã đặt" : "Trống";

                            string sqlCapNhatBan = @"
                                UPDATE dbo.Ban
                                SET trangThai = @trangThai
                                WHERE maBan = @maBan";

                            using (SqlCommand cmd = new SqlCommand(
                                sqlCapNhatBan, conn, transaction))
                            {
                                cmd.Parameters.Add(
                                    "@trangThai",
                                    SqlDbType.NVarChar,
                                    50).Value = trangThaiBanMoi;

                                cmd.Parameters.Add(
                                    "@maBan",
                                    SqlDbType.Int).Value = maBan;

                                cmd.ExecuteNonQuery();
                            }
                        }

                        // 9. Hoàn tất giao dịch
                        transaction.Commit();

                        return true;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        // 5. CẬP NHẬT SỐ LƯỢNG MÓN TRONG PHIẾU
        public void CapNhatSoLuongMonTrongPhieu(
            int maBan,
            int maMonAn,
            int soLuongMoi,
            int? maDatBan = null)
        {
            if (maBan <= 0 ||
                maMonAn <= 0 ||
                soLuongMoi <= 0)
            {
                throw new Exception(
                    "Thông tin cập nhật không hợp lệ.");
            }

            using (SqlConnection conn =
                new SqlConnection(connectionString))
            {
                conn.Open();

                using (SqlTransaction tran =
                    conn.BeginTransaction(IsolationLevel.Serializable))
                {
                    try
                    {
                        int maPhieu = 0;

                        // 1. Tìm đúng phiếu theo bàn và mã đặt bàn
                        string sqlTimPhieu = @"
                            SELECT TOP 1 maPhieuGoiMon
                            FROM dbo.PhieuGoiMon WITH (UPDLOCK, HOLDLOCK)
                            WHERE maBan = @maBan
                              AND trangThai IN
                              (
                                  N'Chờ xử lý',
                                  N'Đang phục vụ'
                              )
                              AND
                              (
                                  (@maDatBan IS NULL AND maDatBan IS NULL)
                                  OR maDatBan = @maDatBan
                              )
                            ORDER BY maPhieuGoiMon DESC";

                        using (SqlCommand cmd = new SqlCommand(
                            sqlTimPhieu, conn, tran))
                        {
                            cmd.Parameters.Add(
                                "@maBan",
                                SqlDbType.Int).Value = maBan;

                            cmd.Parameters.Add(
                                "@maDatBan",
                                SqlDbType.Int).Value =
                                maDatBan.HasValue
                                    ? (object)maDatBan.Value
                                    : DBNull.Value;

                            object ketQua = cmd.ExecuteScalar();

                            if (ketQua == null ||
                                ketQua == DBNull.Value)
                            {
                                throw new Exception(
                                    "Bàn chưa có phiếu gọi món đang mở.");
                            }

                            maPhieu = Convert.ToInt32(ketQua);
                        }

                        // 2. Kiểm tra phiếu đã lập hóa đơn chưa
                        string sqlKiemTraHoaDon = @"
                            SELECT COUNT(*)
                            FROM dbo.HoaDon
                            WHERE maPhieuGoiMon = @maPhieu";

                        using (SqlCommand cmd = new SqlCommand(
                            sqlKiemTraHoaDon, conn, tran))
                        {
                            cmd.Parameters.Add(
                                "@maPhieu",
                                SqlDbType.Int).Value = maPhieu;

                            if (Convert.ToInt32(cmd.ExecuteScalar()) > 0)
                            {
                                throw new Exception(
                                    "Phiếu gọi món đã lập hóa đơn, " +
                                    "không thể cập nhật.");
                            }
                        }

                        // 3. Cập nhật số lượng
                        string sqlCapNhat = @"
                            UPDATE dbo.ChiTietPhieuGoiMon
                            SET soLuong = @soLuong
                            WHERE maPhieuGoiMon = @maPhieu
                              AND maMonAn = @maMonAn";

                        using (SqlCommand cmd = new SqlCommand(
                            sqlCapNhat, conn, tran))
                        {
                            cmd.Parameters.Add(
                                "@soLuong",
                                SqlDbType.Int).Value = soLuongMoi;

                            cmd.Parameters.Add(
                                "@maPhieu",
                                SqlDbType.Int).Value = maPhieu;

                            cmd.Parameters.Add(
                                "@maMonAn",
                                SqlDbType.Int).Value = maMonAn;

                            int soDong = cmd.ExecuteNonQuery();

                            if (soDong == 0)
                            {
                                throw new Exception(
                                    "Không tìm thấy món trong phiếu gọi món.");
                            }
                        }

                        // 4. Hoàn tất giao dịch
                        tran.Commit();
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
