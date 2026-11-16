using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Krypton.Toolkit;
using QuanLyNhaHang.Data;
using QuanLyNhaHang.Forms;

namespace QuanLyNhaHang
{
    public partial class frmTrangChu : Form
    {
        private readonly string tenNhanVien;
        private readonly int? maNhanVien;
        private readonly string vaiTro;

        // Màu nền mặc định của nút menu
        private readonly Color mauMacDinh =
            Color.FromArgb(0, 87, 153);

        // Màu nền khi nút được chọn
        private readonly Color mauDangChon =
            Color.FromArgb(126, 211, 245);

        // Màu chữ mặc định
        private readonly Color mauChuMacDinh = Color.White;

        // Màu chữ khi được chọn
        private readonly Color mauChuDangChon =
            Color.FromArgb(0, 65, 110);

        private KryptonButton[] cacNutMenu;
        private Form formConHienTai;

        // UserControl Dashboard
        private ucTrangChu dashboard;

        // Tiêu đề chức năng ở Header
        private Label lblTieuDeTrang;

        // Constructor mặc định
        // Không tự động cấp quyền quản trị
        public frmTrangChu()
            : this("Nhân viên", null, "Nhân viên")
        {
        }

        // Constructor cũ chỉ truyền tên
        public frmTrangChu(string tenNguoiDung)
            : this(tenNguoiDung, null, "Nhân viên")
        {
        }

        // Constructor cũ truyền tên và mã nhân viên
        public frmTrangChu(
            string tenNguoiDung,
            int? maNhanVien)
            : this(tenNguoiDung, maNhanVien, "Nhân viên")
        {
        }

        // Constructor mới nhận tên, mã nhân viên và vai trò
        public frmTrangChu(
            string tenNguoiDung,
            int? maNhanVien,
            string vaiTro)
        {
            InitializeComponent();

            this.tenNhanVien =
                string.IsNullOrWhiteSpace(tenNguoiDung)
                ? "Nhân viên"
                : tenNguoiDung;

            this.maNhanVien = maNhanVien;

            this.vaiTro =
                string.IsNullOrWhiteSpace(vaiTro)
                ? "Nhân viên"
                : vaiTro.Trim();

            KhoiTaoGiaoDien();
            GanSuKienMenu();
            PhanQuyenMenu();
        }

        // ==========================================
        // KHỞI TẠO GIAO DIỆN
        // ==========================================

        private void KhoiTaoGiaoDien()
        {
            // Thông tin nhân viên và ngày tháng
            label4.Text = "Quản lý • " + tenNhanVien;

            label3.Text = DateTime.Now.ToString(
                "dddd, dd/MM/yyyy",
                CultureInfo.GetCultureInfo("vi-VN"));

            // Danh sách nút menu
            cacNutMenu = new KryptonButton[]
            {
        btnTrangChu,
        btnQuanLyBan,
        btnGoiMon,
        btnHoaDon,
        btnThanhToan,
        btnQuanLyDanhMuc,
        btnQuanLyMonAn,
        btnNhanVien,
        btnThongKe
            };

            foreach (KryptonButton nut in cacNutMenu)
            {
                nut.Cursor = Cursors.Hand;
                CauHinhIconMenu(nut);
                DatMauNut(
                    nut,
                    mauMacDinh,
                    mauChuMacDinh);
            }

            // Tiêu đề Header
            KhoiTaoTieuDeHeader("Tổng quan");

            // Chọn menu Trang chủ mặc định
            ChonNutMenu(btnTrangChu);

            // Khi Form chính được mở, tự động hiển thị Dashboard
            Load += (s, e) =>
            {
                HienThiTrangChu();
            };
        }

        // TIÊU ĐỀ HEADER

        private void KhoiTaoTieuDeHeader(string tieuDe)
        {
            lblTieuDeTrang = TimLabelTheoNoiDung(
                panelContent,
                "Tổng quan");

            if (lblTieuDeTrang == null)
            {
                lblTieuDeTrang = new Label
                {
                    Name = "lblTieuDeTrang",
                    AutoSize = true,
                    Text = tieuDe,
                    Font = new Font(
                        "Segoe UI",
                        15,
                        FontStyle.Bold),
                    ForeColor = Color.FromArgb(33, 37, 41),
                    BackColor = Color.Transparent,
                    Location = new Point(16, 12)
                };

                panelContent.Controls.Add(lblTieuDeTrang);
                lblTieuDeTrang.BringToFront();
            }
            else
            {
                lblTieuDeTrang.Text = tieuDe;
            }
        }

