
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

using QuanLyNhaHang.Data;
using QuanLyNhaHang.Models;
using QuanLyNhaHang.Repositories;
using QuanLyNhaHang.Services;
namespace QuanLyNhaHang.Forms
{
    public partial class frmChiTietDatBan : Form
    {
        private readonly int maDatBan;

        private readonly DatBanRepository datBanRepository =
            new DatBanRepository();

        private readonly ChiTietDatBanRepository chiTietDatBanRepository =
            new ChiTietDatBanRepository();
        private readonly DatBanService datBanService =
            new DatBanService();
        public frmChiTietDatBan(int maDatBan)
        {
            InitializeComponent();

            this.maDatBan = maDatBan;

            this.Load += frmChiTietDatBan_Load;

            btnDong.Click += btnDong_Click;
            btnInPhieu.Click += btnInPhieu_Click;
        }

        private void frmChiTietDatBan_Load(object sender, EventArgs e)
        {
            ThietLapGiaoDien();
            LoadThongTinDatBan();
        }

        // 1. THIẾT LẬP GIAO DIỆN

        private void ThietLapGiaoDien()
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(245, 249, 252);

            // Màu chủ đạo
            Color xanhChuDao = Color.FromArgb(0, 87, 153);
            Color xanhDam = Color.FromArgb(0, 65, 110);
            Color xanhNhat = Color.FromArgb(225, 243, 253);
            Color camNhan = Color.FromArgb(255, 139, 0);
            Color trang = Color.White;

            // Tiêu đề chính
            // Đổi tên pnlTieuDe theo tên Panel thực tế của bạn.
            // Ví dụ:
            // pnlTieuDe.BackColor = xanhChuDao;

            // Thiết lập DataGridView
            dgvDanhSachBan.AutoGenerateColumns = false;
            dgvDanhSachBan.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            dgvDanhSachBan.ReadOnly = true;
            dgvDanhSachBan.AllowUserToAddRows = false;
            dgvDanhSachBan.AllowUserToDeleteRows = false;
            dgvDanhSachBan.RowHeadersVisible = false;

            dgvDanhSachBan.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            dgvDanhSachBan.MultiSelect = false;
            dgvDanhSachBan.BackgroundColor = trang;
            dgvDanhSachBan.BorderStyle = BorderStyle.None;

            dgvDanhSachBan.CellBorderStyle =
                DataGridViewCellBorderStyle.SingleHorizontal;

            dgvDanhSachBan.GridColor =
                Color.FromArgb(220, 230, 238);

            dgvDanhSachBan.EnableHeadersVisualStyles = false;

            // Header
            dgvDanhSachBan.ColumnHeadersDefaultCellStyle.BackColor =
                xanhChuDao;

            dgvDanhSachBan.ColumnHeadersDefaultCellStyle.ForeColor =
                trang;

            dgvDanhSachBan.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 10, FontStyle.Bold);

            dgvDanhSachBan.ColumnHeadersDefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleCenter;

            dgvDanhSachBan.ColumnHeadersHeight = 42;
            dgvDanhSachBan.ColumnHeadersHeightSizeMode =
                DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            // Nội dung
            dgvDanhSachBan.DefaultCellStyle.Font =
                new Font("Segoe UI", 10, FontStyle.Regular);

            dgvDanhSachBan.DefaultCellStyle.ForeColor = xanhDam;

            dgvDanhSachBan.DefaultCellStyle.BackColor = trang;

            dgvDanhSachBan.DefaultCellStyle.SelectionBackColor =
                Color.FromArgb(204, 231, 248);

            dgvDanhSachBan.DefaultCellStyle.SelectionForeColor =
                xanhDam;

            dgvDanhSachBan.AlternatingRowsDefaultCellStyle.BackColor =
                Color.FromArgb(242, 248, 252);

            dgvDanhSachBan.RowTemplate.Height = 38;

