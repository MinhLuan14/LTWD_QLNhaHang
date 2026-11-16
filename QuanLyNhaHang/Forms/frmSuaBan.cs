
using System;
using System.Windows.Forms;
using QuanLyNhaHang.Models;
using QuanLyNhaHang.Services;
namespace QuanLyNhaHang.Forms
{
    public partial class frmSuaBan : Form
    {
        // Lưu mã bàn để sử dụng khi cập nhật SQL
        private int maBan;
        private readonly BanService banService = new BanService();
        // Constructor mặc định dành cho Designer
        public frmSuaBan()
        {
            InitializeComponent();
            LoadTrangThai();
        }

        // Constructor nhận thông tin bàn từ frmQuanLyBan
        public frmSuaBan(int maBan, string tenBan, int soCho, string trangThai)
        {
            InitializeComponent();

            this.maBan = maBan;

            LoadTrangThai();
            HienThiThongTinBan(tenBan, soCho, trangThai);
        }

        // Nạp danh sách trạng thái
        private void LoadTrangThai()
        {
            cboTrangThai.Items.Clear();

            cboTrangThai.Items.Add("Trống");
            cboTrangThai.Items.Add("Đang phục vụ");
            cboTrangThai.Items.Add("Đã đặt");

            cboTrangThai.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        // Hiển thị thông tin bàn lên giao diện
        private void HienThiThongTinBan(
            string tenBan,
            int soCho,
            string trangThai)
        {
            txtTenBan.Text = tenBan;

            if (soCho >= nudSoCho.Minimum &&
                soCho <= nudSoCho.Maximum)
            {
                nudSoCho.Value = soCho;
            }

            if (cboTrangThai.Items.Contains(trangThai))
            {
                cboTrangThai.SelectedItem = trangThai;
            }
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnThayDoi_Click(object sender, EventArgs e)
        {
            // Kiểm tra tên bàn
            if (string.IsNullOrWhiteSpace(txtTenBan.Text))
            {
                MessageBox.Show(
                    "Vui lòng nhập tên bàn!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtTenBan.Focus();
                return;
            }

            // Kiểm tra trạng thái
            if (cboTrangThai.SelectedIndex == -1)
            {
                MessageBox.Show(
                    "Vui lòng chọn trạng thái bàn!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                cboTrangThai.Focus();
                return;
            }

            // Lấy số bàn từ tên bàn
            string tenBan = txtTenBan.Text.Trim();

            if (!tenBan.StartsWith("Bàn ", StringComparison.OrdinalIgnoreCase) ||
                !int.TryParse(tenBan.Substring(4).Trim(), out int soBan) ||
                soBan <= 0)
            {
                MessageBox.Show(
                    "Tên bàn phải có dạng Bàn 1, Bàn 2,...",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtTenBan.Focus();
                return;
            }

            // Tạo đối tượng bàn cần cập nhật
            Ban ban = new Ban
            {
                MaBan = maBan,
                SoBan = soBan,
                SoChoNgoi = (int)nudSoCho.Value,
                TrangThai = cboTrangThai.SelectedItem.ToString()
            };

            // Gọi Service cập nhật
            bool ketQua = banService.SuaBan(ban);

            if (ketQua)
            {
                MessageBox.Show(
                    "Cập nhật thông tin bàn thành công!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show(
                    "Cập nhật thông tin bàn thất bại!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}
