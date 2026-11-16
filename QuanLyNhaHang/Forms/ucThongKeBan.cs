
using System;
using System.ComponentModel;
using System.Windows.Forms;
using QuanLyNhaHang.Models;
using QuanLyNhaHang.Services;

namespace QuanLyNhaHang.Forms
{
    public partial class UserControl1 : UserControl
    {
        private ThongKeService thongKeService;

        public UserControl1()
        {
            InitializeComponent();

            // Nạp icon cho giao diện
            LoadIcon();

            // Không tải dữ liệu khi đang thiết kế giao diện
            if (IsInDesignMode())
                return;
        }

        // Kiểm tra UserControl có đang được mở trong Designer không
        private bool IsInDesignMode()
        {
            return LicenseManager.UsageMode == LicenseUsageMode.Designtime
                   || DesignMode;
        }

        // Nạp hình ảnh
        private void LoadIcon()
        {
            picBanTrong.Image = Properties.Resources.iconBanTrong;
            picDangPhucVu.Image = Properties.Resources.iconDangPhucVu;
            picBanDaDat.Image = Properties.Resources.iconBanDaDat;

            picBanTrong.SizeMode = PictureBoxSizeMode.Zoom;
            picDangPhucVu.SizeMode = PictureBoxSizeMode.Zoom;
            picBanDaDat.SizeMode = PictureBoxSizeMode.Zoom;
        }

        // Lấy dữ liệu thống kê từ SQL Server và hiển thị lên giao diện
        private void LoadThongKe()
        {
            try
            {
                if (thongKeService == null)
                    thongKeService = new ThongKeService();

                ThongKeBan data = thongKeService.GetThongKeBan();

                if (data == null)
                    return;

                lblSoBanTrong.Text = data.SoBanTrong.ToString("00");
                lblBanDangPhucVu.Text = data.SoBanDangPhucVu.ToString("00");
                lblSoBanDaDat.Text = data.SoBanDaDat.ToString("00");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể tải dữ liệu thống kê bàn.\n" + ex.Message,
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        public void RefreshThongKe()
        {
            LoadThongKe();
        }
    }
}
