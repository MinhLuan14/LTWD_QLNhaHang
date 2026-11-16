
using Krypton.Toolkit;
using QuanLyNhaHang.Models;
using QuanLyNhaHang.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Runtime.InteropServices;
namespace QuanLyNhaHang.Forms
{
    public partial class frmNhanVien : Form
    {
        private readonly NhanVienService nhanVienService = new NhanVienService();

        private DataTable dtNhanVien = new DataTable();
        private int maNhanVienDangChon = 0;
        private int maTaiKhoanDangChon = 0;
        private string tenDangNhapDangChon = "";

        // Khởi tạo form và đăng ký các sự kiện.
        public frmNhanVien()
        {
            InitializeComponent();

            Load += frmNhanVien_Load;

            btnThem.Click += btnThem_Click;
            btnCapNhat.Click += btnCapNhat_Click;
            btnXoa.Click += btnXoa_Click;
            btnLamMoi.Click += btnLamMoi_Click;

            kryptonDataGridView2.CellClick += kryptonDataGridView2_CellClick;

            txtTimKiem.TextChanged += txtTimKiem_TextChanged;
            cboLocVaiTro.SelectedIndexChanged += cboLocVaiTro_SelectedIndexChanged;
            cboLocTrangThai.SelectedIndexChanged += cboLocTrangThai_SelectedIndexChanged;
        }