            // Form
            this.Font = new Font("Segoe UI", 10);
        }


        // 2. LOAD THÔNG TIN PHIẾU ĐẶT BÀN
        private void LoadThongTinDatBan()
        {
            try
            {
                using (SqlConnection conn =
                    new DbConnection().GetConnection())
                {
                    conn.Open();

                    DatBan datBan =
                        datBanRepository.LayDatBanTheoMa(
                            maDatBan, conn);

                    if (datBan == null)
                    {
                        MessageBox.Show(
                            "Không tìm thấy thông tin phiếu đặt bàn!",
                            "Thông báo",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);

                        this.Close();
                        return;
                    }

                    // Thông tin phiếu đặt
                    lblMaDatBan.Text =
                        "Mã phiếu: DB" +
                        datBan.MaDatBan.ToString("D3");

                    lblTrangThai.Text = datBan.TrangThai;

                    // Thông tin khách hàng
                    lblHoTen.Text =
                        "Họ tên: " + (datBan.TenKhachHang ?? "Chưa cập nhật");

                    lblSoDienThoai.Text =
                        "Số điện thoại: " + (datBan.SoDienThoai ?? "Chưa cập nhật");

                    lblDiaChi.Text =
                        "Địa chỉ: " + (datBan.DiaChi ?? "Chưa cập nhật");

                    // Thông tin đặt bàn
                    lblNgayDat.Text =
                        "Ngày đặt: " +
                        datBan.NgayDat.ToString("dd/MM/yyyy");

                    lblGioDat.Text =
                        "Giờ đặt: " +
                        datBan.GioDat.ToString(@"hh\:mm");

                    lblSoLuongKhach.Text =
                        "Số lượng khách: " +
                        datBan.SoLuongKhach + " người";

                    lblGhiChu.Text =
                        "Ghi chú: " +
                        (datBan.GhiChu ?? "Không có");

                    // Danh sách bàn
                    List<ChiTietDatBan> danhSachBan =
                        chiTietDatBanRepository
                        .LayDanhSachBanTheoMaDatBan(
                            maDatBan, conn);

                    dgvDanhSachBan.DataSource = danhSachBan;

                    // Chỉ hiển thị các cột cần thiết
                    AnHienCot();

                    // Thống kê
                    lblTongSoBan.Text =
                        "Tổng số bàn: " + danhSachBan.Count;

                    int tongSoChoNgoi = 0;

                    foreach (ChiTietDatBan ban in danhSachBan)
                    {
                        tongSoChoNgoi += ban.soChoNgoi;
                    }

                    lblTongSoChoNgoi.Text =
                        "Tổng sức chứa: " +
                        tongSoChoNgoi + " chỗ";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi khi tải thông tin đặt bàn:\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
        // 3. ẨN CÁC CỘT KHÔNG CẦN THIẾT
        private void AnHienCot()
        {
            // Ẩn mã chi tiết đặt bàn và mã phiếu đặt
            if (dgvDanhSachBan.Columns["MaChiTietDatBan"] != null)
                dgvDanhSachBan.Columns["MaChiTietDatBan"].Visible = false;

            if (dgvDanhSachBan.Columns["MaDatBan"] != null)
                dgvDanhSachBan.Columns["MaDatBan"].Visible = false;

            // Mã bàn
            if (dgvDanhSachBan.Columns["maBan"] != null)
            {
                dgvDanhSachBan.Columns["maBan"].Visible = true;
                dgvDanhSachBan.Columns["maBan"].HeaderText = "Mã bàn";
                dgvDanhSachBan.Columns["maBan"].DisplayIndex = 0;
            }

            // Tên bàn
            if (dgvDanhSachBan.Columns["soBan"] != null)
            {
                dgvDanhSachBan.Columns["soBan"].Visible = true;
                dgvDanhSachBan.Columns["soBan"].HeaderText = "Tên bàn";
                dgvDanhSachBan.Columns["soBan"].DisplayIndex = 1;
            }

            // Số chỗ ngồi
            if (dgvDanhSachBan.Columns["soChoNgoi"] != null)
            {
                dgvDanhSachBan.Columns["soChoNgoi"].Visible = true;
                dgvDanhSachBan.Columns["soChoNgoi"].HeaderText = "Số chỗ ngồi";
                dgvDanhSachBan.Columns["soChoNgoi"].DisplayIndex = 2;
            }

            // Trạng thái
            if (dgvDanhSachBan.Columns["trangThai"] != null)
            {
                dgvDanhSachBan.Columns["trangThai"].Visible = true;
                dgvDanhSachBan.Columns["trangThai"].HeaderText = "Trạng thái";
                dgvDanhSachBan.Columns["trangThai"].DisplayIndex = 3;
            }
        }
        // 4. NÚT ĐÓNG
        private void btnDong_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        // 5. NÚT IN PHIẾU
        private void btnInPhieu_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "Chức năng in phiếu sẽ được triển khai sau!",
                "Thông báo",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void btnHuyBan_Click(object sender, EventArgs e)
        {
            // 1. Kiểm tra bàn được chọn
            int? maBan = LayMaBanDangChon();

            if (!maBan.HasValue)
            {
                MessageBox.Show(
                    "Vui lòng chọn bàn cần hủy!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            // 2. Xác nhận hủy
            DialogResult result = MessageBox.Show(
                "Bạn có chắc chắn muốn hủy bàn này không?",
                "Xác nhận hủy bàn",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
                return;

            try
            {
                // 3. Gọi Service hủy bàn
                datBanService.HuyMotBan(maDatBan, maBan.Value);

                MessageBox.Show(
                    "Hủy bàn thành công!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                // 4. Tải lại thông tin
                LoadThongTinDatBan();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnHuyToanBo_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Bạn có chắc chắn muốn hủy toàn bộ phiếu đặt bàn này không?\n" +
                "Tất cả bàn thuộc phiếu sẽ được trả về trạng thái Trống.",
                "Xác nhận hủy toàn bộ",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result != DialogResult.Yes)
                return;

            try
            {
                // 1. Gọi Service hủy toàn bộ phiếu
                datBanService.HuyDatBan(maDatBan);

                MessageBox.Show(
                    "Đã hủy toàn bộ phiếu đặt bàn thành công!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                // 2. Đóng form chi tiết
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
        
        private int? LayMaBanDangChon()
        {
            if (dgvDanhSachBan.CurrentRow == null)
                return null;

            object value =
                dgvDanhSachBan.CurrentRow.Cells["maBan"].Value;

            if (value == null || value == DBNull.Value)
                return null;

            return Convert.ToInt32(value);
        }

    }
}
