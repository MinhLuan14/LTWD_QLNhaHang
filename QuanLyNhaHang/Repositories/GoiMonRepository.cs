using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using QuanLyNhaHang.Data;
using QuanLyNhaHang.Models;

namespace QuanLyNhaHang.Repositories
{
    public class GoiMonRepository
    {
        private readonly DbConnection db = new DbConnection();

        public PhieuGoiMon GetPhieuGoiMonHienTai(int maBan)
        {
            try
            {
                using (SqlConnection conn = db.GetConnection())
                {
                    conn.Open();
                    string sql = @"
                        SELECT TOP 1 maPhieuGoiMon, maBan, maNhanVien, ngayGoi, gioGoi, trangThai, ghiChu
                        FROM PhieuGoiMon
                        WHERE maBan = @maBan AND trangThai <> N'Đã thanh toán'
                        ORDER BY maPhieuGoiMon DESC";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@maBan", maBan);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new PhieuGoiMon
                                {
                                    MaPhieuGoiMon = Convert.ToInt32(reader["maPhieuGoiMon"]),
                                    MaBan = Convert.ToInt32(reader["maBan"]),
                                    MaNhanVien = reader["maNhanVien"] != DBNull.Value ? (int?)Convert.ToInt32(reader["maNhanVien"]) : null,
                                    NgayGoi = Convert.ToDateTime(reader["ngayGoi"]),
                                    GioGoi = (TimeSpan)reader["gioGoi"],
                                    TrangThai = reader["trangThai"].ToString(),
                                    GhiChu = reader["ghiChu"] != DBNull.Value ? reader["ghiChu"].ToString() : string.Empty
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Trả về null nếu có lỗi
            }
            return null;
        }

        public List<ChiTietPhieuGoiMon> GetChiTietGoiMon(int maPhieuGoiMon)
        {
            List<ChiTietPhieuGoiMon> list = new List<ChiTietPhieuGoiMon>();
            try
            {
                using (SqlConnection conn = db.GetConnection())
                {
                    conn.Open();
                    string sql = @"
                        SELECT c.maChiTietPhieuGoiMon, c.maPhieuGoiMon, c.maMonAn, m.tenMonAn, c.soLuong, c.donGia
                        FROM ChiTietPhieuGoiMon c
                        INNER JOIN MonAn m ON c.maMonAn = m.maMonAn
                        WHERE c.maPhieuGoiMon = @maPhieuGoiMon
                        ORDER BY c.maChiTietPhieuGoiMon";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@maPhieuGoiMon", maPhieuGoiMon);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                list.Add(new ChiTietPhieuGoiMon
                                {
                                    MaChiTietPhieuGoiMon = Convert.ToInt32(reader["maChiTietPhieuGoiMon"]),
                                    MaPhieuGoiMon = Convert.ToInt32(reader["maPhieuGoiMon"]),
                                    MaMonAn = Convert.ToInt32(reader["maMonAn"]),
                                    TenMonAn = reader["tenMonAn"].ToString(),
                                    SoLuong = Convert.ToInt32(reader["soLuong"]),
                                    DonGia = Convert.ToDecimal(reader["donGia"])
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            return list;
        }

        public bool ThemHoacCapNhatMon(int maBan, int maMonAn, int soLuongThayDoi, decimal donGia, string ghiChu, int maNhanVien = 1)
        {
            using (SqlConnection conn = db.GetConnection())
            {
                conn.Open();
                using (SqlTransaction tran = conn.BeginTransaction())
                {
                    try
                    {
                        int maPhieu = 0;
                        string sqlCheckPhieu = @"
                            SELECT TOP 1 maPhieuGoiMon 
                            FROM PhieuGoiMon 
                            WHERE maBan = @maBan AND trangThai <> N'Đã thanh toán'
                            ORDER BY maPhieuGoiMon DESC";

                        using (SqlCommand cmdCheck = new SqlCommand(sqlCheckPhieu, conn, tran))
                        {
                            cmdCheck.Parameters.AddWithValue("@maBan", maBan);
                            object objPhieu = cmdCheck.ExecuteScalar();
                            if (objPhieu != null && objPhieu != DBNull.Value)
                            {
                                maPhieu = Convert.ToInt32(objPhieu);
                            }
                        }

                        // Nếu chưa có phiếu thì tạo mới
                        if (maPhieu == 0)
                        {
                            if (soLuongThayDoi <= 0)
                            {
                                tran.Rollback();
                                return false; // Không thể bớt món khi chưa có phiếu
                            }

                            string sqlInsertPhieu = @"
                                INSERT INTO PhieuGoiMon (maBan, maNhanVien, ngayGoi, gioGoi, trangThai, ghiChu)
                                VALUES (@maBan, @maNhanVien, CAST(GETDATE() AS DATE), CAST(GETDATE() AS TIME(0)), N'Đang phục vụ', @ghiChu);
                                SELECT SCOPE_IDENTITY();";

                            using (SqlCommand cmdInsertPhieu = new SqlCommand(sqlInsertPhieu, conn, tran))
                            {
                                cmdInsertPhieu.Parameters.AddWithValue("@maBan", maBan);
                                cmdInsertPhieu.Parameters.AddWithValue("@maNhanVien", maNhanVien);
                                cmdInsertPhieu.Parameters.AddWithValue("@ghiChu", string.IsNullOrWhiteSpace(ghiChu) ? (object)DBNull.Value : ghiChu);
                                maPhieu = Convert.ToInt32(cmdInsertPhieu.ExecuteScalar());
                            }

                            // Cập nhật trạng thái bàn sang Đang phục vụ
                            string sqlUpdateBan = @"UPDATE Ban SET trangThai = N'Đang phục vụ' WHERE maBan = @maBan";
                            using (SqlCommand cmdBan = new SqlCommand(sqlUpdateBan, conn, tran))
                            {
                                cmdBan.Parameters.AddWithValue("@maBan", maBan);
                                cmdBan.ExecuteNonQuery();
                            }
                        }

                        // Kiểm tra xem món đã có trong chi tiết chưa
                        string sqlCheckChiTiet = @"
                            SELECT maChiTietPhieuGoiMon, soLuong 
                            FROM ChiTietPhieuGoiMon 
                            WHERE maPhieuGoiMon = @maPhieuGoiMon AND maMonAn = @maMonAn";

                        int maChiTiet = 0;
                        int soLuongCu = 0;
                        using (SqlCommand cmdCheckChiTiet = new SqlCommand(sqlCheckChiTiet, conn, tran))
                        {
                            cmdCheckChiTiet.Parameters.AddWithValue("@maPhieuGoiMon", maPhieu);
                            cmdCheckChiTiet.Parameters.AddWithValue("@maMonAn", maMonAn);
                            using (SqlDataReader r = cmdCheckChiTiet.ExecuteReader())
                            {
                                if (r.Read())
                                {
                                    maChiTiet = Convert.ToInt32(r["maChiTietPhieuGoiMon"]);
                                    soLuongCu = Convert.ToInt32(r["soLuong"]);
                                }
                            }
                        }

                        if (maChiTiet > 0)
                        {
                            int soLuongMoi = soLuongCu + soLuongThayDoi;
                            if (soLuongMoi > 0)
                            {
                                string sqlUpdateChiTiet = @"
                                    UPDATE ChiTietPhieuGoiMon 
                                    SET soLuong = @soLuong, donGia = @donGia 
                                    WHERE maChiTietPhieuGoiMon = @maChiTiet";
                                using (SqlCommand cmdUpdate = new SqlCommand(sqlUpdateChiTiet, conn, tran))
                                {
                                    cmdUpdate.Parameters.AddWithValue("@soLuong", soLuongMoi);
                                    cmdUpdate.Parameters.AddWithValue("@donGia", donGia);
                                    cmdUpdate.Parameters.AddWithValue("@maChiTiet", maChiTiet);
                                    cmdUpdate.ExecuteNonQuery();
                                }
                            }
                            else
                            {
                                // Số lượng <= 0 thì xoá món
                                string sqlDelete = "DELETE FROM ChiTietPhieuGoiMon WHERE maChiTietPhieuGoiMon = @maChiTiet";
                                using (SqlCommand cmdDelete = new SqlCommand(sqlDelete, conn, tran))
                                {
                                    cmdDelete.Parameters.AddWithValue("@maChiTiet", maChiTiet);
                                    cmdDelete.ExecuteNonQuery();
                                }
                            }
                        }
                        else
                        {
                            if (soLuongThayDoi > 0)
                            {
                                string sqlInsertChiTiet = @"
                                    INSERT INTO ChiTietPhieuGoiMon (maPhieuGoiMon, maMonAn, soLuong, donGia)
                                    VALUES (@maPhieuGoiMon, @maMonAn, @soLuong, @donGia)";
                                using (SqlCommand cmdInsert = new SqlCommand(sqlInsertChiTiet, conn, tran))
                                {
                                    cmdInsert.Parameters.AddWithValue("@maPhieuGoiMon", maPhieu);
                                    cmdInsert.Parameters.AddWithValue("@maMonAn", maMonAn);
                                    cmdInsert.Parameters.AddWithValue("@soLuong", soLuongThayDoi);
                                    cmdInsert.Parameters.AddWithValue("@donGia", donGia);
                                    cmdInsert.ExecuteNonQuery();
                                }
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(ghiChu))
                        {
                            string sqlUpdateNote = "UPDATE PhieuGoiMon SET ghiChu = @ghiChu WHERE maPhieuGoiMon = @maPhieu";
                            using (SqlCommand cmdNote = new SqlCommand(sqlUpdateNote, conn, tran))
                            {
                                cmdNote.Parameters.AddWithValue("@ghiChu", ghiChu);
                                cmdNote.Parameters.AddWithValue("@maPhieu", maPhieu);
                                cmdNote.ExecuteNonQuery();
                            }
                        }

                        tran.Commit();
                        return true;
                    }
                    catch (Exception)
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
        }

        public bool XoaMonKhoiPhieu(int maChiTiet)
        {
            using (SqlConnection conn = db.GetConnection())
            {
                conn.Open();
                string sql = "DELETE FROM ChiTietPhieuGoiMon WHERE maChiTietPhieuGoiMon = @maChiTiet";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@maChiTiet", maChiTiet);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}