        // Khởi tạo dữ liệu và giao diện khi form được mở.
        private void frmNhanVien_Load(object sender, EventArgs e)
        {
            try
            {
                CauHinhGiaoDien();
                CauHinhTrangThai();
                CauHinhBoLoc();
                CauHinhLuoi();

                nhanVienService.KiemTraCotTrangThai();
                TaoPlaceholder();
                NapDanhSachNhanVien();
                NapDanhSachVaiTro();
                LamMoi();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể tải danh sách nhân viên!\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // Cấu hình giao diện các ô chọn.
        // Cấu hình giao diện các ô chọn và bảng nhân viên.
        private void CauHinhGiaoDien()
        {
            // Cấu hình ComboBox trạng thái.
            cboTrangThai.DropDownStyle = ComboBoxStyle.DropDownList;
            cboTrangThai.Font = new Font("Segoe UI", 12);

            // Cấu hình hai ComboBox lọc.
            cboLocVaiTro.DropDownStyle = ComboBoxStyle.DropDownList;
            cboLocTrangThai.DropDownStyle = ComboBoxStyle.DropDownList;
            cboLocVaiTro.Font = new Font("Segoe UI", 10);
            cboLocTrangThai.Font = new Font("Segoe UI", 10);

            // Cấu hình màu nền tổng thể của bảng.
            kryptonDataGridView2.BackgroundColor = Color.White;
            kryptonDataGridView2.GridColor = Color.FromArgb(210, 225, 235);
            kryptonDataGridView2.BorderStyle = BorderStyle.None;

            // Cấu hình tiêu đề cột.
            kryptonDataGridView2.EnableHeadersVisualStyles = false;
            kryptonDataGridView2.ColumnHeadersDefaultCellStyle.BackColor =
                Color.FromArgb(225, 244, 252);
            kryptonDataGridView2.ColumnHeadersDefaultCellStyle.ForeColor =
                Color.FromArgb(0, 65, 110);
            kryptonDataGridView2.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 10, FontStyle.Bold);
            kryptonDataGridView2.ColumnHeadersDefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleCenter;

            // Cấu hình màu chữ và nền các dòng thông thường.
            kryptonDataGridView2.DefaultCellStyle.BackColor = Color.White;
            kryptonDataGridView2.DefaultCellStyle.ForeColor =
                Color.FromArgb(45, 55, 65);
            kryptonDataGridView2.DefaultCellStyle.Font =
                new Font("Segoe UI", 10);
            kryptonDataGridView2.DefaultCellStyle.SelectionBackColor =
                Color.FromArgb(126, 211, 245);
            kryptonDataGridView2.DefaultCellStyle.SelectionForeColor =
                Color.FromArgb(0, 65, 110);

            // Tạo màu xen kẽ cho các dòng để dễ quan sát.
            kryptonDataGridView2.AlternatingRowsDefaultCellStyle.BackColor =
                Color.FromArgb(240, 249, 253);
            kryptonDataGridView2.AlternatingRowsDefaultCellStyle.ForeColor =
                Color.FromArgb(45, 55, 65);

            // Căn giữa nội dung các ô.
            kryptonDataGridView2.DefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleLeft;

            // Thiết lập chiều cao dòng.
            kryptonDataGridView2.RowTemplate.Height = 38;

            // Không cho phép người dùng tự tạo thêm dòng.
            kryptonDataGridView2.AllowUserToAddRows = false;
        }

        // Nạp trạng thái dùng cho thao tác thêm và cập nhật.
        private void CauHinhTrangThai()
        {
            cboTrangThai.Items.Clear();

            cboTrangThai.Items.Add("Đang làm");
            cboTrangThai.Items.Add("Nghỉ làm");

            cboTrangThai.SelectedIndex = 0;
        }

        // Khởi tạo hai bộ lọc với lựa chọn mặc định.
        private void CauHinhBoLoc()
        {
            cboLocVaiTro.Items.Clear();
            cboLocVaiTro.Items.Add("Tất cả vai trò");
            cboLocVaiTro.SelectedIndex = 0;

            cboLocTrangThai.Items.Clear();
            cboLocTrangThai.Items.Add("Tất cả trạng thái");
            cboLocTrangThai.Items.Add("Đang làm");
            cboLocTrangThai.Items.Add("Nghỉ làm");
            cboLocTrangThai.SelectedIndex = 0;
        }

        // Nạp các vai trò đang tồn tại trong danh sách nhân viên.
        private void NapDanhSachVaiTro()
        {
            string vaiTroDangChon = cboLocVaiTro.Text;

            cboLocVaiTro.Items.Clear();
            cboLocVaiTro.Items.Add("Tất cả vai trò");

            if (dtNhanVien.Columns.Contains("VaiTro"))
            {
                var danhSachVaiTro = dtNhanVien.AsEnumerable()
                    .Where(r => !r.IsNull("VaiTro"))
                    .Select(r => r["VaiTro"].ToString().Trim())
                    .Where(v => !string.IsNullOrWhiteSpace(v))
                    .Distinct()
                    .OrderBy(v => v)
                    .ToList();

                foreach (string vaiTro in danhSachVaiTro)
                {
                    cboLocVaiTro.Items.Add(vaiTro);
                }
            }

            int viTri = cboLocVaiTro.Items.IndexOf(vaiTroDangChon);
            cboLocVaiTro.SelectedIndex = viTri >= 0 ? viTri : 0;
        }

        // Cấu hình các cột của bảng nhân viên.
        private void CauHinhLuoi()
        {
            kryptonDataGridView2.AutoGenerateColumns = false;
            kryptonDataGridView2.ReadOnly = true;
            kryptonDataGridView2.AllowUserToAddRows = false;
            kryptonDataGridView2.AllowUserToDeleteRows = false;
            kryptonDataGridView2.MultiSelect = false;
            kryptonDataGridView2.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            cloMaNV.DataPropertyName = "MaNV";
            cloHoTen.DataPropertyName = "HoTen";
            cloDienThoai.DataPropertyName = "DienThoai";
            cloVaiTro.DataPropertyName = "VaiTro";
            cloTrangThai.DataPropertyName = "TrangThai";

            kryptonDataGridView2.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;
        }

        // Lấy danh sách nhân viên và hiển thị lên DataGridView.
        private void NapDanhSachNhanVien()
        {
            dtNhanVien = nhanVienService.LayDanhSach();

            if (dtNhanVien == null)
                dtNhanVien = new DataTable();

            // Xóa bộ lọc cũ trước khi gán dữ liệu.
            dtNhanVien.DefaultView.RowFilter = "";

            // Gán trực tiếp DefaultView để DataGridView nhận dữ liệu.
            kryptonDataGridView2.DataSource = null;
            kryptonDataGridView2.DataSource = dtNhanVien.DefaultView;

            // Áp dụng lại các điều kiện lọc.
            ApDungBoLoc();
        }

        // Áp dụng đồng thời bộ lọc vai trò, trạng thái và từ khóa.
        // Lọc nhân viên theo vai trò, trạng thái và nội dung tìm kiếm.
        private void ApDungBoLoc()
        {
            if (dtNhanVien == null || dtNhanVien.Columns.Count == 0)
                return;

            List<string> dieuKien = new List<string>();

            string vaiTro = cboLocVaiTro.Text.Trim();
            string trangThai = cboLocTrangThai.Text.Trim();
            string tuKhoa = txtTimKiem.Text.Trim();

            // Lọc theo vai trò.
            if (!string.IsNullOrWhiteSpace(vaiTro) &&
                vaiTro != "Tất cả vai trò" &&
                dtNhanVien.Columns.Contains("VaiTro"))
            {
                dieuKien.Add("[VaiTro] = '" + vaiTro.Replace("'", "''") + "'");
            }

            // Lọc theo trạng thái.
            if (!string.IsNullOrWhiteSpace(trangThai) &&
                trangThai != "Tất cả trạng thái" &&
                dtNhanVien.Columns.Contains("TrangThai"))
            {
                dieuKien.Add("[TrangThai] = '" + trangThai.Replace("'", "''") + "'");
            }

            // Không tìm kiếm nếu TextBox đang chứa nội dung placeholder.
            if (!string.IsNullOrWhiteSpace(tuKhoa) &&
                tuKhoa != "Tìm nhân viên...")
            {
                tuKhoa = tuKhoa.Replace("'", "''")
                               .Replace("[", "[[]")
                               .Replace("%", "[%]")
                               .Replace("*", "[*]");

                List<string> dieuKienTimKiem = new List<string>();

                // Tìm theo mã nhân viên.
                if (dtNhanVien.Columns.Contains("MaNV"))
                    dieuKienTimKiem.Add("Convert([MaNV], 'System.String') LIKE '%" + tuKhoa + "%'");

                // Tìm theo họ tên.
                if (dtNhanVien.Columns.Contains("HoTen"))
                    dieuKienTimKiem.Add("[HoTen] LIKE '%" + tuKhoa + "%'");

                // Tìm theo số điện thoại.
                if (dtNhanVien.Columns.Contains("DienThoai"))
                    dieuKienTimKiem.Add("[DienThoai] LIKE '%" + tuKhoa + "%'");

                // Tìm theo tên đăng nhập.
                if (dtNhanVien.Columns.Contains("TenDangNhap"))
                    dieuKienTimKiem.Add("[TenDangNhap] LIKE '%" + tuKhoa + "%'");

                if (dieuKienTimKiem.Count > 0)
                    dieuKien.Add("(" + string.Join(" OR ", dieuKienTimKiem) + ")");
            }

            // Áp dụng toàn bộ điều kiện lọc.
            dtNhanVien.DefaultView.RowFilter = string.Join(" AND ", dieuKien);

            // Cập nhật lại giao diện bảng.
            kryptonDataGridView2.Refresh();
        }

        // Xử lý khi thay đổi bộ lọc vai trò.
        private void cboLocVaiTro_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApDungBoLoc();
        }

