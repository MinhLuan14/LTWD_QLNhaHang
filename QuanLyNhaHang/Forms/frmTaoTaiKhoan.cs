
using System;
using System.Linq;
using System.Windows.Forms;

using QuanLyNhaHang.Models;
using QuanLyNhaHang.Services;

namespace QuanLyNhaHang
{
    public partial class frmTaoTaiKhoan : Form
    {
        private readonly AuthService authService = new AuthService();

        public frmTaoTaiKhoan()
        {
            InitializeComponent();

            AcceptButton = btnTaoTaiKhoan;
        }

        private void btnTaoTaiKhoan_Click(object sender, EventArgs e)
        {
            string hoVaTen = txtHoVaTen.Text.Trim();
            string tenDangNhap = txtTenDangNhap.Text.Trim();
            string matKhau = txtMatKhau.Text;
            string xacNhanMatKhau = txtXacNhanMatKhau.Text;
            string soDienThoai = txtSoDienThoai.Text.Trim();
            string diaChi = txtDiaChi.Text.Trim();

            // 1. Kiểm tra dữ liệu bắt buộc
            if (string.IsNullOrWhiteSpace(hoVaTen) ||
                string.IsNullOrWhiteSpace(tenDangNhap) ||
                string.IsNullOrWhiteSpace(matKhau) ||
                string.IsNullOrWhiteSpace(xacNhanMatKhau) ||
                string.IsNullOrWhiteSpace(soDienThoai))
            {
                MessageBox.Show(
                    "Vui lòng nhập đầy đủ họ tên, tên đăng nhập, mật khẩu và số điện thoại.",
                    "Thiếu thông tin",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            // 2. Kiểm tra họ tên
            string[] tachHoTen = hoVaTen.Split(
                new char[] { ' ' },
                StringSplitOptions.RemoveEmptyEntries);

            if (tachHoTen.Length < 2)
            {
                MessageBox.Show(
                    "Vui lòng nhập đầy đủ họ và tên.",
                    "Họ tên không hợp lệ",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                txtHoVaTen.Focus();
                return;
            }

            // 3. Kiểm tra tên đăng nhập
            if (tenDangNhap.Length < 3 ||
                tenDangNhap.Length > 50 ||
                tenDangNhap.Any(char.IsWhiteSpace))
            {
                MessageBox.Show(
                    "Tên đăng nhập phải từ 3 đến 50 ký tự và không chứa khoảng trắng.",
                    "Tên đăng nhập không hợp lệ",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                txtTenDangNhap.Focus();
                return;
            }

            // 4. Kiểm tra mật khẩu
            if (matKhau.Length < 8)
            {
                MessageBox.Show(
                    "Mật khẩu phải có ít nhất 8 ký tự.",
                    "Mật khẩu không hợp lệ",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                txtMatKhau.Focus();
                return;
            }

            if (matKhau != xacNhanMatKhau)
            {
                MessageBox.Show(
                    "Mật khẩu xác nhận không khớp.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                txtXacNhanMatKhau.Focus();
                return;
            }

            // 5. Kiểm tra số điện thoại
            if (soDienThoai.Length != 10 ||
                !soDienThoai.All(char.IsDigit))
            {
                MessageBox.Show(
                    "Số điện thoại phải gồm đúng 10 chữ số.",
                    "Số điện thoại không hợp lệ",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                txtSoDienThoai.Focus();
                return;
            }

            // 6. Tách họ và tên
            string tenNhanVien = tachHoTen[tachHoTen.Length - 1];

            string hoVaTenLot = string.Join(
                " ",
                tachHoTen.Take(tachHoTen.Length - 1));

            // 7. Tạo đối tượng nhân viên
            NhanVien nhanVien = new NhanVien
            {
                HoVaTenLot = hoVaTenLot,
                TenNhanVien = tenNhanVien,
                SoDienThoai = soDienThoai,
                DiaChi = string.IsNullOrWhiteSpace(diaChi)
                    ? null
                    : diaChi
            };

            try
            {
                // 8. Gọi AuthService tạo tài khoản và nhân viên
                authService.DangKyNhanVien(
                    nhanVien,
                    tenDangNhap,
                    null,
                    matKhau);

                MessageBox.Show(
                    "Tạo tài khoản và thông tin nhân viên thành công!",
                    "Hoàn tất",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Không thể tạo tài khoản",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        private void btnQuayLaiDangNhap_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
