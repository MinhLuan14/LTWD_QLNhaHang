
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;
using QuanLyNhaHang.Models;
using QuanLyNhaHang.Services;
using System.Drawing;
using System.Drawing.Printing;
namespace QuanLyNhaHang
{
    public partial class frmHoaDon : Form
    {
        private readonly HoaDonService hoaDonService =
            new HoaDonService();

        private HoaDon hoaDonDangChon = null;

        private bool dangCapNhatTien = false;
        private bool dangTaiDanhSach = false;
        private bool daKhoiTao = false;

        private readonly CultureInfo vanHoa =
            CultureInfo.GetCultureInfo("vi-VN");

        private string tenNhanVien;
        // Dữ liệu phục vụ in hóa đơn
        private PrintDocument printDocument;
        private HoaDon hoaDonIn;
        private List<ChiTietHoaDon> danhSachChiTietIn;
        private int viTriDongIn;

        private decimal tamTinhIn;
        private decimal giamGiaIn;
        private decimal tienThueIn;
        private decimal tongTienIn;
        public frmHoaDon(string tenNhanVien)
        {
            InitializeComponent();

            this.tenNhanVien = tenNhanVien;

            txtTamTinh.TextChanged += (s, e) => CapNhatTongTien();
            txtGiamGia.TextChanged += (s, e) => CapNhatTongTien();

            dgvHoaDon.SelectionChanged += dgvHoaDon_SelectionChanged;

            txtTimMaHoaDon.TextChanged -= txtTimMaHoaDon_TextChanged;
            txtTimMaHoaDon.TextChanged += txtTimMaHoaDon_TextChanged;

            txtTimMaHoaDon.KeyPress -= txtTimMaHoaDon_KeyPress;
            txtTimMaHoaDon.KeyPress += txtTimMaHoaDon_KeyPress;

            cboLocTrangThai.SelectedIndexChanged -= cboLocTrangThai_SelectedIndexChanged;
            cboLocTrangThai.SelectedIndexChanged += cboLocTrangThai_SelectedIndexChanged;
            // Tự động lọc hóa đơn khi thay đổi ngày bắt đầu hoặc ngày kết thúc.
            kdtpTuNgay.ValueChanged -= kdtpNgay_ValueChanged;
            kdtpTuNgay.ValueChanged += kdtpNgay_ValueChanged;

            kdtpDenNgay.ValueChanged -= kdtpNgay_ValueChanged;
            kdtpDenNgay.ValueChanged += kdtpNgay_ValueChanged;
            txtThue.ReadOnly = true;
        }

        // 1. KHỞI TẠO FORM

        private void frmHoaDon_Load(object sender, EventArgs e)
        {
            try
            {
                KhoiTaoBoLoc();
                CauHinhDataGridView();

                kdtpTuNgay.Value = DateTime.Today.AddMonths(-1);
                kdtpDenNgay.Value = DateTime.Today;

                daKhoiTao = true;

                txtGiamGia.Text = "0";

                TaiDanhSachHoaDon();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể khởi tạo danh sách hóa đơn.\n\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void KhoiTaoBoLoc()
        {
            cboLocTrangThai.Items.Clear();

            cboLocTrangThai.Items.Add("Tất cả");
            cboLocTrangThai.Items.Add("Chưa thanh toán");
            cboLocTrangThai.Items.Add("Đang xử lý");
            cboLocTrangThai.Items.Add("Đã thanh toán");

            cboLocTrangThai.SelectedIndex = 0;
        }

        private void CauHinhDataGridView()
        {
            dgvHoaDon.AutoGenerateColumns = false;
            dgvHoaDon.ReadOnly = true;
            dgvHoaDon.AllowUserToAddRows = false;
            dgvHoaDon.AllowUserToDeleteRows = false;
            dgvHoaDon.MultiSelect = false;
            dgvHoaDon.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;
            dgvHoaDon.RowHeadersVisible = false;

            dgvHoaDon.Columns["colMaHD"].ReadOnly = true;
            dgvHoaDon.Columns["colBan"].ReadOnly = true;
            dgvHoaDon.Columns["colNgay"].ReadOnly = true;
            dgvHoaDon.Columns["colTongTien"].ReadOnly = true;
            dgvHoaDon.Columns["colTrangThai"].ReadOnly = true;

            dgvChiTietHoaDon.AutoGenerateColumns = false;
            dgvChiTietHoaDon.ReadOnly = true;
            dgvChiTietHoaDon.AllowUserToAddRows = false;
            dgvChiTietHoaDon.AllowUserToDeleteRows = false;
            dgvChiTietHoaDon.MultiSelect = false;
            dgvChiTietHoaDon.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;
            dgvChiTietHoaDon.RowHeadersVisible = false;
            // Font chữ nội dung bảng: không in đậm
            dgvHoaDon.DefaultCellStyle.Font =
                new Font("Segoe UI", 9F, FontStyle.Regular);

            dgvHoaDon.RowsDefaultCellStyle.Font =
                new Font("Segoe UI", 9F, FontStyle.Regular);

            // Font chữ tiêu đề cột
            dgvHoaDon.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 9F, FontStyle.Bold);

            // Bảng chi tiết hóa đơn
            dgvChiTietHoaDon.DefaultCellStyle.Font =
                new Font("Segoe UI", 9F, FontStyle.Regular);

            dgvChiTietHoaDon.RowsDefaultCellStyle.Font =
                new Font("Segoe UI", 9F, FontStyle.Regular);

            dgvChiTietHoaDon.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 9F, FontStyle.Bold);
        }

