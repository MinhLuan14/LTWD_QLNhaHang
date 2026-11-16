using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Configuration;
using System.Windows.Forms;
using QuanLyNhaHang.Services;
using QuanLyNhaHang.Models;
using QuanLyNhaHang.Repositories;

namespace QuanLyNhaHang
{
    public partial class frmGoiMon : Form
    {        // 1. KHAI BÁO
        private readonly string tenNhanVien;
        private readonly int? maNhanVien;
        private MonAn monDangChon;
        private int maBan = 1;
        private int? maDatBan;
        private int? maKhachHang;

        // Lưu mã đặt bàn được truyền từ bên ngoài, nếu có
        private readonly int? maDatBanMacDinh;
        private readonly MonAnRepository monAnRepository =
            new MonAnRepository();

        private readonly LoaiMonRepository loaiMonRepository =
            new LoaiMonRepository();

        private readonly PhieuGoiMonService phieuGoiMonService =
                new PhieuGoiMonService();

        // Danh sách món ăn lấy từ database
        private List<MonAn> danhSachMonAn = new List<MonAn>();

        // Món đã được lưu trong SQL Server
        private readonly Dictionary<int, int> monDaLuu =
            new Dictionary<int, int>();

        // Món mới thêm, chưa xác nhận lưu vào phiếu
        private readonly Dictionary<int, int> gioHang =
            new Dictionary<int, int>();

        private bool dangLoadDuLieu = false;
        private bool dangChonMonTuDataGridView = false;
        private readonly string connectionString =
            ConfigurationManager.ConnectionStrings[
                "QuanLyNhaHangConnection"
            ].ConnectionString;
        // 2. CLASS HỖ TRỢ COMBOBOX
        private class BanItem
        {
            public int MaBan { get; set; }
            public int SoBan { get; set; }
            public int SoChoNgoi { get; set; }
            public string TrangThai { get; set; }

            public string TenHienThi
            {
                get
                {
                    return "Bàn " + SoBan +
                           " - " + TrangThai;
                }
            }
        }

        private class DanhMucItem
        {
            public int MaLoaiMon { get; set; }
            public string TenLoaiMon { get; set; }
        }
        // 3. CONSTRUCTOR
        public frmGoiMon()
    : this("Quản trị viên", 1, null, null)
        {
        }

        public frmGoiMon(string tenNguoiDung)
            : this(tenNguoiDung, 1, null, null)
        {
        }

        public frmGoiMon(string tenNguoiDung, int idBan)
            : this(tenNguoiDung, idBan, null, null)
        {
        }

        public frmGoiMon(
            string tenNguoiDung,
            int idBan,
            int? idNhanVien)
            : this(tenNguoiDung, idBan, idNhanVien, null)
        {
        }

