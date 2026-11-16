
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using QuanLyNhaHang.Models;
using QuanLyNhaHang.Services;

namespace QuanLyNhaHang
{
    public partial class frmQuanLyMonAn : Form
    {
        private readonly MonAnService monAnService =
            new MonAnService();

        private readonly LoaiMonService loaiMonService =
            new LoaiMonService();

        private List<MonAn> danhSachMonAn =
            new List<MonAn>();

        private int maMonAnDangChon = 0;
        private string duongDanHinh = "";

        public frmQuanLyMonAn()
        {
            InitializeComponent();
        }

        // 1. LOAD FORM
        private void frmQuanLyMonAn_Load(object sender, EventArgs e)
        {
      
            picHinhMon.SizeMode = PictureBoxSizeMode.Zoom;
            txtTimKiem.Clear();

            CauHinhDGV();
            LoadDanhMuc();
            LoadTrangThai();
            LoadDanhSachMonAn();

            LamMoi();
        }

        // 2. CẤU HÌNH DATAGRIDVIEW
        private void CauHinhDGV()
        {
            // Không tự động tạo cột từ thuộc tính của Model
            dgvMonAn.AutoGenerateColumns = false;

            dgvMonAn.ReadOnly = true;
            dgvMonAn.MultiSelect = false;
            dgvMonAn.AllowUserToAddRows = false;

            dgvMonAn.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            dgvMonAn.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            dgvMonAn.Columns["MaMonAn"].DataPropertyName = "MaMonAn";
            dgvMonAn.Columns["TenMonAn"].DataPropertyName = "TenMonAn";
            dgvMonAn.Columns["TenLoaiMon"].DataPropertyName = "TenLoaiMon";
            dgvMonAn.Columns["DonGia"].DataPropertyName = "DonGia";
            dgvMonAn.Columns["DonVi"].DataPropertyName = "DonVi";
            dgvMonAn.Columns["TrangThai"].DataPropertyName = "TrangThai";

            dgvMonAn.Columns["DonGia"].DefaultCellStyle.Format = "#,##0";
        
            dgvMonAn.CellFormatting -= dgvMonAn_CellFormatting;
            dgvMonAn.CellFormatting += dgvMonAn_CellFormatting;
        }

        // 3. LOAD DANH MỤC
        private void LoadDanhMuc()
        {
            try
            {
                List<LoaiMon> danhSach =
                    loaiMonService.GetAll();

                cboMaDanhMuc.DataSource = danhSach;
                cboMaDanhMuc.DisplayMember = "TenLoaiMon";
                cboMaDanhMuc.ValueMember = "MaLoaiMon";

                cboMaDanhMuc.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi tải danh mục: " + ex.Message);
            }
        }

        // 4. LOAD TRẠNG THÁI
        private void LoadTrangThai()
        {
            cboTrangThai.Items.Clear();
            cboTrangThai.Items.Add("Đang bán");
            cboTrangThai.Items.Add("Ngừng bán");
            cboTrangThai.SelectedIndex = 0;

            cboLocTrangThai.Items.Clear();
            cboLocTrangThai.Items.Add("Tất cả");
            cboLocTrangThai.Items.Add("Đang bán");
            cboLocTrangThai.Items.Add("Ngừng bán");
            cboLocTrangThai.SelectedIndex = 0;
        }

        // 5. LOAD DANH SÁCH MÓN ĂN
        private void LoadDanhSachMonAn()
        {
            try
            {
                danhSachMonAn = monAnService.GetAll();
                LocDanhSach();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi tải danh sách món ăn: " + ex.Message);
            }
        }

        // 6. HIỂN THỊ TRẠNG THÁI TRÊN DGV
        private void dgvMonAn_CellFormatting(
    object sender,
    DataGridViewCellFormattingEventArgs e)
        {
            // Bỏ qua tiêu đề cột
            if (e.RowIndex < 0)
                return;

            // Chỉ xử lý cột trạng thái
            if (dgvMonAn.Columns[e.ColumnIndex].Name == "TrangThai"
                && e.Value != null)
            {
                bool trangThai;

                if (e.Value is bool)
                {
                    trangThai = (bool)e.Value;
                }
                else if (e.Value.ToString() == "1")
                {
                    trangThai = true;
                }
                else if (e.Value.ToString() == "0")
                {
                    trangThai = false;
                }
                else if (!bool.TryParse(e.Value.ToString(), out trangThai))
                {
                    return;
                }

                e.Value = trangThai ? "Đang bán" : "Ngừng bán";
                e.FormattingApplied = true;
            }
        }

        // 7. NÚT THÊM
        private void btnThem_Click(object sender, EventArgs e)
        {
            LamMoi();
            txtTenMonAn.Focus();
        }

        // 8. NÚT LƯU
        private void btnLuu_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra tên món
                if (string.IsNullOrWhiteSpace(txtTenMonAn.Text))
                {
                    MessageBox.Show("Vui lòng nhập tên món ăn!");
                    txtTenMonAn.Focus();
                    return;
                }

