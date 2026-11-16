
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using QuanLyNhaHang.Models;
using QuanLyNhaHang.Services;

namespace QuanLyNhaHang.Forms
{
    public partial class ucTrangChu : UserControl
    {
        private readonly DashboardTrangChuService dashboardService =
            new DashboardTrangChuService();

        public ucTrangChu()
        {
            InitializeComponent();

            KhoiTaoHinhAnh();
            KhoiTaoGiaoDien();
            KhoiTaoSuKienClick();
        }

        #region 1. KHỞI TẠO GIAO DIỆN

        // Khởi tạo màu sắc, hình ảnh, biểu đồ và bảng hóa đơn.
        private void KhoiTaoGiaoDien()
        {
            this.BackColor = Color.FromArgb(225, 245, 254);

            picDoanhThu.SizeMode = PictureBoxSizeMode.Zoom;
            picHoaDon.SizeMode = PictureBoxSizeMode.Zoom;
            picBanTrong.SizeMode = PictureBoxSizeMode.Zoom;
            picDangPhucVu.SizeMode = PictureBoxSizeMode.Zoom;

            picTruyCapNhanh.SizeMode = PictureBoxSizeMode.Zoom;
            picBan.SizeMode = PictureBoxSizeMode.Zoom;
            picCanXuLy.SizeMode = PictureBoxSizeMode.Zoom;

            KhoiTaoBieuDoDoanhThu();
            KhoiTaoBieuDoTinhTrangBan();
            KhoiTaoBangHoaDon();
        }

        // Gán hình ảnh từ Resources cho các PictureBox trên Dashboard.
        private void KhoiTaoHinhAnh()
        {
            picDoanhThu.Image = Properties.Resources.IconDoanhThu;
            picHoaDon.Image = Properties.Resources.iconHoaDon;
            picBanTrong.Image = Properties.Resources.iconBanTrong;
            picDangPhucVu.Image = Properties.Resources.iconDangPhucVu;

            picTruyCapNhanh.Image = Properties.Resources.grid;
            picBan.Image = Properties.Resources.iconTCQLDatBan;
            picCanXuLy.Image = Properties.Resources.alert_triangle;

            picXLHoaDon.Image = Properties.Resources.iconHD;
            pictureBox2.Image = Properties.Resources.iconPhieuGoiMont;
            pictureBox3.Image = Properties.Resources.iconcalendar_clock;

            picDatBan.Image = Properties.Resources.iconTCQLDatBan;
            picGoiMon.Image = Properties.Resources.Food_Order;
            picQLHoaDon.Image = Properties.Resources.iconHD;
            picThongKe.Image = Properties.Resources.Data_Analytics_Dashboard;
        }

        #endregion

        #region 2. KHỞI TẠO BIỂU ĐỒ

        // Khởi tạo biểu đồ đường thể hiện doanh thu theo từng giờ.
        private void KhoiTaoBieuDoDoanhThu()
        {
            ChartDoanhThu.Series.Clear();
            ChartDoanhThu.ChartAreas.Clear();
            ChartDoanhThu.Legends.Clear();

            ChartDoanhThu.BackColor = Color.White;

            ChartArea area = new ChartArea("ChartArea1");

            area.AxisX.MajorGrid.Enabled = false;
            // Thiết lập hiển thị trục thời gian.
            area.AxisX.Interval = 2;
            area.AxisX.LabelStyle.Angle = 0;
            area.AxisX.LabelStyle.Font =
                new Font("Segoe UI", 8);

            area.AxisX.MajorGrid.Enabled = false;

            area.AxisY.LabelStyle.Format = "N0";
            area.AxisY.Minimum = 0;
            area.AxisY.MajorGrid.LineColor =
                Color.FromArgb(229, 231, 235);

            area.AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dash;

            area.AxisX.LabelStyle.ForeColor =
                Color.FromArgb(100, 116, 139);

            area.AxisY.LabelStyle.ForeColor =
                Color.FromArgb(100, 116, 139);

            area.AxisX.LineColor =
                Color.FromArgb(203, 213, 225);

            area.AxisY.LineColor =
                Color.FromArgb(203, 213, 225);

            area.AxisX.Interval = 1;
            area.AxisY.LabelStyle.Format = "N0";

            ChartDoanhThu.ChartAreas.Add(area);

            Series series = new Series("DoanhThu");

            series.ChartArea = "ChartArea1";
            series.ChartType = SeriesChartType.Spline;
            series.BorderWidth = 3;
            series.Color = Color.FromArgb(37, 99, 235);

            series.MarkerStyle = MarkerStyle.Circle;
            series.MarkerSize = 7;
            series.MarkerColor = Color.FromArgb(242, 139, 0);

            series.IsVisibleInLegend = false;
            series.ToolTip = "#VALX: #VALY{N0} đ";

            ChartDoanhThu.Series.Add(series);
        }