        private Label TimLabelTheoNoiDung(
            Control parent,
            string noiDung)
        {
            foreach (Control control in parent.Controls)
            {
                if (control is Label label &&
                    string.Equals(
                        label.Text.Trim(),
                        noiDung,
                        StringComparison.OrdinalIgnoreCase))
                {
                    return label;
                }

                Label ketQua =
                    TimLabelTheoNoiDung(control, noiDung);

                if (ketQua != null)
                    return ketQua;
            }

            return null;
        }

        private void CapNhatTieuDe(string tieuDe)
        {
            if (lblTieuDeTrang != null)
                lblTieuDeTrang.Text = tieuDe;
        }

        // ==========================================
        // MÀU SẮC MENU
        // ==========================================

        private void DatMauNut(
            KryptonButton nut,
            Color mauNen,
            Color mauChu)
        {
            nut.StateCommon.Back.Color1 = mauNen;
            nut.StateCommon.Back.Color2 = mauNen;

            nut.StateCommon.Content.ShortText.Color1 = mauChu;
            nut.StateNormal.Content.ShortText.Color1 = mauChu;
            nut.StateTracking.Content.ShortText.Color1 = mauChu;
            nut.StatePressed.Content.ShortText.Color1 = mauChu;
        }

        private void ChonNutMenu(KryptonButton nutDuocChon)
        {
            foreach (KryptonButton nut in cacNutMenu)
            {
                if (nut == nutDuocChon)
                {
                    DatMauNut(
                        nut,
                        mauDangChon,
                        mauChuDangChon);
                }
                else
                {
                    DatMauNut(
                        nut,
                        mauMacDinh,
                        mauChuMacDinh);
                }
            }
        }

        // ==========================================
        // PHÂN QUYỀN
        // ==========================================