        // Xử lý khi thay đổi bộ lọc trạng thái.
        private void cboLocTrangThai_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApDungBoLoc();
        }

        // Xử lý khi nhập nội dung tìm kiếm.
        private void txtTimKiem_TextChanged(object sender, EventArgs e)
        {
            ApDungBoLoc();
        }

        // Hiển thị thông tin nhân viên khi chọn một dòng trong bảng.
        private void kryptonDataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            DataGridViewRow dong = kryptonDataGridView2.Rows[e.RowIndex];

            if (dong.DataBoundItem == null)
                return;

            DataRowView duLieu = dong.DataBoundItem as DataRowView;

            if (duLieu == null)
                return;

            maNhanVienDangChon = LayGiaTriInt(duLieu, "MaNV");
            maTaiKhoanDangChon = LayGiaTriInt(duLieu, "MaTaiKhoan");

            tenDangNhapDangChon = LayGiaTriChuoi(duLieu, "TenDangNhap");

            txtHoTen.Text = LayGiaTriChuoi(duLieu, "HoTen");
            txtSoDienThoai.Text = LayGiaTriChuoi(duLieu, "DienThoai");
            txtTenDangNhap.Text = tenDangNhapDangChon;
            txtVaiTro.Text = LayGiaTriChuoi(duLieu, "VaiTro");