        // Khởi tạo biểu đồ vòng thể hiện tình trạng các bàn ăn.
        private void KhoiTaoBieuDoTinhTrangBan()
        {
            cTinhTrangBan.Series.Clear();
            cTinhTrangBan.ChartAreas.Clear();
            cTinhTrangBan.Legends.Clear();

            cTinhTrangBan.BackColor = Color.FromArgb(239, 246, 255);

            var area = new System.Windows.Forms.DataVisualization.Charting.ChartArea("ChartArea1");
            area.BackColor = Color.FromArgb(239, 246, 255);

            cTinhTrangBan.ChartAreas.Add(area);

            var series = new System.Windows.Forms.DataVisualization.Charting.Series("TinhTrangBan");

            series.ChartArea = "ChartArea1";
            series.ChartType =
                System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Doughnut;

            series.IsValueShownAsLabel = false;
            series.IsVisibleInLegend = false;
            series["DoughnutRadius"] = "65";

            cTinhTrangBan.Series.Add(series);
        }

        #endregion

        #region 3. KHỞI TẠO BẢNG HÓA ĐƠN

        // Thiết lập thuộc tính hiển thị cho bảng hóa đơn gần đây.
        private void KhoiTaoBangHoaDon()
        {
            dgvHoaDonGanDay.ReadOnly = true;
            dgvHoaDonGanDay.AllowUserToAddRows = false;
            dgvHoaDonGanDay.AllowUserToDeleteRows = false;
            dgvHoaDonGanDay.MultiSelect = false;

            dgvHoaDonGanDay.AutoGenerateColumns = false;

            dgvHoaDonGanDay.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            dgvHoaDonGanDay.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            dgvHoaDonGanDay.RowHeadersVisible = false;

            // Liên kết dữ liệu với các cột đã tạo trong Designer.
            colMaHoaDon.DataPropertyName = "MaHoaDon";
            colBan.DataPropertyName = "Ban";
            colThoiGian.DataPropertyName = "ThoiGian";
            colTongTien.DataPropertyName = "TongTien";
            colTrangThai.DataPropertyName = "TrangThai";

            colMaHoaDon.HeaderText = "Mã HĐ";
            colBan.HeaderText = "Bàn";
            colThoiGian.HeaderText = "Thời gian";
            colTongTien.HeaderText = "Tổng tiền";
            colTrangThai.HeaderText = "Trạng thái";

            colTongTien.DefaultCellStyle.Format = "N0";
        }

        #endregion

        #region 4. TẢI DỮ LIỆU DASHBOARD

