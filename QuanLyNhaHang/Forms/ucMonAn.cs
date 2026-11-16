using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using QuanLyNhaHang.Models;
using System.Drawing.Drawing2D;
namespace QuanLyNhaHang
{
    public partial class ucMonAn : UserControl
    {
        public int MaMonAn { get; private set; }

        private MonAn monDangHienThi;

        // Sự kiện khi chọn thẻ món
        public event Action<MonAn> OnChonMon;

        // Sự kiện khi bấm nút Thêm Món trên thẻ
        public event Action<int> OnThemMon;
        // Tạo vùng bo góc
        private Region TaoVungBoGoc(Control control, int banKinh)
        {
            int w = control.Width;
            int h = control.Height;

            if (w <= 0 || h <= 0)
                return null;

            int r = Math.Min(banKinh, Math.Min(w, h) / 2);

            GraphicsPath path = new GraphicsPath();

            path.AddArc(0, 0, r * 2, r * 2, 180, 90);
            path.AddArc(w - r * 2, 0, r * 2, r * 2, 270, 90);
            path.AddArc(w - r * 2, h - r * 2, r * 2, r * 2, 0, 90);
            path.AddArc(0, h - r * 2, r * 2, r * 2, 90, 90);

            path.CloseFigure();

            Region region = new Region(path);
            path.Dispose();

            return region;
        }

        // Cập nhật vùng bo góc
        private void CapNhatBoGoc(Control control, int banKinh)
        {
            Region vungCu = control.Region;

            control.Region = TaoVungBoGoc(control, banKinh);

            if (vungCu != null)
                vungCu.Dispose();
        }
        public ucMonAn()
        {
            InitializeComponent();

            this.Click += ChonMon_Click;
            panel1.Click += ChonMon_Click;
            picHinhMon.Click += ChonMon_Click;
            lblTenMon.Click += ChonMon_Click;
            lblDonGia.Click += ChonMon_Click;
            lblTrangThai.Click += ChonMon_Click;

            // Tự động bo góc khi kích thước thay đổi
            panel1.Resize += (s, e) =>
                CapNhatBoGoc(panel1, 15);

            lblTrangThai.Resize += (s, e) =>
                CapNhatBoGoc(lblTrangThai, 12);
        }

        public void HienThiMon(MonAn mon)
        {
            if (mon == null)
                return;

            monDangHienThi = mon;
            MaMonAn = mon.MaMonAn;

            lblTenMon.Text = mon.TenMonAn;
            lblDonGia.Text = mon.DonGia.ToString("N0") + " đ";

            lblTrangThai.Text = mon.TrangThai
                ? "Đang bán"
                : "Ngừng bán";

            if (mon.TrangThai)
            {
                lblTrangThai.BackColor = Color.FromArgb(220, 245, 230);
                lblTrangThai.ForeColor = Color.FromArgb(25, 135, 75);
            }
            else
            {
                lblTrangThai.BackColor = Color.FromArgb(255, 225, 225);
                lblTrangThai.ForeColor = Color.FromArgb(200, 50, 50);
            }

            btnThemMon.Enabled = mon.TrangThai;

            picHinhMon.SizeMode = PictureBoxSizeMode.Zoom;

            // Giải phóng ảnh cũ
            if (picHinhMon.Image != null)
            {
                Image anhCu = picHinhMon.Image;
                picHinhMon.Image = null;
                anhCu.Dispose();
            }

            string tenHinh = mon.HinhMon;

            string duongDan = string.IsNullOrWhiteSpace(tenHinh)
                ? ""
                : Path.Combine(
                    Application.StartupPath,
                    "HinhAnh",
                    Path.GetFileName(tenHinh)
                );

         

            if (File.Exists(duongDan))
            {
                using (Image img = Image.FromFile(duongDan))
                {
                    picHinhMon.Image = new Bitmap(img);
                }
            }
            else
            {
                picHinhMon.Image = null;

                System.Diagnostics.Debug.WriteLine(
                    "Không tìm thấy ảnh món ăn: " + duongDan
                );
            }
        }

        private void ChonMon_Click(object sender, EventArgs e)
        {
            if (monDangHienThi == null)
                return;

            OnChonMon?.Invoke(monDangHienThi);
        }

        private void btnThemMon_Click(object sender, EventArgs e)
        {
            if (monDangHienThi == null || !monDangHienThi.TrangThai)
                return;

            OnThemMon?.Invoke(MaMonAn);
        }

        private void lblTrangThai_Click(object sender, EventArgs e)
        {

        }
    }
}