            string trangThai = LayGiaTriChuoi(duLieu, "TrangThai");

            if (cboTrangThai.Items.Contains(trangThai))
                cboTrangThai.SelectedItem = trangThai;
            else
                cboTrangThai.SelectedIndex = 0;

            btnCapNhat.Enabled = true;
            btnXoa.Enabled = true;
            btnThem.Enabled = true;
        }

        // Lấy giá trị số nguyên an toàn từ DataRowView.
        private int LayGiaTriInt(DataRowView dong, string tenCot)
        {
            if (dong == null || !dong.DataView.Table.Columns.Contains(tenCot))
                return 0;

            if (dong[tenCot] == DBNull.Value)
                return 0;

            int.TryParse(dong[tenCot].ToString(), out int ketQua);
            return ketQua;
        }

        // Lấy giá trị chuỗi an toàn từ DataRowView.
        private string LayGiaTriChuoi(DataRowView dong, string tenCot)
        {
            if (dong == null || !dong.DataView.Table.Columns.Contains(tenCot))
                return "";

            return dong[tenCot] == DBNull.Value
                ? ""
                : dong[tenCot].ToString();
        }

        // Kiểm tra dữ liệu nhân viên trước khi thêm hoặc cập nhật.
        private bool KiemTraThongTinCoBan()
        {
            if (string.IsNullOrWhiteSpace(txtHoTen.Text))
            {
                MessageBox.Show("Vui lòng nhập họ và tên nhân viên!");
                txtHoTen.Focus();
                return false;
            }

            if (txtHoTen.Text.Trim().Length < 2)
            {
                MessageBox.Show("Họ tên nhân viên phải có ít nhất 2 ký tự!");
                txtHoTen.Focus();
                return false;
            }

            string soDienThoai = txtSoDienThoai.Text.Trim();

            if (soDienThoai.Length != 10 ||
                !soDienThoai.All(char.IsDigit))
            {
                MessageBox.Show("Số điện thoại phải gồm đúng 10 chữ số!");
                txtSoDienThoai.Focus();
                return false;
            }

            if (cboTrangThai.SelectedIndex < 0)
            {
                MessageBox.Show("Vui lòng chọn trạng thái nhân viên!");
                cboTrangThai.Focus();
                return false;
            }

            return true;
        }

        // Tạo đối tượng nhân viên từ dữ liệu trên giao diện.
        private NhanVien TaoDuLieuNhanVien()
        {
            return nhanVienService.TaoNhanVien(
               txtHoTen.Text.Trim(),
               txtSoDienThoai.Text.Trim(),
               txtTenDangNhap.Text.Trim()
           );
        }

        // Thêm nhân viên mới không tạo tài khoản đăng nhập.
        // Thêm nhân viên phục vụ mới không tạo tài khoản đăng nhập.
        private void btnThem_Click(object sender, EventArgs e)
        {
            if (!KiemTraThongTinCoBan())
                return;

            try
            {
                // Tạo thông tin nhân viên mới.
                NhanVien nhanVien = TaoDuLieuNhanVien();

                // Lưu nhân viên vào cơ sở dữ liệu.
                nhanVienService.ThemNhanVien(nhanVien);

                MessageBox.Show(
                    "Thêm nhân viên phục vụ thành công!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                // Tải lại danh sách nhân viên.
                NapDanhSachNhanVien();
                NapDanhSachVaiTro();

                // Đưa form về trạng thái ban đầu.
                LamMoi();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Thêm nhân viên thất bại!\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
        // Cập nhật thông tin nhân viên đang được chọn.
        private void btnCapNhat_Click(object sender, EventArgs e)
        {
            if (maNhanVienDangChon <= 0)
            {
                MessageBox.Show("Vui lòng chọn nhân viên cần cập nhật!");
                return;
            }

            if (!KiemTraThongTinCoBan())
                return;

            try
            {
                // Tạo thông tin nhân viên từ giao diện.
                NhanVien nhanVien = TaoDuLieuNhanVien();

                // Cập nhật hồ sơ nhưng giữ nguyên liên kết tài khoản.
                nhanVienService.CapNhatNhanVien(
                    maNhanVienDangChon,
                    maTaiKhoanDangChon,
                    nhanVien,
                    null,
                    "Nhân viên phục vụ",
                    cboTrangThai.Text.Trim()
                );

                MessageBox.Show(
                    "Cập nhật thông tin nhân viên thành công!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                // Tải lại danh sách nhân viên.
                NapDanhSachNhanVien();
                NapDanhSachVaiTro();
                LamMoi();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Cập nhật nhân viên thất bại!\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // Ngừng làm việc thay vì xóa hoàn toàn nhân viên khỏi cơ sở dữ liệu.
        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (maNhanVienDangChon <= 0)
            {
                MessageBox.Show("Vui lòng chọn nhân viên cần ngừng làm việc!");
                return;
            }

            DialogResult ketQua = MessageBox.Show(
                "Bạn có chắc chắn muốn ngừng làm việc nhân viên này không?",
                "Xác nhận",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (ketQua != DialogResult.Yes)
                return;

            try
            {
                nhanVienService.NgungLamNhanVien(maNhanVienDangChon);

                MessageBox.Show(
                    "Đã cập nhật trạng thái nhân viên thành Nghỉ làm!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                NapDanhSachNhanVien();
                NapDanhSachVaiTro();
                LamMoi();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể cập nhật trạng thái nhân viên!\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // Xóa dữ liệu nhập và bỏ chọn nhân viên hiện tại.
        private void LamMoi()
        {
            maNhanVienDangChon = 0;
            maTaiKhoanDangChon = 0;
            tenDangNhapDangChon = "";

            txtHoTen.Clear();
            txtSoDienThoai.Clear();

            // Mặc định chỉ thêm nhân viên phục vụ.
            txtVaiTro.Text = "Nhân viên phục vụ";

            // Mặc định trạng thái đang làm.
            if (cboTrangThai.Items.Count > 0)
                cboTrangThai.SelectedItem = "Đang làm";

            // Bỏ chọn dòng trong bảng.
            kryptonDataGridView2.ClearSelection();

            // Chỉ cho phép thêm mới khi chưa chọn nhân viên.
            btnThem.Enabled = true;
            btnCapNhat.Enabled = false;
            btnXoa.Enabled = false;
        }

        // Làm mới danh sách và đưa hai bộ lọc về trạng thái ban đầu.
        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            txtTimKiem.Clear();

            if (cboLocVaiTro.Items.Count > 0)
                cboLocVaiTro.SelectedIndex = 0;

            if (cboLocTrangThai.Items.Count > 0)
                cboLocTrangThai.SelectedIndex = 0;

            try
            {
                NapDanhSachNhanVien();
                NapDanhSachVaiTro();
                LamMoi();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Làm mới danh sách thất bại!\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
        // Khai báo Windows API để thiết lập placeholder cho TextBox.
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr SendMessage(
            IntPtr hWnd,
            int msg,
            IntPtr wParam,
            string lParam
        );

        // Thiết lập placeholder cho ô tìm kiếm.
        private void TaoPlaceholder()
        {
            if (!txtTimKiem.IsHandleCreated)
                txtTimKiem.CreateControl();

            SendMessage(
                txtTimKiem.Handle,
                0x1501,
                (IntPtr)1,
                "Nhập mã nhân viên, họ tên, số điện thoại..."
            );
        }
    }
}
