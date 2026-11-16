
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;

using QuanLyNhaHang.Data;
using QuanLyNhaHang.Models;
using QuanLyNhaHang.Repositories;

namespace QuanLyNhaHang.Services
{
    public class DatBanService
    {
        private readonly DbConnection dbConnection = new DbConnection();

        private readonly DatBanRepository datBanRepository =
            new DatBanRepository();

        private readonly KhachHangRepository khachHangRepository =
            new KhachHangRepository();

        private readonly ChiTietDatBanRepository chiTietDatBanRepository =
            new ChiTietDatBanRepository();

        public bool DatBan(
            List<int> danhSachMaBan,
            string hoTenKhachHang,
            string soDienThoai,
            int soLuongKhach,
            DateTime ngayGioDat,
            string ghiChu,
            string diaChi)
        {
            // 1. Kiểm tra dữ liệu đầu vào
            if (danhSachMaBan == null || danhSachMaBan.Count == 0)
                throw new Exception("Vui lòng chọn ít nhất một bàn.");

            if (danhSachMaBan.Any(ma => ma <= 0))
                throw new Exception("Mã bàn không hợp lệ.");

            if (danhSachMaBan.Distinct().Count() != danhSachMaBan.Count)
                throw new Exception("Danh sách bàn không được trùng lặp.");

            if (string.IsNullOrWhiteSpace(hoTenKhachHang))
                throw new Exception("Vui lòng nhập họ tên khách hàng.");

            if (string.IsNullOrWhiteSpace(soDienThoai))
                throw new Exception("Vui lòng nhập số điện thoại.");

            if (!Regex.IsMatch(soDienThoai.Trim(), @"^0\d{9}$"))
                throw new Exception(
                    "Số điện thoại phải gồm 10 chữ số và bắt đầu bằng số 0.");

            if (soLuongKhach <= 0)
                throw new Exception("Số lượng khách phải lớn hơn 0.");

            if (ngayGioDat < DateTime.Now)
                throw new Exception(
                    "Thời gian đặt bàn không được nhỏ hơn thời gian hiện tại.");

            if (string.IsNullOrWhiteSpace(diaChi))
                throw new Exception("Vui lòng nhập địa chỉ khách hàng.");

            // 2. Chuẩn hóa thông tin
            hoTenKhachHang = Regex.Replace(
                hoTenKhachHang.Trim(),
                @"\s+",
                " ");

            soDienThoai = soDienThoai.Trim();
            diaChi = diaChi.Trim();

            // 3. Tách họ tên
            string[] parts = hoTenKhachHang.Split(' ');

            if (parts.Length < 2)
                throw new Exception(
                    "Vui lòng nhập đầy đủ họ và tên khách hàng.");

            string tenKhachHang = parts[parts.Length - 1];

            string hoVaTenLot = string.Join(
                " ",
                parts,
                0,
                parts.Length - 1);

            // 4. Mở kết nối và bắt đầu transaction
            using (SqlConnection conn = dbConnection.GetConnection())
            {
                conn.Open();

                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // 5. Kiểm tra các bàn và tính tổng sức chứa
                        int tongSoChoNgoi = 0;

                        foreach (int maBan in danhSachMaBan)
                        {
                            string sql = @"
                                SELECT soChoNgoi
                                FROM Ban WITH (UPDLOCK, HOLDLOCK)
                                WHERE maBan = @maBan
                                  AND trangThai = N'Trống'";

                            using (SqlCommand cmd = new SqlCommand(
                                sql, conn, transaction))
                            {
                                cmd.Parameters.Add(
                                    "@maBan", SqlDbType.Int).Value = maBan;

                                object result = cmd.ExecuteScalar();

                                if (result == null)
                                {
                                    throw new Exception(
                                        "Có bàn không còn trống hoặc không tồn tại. " +
                                        "Vui lòng kiểm tra lại danh sách bàn.");
                                }

                                tongSoChoNgoi += Convert.ToInt32(result);
                            }
                        }

                        // 6. Kiểm tra sức chứa
                        if (tongSoChoNgoi < soLuongKhach)
                        {
                            throw new Exception(
                                "Tổng sức chứa của các bàn đã chọn không đủ " +
                                "cho số lượng khách.");
                        }

                        // 7. Tìm khách hàng theo số điện thoại và họ tên
                        KhachHang khachHang =
                            khachHangRepository.TimTheoSoDienThoaiVaHoTen(
                                soDienThoai,
                                hoVaTenLot,
                                tenKhachHang,
                                conn,
                                transaction);

                        // 8. Nếu chưa có đúng khách hàng thì thêm mới
                        if (khachHang == null)
                        {
                            KhachHang khachHangMoi = new KhachHang
                            {
                                HoVaTenLot = hoVaTenLot,
                                TenKhachHang = tenKhachHang,
                                SoDienThoai = soDienThoai,
                                DiaChi = diaChi,
                                MaTaiKhoan = null
                            };

                            int maKhachHang =
                                khachHangRepository.ThemKhachHang(
                                    khachHangMoi,
                                    conn,
                                    transaction);

                            if (maKhachHang <= 0)
                                throw new Exception(
                                    "Không thể thêm thông tin khách hàng.");

                            khachHang = new KhachHang
                            {
                                MaKhachHang = maKhachHang,
                                HoVaTenLot = hoVaTenLot,
                                TenKhachHang = tenKhachHang,
                                SoDienThoai = soDienThoai,
                                DiaChi = diaChi
                            };
                        }

                        // 9. Tạo phiếu đặt bàn
                        DatBan datBan = new DatBan
                        {
                            NgayDat = ngayGioDat.Date,
                            GioDat = ngayGioDat.TimeOfDay,
                            SoLuongKhach = soLuongKhach,
                            TrangThai = "Đã đặt",
                            GhiChu = string.IsNullOrWhiteSpace(ghiChu)
                                ? null
                                : ghiChu.Trim(),
                            MaKhachHang = khachHang.MaKhachHang
                        };

                        // 10. Thêm phiếu đặt vào bảng DatBan
                        int maDatBan = datBanRepository.ThemDatBan(
                            datBan,
                            conn,
                            transaction);

                        if (maDatBan <= 0)
                            throw new Exception(
                                "Không thể tạo phiếu đặt bàn.");

                        // 11. Thêm danh sách bàn vào ChiTietDatBan
                        List<ChiTietDatBan> danhSachChiTiet =
                            danhSachMaBan.Select(maBan =>
                                new ChiTietDatBan
                                {
                                    MaDatBan = maDatBan,
                                    maBan = maBan
                                }).ToList();

                        chiTietDatBanRepository.ThemDanhSachBan(
                            danhSachChiTiet,
                            maDatBan,
                            conn,
                            transaction);

                        // 12. Cập nhật trạng thái các bàn
                        foreach (int maBan in danhSachMaBan)
                        {
                            string sql = @"
                                UPDATE Ban
                                SET trangThai = N'Đã đặt'
                                WHERE maBan = @maBan
                                  AND trangThai = N'Trống'";

                            using (SqlCommand cmd = new SqlCommand(
                                sql, conn, transaction))
                            {
                                cmd.Parameters.Add(
                                    "@maBan", SqlDbType.Int).Value = maBan;

                                int rows = cmd.ExecuteNonQuery();

                                if (rows != 1)
                                {
                                    throw new Exception(
                                        "Không thể cập nhật trạng thái bàn. " +
                                        "Vui lòng thử lại.");
                                }
                            }
                        }

                        // 13. Hoàn tất transaction
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
        public bool HuyDatBan(int maDatBan)
        {
            if (maDatBan <= 0)
                throw new Exception("Mã đặt bàn không hợp lệ.");

            using (SqlConnection conn = dbConnection.GetConnection())
            {
                conn.Open();

                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // 1. Kiểm tra phiếu đặt bàn
                        string sqlKiemTra = @"
                    SELECT trangThai
                    FROM DatBan WITH (UPDLOCK, HOLDLOCK)
                    WHERE maDatBan = @maDatBan";

                        string trangThai;

                        using (SqlCommand cmd = new SqlCommand(
                            sqlKiemTra, conn, transaction))
                        {
                            cmd.Parameters.Add("@maDatBan", SqlDbType.Int)
                                .Value = maDatBan;

                            object result = cmd.ExecuteScalar();

                            if (result == null || result == DBNull.Value)
                                throw new Exception("Không tìm thấy phiếu đặt bàn.");

                            trangThai = result.ToString();
                        }

                        // 2. Chỉ cho phép hủy phiếu chưa phục vụ
                        if (trangThai == "Đã hủy")
                            throw new Exception("Phiếu đặt bàn này đã được hủy.");

                        if (trangThai == "Đang phục vụ" ||
                            trangThai == "Đã hoàn thành")
                        {
                            throw new Exception(
                                "Không thể hủy phiếu đặt bàn đã bắt đầu phục vụ.");
                        }

                        if (trangThai == "Khách không đến")
                            throw new Exception(
                                "Phiếu đặt bàn đã được ghi nhận là khách không đến.");

                        // 3. Cập nhật trạng thái phiếu đặt bàn
                        string sqlHuyDatBan = @"
                    UPDATE DatBan
                    SET trangThai = N'Đã hủy'
                    WHERE maDatBan = @maDatBan";

                        using (SqlCommand cmd = new SqlCommand(
                            sqlHuyDatBan, conn, transaction))
                        {
                            cmd.Parameters.Add("@maDatBan", SqlDbType.Int)
                                .Value = maDatBan;

                            if (cmd.ExecuteNonQuery() != 1)
                                throw new Exception(
                                    "Không thể cập nhật trạng thái phiếu đặt bàn.");
                        }

                        // 4. Trả tất cả bàn thuộc phiếu về trạng thái Trống
                        string sqlTraBan = @"
                    UPDATE Ban
                    SET trangThai = N'Trống'
                    WHERE maBan IN
                    (
                        SELECT maBan
                        FROM ChiTietDatBan
                        WHERE maDatBan = @maDatBan
                    )
                    AND trangThai = N'Đã đặt'";

                        using (SqlCommand cmd = new SqlCommand(
                            sqlTraBan, conn, transaction))
                        {
                            cmd.Parameters.Add("@maDatBan", SqlDbType.Int)
                                .Value = maDatBan;

                            cmd.ExecuteNonQuery();
                        }

                        // 5. Hoàn tất transaction
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
        public bool HuyMotBan(int maDatBan, int maBan)
        {
            if (maDatBan <= 0 || maBan <= 0)
                throw new Exception("Mã đặt bàn hoặc mã bàn không hợp lệ.");

            using (SqlConnection conn = dbConnection.GetConnection())
            {
                conn.Open();

                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // 1. Kiểm tra trạng thái phiếu đặt bàn
                        string sqlKiemTra = @"
                    SELECT trangThai
                    FROM DatBan WITH (UPDLOCK, HOLDLOCK)
                    WHERE maDatBan = @maDatBan";

                        string trangThaiDatBan;

                        using (SqlCommand cmd = new SqlCommand(
                            sqlKiemTra, conn, transaction))
                        {
                            cmd.Parameters.Add("@maDatBan", SqlDbType.Int)
                                .Value = maDatBan;

                            object result = cmd.ExecuteScalar();

                            if (result == null || result == DBNull.Value)
                                throw new Exception("Không tìm thấy phiếu đặt bàn.");

                            trangThaiDatBan = result.ToString();
                        }

                        if (trangThaiDatBan == "Đã hủy" ||
                            trangThaiDatBan == "Đã hoàn thành" ||
                            trangThaiDatBan == "Khách không đến")
                        {
                            throw new Exception(
                                "Không thể hủy bàn trong phiếu đặt này.");
                        }

                        if (trangThaiDatBan == "Đang phục vụ")
                        {
                            throw new Exception(
                                "Không thể hủy bàn khi khách đang được phục vụ.");
                        }

                        // 2. Kiểm tra bàn có thuộc phiếu đặt không
                        string sqlKiemTraBan = @"
                    SELECT COUNT(*)
                    FROM ChiTietDatBan
                    WHERE maDatBan = @maDatBan
                      AND maBan = @maBan";

                        using (SqlCommand cmd = new SqlCommand(
                            sqlKiemTraBan, conn, transaction))
                        {
                            cmd.Parameters.Add("@maDatBan", SqlDbType.Int)
                                .Value = maDatBan;

                            cmd.Parameters.Add("@maBan", SqlDbType.Int)
                                .Value = maBan;

                            int count = Convert.ToInt32(cmd.ExecuteScalar());

                            if (count == 0)
                            {
                                throw new Exception(
                                    "Bàn này không thuộc phiếu đặt đang chọn.");
                            }
                        }

                        // 3. Xóa bàn khỏi chi tiết phiếu đặt
                        string sqlXoaChiTiet = @"
                    DELETE FROM ChiTietDatBan
                    WHERE maDatBan = @maDatBan
                      AND maBan = @maBan";

                        using (SqlCommand cmd = new SqlCommand(
                            sqlXoaChiTiet, conn, transaction))
                        {
                            cmd.Parameters.Add("@maDatBan", SqlDbType.Int)
                                .Value = maDatBan;

                            cmd.Parameters.Add("@maBan", SqlDbType.Int)
                                .Value = maBan;

                            if (cmd.ExecuteNonQuery() != 1)
                            {
                                throw new Exception(
                                    "Không thể xóa bàn khỏi chi tiết phiếu đặt.");
                            }
                        }

                        // 4. Trả bàn về trạng thái Trống
                        string sqlTraBan = @"
                    UPDATE Ban
                    SET trangThai = N'Trống'
                    WHERE maBan = @maBan
                      AND trangThai = N'Đã đặt'";

                        using (SqlCommand cmd = new SqlCommand(
                            sqlTraBan, conn, transaction))
                        {
                            cmd.Parameters.Add("@maBan", SqlDbType.Int)
                                .Value = maBan;

                            if (cmd.ExecuteNonQuery() != 1)
                            {
                                throw new Exception(
                                    "Không thể cập nhật trạng thái bàn.");
                            }
                        }

                        // 5. Kiểm tra phiếu còn bàn nào không
                        string sqlDemBan = @"
                    SELECT COUNT(*)
                    FROM ChiTietDatBan
                    WHERE maDatBan = @maDatBan";

                        int soBanConLai;

                        using (SqlCommand cmd = new SqlCommand(
                            sqlDemBan, conn, transaction))
                        {
                            cmd.Parameters.Add("@maDatBan", SqlDbType.Int)
                                .Value = maDatBan;

                            soBanConLai = Convert.ToInt32(cmd.ExecuteScalar());
                        }

                        // 6. Nếu không còn bàn thì hủy phiếu
                        if (soBanConLai == 0)
                        {
                            string sqlHuyPhieu = @"
                        UPDATE DatBan
                        SET trangThai = N'Đã hủy'
                        WHERE maDatBan = @maDatBan";

                            using (SqlCommand cmd = new SqlCommand(
                                sqlHuyPhieu, conn, transaction))
                            {
                                cmd.Parameters.Add("@maDatBan", SqlDbType.Int)
                                    .Value = maDatBan;

                                cmd.ExecuteNonQuery();
                            }
                        }

                        // 7. Hoàn tất transaction
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
    }
}
