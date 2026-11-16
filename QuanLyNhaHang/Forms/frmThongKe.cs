
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using ClosedXML.Excel;
using QuanLyNhaHang.Models;
using QuanLyNhaHang.Services;

namespace QuanLyNhaHang
{
    public partial class frmThongKe : Form
    {
        private readonly ThongKeServiceMain thongKeService =
            new ThongKeServiceMain();

        // Dữ liệu thống kê lấy từ Service.
        private ThongKeTongQuan thongKeTongQuan;

        private List<ThongKeDoanhThuNgay> danhSachDoanhThuTheoNgay =
            new List<ThongKeDoanhThuNgay>();

        private List<ThongKeMonBanChay> danhSachTopMonBanChay =
            new List<ThongKeMonBanChay>();

        private List<ThongKeKhungGio> danhSachKhungGioDongKhach =
            new List<ThongKeKhungGio>();

        private bool dangKhoiTao = true;

        public frmThongKe()
        {
            InitializeComponent();

            this.Load += frmThongKe_Load;
            kdtpTheoThang.ValueChanged += kdtpTheoThang_ValueChanged;
        }

        // 1. KHỞI TẠO FORM
        private void frmThongKe_Load(object sender, EventArgs e)
        {
            dangKhoiTao = true;

            DateTime homNay = DateTime.Today;

            kdtpTheoThang.Value = homNay;

            kdtpTuNgay.Value = new DateTime(
                homNay.Year,
                homNay.Month,
                1);

            kdtpDenNgay.Value = new DateTime(
                homNay.Year,
                homNay.Month,
                DateTime.DaysInMonth(homNay.Year, homNay.Month));

            CauHinhBieuDo();
            CauHinhGiaoDienBang();
            dangKhoiTao = false;

            TaiDuLieuThongKe();
        }

        // 2. KHI THAY ĐỔI THÁNG
        private void kdtpTheoThang_ValueChanged(object sender, EventArgs e)
        {
            if (dangKhoiTao)
                return;

            DateTime thang = kdtpTheoThang.Value;

            kdtpTuNgay.Value = new DateTime(
                thang.Year,
                thang.Month,
                1);

            kdtpDenNgay.Value = new DateTime(
                thang.Year,
                thang.Month,
                DateTime.DaysInMonth(thang.Year, thang.Month));

            TaiDuLieuThongKe();
        }

        // 3. CẤU HÌNH BIỂU ĐỒ
        private void CauHinhBieuDo()
        {
            // Biểu đồ doanh thu.
            if (ChartDoanhThu.Series.IndexOf("DoanhThu") >= 0)
            {
                Series series = ChartDoanhThu.Series["DoanhThu"];

                series.Points.Clear();
                series.XValueType = ChartValueType.String;
                series.YValueType = ChartValueType.Double;

                ChartDoanhThu.ChartAreas[0].AxisX.LabelStyle.Enabled = true;
                ChartDoanhThu.ChartAreas[0].AxisX.Interval = 1;
                ChartDoanhThu.ChartAreas[0].AxisX.LabelStyle.Angle = -45;

                ChartDoanhThu.ChartAreas[0].AxisY.Minimum = 0;
                ChartDoanhThu.ChartAreas[0].AxisY.LabelStyle.Format = "N0";
            }

            // Biểu đồ khung giờ đông khách.
            if (chartKhungGioDongKhach.Series.IndexOf("SoLuotKhach") >= 0)
            {
                Series series =
                    chartKhungGioDongKhach.Series["SoLuotKhach"];

                series.Points.Clear();
                series.XValueType = ChartValueType.String;
                series.YValueType = ChartValueType.Int32;

                chartKhungGioDongKhach.ChartAreas[0]
                    .AxisX.LabelStyle.Enabled = true;

                chartKhungGioDongKhach.ChartAreas[0]
                    .AxisX.Interval = 1;

                chartKhungGioDongKhach.ChartAreas[0]
                    .AxisY.Minimum = 0;

                chartKhungGioDongKhach.ChartAreas[0]
                    .AxisY.Title = "Số hóa đơn";
            }
        }