        // Tải toàn bộ dữ liệu Dashboard theo ngày hiện tại.
        public void TaiDuLieuDashboard()
        {
            try
            {
                ThongKeTrangChuDashboard data =
                    dashboardService.LayDuLieuDashboard(DateTime.Today);

                if (data == null)
                    return;

                HienThiThongKe(data);
                HienThiHoaDonGanDay(data.HoaDonGanDay);
                HienThiBieuDoDoanhThu(data.DoanhThuTheoGio);
                HienThiTinhTrangBan(data.TinhTrangBan);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể tải dữ liệu trang chủ!\n\n" + ex.Message,
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // Hiển thị các chỉ số tổng quan và số lượng công việc cần xử lý.
        private void HienThiThongKe(ThongKeTrangChuDashboard data)
        {
            lblTongDoanhThu.Text =
                data.TongDoanhThu.ToString("N0") + " đ";

            lblTongHoaDon.Text =
                data.TongHoaDon.ToString("N0");

            lblTongBanTrong.Text =
                data.BanTrong.ToString("N0");

            lblDangPhucVu.Text =
                data.DangPhucVu.ToString("N0");

            // Hiển thị số hóa đơn chưa thanh toán.
            lblXLSoLuongHoaDon.Text =
                data.HoaDonCanXuLy.ToString("N0") +
                " Hóa đơn chưa thanh toán";

            // Hiển thị số phiếu gọi món đang chờ xử lý.
            lblXLPhieuGoiMon.Text =
                data.PhieuGoiMonCanXuLy.ToString("N0") +
                " Phiếu gọi món đang chờ xử lý";

            // Hiển thị số bàn sắp đến giờ.
            lblXLDatban.Text =
                data.DatBanCanXuLy.ToString("N0") +
                " bàn sắp đến giờ";

            lblSoViec.Text =
                data.TongViecCanXuLy.ToString("N0");
        }

        // Hiển thị danh sách hóa đơn gần đây lên DataGridView.
        private void HienThiHoaDonGanDay(DataTable dt)
        {
            dgvHoaDonGanDay.DataSource = null;

            if (dt == null)
                return;

            dgvHoaDonGanDay.DataSource = dt;
        }

        // Hiển thị biểu đồ doanh thu theo từng giờ trong ngày.

        // Hiển thị biểu đồ doanh thu đủ 24 giờ trong ngày.
        private void HienThiBieuDoDoanhThu(DataTable dt)
        {
            Series series = ChartDoanhThu.Series["DoanhThu"];

            series.Points.Clear();

            // Khởi tạo doanh thu mặc định bằng 0 cho 24 giờ.
            decimal[] doanhThuTheoGio = new decimal[24];

            if (dt != null)
            {
                foreach (DataRow row in dt.Rows)
                {
                    int gio = Convert.ToInt32(row["Gio"]);

                    decimal doanhThu =
                        Convert.ToDecimal(row["DoanhThu"]);

                    if (gio >= 0 && gio <= 23)
                    {
                        doanhThuTheoGio[gio] = doanhThu;
                    }
                }
            }

            // Thêm dữ liệu của 24 giờ vào biểu đồ.
            for (int gio = 0; gio < 24; gio++)
            {
                int index = series.Points.AddXY(
                    gio.ToString("00") + ":00",
                    doanhThuTheoGio[gio]);

                series.Points[index].ToolTip =
                    gio.ToString("00") + ":00 - " +
                    doanhThuTheoGio[gio].ToString("N0") + " đ";

                // Làm nổi bật những giờ có doanh thu.
                // Chỉ hiển thị điểm đánh dấu khi có doanh thu.
                if (doanhThuTheoGio[gio] > 0)
                {
                    series.Points[index].MarkerStyle = MarkerStyle.Circle;
                    series.Points[index].MarkerSize = 8;
                    series.Points[index].MarkerColor = Color.FromArgb(242, 139, 0);
                }
                else
                {
                    series.Points[index].MarkerStyle = MarkerStyle.None;
                }
            }
        }


        // Hiển thị biểu đồ vòng thể hiện số lượng bàn theo trạng thái.

        // Hiển thị biểu đồ tình trạng bàn và không để nhãn đè lên biểu đồ.
        private void HienThiTinhTrangBan(DataTable dt)
        {
            Series series = cTinhTrangBan.Series["TinhTrangBan"];

            series.Points.Clear();

            // Không hiển thị nhãn trực tiếp trên biểu đồ.
            series.IsValueShownAsLabel = false;
            series.IsVisibleInLegend = false;

            if (dt == null || dt.Rows.Count == 0)
                return;

            foreach (DataRow row in dt.Rows)
            {
                string trangThai = row["TrangThai"].ToString();

                int soLuong = Convert.ToInt32(row["SoLuong"]);

                int index = series.Points.AddXY(
                    trangThai,
                    soLuong);

                DataPoint point = series.Points[index];

                // Không hiển thị chữ trên từng phần của biểu đồ.
                point.Label = string.Empty;
                point.LegendText = trangThai;

                // Hiển thị thông tin khi rê chuột vào biểu đồ.
                point.ToolTip = trangThai + ": " + soLuong + " bàn";

                // Thiết lập màu sắc theo trạng thái bàn.
                switch (trangThai)
                {
                    case "Trống":
                        point.Color = Color.FromArgb(34, 197, 94);
                        break;

                    case "Đang phục vụ":
                        point.Color = Color.FromArgb(59, 130, 246);
                        break;

                    case "Đã đặt":
                        point.Color = Color.FromArgb(245, 158, 11);
                        break;

                    default:
                        point.Color = Color.FromArgb(148, 163, 184);
                        break;
                }
            }
        }


        #endregion

        #region 5. GÁN SỰ KIỆN CLICK

        // Gán sự kiện click cho các khu vực truy cập nhanh và công việc.
        private void KhoiTaoSuKienClick()
        {
            GanClickChoToanBoControl(
                pnlDatBan,
                pnlDatBan_Click);

            GanClickChoToanBoControl(
                pnlGhoiMon,
                pnlGhoiMon_Click);

            GanClickChoToanBoControl(
                pnlHoaDon,
                pnlHoaDon_Click);

            GanClickChoToanBoControl(
                pnlThanhToan,
                pnlThanhToan_Click);

            GanClickChoToanBoControl(
                panel2,
                panelHoaDon_Click);

            GanClickChoToanBoControl(
                panel7,
                panelPhieuGoiMon_Click);

            GanClickChoToanBoControl(
                panel8,
                panelDatBan_Click);
        }

        // Gán sự kiện click đệ quy cho toàn bộ control con bên trong panel.
        private void GanClickChoToanBoControl(
            Control parent,
            EventHandler suKienClick)
        {
            if (parent == null)
                return;

            parent.Click -= suKienClick;
            parent.Click += suKienClick;

            parent.Cursor = Cursors.Hand;

            foreach (Control control in parent.Controls)
            {
                GanClickChoToanBoControl(
                    control,
                    suKienClick);
            }
        }

        #endregion

        #region 6. ĐIỀU HƯỚNG TRUY CẬP NHANH

        // Lấy Form trang chủ đang chứa UserControl Dashboard.
        private frmTrangChu LayFormTrangChu()
        {
            return this.FindForm() as frmTrangChu;
        }

        // Mở chức năng quản lý đặt bàn.
        private void pnlDatBan_Click(object sender, EventArgs e)
        {
            LayFormTrangChu()?.ClickQuanLyBan();
        }

        // Mở chức năng gọi món.
        private void pnlGhoiMon_Click(object sender, EventArgs e)
        {
            LayFormTrangChu()?.ClickGoiMon();
        }

        // Mở chức năng quản lý hóa đơn.
        private void pnlHoaDon_Click(object sender, EventArgs e)
        {
            LayFormTrangChu()?.ClickHoaDon();
        }

        // Mở chức năng quản lý thanh toán.
        private void pnlThanhToan_Click(object sender, EventArgs e)
        {
            LayFormTrangChu()?.ClickThanhToan();
        }

        #endregion

        #region 7. ĐIỀU HƯỚNG CÔNG VIỆC CẦN XỬ LÝ

        // Chuyển đến màn hình xử lý hóa đơn chưa thanh toán.
        private void panelHoaDon_Click(object sender, EventArgs e)
        {
            LayFormTrangChu()?.ClickHoaDon();
        }

        // Chuyển đến màn hình xử lý phiếu gọi món.
        private void panelPhieuGoiMon_Click(object sender, EventArgs e)
        {
            LayFormTrangChu()?.ClickGoiMon();
        }

        // Chuyển đến màn hình quản lý đặt bàn.
        private void panelDatBan_Click(object sender, EventArgs e)
        {
            LayFormTrangChu()?.ClickQuanLyBan();
        }

        #endregion
    }
}
