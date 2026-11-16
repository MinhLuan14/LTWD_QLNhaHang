
using QuanLyNhaHang.Properties;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace QuanLyNhaHang.Forms
{
    public partial class ucBan : UserControl
    {
        public event EventHandler BanClicked;

        private bool _isSelected;

        public ucBan()
        {
            InitializeComponent();

            picBan.Image = Resources.table;
            picBan.SizeMode = PictureBoxSizeMode.Zoom;

            this.Click += ucBan_Click;
            picBan.Click += ucBan_Click;
            lblTenBan.Click += ucBan_Click;
            lblSoChoNgoi.Click += ucBan_Click;
            lblTrangThai.Click += ucBan_Click;

            CapNhatMau();
        }

        private void ucBan_Click(object sender, EventArgs e)
        {
            BanClicked?.Invoke(this, EventArgs.Empty);
        }

        public int MaBan { get; set; }

        public string TenBan
        {
            get => lblTenBan.Text;
            set => lblTenBan.Text = value;
        }

        public int SoBan { get; set; }

        private int _soChoNgoi;

        public int SoChoNgoi
        {
            get => _soChoNgoi;
            set
            {
                _soChoNgoi = value;
                lblSoChoNgoi.Text = value + " chỗ";
            }
        }

        private string _trangThai;

        public string TrangThai
        {
            get => _trangThai;
            set
            {
                _trangThai = value;
                lblTrangThai.Text = value;
                CapNhatMau();
            }
        }

        public bool IsSelected
        {
            get => _isSelected;
        }

        // Đánh dấu chọn hoặc bỏ chọn bàn
        public void SetSelected(bool selected)
        {
            _isSelected = selected;
            CapNhatMau();
        }

        // Cập nhật màu theo trạng thái và lựa chọn
        private void CapNhatMau()
        {
            if (_isSelected)
            {
                this.BackColor = Color.Orange;
                lblTrangThai.ForeColor = Color.DarkRed;
                this.BorderStyle = BorderStyle.Fixed3D;
                return;
            }

            this.BorderStyle = BorderStyle.FixedSingle;

            switch (_trangThai)
            {
                case "Trống":
                    lblTrangThai.ForeColor = Color.Green;
                    this.BackColor = Color.LightGreen;
                    break;

                case "Đã đặt":
                case "Đặt trước":
                    lblTrangThai.ForeColor = Color.DarkOrange;
                    this.BackColor = Color.Khaki;
                    break;

                case "Đang phục vụ":
                case "Có khách":
                    lblTrangThai.ForeColor = Color.Red;
                    this.BackColor = Color.LightCoral;
                    break;

                default:
                    lblTrangThai.ForeColor = Color.Black;
                    this.BackColor = SystemColors.Control;
                    break;
            }
        }
    }
}