                // Kiểm tra đơn giá
                decimal donGia;

                if (!decimal.TryParse(
                    txtDonGia.Text.Trim(), out donGia))
                {
                    MessageBox.Show("Đơn giá không hợp lệ!");
                    txtDonGia.Focus();
                    return;
                }

                if (donGia < 0)
                {
                    MessageBox.Show("Đơn giá không được nhỏ hơn 0!");
                    return;
                }

                // Kiểm tra danh mục
                if (cboMaDanhMuc.SelectedValue == null)
                {
                    MessageBox.Show("Vui lòng chọn danh mục!");
                    cboMaDanhMuc.Focus();
                    return;
                }

                if (cboTrangThai.SelectedIndex < 0)
                {
                    MessageBox.Show("Vui lòng chọn trạng thái!");
                    return;
                }
                duongDanHinh = LuuHinhVaoUngDung(duongDanHinh);
                // Khởi tạo đối tượng món ăn
                MonAn monAn = new MonAn
                {
                    MaMonAn = maMonAnDangChon,
                    TenMonAn = txtTenMonAn.Text.Trim(),
                    DonGia = donGia,
                    MaLoaiMon = Convert.ToInt32(
                        cboMaDanhMuc.SelectedValue),
                    DonVi = txtDonVi.Text.Trim(),
                    HinhMon = duongDanHinh,
                    TrangThai = cboTrangThai.SelectedIndex == 0
                };

                bool ketQua;

