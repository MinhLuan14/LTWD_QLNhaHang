
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using QuanLyNhaHang.Models;
using QuanLyNhaHang.Services;

namespace QuanLyNhaHang
{
    public partial class frmThanhToan : Form
    {
        private readonly ThanhToanService thanhToanService =
            new ThanhToanService();

        private int maThanhToanDangChon = 0;
        private const string PLACEHOLDER_TIM_KIEM ="Nhập mã thanh toán, hóa đơn";
        private const string TAT_CA = "Tất cả";
        private const string CHO_XAC_NHAN = "Đang xử lý";
        private const string DA_THANH_TOAN = "Đã thanh toán";
        private const string KHONG_THANH_CONG = "Không thành công";
        private const string TIEN_MAT = "Tiền mặt";
        private const string CHUYEN_KHOAN = "Chuyển khoản";

        public frmThanhToan()
        {
            InitializeComponent();

            // Đưa nội dung chính lên phía trước nếu Designer có panel chồng lấp.
            pnlNoiDung.BringToFront();

            // Cấu hình giao diện trước khi đăng ký sự kiện lọc.
            CauHinhGiaoDien();
            txtTimKiemMaTTAnMaHD.Enter += txtTimKiem_Enter;
            txtTimKiemMaTTAnMaHD.Leave += txtTimKiem_Leave;
            // Đăng ký sự kiện.
            Load += frmThanhToan_Load;
            txtTimKiemMaTTAnMaHD.TextChanged += BoLoc_Changed;
            cboLocPhuongThuc.SelectedIndexChanged += BoLoc_Changed;
            cboLocTrangThai.SelectedIndexChanged += BoLoc_Changed;
            dgvDanhSachThanhToan.CellClick +=
                dgvDanhSachThanhToan_CellClick;
        }

        // 1. Cấu hình bảng và bộ lọc.
        private void CauHinhGiaoDien()
        {
            dgvDanhSachThanhToan.AutoGenerateColumns = false;
            dgvDanhSachThanhToan.ReadOnly = true;
            dgvDanhSachThanhToan.AllowUserToAddRows = false;
            dgvDanhSachThanhToan.AllowUserToDeleteRows = false;
            dgvDanhSachThanhToan.MultiSelect = false;
            dgvDanhSachThanhToan.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;
            dgvDanhSachThanhToan.RowHeadersVisible = false;
            dgvDanhSachThanhToan.AutoGenerateColumns = false;
            dgvDanhSachThanhToan.ReadOnly = true;
            dgvDanhSachThanhToan.AllowUserToAddRows = false;
            dgvDanhSachThanhToan.AllowUserToDeleteRows = false;
            dgvDanhSachThanhToan.MultiSelect = false;

            dgvDanhSachThanhToan.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            dgvDanhSachThanhToan.RowHeadersVisible = false;

            // Thanh cuộn dọc
            dgvDanhSachThanhToan.ScrollBars = ScrollBars.Vertical;
            MaThanhToan.DataPropertyName = "MaThanhToan";
            MaHoaDon.DataPropertyName = "MaHoaDon";
            Ban.DataPropertyName = "TenBan";
            SoTienThanhToan.DataPropertyName = "SoTienThanhToan";
            PhuongThuc.DataPropertyName = "PhuongThucThanhToan";
            NgayThanhToan.DataPropertyName = "NgayThanhToan";
            TrangThai.DataPropertyName = "TrangThai";
            txtTimKiemMaTTAnMaHD.Text = PLACEHOLDER_TIM_KIEM;
            txtTimKiemMaTTAnMaHD.ForeColor = Color.Gray;
            MaThanhToan.HeaderText = "Mã thanh toán";
            MaHoaDon.HeaderText = "Mã hóa đơn";
            Ban.HeaderText = "Bàn";
            SoTienThanhToan.HeaderText = "Số tiền";
            PhuongThuc.HeaderText = "Phương thức";
            NgayThanhToan.HeaderText = "Ngày thanh toán";
            TrangThai.HeaderText = "Trạng thái";

            SoTienThanhToan.DefaultCellStyle.Format = "N0";
            NgayThanhToan.DefaultCellStyle.Format = "dd/MM/yyyy";

            cboLocPhuongThuc.DropDownStyle =
                ComboBoxStyle.DropDownList;

            cboLocTrangThai.DropDownStyle =
                ComboBoxStyle.DropDownList;

            cboLocPhuongThuc.Items.Clear();
            cboLocPhuongThuc.Items.AddRange(new object[]
            {
                TAT_CA,
                TIEN_MAT,
                CHUYEN_KHOAN
            });
            cboLocPhuongThuc.SelectedIndex = 0;

            cboLocTrangThai.Items.Clear();
            cboLocTrangThai.Items.AddRange(new object[]
            {
                TAT_CA,
                CHO_XAC_NHAN,
                DA_THANH_TOAN,
                KHONG_THANH_CONG
            });
            cboLocTrangThai.SelectedIndex = 0;

            label1.Text = "Chờ xác nhận";
            label2.Text = "Đã thanh toán";
            label3.Text = "Không thành công";

            XoaChiTiet();
        }

