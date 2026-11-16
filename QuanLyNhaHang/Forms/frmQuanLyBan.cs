
using Krypton.Toolkit;
using QuanLyNhaHang.Forms;
using QuanLyNhaHang.Models;
using QuanLyNhaHang.Repositories;
using QuanLyNhaHang.Services;
using QuanLyNhaHang.Data;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace QuanLyNhaHang
{
    public partial class frmQuanLyBan : Form
    {
        private readonly string tenNhanVien;

        // Bàn đang được chọn
        private int? banDangChon;
        private ucBan ucBanDangChon;

        // UserControl thống kê
        private UserControl1 ucThongKeBan;

        // Danh sách bàn
        private readonly List<int> danhSachMaBanDangChon =
            new List<int>();

        private readonly List<ucBan> danhSachUCBanDangChon =
            new List<ucBan>();

        // Danh sách bàn gốc lưu trong bộ nhớ
        private List<Ban> danhSachBanGoc =
            new List<Ban>();

        // Service và Repository
        private readonly BanService banService =
            new BanService();

        private readonly DatBanService datBanService =
            new DatBanService();

        private readonly ChiTietDatBanRepository chiTietDatBanRepository =
            new ChiTietDatBanRepository();

        // Trạng thái khởi tạo
        private bool dangKhoiTao = true;

        // Chống Load nhiều lần trên cùng một instance
        private bool daKhoiTao = false;


        public frmQuanLyBan(string tenNguoiDung)
        {
            InitializeComponent();

            tenNhanVien = tenNguoiDung;

            txtSoLuongKhach.Minimum = 1;
            txtSoLuongKhach.Value = 4;

            // Đăng ký sự kiện tại đây.
            // Kiểm tra Designer để tránh đăng ký trùng.
            txtSoLuongKhach.ValueChanged +=
                txtSoLuongKhach_ValueChanged;

            btnDatBan.Click += btnDatBan_Click;

            // Chỉ đăng ký Load ở một nơi.
            Load += frmQuanLyBan_Load;
        }

        // 1. KHỞI TẠO FORM
        private void frmQuanLyBan_Load(object sender, EventArgs e)
        {
            // Nếu đã khởi tạo rồi thì thoát ngay lập tức
            if (daKhoiTao)
                return;

            // Đánh dấu ngay lập tức trước khi làm bất cứ việc gì tốn thời gian
            daKhoiTao = true;
            dangKhoiTao = true;

            Stopwatch sw = Stopwatch.StartNew();

            try
            {
                // Khởi tạo bộ lọc trước
                cboTrangThai.BeginUpdate();
                try
                {
                    cboTrangThai.Items.Clear();
                    cboTrangThai.Items.Add("Tất cả");
                    cboTrangThai.Items.Add("Trống");
                    cboTrangThai.Items.Add("Đã đặt");
                    cboTrangThai.Items.Add("Đang phục vụ");

                    cboTrangThai.SelectedIndex = 0;
                }
                finally
                {
                    cboTrangThai.EndUpdate();
                }

                Debug.WriteLine("1. Khởi tạo bộ lọc: " + sw.ElapsedMilliseconds + " ms");
                sw.Restart();

                // Tải danh sách bàn từ SQL Server đúng một lần
                TaiLaiDanhSachBan();
                Debug.WriteLine("2. Tải danh sách bàn: " + sw.ElapsedMilliseconds + " ms");
                sw.Restart();

                // Khởi tạo thống kê
                KhoiTaoThongKe();
                Debug.WriteLine("3. Tải thống kê: " + sw.ElapsedMilliseconds + " ms");
                sw.Restart();

                // Hoàn tất khởi tạo
                dangKhoiTao = false;

                // Gợi ý bàn một lần
                GoiYBanTheoSoLuongKhach();
                Debug.WriteLine("4. Gợi ý bàn: " + sw.ElapsedMilliseconds + " ms");

                Debug.WriteLine("===== HOÀN TẤT KHỞI TẠO FORM =====");
            }
            catch (Exception ex)
            {
                // Nếu lỗi thì reset lại cờ để có thể thử lại nếu cần
                daKhoiTao = false;
                dangKhoiTao = false;

                MessageBox.Show(
                    "Không thể khởi tạo quản lý bàn:\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
        // 2. THỐNG KÊ BÀN
        private void KhoiTaoThongKe()
        {
            ucThongKeBan = TimUserControlThongKe(this);

            if (ucThongKeBan == null)
            {
                MessageBox.Show(
                    "Không tìm thấy UserControl thống kê bàn!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            ucThongKeBan.RefreshThongKe();
        }

        private UserControl1 TimUserControlThongKe(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                if (control is UserControl1 uc)
                    return uc;

                UserControl1 result =
                    TimUserControlThongKe(control);

                if (result != null)
                    return result;
            }

            return null;
        }

        private void CapNhatThongKe()
        {
            if (ucThongKeBan != null &&
                !ucThongKeBan.IsDisposed)
            {
                ucThongKeBan.RefreshThongKe();
            }
        }
        // 3. TẢI VÀ HIỂN THỊ DANH SÁCH BÀN
        // Chỉ phương thức này truy vấn dữ liệu bàn từ SQL
        private void TaiLaiDanhSachBan()
        {
            danhSachBanGoc = banService.GetAll();

            ApDungBoLocVaTimKiem();
        }

        // Tải lại dữ liệu bàn sau khi thêm, sửa, đặt hoặc hủy
        private void LoadBan()
        {
            TaiLaiDanhSachBan();
        }

        // Hiển thị danh sách bàn
        private void LoadBan(List<Ban> dsBan)
        {
            flpDanhSachBan.SuspendLayout();

            try
            {
                // Xóa các UserControl cũ
                flpDanhSachBan.Controls.Clear();

                // Xóa lựa chọn cũ
                danhSachMaBanDangChon.Clear();
                danhSachUCBanDangChon.Clear();

                banDangChon = null;
                ucBanDangChon = null;

                foreach (Ban table in dsBan)
                {
                    ucBan ban = new ucBan();

                    ban.MaBan = table.MaBan;
                    ban.SoBan = table.SoBan;
                    ban.TenBan = $"Bàn {table.SoBan}";
                    ban.SoChoNgoi = table.SoChoNgoi;
                    ban.TrangThai = table.TrangThai;

                    ban.BanClicked += Ban_BanClicked;

                    flpDanhSachBan.Controls.Add(ban);
                }

                CapNhatThongTinBanDangChon();
            }
            finally
            {
                flpDanhSachBan.ResumeLayout(true);
            }
        }

        // 4. CHỌN HOẶC BỎ CHỌN BÀN
        private void Ban_BanClicked(object sender, EventArgs e)
        {
            ucBan ban = sender as ucBan;

            if (ban == null)
                return;

            // Bàn đã đặt
            if (ban.TrangThai == "Đã đặt")
            {
                MoFormChiTietDatBan(ban.MaBan);
                return;
            }

            // Bàn đang phục vụ: chỉ chọn một bàn
            if (ban.TrangThai == "Đang phục vụ")
            {
                if (danhSachMaBanDangChon.Contains(ban.MaBan))
                {
                    ban.SetSelected(false);

                    danhSachMaBanDangChon.Clear();
                    danhSachUCBanDangChon.Clear();
                }
                else
                {
                    foreach (ucBan item in danhSachUCBanDangChon)
                    {
                        item.SetSelected(false);
                    }

                    danhSachMaBanDangChon.Clear();
                    danhSachUCBanDangChon.Clear();

                    danhSachMaBanDangChon.Add(ban.MaBan);
                    danhSachUCBanDangChon.Add(ban);

                    ban.SetSelected(true);
                }

                CapNhatThongTinBanDangChon();
                return;
            }

            // Chỉ cho phép chọn nhiều bàn trống
            if (ban.TrangThai != "Trống")
                return;

            if (danhSachMaBanDangChon.Contains(ban.MaBan))
            {
                danhSachMaBanDangChon.Remove(ban.MaBan);
                danhSachUCBanDangChon.Remove(ban);

                ban.SetSelected(false);
            }
            else
            {
                danhSachMaBanDangChon.Add(ban.MaBan);
                danhSachUCBanDangChon.Add(ban);

                ban.SetSelected(true);
            }

            CapNhatThongTinBanDangChon();
        }

        // 5. CẬP NHẬT THÔNG TIN BÀN ĐANG CHỌN
        private void CapNhatThongTinBanDangChon()
        {
            if (danhSachUCBanDangChon.Count == 0)
            {
                lblTenBan.Text = "Chưa chọn bàn";
                lblTrangThai.Text = "";
                lblSoCho.Text = "";

                banDangChon = null;
                ucBanDangChon = null;

                return;
            }

            string tenCacBan = string.Join(
                ", ",
                danhSachUCBanDangChon.Select(x => x.TenBan));

            int tongSoCho = danhSachUCBanDangChon.Sum(
                x => x.SoChoNgoi);

            lblTenBan.Text = tenCacBan;

            lblTrangThai.Text =
                "Đang chọn " +
                danhSachUCBanDangChon.Count +
                " bàn";

            lblSoCho.Text =
                "Tổng sức chứa: " +
                tongSoCho +
                " chỗ";

            ucBanDangChon = danhSachUCBanDangChon[0];
            banDangChon = ucBanDangChon.MaBan;
        }

        // 6. GỢI Ý BÀN THEO SỐ LƯỢNG KHÁCH
        private void txtSoLuongKhach_ValueChanged(
            object sender,
            EventArgs e)
        {
            if (dangKhoiTao)
                return;

            GoiYBanTheoSoLuongKhach();
        }


        private void GoiYBanTheoSoLuongKhach()
        {
            int soLuongKhach = (int)txtSoLuongKhach.Value;

            // Bỏ chọn các bàn cũ
            foreach (ucBan ban in danhSachUCBanDangChon)
            {
                ban.SetSelected(false);
            }

            danhSachMaBanDangChon.Clear();
            danhSachUCBanDangChon.Clear();

            banDangChon = null;
            ucBanDangChon = null;

            // Lấy danh sách bàn trống
            List<ucBan> danhSachBanTrong =
                flpDanhSachBan.Controls
                    .OfType<ucBan>()
                    .Where(x =>
                        x.TrangThai == "Trống" &&
                        x.SoChoNgoi > 0)
                    .OrderBy(x => x.SoBan)
                    .ToList();

            if (danhSachBanTrong.Count == 0)
            {
                CapNhatThongTinBanDangChon();
                return;
            }

            // Tìm tổ hợp bàn tối ưu
            Dictionary<int, List<ucBan>> phuongAn =
                new Dictionary<int, List<ucBan>>();

            phuongAn[0] = new List<ucBan>();

            foreach (ucBan ban in danhSachBanTrong)
            {
                // Sao chép phương án hiện tại để tránh duyệt
                // Dictionary trong lúc đang thay đổi nó
                var cacPhuongAnHienTai = phuongAn.ToList();

                foreach (var item in cacPhuongAnHienTai)
                {
                    int sucChuaMoi = item.Key + ban.SoChoNgoi;

                    List<ucBan> nhomBanMoi =
                        new List<ucBan>(item.Value);

                    nhomBanMoi.Add(ban);

                    // Nếu chưa có tổng sức chứa này thì thêm mới
                    // Nếu đã có thì giữ phương án ít bàn hơn
                    if (!phuongAn.ContainsKey(sucChuaMoi) ||
                        nhomBanMoi.Count < phuongAn[sucChuaMoi].Count)
                    {
                        phuongAn[sucChuaMoi] = nhomBanMoi;
                    }
                    else if (nhomBanMoi.Count ==
                             phuongAn[sucChuaMoi].Count)
                    {
                        // Nếu số bàn bằng nhau, ưu tiên số bàn nhỏ hơn
                        var phuongAnCu = phuongAn[sucChuaMoi];

                        bool uuTienPhuongAnMoi =
                            nhomBanMoi
                                .Select(x => x.SoBan)
                                .OrderBy(x => x)
                                .SequenceEqual(
                                    nhomBanMoi.Select(x => x.SoBan)
                                        .OrderBy(x => x))
                            &&
                            nhomBanMoi
                                .Select(x => x.SoBan)
                                .OrderBy(x => x)
                                .First() <
                            phuongAnCu
                                .Select(x => x.SoBan)
                                .OrderBy(x => x)
                                .First();

                        if (uuTienPhuongAnMoi)
                        {
                            phuongAn[sucChuaMoi] = nhomBanMoi;
                        }
                    }
                }
            }

            // Chọn phương án có sức chứa nhỏ nhất nhưng đủ khách
            var phuongAnTotNhat = phuongAn
                .Where(x => x.Key >= soLuongKhach)
                .OrderBy(x => x.Key)
                .ThenBy(x => x.Value.Count)
                .ThenBy(x => x.Value.Min(b => b.SoBan))
                .FirstOrDefault();

            if (phuongAnTotNhat.Value == null)
            {
                CapNhatThongTinBanDangChon();

                MessageBox.Show(
                    "Hiện tại các bàn trống không đủ sức chứa cho " +
                    soLuongKhach + " khách!",
                    "Gợi ý đặt bàn",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }

            // Đánh dấu các bàn được gợi ý
            foreach (ucBan ban in phuongAnTotNhat.Value)
            {
                danhSachMaBanDangChon.Add(ban.MaBan);
                danhSachUCBanDangChon.Add(ban);

                ban.SetSelected(true);
            }

            CapNhatThongTinBanDangChon();
        }


        // 7. ĐẶT BÀN

        private void btnDatBan_Click(object sender, EventArgs e)
        {
            try
            {
                if (danhSachMaBanDangChon.Count == 0)
                {
                    MessageBox.Show(
                        "Vui lòng chọn ít nhất một bàn cần đặt!",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                string hoTen = txtTenKhachHang.Text.Trim();
                string soDienThoai = txtSoDienThoai.Text.Trim();
                string diaChi = txtDiaChi.Text.Trim();
                string ghiChu = txtGhiChu.Text.Trim();

                if (string.IsNullOrWhiteSpace(hoTen))
                {
                    MessageBox.Show("Vui lòng nhập họ và tên khách hàng!");
                    txtTenKhachHang.Focus();
                    return;
                }

                if (!System.Text.RegularExpressions.Regex.IsMatch(
                    soDienThoai,
                    @"^0\d{9}$"))
                {
                    MessageBox.Show(
                        "Số điện thoại phải gồm 10 chữ số và bắt đầu bằng số 0!");

                    txtSoDienThoai.Focus();
                    return;
                }

                int soLuongKhach = (int)txtSoLuongKhach.Value;

                if (soLuongKhach <= 0)
                {
                    MessageBox.Show("Số lượng khách phải lớn hơn 0!");
                    txtSoLuongKhach.Focus();
                    return;
                }

                if (!DateTime.TryParse(
                    txtNgayDat.Text.Trim(),
                    out DateTime ngayDat))
                {
                    MessageBox.Show("Ngày đặt không hợp lệ!");
                    txtNgayDat.Focus();
                    return;
                }

                TimeSpan gioDat = txtGioDat.Value.TimeOfDay;

                DateTime ngayGioDat =
                    ngayDat.Date.Add(gioDat);

                if (ngayGioDat < DateTime.Now)
                {
                    MessageBox.Show(
                        "Thời gian đặt bàn không được ở quá khứ!",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    txtNgayDat.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(diaChi))
                {
                    MessageBox.Show("Vui lòng nhập địa chỉ khách hàng!");
                    txtDiaChi.Focus();
                    return;
                }

                int tongSoCho = danhSachUCBanDangChon.Sum(
                    x => x.SoChoNgoi);

                if (tongSoCho < soLuongKhach)
                {
                    MessageBox.Show(
                        "Tổng sức chứa các bàn đang chọn không đủ cho số lượng khách!",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                bool result = datBanService.DatBan(
                    danhSachMaBanDangChon,
                    hoTen,
                    soDienThoai,
                    soLuongKhach,
                    ngayGioDat,
                    ghiChu,
                    diaChi);

                if (result)
                {
                    MessageBox.Show(
                        "Đặt bàn thành công!",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    LoadBan();
                    CapNhatThongKe();

                    txtTenKhachHang.Text = "";
                    txtSoDienThoai.Text = "";
                    txtDiaChi.Text = "";
                    txtGhiChu.Text = "";

                    txtNgayDat.Text = "";
                    txtGioDat.Value = DateTime.Now;

                    txtSoLuongKhach.Value = 4;

                    GoiYBanTheoSoLuongKhach();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }
        // 8. HỦY ĐẶT BÀN
        private void btnHuyDat_Click(object sender, EventArgs e)
        {
            if (!banDangChon.HasValue)
            {
                MessageBox.Show(
                    "Vui lòng chọn bàn cần hủy đặt!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            DialogResult result = MessageBox.Show(
                "Bạn có chắc chắn muốn hủy đặt bàn này không?",
                "Xác nhận hủy đặt bàn",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
                return;

            try
            {
                int maBan = banDangChon.Value;
                int? maDatBan;

                using (SqlConnection conn =
                    new DbConnection().GetConnection())
                {
                    conn.Open();

                    maDatBan =
                        chiTietDatBanRepository.LayMaDatBanTheoMaBan(
                            maBan,
                            conn);
                }

                if (!maDatBan.HasValue)
                {
                    MessageBox.Show(
                        "Không tìm thấy phiếu đặt bàn của bàn này!",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                datBanService.HuyDatBan(maDatBan.Value);

                MessageBox.Show(
                    "Hủy đặt bàn thành công!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                LoadBan();
                CapNhatThongKe();
                GoiYBanTheoSoLuongKhach();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
        // 9. THÊM BÀN
        private void btnThemBan_Click(object sender, EventArgs e)
        {
            frmThemBan frm = new frmThemBan();

            frm.BanDaThem += () =>
            {
                LoadBan();
                CapNhatThongKe();
                GoiYBanTheoSoLuongKhach();
            };

            frm.ShowDialog(this);
        }

        // 10. TÌM KIẾM VÀ LỌC BÀN
        private void ApDungBoLocVaTimKiem()
        {
            IEnumerable<Ban> dsBan = danhSachBanGoc;

            // Lọc trạng thái
            if (cboTrangThai.SelectedIndex >= 0 &&
                cboTrangThai.Text != "Tất cả")
            {
                dsBan = dsBan.Where(
                    x => x.TrangThai == cboTrangThai.Text);
            }

            // Tìm kiếm theo số bàn
            string keyword = txtTimKiem.Text.Trim();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                dsBan = dsBan.Where(
                    x => x.SoBan.ToString().Contains(keyword));
            }

            LoadBan(dsBan.ToList());
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            ApDungBoLocVaTimKiem();
            GoiYBanTheoSoLuongKhach();
        }

        private void cboTrangThai_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            // Không tải lại trong lúc khởi tạo
            if (dangKhoiTao ||
                cboTrangThai.SelectedIndex < 0)
            {
                return;
            }

            ApDungBoLocVaTimKiem();
            GoiYBanTheoSoLuongKhach();
        }

        // 11. SỬA BÀN

        private void btnSuaBan_Click(object sender, EventArgs e)
        {
            if (ucBanDangChon == null)
            {
                MessageBox.Show(
                    "Vui lòng chọn bàn cần sửa!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            frmSuaBan frm = new frmSuaBan(
                ucBanDangChon.MaBan,
                ucBanDangChon.TenBan,
                ucBanDangChon.SoChoNgoi,
                ucBanDangChon.TrangThai);

            DialogResult result = frm.ShowDialog(this);

            if (result == DialogResult.OK)
            {
                LoadBan();
                CapNhatThongKe();
                GoiYBanTheoSoLuongKhach();
            }
        }

        // 12. CHI TIẾT ĐẶT BÀN

        private void MoFormChiTietDatBan(int maBan)
        {
            try
            {
                int? maDatBan;

                using (SqlConnection conn =
                    new DbConnection().GetConnection())
                {
                    conn.Open();

                    maDatBan =
                        chiTietDatBanRepository.LayMaDatBanTheoMaBan(
                            maBan,
                            conn);
                }

                if (!maDatBan.HasValue)
                {
                    MessageBox.Show(
                        "Không tìm thấy thông tin phiếu đặt bàn!",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                using (frmChiTietDatBan frm =
                    new frmChiTietDatBan(maDatBan.Value))
                {
                    frm.ShowDialog(this);
                }

                LoadBan();
                CapNhatThongKe();
                GoiYBanTheoSoLuongKhach();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi khi mở chi tiết đặt bàn:\n" +
                    ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}