                // Thêm mới
                if (maMonAnDangChon == 0)
                {
                    ketQua = monAnService.Them(monAn);

                    if (ketQua)
                    {
                        MessageBox.Show(
                            "Thêm món ăn thành công!",
                            "Thông báo",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                }
                // Cập nhật
                else
                {
                    ketQua = monAnService.Sua(monAn);

                    if (ketQua)
                    {
                        MessageBox.Show(
                            "Cập nhật món ăn thành công!",
                            "Thông báo",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                }

                if (ketQua)
                {
                    LoadDanhSachMonAn();
                    LamMoi();
                }
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2601 || ex.Number == 2627)
                {
                    MessageBox.Show("Tên món ăn đã tồn tại!");
                }
                else
                {
                    MessageBox.Show("Lỗi SQL: " + ex.Message);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi: " + ex.Message,
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }
        // 9. CHỌN MÓN ĂN TRÊN DGV
        private void dgvMonAn_CellClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            MonAn monAn =
                dgvMonAn.Rows[e.RowIndex].DataBoundItem as MonAn;

            if (monAn == null)
                return;

            maMonAnDangChon = monAn.MaMonAn;

            txtTenMonAn.Text = monAn.TenMonAn;
            txtDonGia.Text = monAn.DonGia.ToString("0.##");
            txtDonVi.Text = monAn.DonVi ?? "";

            cboMaDanhMuc.SelectedValue = monAn.MaLoaiMon;

            cboTrangThai.SelectedIndex =
                monAn.TrangThai ? 0 : 1;

            duongDanHinh = monAn.HinhMon ?? "";

            HienThiHinhMon(duongDanHinh);
        }

        // 10. CHỌN HÌNH MÓN ĂN
        private void btnChonHinh_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFile = new OpenFileDialog())
            {
                openFile.Title = "Chọn hình món ăn";

                openFile.Filter =
                    "Hình ảnh|*.jpg;*.jpeg;*.png;*.bmp";

                if (openFile.ShowDialog() != DialogResult.OK)
                    return;

                try
                {
                    FileInfo fileInfo = new FileInfo(openFile.FileName);

                    if (fileInfo.Length > 10 * 1024 * 1024)
                    {
                        MessageBox.Show("Hình ảnh không được vượt quá 10MB!");
                        return;
                    }

                    // Sao chép ảnh vào thư mục HinhAnh của ứng dụng
                    duongDanHinh = LuuHinhVaoUngDung(openFile.FileName);

                    // Hiển thị ảnh vừa chọn
                    HienThiHinhMon(duongDanHinh);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Không thể lưu hình ảnh:\n" + ex.Message,
                        "Lỗi",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
        }
        private string LuuHinhVaoUngDung(string duongDanNguon)
        {
            if (string.IsNullOrWhiteSpace(duongDanNguon))
                return "";

            string tenFile = Path.GetFileName(duongDanNguon);

            string thuMucHinh = Path.Combine(
                Application.StartupPath,
                "HinhAnh"
            );

            // Tạo thư mục nếu chưa tồn tại
            if (!Directory.Exists(thuMucHinh))
            {
                Directory.CreateDirectory(thuMucHinh);
            }

            // Xác định đường dẫn nguồn
            string pathNguon = duongDanNguon;

            if (!Path.IsPathRooted(pathNguon))
            {
                pathNguon = Path.Combine(thuMucHinh, tenFile);
            }

            if (!File.Exists(pathNguon))
            {
                throw new FileNotFoundException(
                    "Không tìm thấy file hình ảnh!", pathNguon);
            }

            // Đường dẫn đích trong thư mục HinhAnh của ứng dụng
            string pathDich = Path.Combine(thuMucHinh, tenFile);

            // Tránh sao chép một file vào chính nó
            if (!string.Equals(
                Path.GetFullPath(pathNguon),
                Path.GetFullPath(pathDich),
                StringComparison.OrdinalIgnoreCase))
            {
                File.Copy(pathNguon, pathDich, true);
            }

            // Chỉ trả về tên ảnh để lưu SQL
            return tenFile;
        }
        // 11. HIỂN THỊ HÌNH
        private void HienThiHinhMon(string duongDan)
        {
            if (picHinhMon.Image != null)
            {
                Image anhCu = picHinhMon.Image;
                picHinhMon.Image = null;
                anhCu.Dispose();
            }

            picHinhMon.SizeMode = PictureBoxSizeMode.Zoom;

            if (string.IsNullOrWhiteSpace(duongDan))
                return;

            string path;

            if (Path.IsPathRooted(duongDan))
            {
                // Hỗ trợ đường dẫn cũ
                path = duongDan;
            }
            else
            {
                // Đọc ảnh từ thư mục HinhAnh
                path = Path.Combine(
                    Application.StartupPath,
                    "HinhAnh",
                    Path.GetFileName(duongDan)
                );
            }

            if (!File.Exists(path))
                return;

            using (Image anh = Image.FromFile(path))
            {
                picHinhMon.Image = new Bitmap(anh);
            }
        }

        // 12. NGỪNG BÁN
        private void btnNgungBan_Click(object sender, EventArgs e)
        {
            if (maMonAnDangChon <= 0)
            {
                MessageBox.Show(
                    "Vui lòng chọn món ăn cần ngừng bán!");
                return;
            }

            if (!danhSachMonAn.Any(m =>
                m.MaMonAn == maMonAnDangChon && m.TrangThai))
            {
                MessageBox.Show("Món ăn này đã ngừng bán!");
                return;
            }

            DialogResult result = MessageBox.Show(
                "Bạn có chắc chắn muốn ngừng bán món ăn này?",
                "Xác nhận",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
                return;

            try
            {
                bool ketQua =
                    monAnService.CapNhatTrangThai(
                        maMonAnDangChon, false);

                if (ketQua)
                {
                    MessageBox.Show("Đã ngừng bán món ăn!");

                    LoadDanhSachMonAn();
                    LamMoi();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        // 13. TÌM KIẾM VÀ LỌC
        private void LocDanhSach()
        {
            string tuKhoa =
                txtTimKiem.Text.Trim().ToLower();

            IEnumerable<MonAn> ketQua = danhSachMonAn;

            // Tìm theo tên món
            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                ketQua = ketQua.Where(m =>
                    m.TenMonAn.ToLower().Contains(tuKhoa));
            }

            // Lọc trạng thái
            if (cboLocTrangThai.SelectedIndex == 1)
            {
                ketQua = ketQua.Where(m => m.TrangThai);
            }
            else if (cboLocTrangThai.SelectedIndex == 2)
            {
                ketQua = ketQua.Where(m => !m.TrangThai);
            }

            dgvMonAn.DataSource = null;

            dgvMonAn.AutoGenerateColumns = false;

            dgvMonAn.DataSource = ketQua.ToList();
        }

        private void txtTimKiem_TextChanged(
            object sender, EventArgs e)
        {
            LocDanhSach();
        }

        private void cboLocTrangThai_SelectedIndexChanged(
            object sender, EventArgs e)
        {
            if (danhSachMonAn != null)
                LocDanhSach();
        }

        // 14. LÀM MỚI
        private void LamMoi()
        {
            maMonAnDangChon = 0;

            txtTenMonAn.Clear();
            txtDonGia.Clear();
            txtDonVi.Clear();

            cboMaDanhMuc.SelectedIndex = -1;
            cboTrangThai.SelectedIndex = 0;

            duongDanHinh = "";

            if (picHinhMon.Image != null)
            {
                Image anhCu = picHinhMon.Image;
                picHinhMon.Image = null;
                anhCu.Dispose();
            }

            dgvMonAn.ClearSelection();

            txtTenMonAn.Focus();
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            // Kiểm tra món ăn được chọn
            if (dgvMonAn.CurrentRow == null ||
                dgvMonAn.CurrentRow.DataBoundItem == null)
            {
                MessageBox.Show("Vui lòng chọn món ăn cần xóa!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            MonAn monAn = dgvMonAn.CurrentRow.DataBoundItem as MonAn;

            if (monAn == null)
                return;

            // Xác nhận xóa
            DialogResult ketQua = MessageBox.Show(
                "Bạn có chắc chắn muốn xóa món ăn: " + monAn.TenMonAn + "?",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (ketQua != DialogResult.Yes)
                return;

            try
            {
                monAnService.Xoa(monAn.MaMonAn);

                MessageBox.Show("Xóa món ăn thành công!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                LoadDanhSachMonAn();
                LamMoi();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể xóa món ăn!\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}