        // 4. LẤY DỮ LIỆU THỐNG KÊ
        private void TaiDuLieuThongKe()
        {
            DateTime tuNgay = kdtpTuNgay.Value.Date;
            DateTime denNgay = kdtpDenNgay.Value.Date;

            if (tuNgay > denNgay)
            {
                MessageBox.Show(
                    "Ngày bắt đầu không được lớn hơn ngày kết thúc.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            try
            {
                // Lấy dữ liệu tổng quan.
                thongKeTongQuan =
                    thongKeService.GetThongKeTongQuan(tuNgay, denNgay);

                // Lấy doanh thu theo ngày.
                danhSachDoanhThuTheoNgay =
                    thongKeService.GetDoanhThuTheoNgay(tuNgay, denNgay);

                // Lấy Top 5 món bán chạy.
                danhSachTopMonBanChay =
                    thongKeService.GetTopMonBanChay(tuNgay, denNgay, 5);

                // Lấy thống kê khung giờ.
                danhSachKhungGioDongKhach =
                    thongKeService.GetThongKeKhungGio(tuNgay, denNgay);

                // Đảm bảo danh sách không bị null.
                if (danhSachDoanhThuTheoNgay == null)
                    danhSachDoanhThuTheoNgay =
                        new List<ThongKeDoanhThuNgay>();

                if (danhSachTopMonBanChay == null)
                    danhSachTopMonBanChay =
                        new List<ThongKeMonBanChay>();

                if (danhSachKhungGioDongKhach == null)
                    danhSachKhungGioDongKhach =
                        new List<ThongKeKhungGio>();

                // Hiển thị dữ liệu lên giao diện.
                HienThiTongQuan();
                HienThiBieuDoDoanhThu();
                HienThiTopMonBanChay();
                HienThiKhungGioDongKhach();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể tải dữ liệu thống kê.\nChi tiết: " + ex.Message,
                    "Lỗi thống kê",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // 5. HIỂN THỊ TỔNG QUAN
        private void HienThiTongQuan()
        {
            CultureInfo vi = new CultureInfo("vi-VN");

            if (thongKeTongQuan == null)
            {
                lblTongDoanhThu.Text = "0 đ";
                lblTongHoaDon.Text = "0";
                lblTrungBinhHoaDon.Text = "0 đ";
                return;
            }

            lblTongDoanhThu.Text =
                thongKeTongQuan.TongDoanhThu.ToString("N0", vi) + " đ";

            lblTongHoaDon.Text =
                thongKeTongQuan.SoHoaDon.ToString("N0", vi);

            lblTrungBinhHoaDon.Text =
                thongKeTongQuan.TrungBinhHoaDon.ToString("N0", vi) + " đ";
        }

        // 6. HIỂN THỊ BIỂU ĐỒ DOANH THU THEO NGÀY
        private void HienThiBieuDoDoanhThu()
        {
            Series series = ChartDoanhThu.Series["DoanhThu"];

            series.Points.Clear();

            foreach (ThongKeDoanhThuNgay item in danhSachDoanhThuTheoNgay)
            {
                string ngay = item.Ngay.ToString("dd/MM");

                decimal doanhThu = item.DoanhThu;

                int index = series.Points.AddXY(ngay, doanhThu);

                series.Points[index].ToolTip =
                    ngay + ": " + doanhThu.ToString("N0") + " đ";
            }
        }

        // 7. HIỂN THỊ TOP 5 MÓN BÁN CHẠY
        private void HienThiTopMonBanChay()
        {
            dgvTopMonBanChay.Rows.Clear();

            foreach (ThongKeMonBanChay item in danhSachTopMonBanChay)
            {
                int index = dgvTopMonBanChay.Rows.Add(
                    item.TenMon,
                    item.SoLuong,
                    item.DoanhThu.ToString("N0") + " đ");

                dgvTopMonBanChay.Rows[index].Tag = item;
            }
        }

        // 8. HIỂN THỊ BIỂU ĐỒ KHUNG GIỜ ĐÔNG KHÁCH
        private void HienThiKhungGioDongKhach()
        {
            Series series =
                chartKhungGioDongKhach.Series["SoLuotKhach"];

            series.Points.Clear();

            foreach (ThongKeKhungGio item in danhSachKhungGioDongKhach)
            {
                int index = series.Points.AddXY(
                    item.KhungGio,
                    item.SoHoaDon);

                series.Points[index].ToolTip =
                    item.KhungGio + ": " +
                    item.SoHoaDon + " hóa đơn";
            }
        }

        // 9. NÚT THỐNG KÊ
        private void btnThongKe_Click(object sender, EventArgs e)
        {
            TaiDuLieuThongKe();
        }

        // 10. NÚT XUẤT BÁO CÁO
        private void btnXuatBaoCao_Click(object sender, EventArgs e)
        {
            if (thongKeTongQuan == null)
            {
                MessageBox.Show(
                    "Chưa có dữ liệu để xuất báo cáo.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            using (SaveFileDialog saveDialog = new SaveFileDialog())
            {
                saveDialog.Title = "Xuất báo cáo thống kê Excel";
                saveDialog.Filter = "Excel Workbook (*.xlsx)|*.xlsx";
                saveDialog.DefaultExt = "xlsx";
                saveDialog.AddExtension = true;

                saveDialog.FileName =
                    "BaoCaoThongKe_" +
                    kdtpTuNgay.Value.ToString("ddMMyyyy") +
                    "_" +
                    kdtpDenNgay.Value.ToString("ddMMyyyy") +
                    ".xlsx";

                if (saveDialog.ShowDialog() != DialogResult.OK)
                    return;

                try
                {
                    XuatBaoCaoExcel(saveDialog.FileName);

                    MessageBox.Show(
                        "Xuất báo cáo Excel thành công!",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Không thể xuất báo cáo Excel.\nChi tiết: " + ex.Message,
                        "Lỗi xuất Excel",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }
        // 11. XUẤT BÁO CÁO EXCEL
        private void XuatBaoCaoExcel(string duongDan)
        {
            using (XLWorkbook workbook = new XLWorkbook())
            {
                IXLWorksheet ws = workbook.Worksheets.Add("Báo cáo thống kê");

                // CẤU HÌNH CHUNG
                ws.Style.Font.FontName = "Arial";
                ws.Style.Font.FontSize = 10;
                ws.Style.Font.FontColor = XLColor.FromHtml("#283746");

                ws.ShowGridLines = false;

                // MÀU SẮC
                XLColor xanhDam = XLColor.FromHtml("#00659A");
                XLColor xanhNhat = XLColor.FromHtml("#E1F4FC");
                XLColor xanhNhe = XLColor.FromHtml("#F0F9FD");
                XLColor trang = XLColor.White;
                XLColor chuDam = XLColor.FromHtml("#00416E");
                XLColor vien = XLColor.FromHtml("#D5E1E8");

                // ĐỊNH DẠNG CHUNG CHO BẢNG
                Action<int, int, int, int> keVien =
                    (dongDau, cotDau, dongCuoi, cotCuoi) =>
                    {
                        ws.Range(dongDau, cotDau, dongCuoi, cotCuoi)
                            .Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        ws.Range(dongDau, cotDau, dongCuoi, cotCuoi)
                            .Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                        ws.Range(dongDau, cotDau, dongCuoi, cotCuoi)
                            .Style.Border.OutsideBorderColor = vien;

                        ws.Range(dongDau, cotDau, dongCuoi, cotCuoi)
                            .Style.Border.InsideBorderColor = vien;
                    };

                // TIÊU ĐỀ BÁO CÁO
                ws.Range("A1:C1").Merge();

                ws.Cell("A1").Value = "BÁO CÁO THỐNG KÊ NHÀ HÀNG";

                ws.Range("A1:C1").Style.Font.Bold = true;
                ws.Range("A1:C1").Style.Font.FontSize = 16;
                ws.Range("A1:C1").Style.Font.FontColor = trang;
                ws.Range("A1:C1").Style.Fill.BackgroundColor = xanhDam;
                ws.Range("A1:C1").Style.Alignment.Horizontal =
                    XLAlignmentHorizontalValues.Center;
                ws.Range("A1:C1").Style.Alignment.Vertical =
                    XLAlignmentVerticalValues.Center;

                ws.Row(1).Height = 32;

                // KHOẢNG THỜI GIAN
                ws.Cell("A2").Value = "Từ ngày";
                ws.Cell("B2").Value = kdtpTuNgay.Value.Date;

                ws.Cell("A3").Value = "Đến ngày";
                ws.Cell("B3").Value = kdtpDenNgay.Value.Date;

                ws.Range("A2:A3").Style.Font.Bold = true;
                ws.Range("B2:B3").Style.NumberFormat.Format = "dd/MM/yyyy";

                // HÀM TẠO TIÊU ĐỀ TỪNG PHẦN
                Action<int, string> taoTieuDePhan = (soDong, noiDung) =>
                {
                    ws.Range(soDong, 1, soDong, 3).Merge();

                    ws.Cell(soDong, 1).Value = noiDung;

                    ws.Range(soDong, 1, soDong, 3).Style.Font.Bold = true;
                    ws.Range(soDong, 1, soDong, 3).Style.Font.FontColor = xanhDam;

                    ws.Range(soDong, 1, soDong, 3).Style.Fill.BackgroundColor = xanhNhat;

                    ws.Range(soDong, 1, soDong, 3).Style.Alignment.Vertical =
                        XLAlignmentVerticalValues.Center;

                    ws.Row(soDong).Height = 23;
                };

                // HÀM ĐỊNH DẠNG DÒNG TIÊU ĐỀ CỘT
                Action<int, int> taoTieuDeCot = (soDong, soCot) =>
                {
                    ws.Range(soDong, 1, soDong, soCot).Merge();

                    ws.Range(soDong, 1, soDong, soCot).Style.Font.Bold = true;

                    ws.Range(soDong, 1, soDong, soCot).Style.Font.FontColor = trang;

                    ws.Range(soDong, 1, soDong, soCot).Style.Fill.BackgroundColor = xanhDam;

                    ws.Range(soDong, 1, soDong, soCot).Style.Alignment.Horizontal =
                        XLAlignmentHorizontalValues.Center;

                    ws.Range(soDong, 1, soDong, soCot).Style.Alignment.Vertical =
                        XLAlignmentVerticalValues.Center;

                    ws.Row(soDong).Height = 23;
                };

                // PHẦN 1: TỔNG QUAN

                taoTieuDePhan(5, "TỔNG QUAN");

                ws.Cell("A6").Value = "Chỉ tiêu";
                ws.Cell("B6").Value = "Giá trị";

                taoTieuDeCot(6, 2);

                ws.Cell("A7").Value = "Tổng doanh thu";
                ws.Cell("B7").Value =
                    Convert.ToDouble(thongKeTongQuan.TongDoanhThu);

                ws.Cell("A8").Value = "Tổng hóa đơn";
                ws.Cell("B8").Value = thongKeTongQuan.SoHoaDon;

                ws.Cell("A9").Value = "Trung bình hóa đơn";
                ws.Cell("B9").Value =
                    Convert.ToDouble(thongKeTongQuan.TrungBinhHoaDon);

                ws.Range("B7:B7").Style.NumberFormat.Format = "#,##0 \"VND\"";
                ws.Range("B8:B8").Style.NumberFormat.Format = "#,##0";
                ws.Range("B9:B9").Style.NumberFormat.Format = "#,##0 \"VND\"";

                keVien(6, 1, 9, 2);

                // PHẦN 2: DOANH THU THEO NGÀY

                int dongDoanhThu = 12;

                taoTieuDePhan(dongDoanhThu, "DOANH THU THEO NGÀY");

                ws.Cell(dongDoanhThu + 1, 1).Value = "Ngày";
                ws.Cell(dongDoanhThu + 1, 2).Value = "Doanh thu";

                taoTieuDeCot(dongDoanhThu + 1, 2);

                int dong = dongDoanhThu + 2;

                foreach (ThongKeDoanhThuNgay item in danhSachDoanhThuTheoNgay)
                {
                    ws.Cell(dong, 1).Value = item.Ngay.Date;

                    ws.Cell(dong, 1).Style.NumberFormat.Format = "dd/MM/yyyy";

                    ws.Cell(dong, 2).Value =
                        Convert.ToDouble(item.DoanhThu);

                    ws.Cell(dong, 2).Style.NumberFormat.Format =
                        "#,##0 \"VND\"";

                    dong++;
                }

                keVien(
                    dongDoanhThu + 1,
                    1,
                    dong - 1,
                    2);

                // PHẦN 3: TOP 5 MÓN BÁN CHẠY

                int dongTopMon = dong + 2;

                taoTieuDePhan(dongTopMon, "TOP MÓN BÁN CHẠY");

                ws.Cell(dongTopMon + 1, 1).Value = "Tên món";
                ws.Cell(dongTopMon + 1, 2).Value = "Số lượng";
                ws.Cell(dongTopMon + 1, 3).Value = "Doanh thu";

                taoTieuDeCot(dongTopMon + 1, 3);

                dong = dongTopMon + 2;

                foreach (ThongKeMonBanChay item in danhSachTopMonBanChay)
                {
                    ws.Cell(dong, 1).Value = item.TenMon;
                    ws.Cell(dong, 2).Value = item.SoLuong;

                    ws.Cell(dong, 3).Value =
                        Convert.ToDouble(item.DoanhThu);

                    ws.Cell(dong, 3).Style.NumberFormat.Format =
                        "#,##0 \"VND\"";

                    dong++;
                }

                keVien(
                    dongTopMon + 1,
                    1,
                    dong - 1,
                    3);

                // PHẦN 4: THỐNG KÊ KHUNG GIỜ

                int dongKhungGio = dong + 2;

                taoTieuDePhan(dongKhungGio, "THỐNG KÊ KHUNG GIỜ ĐÔNG KHÁCH");

                ws.Cell(dongKhungGio + 1, 1).Value = "Khung giờ";
                ws.Cell(dongKhungGio + 1, 2).Value = "Số hóa đơn";

                taoTieuDeCot(dongKhungGio + 1, 2);

                dong = dongKhungGio + 2;

                foreach (ThongKeKhungGio item in danhSachKhungGioDongKhach)
                {
                    // Ghi dưới dạng chuỗi để Excel không chuyển thành ngày.
                    ws.Cell(dong, 1).Value = item.KhungGio;

                    ws.Cell(dong, 1).Style.NumberFormat.Format = "@";

                    ws.Cell(dong, 2).Value = item.SoHoaDon;

                    dong++;
                }

                keVien(
                    dongKhungGio + 1,
                    1,
                    dong - 1,
                    2);
                // ĐỊNH DẠNG TOÀN BỘ BÁO CÁO
                // Tự động điều chỉnh độ rộng cột.
                ws.Columns("A:C").AdjustToContents();

                // Thiết lập độ rộng tối thiểu.
                if (ws.Column(1).Width < 22)
                    ws.Column(1).Width = 22;

                if (ws.Column(2).Width < 18)
                    ws.Column(2).Width = 18;

                if (ws.Column(3).Width < 20)
                    ws.Column(3).Width = 20;

                // Căn giữa theo chiều dọc.
                ws.RangeUsed().Style.Alignment.Vertical =
                    XLAlignmentVerticalValues.Center;

                // Căn phải các giá trị tiền.
                ws.Range("B7:B7").Style.Alignment.Horizontal =
                    XLAlignmentHorizontalValues.Right;

                ws.Range("B9:B9").Style.Alignment.Horizontal =
                    XLAlignmentHorizontalValues.Right;

                // Cố định dòng tiêu đề.
                ws.SheetView.FreezeRows(1);

                // Cấu hình trang in.
                ws.PageSetup.PageOrientation = XLPageOrientation.Portrait;
                ws.PageSetup.FitToPages(1, 0);
                ws.PageSetup.CenterHorizontally = true;

                ws.PageSetup.Margins.Top = 0.5;
                ws.PageSetup.Margins.Bottom = 0.5;
                ws.PageSetup.Margins.Left = 0.3;
                ws.PageSetup.Margins.Right = 0.3;

                // Lưu file Excel.
                workbook.SaveAs(duongDan);
            }
        }


        // 12. XỬ LÝ GIÁ TRỊ CSV
        private string GhiCSV(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "\"\"";

            return "\"" + value.Replace("\"", "\"\"") + "\"";
        }
        // CẤU HÌNH GIAO DIỆN BẢNG TOP MÓN BÁN CHẠY
        private void CauHinhGiaoDienBang()
        {
            dgvTopMonBanChay.EnableHeadersVisualStyles = false;

            // Màu nền toàn bộ bảng.
            dgvTopMonBanChay.BackgroundColor = Color.White;
            dgvTopMonBanChay.GridColor = Color.LightGray;

            // Màu tiêu đề cột.
            dgvTopMonBanChay.ColumnHeadersDefaultCellStyle.BackColor =
                Color.FromArgb(225, 244, 252);

            dgvTopMonBanChay.ColumnHeadersDefaultCellStyle.ForeColor =
                Color.FromArgb(0, 65, 110);

            dgvTopMonBanChay.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 9F, FontStyle.Bold);

            // Màu dòng dữ liệu.
            dgvTopMonBanChay.DefaultCellStyle.BackColor = Color.White;
            dgvTopMonBanChay.DefaultCellStyle.ForeColor =
                Color.FromArgb(40, 55, 70);

            dgvTopMonBanChay.DefaultCellStyle.SelectionBackColor =
                Color.FromArgb(126, 211, 245);

            dgvTopMonBanChay.DefaultCellStyle.SelectionForeColor =
                Color.FromArgb(0, 65, 110);

            // Màu dòng xen kẽ.
            dgvTopMonBanChay.AlternatingRowsDefaultCellStyle.BackColor =
                Color.FromArgb(240, 249, 253);

            // Màu mặc định cho dòng mới.
            dgvTopMonBanChay.RowsDefaultCellStyle.BackColor = Color.White;

            // Chiều cao dòng.
            dgvTopMonBanChay.RowTemplate.Height = 25;

            // Không cho người dùng tự thêm dòng.
            dgvTopMonBanChay.AllowUserToAddRows = false;
        }
    }
}