        // 2. TẢI DANH SÁCH HÓA ĐƠN

        private void TaiDanhSachHoaDon()
        {
            if (dangTaiDanhSach)
                return;

            dangTaiDanhSach = true;

            try
            {
                DateTime tuNgay = kdtpTuNgay.Value.Date;
                DateTime denNgay = kdtpDenNgay.Value.Date;

                if (tuNgay > denNgay)
                {
                    MessageBox.Show(
                        "Ngày bắt đầu không được lớn hơn ngày kết thúc.",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    kdtpTuNgay.Focus();
                    return;
                }

                string trangThai =
                    cboLocTrangThai.SelectedItem == null
                        ? "Tất cả"
                        : cboLocTrangThai.SelectedItem.ToString();

                string keyword = txtTimMaHoaDon.Text.Trim();

                List<HoaDon> danhSach =
                    hoaDonService.GetDanhSachHoaDon(
                        tuNgay,
                        denNgay,
                        trangThai,
                        keyword
                    );

                dgvHoaDon.Rows.Clear();

                foreach (HoaDon hd in danhSach)
                {
                    string maHD = "HD" + hd.MaHoaDon.ToString("D4");

                    string ban = hd.SoBan.HasValue
                        ? "Bàn " + hd.SoBan.Value
                        : "Không xác định";

                    string ngayLap = hd.NgayLap.ToString("dd/MM/yyyy");

                    string tongTien =
                        hd.TongTien.ToString("N0", vanHoa) + " đ";

                    int index = dgvHoaDon.Rows.Add(
                        maHD,
                        ban,
                        ngayLap,
                        tongTien,
                        hd.TrangThai
                    );

                    dgvHoaDon.Rows[index].Tag = hd;
                }

                dgvHoaDon.ClearSelection();
                dgvHoaDon.CurrentCell = null;

                XoaChiTietHoaDon();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể tải danh sách hóa đơn.\n\n" + ex.Message,
                    "Lỗi tải dữ liệu",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                dangTaiDanhSach = false;
            }
        }

        // 3. CHỌN HÓA ĐƠN VÀ HIỂN THỊ CHI TIẾT

        private void dgvHoaDon_SelectionChanged(
            object sender,
            EventArgs e)
        {
            if (dangTaiDanhSach)
                return;

            if (dgvHoaDon.CurrentRow == null)
                return;

            HoaDon hd = dgvHoaDon.CurrentRow.Tag as HoaDon;

            if (hd == null)
                return;

            if (hoaDonDangChon != null &&
                hoaDonDangChon.MaHoaDon == hd.MaHoaDon)
            {
                return;
            }

            hoaDonDangChon = hd;

            TaiChiTietHoaDon(hd);
        }

        private void TaiChiTietHoaDon(HoaDon hd)
        {
            try
            {
                if (hd == null)
                    return;

                List<ChiTietHoaDon> danhSach =
                    hoaDonService.GetChiTietHoaDon(hd.MaHoaDon);

                dgvChiTietHoaDon.Rows.Clear();

                decimal tamTinh = 0;

                foreach (ChiTietHoaDon ct in danhSach)
                {
                    decimal thanhTien = ct.SoLuong * ct.DonGia;

                    tamTinh += thanhTien;

                    dgvChiTietHoaDon.Rows.Add(
                        ct.TenMonAn,
                        ct.SoLuong,
                        ct.DonGia.ToString("N0", vanHoa) + " đ",
                        thanhTien.ToString("N0", vanHoa) + " đ"
                    );
                }

                string maHoaDon = "HD" + hd.MaHoaDon.ToString("D4");

                lblMaChiTietHoaDon.Text =
                    "Chi tiết hóa đơn: " + maHoaDon;

                lblBanSo.Text = hd.SoBan.HasValue
                    ? "Bàn số: " + hd.SoBan.Value
                    : "Bàn số: --";

                lblNgayTao.Text =
                    "Ngày tạo: " +
                    hd.NgayLap.ToString("dd/MM/yyyy") +
                    " " + hd.GioLap.ToString(@"hh\:mm");

                lblNhanVien.Text =
                    string.IsNullOrWhiteSpace(hd.TenNhanVien)
                        ? "Nhân viên: --"
                        : "Nhân viên: " + hd.TenNhanVien;

                lblTrangThai.Text = hd.TrangThai;

                // Nạp dữ liệu tiền.
                txtTamTinh.Text = tamTinh.ToString("N0", vanHoa) + " đ";
                txtGiamGia.Text = hd.GiamGia.ToString("N0", vanHoa);
                txtThue.Text = hd.TienThue.ToString("N0", vanHoa) + " đ";

                bool chuaThanhToan =
                    string.Equals(
                        hd.TrangThai?.Trim(),
                        "Chưa thanh toán",
                        StringComparison.OrdinalIgnoreCase
                    );

                // Tính lại tổng tiền sau khi đã nạp đầy đủ dữ liệu.
                CapNhatTongTien();

                bool coChiTiet = danhSach.Count > 0;

                // Cập nhật nút theo trạng thái hóa đơn.
                if (string.Equals(
                    hd.TrangThai?.Trim(),
                    "Đã thanh toán",
                    StringComparison.OrdinalIgnoreCase))
                {
                    btnThanhToan.Text = "Đã thanh toán";
                    btnThanhToan.Enabled = false;
                }
                else if (string.Equals(
                    hd.TrangThai?.Trim(),
                    "Đang xử lý",
                    StringComparison.OrdinalIgnoreCase))
                {
                    btnThanhToan.Text = "Đang xử lý";
                    btnThanhToan.Enabled = false;
                }
                else if (!coChiTiet)
                {
                    btnThanhToan.Text = "Thanh toán";
                    btnThanhToan.Enabled = false;
                }
                else
                {
                    btnThanhToan.Text = "Thanh toán";
                    btnThanhToan.Enabled = true;
                }

                txtGiamGia.ReadOnly = !chuaThanhToan;
                txtThue.ReadOnly = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể tải chi tiết hóa đơn.\n\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // 4. XÓA THÔNG TIN CHI TIẾT

        private void XoaChiTietHoaDon()
        {
            hoaDonDangChon = null;

            dgvChiTietHoaDon.Rows.Clear();

            lblMaChiTietHoaDon.Text = "Chi tiết hóa đơn";
            lblBanSo.Text = "Bàn số: --";
            lblNgayTao.Text = "Ngày tạo: --";
            lblNhanVien.Text = "Nhân viên: --";
            lblTrangThai.Text = "--";

            txtTamTinh.Text = "0 đ";
            txtGiamGia.Text = "0";
            txtThue.Text = "0 đ";

            lblTongTien.Text = "0 đ";

            btnThanhToan.Text = "Thanh toán";
            btnThanhToan.Enabled = false;

            txtGiamGia.ReadOnly = true;
            txtThue.ReadOnly = true;
        }

        // 5. TÌM KIẾM HÓA ĐƠN

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            TaiDanhSachHoaDon();
        }

        // 6. LÀM MỚI

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            txtTimMaHoaDon.Clear();

            kdtpTuNgay.Value = DateTime.Today.AddMonths(-1);
            kdtpDenNgay.Value = DateTime.Today;

            cboLocTrangThai.SelectedIndex = 0;

            TaiDanhSachHoaDon();
        }

        // 7. KIỂM TRA GIẢM GIÁ

        private bool KiemTraGiamGia(out decimal giamGia)
        {
            giamGia = 0;

            if (hoaDonDangChon == null)
            {
                MessageBox.Show(
                    "Vui lòng chọn hóa đơn cần thanh toán.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return false;
            }

            if (!string.Equals(
                hoaDonDangChon.TrangThai?.Trim(),
                "Chưa thanh toán",
                StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show(
                    "Hóa đơn này đã được thanh toán.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return false;
            }

            string giaTri = txtGiamGia.Text.Trim();

            if (string.IsNullOrWhiteSpace(giaTri) ||
                !decimal.TryParse(
                    giaTri,
                    NumberStyles.Number,
                    vanHoa,
                    out giamGia))
            {
                MessageBox.Show(
                    "Giảm giá phải là số hợp lệ.\nVí dụ: 0 hoặc 20000.",
                    "Dữ liệu không hợp lệ",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                txtGiamGia.Focus();
                txtGiamGia.SelectAll();
                return false;
            }

            if (giamGia < 0)
            {
                MessageBox.Show(
                    "Giảm giá không được nhỏ hơn 0.",
                    "Dữ liệu không hợp lệ",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                txtGiamGia.Focus();
                txtGiamGia.SelectAll();
                return false;
            }

            // Kiểm tra giảm giá theo tạm tính thực tế từ chi tiết món ăn.
            decimal tamTinh = ChuyenSangSo(txtTamTinh.Text);

            if (giamGia > tamTinh)
            {
                MessageBox.Show(
                    "Giảm giá không được lớn hơn tạm tính hóa đơn.\n" +
                    "Tạm tính: " +
                    tamTinh.ToString("N0", vanHoa) + " đ",
                    "Dữ liệu không hợp lệ",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                txtGiamGia.Focus();
                txtGiamGia.SelectAll();
                return false;
            }

            return true;
        }

        // 8. THANH TOÁN HÓA ĐƠN

        private void btnThanhToan_Click(object sender, EventArgs e)
        {
            try
            {
                decimal giamGia;

                if (!KiemTraGiamGia(out giamGia))
                    return;

                string phuongThuc = LayPhuongThucThanhToan();

                // Tính tiền thống nhất với phần hiển thị trên giao diện.
                decimal tamTinh = ChuyenSangSo(txtTamTinh.Text);

                decimal tienSauGiam = tamTinh - giamGia;

                decimal tienThue = Math.Round(
                    tienSauGiam * 0.10m,
                    0,
                    MidpointRounding.AwayFromZero
                );

                decimal tongThanhToan = tienSauGiam + tienThue;

                DialogResult xacNhan = MessageBox.Show(
                    "XÁC NHẬN THANH TOÁN\n\n" +

                    "Mã hóa đơn: HD" +
                    hoaDonDangChon.MaHoaDon.ToString("D4") +

                    "\nTạm tính: " +
                    tamTinh.ToString("N0", vanHoa) + " đ" +

                    "\nGiảm giá: " +
                    giamGia.ToString("N0", vanHoa) + " đ" +

                    "\nTiền sau giảm: " +
                    tienSauGiam.ToString("N0", vanHoa) + " đ" +

                    "\nThuế (10%): " +
                    tienThue.ToString("N0", vanHoa) + " đ" +

                    "\nPhương thức: " +
                    phuongThuc +

                    "\n------------------------------" +

                    "\nTổng thanh toán: " +
                    tongThanhToan.ToString("N0", vanHoa) + " đ" +

                    "\n\nBạn có chắc chắn muốn thanh toán?",

                    "Xác nhận thanh toán",

                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (xacNhan != DialogResult.Yes)
                    return;

                bool ketQua = hoaDonService.ThanhToanHoaDon(
                    hoaDonDangChon.MaHoaDon,
                    giamGia,
                    phuongThuc
                );

                if (ketQua)
                {
                    if (phuongThuc == "Chuyển khoản")
                    {
                        MessageBox.Show(
                            "Đã tạo yêu cầu thanh toán chuyển khoản.\n" +
                            "Vui lòng chờ nhân viên kiểm tra và xác nhận tiền đã nhận.",
                            "Đang chờ xác nhận",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );
                    }
                    else
                    {
                        MessageBox.Show(
                            "Thanh toán hóa đơn thành công!",
                            "Thông báo",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );
                    }

                    chkChuyenKhoan.Checked = false;
                    TaiDanhSachHoaDon();
                }
                else
                {
                    MessageBox.Show(
                        "Không thể thanh toán hóa đơn.\n" +
                        "Vui lòng kiểm tra lại trạng thái hóa đơn.",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Thanh toán thất bại.\n\n" + ex.Message,
                    "Lỗi thanh toán",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // 9. IN HÓA ĐƠN

        private void btnInHoaDon_Click(object sender, EventArgs e)
        {
            try
            {
                if (hoaDonDangChon == null)
                {
                    MessageBox.Show(
                        "Vui lòng chọn hóa đơn cần in.",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                // Lấy lại chi tiết hóa đơn mới nhất từ cơ sở dữ liệu.
                hoaDonIn = hoaDonDangChon;

                danhSachChiTietIn =
                    hoaDonService.GetChiTietHoaDon(hoaDonIn.MaHoaDon);

                if (danhSachChiTietIn == null ||
                    danhSachChiTietIn.Count == 0)
                {
                    MessageBox.Show(
                        "Hóa đơn chưa có món ăn nên không thể in.",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                // Tính tiền dựa trên chi tiết món ăn.
                tamTinhIn = 0;

                foreach (ChiTietHoaDon ct in danhSachChiTietIn)
                {
                    tamTinhIn += ct.SoLuong * ct.DonGia;
                }

                giamGiaIn = hoaDonIn.GiamGia;

                if (giamGiaIn < 0)
                    giamGiaIn = 0;

                if (giamGiaIn > tamTinhIn)
                    giamGiaIn = tamTinhIn;

                decimal tienSauGiam = tamTinhIn - giamGiaIn;

                // Hóa đơn chưa thanh toán: tính thuế dự kiến 10%.
                // Hóa đơn đã thanh toán hoặc đang xử lý: lấy thuế đã lưu.
                if (string.Equals(
                    hoaDonIn.TrangThai?.Trim(),
                    "Chưa thanh toán",
                    StringComparison.OrdinalIgnoreCase))
                {
                    tienThueIn = Math.Round(
                        tienSauGiam * 0.10m,
                        0,
                        MidpointRounding.AwayFromZero
                    );
                }
                else
                {
                    tienThueIn = hoaDonIn.TienThue;
                }

                tongTienIn = tienSauGiam + tienThueIn;

                // Khởi tạo tài liệu in.
                viTriDongIn = 0;

                printDocument = new PrintDocument();

                printDocument.DocumentName =
                    "HoaDon_HD" + hoaDonIn.MaHoaDon.ToString("D4");

                printDocument.DefaultPageSettings.Landscape = false;
                printDocument.DefaultPageSettings.PaperSize =
                    new PaperSize("A4", 827, 1169);

                printDocument.DefaultPageSettings.Margins =
                    new Margins(50, 50, 50, 50);

                printDocument.PrintPage += PrintDocument_PrintPage;

                // Hiển thị bản xem trước.
                using (PrintPreviewDialog preview = new PrintPreviewDialog())
                {
                    preview.Document = printDocument;
                    preview.WindowState = FormWindowState.Maximized;
                    preview.StartPosition = FormStartPosition.CenterScreen;

                    preview.ShowDialog(this);
                }

                printDocument.PrintPage -= PrintDocument_PrintPage;
                printDocument.Dispose();
                printDocument = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể tạo bản in hóa đơn.\n\n" + ex.Message,
                    "Lỗi in hóa đơn",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // 10. HÀM XỬ LÝ TIỀN

        private decimal ChuyenSangSo(string giaTri)
        {
            if (string.IsNullOrWhiteSpace(giaTri))
                return 0;

            giaTri = giaTri.Replace("đ", "")
                           .Replace("Đ", "")
                           .Replace("₫", "")
                           .Replace(" ", "")
                           .Trim();

            decimal.TryParse(
                giaTri,
                NumberStyles.Number,
                vanHoa,
                out decimal ketQua
            );

            return ketQua;
        }

        private void CapNhatTongTien()
        {
            if (dangCapNhatTien)
                return;

            try
            {
                dangCapNhatTien = true;

                decimal tamTinh = ChuyenSangSo(txtTamTinh.Text);
                decimal giamGia = ChuyenSangSo(txtGiamGia.Text);

                decimal tienSauGiam = tamTinh - giamGia;

                if (tienSauGiam < 0)
                    tienSauGiam = 0;

                decimal thue;

                bool chuaThanhToan =
                    hoaDonDangChon == null ||
                    string.Equals(
                        hoaDonDangChon.TrangThai?.Trim(),
                        "Chưa thanh toán",
                        StringComparison.OrdinalIgnoreCase
                    );

                if (chuaThanhToan)
                {
                    // Thuế được tính trên tiền sau khi giảm giá.
                    thue = Math.Round(
                        tienSauGiam * 0.10m,
                        0,
                        MidpointRounding.AwayFromZero
                    );

                    txtThue.Text = thue.ToString("N0", vanHoa) + " đ";
                }
                else
                {
                    // Hóa đơn đã thanh toán: giữ thuế đã lưu.
                    thue = ChuyenSangSo(txtThue.Text);
                }

                decimal tongTien = tienSauGiam + thue;

                lblTongTien.Text =
                    tongTien.ToString("N0", vanHoa) + " đ";
            }
            finally
            {
                dangCapNhatTien = false;
            }
        }

        private string LayPhuongThucThanhToan()
        {
            return chkChuyenKhoan.Checked
                ? "Chuyển khoản"
                : "Tiền mặt";
        }

        // 11. SỰ KIỆN TÌM KIẾM VÀ LỌC

        private void txtTimMaHoaDon_TextChanged(object sender, EventArgs e)
        {
            if (daKhoiTao)
                TaiDanhSachHoaDon();
        }

        private void txtTimMaHoaDon_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                e.Handled = true;
        }

        private void cboLocTrangThai_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!daKhoiTao || dangTaiDanhSach)
                return;

            TaiDanhSachHoaDon();
        }
        // Tự động tải lại danh sách khi thay đổi khoảng ngày.
        private void kdtpNgay_ValueChanged(object sender, EventArgs e)
        {
            if (!daKhoiTao || dangTaiDanhSach)
                return;

            TaiDanhSachHoaDon();
        }
        private void PrintDocument_PrintPage(
    object sender,
    PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;

            Rectangle bounds = e.MarginBounds;

            float x = bounds.Left;
            float y = bounds.Top;
            float width = bounds.Width;

            using (Font fontTieuDe = new Font("Arial", 20, FontStyle.Bold))
            using (Font fontTieuDePhu = new Font("Arial", 16, FontStyle.Bold))
            using (Font fontPhu = new Font("Arial", 10, FontStyle.Regular))
            using (Font fontDam = new Font("Arial", 10, FontStyle.Bold))
            using (Font fontThuong = new Font("Arial", 9, FontStyle.Regular))
            using (Font fontTong = new Font("Arial", 12, FontStyle.Bold))
            using (Pen pen = new Pen(Color.Black, 1))
            using (StringFormat canGiua = new StringFormat())
            using (StringFormat canPhai = new StringFormat())
            {
                canGiua.Alignment = StringAlignment.Center;
                canGiua.LineAlignment = StringAlignment.Center;

                canPhai.Alignment = StringAlignment.Far;
                canPhai.LineAlignment = StringAlignment.Center;
                // PHẦN ĐẦU HÓA ĐƠN - CHỈ IN Ở TRANG ĐẦU
                if (viTriDongIn == 0)
                {
                    g.DrawString(
                        "NHÀ HÀNG",
                        fontTieuDe,
                        Brushes.Black,
                        new RectangleF(x, y, width, 35),
                        canGiua
                    );

                    y += 42;

                    g.DrawString(
                        "HÓA ĐƠN THANH TOÁN",
                        fontTieuDePhu,
                        Brushes.Black,
                        new RectangleF(x, y, width, 30),
                        canGiua
                    );

                    y += 40;

                    g.DrawLine(pen, x, y, x + width, y);

                    y += 15;

                    string maHD =
                        "HD" + hoaDonIn.MaHoaDon.ToString("D4");

                    g.DrawString(
                        "Mã hóa đơn: " + maHD,
                        fontDam,
                        Brushes.Black,
                        x,
                        y
                    );

                    g.DrawString(
                        "Bàn số: " +
                        (hoaDonIn.SoBan.HasValue
                            ? hoaDonIn.SoBan.Value.ToString()
                            : "--"),
                        fontDam,
                        Brushes.Black,
                        x + width / 2,
                        y
                    );

                    y += 25;

                    g.DrawString(
                        "Ngày lập: " +
                        hoaDonIn.NgayLap.ToString("dd/MM/yyyy") +
                        " " +
                        hoaDonIn.GioLap.ToString(@"hh\:mm"),
                        fontThuong,
                        Brushes.Black,
                        x,
                        y
                    );

                    y += 22;

                    string tenNV = string.IsNullOrWhiteSpace(
                        hoaDonIn.TenNhanVien)
                            ? tenNhanVien
                            : hoaDonIn.TenNhanVien;

                    g.DrawString(
                        "Nhân viên: " + tenNV,
                        fontThuong,
                        Brushes.Black,
                        x,
                        y
                    );

                    y += 30;
                }
                else
                {
                    g.DrawString(
                        "HÓA ĐƠN - HD" +
                        hoaDonIn.MaHoaDon.ToString("D4") +
                        " (Tiếp theo)",
                        fontDam,
                        Brushes.Black,
                        x,
                        y
                    );

                    y += 35;
                }
                // TIÊU ĐỀ BẢNG CHI TIẾT MÓN ĂN
                float cotSTT = 40;
                float cotMon = width * 0.42f;
                float cotSL = 55;
                float cotDonGia = width * 0.22f;

                float cotThanhTien =
                    width - cotSTT - cotMon - cotSL - cotDonGia;

                float xSTT = x;
                float xMon = xSTT + cotSTT;
                float xSL = xMon + cotMon;
                float xDonGia = xSL + cotSL;
                float xThanhTien = xDonGia + cotDonGia;

                float chieuCaoDong = 30;

                g.FillRectangle(
                    Brushes.Gainsboro,
                    x,
                    y,
                    width,
                    chieuCaoDong
                );

                g.DrawRectangle(
                    pen,
                    x,
                    y,
                    width,
                    chieuCaoDong
                );

                g.DrawString(
                    "STT",
                    fontDam,
                    Brushes.Black,
                    new RectangleF(xSTT, y, cotSTT, chieuCaoDong),
                    canGiua
                );

                g.DrawString(
                    "Tên món",
                    fontDam,
                    Brushes.Black,
                    new RectangleF(xMon + 4, y, cotMon - 4, chieuCaoDong),
                    canGiua
                );

                g.DrawString(
                    "SL",
                    fontDam,
                    Brushes.Black,
                    new RectangleF(xSL, y, cotSL, chieuCaoDong),
                    canGiua
                );

                g.DrawString(
                    "Đơn giá",
                    fontDam,
                    Brushes.Black,
                    new RectangleF(xDonGia, y, cotDonGia, chieuCaoDong),
                    canGiua
                );

                g.DrawString(
                    "Thành tiền",
                    fontDam,
                    Brushes.Black,
                    new RectangleF(xThanhTien, y, cotThanhTien, chieuCaoDong),
                    canGiua
                );

                y += chieuCaoDong;
                // IN DANH SÁCH MÓN ĂN
                while (viTriDongIn < danhSachChiTietIn.Count)
                {
                    // Dành khoảng trống để in phần tổng tiền
                    // ở cuối trang cuối cùng.
                    float khoangTrongCuoiTrang = 240;

                    if (y + chieuCaoDong >
                        bounds.Bottom - khoangTrongCuoiTrang)
                    {
                        e.HasMorePages = true;
                        return;
                    }

                    ChiTietHoaDon ct =
                        danhSachChiTietIn[viTriDongIn];

                    decimal thanhTien = ct.SoLuong * ct.DonGia;

                    RectangleF dong = new RectangleF(
                        x,
                        y,
                        width,
                        chieuCaoDong
                    );

                    g.DrawRectangle(
                        pen,
                        dong.X,
                        dong.Y,
                        dong.Width,
                        dong.Height
                    );

                    g.DrawString(
                        (viTriDongIn + 1).ToString(),
                        fontThuong,
                        Brushes.Black,
                        new RectangleF(xSTT, y, cotSTT, chieuCaoDong),
                        canGiua
                    );

                    g.DrawString(
                        ct.TenMonAn,
                        fontThuong,
                        Brushes.Black,
                        new RectangleF(xMon + 5, y, cotMon - 10, chieuCaoDong),
                        canGiua
                    );

                    g.DrawString(
                        ct.SoLuong.ToString(),
                        fontThuong,
                        Brushes.Black,
                        new RectangleF(xSL, y, cotSL, chieuCaoDong),
                        canGiua
                    );

                    g.DrawString(
                        ct.DonGia.ToString("N0", vanHoa),
                        fontThuong,
                        Brushes.Black,
                        new RectangleF(xDonGia, y, cotDonGia - 5, chieuCaoDong),
                        canPhai
                    );

                    g.DrawString(
                        thanhTien.ToString("N0", vanHoa),
                        fontThuong,
                        Brushes.Black,
                        new RectangleF(xThanhTien, y, cotThanhTien - 5, chieuCaoDong),
                        canPhai
                    );

                    y += chieuCaoDong;
                    viTriDongIn++;
                }
                // TỔNG KẾT HÓA ĐƠN - TRANG CUỐI
                y += 20;

                g.DrawLine(pen, x, y, x + width, y);

                y += 15;

                float cotGiaTri = x + width - 220;

                g.DrawString(
                    "Tạm tính:",
                    fontThuong,
                    Brushes.Black,
                    x + 100,
                    y
                );

                g.DrawString(
                    tamTinhIn.ToString("N0", vanHoa) + " đ",
                    fontThuong,
                    Brushes.Black,
                    new RectangleF(cotGiaTri, y, 220, 22),
                    canPhai
                );

                y += 25;

                g.DrawString(
                    "Giảm giá:",
                    fontThuong,
                    Brushes.Black,
                    x + 100,
                    y
                );

                g.DrawString(
                    giamGiaIn.ToString("N0", vanHoa) + " đ",
                    fontThuong,
                    Brushes.Black,
                    new RectangleF(cotGiaTri, y, 220, 22),
                    canPhai
                );

                y += 25;

                g.DrawString(
                    "Thuế (10%):",
                    fontThuong,
                    Brushes.Black,
                    x + 100,
                    y
                );

                g.DrawString(
                    tienThueIn.ToString("N0", vanHoa) + " đ",
                    fontThuong,
                    Brushes.Black,
                    new RectangleF(cotGiaTri, y, 220, 22),
                    canPhai
                );

                y += 30;

                g.DrawLine(pen, x + 100, y, x + width, y);

                y += 12;

                g.DrawString(
                    "TỔNG THANH TOÁN:",
                    fontTong,
                    Brushes.Black,
                    x + 100,
                    y
                );

                g.DrawString(
                    tongTienIn.ToString("N0", vanHoa) + " đ",
                    fontTong,
                    Brushes.Black,
                    new RectangleF(cotGiaTri, y, 220, 25),
                    canPhai
                );

                y += 50;

                g.DrawString(
                    "Cảm ơn quý khách và hẹn gặp lại!",
                    fontDam,
                    Brushes.Black,
                    new RectangleF(x, y, width, 25),
                    canGiua
                );

                e.HasMorePages = false;
            }
        }
    }
}