        // 2. Khi mở form, tải danh sách và thống kê.
        private void frmThanhToan_Load(object sender, EventArgs e)
        {
            TaiDuLieuThanhToan();
            TaiThongKeThanhToan();
        }

        // 3. Tải danh sách giao dịch.
        private void TaiDuLieuThanhToan()
        {
            try
            {
                string tuKhoa = txtTimKiemMaTTAnMaHD.Text.Trim();

                if (tuKhoa == PLACEHOLDER_TIM_KIEM)
                    tuKhoa = "";

                string phuongThuc =
                    cboLocPhuongThuc.SelectedItem?.ToString() ?? "";

                string trangThai =
                    cboLocTrangThai.SelectedItem?.ToString() ?? "";

                if (phuongThuc == TAT_CA)
                    phuongThuc = "";

                if (trangThai == TAT_CA)
                    trangThai = "";

                List<ThanhToan> danhSach =
                    thanhToanService.GetDanhSachThanhToan(
                        tuKhoa,
                        phuongThuc,
                        trangThai
                    );

                dgvDanhSachThanhToan.DataSource = null;
                dgvDanhSachThanhToan.DataSource = danhSach;

                DinhDangDanhSach();

                dgvDanhSachThanhToan.ClearSelection();

                if (danhSach.Count == 0)
                    XoaChiTiet();
                lblThongBaoKiemTra.Text ="Vui lòng chọn giao dịch để xem thông tin.";
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể tải danh sách thanh toán!\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // 4. Định dạng màu trạng thái trong bảng.
        private void DinhDangDanhSach()
        {
            foreach (DataGridViewRow row in dgvDanhSachThanhToan.Rows)
            {
                string trangThai =
                    Convert.ToString(row.Cells["TrangThai"].Value);

                if (trangThai == CHO_XAC_NHAN)
                {
                    row.Cells["TrangThai"].Style.ForeColor =
                        Color.DarkOrange;
                }
                else if (trangThai == DA_THANH_TOAN)
                {
                    row.Cells["TrangThai"].Style.ForeColor =
                        Color.Green;
                }
                else if (trangThai == KHONG_THANH_CONG)
                {
                    row.Cells["TrangThai"].Style.ForeColor =
                        Color.Red;
                }
            }
        }

        // 5. Tải thống kê giao dịch.
        private void TaiThongKeThanhToan()
        {
            try
            {
                ThongKeThanhToan thongKe =
                    thanhToanService.GetThongKeThanhToan();

                lblHDChoXacNhan.Text =
                    thongKe.ChoXacNhan.ToString();

                lblDaThanhToan.Text =
                    thongKe.DaThanhToan.ToString();

                lblKhongThanhCong.Text =
                    thongKe.KhongThanhCong.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể tải thống kê thanh toán!\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // 6. Xử lý khi thay đổi từ khóa hoặc bộ lọc.
        private void BoLoc_Changed(object sender, EventArgs e)
        {
            TaiDuLieuThanhToan();
        }

        // 7. Khi chọn giao dịch trong bảng.
        private void dgvDanhSachThanhToan_CellClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            object giaTri =
                dgvDanhSachThanhToan.Rows[e.RowIndex]
                .Cells["MaThanhToan"].Value;

            if (giaTri == null || giaTri == DBNull.Value)
                return;

            maThanhToanDangChon = Convert.ToInt32(giaTri);

            TaiChiTietThanhToan(maThanhToanDangChon);
        }

        // 8. Tải chi tiết một giao dịch.
        private void TaiChiTietThanhToan(int maThanhToan)
        {
            try
            {
                ThanhToan thanhToan =
                    thanhToanService.GetChiTietThanhToan(maThanhToan);

                if (thanhToan == null)
                {
                    XoaChiTiet();
                    return;
                }

                lblMaThanhToan.Text =
                    "TT" + thanhToan.MaThanhToan.ToString("D4");

                lblMaHoaDon.Text =
                    "HD" + thanhToan.MaHoaDon.ToString("D4");

                lblPhuongThuc.Text =
                    thanhToan.PhuongThucThanhToan;

                lblSoTienThanhToan.Text =
                    thanhToan.SoTienThanhToan.ToString("N0") + " đ";

                lblNgayGioThanhToan.Text =
                    thanhToan.NgayGioThanhToan.ToString(
                        "dd/MM/yyyy HH:mm:ss"
                    );

                lblTrangThai.Text = thanhToan.TrangThai;

                // Cơ sở dữ liệu hiện chưa có hai cột này.
                lblMaGiaoDich.Text = "Chưa cập nhật";
                lblGhiChu.Text = "Không có";

                CapNhatGiaoDienTheoTrangThai(
                    thanhToan.PhuongThucThanhToan,
                    thanhToan.TrangThai
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể tải chi tiết thanh toán!\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // 9. Cập nhật nút thao tác theo phương thức và trạng thái.
        private void CapNhatGiaoDienTheoTrangThai(
            string phuongThuc,
            string trangThai)
        {
            bool coTheDuyet =
             phuongThuc == CHUYEN_KHOAN &&
             (trangThai == CHO_XAC_NHAN ||
              trangThai == KHONG_THANH_CONG);

            btnXacNhanThanhToan.Enabled = coTheDuyet;

            // Chỉ cho phép từ chối khi giao dịch đang chờ xác nhận.
            btnTuChoiThanhToan.Enabled =
                phuongThuc == CHUYEN_KHOAN &&
                trangThai == CHO_XAC_NHAN;


            if (phuongThuc == TIEN_MAT)
            {
                lblThongBaoKiemTra.Text =
                    "Thanh toán tiền mặt đã được ghi nhận tại quầy.";
            }
            else if (trangThai == KHONG_THANH_CONG)
            {
                lblThongBaoKiemTra.Text =
                    "Giao dịch đã bị từ chối. Có thể xác nhận lại nếu đã nhận được tiền.";
            }
            else if (coTheDuyet)
            {
                lblThongBaoKiemTra.Text =
                    "Vui lòng kiểm tra tài khoản ngân hàng trước khi xác nhận.";
            }
            else if (trangThai == DA_THANH_TOAN)
            {
                lblThongBaoKiemTra.Text =
                    "Giao dịch đã được xác nhận thanh toán.";
            }
            else
            {
                lblThongBaoKiemTra.Text =
                    "Giao dịch không còn ở trạng thái chờ xác nhận.";
            }


            if (trangThai == CHO_XAC_NHAN)
                lblTrangThai.ForeColor = Color.DarkOrange;
            else if (trangThai == DA_THANH_TOAN)
                lblTrangThai.ForeColor = Color.Green;
            else
                lblTrangThai.ForeColor = Color.Red;
        }

        // 10. Xác nhận đã nhận được tiền chuyển khoản.
        private void btnXacNhanThanhToan_Click(
            object sender,
            EventArgs e)
        {
            if (maThanhToanDangChon <= 0)
            {
                MessageBox.Show(
                    "Vui lòng chọn giao dịch cần xác nhận!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            DialogResult result = MessageBox.Show(
                "Bạn đã kiểm tra và xác nhận đã nhận được tiền từ giao dịch này?",
                "Xác nhận thanh toán",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result != DialogResult.Yes)
                return;

            try
            {
                bool ketQua =
                    thanhToanService.XacNhanThanhToan(
                        maThanhToanDangChon
                    );

                if (!ketQua)
                {
                    MessageBox.Show(
                        "Không thể xác nhận giao dịch này!",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                MessageBox.Show(
                    "Xác nhận thanh toán thành công!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                LamMoiDuLieu();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể xác nhận thanh toán!\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // 11. Từ chối giao dịch chuyển khoản.
        private void btnTuChoiThanhToan_Click(
            object sender,
            EventArgs e)
        {
            if (maThanhToanDangChon <= 0)
            {
                MessageBox.Show(
                    "Vui lòng chọn giao dịch cần từ chối!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            DialogResult result = MessageBox.Show(
                "Bạn có chắc chắn muốn từ chối giao dịch này?\n" +
                "Chỉ từ chối khi đã kiểm tra và xác định chưa nhận được tiền.",
                "Từ chối thanh toán",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result != DialogResult.Yes)
                return;

            try
            {
                bool ketQua =
                    thanhToanService.TuChoiThanhToan(
                        maThanhToanDangChon
                    );

                if (!ketQua)
                {
                    MessageBox.Show(
                        "Không thể từ chối giao dịch này!",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                MessageBox.Show(
                    "Đã từ chối giao dịch thanh toán!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                LamMoiDuLieu();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể từ chối giao dịch!\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // 12. Xóa thông tin chi tiết.
        private void XoaChiTiet()
        {
            maThanhToanDangChon = 0;

            lblMaThanhToan.Text = "---";
            lblMaHoaDon.Text = "---";
            lblPhuongThuc.Text = "---";
            lblNgayGioThanhToan.Text = "---";
            lblMaGiaoDich.Text = "Chưa cập nhật";
            lblGhiChu.Text = "Không có";
            lblSoTienThanhToan.Text = "0 đ";
            lblTrangThai.Text = "---";

            lblThongBaoKiemTra.Text =
                "Vui lòng chọn một giao dịch để xem chi tiết.";

            btnXacNhanThanhToan.Enabled = false;
            btnTuChoiThanhToan.Enabled = false;
        }
        // Khi nhấn vào ô tìm kiếm.
        private void txtTimKiem_Enter(object sender, EventArgs e)
        {
            if (txtTimKiemMaTTAnMaHD.Text == PLACEHOLDER_TIM_KIEM)
            {
                txtTimKiemMaTTAnMaHD.Text = "";
                txtTimKiemMaTTAnMaHD.ForeColor = Color.Black;
            }
        }

        // Khi rời ô tìm kiếm.
        private void txtTimKiem_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(
                txtTimKiemMaTTAnMaHD.Text))
            {
                txtTimKiemMaTTAnMaHD.Text = PLACEHOLDER_TIM_KIEM;
                txtTimKiemMaTTAnMaHD.ForeColor = Color.Gray;
            }
        }
        // 13. Làm mới danh sách và thống kê.
        private void LamMoiDuLieu()
        {
            XoaChiTiet();
            TaiDuLieuThanhToan();
            TaiThongKeThanhToan();
        }
    }
}