        private void PhanQuyenMenu()
        {
            // Mặc định ẩn toàn bộ menu
            foreach (KryptonButton nut in cacNutMenu)
            {
                nut.Visible = false;
            }

            // Trang chủ được phép truy cập với mọi vai trò
            btnTrangChu.Visible = true;

            switch (vaiTro)
            {
                case "Quản trị viên":

                    // Quản trị viên được sử dụng toàn bộ chức năng
                    foreach (KryptonButton nut in cacNutMenu)
                    {
                        nut.Visible = true;
                    }

                    break;

                case "Quản lý":

                    btnTrangChu.Visible = true;
                    btnQuanLyBan.Visible = true;
                    btnGoiMon.Visible = true;
                    btnHoaDon.Visible = true;
                    btnThanhToan.Visible = true;
                    btnQuanLyDanhMuc.Visible = true;
                    btnQuanLyMonAn.Visible = true;
                    btnThongKe.Visible = true;

                    break;

                case "Nhân viên":

                    btnTrangChu.Visible = true;
                    btnQuanLyBan.Visible = true;
                    btnGoiMon.Visible = true;
                    btnHoaDon.Visible = true;
                    btnThanhToan.Visible = true;

                    break;

                default:

                    // Vai trò không hợp lệ
                    foreach (KryptonButton nut in cacNutMenu)
                    {
                        nut.Visible = false;
                    }

                    MessageBox.Show(
                        "Tài khoản chưa được phân quyền hợp lệ.",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    break;
            }
        }

        // Kiểm tra quyền trước khi mở chức năng
        private bool CoQuyen(string chucNang)
        {
            switch (vaiTro)
            {
                case "Quản trị viên":
                    return true;

                case "Quản lý":
                    return chucNang != "QuanLyNhanVien";

                case "Nhân viên":

                    return chucNang == "TrangChu"
                        || chucNang == "QuanLyBan"
                        || chucNang == "GoiMon"
                        || chucNang == "HoaDon"
                        || chucNang == "ThanhToan";

                default:
                    return false;
            }
        }

        // Hiển thị thông báo khi không đủ quyền
        private bool KiemTraQuyen(string chucNang)
        {
            if (!CoQuyen(chucNang))
            {
                MessageBox.Show(
                    "Bạn không có quyền truy cập chức năng này!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return false;
            }

            return true;
        }

        // ==========================================
        // GÁN SỰ KIỆN MENU
        // ==========================================

        private void GanSuKienMenu()
        {
            btnTrangChu.Click += btnTrangChu_Click;
            btnQuanLyBan.Click += btnQuanLyBan_Click;
            btnGoiMon.Click += btnGoiMon_Click;
            btnQuanLyDanhMuc.Click += btnQuanLyDanhMuc_Click;
            btnHoaDon.Click += btnHoaDon_Click;
            btnThanhToan.Click += btnThanhToan_Click;
            btnQuanLyMonAn.Click += btnQuanLyMonAn_Click;
            btnNhanVien.Click += btnNhanVien_Click;
            btnThongKe.Click += btnThongKe_Click;
        }

        // ==========================================
        // CHUYỂN FORM VÀO PANEL NỘI DUNG
        // ==========================================

        private void MoFormTrongPanel(Form formCon)
        {
            if (formCon == null)
                return;

            // Đóng Form con đang mở
            DongFormConHienTai();

            // Xóa UserControl Dashboard khỏi vùng nội dung
            pnlNoiDung.Controls.Clear();

            formConHienTai = formCon;

            formCon.TopLevel = false;
            formCon.FormBorderStyle = FormBorderStyle.None;
            formCon.Dock = DockStyle.Fill;

            pnlNoiDung.Controls.Add(formCon);

            formCon.Show();
            formCon.BringToFront();
        }

        private void DongFormConHienTai()
        {
            if (formConHienTai == null)
                return;

            Form formCu = formConHienTai;
            formConHienTai = null;

            pnlNoiDung.Controls.Remove(formCu);

            formCu.Close();
            formCu.Dispose();
        }

        // Hiển thị Dashboard trang chủ và cập nhật dữ liệu mới nhất.
        private void HienThiTrangChu()
        {
            if (!KiemTraQuyen("TrangChu"))
                return;

            DongFormConHienTai();

            pnlNoiDung.Controls.Clear();

            if (dashboard == null || dashboard.IsDisposed)
            {
                dashboard = new ucTrangChu();
                dashboard.Dock = DockStyle.Fill;
            }

            pnlNoiDung.Controls.Add(dashboard);

            dashboard.BringToFront();

            // Cập nhật dữ liệu Dashboard mỗi khi quay về trang chủ.
            dashboard.TaiDuLieuDashboard();

            CapNhatTieuDe("Tổng quan");

            ChonNutMenu(btnTrangChu);
        }

        // CÁC CHỨC NĂNG MENU
        private void btnTrangChu_Click(
            object sender,
            EventArgs e)
        {
            HienThiTrangChu();
        }

        private void btnQuanLyBan_Click(
            object sender,
            EventArgs e)
        {
            if (!KiemTraQuyen("QuanLyBan"))
                return;

            ChonNutMenu(btnQuanLyBan);
            CapNhatTieuDe("Quản lý bàn");

            MoFormTrongPanel(
                new frmQuanLyBan(tenNhanVien));
        }

        private void btnGoiMon_Click(
            object sender,
            EventArgs e)
        {
            if (!KiemTraQuyen("GoiMon"))
                return;

            ChonNutMenu(btnGoiMon);
            CapNhatTieuDe("Gọi món");

            MoFormTrongPanel(
                new frmGoiMon(
                    tenNhanVien,
                    1,
                    maNhanVien
                )
            );
        }

        private void btnHoaDon_Click(
            object sender,
            EventArgs e)
        {
            if (!KiemTraQuyen("HoaDon"))
                return;

            ChonNutMenu(btnHoaDon);
            CapNhatTieuDe("Quản lý hóa đơn");

            MoFormTrongPanel(
                new frmHoaDon(tenNhanVien));
        }

        private void btnThanhToan_Click(
            object sender,
            EventArgs e)
        {
            if (!KiemTraQuyen("ThanhToan"))
                return;

            ChonNutMenu(btnThanhToan);
            CapNhatTieuDe("Thanh toán");

            MoFormTrongPanel(
                new frmThanhToan());
        }

        private void btnQuanLyDanhMuc_Click(
            object sender,
            EventArgs e)
        {
            if (!KiemTraQuyen("QuanLyDanhMuc"))
                return;

            ChonNutMenu(btnQuanLyDanhMuc);
            CapNhatTieuDe("Quản lý danh mục");

            MoFormTrongPanel(
                new frmQuanLyDanhMuc());
        }

        private void btnQuanLyMonAn_Click(
            object sender,
            EventArgs e)
        {
            if (!KiemTraQuyen("QuanLyMonAn"))
                return;

            ChonNutMenu(btnQuanLyMonAn);
            CapNhatTieuDe("Quản lý món ăn");

            MoFormTrongPanel(
                new frmQuanLyMonAn());
        }

        private void btnNhanVien_Click(
            object sender,
            EventArgs e)
        {
            if (!KiemTraQuyen("QuanLyNhanVien"))
                return;

            ChonNutMenu(btnNhanVien);
            CapNhatTieuDe("Quản lý nhân viên");

            MoFormTrongPanel(
                new frmNhanVien());
        }

        private void btnThongKe_Click(
            object sender,
            EventArgs e)
        {
            if (!KiemTraQuyen("ThongKe"))
                return;

            ChonNutMenu(btnThongKe);
            CapNhatTieuDe("Thống kê");

            MoFormTrongPanel(
                new frmThongKe());
        }

        // ==========================================
        // THAO TÁC NHANH TRÊN DASHBOARD
        // ==========================================

        private void btnTaoGoiMon_Click(
            object sender,
            EventArgs e)
        {
            if (!KiemTraQuyen("GoiMon"))
                return;

            ChonNutMenu(btnGoiMon);
            CapNhatTieuDe("Gọi món");

            MoFormTrongPanel(
                new frmGoiMon(
                    tenNhanVien,
                    1,
                    maNhanVien
                )
            );
        }

        private void btnChonBan_Click(
            object sender,
            EventArgs e)
        {
            if (!KiemTraQuyen("QuanLyBan"))
                return;

            ChonNutMenu(btnQuanLyBan);
            CapNhatTieuDe("Quản lý bàn");

            MoFormTrongPanel(
                new frmQuanLyBan(tenNhanVien));
        }

        // ==========================================
        // ĐĂNG XUẤT VÀ ĐÓNG FORM
        // ==========================================

        protected override void OnFormClosed(
            FormClosedEventArgs e)
        {
            DongFormConHienTai();

            base.OnFormClosed(e);
        }

        private void btnDangXuat_Click(object sender, EventArgs e)
        {
            DialogResult ketQua = MessageBox.Show(
                "Bạn có chắc chắn muốn đăng xuất không?",
                "Xác nhận đăng xuất",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (ketQua == DialogResult.Yes)
            {
                this.Hide();

                frmDangNhap frm = new frmDangNhap();
                frm.Show();

                this.Close();
            }
        }

        public void ClickQuanLyBan()
        {
            btnQuanLyBan.PerformClick();
        }

        public void ClickGoiMon()
        {
            btnGoiMon.PerformClick();
        }

        public void ClickHoaDon()
        {
            btnHoaDon.PerformClick();
        }

        public void ClickThanhToan()
        {
            btnThanhToan.PerformClick();
        }

        private void CauHinhIconMenu(KryptonButton nut)
        {
            // Căn icon nằm bên trái
            nut.StateCommon.Content.Image.ImageH =
                PaletteRelativeAlign.Near;

            // Căn icon ở giữa theo chiều dọc
            nut.StateCommon.Content.Image.ImageV =
                PaletteRelativeAlign.Center;

            // Căn chữ nằm bên trái
            nut.StateCommon.Content.ShortText.TextH =
                PaletteRelativeAlign.Near;

            // Căn chữ ở giữa theo chiều dọc
            nut.StateCommon.Content.ShortText.TextV =
                PaletteRelativeAlign.Center;

            // Khoảng cách lề bên trong nút
            nut.StateCommon.Content.Padding =
                new Padding(12, 0, 8, 0);
        }
    }
}