        public frmGoiMon(
            string tenNguoiDung,
            int idBan,
            int? idNhanVien,
            int? idDatBan)
        {
            InitializeComponent();

            tenNhanVien = tenNguoiDung;
            maBan = Math.Max(1, idBan);
            maNhanVien = idNhanVien;
            maDatBanMacDinh = idDatBan;
            maDatBan = idDatBan;
        }
        // 4. FORM LOAD
        private void frmGoiMon_Load(object sender, EventArgs e)
        {
            try
            {
                dangLoadDuLieu = true;

                CauHinhDanhSachMon();
                CauHinhGioHang();
                CauHinhTimKiem();

                LoadDanhSachMonAn();
                LoadDanhMuc();
                LoadDanhSachBan();

                // Tự lấy mã đặt bàn và khách hàng của bàn ban đầu
                CapNhatThongTinDatBanTheoBan(maBan);

                HienThiDanhSachMon();

                dangLoadDuLieu = false;

                // Tải món đã gọi của bàn hiện tại
                LoadMonDaLuuTheoBan();

                CapNhatThongTinDonGoi();
            }
            catch (Exception ex)
            {
                dangLoadDuLieu = false;

                MessageBox.Show(
                    "Không thể tải dữ liệu gọi món.\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
        // 5. CẤU HÌNH GIAO DIỆN
        private void CauHinhDanhSachMon()
        {
            flpDanhSachMon.FlowDirection =
                FlowDirection.LeftToRight;

            flpDanhSachMon.WrapContents = true;
            flpDanhSachMon.AutoScroll = true;
        }

        private void CauHinhGioHang()
        {
            dgvHienMonAn.AutoGenerateColumns = false;

            TenMon.DataPropertyName = "tenMonAn";
            DonGia.DataPropertyName = "donGia";
            SoLuong.DataPropertyName = "soLuong";
            ThanhTien.DataPropertyName = "thanhTien";

            DonGia.DefaultCellStyle.Format = "N0";
            ThanhTien.DefaultCellStyle.Format = "N0";

            // Cột mã món ẩn để xử lý xóa
            if (!dgvHienMonAn.Columns.Contains("MaMonAn"))
            {
                DataGridViewTextBoxColumn cotMaMon =
                    new DataGridViewTextBoxColumn();

                cotMaMon.Name = "MaMonAn";
                cotMaMon.DataPropertyName = "maMonAn";
                cotMaMon.Visible = false;

                dgvHienMonAn.Columns.Add(cotMaMon);
            }

            dgvHienMonAn.AllowUserToAddRows = false;
            dgvHienMonAn.ReadOnly = true;
            dgvHienMonAn.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;
            dgvHienMonAn.MultiSelect = false;
        }

        private void CauHinhTimKiem()
        {
            txtTimKiem.Text = "Nhập tên món ăn cần tìm...";
            txtTimKiem.ForeColor = Color.Gray;

            txtTimKiem.Enter -= txtTimKiem_Enter;
            txtTimKiem.Leave -= txtTimKiem_Leave;

            txtTimKiem.Enter += txtTimKiem_Enter;
            txtTimKiem.Leave += txtTimKiem_Leave;
        }
        private void txtTimKiem_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTimKiem.Text))
            {
                txtTimKiem.Text = "Nhập tên món ăn cần tìm...";
                txtTimKiem.ForeColor = Color.Gray;
            }
        }
        private void txtTimKiem_Enter(object sender, EventArgs e)
        {
            if (txtTimKiem.Text == "Nhập tên món ăn cần tìm...")
            {
                txtTimKiem.Text = "";
                txtTimKiem.ForeColor = Color.Black;
            }
        }
        // 6. LOAD DANH SÁCH MÓN ĂN
        private void LoadDanhSachMonAn()
        {
            danhSachMonAn = monAnRepository.GetAll()
                .Where(m => m.TrangThai == true)
                .ToList();
        }
        // 7. LOAD DANH MỤC MÓN ĂN
        private void LoadDanhMuc()
        {
            List<DanhMucItem> danhSach =
                new List<DanhMucItem>();

            danhSach.Add(new DanhMucItem
            {
                MaLoaiMon = 0,
                TenLoaiMon = "Tất cả danh mục"
            });

            var danhMucTuDatabase =
                loaiMonRepository.GetAll();

            foreach (var item in danhMucTuDatabase)
            {
                danhSach.Add(new DanhMucItem
                {
                    MaLoaiMon = item.MaLoaiMon,
                    TenLoaiMon = item.TenLoaiMon
                });
            }

            cboDanhMuc.DataSource = null;
            cboDanhMuc.DisplayMember = "TenLoaiMon";
            cboDanhMuc.ValueMember = "MaLoaiMon";
            cboDanhMuc.DataSource = danhSach;

            cboDanhMuc.SelectedValue = 0;
        }
        // 8. LOAD DANH SÁCH BÀN

