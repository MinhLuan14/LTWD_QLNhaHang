using QuanLyNhaHang.Models;
using QuanLyNhaHang.Services;
using System;
using System.Windows.Forms;

namespace QuanLyNhaHang.Forms
{
    public partial class frmThemBan : Form
    {
        public event Action BanDaThem;
        public frmThemBan()
        {
            InitializeComponent();
        }
        private void ClearForm()
        {
            txtBanSo.Clear();
            nudSoChoNgoi.Value = 1;
            cboTrangThai.SelectedIndex = 0;
            txtBanSo.Focus();
        }
        private void frmThemSuaBan_Load(object sender, EventArgs e)
        {
            // Khởi tạo danh sách trạng thái
            cboTrangThai.Items.Clear();
            cboTrangThai.Items.Add("Trống");
            cboTrangThai.Items.Add("Có khách");
            cboTrangThai.Items.Add("Đặt trước");

            cboTrangThai.SelectedIndex = 0;

            // Thiết lập số chỗ ngồi
            nudSoChoNgoi.Minimum = 1;
            nudSoChoNgoi.Maximum = 20;
            nudSoChoNgoi.Value = 1;
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtBanSo.Text))
            {
                MessageBox.Show(
                    "Vui lòng nhập số bàn!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtBanSo.Focus();
                return;
            }

            if (!int.TryParse(txtBanSo.Text.Trim(), out int soBan))
            {
                MessageBox.Show(
                    "Số bàn phải là số!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtBanSo.Focus();
                return;
            }

            Ban ban = new Ban
            {
                SoBan = soBan,
                SoChoNgoi = (int)nudSoChoNgoi.Value,
                TrangThai = cboTrangThai.Text
            };

            BanService service = new BanService();

            if (service.ThemBan(ban))
            {
                MessageBox.Show(
                    "Thêm bàn thành công!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                // Báo cho form cha load lại dữ liệu
                BanDaThem?.Invoke();

                // Xóa dữ liệu để nhập tiếp
                ClearForm();
            }
            else
            {
                MessageBox.Show(
                    "Thêm bàn thất bại!",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            // Làm mới dữ liệu
            txtBanSo.Clear();
            nudSoChoNgoi.Value = 1;
            cboTrangThai.SelectedIndex = 0;

            txtBanSo.Focus();
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Bạn có muốn thoát không?",
                "Xác nhận",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                this.Close();
            }
        }
    }
}