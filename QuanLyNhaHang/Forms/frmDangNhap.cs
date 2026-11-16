
using System;
using System.Data;
using System.Windows.Forms;
using QuanLyNhaHang.Services;

namespace QuanLyNhaHang
{
    public partial class frmDangNhap : Form
    {
        private readonly AuthService authService = new AuthService();

        public frmDangNhap()
        {
            InitializeComponent();

            btnDangNhap.Click += btnDangNhap_Click;
            btnTaoTaiKhoan.Click += btnTaoTaiKhoan_Click;

            // Sự kiện hiện/ẩn mật khẩu
            chkHienMatKhau.CheckedChanged += chkHienMatKhau_CheckedChanged;

            AcceptButton = btnDangNhap;

            txtTenDangNhap.CharacterCasing = CharacterCasing.Lower;

            // Mặc định ẩn mật khẩu
            chkHienMatKhau.Checked = false;
            txtMAtKhau.UseSystemPasswordChar = true;
        }

        // 1. ĐĂNG NHẬP
        private void btnDangNhap_Click(object sender, EventArgs e)
        {
            string username = txtTenDangNhap.Text.Trim();
            string password = txtMAtKhau.Text;

            // Kiểm tra dữ liệu đầu vào
            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show(
                    "Vui lòng nhập tên đăng nhập và mật khẩu.",
                    "Thiếu thông tin",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            try
            {
                // Xác thực tài khoản từ SQL Server
                DataRow account = authService.DangNhap(
                    username,
                    password
                );

                if (account == null)
                {
                    MessageBox.Show(
                        "Tên đăng nhập hoặc mật khẩu không đúng.",
                        "Đăng nhập thất bại",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    txtMAtKhau.Clear();
                    txtMAtKhau.Focus();
                    return;
                }

                // 2. LẤY MÃ NHÂN VIÊN
                if (!account.Table.Columns.Contains("maNhanVien") ||
                    account["maNhanVien"] == DBNull.Value)
                {
                    MessageBox.Show(
                        "Tài khoản chưa được liên kết với hồ sơ nhân viên.",
                        "Không tìm thấy nhân viên",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    return;
                }

                int maNhanVien = Convert.ToInt32(
                    account["maNhanVien"]
                );

                if (maNhanVien <= 0)
                {
                    MessageBox.Show(
                        "Mã nhân viên không hợp lệ.",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    return;
                }

                // 3. LẤY HỌ TÊN NHÂN VIÊN
                string hoVaTenLot =
                    account.Table.Columns.Contains("hoVaTenLot")
                    ? Convert.ToString(account["hoVaTenLot"])
                    : string.Empty;

                string tenNhanVien =
                    account.Table.Columns.Contains("tenNhanVien")
                    ? Convert.ToString(account["tenNhanVien"])
                    : string.Empty;

                string fullName =
                    (hoVaTenLot + " " + tenNhanVien).Trim();

                if (string.IsNullOrWhiteSpace(fullName))
                {
                    fullName = username;
                }

                // 4. LẤY VAI TRÒ TÀI KHOẢN
                string vaiTro = account.Table.Columns.Contains("vaiTro")
                    ? Convert.ToString(account["vaiTro"]).Trim()
                    : string.Empty;

                // Kiểm tra vai trò
                if (string.IsNullOrWhiteSpace(vaiTro))
                {
                    MessageBox.Show(
                        "Tài khoản chưa được thiết lập vai trò.",
                        "Lỗi phân quyền",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    return;
                }

                // 5. MỞ TRANG CHỦ VÀ TRUYỀN THÔNG TIN
                Hide();

                using (frmTrangChu home = new frmTrangChu(
                    fullName,
                    maNhanVien,
                    vaiTro))
                {
                    home.ShowDialog();
                }

                // 5. HIỂN THỊ LẠI FORM ĐĂNG NHẬP
                if (!IsDisposed)
                {
                    txtMAtKhau.Clear();

                    Show();
                    txtTenDangNhap.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể đăng nhập. Vui lòng kiểm tra kết nối cơ sở dữ liệu.\n\n" +
                    "Chi tiết: " + ex.Message,
                    "Lỗi đăng nhập",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // 6. MỞ FORM TẠO TÀI KHOẢN
        private void btnTaoTaiKhoan_Click(object sender, EventArgs e)
        {
            using (frmTaoTaiKhoan form = new frmTaoTaiKhoan())
            {
                form.ShowDialog(this);
            }
        }

        // 7. HIỆN / ẨN MẬT KHẨU
        private void chkHienMatKhau_CheckedChanged(object sender, EventArgs e)
        {
            if (chkHienMatKhau.Checked)
            {
                // Hiển thị mật khẩu
                txtMAtKhau.UseSystemPasswordChar = false;
                txtMAtKhau.PasswordChar = '\0';
            }
            else
            {
                // Ẩn mật khẩu
                txtMAtKhau.UseSystemPasswordChar = true;
            }

            txtMAtKhau.Focus();

            // Đưa con trỏ về cuối nội dung mật khẩu
            txtMAtKhau.SelectionStart = txtMAtKhau.Text.Length;
        }

        // 8. SỰ KIỆN LOGO
        private void picLogo_Click(object sender, EventArgs e)
        {
            // Xử lý sự kiện logo nếu cần
        }

        // 9. SỰ KIỆN PANEL
        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            // Xử lý sự kiện vẽ panel nếu cần
        }

        // 10. FORM LOAD
        private void frmDangNhap_Load(object sender, EventArgs e)
        {
        }
    }
}