        private void LoadDanhSachBan()
        {
            List<BanItem> danhSachBan = new List<BanItem>();

            using (SqlConnection conn =
                new SqlConnection(connectionString))
            {
                string sql = @"
            SELECT
                b.maBan,
                b.soBan,
                b.soChoNgoi,
                b.trangThai
            FROM dbo.Ban b
            WHERE
                (
                    b.trangThai IN
                    (
                        N'Trống',
                        N'Đang phục vụ',
                        N'Đã đặt'
                    )
                    OR b.maBan = @maBan
                )
                AND
                (
                    @maDatBan IS NULL
                    OR EXISTS
                    (
                        SELECT 1
                        FROM dbo.ChiTietDatBan ct
                        WHERE ct.maBan = b.maBan
                          AND ct.maDatBan = @maDatBan
                    )
                )
            ORDER BY b.soBan;";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add(
                        "@maBan", SqlDbType.Int).Value = maBan;

                    cmd.Parameters.Add(
                        "@maDatBan", SqlDbType.Int).Value =
                        maDatBanMacDinh.HasValue
                            ? (object)maDatBanMacDinh.Value
                            : DBNull.Value;

                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            danhSachBan.Add(new BanItem
                            {
                                MaBan = Convert.ToInt32(reader["maBan"]),
                                SoBan = Convert.ToInt32(reader["soBan"]),
                                SoChoNgoi = Convert.ToInt32(reader["soChoNgoi"]),
                                TrangThai = reader["trangThai"].ToString()
                            });
                        }
                    }
                }
            }

            // Kiểm tra bàn ban đầu có nằm trong danh sách hợp lệ không
            if (!danhSachBan.Any(b => b.MaBan == maBan))
            {
                throw new Exception(
                    maDatBan.HasValue
                        ? "Bàn không thuộc đặt bàn này hoặc không còn hợp lệ."
                        : "Không tìm thấy bàn cần gọi món.");
            }

            cboChonBan.DataSource = null;
            cboChonBan.DisplayMember = "TenHienThi";
            cboChonBan.ValueMember = "MaBan";
            cboChonBan.DataSource = danhSachBan;

            cboChonBan.SelectedValue = maBan;
        }

        // 9. HIỂN THỊ DANH SÁCH MÓN ĂN
        private void HienThiDanhSachMon()
        {
            flpDanhSachMon.SuspendLayout();

            try
            {
                // Giải phóng các UserControl cũ
                foreach (Control control in flpDanhSachMon.Controls)
                {
                    control.Dispose();
                }

                flpDanhSachMon.Controls.Clear();

                List<MonAn> danhSachLoc = LocDanhSachMon();

                foreach (MonAn mon in danhSachLoc)
                {
                    ucMonAn uc = new ucMonAn();

                    uc.Width = 190;
                    uc.Height = 200;
                    uc.Margin = new Padding(5);

                    uc.HienThiMon(mon);

                    // Chọn món để hiển thị ảnh, tên, giá bên dưới
                    uc.OnChonMon += Uc_OnChonMon;

                    // Nút Thêm món trên thẻ
                    uc.OnThemMon += Uc_OnThemMon;

                    flpDanhSachMon.Controls.Add(uc);
                }
            }
            finally
            {
                flpDanhSachMon.ResumeLayout();
            }
        }
        // 10. LỌC MÓN THEO TÊN VÀ DANH MỤC
        private List<MonAn> LocDanhSachMon()
        {
            IEnumerable<MonAn> ketQua =
                danhSachMonAn;

            string tuKhoa = txtTimKiem.Text.Trim();

            if (tuKhoa == "Nhập tên món ăn cần tìm...")
                tuKhoa = "";

            if (!string.IsNullOrEmpty(tuKhoa))
            {
                ketQua = ketQua.Where(m =>
                    m.TenMonAn.IndexOf(
                        tuKhoa,
                        StringComparison.OrdinalIgnoreCase
                    ) >= 0
                );
            }

            if (cboDanhMuc.SelectedValue != null)
            {
                int maLoai = 0;

                int.TryParse(
                    cboDanhMuc.SelectedValue.ToString(),
                    out maLoai
                );

                if (maLoai > 0)
                {
                    ketQua = ketQua.Where(m =>
                        m.MaLoaiMon == maLoai
                    );
                }
            }

            return ketQua.ToList();
        }
        // 11. SỰ KIỆN THÊM MÓN TỪ UC

        private void Uc_OnThemMon(int maMonAn)
        {
            try
            {
                MonAn mon = danhSachMonAn.FirstOrDefault(
                    m => m.MaMonAn == maMonAn
                );

                if (mon == null)
                {
                    MessageBox.Show("Không tìm thấy món ăn!");
                    return;
                }

                Dictionary<int, int> monThem =
                    new Dictionary<int, int>();

                monThem.Add(maMonAn, 1);

                phieuGoiMonService.LuuGoiMon(
                    maBan,
                    maNhanVien,
                    monThem,
                    maDatBan
                );

                LoadMonDaLuuTheoBan();
                CapNhatThongTinDonGoi();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể thêm món:\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // 12. HIỂN THỊ GIỎ HÀNG
        private void HienThiGioHang()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("maMonAn", typeof(int));
            dt.Columns.Add("tenMonAn", typeof(string));
            dt.Columns.Add("donGia", typeof(decimal));
            dt.Columns.Add("soLuong", typeof(int));
            dt.Columns.Add("thanhTien", typeof(decimal));

            decimal tongTien = 0;

            // Gộp số lượng món đã lưu và món mới chưa lưu
            Dictionary<int, int> danhSachHienThi =
                new Dictionary<int, int>(monDaLuu);

            foreach (var item in gioHang)
            {
                if (danhSachHienThi.ContainsKey(item.Key))
                    danhSachHienThi[item.Key] += item.Value;
                else
                    danhSachHienThi.Add(item.Key, item.Value);
            }

            foreach (var item in danhSachHienThi)
            {
                MonAn mon = danhSachMonAn
                    .FirstOrDefault(m => m.MaMonAn == item.Key);

                if (mon == null)
                    continue;

                decimal thanhTien = mon.DonGia * item.Value;
                tongTien += thanhTien;

                dt.Rows.Add(
                    mon.MaMonAn,
                    mon.TenMonAn,
                    mon.DonGia,
                    item.Value,
                    thanhTien
                );
            }

            dgvHienMonAn.DataSource = dt;

            lblTongTien.Text = tongTien.ToString("N0") + " VNĐ";
        }

        // 13. XÓA MÓN KHỎI GIỎ HÀNG

        private void btnXoaMon_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Kiểm tra món được chọn
                if (dgvHienMonAn.CurrentRow == null ||
                    dgvHienMonAn.CurrentRow.IsNewRow)
                {
                    MessageBox.Show(
                        "Vui lòng chọn món cần xóa!",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                // 2. Lấy mã món ăn
                object giaTri = dgvHienMonAn.CurrentRow
                    .Cells["MaMonAn"].Value;

                if (giaTri == null || giaTri == DBNull.Value)
                    return;

                int maMon = Convert.ToInt32(giaTri);

                // 3. Lấy tên món
                MonAn mon = danhSachMonAn.FirstOrDefault(
                    m => m.MaMonAn == maMon
                );

                string tenMon = mon != null
                    ? mon.TenMonAn
                    : "Món ăn";

                // 4. Xác nhận xóa
                DialogResult xacNhan = MessageBox.Show(
                    "Bạn có chắc muốn xóa món \"" + tenMon +
                    "\" khỏi phiếu gọi món không?\n\n" +
                    "Toàn bộ số lượng của món này sẽ bị xóa.",
                    "Xác nhận xóa món",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (xacNhan != DialogResult.Yes)
                    return;

                // 5. Xóa món đã lưu trong database thông qua Service
                bool daLuu = monDaLuu.ContainsKey(maMon);

                if (daLuu)
                {
                    phieuGoiMonService.XoaMonKhoiPhieu(
                        maBan,
                        maMon,
                        maDatBan
                    );
                }

                // 6. Xóa món chưa lưu trong giỏ hàng
                if (gioHang.ContainsKey(maMon))
                {
                    gioHang.Remove(maMon);
                }

                // 7. Tải lại dữ liệu
                LoadMonDaLuuTheoBan();

                MessageBox.Show(
                    "Đã xóa món khỏi phiếu gọi món thành công!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể xóa món:\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // 14. LƯU PHIẾU GỌI MÓN

        private void btnThemVaoDon_Click(object sender, EventArgs e)
        {
            try
            {
                if (monDangChon == null)
                {
                    MessageBox.Show("Vui lòng chọn món ăn trước!");
                    return;
                }

                if (!monDangChon.TrangThai)
                {
                    MessageBox.Show("Món ăn hiện đã ngừng bán!");
                    return;
                }

                int soLuong = (int)nudSoLuong.Value;
                int maMon = monDangChon.MaMonAn;

                if (soLuong <= 0)
                {
                    MessageBox.Show("Số lượng phải lớn hơn 0!");
                    return;
                }

                if (dangChonMonTuDataGridView)
                {
                    // Cập nhật số lượng thông qua Service
                    phieuGoiMonService.CapNhatSoLuongMonTrongPhieu(
                        maBan,
                        maMon,
                        soLuong,
                        maDatBan
                    );

                    MessageBox.Show(
                        "Đã cập nhật số lượng món thành công!",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
                else
                {
                    // Thêm món thông qua Service
                    Dictionary<int, int> monThem =
                        new Dictionary<int, int>();

                    monThem.Add(maMon, soLuong);

                    phieuGoiMonService.LuuGoiMon(
                        maBan,
                        maNhanVien,
                        monThem,
                        maDatBan
                    );

                    MessageBox.Show(
                        "Đã thêm món vào đơn gọi món!",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }

                // Tải lại số lượng thực tế từ database
                LoadMonDaLuuTheoBan();

                dangChonMonTuDataGridView = false;
                btnThemVaoDon.Text = "Thêm vào đơn";
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể xử lý món ăn:\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }



        // 15. SỰ KIỆN TÌM KIẾM
        private void txtTimKiem_TextChanged(object sender, EventArgs e)
        {
            if (dangLoadDuLieu)
                return;

            HienThiDanhSachMon();
        }

        // 16. SỰ KIỆN CHỌN DANH MỤC
         private void cboDanhMuc_SelectedIndexChanged(object sender, EventArgs e)
                {
                    if (dangLoadDuLieu)
                        return;

                    HienThiDanhSachMon();
          }

        // 17. SỰ KIỆN CHỌN BÀN
        private void cboChonBan_SelectedIndexChanged(
    object sender,
    EventArgs e)
        {
            if (dangLoadDuLieu)
                return;

            BanItem banDuocChon =
                cboChonBan.SelectedItem as BanItem;

            if (banDuocChon == null)
                return;

            if (maBan == banDuocChon.MaBan)
                return;

            // Lưu lại thông tin bàn và đặt bàn trước khi đổi.
            int maBanCu = maBan;
            int? maDatBanCu = maDatBan;
            int? maKhachHangCu = maKhachHang;

            try
            {
                // Tìm thông tin đặt bàn của bàn mới.
                CapNhatThongTinDatBanTheoBan(
                    banDuocChon.MaBan
                );

                // Cập nhật bàn hiện tại.
                maBan = banDuocChon.MaBan;

                // Xóa giỏ hàng chưa lưu của bàn trước.
                gioHang.Clear();

                // Tải lại món theo đúng bàn và mã đặt bàn mới.
                LoadMonDaLuuTheoBan();

                CapNhatThongTinDonGoi();
            }
            catch (Exception ex)
            {
                // Khôi phục toàn bộ thông tin bàn cũ.
                maBan = maBanCu;
                maDatBan = maDatBanCu;
                maKhachHang = maKhachHangCu;

                MessageBox.Show(
                    "Không thể chọn bàn:\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );

                dangLoadDuLieu = true;

                try
                {
                    cboChonBan.SelectedValue = maBanCu;
                }
                finally
                {
                    dangLoadDuLieu = false;
                }
            }
        }
        // 18. SỰ KIỆN CLICK DATAGRIDVIEW
        private void dgvHienMonAn_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvHienMonAn.CurrentRow == null ||
                    dgvHienMonAn.CurrentRow.IsNewRow)
                    return;

                object giaTri = dgvHienMonAn.CurrentRow
                    .Cells["MaMonAn"].Value;

                if (giaTri == null || giaTri == DBNull.Value)
                    return;

                int maMonAn = Convert.ToInt32(giaTri);

                MonAn mon = danhSachMonAn.FirstOrDefault(
                    m => m.MaMonAn == maMonAn
                );

                if (mon == null)
                    return;

                dangChonMonTuDataGridView = true;
                monDangChon = mon;

                lblTenMonChon.Text = mon.TenMonAn;
                lblGiaMonChon.Text = mon.DonGia.ToString("N0") + " đ";

                // Lấy số lượng đang hiển thị trong DataGridView
                int soLuong = Convert.ToInt32(
                    dgvHienMonAn.CurrentRow.Cells["SoLuong"].Value
                );

                nudSoLuong.Minimum = 1;
                nudSoLuong.Maximum = 100;
                nudSoLuong.Value = Math.Min(
                    Math.Max(soLuong, 1),
                    (int)nudSoLuong.Maximum
                );

                HienThiHinhMon(mon.HinhMon);

                btnThemVaoDon.Text = "Cập nhật số lượng";
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể hiển thị thông tin món ăn:\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
 
// 19. HIỂN THỊ HÌNH MÓN ĂN
private void HienThiHinhMon(string duongDan)
        {
            try
            {
                // Giải phóng ảnh cũ
                if (picHinhMonDuocChon.Image != null)
                {
                    Image anhCu = picHinhMonDuocChon.Image;
                    picHinhMonDuocChon.Image = null;
                    anhCu.Dispose();
                }

                picHinhMonDuocChon.SizeMode =
                    PictureBoxSizeMode.Zoom;

                if (string.IsNullOrWhiteSpace(duongDan))
                    return;

                // Chỉ lấy tên file, không phụ thuộc đường dẫn cũ
                string tenFile = Path.GetFileName(duongDan);

                // Đường dẫn ảnh trong thư mục HinhAnh
                string path = Path.Combine(
                    Application.StartupPath,
                    "HinhAnh",
                    tenFile
                );

                // Kiểm tra ảnh có tồn tại không
                if (!File.Exists(path))
                {
                    System.Diagnostics.Debug.WriteLine(
                        "Không tìm thấy ảnh: " + path
                    );
                    return;
                }

                // Đọc ảnh và giải phóng file sau khi tải
                using (Image img = Image.FromFile(path))
                {
                    picHinhMonDuocChon.Image = new Bitmap(img);
                }
            }
            catch (Exception ex)
            {
                picHinhMonDuocChon.Image = null;

                MessageBox.Show(
                    "Không thể tải hình món ăn:\n" + ex.Message,
                    "Lỗi hình ảnh",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
        }

        // 21. GIẢI PHÓNG HÌNH ẢNH KHI ĐÓNG FORM
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (picHinhMonDuocChon.Image != null)
            {
                picHinhMonDuocChon.Image.Dispose();
                picHinhMonDuocChon.Image = null;
            }

            base.OnFormClosed(e);
        }

        private void CapNhatThongTinDonGoi()
        {
            BanItem ban = cboChonBan.SelectedItem as BanItem;

            if (ban != null)
                lblBanSo.Text = "🪑 Bàn: " + ban.SoBan;
            else
                lblBanSo.Text = "🪑 Bàn: --";

            lblTenNhanVien.Text = "👤 Nhân viên: " + tenNhanVien;

            DateTime hienTai = DateTime.Now;

            lblThoiGian.Text = "🕒 " + hienTai.ToString("HH:mm:ss");
            lblNgayHienTai.Text = "📅 " + hienTai.ToString("dd/MM/yyyy");

            label3.Text = "📝 Ghi chú:";
        }
        private void Uc_OnChonMon(MonAn mon)
        {
            if (mon == null)
                return;

            dangChonMonTuDataGridView = false;
            monDangChon = mon;

            lblTenMonChon.Text = mon.TenMonAn;
            lblGiaMonChon.Text = mon.DonGia.ToString("N0") + " đ";

            nudSoLuong.Minimum = 1;
            nudSoLuong.Maximum = 100;
            nudSoLuong.Value = 1;

            HienThiHinhMon(mon.HinhMon);

            btnThemVaoDon.Text = "Thêm vào đơn";
        }


        private void btnInHoaDon_Click(object sender, EventArgs e)
        {
            try
            {
                if (!maNhanVien.HasValue || maNhanVien.Value <= 0)
                {
                    MessageBox.Show("Không xác định được mã nhân viên!");
                    return;
                }

                if (maBan <= 0)
                {
                    MessageBox.Show("Vui lòng chọn bàn!");
                    return;
                }

                DialogResult xacNhan = MessageBox.Show(
                    "Bạn có chắc muốn hoàn tất đơn gọi món của bàn này?\n\n" +
                    "Hệ thống sẽ lập hóa đơn và kết thúc phục vụ tại bàn.",
                    "Xác nhận lập hóa đơn",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (xacNhan != DialogResult.Yes)
                    return;

                // 1. Lưu những món còn trong giỏ hàng
                if (gioHang.Count > 0)
                {
                    phieuGoiMonService.LuuGoiMon(
                        maBan,
                        maNhanVien.Value,
                        gioHang,
                        maDatBan
                    );

                    gioHang.Clear();
                }

                // 2. Lấy phiếu gọi món đang mở
                int maPhieuGoiMon =
                    phieuGoiMonService.GetMaPhieuDangMo(
                        maBan,
                        maDatBan
                    );

                if (maPhieuGoiMon <= 0)
                {
                    MessageBox.Show(
                        "Bàn này chưa có phiếu gọi món hoặc phiếu đã được lập hóa đơn!"
                    );
                    return;
                }

                // 3. Lập hóa đơn thông qua HoaDonService
                HoaDonService hoaDonService = new HoaDonService();

                int maHoaDon = hoaDonService.LapHoaDon(
                    maPhieuGoiMon,
                    maNhanVien.Value
                );

                // 4. Xóa dữ liệu món trên giao diện
                monDaLuu.Clear();
                gioHang.Clear();

                dgvHienMonAn.DataSource = null;
                lblTongTien.Text = "0 VNĐ";

                monDangChon = null;

                lblTenMonChon.Text = "";
                lblGiaMonChon.Text = "";
                nudSoLuong.Value = 1;

                HienThiHinhMon(null);

                btnThemVaoDon.Text = "Thêm vào đơn";

                // 5. Tải lại danh sách bàn
                dangLoadDuLieu = true;

                try
                {
                    LoadDanhSachBan();
                }
                finally
                {
                    dangLoadDuLieu = false;
                }

                // 6. Cập nhật thông tin đơn gọi món
                CapNhatThongTinDonGoi();

                MessageBox.Show(
                    "Lập hóa đơn thành công!\n\n" +
                    "Mã hóa đơn: " + maHoaDon + "\n" +
                    "Mã phiếu gọi món: " + maPhieuGoiMon + "\n" +
                    "Trạng thái: Chưa thanh toán\n" +
                    "Bàn đã được giải phóng.",
                    "Thành công",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (Exception ex)
            {
                dangLoadDuLieu = false;

                MessageBox.Show(
                    "Không thể lập hóa đơn:\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }


        private void LoadMonDaLuuTheoBan()
        {
            // Xóa dữ liệu món của bàn trước
            monDaLuu.Clear();

            // Lấy mã phiếu đang mở thông qua Service
            int maPhieu = phieuGoiMonService.GetMaPhieuDangMo(
                maBan,
                maDatBan
            );

            if (maPhieu > 0)
            {
                Dictionary<int, int> danhSach =
                    phieuGoiMonService.GetChiTietPhieuGoiMon(
                        maPhieu
                    );

                foreach (var item in danhSach)
                {
                    monDaLuu[item.Key] = item.Value;
                }
            }

            HienThiGioHang();
        }

        private void CapNhatThongTinDatBanTheoBan(int idBan)
        {
            // Dùng biến tạm để không làm mất thông tin cũ
            // nếu quá trình tìm đặt bàn xảy ra lỗi.
            int? maDatBanMoi = null;
            int? maKhachHangMoi = null;

            using (SqlConnection conn =
                new SqlConnection(connectionString))
            {
                conn.Open();

                // Trường hợp form được mở từ một lượt đặt bàn cụ thể
                if (maDatBanMacDinh.HasValue)
                {
                    string sql = @"
                SELECT
                    db.maDatBan,
                    db.maKhachHang
                FROM dbo.DatBan db
                INNER JOIN dbo.ChiTietDatBan ct
                    ON db.maDatBan = ct.maDatBan
                WHERE db.maDatBan = @maDatBan
                  AND ct.maBan = @maBan
                  AND db.trangThai IN
                  (
                      N'Đã đặt',
                      N'Đang phục vụ'
                  )";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add(
                            "@maDatBan",
                            SqlDbType.Int).Value = maDatBanMacDinh.Value;

                        cmd.Parameters.Add(
                            "@maBan",
                            SqlDbType.Int).Value = idBan;

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                maDatBanMoi =
                                    Convert.ToInt32(reader["maDatBan"]);

                                maKhachHangMoi =
                                    Convert.ToInt32(reader["maKhachHang"]);
                            }
                            else
                            {
                                throw new Exception(
                                    "Bàn không thuộc lượt đặt bàn hợp lệ."
                                );
                            }
                        }
                    }
                }
                else
                {
                    // Nhân viên tự chọn bàn:
                    // tìm xem bàn có thuộc một lượt đặt bàn đang hiệu lực không.
                    ChiTietDatBanRepository repository =
                        new ChiTietDatBanRepository();

                    var thongTin =
                        repository.LayThongTinDatBanTheoMaBan(
                            idBan,
                            conn
                        );

                    if (thongTin != null)
                    {
                        maDatBanMoi = thongTin.Item1;
                        maKhachHangMoi = thongTin.Item2;
                    }
                }
            }

            // Chỉ cập nhật thông tin sau khi truy vấn thành công.
            maDatBan = maDatBanMoi;
            maKhachHang = maKhachHangMoi;
        }
    }
}